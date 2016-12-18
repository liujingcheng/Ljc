using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Ljc.DesktopApp
{
    /// <summary>
    /// TaskbarNotifier.xaml 的交互逻辑
    /// </summary>
    public partial class TaskbarNotifier : Window
    {
        DispatcherTimer timer;
        public double EndTop { get; set; }
        /// <summary>
        /// 是否重新启动蕃茄时间
        /// </summary>
        public bool RestartTomatoTimer = true;
        public TaskbarNotifier(string title, string content)
        {
            InitializeComponent();
            titleLb.Content = title;
            contentTxt.Text = content;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.EndTop = SystemParameters.WorkArea.Height - this.Height;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
        }

        public void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RestartTomatoTimer = true;
            HideWin();
        }

        public void HideWin()
        {
            this.IsEnabled = false;
            mGrid.OpacityMask = this.Resources["ClosedBrush"] as LinearGradientBrush;
            Storyboard std = this.Resources["ClosedStoryboard"] as Storyboard;
            std.Completed += delegate { this.Hide(); };
            std.Begin();
        }

        public void ChangeTip(string tip)
        {
            contentTxt.Text = tip;
        }

    }
}
