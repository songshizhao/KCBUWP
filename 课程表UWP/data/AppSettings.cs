using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace 课程表UWP.data
{
    public class AppSettings
    {

        public AppSettings() { }

        //位置信息-经度
        public double Latitude
        {
            get
            {
                return ReadSetting<double>("Latitude");
            }
            set
            {
                WriteSetting<double>("Latitude", value);
            }
        }
        //位置信息-纬度
        public double Longitude
        {
            get
            {
                return ReadSetting<double>("Longitude");
            }
            set
            {
                WriteSetting<double>("Longitude", value);
            }

        }




        /// <summary>
        /// 写入存储信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="Tvalue"></param>
        public static void WriteSetting<T>(string key, T Tvalue)// where T : struct
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            if (root.Values.TryGetValue(key, out object oldkey))
            {
                root.Values[key] = Tvalue;
            }
            else
            {
                root.Values.Add(key, Tvalue);
            }
        }
        /// <summary>
        /// 有条件的写入存储信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="Tvalue"></param>
        public static void WriteSettingIfBigger<T>(string key, T Tvalue)
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            if (root.Values.TryGetValue(key, out object oldvalue))
            {
                if (Convert.ToDouble(oldvalue) < Convert.ToDouble(Tvalue))
                {
                    root.Values[key] = Tvalue;
                }
            }
            else
            {
                root.Values.Add(key, Tvalue);
            }
        }

        /// <summary>
        /// 读取存储信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T ReadSetting<T>(string key)
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;

            if (root.Values.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            else
            {
                return default(T);
            }
        }







        private bool FirstStart;
        public bool IsAppFirstStart
        {
            get
            {
                object Setting = ReadSetting("FirstStart");
                if (Setting != null)
                {
                    FirstStart = Convert.ToBoolean(Setting);
                }
                else
                {
                    FirstStart = true;
                    WriteSetting_OverridesAllways("FirstStart", false);
                }
                return FirstStart;
            }
            set
            {
                FirstStart = value;//设置数据，并写入设置
                WriteSetting_OverridesAllways("FirstStart", FirstStart);//音量不管任何时候都进行重写
            }
        }



        private bool _EnableNotify;
        public bool EnableNotify
        {
            get
            {
                object Setting = ReadSetting("EnableNotify");
                if (Setting != null)
                {
                    //能找到存储的设置项
                    _EnableNotify = Convert.ToBoolean(Setting);
                }
                else
                {
                    //找不到存储的设置项，默认为false
                    _EnableNotify = false;
                    WriteSetting_OverridesAllways("EnableNotify", false);
                }
                return _EnableNotify;
            }
            set
            {
                //将value存储进入设置项
                _EnableNotify = value;
                WriteSetting_OverridesAllways("EnableNotify", value);//音量不管任何时候都进行重写
            }
        }




        //打开的默认文档
        private string defaultFileName;
        public string DefaultFileName
        {
            get
            {
                object Setting = ReadSetting("DefaultFileName");
                if (Setting != null)
                {
                    defaultFileName = Convert.ToString(Setting);
                }
                else
                {
                    defaultFileName = "class";
                }
                return defaultFileName;
            }
            set
            {
                defaultFileName = value;
                WriteSetting_OverridesAllways("DefaultFileName", value);
            }
        }





        private string userName;
        //用户名
        public string UserName//
        {
            get
            {
                object Setting = ReadSetting("UserName");
                if (Setting != null)
                {
                    userName = Convert.ToString(Setting);
                }
                else
                {
                    userName = "";
                }
                return userName;
            }
            set
            {
                userName = value;
                WriteSetting_OverridesAllways("UserName", value);
            }
        }

        //是否保存用户名
        private bool saveUserName;
        public bool SaveUserName
        {
            get
            {
                object Setting = ReadSetting("SaveUserName");
                if (Setting != null)
                {
                    saveUserName = (bool)Setting;
                }
                else
                {
                    saveUserName = false;
                }
                return saveUserName;
            }
            set
            {
                saveUserName = value;
                WriteSetting_OverridesAllways("SaveUserName", value);
            }
        }


        private int perferWidth;

        public int PerferWidth
        {
            get
            {

                object Setting = ReadSetting("PerferWidth");
                if (Setting != null)
                {
                    perferWidth = (int)Setting;
                }
                else
                {
                    perferWidth = 400;
                }
                return perferWidth;
            }
            set
            {
                perferWidth = value;
                WriteSetting_OverridesAllways("PerferWidth", value);
            }
        }


        private int perferHeight;

        public int PerferHeight
        {
            get
            {

                object Setting = ReadSetting("PerferHeight");
                if (Setting != null)
                {
                    perferHeight = (int)Setting;
                }
                else
                {
                    perferHeight = 300;
                }
                return perferHeight;
            }
            set
            {
                perferHeight = value;
                WriteSetting_OverridesAllways("PerferHeight", value);
            }
        }




        public object ReadSetting(string key)
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            //读取历史数据
            if (root.Values.TryGetValue(key, out object value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }
        //如果设置项存在，覆盖设置项，不存在则创建设置项
        public void WriteSetting_OverridesAllways(string key, object value)
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            if (root.Values.TryGetValue(key, out object oldkey))
            {
                root.Values[key] = value;
            }
            else
            {
                root.Values.Add(key, value);
            }
        }
        //如果设置项存在，比较设置项大小，如果较大则填入设置项，不存在则创建设置项
        public void WriteSetting_OverridesIfBigger(string key, object value)
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            if (root.Values.TryGetValue(key, out object oldvalue))
            {
                if (Convert.ToDouble(oldvalue) < Convert.ToDouble(value))
                {
                    root.Values[key] = value;
                }
            }
            else
            {
                root.Values.Add(key, value);
            }
        }




		private string backgroundImgToken;
		//用户名
		public string BackgroundImgToken//
		{
			get
			{
				object Setting = ReadSetting("BackgroundImgToken");
				if (Setting != null)
				{
					backgroundImgToken = Convert.ToString(Setting);
				}
				else
				{
					backgroundImgToken = "";
				}
				return backgroundImgToken;
			}
			set
			{
				backgroundImgToken = value;
				WriteSetting_OverridesAllways("BackgroundImgToken", value);
			}
		}

	}
}
