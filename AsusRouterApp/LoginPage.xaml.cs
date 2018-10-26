using AsusRouterApp.Class;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace AsusRouterApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public string host { get; set; } = "http://";

        public string user { get; set; } = "";

        public string pwd { get; set; } = "";

        public LoginPageBrush pageBrush;

        PnpObjectWatcher deviceWatcher;

        public LoginPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
            pageBrush = new LoginPageBrush(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            deviceWatcher =PnpObject.CreateWatcher(PnpObjectType.DeviceInterface, new string[] { "System.Devices.DeviceInstanceId", "System.Devices.DeviceManufacturer", "System.ItemNameDisplay" });
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Start();
        }

        private void DeviceWatcher_Added(PnpObjectWatcher sender, PnpObject args)
        {
            var asdf=args.Properties.ToArray();
            System.Diagnostics.Debug.WriteLine("ItemNameDisplay:" + (string)args.Properties["System.ItemNameDisplay"]);
        }

        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                LoginBtn_Click(null,null);
            }
        }

        /// <summary>
        /// Fluent Design System兼容方案
        /// </summary>
        public class LoginPageBrush : INotifyPropertyChanged
        {
            public Brush mainGrid;
            public Brush btnGrid;

            public Utils.UI.AccentColor accentColor;

            public LoginPageBrush(Page page)
            {
                accentColor = new Utils.UI.AccentColor();
                accentColor.AccentColorChanged += async (value) =>
                {
                    await page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,() =>
                    {
                        if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush"))
                        {
                            //整个页面采用FDS
                            mainGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, accentColor.accentColor, Color.FromArgb(255, 70, 171, 255), 0.3);
                            btnGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, accentColor.accentColor, Color.FromArgb(255, 70, 171, 255), 0.6);
                        }
                        else
                        {
                            btnGrid = new SolidColorBrush(accentColor.accentColor);
                        }
                        RaisePropertyChanged("mainGrid");
                        RaisePropertyChanged("btnGrid");
                    });
                };
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush"))
                {
                    //整个页面采用FDS
                    mainGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, accentColor.accentColor, Color.FromArgb(255, 70, 171, 255), 0.3);
                    btnGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, accentColor.accentColor, Color.FromArgb(255, 70, 171, 255), 0.6);
                }
                else
                {
                    var imageSource = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/background.jpg") };
                    mainGrid = new ImageBrush() { ImageSource = imageSource, Stretch = Stretch.UniformToFill };
                    btnGrid = new SolidColorBrush(accentColor.accentColor);
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void RaisePropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!host.Contains("http"))
            {
                notificationError.Show(Utils.AppResources.GetString("HostError"));
                return;
            }
            if (user.Length==0)
            {
                notificationError.Show(Utils.AppResources.GetString("NullUserName"));
                return;
            }
            if (pwd.Length == 0)
            {
                notificationError.Show(Utils.AppResources.GetString("NullPassword"));
                return;
            }
            RouterAPI.Url.Host = host;
            var loginRes = await RouterAPI.Login(user,pwd);
            if (loginRes)
                this.Frame.Navigate(typeof(MainPage), null);
            else
                notificationError.Show(Utils.AppResources.GetString("LoginFailure"), 2000);
        }
    }
}
