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
        public CustomMediaTransportControls()
        {
            this.DefaultStyleKey = typeof(CustomMediaTransportControls);
        }
        protected override void OnApplyTemplate()
        {
            // This is where you would get your custom button and create an event handler for its click method.
            Button myFullButton = GetTemplateChild("myFullWindowButton") as Button;
            myFullButton.Click += MyFullButton_Click;
            base.OnApplyTemplate();
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
