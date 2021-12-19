using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;

namespace 课程表UWP.data
{
	public class FilesHelpper
	{
		//加载本地文件
		public static async Task<IReadOnlyList<StorageFile>> GetLocalFilesAsync()
		{
			StorageFolder folder = ApplicationData.Current.LocalFolder;

			IReadOnlyList<StorageFile> StorageFiles = await folder.GetFilesAsync();

			return StorageFiles;
		}

		//打开本地文件存储路径
		public static async void CheckLocalFileInExplorer()
		{
			var MyFolderLauncherOptions = new FolderLauncherOptions();
			StorageFolder folder = ApplicationData.Current.LocalFolder;
			await Launcher.LaunchFolderAsync(folder, MyFolderLauncherOptions);
		}

		public static async Task SaveTextToFileAsync(string text, StorageFile file, Encoding encoding = null)
		{
			if (encoding == null)
			{
				//获取记事本的编码
				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
				encoding = Encoding.GetEncoding(0);
			}
			using (Stream Fstream = await file.OpenStreamForWriteAsync())
			{
				using (StreamWriter writer = new StreamWriter(Fstream, encoding))
				{
					await writer.WriteAsync(text);
				}
			}
		}

		//打开本地文件存储路径
		public static async Task OpenFilePath(StorageFile file, string faToken = "")
		{
			//获取文件所在的文件夹路径
			var path = file.Path.Replace(@"\" + file.Name, "");
			Debug.WriteLine(path);

			//检查是否拥有文件夹权限
			try
			{
				StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
				//Debug.WriteLine("拥有文件夹权限,打开文件夹");
				await Launcher.LaunchFolderAsync(folder);
			}
			catch (Exception ex)
			{

				//Debug.WriteLine("无法获取文件夹"+ex.Message);




				var dialog = new MessageDialog(AppResources.GetString("请求文件夹权限说明"), AppResources.GetString("请求文件夹权限标题"));
				dialog.Commands.Add(new UICommand(AppResources.GetString("请求权限获取"), async cmd =>
				{
					FolderPicker fPicker = new FolderPicker();
					fPicker.FileTypeFilter.Add(".txt");
					fPicker.FileTypeFilter.Add(".rtf");
					fPicker.FileTypeFilter.Add(".md");
					var selectedFolder = await fPicker.PickSingleFolderAsync();
					if (selectedFolder != null)
					{
						//string mruToken = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file, "20120716");

						// Add to FA without metadata
						var faToken1 = StorageApplicationPermissions.FutureAccessList.Add(selectedFolder);


					}
				}, commandId: 0));
				dialog.Commands.Add(new UICommand(AppResources.GetString("请求权限拒绝"), cmd => { }, commandId: 1));
				//设置默认按钮，不设置的话默认的确认按钮是第一个按钮
				dialog.DefaultCommandIndex = 0;
				dialog.CancelCommandIndex = 1;
				//获取返回值
				var result = await dialog.ShowAsync();

			}

		}









		//获取工作文件夹
		public static async Task<StorageFile> GetAuthorisedFile(string token)
		{

			try
			{
				StorageFile file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(token);

				return file;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}


			//try
			//{

			//	return folder;
			//}
			//catch (Exception ex)
			//{
			//	Debug.WriteLine(ex.Message);
			//	return ApplicationData.Current.LocalFolder;

			//}



			//UserSetting Us = new UserSetting();
			//if (Us.WorkFolderToken.Length > 0)
			//{
			//	try
			//	{
			//		StorageFolder folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(Us.WorkFolderToken);
			//		return folder;
			//	}
			//	catch (Exception ex)
			//	{
			//		Debug.WriteLine(ex.Message);
			//		return ApplicationData.Current.LocalFolder;

			//	}
			//}
			//else
			//{
			//	return ApplicationData.Current.LocalFolder;
			//}


		}


		//打开本地文件存储路径
		public static async Task<StorageFile> CreateFile(string filename)
		{
			try
			{
				var folder = KnownFolders.AppCaptures;//await GetWorkFolder();
				var file = await folder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
				return file;
			}
			catch (Exception ex)
			{

				MessageDialog ErrorBox = new MessageDialog(ex.Message);
				await ErrorBox.ShowAsync();
				return null;
			}


		}


	}



	public static class AppResources
	{
		private static ResourceLoader CurrentResourceLoader
		{
			get { return _loader ?? (_loader = ResourceLoader.GetForCurrentView("Resources")); }
		}

		private static ResourceLoader _loader;
		private static readonly Dictionary<string, string> ResourceCache = new Dictionary<string, string>();

		public static string GetString(string key)
		{
			string s;
			if (ResourceCache.TryGetValue(key, out s))
			{
				return s;
			}
			else
			{
				s = CurrentResourceLoader.GetString(key);
				ResourceCache[key] = s;
				return s;
			}
		}



		/// <summary>
		/// AppName
		/// </summary>
		public static string AppName
		{
			get
			{

				return CurrentResourceLoader.GetString("AppName");
			}
		}
	}

}
