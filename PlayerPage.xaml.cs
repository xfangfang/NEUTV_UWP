using NetEasePlayer_UWP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;

using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace NetEasePlayer_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 

    public sealed partial class PlayerPage : Page
    {
        Dictionary<String, List<One>> listMap = new Dictionary<string, List<One>>();
        Live live;
        One playing;
        bool isLive = true;
        TappedEventHandler listTapHandler;
        RightTappedEventHandler listRightTapHandler;
        static SemaphoreSlim _sem = new SemaphoreSlim(3);

        DanmakuPlayer danmakuPlayer;
        TimeSpan nowPosition = new TimeSpan(0);
        public PlayerPage()
        {
            this.InitializeComponent();

            video_player.MediaPlayer.PlaybackSession.PlaybackStateChanged += myPlaybackSession_PlaybackStateChanged;
            video_player.MediaPlayer.PlaybackSession.PositionChanged += myPlaybackSession_PositionChangedAsync;
        }
        // 能实时获取player position，可换另一种方式实现弹幕添加
        private async void myPlaybackSession_PositionChangedAsync(MediaPlaybackSession sender, object args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                // Your UI update code goes here!
                var currentPosion = video_player.MediaPlayer.PlaybackSession.Position;
                currentPosion = (TimeSpan)currentPosion;
                if(((int)currentPosion.TotalSeconds - (int)nowPosition.TotalSeconds) == 1)
                {
                    nowPosition = currentPosion;
                    danmakuPlayer.updateDanmaku(currentPosion);
                }
                else if((int)System.Math.Abs(((int)currentPosion.TotalSeconds - (int)nowPosition.TotalSeconds) )>= 3) {
                    danmakuPlayer.storyBoards.Clear();
                    danmakuPlayer.updateDanmaku(currentPosion);
                }
                //Debug.WriteLine((int)currentPosion.TotalSeconds);
               // Debug.WriteLine((int)nowPosition.TotalSeconds);
            });
        }
        
        //弹幕的播放和暂停
        private async void myPlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            //必须使用Dispatcher切换到主UI线程
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                // Your UI update code goes here!
                Debug.WriteLine("enter state changed");
                var currentState = video_player.MediaPlayer.PlaybackSession.PlaybackState;
                if (currentState == MediaPlaybackState.Playing)
                {
                    danmakuPlayer.Play();
                }
                else if (currentState == MediaPlaybackState.Paused)
                {
                    danmakuPlayer.Pause();
                }
            });
        }

        #region NavigatePlayList
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            live = (Live)e.Parameter;
            isLive = true;
            Play(live.Url);
            InitListTapEvent();

            GetList(new Uri(String.Format("https://hdtv.neu6.edu.cn/{0}.review",
                live.GetSimpleName())));
        }
        private void InitListTapEvent()
        {
            if(listTapHandler == null)
            {
                listTapHandler = new TappedEventHandler((sender, e) =>
                {
                    var o = ((One)((TextBlock)((ListBox)sender).SelectedItem).Tag);

                    playing = o;isLive = false;

                    Play("http://media2.neu6.edu.cn/review/program-"
                            + o.start.ToString() + "-"
                            + o.end.ToString() + "-" + live.GetSimpleName() + ".m3u8");
                });
                see_back_list.Tapped += listTapHandler;
            }
            
        }


       
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            video_player.MediaPlayer.Source = null;
            select_date_combobox.Items.Clear();
            select_date_combobox.SelectedValuePath = "";
        }
        private void Play(String url)
        {
            Debug.WriteLine(url);
            nowPosition = new TimeSpan(0);
            Debug.WriteLine("init now time=" + (int)nowPosition.TotalSeconds);
            if (isLive)
                danmakuPlayer = new DanmakuPlayer();
            else
            {
                Debug.WriteLine("new playback player");
                danmakuPlayer = new DanmakuPlayer(Converter.Instance.TimeConverter(playing.start),
                                                  Converter.Instance.TimeConverter(playing.end), live.Name);
            }
                
            if (container.Children != null)
                container.Children.Clear();
            container.Children.Add(danmakuPlayer.container);

            video_player.MediaPlayer.Source = MediaSource.CreateFromUri( new Uri(url) );

            video_player.Visibility = Visibility.Visible;
            danmakuPlayer.Start();    

        }
        /*
        private async void MediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            //_duration = sender.Duration.GetValueOrDefault();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {

                timeLine.Minimum = 0;
                timeLine.Maximum = _duration.TotalSeconds;
                timeLine.StepFrequency = 1;

                Debug.WriteLine("Dispatcher");
            });
        }*/
        public async Task GetList(Uri uri)
        {

            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync(uri);
                var buffer = await response.Content.ReadAsBufferAsync();
                //转为UTF-8格式
                DataReader reader = DataReader.FromBuffer(buffer);
                byte[] fileContent = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(fileContent);
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                var str = encoding.GetString(fileContent);
                listMap.Clear();
                //解析json
                JsonObject jsonObject = JsonObject.Parse(str);
                foreach(String date in jsonObject.Keys)
                {
                    JsonArray lives = jsonObject[date].GetArray();
                    List<One> day = new List<One>();
                    foreach(JsonValue one in lives)
                    {
                        
                        var ones = one.GetArray();
                        var o1 = ones.ElementAt(0);
                        long start = Convert.ToInt64(ones.GetStringAt(0));
                        long end = -1;
                        try
                        {
                            end = Convert.ToInt64(ones.GetStringAt(1));

                        }
                        catch
                        {
                            end = Convert.ToInt64(ones.GetNumberAt(1));
                        }
                        String name = ones.GetStringAt(2);
                        day.Add(new One(start,end,name));
                        
                    }
                    listMap.Add(date, day);
                }
                //向页面添加
                var dates = new List<String>(listMap.Keys);
                dates.Sort();
                dates.Reverse();
                String defultComboboxTitle="";
                if(dates.Count() != 0)
                {
                    defultComboboxTitle = dates[0];
                }
                select_date_combobox.Items.Clear();
                foreach (var i in dates)
                {
                    select_date_combobox.Items.Add(i);
                }

                select_date_combobox.SelectionChanged += new SelectionChangedEventHandler((sender,e)=> {
                    see_back_list.Items.Clear();
                    if (select_date_combobox.SelectedItem != null)
                    {
                        foreach (var i in listMap[select_date_combobox.SelectedItem.ToString()])
                        {
                            var t = new TextBlock
                            {
                                Text = i.name,
                                Tag = i
                            };
                            see_back_list.Items.Add(t);
                        }
                    }
                });
                select_date_combobox.SelectedIndex = 0;
               // select_date_combobox.SelectedValuePath = defultComboboxTitle;

                /*
                       "2018-04-05":[

                       [
                 　　　　　　"1522858200",
                 　　　　　　"1522859220",
                 　　　　　　"欢乐中国人Ⅱ"
                　　　　],
                　　　　[
                　　　　　　"1522859220",
                　　　　　　"1522859700",
                　　　　　　"魅力中国城"
                　　　　],

                    */

            }
            catch (Exception e)
            {
                Debug.WriteLine("xxxxx{0}", e);
            }
            finally
            {
                //mainpage_progress_ring.IsActive = false;
            }
            
        }
        #endregion

        #region chooseDanmakuMode
        /// <summary>
        /// 选择弹幕模式
        /// 实现方式太暴力辣
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Option1CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Option1CheckBox.IsChecked = true;
            Option2CheckBox.IsChecked = false;
            Option3CheckBox.IsChecked = false;
        }

        private void Option2CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Option2CheckBox.IsChecked = true;
            Option1CheckBox.IsChecked = false;
            Option3CheckBox.IsChecked = false;
        }

        private void Option3CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Option3CheckBox.IsChecked = true;
            Option2CheckBox.IsChecked = false;
            Option1CheckBox.IsChecked = false;
        }

        private void Option1CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Option2CheckBox.IsChecked == false && Option3CheckBox.IsChecked == false)
                Option1CheckBox.IsChecked = true;
            else
                Option1CheckBox.IsChecked = false;
        }

        private void Option2CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Option1CheckBox.IsChecked == false && Option3CheckBox.IsChecked == false)
                Option2CheckBox.IsChecked = true;
            else
                Option2CheckBox.IsChecked = false;
        }

        private void Option3CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Option2CheckBox.IsChecked == false && Option1CheckBox.IsChecked == false)
                Option3CheckBox.IsChecked = true;
            else
                Option3CheckBox.IsChecked = false;
        }
        #endregion

        /// <summary>
        /// 发射弹幕按钮按下时，根据弹幕模式添加对应弹幕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void send_Click(object sender, RoutedEventArgs e)
        {
            string mode ,danmakuText;
            TimeSpan offset;
            if (Option1CheckBox.IsChecked == true) mode = "top";
            else if (Option2CheckBox.IsChecked == true) mode = "scroll";
            else   mode = "bottom";

            danmakuText = danmakuInput.Text.ToString();
            danmakuInput.Text = "";

            DateTime now = System.DateTime.Now; DateTime date;
            if (isLive)
                date = now;
            else
                date = Converter.Instance.TimeConverter(playing.start) + video_player.MediaPlayer.PlaybackSession.Position;

             //offset 需要修改
            if (isLive)
                offset = new TimeSpan(0);
            else
                offset = video_player.MediaPlayer.TimelineControllerPositionOffset;

            Danmaku danmaku = new Danmaku
            {
                Text = danmakuText,
                Mode = mode,
                Date = date,
                Channel_id = live.Name,
                Offset = offset
            };
            Debug.WriteLine("post danmaku");
            DanmakuManager.Instance.AddDanmakuAsync(danmaku);

            Debug.WriteLine("post sc!");
            if ( mode == "scroll")
            {
                danmakuPlayer.AddScrollDanmaku(danmaku,true);
            }
            else if(mode == "top")
            {
                danmakuPlayer.AddTopDanmakuAsync(danmaku,true);
            }
            else
            {
                danmakuPlayer.AddBottomDanmaku(danmaku,true);
            }
        }
        /// <summary>
        /// 全屏/退出全屏时调整MediaPlayerElement控件位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customMTC_Fulled(object sender, EventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            bool isFull = view.IsFullScreenMode;
            if (!isFull)
            {
                danmakuPlayer.ViewHeight = 500;
                tool.Visibility = Visibility.Visible;
                video_player.SetValue(Grid.RowProperty, 0);
                video_player.SetValue(Grid.RowSpanProperty, 2);

                player.SetValue(Grid.ColumnProperty, 1);
                player.SetValue(Grid.ColumnSpanProperty, 3);

                select_date_combobox.Visibility = Visibility.Visible;
                see_back_list.Visibility = Visibility.Visible;
            }
            else
            {
                danmakuPlayer.ViewHeight = 725;
                video_player.SetValue(Grid.RowProperty, 0);
                video_player.SetValue(Grid.RowSpanProperty, 3);
                tool.Visibility = Visibility.Collapsed;

                player.SetValue(Grid.RowProperty, 0);
                player.SetValue(Grid.ColumnProperty, 0);
                player.SetValue(Grid.ColumnSpanProperty, 4);

                select_date_combobox.Visibility = Visibility.Collapsed;
                see_back_list.Visibility = Visibility.Collapsed;
            }
            

        }
        /// <summary>
        /// xfang 下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customMTC_Downloaded(object sender, EventArgs e)
        {
            Debug.WriteLine("click Download");
            DownManager.DownloadShowAsync(playing, live,new down(downProgress));
        }
        /// <summary>
        /// xfang 直播
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customMTC_Lived(object sender, EventArgs e)
        {
            Debug.WriteLine("click goLive");

            Play(live.Url);
            
        }

        private void customMTC_DanmakuOpened(object sender, EventArgs e)
        {
            if(container.Visibility == Visibility.Collapsed){
                container.Visibility = Visibility.Visible;
            }
            else
            {
                container.Visibility = Visibility.Collapsed;
            }
        }
    }
    class down : IDown
    {
        TextBlock text;
        public down(TextBlock t)
        {
            this.text = t;
        }
        static int index = 0;
        static int len = 0;
        public void Start(int len)
        {
            down.len = len;
            down.index = 0;
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.High,
                new DispatchedHandler(() =>
                {
                    this.text.Text = 0+"/"+len;
                    DownManager.ShowDialog("开始下载");
                }));
                
        }
        public void Process()
        {
            down.index++;
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
               CoreDispatcherPriority.High,
               new DispatchedHandler(() =>
               {
                   this.text.Text = String.Format("{0}/{1}", down.index, down.len);
               }));
        }
        public void End()
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
            CoreDispatcherPriority.High,
            new DispatchedHandler(() => {
                DownManager.ShowDialog("下载完毕");
                this.text.Text = "";
            })
);
        }
    }
}
