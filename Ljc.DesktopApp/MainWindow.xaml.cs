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

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            Timer.Interval = TimeSpan.FromSeconds(3);
            Timer.Tick += new EventHandler(HideNotifWin);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
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
                            TaskbarNotifier taskbarnotifier1 = new TaskbarNotifier("提示", "发生异常！" + exception.Message);
                            taskbarnotifier1.Show();
                            continue;
                        }

                        if (checkResult != "true")
                        {
                            this.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                MyTaskbarNotifier.Show();
                                Timer.Start();
                            });
                        }
                        Thread.Sleep(60000);

                    }
                    catch (Exception ex)
                    {
                        TaskbarNotifier taskbarnotifier1 = new Ljc.DesktopApp.TaskbarNotifier("提示", "发生异常！" + ex.Message);
                        taskbarnotifier1.Show();
                    }
                }
            });
        }

        private void HideNotifWin(object sender, System.EventArgs e)
        {
            MyTaskbarNotifier.Image_MouseLeftButtonDown(null, null);
            Timer.Stop();
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
    }
}
