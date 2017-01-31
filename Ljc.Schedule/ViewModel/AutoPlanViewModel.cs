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
using Microsoft.Win32;
using NPOI.OpenXml4Net.OPC;

namespace Ljc.Schedule.ViewModel
{
    public class AutoPlanViewModel
    {
        private IList<TaskModel> _taskModels;
        private string _sourceFileName;
        /// <summary>
        /// 排计划时是否排除周末两天
        /// </summary>
        private bool _isExcludeWeekend = true;

        /// <summary>
        /// 要包含的日期集合
        /// </summary>
        private List<DateTime> _includeDays;
        /// <summary>
        /// 要排除的日期集合
        /// </summary>
        private List<DateTime> _excludeDays;

        public AutoPlanViewModel()
        {
            XmlParse(out _includeDays, out _excludeDays);
            bool.TryParse(ConfigurationManager.AppSettings["exclude-weekend"], out _isExcludeWeekend);
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
                           CalSchedule(_taskModels);
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
        /// 自动排期（单任务支持的工作量范围：0.5-7）
        /// </summary>
        /// <param name="list"></param>
        private void CalSchedule(IList<TaskModel> list)
        {
            var members = new List<string>();
            var memberStrList = list.Select(p => p.TaskMember).Distinct().ToList();
            foreach (var memberStr in memberStrList)
            {
                if (memberStr.Contains("、"))
                {
                    var subMembers = memberStr.Split('、').ToList();
                    foreach (var subMember in subMembers)
                    {
                        if (!members.Contains(subMember))
                        {
                            members.Add(subMember);
                        }
                    }
                }
                else if (!members.Contains(memberStr))
                {
                    members.Add(memberStr);
                }
            }

            foreach (var taskModel in list)
            {
                taskModel.PlanEndTime = null;
            }
            foreach (var member in members)
            {
                var subList = list.Where(p => p.TaskMember.Contains(member)).ToList();
                if (subList.Count == 0 || subList.First().PlanStartTime == null)
                {
                    continue;
                }

                string timeFormat = "yyyy/MM/dd";
                //string timeFormat = "yyyy/MM/dd HH:mm:ss";
                string dateFormat = "yyyy/MM/dd";
                var firstStartTime = DateTime.Parse(subList.First().PlanStartTime);
                DateTime lastEndTime = firstStartTime;
                foreach (var taskModel in subList)
                {
                    taskModel.HolidayRemark = null;//先清空

                    var startTime = lastEndTime;
                    taskModel.PlanStartTime = startTime.ToString(timeFormat);
                    var spentDays = double.Parse(taskModel.PlanSpentDays);


                    if (startTime.AddDays(spentDays).Hour != 0 && startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2016/12/08"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2016/12/09"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2016/12/10"
                            || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2016/12/11")
                    //搬家2天+周末2天
                    {
                        spentDays += 4;
                        taskModel.HolidayRemark = "搬家2天+周末2天";
                    }
                    else if (startTime.AddDays(spentDays).Hour != 0 && startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2016/12/31"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/01"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/02")
                    //元旦放假3天
                    {
                        spentDays += 3;
                        taskModel.HolidayRemark = "元旦放假3天";
                    }
                    else if (startTime.AddDays(spentDays).Hour != 0 && startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2016/01/21"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/22"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/23"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/24"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/25"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/26"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/27"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/28"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/29"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/30"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/31"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/02/01"
                          || startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/02/02")
                    //春节假期
                    {
                        spentDays += 13;
                        taskModel.HolidayRemark = "春节假期";
                    }
                    else if (HasCrossedWeekend(startTime, spentDays))//TODO:暂时只支持一个周末的跨跃，多周末待以后支持（或者约束工作量不能超过一周也行）
                    //周末两天休假
                    {
                        spentDays += 2;
                        taskModel.HolidayRemark = "周末两天休假";
                    }

                    var endTime = startTime.AddDays(spentDays);

                    //.Hour == 0代表它是零点。如果endTime正好是周六零点，其实它也就是周五结束，所以从日期上看的话要退后一天才符合常识理解
                    var endDateStr = endTime.Hour == 0 ? endTime.AddDays(-1).ToString(timeFormat) : endTime.ToString(timeFormat);
                    if (taskModel.PlanEndTime == null)
                    {
                        taskModel.PlanEndTime = endDateStr;
                    }
                    else if (taskModel.PlanEndTime != endDateStr)
                    //多人任务但排的计划时间不一致时，分别显示
                    {
                        taskModel.PlanEndTime += "\n" + member + ":" + endDateStr;
                    }

                    if (startTime.AddDays(spentDays).Hour == 0 && startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2016/12/08")
                    //搬家2天+周末2天
                    {
                        lastEndTime = endTime.AddDays(4);
                        taskModel.HolidayRemark = endDateStr + "之后是搬家2天+周末2天";
                    }
                    else if (startTime.AddDays(spentDays).Hour == 0 && startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2016/12/31")
                    //元旦放假3天
                    {
                        lastEndTime = endTime.AddDays(3);
                        taskModel.HolidayRemark = endDateStr + "之后是元旦3天假期";
                    }
                    else if (startTime.AddDays(spentDays).Hour == 0 && startTime.AddDays(spentDays).Date.ToString(dateFormat) == "2017/01/21")
                    //春节假期
                    {
                        lastEndTime = endTime.AddDays(13);
                        taskModel.HolidayRemark = endDateStr + "春节假期";
                    }
                    else if (endTime.Hour == 0 && endTime.DayOfWeek == DayOfWeek.Saturday)//.Hour == 0代表它是零点。如果endTime正好是周六零点，其实它也就是周五结束，且下个任务应从下周一开始
                    //周末两天休假
                    {
                        lastEndTime = endTime.AddDays(2);
                        taskModel.HolidayRemark = endDateStr + "之后是周末2天假期";
                    }
                    else
                    {
                        lastEndTime = endTime;
                    }
                }
            }

        }

        /// <summary>
        /// 是否有跨跃了周末(目前只考虑到跨一个周末的情况）
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
        /// 读取自定义日期配置文件
        /// </summary>
        private void XmlParse(out List<DateTime> includeDays, out List<DateTime> excludeDays)
        {
            includeDays = new List<DateTime>();
            excludeDays = new List<DateTime>();
            string rootPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//获取或设置包含该应用程序的目录的名称。
            var xmlParse = new XmlParse(Path.Combine(rootPath, @"CustomDate.xml"));
            var customdate = xmlParse.Document.Element("customdate");
            if (customdate != null)
            {
                var includeDate = customdate.Element("includedate");
                if (includeDate != null)
                {
                    foreach (var date in includeDate.Elements("date"))
                    {
                        try
                        {
                            var startDateStr = date.Attribute("startdate").Value;
                            var startDate = Convert.ToDateTime(startDateStr);
                            var endDateStr = date.Attribute("enddate").Value;
                            var endDate = Convert.ToDateTime(endDateStr);
                            _includeDays.AddRange(GetAllDatesBetween(startDate, endDate));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    _includeDays = _includeDays.Distinct().ToList();
                }

                var excludeDate = customdate.Element("excludedate");
                if (excludeDate != null)
                {
                    foreach (var date in excludeDate.Elements("date"))
                    {
                        try
                        {
                            var startDateStr = date.Attribute("startdate").Value;
                            var startDate = Convert.ToDateTime(startDateStr);
                            var endDateStr = date.Attribute("enddate").Value;
                            var endDate = Convert.ToDateTime(endDateStr);
                            _excludeDays.AddRange(GetAllDatesBetween(startDate, endDate));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    _excludeDays = _excludeDays.Distinct().ToList();
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

    }
}
