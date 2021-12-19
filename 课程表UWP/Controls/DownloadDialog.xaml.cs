
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace 课程表UWP.Controls
{
    public enum DownloadResult
    {
        ok,
        cancel,

    }
    public sealed partial class DownloadDialog : ContentDialog,INotifyPropertyChanged
    {

        public DownloadResult DownloadResult { get; set; } = DownloadResult.cancel;
        public string Result { get; set; }
		public string InitalFileName
		{
			get { return ""; }
			set { FileNameBox.Text = value; }
		} 
		private bool _IsBusy=false;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set {SetProperty(ref _IsBusy, value); }
        }

        //public IRandomAccessStream FileStream;
        public StorageFile File { get; set; }





        //实现INotify接口
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





        public DownloadDialog()
        {
            this.InitializeComponent();
			

		}

        private  void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

           //ContentDialogButtonClickDeferral deferral = args.GetDeferral();



            Result = FileNameBox.Text;


           // deferral.Complete();
           
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = "";
        }

        //private async void browsefile(object sender, RoutedEventArgs e)
        //{
        //    await PickFile();
        //}


        //public async Task PickFile()
        //{
        //    IsBusy = true;

        //    StorageFolder LOCAL = ApplicationData.Current.LocalFolder;
        //    DataPackage dataPackage = new DataPackage();
        //    dataPackage.SetText(LOCAL.Path);
        //    Clipboard.SetContent(dataPackage);

        //    FileOpenPicker open = new FileOpenPicker();
        //    open.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        //    open.FileTypeFilter.Add(".rtf");
        //    File = await open.PickSingleFileAsync();
        //    if (File!=null)
        //    {
        //        FileNameBox.Text = File.Name;
        //    }
            
        //    IsBusy = false;

        //}



        public async Task Download()
        {
            IsBusy = true;
            if (File != null)
            {
                using (var streamIn = await File.OpenAsync(FileAccessMode.Read))
                {

                }
            }
            else
            {
                Result = "没有选择文件";
            }

            IsBusy = false;
        }

        private void FileNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
