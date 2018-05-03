using NetEasePlayer_UWP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
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
        TappedEventHandler listTapHandler;

        MediaPlayer mediaPlayer = new MediaPlayer();
        MediaTimelineController mediaTimelineController = new MediaTimelineController();
        DanmakuPlayer danmakuPlayer;
        
        public PlayerPage()
        {
            this.InitializeComponent();

            //
           
        }
        
         protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            live = (Live)e.Parameter;
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
            danmakuPlayer = new DanmakuPlayer(video_player.MediaPlayer.TimelineController);
            container.Children.Add(danmakuPlayer.container);

            Debug.WriteLine(player.Width + " player " + player.Height);
            Debug.WriteLine(tool.Width + " tool " + player.Height);
            video_player.MediaPlayer.Source = MediaSource.CreateFromUri( new Uri(url) );
            //mediaPlayer.TimelineController = mediaTimelineController;
            //var mediaSource = MediaSource.CreateFromUri(new Uri(url));
            // mediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
            //video_player.SetMediaPlayer(mediaPlayer);
            //mediaTimelineController.Start();
            danmakuPlayer.Start();    
            video_player.Visibility = Visibility.Visible;
            
            danmakuPlayer.addTest("init");
        }
        private async void MediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            //_duration = sender.Duration.GetValueOrDefault();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                /*
                timeLine.Minimum = 0;
                timeLine.Maximum = _duration.TotalSeconds;
                timeLine.StepFrequency = 1;
                */
            });
        }
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

        private void send_Click(object sender, RoutedEventArgs e)
        {
            string mode ,danmakuText;
            int offset;
            if (Option1CheckBox.IsChecked == true) mode = "top";
            else if (Option2CheckBox.IsChecked == true) mode = "scroll";
            else   mode = "bottom";

            danmakuText = danmakuInput.Text.ToString();
            danmakuInput.Text = "";
            offset = video_player.MediaPlayer.TimelineControllerPositionOffset.Seconds;
            //Debug.WriteLine(video_player.MediaPlayer.TimelineController.State.ToString());
            Debug.WriteLine("offset = " + offset.ToString());
            Debug.WriteLine(video_player.MediaPlayer.PlaybackSession.PlaybackState.ToString());
            Debug.WriteLine(video_player.MediaPlayer.PlaybackSession.Position.Seconds);
            offset = 3;
            danmakuPlayer.addTest(danmakuText);
            Danmaku danmaku = new Danmaku("23",mode,"20180428","02222",offset,danmakuText);
            if( mode == "scroll")
            {
                danmakuPlayer.AddScrollDanmaku(danmaku);
            }
            else if(mode == "top")
            {
                danmakuPlayer.AddTopDanmakuAsync(danmaku);
            }
            else
            {
                danmakuPlayer.AddBottomDanmaku(danmaku);
            }
        }

        private void customMTC_Fulled(object sender, EventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            bool isFull = view.IsFullScreenMode;
            if (!isFull)
            {
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
    }
}
