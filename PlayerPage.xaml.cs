using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace NetEasePlayer_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PlayerPage : Page
    {
        public PlayerPage()
        {
            this.InitializeComponent();
        }
         protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Live live = (Live)e.Parameter;
            Debug.WriteLine(Live.GetLiveName(live.GetUrlList()[0]));
            Play(live.Url);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            video_player.MediaPlayer.Source = null;
        }
        private void Play(String url)
        {
            video_player.Source = MediaSource.CreateFromUri( new Uri(url) );
            video_player.Visibility = Visibility.Visible;
        }

    }
}
