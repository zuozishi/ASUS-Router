using AsusRouterApp.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace AsusRouterApp.Control
{
    public sealed partial class ClientDialog : ContentDialog
    {
        public Model.Client.ClientInfo client { get; set; }

        public ClientDialogBrush dialogBrush { get; set; }

        public List<TypeModel> typeModels { get; set; }

        public bool qos_enable { get; set; } = false;

        public long qos_down { get; set; } = 100;

        public long qos_up { get; set; } = 100;

        public int SelectedIndex { get; set; }

        /// <summary>
        /// Fluent Design System兼容方案
        /// </summary>
        public class ClientDialogBrush
        {

            public Brush mainGrid;
            public Brush textBrush;

            public ClientDialogBrush()
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush"))
                {
                    //整个页面采用FDS
                    mainGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, Edi.UWP.Helpers.UI.GetAccentColor(), Color.FromArgb(255, 70, 171, 255), 0.3);
                    textBrush = new SolidColorBrush(Colors.White);
                }
                else
                {
                    //主页面白色
                    mainGrid = new SolidColorBrush(Color.FromArgb(255, 70, 171, 255));
                    textBrush = new SolidColorBrush(Colors.Black);
                }
            }
        }

        public class TypeModel
        {
            public BitmapImage image
            {
                get
                {
                    string path = "ms-appx:///Assets/icon/device/";
                    switch (type)
                    {
                        case Model.DeviceType.Laptop:
                            path += "device_laptop.png";
                            break;
                        case Model.DeviceType.MAC:
                            path += "device_mac.png";
                            break;
                        case Model.DeviceType.PAD:
                            path += "device_pad.png";
                            break;
                        case Model.DeviceType.Phone:
                            path += "device_phone.png";
                            break;
                        case Model.DeviceType.TV:
                            path += "device_tv.png";
                            break;
                        case Model.DeviceType.PC:
                            path += "device_windows.png";
                            break;
                        case Model.DeviceType.Windows:
                            path += "icon_windows.png";
                            break;
                        case Model.DeviceType.Android:
                            path += "icon_android.png";
                            break;
                        case Model.DeviceType.Linux:
                            path += "icon_linux_pc.png";
                            break;
                        case Model.DeviceType.NAS_Server:
                            path += "icon_nas_server.png";
                            break;
                        case Model.DeviceType.Printer:
                            path += "icon_printer.png";
                            break;
                        case Model.DeviceType.Repeater:
                            path += "icon_repeater.png";
                            break;
                        case Model.DeviceType.Scanner:
                            path += "icon_scanner.png";
                            break;
                        case Model.DeviceType.Unknown_Wired:
                            path += "icon_unknown_wired.png";
                            break;
                        case Model.DeviceType.Unknown_Wireless:
                            path += "icon_unknown_wireless.png";
                            break;
                        default:
                            path += "icon_unknown_wired.png";
                            break;
                    }
                    return new BitmapImage() { UriSource = new Uri(path) };
                }
            }

            public string name
            {
                get
                {
                    return type.ToString();
                }
            }

            public Model.DeviceType type { get; set; }

            public TypeModel(Model.DeviceType type) { this.type = type; }

            public static List<TypeModel> GetAllModel()
            {
                var list = new List<TypeModel>();
                foreach (Model.DeviceType item in Enum.GetValues(typeof(Model.DeviceType)))
                {
                    list.Add(new TypeModel(item));
                }
                return list;
            }
        }

        public ClientDialog(Model.Client.ClientInfo clientInfo)
        {
            this.client = clientInfo;
            dialogBrush = new ClientDialogBrush();
            qos_enable = client.isQosLimit;
            qos_down = client.qos_down/1024;
            qos_up = client.qos_up/1024;
            typeModels = TypeModel.GetAllModel();
            for (int i = 0; i < typeModels.Count; i++)
            {
                if(typeModels[i].type==client.deviceType)
                {
                    SelectedIndex = i;
                    break;
                }
            }
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Setting.DeviceName.SetDeviceName(client.mac, client.name);
            Setting.DeviceType.SetDeviceType(client.mac, (comboBox.SelectedItem as TypeModel).type);
            if (checkBox.IsChecked == true)
                Setting.WhiteList.AddDevice(client.mac);
            else if (checkBox.IsChecked == false)
                Setting.WhiteList.RemoveDevice(client.mac);
            long down = qos_down * 1024;
            long up = qos_up * 1024;
            if (qos_enable != client.isQosLimit || down != client.qos_down || up != client.qos_up)
            {
                var qosRules = await RouterAPI.GetQosRuleList();
                qosRules.AppendRule(new Model.QosRule()
                {
                    mac=client.mac,
                    enable=qos_enable,
                    up=up,
                    down=down
                });
                await RouterAPI.SetQosRuleList(qosRules);
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            comboBox.SelectedItem = typeModels[SelectedIndex];
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
