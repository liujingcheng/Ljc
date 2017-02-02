using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using CanYouLib.ExcelLib;
using CanYouLib.ExcelLib.Utility;
using GalaSoft.MvvmLight.Command;
using Ljc.Common;
using Ljc.Schedule.Models;
using Ljc.Schedule.Tool;
using Microsoft.Win32;

namespace Ljc.Schedule.ViewModel
{
    public class AutoPlanViewModel
    {
        private IList<TaskModel> _taskModels;
        private string _sourceFileName;

        /// <summary>
        /// 获取综合了所有用户自定义的配置之后最终要被排除的所有日期段
        /// </summary>
        /// <param name="firstStartTime">计划编排的起始日期</param>
        /// <returns></returns>
        private List<CoherentDateRange> GetAllExcludedDateRanges(DateTime firstStartTime)
        {
            bool excludeWeekend;// 排计划时是否排除周末两天
            List<DateTime> includeDays;//要包含的日期集合
            List<DateTime> excludeDays;// 要排除的日期集合
            List<CoherentDateRange> customIncludeDateRanges;// 用户自定义要被包含的日期段
            List<CoherentDateRange> customExcludeDateRanges;// 用户自定义要被排除的日期段

            CustomDateXmlParse(out includeDays, out excludeDays, out excludeWeekend, out customIncludeDateRanges, out customExcludeDateRanges);

            //如果指定了要排除周末,那没被包含的周末要加入到被排除列表里
            if (excludeWeekend)
            {
                var weekends = GetAllWeekends(firstStartTime, 365);//默认取未来一年内的所有周末
                foreach (var date in weekends)
                {
                    if (!excludeDays.Contains(date))
                    {
                        excludeDays.Add(date);
                    }
                }
            }

            //用户指定要包含的日期不能排除
            foreach (var includeDay in includeDays)
            {
                if (excludeDays.Contains(includeDay))
                {
                    excludeDays.Remove(includeDay);
                }
            }

            var excludedDateRanges = GetAllCoherentDateRanges(excludeDays);

            //将用户自定义要被排除的日期段的备注合并到最终要被排除的日期段
            foreach (var excludedDateRange in excludedDateRanges)
            {
                foreach (var customExcludeDateRange in customExcludeDateRanges)
                {
                    if (customExcludeDateRange.StartDate >= excludedDateRange.StartDate
                        && customExcludeDateRange.EndDate <= excludedDateRange.EndDate)
                    {
                        excludedDateRange.Remark += customExcludeDateRange.Remark;
                    }
                }
            }

            return excludedDateRanges;
        }

        /// <summary>
        /// 导入计划Excel
        /// </summary>
        public RelayCommand ImportPlanCommand
        {
            get
            {
                return new RelayCommand(() =>
               {
                   try
                   {
                       var importExcel = new ImportExcel();
                       var dialog =
                           new Microsoft.Win32.OpenFileDialog { Filter = "excel|*.xls;*.xlsx" };
                       if (dialog.ShowDialog() == true)
                       {
                           _sourceFileName = dialog.FileName;
                           var ds = importExcel.ImportDataSet(_sourceFileName, false, 1, 0);
                           var dt = ds.Tables[0];
                           _taskModels = ModelConverter<TaskModel>.ConvertToModel(dt);
                           if (_taskModels.Count == 0)
                           {
                               return;
                           }
                           DateTime firstStartTime;
                           if (!DateTime.TryParse(_taskModels.First().PlanStartTime, out firstStartTime))
                           {
                               MessageBox.Show("首记录起始日期缺失或格式不正确，请检查!");
                               return;
                           }

                           var excludedDateRanges = GetAllExcludedDateRanges(firstStartTime);
                           CalSchedule(_taskModels, excludedDateRanges);
                       }
                   }
                   catch (Exception)
                   {
                       MessageBox.Show("1. 请确认Excel没有被其它进程占用；2. 请确认它是2003版本的xls");
                   }
               });
            }
        }

        /// <summary>
        /// 导出计划Excel
        /// </summary>
        public RelayCommand ExportPlanCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var sheetName = "Sheet1";
                    var exportExcel = new ExportExcel();
                    string rootPath = AppDomain.CurrentDomain.BaseDirectory;
                    var newFileNamePrifix = Path.GetFileNameWithoutExtension(_sourceFileName).TrimEnd('1', '2', '3', '4', '5', '6', '7', '8', '9', '0');
                    var fileId = Path.GetFileNameWithoutExtension(newFileNamePrifix) + DateTime.Now.ToString("yyyyMMddHHmmss");//避免文件重复
                    var tempFilePath = rootPath + "\\" + fileId + ".xls";//存放临时文件的路径
                    exportExcel.CreateExcel(sheetName, 1);
                    var docuSum = new DocumentSummary()
                    {
                        ApplicationName = "AutoSchedule",
                        Author = "ljc",
                        //FirstRow = 0
                    };

                    var fileData = exportExcel.ExportData(_taskModels, sheetName, rootPath, docuSum);
                    var file = new FileStream(tempFilePath, FileMode.Create);
                    file.Write(fileData, 0, fileData.Length - 1);
                    file.Close();

                    #region 执行导出逻辑
                    using (ExcelExpert report = new ExcelExpert(tempFilePath))
                    {
                        SaveFileDialog dialog = new SaveFileDialog
                        {
                            Filter = "文档|*.xls;*.xlsx",
                            FileName = fileId,
                            RestoreDirectory = true
                        };
                        if (dialog.ShowDialog() == true)
                        {
                            report.Save(dialog.FileName);

                            if (MessageBox.Show("下载成功！是否打开文件？") == MessageBoxResult.OK)
                            {
                                System.Diagnostics.Process.Start(dialog.FileName);
                            }
                            File.Delete(tempFilePath);//删除临时文件
                        }
                        else
                        {
                            File.Delete(tempFilePath);//删除临时文件
                        }
                    }
                    #endregion
                });
            }
        }

        /// <summary>
        /// 自动排期（单任务支持的工作量最小粒度：0.5人/天）
        /// </summary>
        /// <param name="list">任务列表</param>
        /// <param name="excludedDateRanges">综合了所有用户自定义的配置之后最终要被排除的所有日期段</param>
        private void CalSchedule(IList<TaskModel> list, List<CoherentDateRange> excludedDateRanges)
        {
            var members = list.Select(p => p.TaskMember).Distinct().ToList();

            foreach (var taskModel in list)
            {
                taskModel.PlanEndTime = null;
            }
            foreach (var member in members)
            {
                var subList = list.Where(p => p.TaskMember.Contains(member)).ToList();
                DateTime firstStartTime;
                if (!DateTime.TryParse(subList.First().PlanStartTime, out firstStartTime))
                {
                    MessageBox.Show(member + "的起始日期缺失或格式不正确，请检查!");
                    continue;
                }

                string timeFormat = "yyyy/MM/dd";
                //string timeFormat = "yyyy/MM/dd HH:mm:ss";
                DateTime lastEndTime = firstStartTime;
                foreach (var taskModel in subList)
                {
                    taskModel.HolidayRemark = null;//先清空

                    var startTime = lastEndTime;
                    taskModel.PlanStartTime = startTime.ToString(timeFormat);
                    double spentDays;
                    if (!double.TryParse(taskModel.PlanSpentDays, out spentDays))
                    {
                        MessageBox.Show("有些任务的工作量缺失或格式不正确，请检查！");
                        return;
                    }

                    var crossedDateRanges = GetPossibleCrossCoherentDateRanges(startTime, spentDays, excludedDateRanges);
                    CoherentDateRange excludedDateRange = null;
                    if (crossedDateRanges.Count > 0)
                    //将跨跃的假期合并,方便后面endTime的计算
                    {
                        excludedDateRange = new CoherentDateRange(crossedDateRanges.First().StartDate,
                            crossedDateRanges.Last().EndDate,
                            crossedDateRanges.Aggregate(string.Empty,
                                (current, range) => current + range.Remark));
                        spentDays -= excludedDateRange.TotalDays - crossedDateRanges.Sum(p => p.TotalDays);
                    }

                    if (excludedDateRange != null)
                    {
                        spentDays += excludedDateRange.TotalDays;
                        taskModel.HolidayRemark += excludedDateRange.Remark;
                    }

                    var endTime = startTime.AddDays(spentDays);
                    //.Hour == 0代表它是零点。如果endTime正好是周六零点，其实它也就是周五结束，所以从日期上看的话要退后一天才符合常识理解（其它假期也一样）
                    var endDateStr = endTime.Hour == 0 ? endTime.AddDays(-1).ToString(timeFormat) : endTime.ToString(timeFormat);
                    if (taskModel.PlanEndTime == null)
                    {
                        taskModel.PlanEndTime = endDateStr;
                    }

                    var nextExcludedDateRange = excludedDateRanges.FirstOrDefault(p => p.StartDate == endTime.Date);//如果endTime的下一天就是假期,则取出这个假期
                    if (nextExcludedDateRange != null && endTime.Hour == 0)
                    //.Hour == 0代表它是零点。如果endTime正好是周六零点，其实它也就是周五结束，且下个任务应从下周一开始
                    {
                        lastEndTime = endTime.AddDays(nextExcludedDateRange.TotalDays);
                        taskModel.HolidayRemark += endDateStr + "之后是" + nextExcludedDateRange.Remark;
                    }
                    else
                    {
                        lastEndTime = endTime;
                    }
                }
            }

        }

        /// <summary>
        /// 找出所有可能跨跃的连续日期段
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="spentDays">所需工作量</param>
        /// <param name="dateRanges">所有连续日期段</param>
        /// <returns></returns>
        private List<CoherentDateRange> GetPossibleCrossCoherentDateRanges(DateTime startTime, double spentDays,
            List<CoherentDateRange> dateRanges)
        {
            var list = new List<CoherentDateRange>();
            for (int i = 0; i < dateRanges.Count; i++)
            {
                if (HasCrossedDateRange(startTime, spentDays, dateRanges[i]))
                {
                    list.Add(dateRanges[i]);
                    spentDays += dateRanges[i].TotalDays;
                }
            }

            return list;
            //return dateRanges.Where(p => startTime <= p.EndDate && startTime.AddDays(spentDays) >= p.StartDate).ToList();
        }

        /// <summary>
        /// 中间是否经过了一个日期段
        /// </summary>
        /// <returns></returns>
        private bool HasCrossedDateRange(DateTime startTime, double spentDays, CoherentDateRange dateRange)
        {
            var gapDays = 0.5;
            while (gapDays <= spentDays)
            {
                var addedStart = startTime.AddDays(gapDays);
                if (addedStart.Hour != 0 && addedStart.Date == dateRange.StartDate.Date//.Hour != 0代表其是从中午开始的情况（工作量是半天的）,如果.Hour==0代表它刚好是前一天晚上完成的,所以不算跨跃
                               || addedStart.Date > dateRange.StartDate.Date && addedStart.Date <= dateRange.EndDate.Date)//要把StartDate排除
                {
                    return true;
                }
                gapDays += 0.5;
            }
            return false;
        }

        /// <summary>
        /// 中间是否经过了一个周末(目前只考虑到跨一个周末的情况）
        /// </summary>
        /// <returns></returns>
        private bool HasCrossedWeekend(DateTime startTime, double spentDays)
        {
            var gapDays = 0.5;
            while (gapDays <= spentDays)
            {
                var addedStart = startTime.AddDays(gapDays);
                if (addedStart.Hour != 0 && addedStart.DayOfWeek == DayOfWeek.Saturday//.Hour != 0代表其是从中午开始的情况（工作量是半天的）
                               || addedStart.DayOfWeek == DayOfWeek.Sunday)
                {
                    return true;
                }
                gapDays += 0.5;
            }
            return false;
        }

        /// <summary>
        /// 解析自定义日期配置文件
        /// </summary>
        /// <param name="includeDays">用户自定义要被包含的所有日期</param>
        /// <param name="excludeDays">用户自定义要被排除的所有日期</param>
        /// <param name="excludeWeekend">是否排除周末(默认排除)</param>
        /// <param name="customIncludeDateRanges">用户自定义要被包含的日期段</param>
        /// <param name="customExcludeDateRanges">用户自定义要被排除的日期段</param>
        private void CustomDateXmlParse(out List<DateTime> includeDays, out List<DateTime> excludeDays, out bool excludeWeekend,
            out List<CoherentDateRange> customIncludeDateRanges, out List<CoherentDateRange> customExcludeDateRanges)
        {
            includeDays = new List<DateTime>();
            excludeDays = new List<DateTime>();
            excludeWeekend = true;
            customIncludeDateRanges = new List<CoherentDateRange>();
            customExcludeDateRanges = new List<CoherentDateRange>();

            string rootPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//获取或设置包含该应用程序的目录的名称。
            var xmlParse = new XmlParse(Path.Combine(rootPath, @"CustomDate.xml"));
            var customdate = xmlParse.Document.Element("customdate");
            if (customdate != null)
            {
                var excludeWeekendNode = customdate.Element("exclude-weekend");
                if (excludeWeekendNode != null)
                {
                    var excludeWeekendAttr = excludeWeekendNode.Attribute("value");
                    if (excludeWeekendAttr != null)
                    {
                        bool.TryParse(excludeWeekendAttr.Value, out excludeWeekend);
                    }
                }

                var includeDate = customdate.Element("include-date");
                if (includeDate != null)
                {
                    foreach (var date in includeDate.Elements("date"))
                    {
                        try
                        {
                            var startDateStr = date.Attribute("start-date").Value;
                            var startDate = Convert.ToDateTime(startDateStr).Date;
                            var endDateStr = date.Attribute("end-date").Value;
                            var endDate = Convert.ToDateTime(endDateStr).Date;
                            includeDays.AddRange(GetAllDatesBetween(startDate, endDate));

                            string remark = null;
                            var remarkAttr = date.Attribute("remark");
                            if (remarkAttr != null)
                            {
                                remark = remarkAttr.Value;
                            }
                            customIncludeDateRanges.Add(new CoherentDateRange(startDate, endDate, remark));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    includeDays = includeDays.Distinct().ToList();
                    customIncludeDateRanges = customIncludeDateRanges.Distinct(new CoherentDateRangeComparer()).ToList();
                }

                var excludeDate = customdate.Element("exclude-date");
                if (excludeDate != null)
                {
                    foreach (var date in excludeDate.Elements("date"))
                    {
                        try
                        {
                            var startDateStr = date.Attribute("start-date").Value;
                            var startDate = Convert.ToDateTime(startDateStr).Date;
                            var endDateStr = date.Attribute("end-date").Value;
                            var endDate = Convert.ToDateTime(endDateStr).Date;
                            excludeDays.AddRange(GetAllDatesBetween(startDate, endDate));

                            string remark = null;
                            var remarkAttr = date.Attribute("remark");
                            if (remarkAttr != null)
                            {
                                remark = remarkAttr.Value;
                            }
                            customExcludeDateRanges.Add(new CoherentDateRange(startDate, endDate, remark));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    excludeDays = excludeDays.Distinct().ToList();
                    customExcludeDateRanges = customExcludeDateRanges.Distinct(new CoherentDateRangeComparer()).ToList();
                }
            }
        }

        /// <summary>
        /// 获取两个日期间所有日期(含本身)
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        private List<DateTime> GetAllDatesBetween(DateTime date1, DateTime date2)
        {
            var list = new List<DateTime>();

            var minDate = date1;
            var maxDate = date2;
            if (date1 > date2)
            {
                minDate = date2;
                maxDate = date1;
            }

            list.Add(minDate);
            while (minDate < maxDate)
            {
                minDate = minDate.AddDays(1);
                list.Add(minDate);
            }

            return list;
        }

        /// <summary>
        /// 给定的所有日期中取所有连续的日期范围
        /// </summary>
        /// <param name="rawDates">给定的所有日期</param>
        /// <returns></returns>
        private List<CoherentDateRange> GetAllCoherentDateRanges(List<DateTime> rawDates)
        {
            var list = new List<CoherentDateRange>();
            if (rawDates == null || rawDates.Count == 0)
            {
                return list;
            }
            if (rawDates.Count == 1)
            {
                list.Add(new CoherentDateRange(rawDates[0], rawDates[0]));
                return list;
            }

            var orderedDates = rawDates.OrderBy(p => p).Select(q => new DateTime(q.Year, q.Month, q.Day)).ToList();//复制一份,不影响原有列表
            if (orderedDates.Count == 2)
            //只有两个直接返回
            {
                list.Add(new CoherentDateRange(orderedDates[0], orderedDates[1]));
                return list;
            }

            for (int i = 0, j = 0; i < orderedDates.Count - 2;)
            {
                if ((orderedDates[i + 1] - orderedDates[i]).Days > 1)
                {
                    var isAfterNextDaySingle = (orderedDates[i + 2] - orderedDates[i + 1]).Days > 1;//下下一天是否是个孤立日
                    list.Add(i > j
                        ? new CoherentDateRange(orderedDates[j], orderedDates[i])
                        : new CoherentDateRange(orderedDates[j], orderedDates[i + 1]));//i==j
                    if (isAfterNextDaySingle)
                    {
                        list.Add(new CoherentDateRange(orderedDates[i + 1], orderedDates[i + 1]));
                    }

                    if (i > j)
                    {
                        if (isAfterNextDaySingle)
                        {
                            i += 2;
                        }
                        else
                        {
                            i += 1;
                        }
                    }
                    else
                    //i==j
                    {
                        if (isAfterNextDaySingle)
                        {
                            i += 3;
                        }
                        else
                        {
                            i += 2;
                        }
                    }
                    j = i;
                }
                else
                {
                    i++;
                }
            }

            return list;
        }

        /// <summary>
        /// 从startDate起取未来futureDays内所有的周末日期
        /// </summary>
        /// <param name="startDate">起始日期</param>
        /// <param name="futureDays">未来多少天内</param>
        /// <returns></returns>
        private List<DateTime> GetAllWeekends(DateTime? startDate, int futureDays)
        {
            var list = new List<DateTime>();

            DateTime day = startDate ?? DateTime.Now;
            for (int i = 0; i < futureDays; i++)
            {
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    list.Add(day);
                }
                day = day.AddDays(1);
            }

            return list;
        }

    }
}
