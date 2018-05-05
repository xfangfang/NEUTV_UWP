using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace NetEasePlayer_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>

    public sealed partial class MainPage : Page
    {

        private String url = "http://hdtv.neu6.edu.cn/hdtv.json";
        Frame root = Window.Current.Content as Frame;

        public MainPage()
        {
            this.InitializeComponent();
            //背景模糊
            //this.InitializeFrostedGlass(GlassHost);
            //去掉标题栏
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;
            //var view = ApplicationView.GetForCurrentView();
            // view.TitleBar.ButtonBackgroundColor = Colors.Transparent; //将标题栏的三个键背景设为透明
            //view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent; //失去焦点时，将三个键背景设为透明
            //view.TitleBar.ButtonInactiveForegroundColor = Colors.White; //失去焦点时，将三个键前景色设为白色```

            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                root.CanGoBack ?
                AppViewBackButtonVisibility.Visible : Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
            root.Navigated += OnNavigated;

            Gets(new Uri(this.url));
        }
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();

            }
        }


        //添加一个pivot页面
        public void AddItem(PlayList playList)
        {
            GridView g = new GridView();
            g.HorizontalAlignment = HorizontalAlignment.Center;

            PivotItem pi1 = new PivotItem
            {
                Header = playList.Name,
                Content = g
            };
            mainpage_pivot.Items.Add(pi1);
            foreach (Live obj in playList.Livelist)
            {
                TextBlock t = new TextBlock
                {
                    Text = obj.Name,
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };


                Button b = new Button
                {
                    Content = "\xE006",
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Tag = obj,
                    Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
                    FontFamily = font,
                    Width = 50,
                    Height = 50,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0))
                };
                b.Name = obj.Name;  //方便查找相应的btn
                b.Click += MainPage_Click;//主界面的点击事件
                btns.Add(b);//将主界面的每个btn都加到列表里


                Image img = new Image
                {
                    Source = new BitmapImage(new Uri("https://hdtv.neu6.edu.cn/wall/img/" + obj.GetSimpleName() + "_s.png")),
                    //TODO 可以换一种更睿智的方法，目前显示效果不够好
                    //高清频道和非高清频道图片分辨率不同,手动设置图片宽高
                    //否则非高清频道 textblock无法显示
                    Width = 300,
                    Height = 225
                };


                bool a = false;
                foreach (Live live1 in playList1.Livelist)
                {
                    if (obj.Name == live1.Name) { a = true; }
                }
                if (a) { b.Content = "\xE00B"; }//初始化主界面button的值

                StackPanel itemPanel = new StackPanel();
                itemPanel.Margin = new Thickness(8);
                itemPanel.Children.Add(img);
                itemPanel.Children.Add(t);
                itemPanel.Children.Add(b);
                itemPanel.Tag = obj;
                g.Items.Add(itemPanel);
            }



            g.IsItemClickEnabled = true;
            g.ItemClick += new ItemClickEventHandler((sender, arg) => {
                Live live = (Live)((StackPanel)arg.ClickedItem).Tag;
                //Debug.WriteLine(live.Name);
                root.Navigate(typeof(PlayerPage), live);

            });
        }

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.UI.Xaml.Media.FontFamily font = new Windows.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets");
        public PlayList playList1 = new PlayList("我的收藏", "ilike");
        public GridView grid;
        public List<StackPanel> panels = new List<StackPanel>();
        public List<Button> btns = new List<Button>(); //初始化
        public GridView createGridView(PlayList playList) //单独创建一个名为我的收藏的gridview，返回值为这个gridview
        {
            grid = new GridView();
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            PivotItem pi1 = new PivotItem
            {
                Header = playList.Name,
                Content = grid
            };
            mainpage_pivot.Items.Add(pi1);
            grid.IsItemClickEnabled = true;
            grid.ItemClick += new ItemClickEventHandler((sender, arg) => {
                Live live = (Live)((StackPanel)arg.ClickedItem).Tag;
                //Debug.WriteLine(live.Name);
                root.Navigate(typeof(PlayerPage), live);
                Debug.WriteLine(live.GetSimpleName());
            });
            return grid;
        }
        async void MainPage_Click(object sender, RoutedEventArgs e)  //这个是主页面的点击事件
        {
            Button btn = (Button)sender;
            if (btn.Content.ToString() == "\xE006")
            {
                Live live = (Live)((Button)sender).Tag;
                btn.Content = "\xE00B";
                playList1.AddLive(live);
                AddLivesToPivot(grid, live);
                StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt", CreationCollisionOption.OpenIfExists);
                String b = await FileIO.ReadTextAsync(sampleFile);
                await FileIO.WriteTextAsync(sampleFile, b + "+" + live.Url + "+" + live.Name + "+" + live.Uid);
                //未收藏时的功能
            }
            else
            {
                Live live = (Live)((Button)sender).Tag;
                btn.Content = "\xE006";
                playList1.RemoveLive(live);
                StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt", CreationCollisionOption.ReplaceExisting);
                foreach (Live live2 in playList1.Livelist)
                {
                    String b = await FileIO.ReadTextAsync(sampleFile);
                    await FileIO.WriteTextAsync(sampleFile, b + "+" + live2.Url + "+" + live2.Name + "+" + live2.Uid);
                    Debug.WriteLine("a");
                }
                foreach (StackPanel stackpanel in panels)
                { if (stackpanel.Name == live.Name) { grid.Items.Remove(stackpanel); } }
            }//取消收藏时的功能
        }

        async void Favorite_Click(object sender, RoutedEventArgs e) //我的收藏界面的点击事件
        {
            StackPanel itemPanel = (StackPanel)((Button)sender).Tag;
            grid.Items.Remove(itemPanel);
            Live live2 = (Live)(itemPanel).Tag;
            playList1.RemoveLive(live2);
            foreach (Button button in btns)
            { if (button.Name == live2.Name) { button.Content = "\xE006"; } }
            StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt", CreationCollisionOption.ReplaceExisting);
            foreach (Live live in playList1.Livelist)
            {

                String b = await FileIO.ReadTextAsync(sampleFile);
                await FileIO.WriteTextAsync(sampleFile, b + "+" + live.Url + "+" + live.Name + "+" + live.Uid);
                Debug.WriteLine("a");
            }

        }
        void AddLivesToPivot(GridView grid, Live a)  //向我的收藏里添加新的频道
        {
            StackPanel itemPanel = new StackPanel();
            TextBlock t = new TextBlock
            {
                Text = a.Name,
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            Button c = new Button
            {
                Content = "\xE00B",
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                Tag = itemPanel,
                Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255)),
                FontFamily = font,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),

            };
            c.Click += Favorite_Click;


            Image img = new Image
            {
                Source = new BitmapImage(new Uri("https://hdtv.neu6.edu.cn/wall/img/" + a.GetSimpleName() + "_s.png")),
                //TODO 可以换一种更睿智的方法，目前显示效果不够好
                //高清频道和非高清频道图片分辨率不同,手动设置图片宽高
                //否则非高清频道 textblock无法显示
                Width = 300,
                Height = 225
            };

            itemPanel.Margin = new Thickness(8);
            itemPanel.Children.Add(img);
            itemPanel.Children.Add(t);
            itemPanel.Children.Add(c);
            itemPanel.Tag = a;
            itemPanel.Name = a.Name;
            grid.Items.Add(itemPanel);
            panels.Add(itemPanel);

        }


        // https://docs.microsoft.com/zh-cn/uwp/api/windows.storage.pickers.fileopenpicker#examples

        public async Task Gets(Uri uri)
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

                JsonObject jsonObject = JsonObject.Parse(str);
                JsonArray types = jsonObject["type"].GetArray();
                JsonArray lives = jsonObject["live"].GetArray();

                //储存节目单
                Dictionary<String, PlayList> listMap = new Dictionary<string, PlayList>();

                foreach (JsonValue obj in types)
                {
                    String name = obj.GetObject()["name"].GetString();
                    String uid = obj.GetObject()["id"].GetString();
                    listMap.Add(uid, new PlayList(name, uid));
                }

                // Live json格式
                // "num":50001,
                // "itemid":"uid0",
                // "name":"CCTV-1高清",
                // "urllist":"http:....."

                foreach (JsonValue obj in lives)
                {
                    String name = obj.GetObject()["name"].GetString();
                    String itemid = obj.GetObject()["itemid"].GetString();
                    String urllist = obj.GetObject()["urllist"].GetString();
                    listMap[itemid].AddLive(new Live(urllist, name, itemid));
                }


                GridView grid = this.createGridView(playList1);//创建我的收藏的gridview
                StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt", CreationCollisionOption.OpenIfExists);
                String timestamp = await FileIO.ReadTextAsync(sampleFile);
                string[] Array = timestamp.Split(new char[] { '+' });
                for (int i = 1; i < Array.Length - 2; i = i + 3)
                {
                    Live live = new Live(Array[i], Array[i + 1], Array[i + 2]);
                    playList1.AddLive(live);
                    AddLivesToPivot(grid, live);
                } //把我的收藏里面的频道加到gridview里


                //添加到页面
                foreach (KeyValuePair<String, PlayList> pair in listMap)
                {
                    PlayList playList = pair.Value;
                    this.AddItem(playList);

                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("xxxxx{0}", e);
                button_open.Visibility = Visibility.Visible;
            }
            finally
            {
                mainpage_progress_ring.IsActive = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainpage_progress_ring.IsActive = true;
            Gets(new Uri(this.url));
            button_open.Visibility = Visibility.Collapsed;
        }

        private void InitializeFrostedGlass(UIElement glassHost)
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(glassHost);
            Compositor compositor = hostVisual.Compositor;
            var backdropBrush = compositor.CreateHostBackdropBrush();
            var glassVisual = compositor.CreateSpriteVisual();
            glassVisual.Brush = backdropBrush;
            ElementCompositionPreview.SetElementChildVisual(glassHost, glassVisual);
            var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);
            glassVisual.StartAnimation("Size", bindSizeAnimation);
        }

    }
}




