using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace 课程表UWP.Pages
{
    public sealed partial class SignUpDialog : ContentDialog,INotifyPropertyChanged
    {
        //实现INotify接口
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
        //实现INotify接口

        private string _message="";
        public string Message
        {
            get { return _message; }
            set { _message = value;NotifyPropertyChanged(); }
        }


        private bool _Isbusy = false;
        public bool IsBusy
        {
            get { return _Isbusy; }
            set { SetProperty(ref _Isbusy, value); }
        }




        
       
        public bool isUsernameReady;
        public bool isPasswordReady;
        public bool isPasswordRepitReady;
        public bool isEmailReady;


        public SignUpDialog()
        {

            this.InitializeComponent();


            EmailInput.LostFocus += EmailInput_LostFocus;

            PasswordInput.LostFocus += PasswordInput_LostFocus;
            PasswordInput2.LostFocus += PasswordInput2_LostFocus;
            UserNameInput.LostFocus += UserNameInput_LostFocus;
            EmailCodeInput.LostFocus += EmailCodeInput_LostFocus;
            

        }


        //该报了用户名输入
        private void UserNameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            var SenderTextBox = sender as TextBox;



            if (!IsUsername(SenderTextBox.Text))
            {
                SenderTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                SenderTextBox.BorderThickness = new Thickness(1);

                RemindMsg("用户名格式不正确");
                isUsernameReady = false;
                
            }
            else
            {
                SenderTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
                SenderTextBox.BorderThickness = new Thickness(2);
                isUsernameReady = true;
            }
        }

        private void EmailCodeInput_LostFocus(object sender, RoutedEventArgs e)
        {
            var SenderTextBox = sender as TextBox;



            //if (!IsUsername(SenderTextBox.Text))
            //{
            //    SenderTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            //    SenderTextBox.BorderThickness = new Thickness(1);

            //    RemindMsg("用户名格式不正确");

            //    SenderTextBox.BorderThickness = new Thickness(1);
            //    isEmailReady = false;
            //}
            //else
            //{
            //    SenderTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
            //    SenderTextBox.BorderThickness = new Thickness(2);
            //    isEmailReady = true;
            //}
        }

        private void PasswordInput2_LostFocus(object sender, RoutedEventArgs e)
        {
            var SenderTextBox = sender as PasswordBox;



            if (SenderTextBox.Password!=PasswordInput.Password)
            {
                SenderTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                SenderTextBox.BorderThickness = new Thickness(2);

                isPasswordRepitReady = false;
                RemindMsg("两次密码输入不一致");

             
            }
            else
            {

                SenderTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
                SenderTextBox.BorderThickness = new Thickness(2);

                isPasswordRepitReady =true;
            }
        }



        private void PasswordInput_LostFocus(object sender, RoutedEventArgs e)
        {
            var SenderTextBox = sender as PasswordBox;



            if (!IsPassword(SenderTextBox.Password))
            {

                SenderTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                SenderTextBox.BorderThickness = new Thickness(1);

                isPasswordReady = false;
                RemindMsg("密码格式不正确");

            }
            else
            {

                SenderTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
                SenderTextBox.BorderThickness = new Thickness(2);

                isPasswordReady = true;
            }
        }

        private void EmailInput_LostFocus(object sender, RoutedEventArgs e)
        {
            var SenderTextBox = sender as TextBox;



            if (!IsEmail(SenderTextBox.Text))
            {

                SenderTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                SenderTextBox.BorderThickness = new Thickness(1);
                RemindMsg("不是有效的电子邮箱地址");
                isEmailReady = false;
            }
            else
            {
                SenderTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
                SenderTextBox.BorderThickness = new Thickness(2);

                isEmailReady = true;
            }
        }







        public void RemindMsg(string msg)
        {

            if (msg.Length > 0)
            {
                MessageContainer.Visibility = Visibility.Visible;
                Message = msg;
                MsgStoryboard.Begin();
            }
            MsgStoryboard.Completed += MsgStoryboard_Completed;
        }

        private void MsgStoryboard_Completed(object sender, object e)
        {
            MessageContainer.Visibility = Visibility.Collapsed;
            Message = "";
        }




        #region 辅助函数
        public bool IsUsername(string name)
        {
            Regex rgx = new Regex(@"^[\u4e00-\u9fff\w]{5,16}$");
            return rgx.IsMatch(name);
        }
        public bool IsPassword(string pwd)
        {
            Regex rgx = new Regex("^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,10}$");//密码范围广
            return rgx.IsMatch(pwd);
        }
        public bool IsEmail(string email)
        {
            Regex rgx = new Regex("^[a-z0-9]+([._\\-]*[a-z0-9])*@([a-z0-9]+[-a-z0-9]*[a-z0-9]+.){1,63}[a-z0-9]+$");//密码范围广
            return rgx.IsMatch(email);
        }







        #endregion

        private async void SingupButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!isEmailReady)
            {
                RemindMsg("邮箱地址格式不正确");
                return;
            }
            if (!isPasswordReady)
            {
                RemindMsg("密码格式不正确");
                return;
            }
            if (!isPasswordRepitReady)
            {
                RemindMsg("两次输入密码不一致");
                return;
            }
            if (!isUsernameReady)
            {
                RemindMsg("用户名为空或不支持的用户名格式");
                return;
            }
			UserService.userSoapClient client = new UserService.userSoapClient();
			//CloudService.kechengbiaoSoapClient client = new CloudService.kechengbiaoSoapClient();
			//client.RegistUserAsync()
			var x = await client.RegistUserAsync(UserNameInput.Text, PasswordInput.Password, EmailInput.Text, EmailCodeInput.Text);

            if (x.IsSuccess)
            {
                RemindMsg("注册成功请前往登录");

        
                var popUp = new MessageDialog("是否前往登录?", "注册成功");
                popUp.Commands.Add(new UICommand("好的!", async cmd => {
                    this.Hide();
                    LoginDialog LD = new LoginDialog();
                    await LD.ShowAsync();

                }, commandId: 0));
                popUp.Commands.Add(new UICommand("不..", cmd => { }, commandId: 1));
                //设置默认按钮，不设置的话默认的确认按钮是第一个按钮
                popUp.DefaultCommandIndex = 0;
                popUp.CancelCommandIndex = 1;
                await popUp.ShowAsync();
            }
            else
            {
                RemindMsg("注册失败"+x.MsgAlert);
            }
        }
        
        private async void GetEmailCodeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

            //发送邮件验证码
            //!为了防止重复点击,设置点击频率间隔15秒
            //(已更改为服务器验证,无需客户端验证)

            if (isEmailReady)
            {
                CloudService.kechengbiaoSoapClient client = new CloudService.kechengbiaoSoapClient();
                var x=await client.SendEmailCodeAsync(EmailInput.Text);

                if (x.IsSuccess)
                {
                    RemindMsg("已发送邮箱验证码");
                }
                else
                {
                    RemindMsg("发送失败:"+x.MsgAlert);
                }
            }
            else
            {
                RemindMsg("邮箱地址格式不正确");
            }
        }

        private void closeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
