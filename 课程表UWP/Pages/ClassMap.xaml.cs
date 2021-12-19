using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.Controls;
using 课程表UWP.data;
using 课程表UWP.Models;
using static 课程表UWP.Models.StaticFunc;


namespace 课程表UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ClassMap : Page,INotifyPropertyChanged
    {

        // 实现INotify接口------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }





        int LoadItemCount = 10;


        //全局设置
        public GloableSetting GS { get; set; } = new GloableSetting();
        //是否在忙
        private bool _isbusy;
        public bool Isbusy
        {
            get { return _isbusy; }
            set { _isbusy = value; NotifyPropertyChanged(); }
        }



        private GeocoderSearchResponseResultAddressComponent _address=new GeocoderSearchResponseResultAddressComponent();
        public GeocoderSearchResponseResultAddressComponent Address
        {
            get { return _address; }
            set { _address = value; NotifyPropertyChanged(); }
        }

        


        //显示地点
        private ObservableCollection<PointOfInformation> _mapItemsSource = new ObservableCollection<PointOfInformation>();
        public ObservableCollection<PointOfInformation> MapItemsSource
        {
            get { return _mapItemsSource; }
            set { _mapItemsSource = value; NotifyPropertyChanged(); }
        }


        public double Latitude
        {
            get { return AppSettings.ReadSetting<double>("Latitude"); }
            set { AppSettings.WriteSetting<double>("Latitude",value); NotifyPropertyChanged(); }
        }
        public double Longitude
        {
            get { return AppSettings.ReadSetting<double>("Longitude"); }
            set { AppSettings.WriteSetting<double>("Longitude", value); NotifyPropertyChanged(); }
        }

        //public string Longitude
        //{
        //    get { return AppSettings.ReadSetting<double>("Longitude"); }
        //    set { AppSettings.WriteSetting<double>("Longitude", value); NotifyPropertyChanged(); }
        //}





        //地图标记资源
        RandomAccessStreamReference mapIconStreamReference;
        RandomAccessStreamReference mapBillboardStreamReference;
        RandomAccessStreamReference mapModelStreamReference;


        MapIcon  mapIcon = new MapIcon();

        public ClassMap()
        {



            this.InitializeComponent();
 
            myMap.Loaded += MyMap_Loaded;
            myMap.MapTapped += MyMap_MapTapped;
        }

       


        


        private void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            var tappedGeoPosition = args.Location.Position;
           
            mapIcon.Location = new Geopoint(tappedGeoPosition);



            //设置经纬度
            Latitude = tappedGeoPosition.Latitude;
            Longitude = tappedGeoPosition.Longitude;

        }











        public async Task GetNearByList(string 省份, string 城市,int m,int n)
        {
            CloudService.kechengbiaoSoapClient client = new CloudService.kechengbiaoSoapClient();
            var ScheduleList=await client.GetScheluleListNearbyAsync(省份, 城市,m,n);
            MapItemsSource.Clear();
            foreach (var schedule in ScheduleList)
            {

                try
                {
                    PointOfInformation scheduleSource = new PointOfInformation();
                    scheduleSource.ClassName = schedule.ScheduleName;
                    scheduleSource.Location = new Geopoint(new BasicGeoposition { Latitude = schedule.location.Latitude, Longitude = schedule.location.Longitude });
                    scheduleSource.CreateTime = schedule.CreateTime;
                    scheduleSource.id = schedule.ID;
                    if (schedule.ImageUrl.Length<2)
                    {
                        scheduleSource.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Robot01.jpg"));
                    }
                    else
                    {
                        scheduleSource.ImageSource = new BitmapImage(new Uri(schedule.ImageUrl));
                    }
                    
                    scheduleSource.NormalizedAnchorPoint = new Point(0.5, 1);
                    scheduleSource.statis = schedule.Statis;
                    scheduleSource.UserName = schedule.UserName;


                    scheduleSource.Distance = GetDistance(schedule.location.Latitude, schedule.location.Longitude,Latitude, Longitude)*0.001;



                    MapItemsSource.Add(scheduleSource);
                }
                catch (Exception ex)
                {

                    Debug.WriteLine(ex.Message);
                }

                


                
            }
        }



        //private void AddXamlChildren(List<PointOfInformation> list)
        //{


        //    //MapItemsSource

        //    BasicGeoposition center = myMap.Center.Position;
        //    var pinUri = new Uri("ms-appx:///Assets/MapPin.png");
        //    MapItems.ItemsSource = list;

        //    //MapItems.ItemsSource = new List<PointOfInformation>()
        //    //{
        //    //    new PointOfInformation()
        //    //    {
        //    //        ClassName = "Place One",

        //    //        Location = new Geopoint(new BasicGeoposition()
        //    //        {
        //    //            Latitude = center.Latitude + 0.001,
        //    //            Longitude = center.Longitude - 0.001
        //    //        }),
        //    //        NormalizedAnchorPoint = new Point(0.5, 1),
        //    //    },
        //    //};
        //}







        private async void MyMap_Loaded(object sender, RoutedEventArgs e)
        {

            Isbusy = true;





            try
            {


				var position = await Gloables.GetPosition();
				Latitude = position.Coordinate.Point.Position.Latitude;
				Longitude = position.Coordinate.Point.Position.Longitude;


				myMap.Center = position.Coordinate.Point;


				var 用户密钥 = "E90MkF5yCuKmdRRXLO1VpepxHtzVfDkH";
                var 输出格式类型 = "xml";
                string uri = $"http://api.map.baidu.com/geocoder?location={Latitude},{Longitude}&output={输出格式类型}&key={用户密钥}";//
                HttpClient BaiidMapClient = new HttpClient();
                HttpResponseMessage response = await BaiidMapClient.GetAsync(uri);
                String result = response.Content.ReadAsStringAsync().Result;
                XmlSerializer xmlSearializer = new XmlSerializer(typeof(GeocoderSearchResponse));
                GeocoderSearchResponse GeoResponse;
                using (StringReader sr = new StringReader(result))
                {
                    GeoResponse = (GeocoderSearchResponse)xmlSearializer.Deserialize(sr);
                }


                Address = GeoResponse.result.addressComponent;
                MyAddressTextBlock.Text = Address.province+"  "+ Address.city + "  " + Address.district;

                await GetNearByList(Address.province, Address.city,0,LoadItemCount);
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
            


            Isbusy = false;


            //添加地点指针
            mapIcon.Title = "我的位置";
            mapIcon.Location = (sender as MapControl).Center;
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/mappin.png"));
            mapIcon.ZIndex = 0;
            myMap.MapElements.Add(mapIcon);

        }

 


        //清楚标记
        private void mapClearButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            myMap.MapElements.Clear();
        }
        //添加图标标记
        private void mapIconAddButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MapIcon mapIcon1 = new MapIcon
            {
                Location = myMap.Center,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Title = "My Friend",
                Image = mapIconStreamReference,
                ZIndex = 0
            };
            myMap.MapElements.Add(mapIcon1);
        }

        /// <summary>
        /// This method will create a new billboard at the center of the map with the current camera as reference
        /// </summary>
        private void mapBillboardAddButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // MapBillboard scales with respect to the perspective projection of the 3D camera.
            // The reference camera determines at what view distance the billboard image appears at 1x scale.
            MapBillboard mapBillboard = new MapBillboard(myMap.ActualCamera);
            mapBillboard.Location = myMap.Center;
            mapBillboard.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapBillboard.Image = mapBillboardStreamReference;
            myMap.MapElements.Add(mapBillboard);
        }










        private async Task LoadPlaceInfoAsync()
        {
            Uri dataUri = new Uri("ms-appx:///places.txt");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            IList<string> lines = await FileIO.ReadLinesAsync(file);

            // In the places.txt file, each place is represented by three lines:
            // Place name, latitude, and longitude.
            for (int i = 0; i < lines.Count; i += 3)
            {
                PlaceInfo place = new PlaceInfo
                {
                    Name = lines[i],
                    Location = new PlaceLocation(double.Parse(lines[i + 1]), double.Parse(lines[i + 2]))
                };

                //places.Add(place);*********
            }
        }



        //点击我的位置
        private async void MyPositionButton_Click(object sender, RoutedEventArgs e)
        {
            Isbusy = true;
            var position = await Gloables.GetPosition();
            mapIcon.Location = position.Coordinate.Point;
            myMap.Center = position.Coordinate.Point;
            
            //设置经纬度
            Latitude = position.Coordinate.Point.Position.Latitude;
            Longitude= position.Coordinate.Point.Position.Longitude;

            Isbusy = false;
        }


        


        private async void LoginClick(object sender, RoutedEventArgs e)
        {

            LoginDialog LD = new LoginDialog();
            await LD.ShowAsync();
            switch (LD.Result)
            {
                case SignInResult.SignInOK:
                    App.IsLogin = true;
                    App.Username = LD.UserName;
                    App.Password = LD.PassWord;
                    break;
                case SignInResult.SignInFail:
                    break;
                case SignInResult.SignInCancel:
                    break;
                case SignInResult.Nothing:
                    break;
                default:
                    break;
            }

        }




        private async void PointButton_Click(object sender, RoutedEventArgs e)
        {
            var pointInfo = (PointOfInformation)(sender as Button).DataContext;
            var id = pointInfo.id;
            CloudService.kechengbiaoSoapClient client = new CloudService.kechengbiaoSoapClient();

            var data = await client.DownloadClassByIdAsync(id);
            byte[] filebyte = data.DownloadClassByIdResult;

            if (filebyte != null)
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync(pointInfo.ClassName, CreationCollisionOption.GenerateUniqueName);
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
                    return;

                }

            }
            else
            {
                var dialog = new MessageDialog("找不到服务器上的文件", "消息提示");
                await dialog.ShowAsync();
            }


            //Debug.WriteLine(uri);
            //Debug.WriteLine(result);
            //Debug.WriteLine(GeoResponse.result.addressComponent.province+GeoResponse.result.addressComponent.city+ GeoResponse.result.addressComponent.district);
        }

        private async void UploadClick(object sender, RoutedEventArgs e)
        {





            if (App.IsLogin)
            {
                var uploadDialog = new UploadDialog();
                uploadDialog.经度 = mapIcon.Location.Position.Longitude;
                uploadDialog.纬度 = mapIcon.Location.Position.Latitude;
                await uploadDialog.ShowAsync();
            }
            else
            {

                var dialog = new MessageDialog("当前尚未登录,是否登录?", "消息提示");
                dialog.Commands.Add(new UICommand("登录", async cmd => {
                    LoginDialog LD = new LoginDialog();
                    await LD.ShowAsync();
                    switch (LD.Result)
                    {
                        case SignInResult.SignInOK:
                            App.IsLogin = true;
                            App.Username = LD.UserName;
                            App.Password = LD.PassWord;
                            break;
                        case SignInResult.SignInFail:
                            break;
                        case SignInResult.SignInCancel:
                            break;
                        case SignInResult.Nothing:
                            break;
                        default:
                            break;
                    }
                }, commandId: 0));
                dialog.Commands.Add(new UICommand("取消", cmd => { }, commandId: 1));
                //设置默认按钮，不设置的话默认的确认按钮是第一个按钮
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;
                

                await dialog.ShowAsync();



                
            }






        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scroller = sender as ScrollViewer;
            var scrollableHeight = scroller.ScrollableHeight;


            if (scroller.VerticalOffset >= scrollableHeight)
            {
                Isbusy= true;

                //currentPage += 25;
                //await FindAll(currentPage + 1, currentPage + 25);
                Isbusy = false;

            }

        }
        //点击课表标签
        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PointOfInformation poi =e.ClickedItem as PointOfInformation;
            var popUp = new MessageDialog($"下载当前所选项吗?课表名称:{poi.ClassName}","提示");
            popUp.Commands.Add(new UICommand("下载", async cmd => {
                LoginDialog LD = new LoginDialog();
                await LD.ShowAsync();
                
            }, commandId: 0));
            popUp.Commands.Add(new UICommand("取消", cmd => { }, commandId: 1));
            //设置默认按钮，不设置的话默认的确认按钮是第一个按钮
            popUp.DefaultCommandIndex = 0;
            popUp.CancelCommandIndex = 1;
            await popUp.ShowAsync();



        }
        /// <summary>
        /// 按照距离排序
        /// </summary>
        private void RankByDistance_Click(object sender, RoutedEventArgs e)
        {


            ObservableCollection<PointOfInformation> newList=new ObservableCollection<PointOfInformation>();

            var list = MapItemsSource.OrderBy(x => x.Distance).ToList();
            MapItemsSource.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                MapItemsSource.Add(list[i]);
            }
            
        }
        /// <summary>
        /// 按照时间排序
        /// </summary>
        private void RankByTime_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<PointOfInformation> newList = new ObservableCollection<PointOfInformation>();

            var list = MapItemsSource.OrderByDescending(x => x.CreateTime).ToList();
            MapItemsSource.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                MapItemsSource.Add(list[i]);
            }
        }

        
        /// <summary>
        /// 按照下载量排序
        /// </summary>
        private void RankByDownload_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<PointOfInformation> newList = new ObservableCollection<PointOfInformation>();

            var list = MapItemsSource.OrderBy(x => x.statis).ToList();
            MapItemsSource.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                MapItemsSource.Add(list[i]);
            }
        }

 
    }





    public sealed class PlaceInfo
    {
        public string Name { get; set; }
        public PlaceLocation Location { get; set; }
    }

    public sealed class PointOfInformation
    {
        public Geopoint Location { get; set; }
        public Point NormalizedAnchorPoint { get; set; } = new Point(0.5, 1);
        public string ClassName { get; set; } 
        public string UserName { get; set; }


        public ImageSource ImageSource { get; set; }

        public double Distance;


        public DateTime CreateTime { get; set; }
        //受欢迎程度
        public int id { get; set; }
        public int statis { get; set; }
    }

    

    public struct PlaceLocation
    {
        public PlaceLocation(double latitude, double longitude)
        {
            Geoposition = new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
            MapCoordinates = GetMapCoordinates(Geoposition);
        }
        public BasicGeoposition Geoposition { get; }
        public Point MapCoordinates { get; }

        static private Point GetMapCoordinates(BasicGeoposition geoposition)
        {
            // This formula comes from https://msdn.microsoft.com/en-us/library/bb259689.aspx

            // Clamp latitude to the range -85.05112878 to 85.05112878.
            // This keeps Y in range and avoids the singularity at the pole.
            double latitude = Math.Max(Math.Min(geoposition.Latitude, 85.05112878), -85.05112878);

            double sinLatitude = Math.Sin(latitude * Math.PI / 180.0);
            return new Point
            {
                X = (geoposition.Longitude + 180.0) / 360.0,
                Y = 0.5 - Math.Log((1.0 + sinLatitude) / (1.0 - sinLatitude)) / (4 * Math.PI)
            };
        }
    }



    public class GloableSetting : INotifyPropertyChanged
    {
        // 实现INotify接口------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        // 实现INotify接口------------------------------------


        private double _zoomSlider = 15;
        public double ZoomSlider
        {
            get { return _zoomSlider; }
            set { _zoomSlider = value; NotifyPropertyChanged(); }
        }


        private double _heading = 0;
        public double Heading
        {
            get { return _heading; }
            set { _heading = value; NotifyPropertyChanged(); }
        }

        private double _desiredPitch = 30;
        public double DesiredPitch
        {
            get { return _desiredPitch; }
            set { _desiredPitch = value; NotifyPropertyChanged(); }
        }

        private MapStyle mapStyle = MapStyle.Road;
        public MapStyle MyMapStyle
        {
            get { return mapStyle; }
            set { mapStyle = value; NotifyPropertyChanged(); }
        }

        private MapProjection mapProjection = MapProjection.Globe;
        public MapProjection MyMapProjection
        {
            get { return mapProjection; }
            set { mapProjection = value; NotifyPropertyChanged(); }
        }

        private bool trafficFlowVisible;
        public bool TrafficFlowVisible
        {
            get { return trafficFlowVisible; }
            set { trafficFlowVisible = value; NotifyPropertyChanged(); }
        }



    }


    public class Gloables
    {

        public async static Task<Geoposition> GetPosition(uint acc = 0)
        {
            Geoposition position = null;

            //请求对位置的访问权
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Unspecified:
                    //定位未开启提示是否跳转到 设置 页面 
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings://privacy/location"));
                    break;
                case GeolocationAccessStatus.Allowed:
                    //允许获取 
                    //实例化定位类，并设置经纬度精确度（单位：米），一般为零，为保护用户隐私，建议减少精确度
                    var geolocator = new Geolocator { DesiredAccuracyInMeters = acc };
                    //异步获取设备位置，并将位置保存到变量中（Geoposition类型）
                    position = await geolocator.GetGeopositionAsync();

                    //返回位置

                    break;
                case GeolocationAccessStatus.Denied:
                    //不允许获取位置信息时 给予提示 然后根据情况选择是否跳转到 设置 界面 
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings://privacy/location"));

                    break;
                default:
                    break;
            }

            return position;

        }









    }


    public class DistanceConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = "";
            var d = (double)value;
            if (d<1)
            {
                //变成m为单位
                d = d * 1000;
                result=String.Format("{0:N2}",d)+" m";

            }
            else
            {
                result = String.Format("{0:N2}", d) + " km";
            }

            return result;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return 0;
        }
    }
    public class DateTimeConverter : IValueConverter
    {
      

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var time = (DateTime)value;
            return time.ToString("yyyy年MM月dd日");
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
