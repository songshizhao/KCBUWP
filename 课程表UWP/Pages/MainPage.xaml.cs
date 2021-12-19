using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.Controls;
using 课程表UWP.data;
using 课程表UWP.Pages;

namespace 课程表UWP
{

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public static Frame MainPageFrame;

        private string _DateDisplay;
        public string DateDisplay
        {
            get { return _DateDisplay; }
            set { SetProperty(ref _DateDisplay, value); }
        }
        // 实现INotify接口************************************
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
        // 实现INotify接口************************************




        public MainPage()
        {
            this.InitializeComponent();
            MainPageFrame = mainFrame;


            // 启动大小 和上次调整后相同
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            //ApplicationView.PreferredLaunchViewSize = new Size(1280, 720);
            //ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(1280, 720));

            // 扩展标题栏和改变标题栏外观
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(MyTitle);



            switch (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Desktop":
                    //电脑改变标题栏样式
                    var appTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
                    appTitleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
                    appTitleBar.ButtonHoverBackgroundColor = Windows.UI.Colors.Transparent;
					appTitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;

					appTitleBar.ButtonForegroundColor = Windows.UI.Colors.White;
					appTitleBar.ButtonHoverForegroundColor = Windows.UI.Colors.IndianRed;
                    //显示时间右移
                    TimeDisplay.Margin = new Thickness(0,0,180,0);
                    break;
                case "Windows.Mobile":
                    break;
                case "Windows.Tablet":
                    break;
                case "Windows.SurfaceHub":
                    break;
                case "Windows.Xbox":
                    break;
                case "Windows.Iot":
                    break;
                default:
                    break;
            }


            this.Loaded += MainPage_Loaded;


            DateDisplay = "今天 星期";
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    DateDisplay += "五";
                    break;
                case DayOfWeek.Monday:
                    DateDisplay += "一";
                    break;
                case DayOfWeek.Saturday:
                    DateDisplay += "六";
                    break;
                case DayOfWeek.Sunday:
                    DateDisplay += "日";
                    break;
                case DayOfWeek.Thursday:
                    DateDisplay += "四";
                    break;
                case DayOfWeek.Tuesday:
                    DateDisplay += "二";
                    break;
                case DayOfWeek.Wednesday:
                    DateDisplay += "三";
                    break;
                default:
                    break;
            }
            DateDisplay +=  DateTime.Now.ToString("M月d日");
            Debug.WriteLine(DateDisplay);
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }



        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
			

			mainFrame.Navigate(typeof(SchedulePage));


			//设置皮肤



			try
			{
				var SkinImageBrush = new ImageBrush();
				StorageFile file = null;
				switch (AppSettings.ReadSetting<string>("skin"))
				{
					case "":
						file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///skin/1.jpg"));
						break;
					case "1":
						file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///skin/1.jpg"));
						break;
					case "2":
						file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///skin/2.jpg"));
						break;
					case "3":
						file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///skin/3.jpg"));
						break;
					case "4":
						file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///skin/4.jpg"));
						break;
					case "5":
						file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///skin/5.jpg"));
						break;
					case "6":
						file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///skin/6.jpg"));
						break;
					default:
						if (AppSettings.ReadSetting<string>("skin")==null)
						{
							file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///skin/1.jpg"));
						}
						else
						{
							file = await FilesHelpper.GetAuthorisedFile(AppSettings.ReadSetting<string>("skin"));
						}
						break;
				}
				if (file!=null)
				{
					using (IRandomAccessStream ir = await file.OpenAsync(FileAccessMode.Read))
					{
						BitmapImage bi = new BitmapImage();
						await bi.SetSourceAsync(ir);
						SkinImageBrush.ImageSource = bi;
						this.Background = SkinImageBrush;
					}
				}

			}
			catch (Exception)
			{

			}


		}


    
        //呼出汉堡菜单
        private void menu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
        }
        //汉堡菜单选项
        private async void Item_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListBoxItem item= (ListBoxItem)sender;
            switch (item.Tag.ToString())
            {
                case "我的课表":
                    mainFrame.Navigate(typeof(SchedulePage));
                    break;
				case "课表地图":
					mainFrame.Navigate(typeof(ClassMap));
					break;
				case "本地文件":
                    mainFrame.Navigate(typeof(MySchedule));
                    break;
                case "课表同步":
                    mainFrame.Navigate(typeof(AsyncPage));
                    break;
                case "课表设置":
                    mainFrame.Navigate(typeof(SetPage));
                    break;
                case "关于作者":
                    mainFrame.Navigate(typeof(AboutPage));
                    break;
                case "更新日志":
                    mainFrame.Navigate(typeof(BlogPage));
                    break;
                case "给予好评":
                    //var Uri = new Uri(string.Format("ms-windows-store:navigate?appid={0}", "9NBLGGH4V0P5"));
                    //var Uri = new Uri("ms-windows-store://pdp/?productid=9nblggh4v0p5");
                    //ms - windows - store://review/?ProductId=9nblggh4v0p5
                    var Uri = new Uri("ms-windows-store://review/?productid=9nblggh4v0p5");
                    await Launcher.LaunchUriAsync(Uri);
                    break;
   
                default:
                    break;
            }
        }

		private void ChangeSkin_Click(object sender, RoutedEventArgs e)
		{

			var btn = sender as Button;
			var flyout = new Flyout();
			ChangeSkin cs = new ChangeSkin();
			cs.ChangeSkinAction += Cs_ChangeSkinAction;
			flyout.Content = cs;
			flyout.ShowAt(btn);
		}

		private void Cs_ChangeSkinAction(Windows.UI.Xaml.Media.ImageBrush obj)
		{
			this.Background = obj;
		}
	}

}








//private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
//{
//    MainHub.Height = Window.Current.Bounds.Height - 65;
//}

//protected override void OnNavigatedTo(NavigationEventArgs e)
//{

//    findday();
//}