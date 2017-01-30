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
        }

        private void HideNotifWin(object sender, System.EventArgs e)
        {
            MyTaskbarNotifier.HideWin();
            Timer.Stop();
        }

        private void NoticeTomatoTimeout(object sender, System.EventArgs e)
        {
            ShowTip("蕃茄时间到了！", false);
            _tomatoTimer.Stop();
        }

        Thread threadPlay = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //threadPlay = new Thread(new ThreadStart(playsound));
            //threadPlay.Start();
            string title = titleTxt.Text;
            string content = contentTxt.Text;
            Ljc.DesktopApp.TaskbarNotifier taskbarnotifier = new Ljc.DesktopApp.TaskbarNotifier(title, content);
            taskbarnotifier.Show();
        }
        private void playsound()
        {
            string soundName = "msg.wav";
            //PlaySound.Play(soundName);
            threadPlay.Abort();
        }



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
        /// 桌面右下角显示提示信息
        /// </summary>
        /// <param name="tip">提示信息</param>
        /// <param name="autoHide">提示是否自动消失</param>
        private void ShowTip(string tip, bool autoHide = true)
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                MyTaskbarNotifier.ChangeTip(tip);
                MyTaskbarNotifier.Show();
                if (autoHide)
                {
                    Timer.Start();
                }
            });
        }
    }
}
