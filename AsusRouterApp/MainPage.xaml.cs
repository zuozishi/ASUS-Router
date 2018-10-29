using AsusRouterApp.Class;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace AsusRouterApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<Model.Client.ClientInGroup> clientGroup;

        public MainDataProvider data;

        public MainPageBrush brushMainPage;

        private bool closeAlertWLGrid=false;

        private bool closeAlertLimitGrid = false;

        /// <summary>
        /// Fluent Design System兼容方案
        /// </summary>
        public class MainPageBrush : INotifyPropertyChanged
        {
            public Brush mainGrid;
            public Brush topGrid;
            public Brush menuGrid;

            public Brush alertGrid;
            public Brush wanInfoGrid;
            public Brush rateInfoGrid1;
            public Brush rateInfoGrid2;
            public Brush rateInfoGrid3;
            public Brush clientStateGrid;
            public Brush sysStateGrid;
            public Brush cpu1StateGrid;
            public Brush cpu2StateGrid;

            public Utils.UI.AccentColor accentColor;

            public MainPageBrush(CoreDispatcher dispatcher)
            {
                accentColor = new Utils.UI.AccentColor();
                accentColor.AccentColorChanged +=async (value) =>
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush"))
                        {
                            mainGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, accentColor.accentColor, accentColor.accentColor, 0.3);
                            wanInfoGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, accentColor.accentColor, Color.FromArgb(255, 70, 171, 255), 0.6);
                        }
                        else
                        {
                            wanInfoGrid = new SolidColorBrush(accentColor.accentColor);
                        }
                        RaisePropertyChanged("mainGrid");
                        RaisePropertyChanged("wanInfoGrid");
                    });
                };
                
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush"))
                {
                    //整个页面采用FDS
                    mainGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, accentColor.accentColor, accentColor.accentColor, 0.3);
                    //顶部栏跟随FDS
                    topGrid = new SolidColorBrush(Colors.Transparent);
                    menuGrid = new SolidColorBrush(Colors.Transparent);
                    //主页色块
                    alertGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, Colors.OrangeRed, Colors.OrangeRed, 0.6);
                    wanInfoGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, accentColor.accentColor, Color.FromArgb(255, 70, 171, 255), 0.6);
                    rateInfoGrid1 = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, Color.FromArgb(255, 37,180,128), Color.FromArgb(255, 37, 180, 128), 0.6);
                    rateInfoGrid2 = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, Color.FromArgb(255, 127, 87, 197), Color.FromArgb(255, 127, 87, 197), 0.6);
                    rateInfoGrid3 = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, Color.FromArgb(255, 107, 197, 87), Color.FromArgb(255, 107, 197, 87), 0.6);
                    clientStateGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, Color.FromArgb(255, 37, 174, 180), Color.FromArgb(255, 37, 174, 180), 0.6);
                    sysStateGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, Color.FromArgb(255, 211, 82, 62), Color.FromArgb(255, 211, 82, 62), 0.6);
                    cpu1StateGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, Color.FromArgb(255, 37, 95, 180), Color.FromArgb(255, 37, 95, 180), 0.6);
                    cpu2StateGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, Color.FromArgb(255, 180, 37, 174), Color.FromArgb(255, 180, 37, 174), 0.6);
                }
                else
                {
                    //主页面白色
                    mainGrid = new SolidColorBrush(Color.FromArgb(255, 70, 171, 255));
                    //顶部栏蓝色
                    topGrid = new SolidColorBrush(Colors.Transparent);
                    menuGrid = new SolidColorBrush(Colors.Transparent);
                    //主页色块
                    alertGrid = new SolidColorBrush(Colors.OrangeRed);
                    wanInfoGrid = new SolidColorBrush(accentColor.accentColor);
                    rateInfoGrid1 = new SolidColorBrush(Color.FromArgb(255, 37, 180, 128));
                    rateInfoGrid2 = new SolidColorBrush(Color.FromArgb(255, 127, 87, 197));
                    rateInfoGrid3 = new SolidColorBrush(Color.FromArgb(255, 107, 197, 87));
                    clientStateGrid = new SolidColorBrush(Color.FromArgb(255, 37, 174, 180));
                    sysStateGrid = new SolidColorBrush(Color.FromArgb(255, 211, 82, 62));
                    cpu1StateGrid = new SolidColorBrush(Color.FromArgb(255, 37, 95, 180));
                    cpu2StateGrid = new SolidColorBrush(Color.FromArgb(255, 180, 37, 174));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void RaisePropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            brushMainPage = new MainPageBrush(this.Dispatcher);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            UI_Init();
            if (Setting.DemoAccount)
            {
                data =await MainDataProvider.GetDemoData();
                UpdateUI();
            }
            else
            {
                data = new MainDataProvider(2);
                data.DataUpdate +=async () =>
                {
                    try
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                            UpdateUI();
                        });
                    }
                    catch (Exception)
                    {
                    }
                };
            }
            if (Setting.FileStart)
            {
                var loadRes = Setting.Data.Load(Setting.FileData);
                await new Control.FileActivatedDialog(loadRes).ShowAsync();
            }
            string ver=SystemInformation.ApplicationVersion.ToFormattedString();
            if ((string)Setting.GetSetting("version", "") != ver)
            {
                Setting.SetSetting("version",ver);
                var logDialog = new Control.UpdateLogDialog();
                logDialog.LoadLog();
                await logDialog.ShowAsync();
            }
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            toastTaskCheckBox.IsChecked= await Setting.Task.isWhiteListToastTaskReg();
            if (Setting.ToastStart)
                pivotView.SelectedIndex = 1;
        }

        /// <summary>
        /// UI初始化
        /// </summary>
        private void UI_Init()
        {
            //数据加载前隐藏UI
            grid_waninfo.Visibility = Visibility.Collapsed;
            grid_netrate.Visibility = Visibility.Collapsed;
            grid_state.Visibility = Visibility.Collapsed;
            grid_cpu.Visibility = Visibility.Collapsed;
            //Contact启动隐藏顶部栏
            if (Setting.ContactStart) topGrid.Visibility = Visibility.Collapsed;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                var menu = new MenuFlyoutItem();
                menu.Text = "Export Demo Data";
                menu.Click += async (s, e) =>
                {
                    if (data != null)
                    {
                        var res = await data.ExportDemoData();
                        if (res)
                            notification.Show("Export Demo Data Success");
                        else
                            notification.Show("Export Demo Data Failed");
                    }
                };
                menuFlyout.Items.Add(menu);
            }
        }

        private void UpdateUI()
        {
            //广域网及路由器信息
            if(data.wanInfo !=null)
            {
                textBlock_model.Text = data.wanInfo.model;
                textBlock_title.Text = data.wanInfo.name;
                textBlock_devName.Text = data.wanInfo.name;
                textBlock_ipAdd.Text = data.wanInfo.ipaddr;
                textBlock_state.Text = data.wanInfo.statusstr;
                textBlock_ddns.Text = data.wanInfo.ddns;
                grid_waninfo.Visibility = Visibility.Visible;
                listview_wanInfo.DataContext = data.wanInfo;
                if (!Setting.ContactStart) button_pinContact.Visibility = Visibility.Visible;
            }

            //网络速率
            if(data.netRate !=null)
            {
                if (data.netSpeed == null) data.netSpeed = new Model.NetSpeed();
                data.netSpeed.Update(data.netRate);

                textBlock_rateWAN_tx.Text = data.netSpeed.uploadSpeed_wan_str;
                textBlock_rateWAN_rx.Text = data.netSpeed.downloadSpeed_wan_str;

                textBlock_rate5G_rx.Text = data.netSpeed.uploadSpeed_wl5g_str;
                textBlock_rate5G_tx.Text = data.netSpeed.downloadSpeed_wl5g_str;

                textBlock_rate2G_rx.Text = data.netSpeed.uploadSpeed_wl2g_str;
                textBlock_rate2G_tx.Text = data.netSpeed.downloadSpeed_wl2g_str;
                
                grid_netrate.Visibility = Visibility.Visible;
            }

            //客户端数量
            if(data.clients !=null)
            {
                textBlock_wlNum.Text = (data.clients.WL2G.Count + data.clients.WL5G.Count).ToString();
                textBlock_lanNum.Text = data.clients.LAN.Count.ToString();
                if (clientGroup == null)
                {
                    clientGroup = data.clients.GetGroup();
                    this.itemcollectSource.Source = clientGroup;
                    ZoomOutView.ItemsSource = itemcollectSource.View.CollectionGroups;
                    ZoomInView.ItemsSource = itemcollectSource.View;
                }
                else
                {
                    clientGroup[0].AsyncList(data.clients.LAN);
                    clientGroup[1].AsyncList(data.clients.WL5G);
                    clientGroup[2].AsyncList(data.clients.WL2G);
                    clientGroup[3].AsyncWhiteList(data.clients.NotInBlackDevices);
                }
                grid_state.Visibility = Visibility.Visible;
            }

            //CPU及内存占用情况
            if(data.cpuMemInfo !=null)
            {
                long cpu_all = data.cpuMemInfo.cpu_usage.cpu1_total + data.cpuMemInfo.cpu_usage.cpu2_total;
                long cpu_usage = data.cpuMemInfo.cpu_usage.cpu1_usage + data.cpuMemInfo.cpu_usage.cpu2_usage;
                progress_cpu.Maximum = cpu_all;
                progress_cpu.Value = cpu_usage;
                textBlock_cpu.Text = (cpu_usage * 100 / cpu_all).ToString("f2");

                progress_mem.Maximum = data.cpuMemInfo.memory_usage.mem_total;
                progress_mem.Value = data.cpuMemInfo.memory_usage.mem_used;
                textBlock_mem.Text = (progress_mem.Value * 100 / progress_mem.Maximum).ToString("f2");

                radial_cpu1.Maximum = data.cpuMemInfo.cpu_usage.cpu1_total;
                radial_cpu1.Value = data.cpuMemInfo.cpu_usage.cpu1_usage;
                radial_cpu2.Maximum = data.cpuMemInfo.cpu_usage.cpu2_total;
                radial_cpu2.Value = data.cpuMemInfo.cpu_usage.cpu2_usage;

                textBlock_cpu1.Text=(radial_cpu1.Value*100/ radial_cpu1.Maximum).ToString("f2");
                textBlock_cpu2.Text = (radial_cpu2.Value * 100 / radial_cpu2.Maximum).ToString("f2");
                grid_cpu.Visibility = Visibility.Visible;
            }

            //更新客户端速率
            if(data.devRate !=null&&clientGroup!=null)
            {
                var macs= data.devRate.Keys.ToArray();
                foreach (var mac in macs)
                {
                    for (int i = 0; i < clientGroup.Count; i++)
                    {
                        for (int j = 0; j < clientGroup[i].Clients.Count; j++)
                        {
                            if(mac== clientGroup[i].Clients[j].mac)
                            {
                                clientGroup[i].Clients[j].UpdateRate(data.devRate[mac]);
                                break;
                            }
                        }
                    }
                }
            }

            //更新禁用互联网状态
            if(data.banList !=null&& clientGroup != null)
            {
                for (int i = 0; i < clientGroup.Count; i++)
                {
                    for (int j = 0; j < clientGroup[i].Clients.Count; j++)
                    {
                        clientGroup[i].Clients[j].UpdateBanState(data.banList);
                    }
                }
                var blackDevNum = clientGroup[3].Clients.Where(o=>o.isBan==false).Count();
                if (blackDevNum == 0)
                {
                    alertWLGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (blackDevNumText.Text == "0")
                    {
                        alertWLGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (!closeAlertWLGrid)
                            alertWLGrid.Visibility = Visibility.Visible;
                    }
                }
                blackDevNumText.Text = blackDevNum.ToString();
            }

            //更新带宽限制状态
            if (data.qosRuleList != null && clientGroup != null) {
                for (int i = 0; i < clientGroup.Count; i++)
                {
                    for (int j = 0; j < clientGroup[i].Clients.Count; j++)
                    {
                        clientGroup[i].Clients[j].UpdateQosState(data.qosRuleList);
                    }
                }
                var limitDevNum = data.qosRuleList.Where(o=>o.enable==true&&data.clients.macList.Contains(o.mac)).Count();
                if (limitDevNum == 0)
                {
                    alertBandLimitGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (blackDevNumText.Text == "0")
                    {
                        alertBandLimitGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (!closeAlertLimitGrid)
                            alertBandLimitGrid.Visibility = Visibility.Visible;
                    }
                }
                limitDevNumText.Text = limitDevNum.ToString();
            }

            if (data.wlanInfo !=null)
            {
                listview_wlaninfo.DataContext = data.wlanInfo;
            }
            progress_main.IsIndeterminate = false;
        }

        /// <summary>
        /// 顶部按钮触发,切换PivotItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TopBtn_Click(object sender, RoutedEventArgs e)
        {
            pivotView.SelectedIndex = (sender as Button).TabIndex;
        }

        /// <summary>
        /// Pivot滑动触发,更新顶部按钮Style
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pivotView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            #region 清除设备页面设备列表(否则其他页面滚动异常)
            if (pivotView.SelectedIndex==1)
            {
                if (clientGroup != null)
                {
                    this.itemcollectSource.Source = clientGroup;
                    ZoomOutView.ItemsSource = itemcollectSource.View.CollectionGroups;
                    ZoomInView.ItemsSource = itemcollectSource.View;
                }
            }
            else
            {
                this.itemcollectSource.Source = null;
                ZoomOutView.ItemsSource = null;
                ZoomInView.ItemsSource = null;
            }
            #endregion
            SetSelectBtnSthyle(pivotView.SelectedIndex);
        }

        /// <summary>
        /// 更新顶部按钮Style
        /// </summary>
        /// <param name="index"></param>
        private void SetSelectBtnSthyle(int index)
        {
            var btns = TopBtnsGrid.Children;
            foreach (var item in btns)
            {
                var btn = item as Button;
                if (btn.TabIndex == index)
                {
                    btn.Style = (Style)Resources["TopBtnSelected"];
                }
                else
                {
                    btn.Style = (Style)Resources["TopBtnUnSelected"];
                }
            }
        }

        /// <summary>
        /// 客户端数量信息按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientBtn_Click(object sender, RoutedEventArgs e)
        {
            pivotView.SelectedIndex = 1;
        }

        private void wl0_Toggled(object sender, RoutedEventArgs e)
        {
            var s = sender as ToggleSwitch;
            if(data.wlanInfo != null) data.wlanInfo.wl0_enable = s.IsOn;
        }

        private void wl1_Toggled(object sender, RoutedEventArgs e)
        {
            var s = sender as ToggleSwitch;
            if (data.wlanInfo != null) data.wlanInfo.wl1_enable = s.IsOn;
        }

        /// <summary>
        /// 保存WiFi按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveWifiBtn_Click(object sender, RoutedEventArgs e)
        {
            object templete;
            bool isTemplatePresent = Resources.TryGetValue("SaveWiFiInfoNotificationTemplate", out templete);
            if (isTemplatePresent && templete is DataTemplate)
            {
                notification.Show(templete as DataTemplate);
            }
        }

        /// <summary>
        /// 确定保存WiFi设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveWiFiYes_Clicked(object sender, RoutedEventArgs e)
        {
            notification.Dismiss();
            if (data.wlanInfo == null) return;
            if (data.wlanInfo.wl0_ssid.Length == 0 || data.wlanInfo.wl1_ssid.Length == 0)
            {
                notification.Show(Utils.AppResources.GetString("NullSSID"));
                return;
            }
            if (data.wlanInfo.wl0_wpa_psk.Length == 0 || data.wlanInfo.wl1_wpa_psk.Length == 0)
            {
                notification.Show(Utils.AppResources.GetString("NullPassword"));
                return;
            }
            var res = await RouterAPI.SetWlan(data.wlanInfo);
            if (res)
                notification.Show(Utils.AppResources.GetString("WiFiSetSuccess"));
            else
                notificationError.Show(Utils.AppResources.GetString("WiFiSetError"));
        }

        /// <summary>
        /// 取消保存WiFi设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveWiFiNo_Clicked(object sender, RoutedEventArgs e)
        {
            notification.Dismiss();
        }

        /// <summary>
        /// 重启按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RebootBtn_Click(object sender, RoutedEventArgs e)
        {
            object templete;
            bool isTemplatePresent = Resources.TryGetValue("RebootNotificationTemplate", out templete);
            if (isTemplatePresent && templete is DataTemplate)
            {
                notification.Show(templete as DataTemplate);
            }
        }

        /// <summary>
        /// 重启路由器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RebootYes_Clicked(object sender, RoutedEventArgs e)
        {
            notification.Dismiss();
            var res = await RouterAPI.Reboot();
            if(res)
                notification.Show(Utils.AppResources.GetString("RouterRebootSuccess"));
            else
                notificationError.Show(Utils.AppResources.GetString("RouterRebootError"));
        }

        /// <summary>
        /// 固定路由器到任务栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PinContact_Click(object sender, RoutedEventArgs e)
        {
            var res = await ContractUtils.AddRouter(data.wanInfo.mac, data.wanInfo.name);
            if (res.res)
                ContractUtils.PinContact(res.contact);
        }

        private void WANInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            pivotView.SelectedIndex = 3;
        }

        private async void ClientMenuFlyoutClick(object sender, RoutedEventArgs e)
        {
            var menu = sender as MenuFlyoutItem;
            var client = menu.Tag as Model.Client.ClientInfo;
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            switch (menu.TabIndex)
            {
                case 0:
                    dataPackage.SetText(menu.Text);
                    Clipboard.SetContent(dataPackage);
                    notification.Show(Utils.AppResources.GetString("CopyComplete"), 2000);
                    break;
                case 1:
                    dataPackage.SetText(menu.Text);
                    Clipboard.SetContent(dataPackage);
                    notification.Show(Utils.AppResources.GetString("CopyComplete"), 2000);
                    break;
                case 2:
                    var dialog = new Control.ClientDialog(client);
                    var dialogRes = await dialog.ShowAsync();
                    if (dialogRes == ContentDialogResult.Primary)
                    {
                        clientGroup = null;
                    }
                    break;
                case 3:
                    var res = await RouterAPI.FireWall.SetBan(client.mac);
                    if (res)
                        notification.Show("Success", 2000);
                    else
                        notificationError.Show("Failed", 2000);
                    break;
                case 4:
                    if (client.isWhiteDevice)
                        Setting.WhiteList.RemoveDevice(client.mac);
                    else
                        Setting.WhiteList.AddDevice(client.mac);
                    client.RaisePropertyChanged("isWhiteDevice");
                    client.RaisePropertyChanged("wlMenuText");
                    client.RaisePropertyChanged("devWhiteImage");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginOutBtn_Click(object sender, RoutedEventArgs e)
        {
            RouterAPI.Url.Host = "";
            Setting.SetSetting("auth", "");
            this.Frame.Navigate(typeof(LoginPage), null);
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ExportDataBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var json = Setting.Data.Export();
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                savePicker.FileTypeChoices.Add("Asus Route Data File", new List<string>() { ".ard" });
                savePicker.SuggestedFileName = "Asus Route Data";
                var file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, json);
                    notification.Show(Utils.AppResources.GetString("ExportDataSuccess"), 1000);
                }
            }
            catch (Exception)
            {
                notificationError.Show(Utils.AppResources.GetString("ExportDataError"));
            }
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadDataBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
                openPicker.FileTypeFilter.Add(".ard");
                var file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    string json = await FileIO.ReadTextAsync(file);
                    Setting.Data.Load(json);
                    notification.Show(Utils.AppResources.GetString("LoadDataSuccess"), 1000);
                    clientGroup = null;
                }
            }
            catch (Exception)
            {
                notificationError.Show(Utils.AppResources.GetString("LoadDataError"));
            }
        }

        private void CloseAlertGridBtnClicked(object sender, RoutedEventArgs e)
        {
            int index = (sender as Button).TabIndex;
            if(index==0)
            {
                alertWLGrid.Visibility = Visibility.Collapsed;
                closeAlertWLGrid = true;
            }
            else if (index == 1)
            {
                alertBandLimitGrid.Visibility = Visibility.Collapsed;
                closeAlertLimitGrid = true;
            }
        }

        private void ToastTaskChecked(object sender, RoutedEventArgs e)
        {
            Setting.Task.RegWhiteListToastTask();
            var box = sender as CheckBox;
            if (box.IsChecked == true)
            {
                
            }else if(box.IsChecked == false)
            {
                Setting.Task.UnRegWhiteListToastTask();
            }
        }

        private async void ShoarQrBtnClicked(object sender, RoutedEventArgs e)
        {
            var index = (sender as Button).TabIndex;
            QRCoder.PayloadGenerator.WiFi generator;
            string title = "";
            if (index == 0)
                generator = new QRCoder.PayloadGenerator.WiFi(title = data.wlanInfo.wl0_ssid, data.wlanInfo.wl0_wpa_psk, QRCoder.PayloadGenerator.WiFi.Authentication.WPA);
            else
                generator = new QRCoder.PayloadGenerator.WiFi(title = data.wlanInfo.wl1_ssid, data.wlanInfo.wl1_wpa_psk, QRCoder.PayloadGenerator.WiFi.Authentication.WPA);
            QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData qrCodeData = qrGenerator.CreateQrCode(generator.ToString(), QRCoder.QRCodeGenerator.ECCLevel.Q);
            await new Control.QrCodeDialog(title, "Share Wi-Fi", qrCodeData).ShowAsync();
        }

        private void ShareDataBtn_Click(object sender, RoutedEventArgs e)
        {
            Setting.Data.Share();
        }

        private async void UpdateLogBtn_Click(object sender, RoutedEventArgs e)
        {
            var logDialog = new Control.UpdateLogDialog();
            logDialog.LoadLog();
            await logDialog.ShowAsync();
        }
    }
}
