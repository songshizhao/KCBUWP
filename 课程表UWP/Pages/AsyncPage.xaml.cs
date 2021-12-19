using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using 课程表UWP.Controls;
using 课程表UWP.data;
using 课程表UWP.mywebservice;


namespace 课程表UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AsyncPage : Page, INotifyPropertyChanged
    {


        bool _isPullRefresh = false;
        public bool IsPullRefresh
        {
            get
            {
                return _isPullRefresh;
            }

            set
            {
                _isPullRefresh = value;
                OnPropertyChanged(nameof(IsPullRefresh));
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }











        int currentPage = 1;

        public AsyncPage()
        {
            this.InitializeComponent();
            this.Loaded += AsyncPage_Loaded;

            //listView.DataContext = data;
            
        }

        private async void AsyncPage_Loaded(object sender, RoutedEventArgs e)
        {
			await FindAll(currentPage, currentPage + 25);


			classtitleinput.Text = (new AppSettings()).DefaultFileName;

            //存储设置，记录上传过的课程表名称和ID
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;

            object uploadhistory;
            if (root.Values.TryGetValue("historyupload", out uploadhistory))
            {
                historyupload.Text = uploadhistory.ToString();
            }
        }

        //上传课表
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] filebyte;
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            AppSettings setting = new AppSettings();


            StorageFile file = await folder.GetFileAsync(setting.DefaultFileName);
            if (file != null)
            {
                IBuffer buffer = null;

                using (IRandomAccessStream streamIn = await file.OpenReadAsync())
                {
                    buffer = WindowsRuntimeBuffer.Create((int)streamIn.Size);

                    await streamIn.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.None);

                    filebyte = buffer.ToArray();

                    string classtitle = classtitleinput.Text;

                    if (classtitle == "")
                    {
                        var dialog = new MessageDialog("您输入的课表名称为空，请给你上传的课表起个名字吧~", "消息提示");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        mywebservice.kechengbiaoSoapClient client = new mywebservice.kechengbiaoSoapClient();

                        int xc = client.uploadclassAsync(classtitle, filebyte).Result.uploadclassResult;

                        if (xc == 0)
                        {
                            var dialog = new MessageDialog("您输入的课表名称在服务器已经存在了，请换一个名字存储吧~", "消息提示");
                            await dialog.ShowAsync();
                        }
                        else
                        {
                            string message = "您输入的课表上传成功,课表名为" + classtitle + ",课表的数字ID为" + xc.ToString() + "。";

                            var dialog = new MessageDialog(message, "消息提示");
                            await dialog.ShowAsync();

                            //存储设置，记录上传过的课程表名称和ID

                            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;

                            object uploadhistory;
                            string txt;
                            if (root.Values.TryGetValue("historyupload", out uploadhistory))
                            {
                                txt = uploadhistory.ToString() + "课表名称：" + classtitle + ",课表ID：" + xc.ToString() + "。";
                                root.Values["historyupload"] = txt;
                            }
                            else
                            {
                                txt = "课表名称：" + classtitle + ",课表ID：" + xc.ToString() + "。";
                                root.Values["historyupload"] = txt;
                            }
                            historyupload.Text = txt;
                        }
                    }

                }
            }
        }
        //根据标题下载课表
        private async void btn_searchtitle_Click(object sender, RoutedEventArgs e)
        {
            string classtitle = searchtitleinput.Text;
            if (classtitle == "")
            {
                var dialog = new MessageDialog("标题似乎为空...", "消息提示");
                await dialog.ShowAsync();
                return;
            }
            else
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                try
                {
                    mywebservice.kechengbiaoSoapClient client = new mywebservice.kechengbiaoSoapClient();
                    int size = await client.GetDocumentLenByNameAsync(classtitle);

                    if (size == 0)
                    {
                        var dialog = new MessageDialog("[找不到输入课表文件，size=" + size + "]", "消息提示");
                        await dialog.ShowAsync();
                        return;
                    }

                    byte[] filebyte = new byte[size];// = new byte[size];
                    Debug.WriteLine(size);

                    mywebservice.DownloadClassByNameResponse x = await client.DownloadClassByNameAsync(classtitle);
                    filebyte = x.DownloadClassByNameResult;

                    if (filebyte != null)
                    {
                        StorageFile file = await folder.CreateFileAsync(classtitle, CreationCollisionOption.ReplaceExisting);
                        IBuffer buffer = filebyte.AsBuffer();
                        using (IRandomAccessStream writestream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
                        {
                            await writestream.WriteAsync(buffer);




                            var dialog = new MessageDialog("文件已经下载完成");
                            dialog.Commands.Add(new UICommand("打开", cmd => {
                                Frame.Navigate(typeof(SchedulePage), file);
                            }, commandId: 0));
                            dialog.Commands.Add(new UICommand("取消", cmd => { }, commandId: 1));
                            //设置默认按钮，不设置的话默认的确认按钮是第一个按钮
                            dialog.DefaultCommandIndex = 0;
                            dialog.CancelCommandIndex = 1;
                            //获取返回值
                            var result = await dialog.ShowAsync();

                            //var dialog = new MessageDialog("success：D[课表已经成功载入了，快去看看吧~]", "消息提示");
                            //await dialog.ShowAsync();
                            return;
                        }
                    }
                    else
                    {
                        var dialog = new MessageDialog("error：<[找不到输入title的课表，请确认课表title]", "消息提示");
                        await dialog.ShowAsync();
                    }

                }
                catch (Exception)
                {
                    var dialog = new MessageDialog("error：D[课表id不是数字]", "消息提示");
                    await dialog.ShowAsync();
                }

            }

        }
        //根据id下载
        private async void btn_searchid_Click(object sender, RoutedEventArgs e)
        {
            int classid;

            DownloadDialog dd = new DownloadDialog();
            await dd.ShowAsync();
            string SaveFileName = dd.Result;

            if (SaveFileName.Length>0)
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                try
                {

                    try
                    {
                        classid = Convert.ToInt32(searchidinput.Text);
                    }
                    catch (Exception)
                    {

                        var dialog = new MessageDialog("输入的似乎不是数字...", "消息提示");
                        await dialog.ShowAsync();
                        return;
                    }

                    if (classid <= 0)
                    {
                        var dialog = new MessageDialog("要输入正整数哇...", "消息提示");
                        await dialog.ShowAsync();
                        return;
                    }


                    kechengbiaoSoapClient client = new mywebservice.kechengbiaoSoapClient();
                    int size = await client.GetDocumentLenByIdAsync(classid);

                    if (size == 0)
                    {
                        var dialog = new MessageDialog("[找不到输入id的课表，请确认课表id]", "消息提示");
                        await dialog.ShowAsync();
                        return;
                    }

                    byte[] filebyte = new byte[size];
                    Debug.WriteLine(size);

                    mywebservice.DownloadClassByIdResponse x = await client.DownloadClassByIdAsync(classid);
                    filebyte = x.DownloadClassByIdResult;

                    if (filebyte != null)
                    {
                        StorageFile file = await folder.CreateFileAsync(SaveFileName, CreationCollisionOption.ReplaceExisting);
                        IBuffer buffer = filebyte.AsBuffer();
                        using (IRandomAccessStream writestream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
                        {

                            await writestream.WriteAsync(buffer);


                            var dialog = new MessageDialog("文件已经下载完成，文件名为" + SaveFileName);
                            dialog.Commands.Add(new UICommand("打开", cmd => {
                                Frame.Navigate(typeof(SchedulePage), file);
                            }, commandId: 0));
                            dialog.Commands.Add(new UICommand("取消", cmd => { }, commandId: 1));
                            //设置默认按钮，不设置的话默认的确认按钮是第一个按钮
                            dialog.DefaultCommandIndex = 0;
                            dialog.CancelCommandIndex = 1;
                            //获取返回值
                            var result = await dialog.ShowAsync();

                            return;

                        }



                    }
                    else
                    {
                        var dialog = new MessageDialog("[找不到输入id的课表，请确认课表id]", "消息提示");
                        await dialog.ShowAsync();
                    }

                }
                catch (Exception)
                {
                    var dialog = new MessageDialog("[课表id不是数字]", "消息提示");
                    await dialog.ShowAsync();
                }
            }
            


        }

        ObservableCollection<ClassIndex> data = new ObservableCollection<ClassIndex>();
        private async Task FindAll(int m,int n)
        {
            //data.Clear();
            try
            {
                kechengbiaoSoapClient client = new kechengbiaoSoapClient();
                ArrayOfXElement tables = await client.SelectMNAsync(m,n);
               
                foreach (XElement el in tables.Nodes.Descendants("Table"))
                {
                    ClassIndex table = new ClassIndex();
                    foreach (XElement ell in el.Nodes())
                    {
                        switch (Convert.ToString(ell.Name))
                        {
                            case "ID":
                                table.IndexId = Convert.ToInt32(ell.Value);
                                break;
                            case "classtitle":
                                table.IndexTitle = ell.Value;
                                break;
                            default:
                                break;
                        }
                    }
                    data.Add(table);
                }
                listView.DataContext = data;
            }
            catch (Exception)
            {
            }
        }
        //点击item下载
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Button btn = sender as Button;
            int classid;

            DownloadDialog dd = new DownloadDialog();
            await dd.ShowAsync();
            string SaveFileName = dd.Result;

            if (dd.Result.Length>0)
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                try
                {
                    try
                    {
                        classid = Convert.ToInt32(btn.Tag.ToString());
                    }
                    catch (Exception)
                    {
                        var dialog = new MessageDialog("输入的似乎不是数字...", "消息提示");
                        await dialog.ShowAsync();
                        return;
                    }

                    if (classid <= 0)
                    {
                        var dialog = new MessageDialog("要输入正整数哇...", "消息提示");
                        await dialog.ShowAsync();
                        return;
                    }


                    kechengbiaoSoapClient client = new kechengbiaoSoapClient();
                    int size = await client.GetDocumentLenByIdAsync(classid);

                    if (size == 0)
                    {
                        var dialog = new MessageDialog("[找不到输入id的课表，请确认课表id]", "消息提示");
                        await dialog.ShowAsync();
                        return;
                    }

                    byte[] filebyte = new byte[size];
                    Debug.WriteLine(size);

                    DownloadClassByIdResponse x = await client.DownloadClassByIdAsync(classid);
                    filebyte = x.DownloadClassByIdResult;

                    if (filebyte != null)
                    {

                        StorageFile file = await folder.CreateFileAsync(SaveFileName, CreationCollisionOption.ReplaceExisting);
                        IBuffer buffer = filebyte.AsBuffer();
                        using (IRandomAccessStream writestream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
                        {

                            await writestream.WriteAsync(buffer);
                            var dialog = new MessageDialog("文件已经下载完成，文件名为" + SaveFileName);
                            dialog.Commands.Add(new UICommand("打开", cmd => {
                                Frame.Navigate(typeof(SchedulePage), file);
                            }, commandId: 0));
                            dialog.Commands.Add(new UICommand("取消", cmd => { }, commandId: 1));
                            //设置默认按钮，不设置的话默认的确认按钮是第一个按钮
                            dialog.DefaultCommandIndex = 0;
                            dialog.CancelCommandIndex = 1;
                            //获取返回值
                            var result = await dialog.ShowAsync();
                            return;

                        }

                    }
                    else
                    {
                        var dialog = new MessageDialog("[找不到输入id的课表，请确认课表id]", "消息提示");
                        await dialog.ShowAsync();
                    }
                }
                catch (Exception)
                {
                    var dialog = new MessageDialog("[课表id不是数字]", "消息提示");
                    await dialog.ShowAsync();
                }
            }

 
        }

        private async void NextPageButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            currentPage += 25;
            await FindAll(currentPage, currentPage + 25);
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {

            //var scroller = sender as ScrollViewer;
            //var scrollableHeight = scroller.ScrollableHeight.ToString();

            //Debug.WriteLine("可滚动高度："+ scrollableHeight + "/垂直偏移"+ e.FinalView.VerticalOffset);
           
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scroller = sender as ScrollViewer;
            var scrollableHeight = scroller.ScrollableHeight;

            


            if (scroller.VerticalOffset>= scrollableHeight)
            {
                IsPullRefresh = true;

                currentPage += 25;
                await FindAll(currentPage+1, currentPage + 25);

                IsPullRefresh = false;
            }



            //var extentHeight = scroller.ExtentHeight.ToString();
            //var actualHeight = scroller.ActualHeight.ToString();


            //var sv = sender as ScrollViewer;

            //if (!e.IsIntermediate)
            //{
            //    if (sv.VerticalOffset == 0.0)
            //    {
            //        IsPullRefresh = true;




            //        sv.ChangeView(null, 30, null);
            //    }
            //    IsPullRefresh = false;
            //}


            //var scroller = sender as ScrollViewer;
            //var scrollableHeight = scroller.ScrollableHeight.ToString();
            //var extentHeight = scroller.ExtentHeight.ToString();
            //var actualHeight = scroller.ActualHeight.ToString();


        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

}
            






