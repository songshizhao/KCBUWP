using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
using 课程表UWP.CloudService;
using 课程表UWP.data;
using 课程表UWP.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace 课程表UWP.Controls
{
    public sealed partial class UploadDialog : ContentDialog, INotifyPropertyChanged
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

        //是否在忙
        private bool _isbusy=false;
        public bool Isbusy
        {
            get { return _isbusy; }
            set { _isbusy = value; NotifyPropertyChanged(); }
        }



        private double _经度;
        public double 经度
        {
            get { return _经度; }
            set { _经度 = value; NotifyPropertyChanged(); }
        }


        private double _纬度;
        public double 纬度
        {
            get { return _纬度; }
            set { _纬度 = value; NotifyPropertyChanged(); }
        }


        private string _省份;

        public string 省份
        {
            get { return _省份; }
            set { _省份 = value; NotifyPropertyChanged(); }
        }

        private string _城市;

        public string 城市
        {
            get { return _城市; }
            set { _城市 = value; NotifyPropertyChanged(); }
        }

        private Location location;
        public Location Location
        {
            get { return location; }
            set { location = value; NotifyPropertyChanged(); }
        }
        //默认上传文件名字为 当前默认课表 用户上传前可以修改，但不改变默认设置
        private string uploadFIleName = AppSettings.ReadSetting<string>("DefaultFileName");
        public string UploadFIleName
        {
            get { return uploadFIleName; }
            set { uploadFIleName = value; NotifyPropertyChanged(); }
        }



        public UploadDialog()
        {
            this.InitializeComponent();
            this.Loaded += UploadDialog_Loaded;

			//弹出键盘控制
			InputPane.GetForCurrentView().Showing += KeyBoard_Showing;
			InputPane.GetForCurrentView().Hiding += KeyBoard_Hiding;
		}

        private async void UploadDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Isbusy = true;
            var 用户密钥 = "E90MkF5yCuKmdRRXLO1VpepxHtzVfDkH";
            var 输出格式类型 = "xml";
            string uri = $"http://api.map.baidu.com/geocoder?location={纬度},{经度}&output={输出格式类型}&key={用户密钥}";//
            HttpClient BaiidMapClient = new HttpClient();
            HttpResponseMessage response = await BaiidMapClient.GetAsync(uri);
            String result = response.Content.ReadAsStringAsync().Result;
            XmlSerializer xmlSearializer = new XmlSerializer(typeof(GeocoderSearchResponse));
            GeocoderSearchResponse GeoResponse;
            using (StringReader sr = new StringReader(result))
            {
                GeoResponse = (GeocoderSearchResponse)xmlSearializer.Deserialize(sr);
            }
            Location = new Location
            {
                Province = GeoResponse.result.addressComponent.province,
                City = GeoResponse.result.addressComponent.city,
                District = GeoResponse.result.addressComponent.district,
                Longitude = Convert.ToDouble(GeoResponse.result.location.lng),
                Latitude = Convert.ToDouble(GeoResponse.result.location.lat),
            };
            Isbusy = false;
        }


        private async void Upload_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine(UploadFIleName+"1234");


            Isbusy = true;
            try
            {
                if (App.IsLogin)
                {
                    kechengbiaoSoapClient client = new kechengbiaoSoapClient();
                    UserInfo user = new UserInfo
                    {
                        Username = App.Username,
                        Password = App.Password,
                        Photo = "",
                        IsVip = false,
                    };

                    //读取File为二进制
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

                            string classtitle = UploadFIleName;
                            
                            if (classtitle == "")
                            {
                                var dialog = new MessageDialog("您输入的课表名称为空，请给你上传的课表起个名字吧~", "消息提示");
                                await dialog.ShowAsync();
                            }
                            else
                            {


                                var r = await client.UploadAsync(user, location, filebyte, classtitle, "");

                                
                                var dialog = new MessageDialog(r.UploadResult, "消息提示");
                                await dialog.ShowAsync();


                            }

                        }
                    }



                }
                else
                {

                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }


            Isbusy = false;
        }


        private void CloseDialog_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
            this.Hide();
        }

		private void KeyBoard_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
		{
			ReservedZoneForKeyBoard.Height = new GridLength(1, GridUnitType.Star).Value;
		}

		private void KeyBoard_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
		{
			ReservedZoneForKeyBoard.Height = new GridLength(args.OccludedRect.Height).Value;
		}
	}
}
