
using BackgroundTasks;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.Controls;
using 课程表UWP.data;
using Course = 课程表UWP.data.Course;
using Schedule = 课程表UWP.data.Schedule;

namespace 课程表UWP.Pages
{

	public sealed partial class SchedulePage : Page,INotifyPropertyChanged
	{


		#region 属性和变量


		private string _DateDisplay;
		public string DateDisplay
		{
			get { return _DateDisplay; }
			set { SetProperty(ref _DateDisplay, value); }
		}


		private bool _IsLoadingXml = true;

		public bool IsLoadingXml
		{
			get { return _IsLoadingXml; }
			set { SetProperty(ref _IsLoadingXml, value); }
		}



		
		private ObservableCollection<Course> _Monday = new ObservableCollection<Course>();

		public ObservableCollection<Course> Monday
		{
			get { return _Monday; }
			set { SetProperty(ref _Monday, value); }
		}

		private ObservableCollection<Course> _Tuesday = new ObservableCollection<Course>();

		public ObservableCollection<Course> Tuesday
		{
			get { return _Tuesday; }
			set { SetProperty(ref _Tuesday, value); }
		}
		private ObservableCollection<Course> _Wednesday = new ObservableCollection<Course>();

		public ObservableCollection<Course> Wednesday
		{
			get { return _Wednesday; }
			set { SetProperty(ref _Wednesday, value); }
		}

		private ObservableCollection<Course> _Thursday = new ObservableCollection<Course>();

		public ObservableCollection<Course> Thursday
		{
			get { return _Thursday; }
			set { SetProperty(ref _Thursday, value); }
		}

		private ObservableCollection<Course> _Friday = new ObservableCollection<Course>();

		public ObservableCollection<Course> Friday
		{
			get { return _Friday; }
			set { SetProperty(ref _Friday, value); }
		}

		private ObservableCollection<Course> _Saturday = new ObservableCollection<Course>();

		public ObservableCollection<Course> Saturday
		{
			get { return _Saturday; }
			set { SetProperty(ref _Saturday, value); }
		}

		private ObservableCollection<Course> _Sunday = new ObservableCollection<Course>();

		public ObservableCollection<Course> Sunday
		{
			get { return _Sunday; }
			set { SetProperty(ref _Sunday, value); }
		}



		public static Schedule MySchedule;


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



		public SchedulePage()
		{
			InitializeComponent();
			//this.DataContext = this;
			//commandBar.DataContext = this;
			//paneHeader.DataContext = this;
			
		}

		public static async Task<bool> IsFileExist(string fileName)
		{
			var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
			return item != null;
		}

		//数据读取初始化
		async Task InitLoading()
		{
			IsLoadingXml = true;

			//找到本地文件夹
			StorageFolder folder = ApplicationData.Current.LocalFolder;

			//获取当前要显示的课表的文件名
			AppSettings MySetting = new AppSettings();
			string filename = MySetting.DefaultFileName;
			Debug.WriteLine("打开默认文件,名称为: "+filename);
			
			if (await IsFileExist(filename) == false)
			{
				
				Debug.WriteLine("没有找到默认文件: ");
				if (MySetting.IsAppFirstStart)
				{
					ToastMessage.Toast("欢迎使用课程表UWP，welcom！");
				}
				else
				{
					ToastMessage.Toast("找不到文件：" + filename + " ;数据重置");
					///设置默认打开的文件为class
					MySetting.DefaultFileName = "class";
				}

				///读取xml到内存
				XmlDocument xml = new XmlDocument();
				///获取安装包内xml
				StorageFile DefaultXmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///data\class.xml"));

				var newfile=await DefaultXmlFile.CopyAsync(folder,"class",NameCollisionOption.ReplaceExisting);
				MySetting.DefaultFileName = newfile.DisplayName;

			}


			Debug.WriteLine("读取Xml文件... ");

			//读xml文件
			string s="";
			try
			{
				StorageFile XmlFile = await folder.GetFileAsync(MySetting.DefaultFileName);

				using (Stream File = await XmlFile.OpenStreamForReadAsync())
				{
					using (StreamReader read = new StreamReader(File))
					{
						s = read.ReadToEnd();

					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message+"l210");
			}




			using (StringReader sr = new StringReader(s))
			{
				XmlSerializer xmlSearializer = new XmlSerializer(typeof(Schedule));
				MySchedule = (Schedule)xmlSearializer.Deserialize(sr);
			}



			try
			{
				//11111111111111111111
				if (MySchedule.day1.Courses != null)
				{
					Monday = MySchedule.day1.Courses;
				}
				else
				{
					Day1List.Visibility = Visibility.Collapsed;
				}
				//22222222222222222222
				if (MySchedule.day2.Courses != null)
				{

					Tuesday = MySchedule.day2.Courses;
				}
				else
				{
					Day2List.Visibility = Visibility.Collapsed;
				}
				//333333333333333333333
				if (MySchedule.day3.Courses != null)
				{
					Wednesday = MySchedule.day3.Courses;

				}
				else
				{
					Day3List.Visibility = Visibility.Collapsed;
				}
				//4444444444444444444
				if (MySchedule.day4.Courses != null)
				{
					Thursday = MySchedule.day4.Courses;

				}
				else
				{
					Day4List.Visibility = Visibility.Collapsed;
				}
				//5555555555555555555555
				if (MySchedule.day5.Courses != null)
				{
					Friday = MySchedule.day5.Courses;

				}
				else
				{
					Day5List.Visibility = Visibility.Collapsed;
				}
				//6666666666666666666666
				if (MySchedule.day6.Courses != null)
				{
					Saturday = MySchedule.day6.Courses;

				}
				else
				{
					Day6List.Visibility = Visibility.Collapsed;
				}
				//77777777777777777777777
				if (MySchedule.day7.Courses != null)
				{
					Sunday = MySchedule.day7.Courses;
				}
				else
				{

					Day7List.Visibility = Visibility.Collapsed;
				}
			}
			catch (Exception ex)
			{

				Debug.WriteLine("赋值出错: " + ex.Message);
			}

			IsLoadingXml = false;

			Debug.WriteLine("-----end of intilization ");
		}


		private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			IsLoadingXml = true;
			Course c = (Course)e.ClickedItem;
			ListView listview = sender as ListView;
			string tag = listview.Tag.ToString();

			EditDialog Edit = new EditDialog(c);
			await Edit.ShowAsync();
			switch (Edit.Result)
			{
				case EditResult.cancel:
					break;
				case EditResult.save:

					var st = c.StartTime;
					var et = c.EndTime;


					if (MySchedule.day1 != null)
					{
						for (int i = 0; i < MySchedule.day1.Courses.Count; i++)
						{
							if (MySchedule.day1.Courses[i].Index == Edit.ChangedCourse.Index)
							{
								MySchedule.day1.Courses[i].StartTime = c.StartTime;
								MySchedule.day1.Courses[i].EndTime = c.EndTime;
								break;
							}
						}
					}

					if (MySchedule.day2 != null)
					{
						for (int i = 0; i < MySchedule.day2.Courses.Count; i++)
						{
							if (MySchedule.day2.Courses[i].Index == Edit.ChangedCourse.Index)
							{
								MySchedule.day2.Courses[i].StartTime = c.StartTime;
								MySchedule.day2.Courses[i].EndTime = c.EndTime;
								break;
							}
						}
					}
					if (MySchedule.day3 != null)
					{
						for (int i = 0; i < MySchedule.day3.Courses.Count; i++)
						{
							if (MySchedule.day3.Courses[i].Index == Edit.ChangedCourse.Index)
							{
								MySchedule.day3.Courses[i].StartTime = c.StartTime;
								MySchedule.day3.Courses[i].EndTime = c.EndTime;
								break;
							}
						}
					}
					if (MySchedule.day4 != null)
					{
						for (int i = 0; i < MySchedule.day4.Courses.Count; i++)
						{
							if (MySchedule.day4.Courses[i].Index == Edit.ChangedCourse.Index)
							{
								MySchedule.day4.Courses[i].StartTime = c.StartTime;
								MySchedule.day4.Courses[i].EndTime = c.EndTime;
								break;
							}
						}
					}
					if (MySchedule.day5 != null)
					{
						for (int i = 0; i < MySchedule.day5.Courses.Count; i++)
						{
							if (MySchedule.day5.Courses[i].Index == Edit.ChangedCourse.Index)
							{
								MySchedule.day5.Courses[i].StartTime = c.StartTime;
								MySchedule.day5.Courses[i].EndTime = c.EndTime;
								break;
							}
						}
					}
					if (MySchedule.day6 != null)
					{
						for (int i = 0; i < MySchedule.day6.Courses.Count; i++)
						{
							if (MySchedule.day6.Courses[i].Index == Edit.ChangedCourse.Index)
							{
								MySchedule.day6.Courses[i].StartTime = c.StartTime;
								MySchedule.day6.Courses[i].EndTime = c.EndTime;
								break;
							}
						}
					}

					if (MySchedule.day7 != null)
					{
						for (int i = 0; i < MySchedule.day7.Courses.Count; i++)
						{
							if (MySchedule.day7.Courses[i].Index == Edit.ChangedCourse.Index)
							{
								MySchedule.day7.Courses[i].StartTime = c.StartTime;
								MySchedule.day7.Courses[i].EndTime = c.EndTime;
								break;
							}
						}
					}

					
					StorageFolder folder = ApplicationData.Current.LocalFolder;

					//写入文件并保存，如果不成功。
					try
					{
						AppSettings MySetting = new AppSettings();
						var NewXmlFile = await folder.CreateFileAsync(MySetting.DefaultFileName, CreationCollisionOption.ReplaceExisting);


						using (Stream randAccStream = await NewXmlFile.OpenStreamForWriteAsync())
						{

							XmlSerializer serializer = new XmlSerializer(typeof(Schedule));
							serializer.Serialize(randAccStream, MySchedule);
						}



					}
					catch (Exception err)
					{

						await (new MessageDialog(err.Message + "这里出错了")).ShowAsync();
					}

					IsLoadingXml = false;
					await InitLoading();//重新加载
					break;
				default:
					break;
			}

		}

		private async void delete_Tapped(object sender, TappedRoutedEventArgs e)
		{
			Button btn = (Button)sender;
			string[] index = (string[])btn.Tag;

			var day = index[0];
			var classindex = index[1];

			StorageFolder folder = ApplicationData.Current.LocalFolder;
			StorageFile XmlFileInLocal;

			//读xml文件数据
			XmlFileInLocal = await folder.GetFileAsync("class");
			Stream XmlReader = await XmlFileInLocal.OpenStreamForReadAsync();
			XmlDocument XmlDoc = new XmlDocument();
			XmlDoc.Load(XmlReader);
			XmlReader.Dispose();

			foreach (var element in XmlDoc.DocumentElement)
			{
				XmlElement each_element = (XmlElement)element;
				if (each_element.Name == day)
				{
					foreach (var InnerElement in each_element)
					{
						XmlElement each_InnerElement = (XmlElement)InnerElement;
						if (each_InnerElement.GetAttribute("index") == classindex)
						{
							each_InnerElement.SetAttribute("finished", "1");
						}

					}
				}
			}

			////保存xml到local
			StorageFile xml = await folder.CreateFileAsync("class", CreationCollisionOption.ReplaceExisting);
			Stream fileWriter = await xml.OpenStreamForWriteAsync();
			XmlDoc.Save(fileWriter);
			fileWriter.Dispose();


			Monday.Clear(); Tuesday.Clear(); Thursday.Clear(); Wednesday.Clear(); Friday.Clear(); Saturday.Clear(); Sunday.Clear();

			await InitLoading();



		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.Parameter != null)
			{
				//StorageFile XmlFile = (StorageFile)e.Parameter;
				//ToastMessage.Toast("识别失败");
				try
				{
					StorageFile XmlFile = (StorageFile)e.Parameter;
					string s;
					using (Stream File = await XmlFile.OpenStreamForReadAsync())
					{
						using (StreamReader read = new StreamReader(File))
						{
							s = read.ReadToEnd();

						}
					}
					XmlSerializer xmlSearializer = new XmlSerializer(typeof(Schedule));

					using (StringReader sr = new StringReader(s))
					{
						MySchedule = (Schedule)xmlSearializer.Deserialize(sr);
					}
					//if (MySchedule.day1.Courses.Count<=0)
					//{
					//    ToastMessage.Toast("识别失败");
					//}
					AppSettings MySetting = new AppSettings();
					MySetting.DefaultFileName = XmlFile.Name;

				}
				catch (Exception ex)
				{

					ToastMessage.Toast("识别失败" + ex.Message);
				}
			}
			await InitLoading();//初始化


		}


		//public void BringToView()
		//{
		//	switch (DateTime.Now.DayOfWeek)
		//	{

		//		case DayOfWeek.Monday:
		//			Day1List.StartBringIntoView();
			
		//			break;
		//		case DayOfWeek.Tuesday:
		//			Day2List.StartBringIntoView();
		//			break;
		//		case DayOfWeek.Wednesday:
		//			Day3List.StartBringIntoView();
		//			break;
		//		case DayOfWeek.Thursday:
		//			Day4List.StartBringIntoView();
		//			break;
		//		case DayOfWeek.Saturday:
		//			Day5List.StartBringIntoView();
		//			break;
		//		case DayOfWeek.Friday:
		//			Day6List.StartBringIntoView();
		//			break;
		//		case DayOfWeek.Sunday:
		//			Day7List.StartBringIntoView();
		//			break;



		//		default:
		//			break;
		//	}
		//}

		private async void ToggleButton_Checked(object sender, RoutedEventArgs e)
		{


			StorageFolder folder = ApplicationData.Current.LocalFolder;

			//写入文件并保存，如果不成功。
			try
			{
				AppSettings MySetting = new AppSettings();
				var NewXmlFile = await folder.CreateFileAsync(MySetting.DefaultFileName, CreationCollisionOption.ReplaceExisting);


				using (Stream randAccStream = await NewXmlFile.OpenStreamForWriteAsync())
				{

					XmlSerializer serializer = new XmlSerializer(typeof(Schedule));
					serializer.Serialize(randAccStream, MySchedule);
				}



			}
			catch (Exception err)
			{

				await(new MessageDialog(err.Message + "这里出错了")).ShowAsync();
			}
		}
	}

}
