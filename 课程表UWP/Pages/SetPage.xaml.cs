using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.data;

namespace 课程表UWP.Pages
{

    public sealed partial class SetPage : Page, INotifyPropertyChanged
    {


        private bool _EnableNotify = false;

        public bool EnableNotify
        {
            get
            {
                AppSettings setting = new AppSettings();
                _EnableNotify = setting.EnableNotify;
                return _EnableNotify;
            }
            set
            {
                AppSettings setting = new AppSettings();
                setting.EnableNotify = value;
                _EnableNotify = value;
                SetProperty(ref _EnableNotify, value);
            }
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




        public SetPage()
        {
            this.InitializeComponent();


            //查找设值
            //ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            //object classtitle;
            //if (root.Values.TryGetValue("title", out classtitle))
            //{

            //    titleinput.Text = classtitle.ToString();
            //}
           // titleinput.Text = (new AppSettings()).DefaultFileName;

        }


   

        private void btn_yes_Tapped(object sender, TappedRoutedEventArgs e)
        {

            //ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            //root.Values["title"] = titleinput.Text;


            //(new AppSettings()).DefaultFileName = titleinput.Text;
            //cd1.Hide();
        }

        private void btn_cancel_Tapped(object sender, TappedRoutedEventArgs e)
        {
           // cd1.Hide();
        }


        //private async void editTitle_Click(object sender, RoutedEventArgs e)
        //{
        //    await cd1.ShowAsync();
        //}








        //清除所有课程信息

        //设置每天的课节数
        private async void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            int number;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            //获取安装包内xml
            StorageFile XmlFileInApp = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///data\class.xml"));
            Stream XmlReader = await XmlFileInApp.OpenStreamForReadAsync();
            //读取xml到内存
            XmlDocument xml = new XmlDocument();
            xml.Load(XmlReader);


            foreach (var element in xml.DocumentElement)
            {
                XmlElement each_element = (XmlElement)element;
                XmlNodeList nodelist = each_element.ChildNodes;
                number = cmbbox.SelectedIndex + 1;
                for (int i = nodelist.Count - 1; i >= number; i--)
                {
                    XmlNode n = nodelist[i];
                    each_element.RemoveChild(n);

                }
            }

            //在本地新建xml文件
            StorageFile XmlFileInLocal = await folder.CreateFileAsync("class", CreationCollisionOption.ReplaceExisting);
            //保存xml文件到local
            Stream XmlWriter = await XmlFileInLocal.OpenStreamForWriteAsync();
            xml.Save(XmlWriter);
            XmlWriter.Dispose();

            AppSettings MySetting = new AppSettings();
            MySetting.DefaultFileName = "class";

            MainPage.MainPageFrame.Navigate(typeof(SchedulePage));
            //Frame current = Window.Current.Content as Frame;
            //current.Navigate(typeof(MainPage));
        }

        //private void NotifyToggle_Toggled(object sender, RoutedEventArgs e)
        //{
        //    AppSettings settings = new AppSettings();
        //    settings.EnableNotify = NotifyToggle.IsOn;
        //}

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{

        //    AppSettings settings = new AppSettings();
        //    NotifyToggle.IsOn= settings.EnableNotify;




        //    base.OnNavigatedTo(e);  
        //}

    }

}

