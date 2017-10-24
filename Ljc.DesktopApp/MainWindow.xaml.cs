using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Timer = System.Threading.Timer;

namespace Ljc.DesktopApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 消息提示框
        /// </summary>
        public TaskbarNotifier MyTaskbarNotifier;
        /// <summary>
        /// 隐藏提示框定时器
        /// </summary>
        public System.Timers.Timer HideTimer;
        /// <summary>
        /// 番茄 时间定时器
        /// </summary>
        private DispatcherTimer _tomatoTimer;
        /// <summary>
        /// 番茄 时间(分钟)
        /// </summary>
        private int _tomatoTimeSpan;
        /// <summary>
        /// 消息提示框持续时间(秒)
        /// </summary>
        private int _hideTimerSpan;
        /// <summary>
        /// 检查是否有任务在进行的时间间隔(秒)
        /// </summary>
        private int _checkTaskSpan;
        /// <summary>
        /// 循环提示语间隔时间(分钟)
        /// </summary>
        private int _recurringTipSpan;
        /// <summary>
        /// 非工作时间开始提示小时数(比如晚上8点后,那就是20)
        /// </summary>
        private int _nonWorkHourTipStartHour;
        /// <summary>
        /// 停止写代码的开始提示小时数(比如晚上11点后,那就是23)
        /// </summary>
        private int _stopCodingStartHour;

        #region 初始化

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Init();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            CheckIsAnyTaskGoing();
            RecurringTip();
            SpecificTimeTip();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
#if DEBUG
            _tomatoTimeSpan = 1;
            _hideTimerSpan = 4;
            _checkTaskSpan = 30;
            _recurringTipSpan = 1;
            _nonWorkHourTipStartHour = 12;
            _stopCodingStartHour = 12;
#else

            _tomatoTimeSpan = 25;
            _hideTimerSpan = 4;
            _checkTaskSpan = 30;
            _recurringTipSpan = 40;
            _nonWorkHourTipStartHour = 19;
            _stopCodingStartHour = 23;
#endif

            MyTaskbarNotifier = new TaskbarNotifier("提示", "请记录时间！");
            HideTimer = new System.Timers.Timer(_hideTimerSpan * 1000)
            {
                AutoReset = false,
                Enabled = true
            };
            HideTimer.Elapsed += delegate
            {
                MyTaskbarNotifier.AutoHideWinLater();
            };

            _tomatoTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(_tomatoTimeSpan) };
            _tomatoTimer.Tick += NoticeTomatoTimeout;
        }

        #endregion

        #region 主方法

        /// <summary>
        /// 检查是否有任务在进行,没有要提示.若番茄时间到了要提示.
        /// </summary>
        private void CheckIsAnyTaskGoing()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        var apiUrl = ConfigurationManager.AppSettings["api_url"];

                        var checkResult = string.Empty;
                        var hasException = false;
                        var autoResetEvent = new AutoResetEvent(false);
                        var httpClient = new HttpClient();
                        Exception exception = null;

                        httpClient.GetAsync($"{apiUrl}/time/anygoing").ContinueWith(requestTask =>
                        {
                            try
                            {
                                HttpResponseMessage response = requestTask.Result;
                                response.EnsureSuccessStatusCode();
                                response.Content.ReadAsStringAsync()
                                    .ContinueWith(readTask =>
                                    {
                                        checkResult = readTask.Result;
                                        autoResetEvent.Set();
                                    });
                            }
                            catch (Exception ex)
                            {
                                hasException = true;
                                exception = ex;
                                autoResetEvent.Set();
                            }
                        });

                        autoResetEvent.WaitOne();

                        if (hasException)//child thread has exception
                        {
                            //throw exception;
                            //continue;
                        }

                        bool anygoing;
                        if (bool.TryParse(checkResult, out anygoing))
                        {
                            if (!anygoing)
                            {
                                ShowTip("请记录时间！");
                            }
                            else
                            {
                                if (MyTaskbarNotifier.RestartTomatoTimer)
                                {
                                    _tomatoTimer.Start();
                                    MyTaskbarNotifier.RestartTomatoTimer = false;
                                }
                            }
                        }
                        Thread.Sleep(_checkTaskSpan * 1000);

                    }
                    catch (Exception ex)
                    {
                        //throw ex;
                    }
                }
            });
        }

        /// <summary>
        /// 循环提示
        /// </summary>
        private void RecurringTip()
        {
            int i = 0;
            string[] tips = { "谨慎编程，一朝不慎满盘皆输！", "先写出思路再动手！", "专注才能提高效率！", "奋斗赢得尊重！", "重在效率——单位时间的产出！",
                "不要用执行上的勤奋，掩盖思考上的懒惰！","人但有恒，事无不成！" };
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(_recurringTipSpan)
            };
            timer.Tick += delegate
            {
                ShowTip(tips[i], false);
                if (DateTime.Now.Hour >= _nonWorkHourTipStartHour)
                {
                    ShowTip("不要急着赶进度，注重学习与成长！", false);
                }
                i++;
                if (i == tips.Length)
                {
                    i = 0;
                }
            };
            timer.Start();
        }

        /// <summary>
        /// 定点提示
        /// </summary>
        private void SpecificTimeTip()
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            timer.Tick += delegate
            {
                if (DateTime.Now.Hour >= _stopCodingStartHour)
                {
                    ShowTip("要停止写代码了！");
                }
            };
            timer.Start();
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 提示番茄 时间到了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoticeTomatoTimeout(object sender, System.EventArgs e)
        {
            ShowTip("专注！番茄钟结束，过下今日待办吧！", false, false);
            _tomatoTimer.Stop();
        }

        /// <summary>
        /// 桌面右下角显示提示信息
        /// </summary>
        /// <param name="tip">提示信息</param>
        /// <param name="discardThisTip">如果还有提示未消失,是否要舍弃本次提示</param>
        /// <param name="autoHide">提示是否自动消失</param>
        private void ShowTip(string tip, bool discardThisTip = true, bool autoHide = true)
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                if (tip.Contains("番茄") || !MyTaskbarNotifier.IsVisible)
                {
                    MyTaskbarNotifier.ChangeTip(tip);
                    MyTaskbarNotifier.Show();
                    if (autoHide)
                    {
                        HideTimer.Start();
                    }
                }
                else
                //如果还有提示未关闭
                {
                    if (discardThisTip)
                    //如果要舍弃就直接返回
                    {
                        return;
                    }
                    //另开定时器等待30秒后再执行
                    var aTimer = new System.Timers.Timer(30000)
                    {
                        AutoReset = false,
                        Enabled = true
                    };
                    aTimer.Elapsed += delegate
                    {
                        ShowTip(tip, false, autoHide);
                    };
                }
            });
        }
        #endregion
    }
}
