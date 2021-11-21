using AsusRouterApp.Class;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
        public bool Protocol_Http { get; set; } = true;

        public bool Protocol_Https { get; set; } = false;

        public string host { get; set; } = "";

        public string user { get; set; } = "";

        public string pwd { get; set; } = "";

        public ObservableCollection<string> gateways = new ObservableCollection<string>();

        public LoginPageBrush pageBrush;

        public LoginPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
            pageBrush = new LoginPageBrush(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Controls.AppBarElementContainer"))
            {
                var networks = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var network in networks)
                {
                    var properties = network.GetIPProperties();
                    if (properties != null && properties.GatewayAddresses != null)
                    {
                        foreach (var gateway in properties.GatewayAddresses)
                        {
                            if(!gateway.Address.IsIPv6LinkLocal)gateways.Add(gateway.Address.ToString());
                        }
                    }
                }
                if (gateways.Count > 0) host = gateways[0];
            }
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
            string url = "";
            if (Protocol_Http)
                url += "http://";
            else
                url += "https://";
            url += host;
            var ipres = Uri.TryCreate(url,UriKind.Absolute,out Uri uri);
            if(!ipres)
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
            RouterAPI.Url.Host = url;
            var loginRes = await RouterAPI.Login(user,pwd);
            if (loginRes)
                this.Frame.Navigate(typeof(MainPage), null);
            else
                notificationError.Show(Utils.AppResources.GetString("LoginFailure"), 2000);
        }
    }
}
