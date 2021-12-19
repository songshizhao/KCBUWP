using BackgroundTasks;
using EasyNote.Helper;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace 课程表UWP
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application
    {


        public static string backgroundTaskName = "Reminder";
        public static bool IsLogin = false;
        public static string Username = "";
        public static string Password = "";

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            //处理手机后退键
            bool isHardwareButtonsAPIPresent = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
            if (isHardwareButtonsAPIPresent)
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        //按后退键后退
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }
        }


        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
			SetBackgroundTask();

			switch (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
			{
				case "Windows.Desktop":
					break;
				case "Windows.Mobile":
					StatusBar StatusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
					await StatusBar.HideAsync();
					break;
				case "Windows.Tablet":
					break;
				case "Windows.SurfaceHub":
					break;
				case "Windows.Xbox":
					break;
				case "Windows.Iot":
					break;
				default:
					break;
			}


            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {

                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
                rootFrame.Navigated += Show_Navigation;
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    //rootFrame.Navigate(typeof(SchedulePage), e.Arguments);
					rootFrame.Navigate(typeof(MainPage), e.Arguments);
				}
				// Ensure the current window is active
				Window.Current.Activate();
            }
        }

        /// <summary>
        /// App_BackRequested
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            // 这里面可以任意选择控制哪个Frame 
            // 如果MainPage.xaml中使用了另外的Frame标签进行导航 可在此处获取需要GoBack的Frame
            var rootFrame = Window.Current.Content as Frame;
            // ReSharper disable once PossibleNullReferenceException
            if (!rootFrame.CanGoBack) return;
            rootFrame.GoBack();
            // 设置指示应用程序已执行请求的后退导航操作
            e.Handled = true;
        }

        /// <summary>
        /// Show_Navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Show_Navigation(object sender, NavigationEventArgs e)
        {
            // 每次完成导航 确定下是否显示系统后退按钮
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
               (Window.Current.Content as Frame).BackStack.Any()
               ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;


        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }


        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }






		private async void SetBackgroundTask()
		{
			try
			{
				//是否进行过注册
				bool taskRegistered = false;
				//后台任务名称

				var status = await BackgroundExecutionManager.RequestAccessAsync();
				if (status == BackgroundAccessStatus.Unspecified | status == BackgroundAccessStatus.DeniedByUser | status == BackgroundAccessStatus.DeniedBySystemPolicy)
				{
					ToastMessage.Toast("后台任务权限被禁止");
					return;
				}

				//判断是否已经注册过后台任务
				foreach (var task in BackgroundTaskRegistration.AllTasks)
				{
					Debug.WriteLine("****Task名字：" + task.Value.Name);
					if (task.Value.Name == backgroundTaskName)
					{
						taskRegistered = true;
						//UnregisterBackgroundTask();
						//Debug.WriteLine("****已卸载后台任务：" + task.Value.Name);
						//taskRegistered = false;
						//break;
					}
				}


				//            //如果注册过
				//            if (taskRegistered)
				//            {
				//                UnregisterBackgroundTask();
				//#if DEBUG
				//                ToastMessage.Toast("已卸载后台任务" + DateTime.Now.Second);
				//#endif
				//            }

				//如果没有注册过后台任务
				if (!taskRegistered)
				{
					var builder = new BackgroundTaskBuilder
					{
						Name = backgroundTaskName,
						TaskEntryPoint = "BackgroundTasks.Reminder"
					};
					var quarterTimer = new TimeTrigger(15, false);
					builder.SetTrigger(quarterTimer);
					//await RegisterBackgroundTask("BackgroundTasks.Reminder", "Reminder", new TimeTrigger(30, false), null);
					BackgroundTaskRegistration task = builder.Register();
					Debug.WriteLine("****已z注册后台任务：" + task.Name);
				}

				//var builder = new BackgroundTaskBuilder();
				//builder.Name = exampleTaskName;
				//builder.TaskEntryPoint = "MyRuntimeComponent.Notifications";
				//builder.SetTrigger(new TimeTrigger(15, false));
				////builder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));
				//BackgroundTaskRegistration registration = builder.Register();
			}
			catch (Exception ex)
			{
				ToastMessage.Toast("注册后台任务出现错误:" + ex.Message);
			}
		}
		private static void UnregisterBackgroundTask()
		{
			var task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(i => i.Name.Equals(backgroundTaskName));

			if (task != null)
				task.Unregister(true);
		}

		public static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(string taskEntryPoint,
																string taskName,
																IBackgroundTrigger trigger,
																IBackgroundCondition condition)
		{
			var status = await BackgroundExecutionManager.RequestAccessAsync();
			if (status == BackgroundAccessStatus.Unspecified || status == BackgroundAccessStatus.DeniedByUser)
			{
				return null;
			}

			foreach (var cur in BackgroundTaskRegistration.AllTasks)
			{
				if (cur.Value.Name == taskName)
				{
					cur.Value.Unregister(true);
				}
			}

			var builder = new BackgroundTaskBuilder
			{
				Name = taskName,
				TaskEntryPoint = taskEntryPoint,
			};

			builder.SetTrigger(trigger);

			if (condition != null)
			{
				builder.AddCondition(condition);
			}

			BackgroundTaskRegistration task = builder.Register();

			Debug.WriteLine($"Task {taskName} registered successfully.");

			return task;
		}




	}
}
