using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.data;
using 课程表UWP.Pages;


namespace 课程表UWP.Controls
{
    public enum SignInResult
    {
        SignInOK,
        SignInFail,
        SignInCancel,
        Nothing
    }

    public sealed partial class LoginDialog : ContentDialog, INotifyPropertyChanged
    {
        public SignInResult Result { get;set; }
	

		//实现INotify接口------------------------------------
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
        //实现INotify接口------------------------------------

        private bool _Isbusy = false;
        public bool IsBusy
        {
            get { return _Isbusy; }
            set { SetProperty(ref _Isbusy, value); }
        }



        private string loginReport="";
        public string LoginReport
        {
            get { return loginReport; }
            set { SetProperty(ref loginReport, value);}
        }



        //设置
        public AppSettings ClientSetting = new AppSettings();
        //属性

        public bool? SaveUserName
        {
            get
            {
                return ClientSetting.SaveUserName;
            }
            set { ClientSetting.SaveUserName = (bool)value; NotifyPropertyChanged(); }
        }
        public string UserName
        {
            get
            {
                return AppSettings.ReadSetting<string>("UserName");
            }
            set
            {
                AppSettings.WriteSetting<string>("UserName",value);
                NotifyPropertyChanged();
            }
        }
        public string PassWord
        {
            get
            {
                return AppSettings.ReadSetting<string>("PassWord");
            }
            set
            {
                AppSettings.WriteSetting<string>("PassWord", value);
                NotifyPropertyChanged();
            }
        }





        public LoginDialog()
        {
            this.InitializeComponent();

			//保存的设置
			InputPane.GetForCurrentView().Showing += KeyBoard_Showing;
			InputPane.GetForCurrentView().Hiding += KeyBoard_Hiding;
        }

        public async void Login_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsBusy = true;

			CloudService.kechengbiaoSoapClient client = new CloudService.kechengbiaoSoapClient();
            
            if ((await client.LoginAsync(UserName, PassWord)))
            {
                LoginReport = "以登陆";
                this.Result = SignInResult.SignInOK;
                this.Hide();

				var popUp = new MessageDialog("用户已登录成功", "登录成功");
				await popUp.ShowAsync();




			}
            else
            {
                LoginReport = "登录失败";
                this.Result = SignInResult.SignInFail;
            }

            IsBusy = false;
        }


        private void CloseDialog_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Result = SignInResult.SignInCancel;
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

        private async void SignupButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Result = SignInResult.SignInCancel;
            this.Hide();
            SignUpDialog sud = new SignUpDialog();
            await sud.ShowAsync();
        }
    }
}
