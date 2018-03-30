using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace NetEasePlayer_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Binding binding;
        public MainPage()
        {
            this.InitializeComponent();
  
        }



        // https://docs.microsoft.com/zh-cn/uwp/api/windows.storage.pickers.fileopenpicker#examples
        
        private async System.Threading.Tasks.Task openFileDialogAsync()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".mp4");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                //text_file.Text = "Picked photo: " + file.Name;
                // https://blog.csdn.net/linwh8/article/details/70314698
                video_player.Source = MediaSource.CreateFromStorageFile(file);
                video_player.Visibility = Visibility.Visible;
                button_open.Visibility = Visibility.Collapsed;
                // https://docs.microsoft.com/zh-cn/uwp/api/windows.ui.xaml.controls.mediaplayerelement#handle-media-events
                // http://www.runoob.com/csharp/csharp-event.html
                video_player.MediaPlayer.MediaEnded += new TypedEventHandler<Windows.Media.Playback.MediaPlayer, object>((player,obj)=> {
                    // https://www.cnblogs.com/jiahuafu/p/5478695.html
                    button_open.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,() => {
                        button_open.Visibility = Visibility.Visible;
                        video_player.Visibility = Visibility.Collapsed;
                    });
                });
            } 
            else
            {
                text_file.Text = "未选择文件";
            }
        }

    
        private void Button_Click(object sender, RoutedEventArgs e)
        {
             openFileDialogAsync();
        }
    }
}
