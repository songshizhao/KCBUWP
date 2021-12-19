using BackgroundTasks;
using EasyNote.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.data;
using Course = 课程表UWP.data.Course;

namespace 课程表UWP.Controls
{

    public enum EditResult
    {
        cancel,
        save,
    }

    public sealed partial class EditDialog : ContentDialog, INotifyPropertyChanged
    {


        #region 属性和变量
        private bool _IsBusy = false;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set { SetProperty(ref _IsBusy, value); }
        }

        private Course _ChangedCourse;
        public Course ChangedCourse
        {
            get { return _ChangedCourse; }
            set { SetProperty(ref _ChangedCourse, value);}
        }
 
        public EditResult Result { get; set; } = EditResult.cancel;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

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









        public EditDialog(Course c)
        {

			//sv.Focus(FocusState.Keyboard);
			//nameTxt.Focus(FocusState.Unfocused);

			this.InitializeComponent();

            ChangedCourse = c;

            try
            {
                var start_hour = ChangedCourse.StartTime.Substring(0, 2);
                var start_minute = ChangedCourse.StartTime.Substring(3, 2);
                var end_hour = ChangedCourse.EndTime.Substring(0, 2);
                var end_minute = ChangedCourse.EndTime.Substring(3, 2);
                start_time.Time = new TimeSpan(Convert.ToInt32(start_hour), Convert.ToInt32(start_minute), 0);
                end_time.Time = new TimeSpan(Convert.ToInt32(end_hour), Convert.ToInt32(end_minute), 0);

            

            }
            catch (Exception ex)
            {
                ToastMessage.Toast("读取出错："+ex.Message);
            }

        }



        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ContentDialogButtonClickDeferral deferral = args.GetDeferral();

            ChangedCourse.StartTime = start_time.Time.ToString().Remove(start_time.Time.ToString().Length - 3, 3);
            ChangedCourse.EndTime = end_time.Time.ToString().Remove(end_time.Time.ToString().Length - 3, 3);

            Result = EditResult.save;

            deferral.Complete();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = EditResult.cancel;
        }


    }
}
