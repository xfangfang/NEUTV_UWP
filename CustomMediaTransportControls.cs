using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace NetEasePlayer_UWP
{
    public sealed class CustomMediaTransportControls : MediaTransportControls
    {
        public event EventHandler<EventArgs> Fulled;
        public event EventHandler<EventArgs> Downloaded;
        public event EventHandler<EventArgs> Lived;
        public event EventHandler<EventArgs> DanmakuOpened;
        public CustomMediaTransportControls()
        {
            this.DefaultStyleKey = typeof(CustomMediaTransportControls);
        }
        protected override void OnApplyTemplate()
        {
            // This is where you would get your custom button and create an event handler for its click method.
            Button myFullButton = GetTemplateChild("myFullWindowButton") as Button;
            Button myDownloadButton = GetTemplateChild("myDownloadButton") as Button;
            Button myLiveButton = GetTemplateChild("myLiveButton") as Button;
            AppBarToggleButton myDanmakuButton = GetTemplateChild("myDanmakuButton") as AppBarToggleButton;
            myFullButton.Click += MyFullButton_Click;
            myDownloadButton.Click += MyDownloadButton_Click;
            myLiveButton.Click += MyLiveButton_Click;
            myDanmakuButton.Click += MyDanmakuButton_Click;
            base.OnApplyTemplate();
        }

        private void MyDanmakuButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DanmakuOpened?.Invoke(this, EventArgs.Empty);
        }

        private void MyLiveButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Lived?.Invoke(this, EventArgs.Empty);
        }

        private void MyDownloadButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Downloaded?.Invoke(this, EventArgs.Empty);
        }

        private void MyFullButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            bool isFull = view.IsFullScreenMode;
            if (!isFull)
            {
                view.TryEnterFullScreenMode();
                view.FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Standard;
            }
            else
            {
                view.ExitFullScreenMode();
            }
            Fulled?.Invoke(this, EventArgs.Empty);
        }
    }
}
