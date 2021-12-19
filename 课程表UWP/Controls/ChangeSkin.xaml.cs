using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.data;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace 课程表UWP.Controls
{
    public sealed partial class ChangeSkin : UserControl, INotifyPropertyChanged
	{

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


		public RadioButton SelectedRadio { get; set; }


		public event Action<ImageBrush> ChangeSkinAction;





		private ObservableCollection<StorageFile> _files= new ObservableCollection<StorageFile>();

		public ObservableCollection<StorageFile> files
		{
			get { return _files; }
			set { _files = value; OnPropertyChanged(); }
		}



		public ChangeSkin()
        {
            this.InitializeComponent();
			this.Loaded += ChangeSkin_Loaded;
        }

		private async void ChangeSkin_Loaded(object sender, RoutedEventArgs e)
		{
			StorageFolder folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(Package.Current.InstalledLocation.Path + @"\skin\");


			var sfiles = await folder.GetFilesAsync();
			foreach (var file in sfiles)
			{
				Debug.WriteLine(file.ContentType);

				if (file.ContentType=="image/jpeg")
				{
					files.Add(file);
				}

				
			}

		}









		private void Button_Click(object sender, RoutedEventArgs e)
		{

			if (SelectedRadio==null)
			{
				return;
			}
			else
			{

				var SkinImageBrush = new ImageBrush();
				
				SkinImageBrush.ImageSource = ((Image)SelectedRadio.Content).Source;
				
				//发动事件
				ChangeSkinAction(SkinImageBrush);




                //更换文件
                foreach (var file in files)
                {
					var path = SelectedRadio.Tag.ToString();
					if (file.Path== path)
                    {
						string mruToken = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);

						AppSettings.WriteSetting<string>("skin", mruToken);
					}
                }



				

			}

		}



		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			try
			{
				SelectedRadio = sender as RadioButton;
			}
			catch (Exception)
			{

			}
			
		}


		private async void UserDefine_Click(object sender, RoutedEventArgs e)
		{
			// 选择一个图片作为壁纸
			//AppSettings setting = new AppSettings();

			// 让用户选取一个图片文件
			FileOpenPicker fPicker = new FileOpenPicker();
			fPicker.FileTypeFilter.Add(".jpg");
			fPicker.FileTypeFilter.Add(".jpeg");
			fPicker.FileTypeFilter.Add(".png");
			fPicker.FileTypeFilter.Add(".bmp");
			var file = await fPicker.PickSingleFileAsync();
			if (file != null)
			{
				
				//

				string mruToken = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
				AppSettings.WriteSetting<string>("skin", mruToken);
				// Add to FA without metadata
				//var faToken1 = StorageApplicationPermissions.FutureAccessList.Add(selectedFile);
				//setting.BackgroundImgToken = mruToken;
				//AppSettings.WriteSetting<string>("skin", index.ToString());
				var SkinImageBrush = new ImageBrush();
				//BitmapImage bimg = new BitmapImage();
				Debug.WriteLine($"图片文件路径{file.Path}");
				//bimg.UriSource = new Uri(file.Path);
				//SkinImageBrush.ImageSource = bimg;


				using (IRandomAccessStream ir = await file.OpenAsync(FileAccessMode.Read))
				{
					BitmapImage bi = new BitmapImage();
					await bi.SetSourceAsync(ir);
					SkinImageBrush.ImageSource = bi;

				}

				//替换壁纸
				ChangeSkinAction(SkinImageBrush);
			}





	
			
		}
	}
}
