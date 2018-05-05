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
        public int ViewHeight = 500;
        public int topLineLimit = 3, bottomLineLimit = 3, scrollLineLimit = 5;
        public bool isPause;
        public Grid container = new Grid();
        public MediaPlayer mediaPlayer = new MediaPlayer();
        public MediaTimelineController mediaTimelineController = new MediaTimelineController();
        public List<Danmaku> danmakus;
        public List<Storyboard> storyBoards = new List<Storyboard>();
        //public DanmakuManager danmakuManager = new DanmakuManager();

        public class DanmakuPosition
        {
            public int row;
            public DateTime Date;
            public DanmakuPosition(int _row,DateTime _date)
            {
                row = _row;
                Date = _date;
            }
        }

        public List<DanmakuPosition> topY = new List<DanmakuPosition> (), botY = new List<DanmakuPosition>(), scrY = new List<DanmakuPosition>();
        #region getDanmakuYposition
        public int GetY(Danmaku d, string mode)
        {
            int ret = 0;
            if(mode == "scroll"){
                ret = 0;
                if (scrY != null)
                {
                    for (int i = scrY.Count - 1; i >= 0; i--)
                    {
                        if (d.Date >= scrY[i].Date)
                        {
                            int r = (scrY[i].row + 1) % scrollLineLimit;
                            scrY.Add(new DanmakuPosition(r, d.Date));
                            scrY = scrY.OrderBy(s => s.Date).ToList();
                            ret = r * lineHeight ; break;
                        }
                    }
                }
                else
                {
                    scrY.Add(new DanmakuPosition(0, d.Date));
                }
            }
            else if(mode == "top")
            {
                ret = 0;
                if (topY != null)
                {
                    for (int  i = topY.Count-1;i>=0;i--)
                    {
                        if (d.Date >= topY[i].Date)
                        {
                            int r = (topY[i].row + 1) % topLineLimit;
                            topY.Add(new DanmakuPosition(r, d.Date));
                            topY = topY.OrderBy(s => s.Date).ToList();
                            ret = r * lineHeight; break;
                        }
                    }
                }
                else
                {
                    topY.Add(new DanmakuPosition(0, d.Date));
                }
            }
            else
            {
                ret = ViewHeight - lineHeight;
                if (botY != null)
                {
                    for (int i = botY.Count - 1; i >= 0; i--)
                    {
                        if (d.Date >= botY[i].Date)
                        {
                            int r = (botY[i].row + 1) % bottomLineLimit;
                            botY.Add(new DanmakuPosition(r, d.Date));
                            botY = botY.OrderBy(s => s.Date).ToList();
                            ret = ViewHeight - r * lineHeight- lineHeight; break;
                        }
                    }
                }
                else
                {
                    botY.Add(new DanmakuPosition(0, d.Date));
                }
            }
            Debug.WriteLine("getY===" + ret+"view Height = "+ViewHeight);
            return ret;
        }
        #endregion
        public DanmakuPlayer(DateTime begin,DateTime end,string channel_id)
        {
            #region init
            isPause = false;
            if (topY != null) topY.Clear();
            topY.Add(new DanmakuPosition(0, new DateTime(1998, 07, 15)));
            if (botY != null) botY.Clear();
            botY.Add(new DanmakuPosition(0, new DateTime(1998, 07, 15)));
            if (scrY != null) scrY.Clear();
            scrY.Add(new DanmakuPosition(0, new DateTime(1998, 07, 15)));

            if (this.danmakus != null)
                this.danmakus.Clear();
            if (this.container != null)
                this.container.Children.Clear();
            GetDanmaku( begin, end, channel_id);
            #endregion

        }
        private async void GetDanmaku(DateTime begin, DateTime end, string channel_id)
        {
            try
            {
                this.danmakus = await DanmakuManager.Instance.QueryDanmaku(channel_id, begin, end);

            }
            catch
            {

            }
        }
        public DanmakuPlayer()
        {
            #region init
            isPause = false;
            if(topY != null) topY.Clear();
            topY.Add(new DanmakuPosition(0, new DateTime(1998, 07, 15)));
            if(botY != null) botY.Clear();
            botY.Add(new DanmakuPosition(0, new DateTime(1998, 07, 15)));
            if(scrY != null) scrY.Clear();
            scrY.Add(new DanmakuPosition(0, new DateTime(1998, 07, 15)));

            if (this.danmakus!= null)
                this.danmakus.Clear();
            if(this.container != null)
                this.container.Children.Clear();
            //this.danmakus = danmakuManager.GetInitDanmaku();
            #endregion
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
        public void updateDanmaku(TimeSpan currentP)
        {
            if (danmakus != null)
            {
                foreach (var item in danmakus)
                {
                    Debug.WriteLine("items:" + (int)item.Offset.TotalSeconds + " currentps:" + (int)currentP.TotalSeconds);
                    Debug.WriteLine("items danmaku type:" + item.Mode);
                    if ((int)item.Offset.TotalSeconds == (int)currentP.TotalSeconds)
                    {
                        Debug.WriteLine("begin add danmaku"+item.Offset.TotalSeconds);
                        Debug.WriteLine("items danmaku type:" + item.Mode);
                        if (item.Mode == "scroll")
                        {
                            this.AddScrollDanmaku(item, true);
                        }
                        else if (item.Mode == "top")
                        {
                            this.AddTopDanmakuAsync(item, true);
                        }
                        else
                        {
                            this.AddBottomDanmaku(item, true);
                        }
                    }
                }
            }
            
        }
        public void AddScrollDanmaku(Danmaku danmaku,bool isNewDanmaku)
        {
            double xx, yy;
            xx = 400; yy = GetY(danmaku,danmaku.Mode) ;
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
                sb.BeginTime = TimeSpan.FromSeconds(danmaku.Offset.TotalSeconds);
            }
            storyBoards.Add(sb);

            if (danmakus!=null&&!danmakus.Contains(danmaku)) danmakus.Add(danmaku);

            sb.Begin();
            Debug.WriteLine(danmaku.Text);
        }
        public async void AddTopDanmakuAsync(Danmaku danmaku,bool isNewDanmaku)
        {
            int xx, yy= GetY(danmaku, danmaku.Mode);
            Debug.WriteLine("getYY=" + yy);
            int offset;
            if (isNewDanmaku)
            {
                offset = 0;
            }
            else
            {
                offset = (int)danmaku.Offset.TotalSeconds;
            }
            Grid danmakuGrid = new Grid
            {
                Margin = new Thickness(0, yy, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
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
            double xx, yy = GetY(danmaku, danmaku.Mode);
            int offset;
            if (isNewDanmaku)
            {
                offset = 0;
            }
            else
            {
                offset = (int)danmaku.Offset.TotalSeconds;// danmaku.offset;
            }
            Grid danmakuGrid = new Grid
            {
                Margin = new Thickness(0, yy, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                
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
