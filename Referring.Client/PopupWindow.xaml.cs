using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using EnglishHelper.Core;

namespace Referring.Client
{
    /// <summary>
    /// Логика взаимодействия для PopupWindow.xaml
    /// </summary>
    
    public interface IPopupWindow
    {
        string Title { get; set; }
        string Message { get; set; }
        void ShowMessage(string title, string message);
    }

    public partial class PopupWindow : Window
    {
        public PopupWindow()
        {
            InitializeComponent();

            HookEvents();
            MoveToCursorPosition();
        }

        private void HookEvents()
        {
            this.Activated += PopupWindow_Activated;
            this.Closing += PopupWindow_Closing;
            this.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.DragMove(); //allows drag popup
        }

        public new string Title
        {
            get { return title.Text; }
            set { title.Text = value; }
        }

        public string Message
        {
            get { return message.Text; }
            set { message.Text = value; }
        }

        public void ShowMessage(string title, string message)
        {
            Title = title;
            Message = message;
            this.Show();
        }

        public void CloseWindow()
        {
            PopupWindow_Closing(this, new System.ComponentModel.CancelEventArgs());
        }

        private void MoveToCursorPosition()
        {
            var position = MouseHelper.GetMousePosition();

            //Change coordinates to be more accurate and not to interfere user
            this.Left = position.X + 10;
            this.Top = position.Y - 120;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void PopupWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromMilliseconds(500));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void PopupWindow_Activated(object sender, EventArgs e)
        {
            StartCloseTimer();
            this.Focus();
        }

        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3d);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            PopupWindow_Closing(sender, new System.ComponentModel.CancelEventArgs());
        }
    }
}
