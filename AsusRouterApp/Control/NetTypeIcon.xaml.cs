using AsusRouterApp.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace AsusRouterApp.Control
{
    public sealed partial class NetTypeIcon : UserControl
    {
        public static readonly DependencyProperty NetTypeProperty = DependencyProperty.Register("NetType", typeof(Model.Client.NetType), typeof(NetTypeIcon), null);

        public Model.Client.NetType NetType
        {
            get { return (Model.Client.NetType)GetValue(NetTypeProperty); }
            set {SetValue(NetTypeProperty, value);}
        }

        public ImageSource ImageUrl
        {
            get
            {
                try
                {
                    string path = "ms-appx:///Assets/icon/";
                    switch (NetType)
                    {
                        case Model.Client.NetType.LAN:
                            path += "icon_signal_wired.png";
                            break;
                        case Model.Client.NetType.WL2G:
                            path += "icon_signal_24g.png";
                            break;
                        case Model.Client.NetType.WL5G:
                            path += "icon_signal_5g.png";
                            break;
                        default:
                            path += "icon_signal_wired.png";
                            break;
                    }
                    return new BitmapImage() { UriSource = new Uri(path) };
                }
                catch (Exception)
                {
                    return new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/icon/icon_signal_wired.png") };
                }
;            }
        }

        public NetTypeIcon()
        {
            this.InitializeComponent();
        }
    }
}
