using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            PivotItem pi1 = new PivotItem
            {
                Header = playList.Name,
                Content = g
            };
            mainpage_pivot.Items.Add(pi1);
        
           foreach(Live obj in playList.Livelist)
            {
                TextBlock t = new TextBlock
                {
                    Text = obj.Name,
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri("https://hdtv.neu6.edu.cn/wall/img/" + obj.GetSimpleName() + "_s.png")),
                    //TODO 可以换一种更睿智的方法，目前显示效果不够好
                    //高清频道和非高清频道图片分辨率不同,手动设置图片宽高
                    //否则非高清频道 textblock无法显示
                    Width = 300,
                    Height = 225
                };

                StackPanel itemPanel = new StackPanel();
                itemPanel.Margin = new Thickness(8);
                itemPanel.Children.Add(img);
                itemPanel.Children.Add(t);
                itemPanel.Tag = obj;
                g.Items.Add(itemPanel);
            }
            g.IsItemClickEnabled = true;
            g.ItemClick += new ItemClickEventHandler((sender,arg) => {
                Live live = (Live)((StackPanel)arg.ClickedItem).Tag;
                Debug.WriteLine(live.Name);
                root.Navigate(typeof(PlayerPage),live);

            });
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

                //添加到页面
                foreach(KeyValuePair<String,PlayList> pair in listMap)
                {
                    PlayList playList = pair.Value;
                    this.AddItem(playList);
                }

            }
            catch(Exception e)
            {
                Debug.WriteLine("xxxxx{0}",e);
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
