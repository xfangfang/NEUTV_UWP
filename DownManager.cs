using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Web.Http;

namespace NetEasePlayer_UWP
{
    public interface IDown
    {
        void Process();
        void Start(int len);
        void End();
    }
    class DownManager
    {
        static IDown down;
        private static async Task SaveVideoFile(StorageFolder tempDic,
           StorageFolder downloadDic, List<String> urlList)
        {
            var tsRe = new Regex(@"(?<ts>[0-9]+\.ts)");
            //新建视频文件
            StorageFile tsFile =
                    await downloadDic.CreateFileAsync("whole.ts",
                    CreationCollisionOption.GenerateUniqueName);
            ulong fileSize = 0;
            using (var destinationStream = await tsFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                //合并视频
                foreach (var videoUrl in urlList)
                {
                    StorageFile tempFile =
                        await tempDic.CreateFileAsync(tsRe.Match(videoUrl).Result("${ts}"),
                        CreationCollisionOption.OpenIfExists);
                    //  Debug.WriteLine(tsRe.Match(videoUrl).Result("${ts}"));
                    using (var inputStream = await tempFile.OpenAsync(FileAccessMode.Read))
                    {
                        using (var destinationOutputStream = destinationStream.GetOutputStreamAt(fileSize))
                        {
                            // Debug.WriteLine(fileSize);
                            fileSize += inputStream.Size;
                            await RandomAccessStream.CopyAndCloseAsync(inputStream, destinationOutputStream);
                        }
                    }
                }
            }

            //删除临时文件夹
            await tempDic.DeleteAsync();
            // Update the UI thread with the CoreDispatcher.
            if(down != null)
            {
                down.End();
            }
            

        }

        public static async Task DownloadShowAsync(One show,Live live,IDown d)
        {
            DownManager.down = d;
            // http://media2.neu6.edu.cn/review/program-1524520800-1524526860-chchd.m3u8
            var url = String.Format("http://media2.neu6.edu.cn/review/program-{0}-{1}-{2}.m3u8",
                show.start, show.end, live.GetSimpleName());
            Debug.WriteLine(url);
            //创建下载目录
            var downloadDic = await KnownFolders.VideosLibrary.CreateFolderAsync("NEUTV download",
                CreationCollisionOption.OpenIfExists);
            //下载m3u8文件
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync(new Uri(url));
            var res = await response.Content.ReadAsStringAsync();
            Regex re = new Regex(@"(?<url>http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?)");
            MatchCollection mc = re.Matches(res);
            List<String> urlList = new List<string>();
            var tsRe = new Regex(@"(?<ts>[0-9]+\.ts)");
            foreach (Match m in mc)
            {
                var clipUrl = m.Result("${url}");
                urlList.Add(clipUrl);
            }
            if(down != null)
            {
                down.Start(urlList.Count);
            }
            //创建临时文件夹
            var tempDic = await downloadDic.CreateFolderAsync("temp",
                CreationCollisionOption.GenerateUniqueName);

            //下载 视频分片
            var Tasks = new List<Task>();
            TaskFactory fac = new TaskFactory();

            Debug.WriteLine("start all");
            try
            {
                Task.Run(() =>
                {
                    Parallel.ForEach<string>(urlList, new ParallelOptions() { MaxDegreeOfParallelism = 10 },
                        videoUrl => {
                            Act(videoUrl, httpClient, tempDic).Wait();
                        });
                }).ContinueWith((obj) =>
                {
                    Debug.WriteLine("end all");
                    SaveVideoFile(tempDic, downloadDic, urlList);
                });
            }
            catch (Exception e)
            {

            }
           

        }


        static async Task Act(String videoUrl, HttpClient httpClient, StorageFolder tempDic)
        {
            var tsRe = new Regex(@"(?<ts>[0-9]+\.ts)");
            Debug.WriteLine("start " + tsRe.Match(videoUrl).Result("${ts}"));
            var response = await httpClient.GetAsync(new Uri(videoUrl));
            var sourceStream = await response.Content.ReadAsInputStreamAsync();
            StorageFile destinationFile =
                await tempDic.CreateFileAsync(tsRe.Match(videoUrl).Result("${ts}"),
                CreationCollisionOption.GenerateUniqueName);

            using (var destinationStream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await RandomAccessStream.CopyAndCloseAsync(sourceStream, destinationStream);
            }
            Debug.WriteLine("end " + tsRe.Match(videoUrl).Result("${ts}"));
            if(down != null)
            {
                down.Process();
            }
        }

        public static async Task ShowDialog(String content)
        {
            MessageDialog msg = new MessageDialog(content);
            await msg.ShowAsync();
        }
    }
}
