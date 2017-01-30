using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ljc.DesktopApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Ljc.DesktopApp.TaskbarNotifier MyTaskbarNotifier = new Ljc.DesktopApp.TaskbarNotifier("提示", "请记录时间！");
        public static DispatcherTimer Timer = new DispatcherTimer();
        /// <summary>
        /// 蕃茄时间25分钟
        /// </summary>
        private int _tomatoTimeSpan = 25;

        private DispatcherTimer _tomatoTimer = new DispatcherTimer();

        #region 初始化

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            Timer.Interval = TimeSpan.FromSeconds(3);
            Timer.Tick += new EventHandler(HideNotifWin);

            _tomatoTimer.Interval = TimeSpan.FromMinutes(_tomatoTimeSpan);
            _tomatoTimer.Tick += new EventHandler(NoticeTomatoTimeout);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            CheckIsAnyTaskGoing();
            RecurringTip();
            SpecificTimeTip();
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
                        Thread.Sleep(10000);

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
            string[] tips = { "谨慎编程，一朝不慎满盘皆输！", "先写出思路再动手！", "提高效率，完成计划！" };
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromHours(1)
            };
            timer.Tick += delegate
            {
                ShowTip(tips[i]);
                if (DateTime.Now.Hour >= 19)
                {
                    Thread.Sleep(60000);
                    ShowTip("不要急着赶进度，注重学习与成长！");
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
                if (DateTime.Now.Hour >= 23)
                {
                    ShowTip("要停止写代码了！");
                }
            };
            timer.Start();
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 隐藏提示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideNotifWin(object sender, System.EventArgs e)
        {
            MyTaskbarNotifier.HideWin();
            Timer.Stop();
        }

        /// <summary>
        /// 提示蕃茄时间到了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoticeTomatoTimeout(object sender, System.EventArgs e)
        {
            ShowTip("蕃茄时间到了！", false);
            _tomatoTimer.Stop();
        }

        /// <summary>
        /// 桌面右下角显示提示信息
        /// </summary>
        /// <param name="tip">提示信息</param>
        /// <param name="autoHide">提示是否自动消失</param>
        private void ShowTip(string tip, bool autoHide = true)
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                while (MyTaskbarNotifier.IsVisible)
                {
                    Thread.Sleep(5000);
                }
                MyTaskbarNotifier.ChangeTip(tip);
                MyTaskbarNotifier.Show();
                if (autoHide)
                {
                    Timer.Start();
                }
            });
        }

        #endregion
    }
}
