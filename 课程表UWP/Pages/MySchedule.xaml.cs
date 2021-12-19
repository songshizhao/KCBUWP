using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 课程表UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MySchedule : Page, INotifyPropertyChanged
    {

        public MySchedule()
        {
            this.InitializeComponent();
        }

        #region 属性和变量
        private bool _IsLoading = true;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }


        IReadOnlyList<StorageFile> _StorageFiles;
        public IReadOnlyList<StorageFile> StorageFiles
        {
            get { return _StorageFiles; }
            set { SetProperty(ref _StorageFiles, value); }
        }

        //实现INotify接口************************************
        public event PropertyChangedEventHandler PropertyChanged;
        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region 事件
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            IsLoading = true;
            //---
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFiles = await folder.GetFilesAsync();
            //---
            IsLoading = false;
            //base.OnNavigatedTo(e);  
        }
        #endregion


        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            StorageFile file=e.ClickedItem as StorageFile;

            MainPage.MainPageFrame.Navigate(typeof(SchedulePage), file);
            //Frame current = Window.Current.Content as Frame;
          
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var t = new FolderLauncherOptions();
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            await Launcher.LaunchFolderAsync(folder, t);
        }

        private async void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadDialog d = new DownloadDialog();
            d.Title = "为课表命名";
            IsLoading = true;
            await d.ShowAsync();
            if (d.Result.Length>0)
            {
                ///读取xml到内存
                XmlDocument xml = new XmlDocument();
                ///获取安装包内xml
                StorageFile DefaultXmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///data\class.xml"));
                using (Stream XmlReader = await DefaultXmlFile.OpenStreamForReadAsync())
                {
                    xml.Load(XmlReader);
                }
                //找到本地文件夹
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile LocalXmlFile = await folder.CreateFileAsync(d.Result, CreationCollisionOption.ReplaceExisting);
                ///保存xml文件到local
                using (Stream XmlWriter = await LocalXmlFile.OpenStreamForWriteAsync())
                {
                    xml.Save(XmlWriter);
                }
                OnNavigatedTo(null);
            }

            IsLoading = false;

        }
    }
}
