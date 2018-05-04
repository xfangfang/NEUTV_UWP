using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace NetEasePlayer_UWP.Models
{
    class DanmakuPlayer
    {
        public int lineHeight = 25;
        public bool isPause;
        public Grid container = new Grid();
        public MediaPlayer mediaPlayer = new MediaPlayer();
        public MediaTimelineController mediaTimelineController = new MediaTimelineController();
        public List<Danmaku> danmakus;
        public List<Storyboard> storyBoards = new List<Storyboard>();
        public DanmakuManager danmakuManager = new DanmakuManager();

        public bool[] isOccupied, isTopOccupied, isBottomOccupied;
        public DanmakuPlayer()
        {
            isPause = false;
            if(this.danmakus!= null)
                this.danmakus.Clear();
            if(this.container != null)
                this.container.Children.Clear();
            //this.danmakus = danmakuManager.GetInitDanmaku();
            if (danmakus != null)
            {
                foreach (Danmaku item in danmakus)
                {

                    if (item.Mode == "scroll")
                    {
                        this.AddScrollDanmaku(item, false);
                    }
                    else if (item.Mode == "top")
                    {
                        this.AddTopDanmakuAsync(item, false);
                    }
                    else
                    {
                        this.AddBottomDanmaku(item, false);
                    }
                }
            }
        }
        #region Add Danmaku
        public void AddScrollDanmaku(Danmaku danmaku,bool isNewDanmaku)
        {
            double xx, yy;
            xx = 400; yy = 200;
            Grid danmakuGrid = new Grid
            {
                Margin = new Thickness(0, yy, 0, 0)
            };
            TextBlock tb = new TextBlock
            {
                Text = danmaku.Text,
                FontSize = 24,
                Foreground = new SolidColorBrush(Colors.White),
            };
 
            danmakuGrid.Children.Add(tb);
            TranslateTransform c = new TranslateTransform();
            danmakuGrid.RenderTransform = c;

            Storyboard sb = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 4*xx,
                To = -xx,
                Duration = new Duration(TimeSpan.FromSeconds(5))
            };
            
            animation.Completed += ((sender, e) => {
                if (container.Children.Contains(danmakuGrid))
                {
                    container.Children.Remove(danmakuGrid);
                }
            });
            
            Storyboard.SetTarget(animation, danmakuGrid);

            Storyboard.SetTargetName(animation, "danmakuGrid");
            Storyboard.SetTargetProperty(animation, "(danmakuGrid.RenderTransform).(TranslateTransform.X)");
            sb.Children.Add(animation);
            container.Children.Add(danmakuGrid);
            if (isNewDanmaku)
            {
                sb.BeginTime = TimeSpan.FromSeconds(0);
            }
            else
            {
                //sb.BeginTime = TimeSpan.FromSeconds(danmaku.offset);
                sb.BeginTime = TimeSpan.FromSeconds(0);
            }
            storyBoards.Add(sb);

            if (danmakus!=null&&!danmakus.Contains(danmaku)) danmakus.Add(danmaku);

            sb.Begin();
            Debug.WriteLine(danmaku.Text);
        }
        public async void AddTopDanmakuAsync(Danmaku danmaku,bool isNewDanmaku)
        {
            double xx, yy=20;
            int offset;
            if (isNewDanmaku)
            {
                offset = 0;
            }
            else
            {
                // offset = danmaku.offset;
                offset = 0;
            }
            Grid danmakuGrid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };
            TextBlock tb = new TextBlock
            {
                Text = danmaku.Text,
                FontSize = 24,
                Foreground = new SolidColorBrush(Colors.White)
            };
            await Task.Delay(offset*1000);
            if (isPause)
            {
                while (isPause)
                {
                    await Task.Delay(50);
                }
            }
            danmakuGrid.Children.Add(tb);
            container.Children.Add(danmakuGrid);
            await Task.Delay(5000);
            if (isPause)
            {
                while (isPause)
                {
                    await Task.Delay(5000);
                }
            }
            if (container.Children.Contains(danmakuGrid))
            {
                container.Children.Remove(danmakuGrid);
            }

        }
        public async void AddBottomDanmaku(Danmaku danmaku,bool isNewDanmaku)
        {
            double xx, yy = 350;
            int offset;
            if (isNewDanmaku)
            {
                offset = 0;
            }
            else
            {
                offset = 0;// danmaku.offset;
            }
            Grid danmakuGrid = new Grid
            {
                Margin = new Thickness(0, yy, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            TextBlock tb = new TextBlock
            {
                Text = danmaku.Text,
                FontSize = 24,
                Foreground = new SolidColorBrush(Colors.White)
            };
            await Task.Delay(offset * 1000);
            if (isPause)
            {
                while (isPause)
                {
                    await Task.Delay(50);
                }
            }
            danmakuGrid.Children.Add(tb);
            container.Children.Add(danmakuGrid);
            await Task.Delay(5000);
            if (isPause)
            {
                while (isPause)
                {
                    await Task.Delay(5000);
                }
            }
            if (container.Children.Contains(danmakuGrid))
            {
                container.Children.Remove(danmakuGrid);
            }
        }
        #endregion
        public void Start()
        {
            foreach (var item in storyBoards)
            {
                item.Begin();
            }
        }
        public void Play()
        {
            isPause = false;
            foreach (var item in storyBoards)
            {
                item.Resume();
            }
        }
        public void Pause()
        {
            isPause = true;
            foreach (var item in storyBoards)
            {
                item.Pause();
            }
        }
    }
}
