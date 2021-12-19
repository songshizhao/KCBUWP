using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace BackgroundTasks
{

	public sealed class Reminder : IBackgroundTask
    {
		//System.Runtime.InteropServices.SpinWaitExtensions;//.LayoutKind sd;

		BackgroundTaskDeferral _deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {

			try
			{
				Debug.WriteLine("启动了后台任务Reminder");
				_deferral = taskInstance.GetDeferral();
#if DEBUG
				//ToastMessage.Toast("正在运行后台任务");
#endif

				//获取最近要上的课
				var newCourse = await GetNearestCourse();
				if (newCourse != null)
				{
					TimeSpan time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Second, 0);
					var hour = newCourse.StartTime.Substring(0, 2);
					var minute = newCourse.StartTime.Substring(3, 2);
					TimeSpan NearestClassTime = new TimeSpan(Convert.ToInt32(hour), Convert.ToInt32(minute), 0);
					if (NearestClassTime.CompareTo(time) >= 0 && newCourse.Name.Length > 0)
					{
						AlertClass(newCourse);
					}

				}

				_deferral.Complete();
			}
			catch (Exception)
			{

			}
        }




        private void AlertClass(Course newCourse)
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            //if (!root.Values.TryGetValue("EnableNotify", out object value))
            //{
            //    Debug.WriteLine("读取默认设置失败，可能未设置是否提示通知");
            //    return ;
            //}
            //else
            //{
            //    Debug.WriteLine("读取默认设置成功");
            //}


            TimeSpan time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Second, 0);
            var hour = newCourse.StartTime.Substring(0, 2);
            var minute = newCourse.StartTime.Substring(3, 2);
            TimeSpan NearestClassTime = new TimeSpan(Convert.ToInt32(hour), Convert.ToInt32(minute), 0);


			if (newCourse.Name == "")
			{
				Debug.WriteLine("课表名称为空,不提示");
				return;
			}
			if (newCourse.Type == 0)
			{
				Debug.WriteLine("课表没有设置提醒");
				return;
			}

			var msg = $"{newCourse.StartTime} 在 {newCourse.Room} 有课: {newCourse.Name}";

			ToastMessage.Toast(msg);

			//更新动态磁贴
			UpdateTiles(newCourse);

        }



		private Course getCourse([ReadOnlyArray()]  Course[] Courses) {
			Course NearestCourse = null;

			//当前时间
			DateTime time = DateTime.Now;
			for (int i = 0; i < Courses.Length; i++)
			{

				Debug.WriteLine("课程名称: "+Courses[i].Name + ": -------");

				var start_hour = Courses[i].StartTime.Substring(0, 2);
				var start_minute = Courses[i].StartTime.Substring(3, 2);
				DateTime courseTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
					Convert.ToInt32(start_hour), Convert.ToInt32(start_minute), 0);
				var leftminute = (courseTime - time).TotalMinutes;

				//如果课程时间晚于当前时间=还没上课
				if (leftminute >= 0)
				{
					Debug.WriteLine("还没上课");
					if (leftminute <= 30)
					{
						NearestCourse = Courses[i];
						Debug.WriteLine("30分钟之内");
					}
					else
					{
						Debug.WriteLine("超过30分钟");
					}
				}
				else
				{
					Debug.WriteLine("时间已过");
				}
			}

			return NearestCourse;
		
		}


        private async Task<Course> GetNearestCourse()
        {
            try
            {
                //找到本地文件夹
                StorageFolder folder = ApplicationData.Current.LocalFolder;

                ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
                //读取当前应用的课程表文件名
                if (!root.Values.TryGetValue("DefaultFileName", out object value))
                {
                    value = "class";
                }
                string filename = value.ToString();
                Debug.WriteLine("读取的课表文件名字为："+ filename);
                //读xml文件
                StorageFile XmlFile = await folder.GetFileAsync(filename);
                string s;
                using (Stream File = await XmlFile.OpenStreamForReadAsync())
                {
                    using (StreamReader read = new StreamReader(File))
                    {
                        s = read.ReadToEnd();

                    }
                }



                XmlSerializer xmlSearializer = new XmlSerializer(typeof(Schedule));
                Schedule MySchedule;
                using (StringReader sr = new StringReader(s))
                {
                    MySchedule = (Schedule)xmlSearializer.Deserialize(sr);
                }
				Debug.WriteLine("读取并反序列...");
				//当前时间
				DateTime time = DateTime.Now;


                Course[] Courses;
                Course NearestCourse=null;
                switch (DateTime.Now.DayOfWeek)
                {
                    case DayOfWeek.Friday:
                        Courses = MySchedule.day5.Courses;
						NearestCourse = getCourse(Courses);
						break;
                    case DayOfWeek.Monday:
                        Courses = MySchedule.day1.Courses;
						NearestCourse = getCourse(Courses);
						break;
                    case DayOfWeek.Saturday:
                        Courses = MySchedule.day6.Courses;
						NearestCourse = getCourse(Courses);
						break;
                    case DayOfWeek.Sunday:
                        Courses = MySchedule.day7.Courses;
						NearestCourse = getCourse(Courses);
						break;
                    case DayOfWeek.Thursday:
                        Courses = MySchedule.day4.Courses;
						NearestCourse = getCourse(Courses);
						break;
                    case DayOfWeek.Tuesday:
                        Courses = MySchedule.day2.Courses;
						NearestCourse = getCourse(Courses);
						break;
                    case DayOfWeek.Wednesday:
                        Courses = MySchedule.day3.Courses;
						NearestCourse = getCourse(Courses);
						break;
                    default:
                        break;
                }


				Debug.WriteLine("课表名字"+NearestCourse.Name);
                return NearestCourse;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("获取选取课程出错" + ex.Message);

				//ToastMessage.Toast("获取选取课程出错"+ex.Message);
				return null;
            }

        }



        private void UpdateTiles(Course newCourse)
        {

            string imgurl = "ms-appx:///images/bg2.jpg";
            string title = "有" + newCourse.Name + "在" + newCourse.Room + "需要参加 " + "@" + newCourse.StartTime;
			string description = "有" + newCourse.Name + "在" + newCourse.Room + "需要参加 " + "@" + newCourse.StartTime;


            string content = $@"
<tile branding='name'> 
  <visual version='3'>
    <binding template='TileMedium'>
      <image src='{imgurl}' placement='peek'/>
      <text>{title}</text>
      <text hint-style='captionsubtle' hint-wrap='true'>{description}</text>
    </binding>
    <binding template='TileWide'>
      <image src='{imgurl}' placement='peek'/>
      <text>{title}</text>
      <text hint-style='captionsubtle' hint-wrap='true'>{description}</text>
    </binding>
    <binding template='TileLarge'>
      <image src='{imgurl}' placement='peek'/>
      <text>{title}</text>
      <text hint-style='captionsubtle' hint-wrap='true'>{description}</text>
    </binding>
  </visual>
</tile>";


            // Load the string into an XmlDocument
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            // Then create the tile notification
            var notification = new TileNotification(doc);
            notification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

            if (SecondaryTile.Exists("MySecondaryTile"))
            {
                // Get its updater
                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile("MySecondaryTile");

                // And send the notification
                updater.Update(notification);
            }



        }






    }





    public sealed class ToastMessage
    {

        public static void Toast(string msg)
        {
		
			//1. create element
			ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            //2.设置消息文本
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(msg));

			//3. 图标
			XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("src", $"ms-appx:///images/bg2.jpg");
            //((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "logo");
            // 4. duration
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "short");

            // 5. audio
            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", $"ms-winsoundevent:Notification.Reminder");
            toastNode.AppendChild(audio);

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }





		//public static void PopToast()
		//{
		//	// Generate the toast notification content and pop the toast
		//	ToastContent content=null;// = GenerateToastContent();
		//	ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
		//}

		//public static void PopToast2(string classname, string time, string room)
		//{
		//	// Generate the toast notification content and pop the toast
		//	ToastContent content = GenerateToastContent2(classname, time, room);
		//	ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
		//}




		//public static ToastContent GenerateToastContent2(string classname, string time, string room)
		//{
		//	return new ToastContent()
		//	{
		//		Launch = "action=viewEvent&eventId=1983",
		//		Scenario = ToastScenario.Reminder,

		//		Visual = new ToastVisual()
		//		{
		//			BindingGeneric = new ToastBindingGeneric()
		//			{

		//				Children =
		//				{
		//					new AdaptiveText()
		//					{
		//						Text = classname
		//					},
		//					new AdaptiveText()
		//					{
		//						Text = time
		//					},
		//					new AdaptiveText()
		//					{
		//						Text = room
		//					}
		//				}
		//			}
		//		},
		//		Actions = new ToastActionsCustom()
		//		{
		//			Inputs =
		//			{
		//				new ToastSelectionBox("snoozeTime")
		//				{
		//					DefaultSelectionBoxItemId = "15",
		//					Items =
		//					{
		//						new ToastSelectionBoxItem("1", "1 minute"),
		//						new ToastSelectionBoxItem("5", "15 minutes"),
		//						new ToastSelectionBoxItem("10", "15 minutes"),
		//						new ToastSelectionBoxItem("15", "15 minutes"),
		//						new ToastSelectionBoxItem("20", "15 minutes"),
		//						new ToastSelectionBoxItem("30", "1 hour"),
		//						//new ToastSelectionBoxItem("240", "4 hours"),
		//						//new ToastSelectionBoxItem("1440", "1 day")
		//					}
		//				}
		//			},

		//			Buttons =
		//			{
		//				new ToastButtonSnooze()
		//				{
		//					SelectionBoxId = "snoozeTime"

		//				},
		//				new ToastButtonDismiss()

		//			}
		//		}
		//	};
		//}

		//public static ToastContent GenerateToastContent()
		//{
		//	return new ToastContent()
		//	{
		//		Launch = "action=viewEvent&eventId=1983",
		//		Scenario = ToastScenario.Reminder,

		//		Visual = new ToastVisual()
		//		{
		//			BindingGeneric = new ToastBindingGeneric()
		//			{

		//				Children =
		//				{
		//					new AdaptiveText()
		//					{
		//						Text = "Adaptive Tiles Meeting"
		//					},
		//					new AdaptiveText()
		//					{
		//						Text = "Conf Room 2001 / Building 135"
		//					},
		//					new AdaptiveText()
		//					{
		//						Text = "10:00 AM - 10:30 AM"
		//					}
		//				}
		//			}
		//		},
		//		Actions = new ToastActionsCustom()
		//		{
		//			Inputs =
		//			{
		//				new ToastSelectionBox("snoozeTime")
		//				{
		//					DefaultSelectionBoxItemId = "15",
		//					Items =
		//					{
		//						new ToastSelectionBoxItem("1", "1 minute"),
		//						new ToastSelectionBoxItem("15", "15 minutes"),
		//						new ToastSelectionBoxItem("60", "1 hour"),
		//						//new ToastSelectionBoxItem("240", "4 hours"),
		//						//new ToastSelectionBoxItem("1440", "1 day")
		//					}
		//				}
		//			},

		//			Buttons =
		//			{
		//				new ToastButtonSnooze()
		//				{
		//					SelectionBoxId = "snoozeTime"

		//				},
		//				new ToastButtonDismiss()

		//			}
		//		}
		//	};
		//}


	}






}
