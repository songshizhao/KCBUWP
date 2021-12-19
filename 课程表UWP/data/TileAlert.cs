using EasyNote.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using System.Xml.Serialization;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using System.Collections.ObjectModel;
using BackgroundTasks;

namespace 课程表UWP.data
{

//    public sealed class TileAlert
//    {

//        //TileUpdateManager.CreateTileUpdaterForApplication().Clear();




//        public static void SetupTileAlert(string f,string s,string b)
//        {
//            // In a real app, these would be initialized with actual data
//            string from = f;
//            string subject = s;
//            string body = b;


//            // TODO - all values need to be XML escaped


//            // Construct the tile content as a string
//            string content = $@"
//<tile>
//    <visual>

//        <binding template='TileMedium'>
//            <text>{from}</text>
//            <text hint-style='captionSubtle'>{subject}</text>
//            <text hint-style='captionSubtle'>{body}</text>
//        </binding>

//        <binding template='TileWide'>
//            <text hint-style='subtitle'>{from}</text>
//            <text hint-style='captionSubtle'>{subject}</text>
//            <text hint-style='captionSubtle'>{body}</text>
//        </binding>

//    </visual>
//</tile>";

//            // Load the string into an XmlDocument
//            XmlDocument doc = new XmlDocument();
//            doc.LoadXml(content);

//            // Then create the tile notification
//            var notification = new TileNotification(doc);
//            notification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);
//            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

//            if (SecondaryTile.Exists("MySecondaryTile"))
//            {
//                // Get its updater
//                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile("MySecondaryTile");

//                // And send the notification
//                updater.Update(notification);
//            }
//        }
//        public async void SetupTileAlert2()
//        {

//            Course newCourse = await GetNearestCourse();
//            if (newCourse.Name=="")
//            {
//                return;
//            }
//            string imgurl = "ms-appx:///images/bg2.jpg";
//            string title = newCourse.Name+"提醒";
//            string description ="接下来"+ newCourse.StartTime + "需要参加哦！😄"+newCourse.Room;


//            string content = $@"
//<tile branding='name'> 
//  <visual version='3'>
//    <binding template='TileMedium'>
//      <image src='{imgurl}' placement='peek'/>
//      <text>{title}</text>
//      <text hint-style='captionsubtle' hint-wrap='true'>{description}</text>
//    </binding>
//    <binding template='TileWide'>
//      <image src='{imgurl}' placement='peek'/>
//      <text>{title}</text>
//      <text hint-style='captionsubtle' hint-wrap='true'>{description}</text>
//    </binding>
//    <binding template='TileLarge'>
//      <image src='{imgurl}' placement='peek'/>
//      <text>{title}</text>
//      <text hint-style='captionsubtle' hint-wrap='true'>{description}</text>
//    </binding>
//  </visual>
//</tile>";





//            // Load the string into an XmlDocument
//            XmlDocument doc = new XmlDocument();
//            doc.LoadXml(content);

//            // Then create the tile notification
//            var notification = new TileNotification(doc);
//            notification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);
//            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

//            if (SecondaryTile.Exists("MySecondaryTile"))
//            {
//                // Get its updater
//                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile("MySecondaryTile");

//                // And send the notification
//                updater.Update(notification);
//            }

//        }










//        private async Task<Course> GetNearestCourse()
//        {
//            try
//            {
//                //找到本地文件夹
//                StorageFolder folder = ApplicationData.Current.LocalFolder;
//                string filename = (new AppSettings()).DefaultFileName;
//                //读xml文件
//                StorageFile XmlFile = await folder.GetFileAsync(filename);
//                string s;
//                using (Stream File = await XmlFile.OpenStreamForReadAsync())
//                {
//                    using (StreamReader read = new StreamReader(File))
//                    {
//                        s = read.ReadToEnd();

//                    }
//                }
//                XmlSerializer xmlSearializer = new XmlSerializer(typeof(Schedule));
//                Schedule MySchedule;
//                using (StringReader sr = new StringReader(s))
//                {
//                    MySchedule = (Schedule)xmlSearializer.Deserialize(sr);
//                }
//                //当前时间
//                TimeSpan time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

//				ObservableCollection<Course> Courses =new ObservableCollection<Course>();
//                switch (DateTime.Now.DayOfWeek)
//                {
//                    case DayOfWeek.Friday:
//                        Courses = MySchedule.day5.Courses;
//                        break;
//                    case DayOfWeek.Monday:
//                        Courses = MySchedule.day1.Courses;
//                        break;
//                    case DayOfWeek.Saturday:
//                        Courses = MySchedule.day6.Courses;
//                        break;
//                    case DayOfWeek.Sunday:
//                        Courses = MySchedule.day7.Courses;
//                        break;
//                    case DayOfWeek.Thursday:
//                        Courses = MySchedule.day4.Courses;
//                        break;
//                    case DayOfWeek.Tuesday:
//                        Courses = MySchedule.day2.Courses;
//                        break;
//                    case DayOfWeek.Wednesday:
//                        Courses = MySchedule.day3.Courses;
//                        break;
//                    default:
//                        break;
//                }
//                Course NearestCourse= Courses[0];
//                //for (int i = 0; i < Courses.Count; i++)
//                //{
//                //    var hour = Courses[i].StartTime.Substring(0, 2);
//                //    var minute = Courses[i].StartTime.Substring(3, 2);
//                //    TimeSpan classtime = new TimeSpan(Convert.ToInt32(hour), Convert.ToInt32(minute), 0);

//                //}
//                foreach (var item in Courses)
//                {
//                    var start_hour = item.StartTime.Substring(0, 2);
//                    var start_minute = item.StartTime.Substring(3, 2);
//                    TimeSpan classtime = new TimeSpan(Convert.ToInt32(start_hour), Convert.ToInt32(start_minute), 0);
//                    //如果课程时间晚于当前时间=还没上课
//                    if (classtime.CompareTo(time)>=0)
//                    {
//                        var hour = NearestCourse.StartTime.Substring(0, 2);
//                        var minute = NearestCourse.StartTime.Substring(3, 2);
//                        TimeSpan NearestClassTime = new TimeSpan(Convert.ToInt32(hour), Convert.ToInt32(minute), 0);
//                        if (classtime.CompareTo(NearestClassTime) <= 0)
//                        {
//                            //如果这个课程时间近于 当前最近的还没上的课 
//                            NearestCourse = item;
//                        }
//                        break;
//                    }
//                }

//                return NearestCourse;
//            }
//            catch (Exception ex)
//            {
//                ToastMessage.Toast(ex.Message);
//                return null;
//            }
            
//        }

//        #region 辅助函数
//        //文件是否存在
//        public async Task<bool> IsFileExist(string fileName)
//        {
//            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
//            return item != null;
//        }
//        #endregion













//    }


}