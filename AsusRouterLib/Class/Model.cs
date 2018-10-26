using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace AsusRouterApp.Class
{
    public class Model
    {
        public class CpuMemInfo
        {
            public CPU cpu_usage { get; set; }
            
            public MEM memory_usage { get; set; }

            public class CPU
            {
                public long cpu1_total { get; set; }
                public long cpu1_usage { get; set; }
                public long cpu2_total { get; set; }
                public long cpu2_usage { get; set; }
            }

            public class MEM
            {
                public long mem_total { get; set; }
                public long mem_free { get; set; }
                public long mem_used { get; set; }
            }
        }
        
        public class NetRate
        {
            public string INTERNET_rx { get; set; }
            public string INTERNET_tx { get; set; }
            public string WIRELESS0_rx { get; set; }
            public string WIRELESS0_tx { get; set; }
            public string WIRELESS1_rx { get; set; }
            public string WIRELESS1_tx { get; set; }

            public long int_rx
            {
                get
                {
                    return Convert.ToInt64(INTERNET_rx, 16);
                }
            }

            public long int_tx
            {
                get
                {
                    return Convert.ToInt64(INTERNET_tx, 16);
                }
            }

            public long wl2g_rx
            {
                get
                {
                    return Convert.ToInt64(WIRELESS0_rx, 16);
                }
            }

            public long wl2g_tx
            {
                get
                {
                    return Convert.ToInt64(WIRELESS0_tx, 16);
                }
            }

            public long wl5g_rx
            {
                get
                {
                    return Convert.ToInt64(WIRELESS1_rx, 16);
                }
            }

            public long wl5g_tx
            {
                get
                {
                    return Convert.ToInt64(WIRELESS1_tx, 16);
                }
            }
        }

        public class NetSpeed
        {
            public double downloadSpeed_wan { get; set; } = 0;
            public double uploadSpeed_wan { get; set; } = 0;

            public double downloadSpeed_wl2g { get; set; } = 0;
            public double uploadSpeed_wl2g { get; set; } = 0;

            public double downloadSpeed_wl5g { get; set; } = 0;
            public double uploadSpeed_wl5g { get; set; } = 0;

            public string downloadSpeed_wan_str { get; set; } = "";
            public string uploadSpeed_wan_str { get; set; } = "";

            public string downloadSpeed_wl2g_str { get; set; } = "";
            public string uploadSpeed_wl2g_str { get; set; } = "";

            public string downloadSpeed_wl5g_str { get; set; } = "";
            public string uploadSpeed_wl5g_str { get; set; } = "";

            private long[][] prev = new long[][]
            {
                new long[] { -1,-1},
                new long[] { -1,-1},
                new long[] { -1,-1}
            };

            private long timestamp = DateTime.Now.CurrentTimeMillis();

            public void Update(NetRate c)
            {
                if(prev[0][0]!=-1&&prev[0][1]!=-1)
                {
                    downloadSpeed_wan = (((c.int_rx < prev[0][0]) ? (c.int_rx + (0xFFFFFFFF - prev[0][0])) : (c.int_rx - prev[0][0])) / 1024.0 / (DateTime.Now.CurrentTimeMillis() - timestamp) * 1000.0);
                    uploadSpeed_wan = (((c.int_tx < prev[0][1]) ? (c.int_tx + (0xFFFFFFFF - prev[0][1])) : (c.int_tx - prev[0][1])) / 1024.0 / (DateTime.Now.CurrentTimeMillis() - timestamp) * 1000.0);
                }
                prev[0][0] = c.int_rx;
                prev[0][1] = c.int_tx;
                if (downloadSpeed_wan < 1024)
                    downloadSpeed_wan_str = Math.Ceiling(downloadSpeed_wan) + " KB/s";
                else
                    downloadSpeed_wan_str = (downloadSpeed_wan/1024).ToString("f2")+" MB/s";
                if (uploadSpeed_wan < 1024)
                    uploadSpeed_wan_str = Math.Ceiling(uploadSpeed_wan) + " KB/s";
                else
                    uploadSpeed_wan_str = (uploadSpeed_wan / 1024).ToString("f2") + " MB/s";

                if (prev[1][0] != -1 && prev[1][1] != -1)
                {
                    downloadSpeed_wl2g = (((c.wl2g_rx < prev[1][0]) ? (c.wl2g_rx + (0xFFFFFFFF - prev[1][0])) : (c.wl2g_rx - prev[1][0])) / 1024.0 / (DateTime.Now.CurrentTimeMillis() - timestamp) * 1000.0);
                    uploadSpeed_wl2g = (((c.wl2g_tx < prev[1][1]) ? (c.wl2g_tx + (0xFFFFFFFF - prev[1][1])) : (c.wl2g_tx - prev[1][1])) / 1024.0 / (DateTime.Now.CurrentTimeMillis() - timestamp) * 1000.0);
                }
                prev[1][0] = c.wl2g_rx;
                prev[1][1] = c.wl2g_tx;
                if (downloadSpeed_wl2g < 1024)
                    downloadSpeed_wl2g_str = Math.Ceiling(downloadSpeed_wl2g) + " KB/s";
                else
                    downloadSpeed_wl2g_str = (downloadSpeed_wl2g / 1024).ToString("f2") + " MB/s";
                if (uploadSpeed_wl2g < 1024)
                    uploadSpeed_wl2g_str = Math.Ceiling(uploadSpeed_wl2g) + " KB/s";
                else
                    uploadSpeed_wl2g_str = (uploadSpeed_wl2g / 1024).ToString("f2") + " MB/s";

                if (prev[2][0] != -1 && prev[2][1] != -1)
                {
                    downloadSpeed_wl5g = (((c.wl5g_rx < prev[2][0]) ? (c.wl5g_rx + (0xFFFFFFFF - prev[2][0])) : (c.wl5g_rx - prev[2][0])) / 1024.0 / (DateTime.Now.CurrentTimeMillis() - timestamp) * 1000.0);
                    uploadSpeed_wl5g = (((c.wl5g_tx < prev[2][1]) ? (c.wl5g_tx + (0xFFFFFFFF - prev[2][1])) : (c.wl5g_tx - prev[2][1])) / 1024.0 / (DateTime.Now.CurrentTimeMillis() - timestamp) * 1000.0);
                }
                prev[2][0] = c.wl5g_rx;
                prev[2][1] = c.wl5g_tx;
                if (downloadSpeed_wl5g < 1024)
                    downloadSpeed_wl5g_str = Math.Ceiling(downloadSpeed_wl5g) + " KB/s";
                else
                    downloadSpeed_wl5g_str = (downloadSpeed_wl5g / 1024).ToString("f2") + " MB/s";
                if (uploadSpeed_wl5g < 1024)
                    uploadSpeed_wl5g_str = Math.Ceiling(uploadSpeed_wl5g) + " KB/s";
                else
                    uploadSpeed_wl5g_str = (uploadSpeed_wl5g / 1024).ToString("f2") + " MB/s";

                timestamp = DateTime.Now.CurrentTimeMillis();
            }
        }

        public class WLANInfo
        {
            public int wl0_radio { get; set; }
            public string wl0_ssid { get; set; }
            public string wl0_wpa_psk { get; set; }
            public int wl1_radio { get; set; }
            public string wl1_ssid { get; set; }
            public string wl1_wpa_psk { get; set; }

            public bool wl0_enable
            {
                get
                {
                    if (wl0_radio == 1)
                        return true;
                    else
                        return false;
                }
                set
                {
                    if (value)
                        wl0_radio = 1;
                    else
                        wl0_radio = 0;
                }
            }

            public bool wl1_enable
            {
                get
                {
                    if (wl1_radio == 1)
                        return true;
                    else
                        return false;
                }
                set
                {
                    if (value)
                        wl1_radio = 1;
                    else
                        wl1_radio = 0;
                }
            }
        }
        
        public class WANInfo
        {
            [JsonProperty("model")]
            public string model { get; set; }
            [JsonProperty("daapd_friendly_name")]
            public string name { get; set; }
            [JsonProperty("0:macaddr")]
            public string mac { get; set; }
            [JsonProperty("ddns_hostname_x")]
            public string ddns { get; set; }
            [JsonProperty("wanlink-status")]
            public int status { get; set; }
            [JsonProperty("wanlink-statusstr")]
            public string statusstr { get; set; }
            [JsonProperty("wanlink-ipaddr")]
            public string ipaddr { get; set; }
            [JsonProperty("wanlink-dns")]
            public string dns { get; set; }
        }

        public class Client
        {
            public List<string> macList { get; set; }

            public Dictionary<string,ClientInfo> clientList { get; set; }

            public List<ClientInfo> LAN
            {
                get
                {
                    var res = new List<ClientInfo>();
                    foreach (var item in clients)
                    {
                        if (!item.isWL) res.Add(item);
                    }
                    return res;
                }
            }

            public List<ClientInfo> WL5G
            {
                get
                {
                    var res = new List<ClientInfo>();
                    foreach (var item in clients)
                    {
                        if (item.isOnline&&item.isWL&&item.netType == NetType.WL5G) res.Add(item);
                    }
                    return res;
                }
            }

            public List<ClientInfo> WL2G
            {
                get
                {
                    var res = new List<ClientInfo>();
                    foreach (var item in clients)
                    {
                        if (item.isOnline && item.isWL && item.netType == NetType.WL2G) res.Add(item);
                    }
                    return res;
                }
            }

            public List<ClientInfo> NotInBlackDevices
            {
                get
                {
                    var res = new List<ClientInfo>();
                    foreach (var item in clients)
                    {
                        if (!item.isWhiteDevice) res.Add(item);
                    }
                    return res;
                }
            }

            public List<ClientInfo> clients
            {
                get
                {
                    var res = new List<ClientInfo>();
                    foreach (var mac in macList)
                    {
                        if (clientList.ContainsKey(mac)&& clientList[mac].isOnline) res.Add(clientList[mac]);
                    }
                    return res;
                }
            }

            public Dictionary<string,WlanInfo> wl5g { get; set; }

            public Dictionary<string,WlanInfo> wl2g { get; set; }

            /// <summary>
            /// 更新设备名称
            /// </summary>
            public void UpdateDeviceName()
            {
                if (clientList != null)
                {
                    for (int i = 0; i < macList.Count; i++)
                    {
                        try
                        {
                            if (clientList[macList[i]] != null)
                            {
                                clientList[macList[i]].name = Setting.DeviceName.GetDeviceName(macList[i], clientList[macList[i]].name);
                                if(clientList[macList[i]].name==null|| clientList[macList[i]].name.Length==0)
                                {
                                    Setting.DeviceName.SetDeviceName(clientList[macList[i]].mac, clientList[macList[i]].mac);
                                    clientList[macList[i]].name = clientList[macList[i]].mac;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }

            /// <summary>
            /// 更新设备类型
            /// </summary>
            public void UpdateDeviceType()
            {
                if (clientList != null)
                {
                    for (int i = 0; i < macList.Count; i++)
                    {
                        try
                        {
                            if (clientList[macList[i]] != null)
                            {
                                clientList[macList[i]].deviceType = Setting.DeviceType.GetDeviceType(macList[i], Setting.DeviceType.GetDefType(clientList[macList[i]].isWL, clientList[macList[i]].name));
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }

            public void Update()
            {
                UpdateDeviceName();
                foreach (var mac in macList)
                {
                    if (clientList.ContainsKey(mac))
                    {
                        clientList[mac].isWL = false;
                        if (wl2g.ContainsKey(mac))
                        {
                            clientList[mac].isWL = true;
                            clientList[mac].netType = NetType.WL2G;
                            clientList[mac].wlanInfo = wl2g[mac];
                        }else if (wl5g.ContainsKey(mac))
                        {
                            clientList[mac].isWL = true;
                            clientList[mac].netType = NetType.WL5G;
                            clientList[mac].wlanInfo = wl5g[mac];
                        }
                        if (!clientList[mac].isWL) clientList[mac].netType = NetType.LAN;
                    }
                }
                UpdateDeviceType();
            }

            public class ClientInfo : INotifyPropertyChanged
            {
                public string name { get; set; }
                public string ip { get; set; }
                public string mac { get; set; }
                public string from { get; set; }
                public bool isOnline { get; set; }
                public bool isWL { get; set; }
                public NetType netType { get; set; }
                public WlanInfo wlanInfo { get; set; }
                public ClientNetSpeed speed { get; set; }

                public DeviceType deviceType { get; set; } = DeviceType.Unknown_Wired;

                public bool isBan { get; set; } = false;

                public bool isQosLimit { get; set; } = false;

                public long qos_down { get; set; } = 0;

                public long qos_up { get; set; } = 0;

                public bool isWhiteDevice
                {
                    get
                    {
                        return Setting.WhiteList.IsWhiteDevice(mac);
                    }
                }

                public string wlMenuText
                {
                    get
                    {
                        if (isWhiteDevice)
                            return Utils.AppResources.GetString("RemoveFromWhiteList");
                        else
                            return Utils.AppResources.GetString("AddToWhiteList");
                    }
                }

                public BitmapImage netTypeImage
                {
                    get
                    {
                        string path = "ms-appx:///Assets/icon/";
                        switch (netType)
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
                }

                public BitmapImage devTypeImage
                {
                    get
                    {
                        string path = "ms-appx:///Assets/icon/device/";
                        switch (deviceType)
                        {
                            case DeviceType.Laptop:
                                path += "device_laptop.png";
                                break;
                            case DeviceType.MAC:
                                path += "device_mac.png";
                                break;
                            case DeviceType.PAD:
                                path += "device_pad.png";
                                break;
                            case DeviceType.Phone:
                                path += "device_phone.png";
                                break;
                            case DeviceType.TV:
                                path += "device_tv.png";
                                break;
                            case DeviceType.PC:
                                path += "device_windows.png";
                                break;
                            case DeviceType.Windows:
                                path += "icon_windows.png";
                                break;
                            case DeviceType.Android:
                                path += "icon_android.png";
                                break;
                            case DeviceType.Linux:
                                path += "icon_linux_pc.png";
                                break;
                            case DeviceType.NAS_Server:
                                path += "icon_nas_server.png";
                                break;
                            case DeviceType.Printer:
                                path += "icon_printer.png";
                                break;
                            case DeviceType.Repeater:
                                path += "icon_repeater.png";
                                break;
                            case DeviceType.Scanner:
                                path += "icon_scanner.png";
                                break;
                            case DeviceType.Unknown_Wired:
                                path += "icon_unknown_wired.png";
                                break;
                            case DeviceType.Unknown_Wireless:
                                path += "icon_unknown_wireless.png";
                                break;
                            default:
                                path += "icon_unknown_wired.png";
                                break;
                        }
                        return new BitmapImage() { UriSource = new Uri(path) };
                    }
                }

                public BitmapImage devBanImage
                {
                    get
                    {
                        if (isBan)
                        {
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/icon/icon_mac_filter_block.png") };
                        }
                        else
                            return null;
                    }
                }

                public BitmapImage devWhiteImage
                {
                    get
                    {
                        if (isWhiteDevice)
                            return null;
                        else
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/icon/blackdevice.png") };
                    }
                }

                public BitmapImage devQosImage
                {
                    get
                    {
                        if (isQosLimit)
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/icon/icon_devices_bandwidth_limiter.png") };
                        else
                            return null;
                    }
                }

                public Visibility speedVisibility { get; set; } = Visibility.Collapsed;

                public void UpdateRate(DeviceRate rate)
                {
                    if (this.speed == null) this.speed = new ClientNetSpeed();
                    this.speed.Update(rate);
                    if (this.speed.downloadSpeed > 100 || this.speed.uploadSpeed > 100)
                        speedVisibility = Visibility.Visible;
                    else
                        speedVisibility = Visibility.Collapsed;
                    RaisePropertyChanged("speed");
                    RaisePropertyChanged("speedVisibility");
                }

                public void UpdateBanState(string[] banList)
                {
                    var query = banList.Where(o => o == this.mac).ToArray();
                    if (query.Length == 0)
                        this.isBan = false;
                    else
                        this.isBan = true;
                    RaisePropertyChanged("isBan");
                    RaisePropertyChanged("devBanImage");
                }

                public void UpdateQosState(Model.QosRuleList qosRules)
                {
                    int index = qosRules.GetRuleIndex(this.mac);
                    if (index >= 0)
                    {
                        isQosLimit = qosRules[index].enable;
                        qos_down = qosRules[index].down;
                        qos_up = qosRules[index].up;
                    }
                    else
                        isQosLimit = false;
                    RaisePropertyChanged("isQosLimit");
                    RaisePropertyChanged("devQosImage");
                }

                public event PropertyChangedEventHandler PropertyChanged;
                public void RaisePropertyChanged(string name)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                }
            }
            
            public enum NetType
            {
                LAN,WL2G, WL5G
            }

            public class WlanInfo
            {
                public string isWL { get; set; }
                public string rssi { get; set; }
            }

            public class ClientNetSpeed
            {
                public double downloadSpeed { get; set; } = 0;
                public double uploadSpeed { get; set; } = 0;

                public string downloadSpeed_str { get; set; } = "";
                public string uploadSpeed_str { get; set; } = "";

                private long[] prev = new long[] { -1, -1 };

                private long timestamp = DateTime.Now.CurrentTimeMillis();

                public void Update(DeviceRate c)
                {
                    if (prev[0] != -1 && prev[1] != -1)
                    {
                        downloadSpeed = (((c.rx < prev[0]) ? (c.rx + (0xFFFFFFFF - prev[0])) : (c.rx - prev[0])) / 1024.0 / (DateTime.Now.CurrentTimeMillis() - timestamp) * 1000.0);
                        uploadSpeed = (((c.tx < prev[1]) ? (c.tx + (0xFFFFFFFF - prev[1])) : (c.tx - prev[1])) / 1024.0 / (DateTime.Now.CurrentTimeMillis() - timestamp) * 1000.0);
                    }
                    prev[0] = c.rx;
                    prev[1] = c.tx;
                    if (downloadSpeed < 1024)
                        downloadSpeed_str = Math.Ceiling(downloadSpeed) + " KB/s";
                    else
                        downloadSpeed_str = (downloadSpeed / 1024).ToString("f2") + " MB/s";
                    if (uploadSpeed < 1024)
                        uploadSpeed_str = Math.Ceiling(uploadSpeed) + " KB/s";
                    else
                        uploadSpeed_str = (uploadSpeed / 1024).ToString("f2") + " MB/s";
                    timestamp = DateTime.Now.CurrentTimeMillis();
                }
            }

            public List<ClientInGroup> GetGroup()
            {
                var res = new List<ClientInGroup>();
                res.Add(new ClientInGroup(Utils.AppResources.GetString("LAN"), LAN));
                res.Add(new ClientInGroup("5G", WL5G));
                res.Add(new ClientInGroup("2.4G", WL2G));
                res.Add(new ClientInGroup(Utils.AppResources.GetString("NotInWhiteList"), NotInBlackDevices));
                return res;
            }

            public class ClientInGroup : INotifyPropertyChanged
            {
                public string key { get; set; }

                public string name
                {
                    get
                    {
                        string res = key;
                        int num = 0;
                        if (Clients != null) num = Clients.Count;
                        res = res + " ("+num+")";
                        return res;
                    }
                }

                public ObservableCollection<ClientInfo> Clients { get; set; }

                public ClientInGroup() { }

                public ClientInGroup(string key,IEnumerable<ClientInfo> collection) {
                    this.key = key;
                    this.Clients = new ObservableCollection<ClientInfo>(collection);
                }

                public void AsyncList(IEnumerable<ClientInfo> data)
                {
                    for (int i = 0; i < Clients.Count; i++)
                    {
                        var query = data.Where(o => o.mac == Clients[i].mac).ToArray();
                        if (query.Length == 0) Clients.RemoveAt(i);
                    }
                    foreach (var item in data)
                    {
                        var query = Clients.Where(o => o.mac == item.mac).ToArray();
                        if (query.Length == 0) Clients.Add(item);
                    }
                    RaisePropertyChanged("name");
                }

                public void AsyncWhiteList(IEnumerable<ClientInfo> data)
                {
                    for (int i = 0; i < Clients.Count; i++)
                    {
                        var query = data.Where(o => o.mac == Clients[i].mac).ToArray();
                        if (query.Length == 0) Clients.RemoveAt(i);
                    }
                    foreach (var item in data)
                    {
                        var query = Clients.Where(o => o.mac == item.mac).ToArray();
                        if (query.Length == 0) Clients.Add(item);
                    }
                    for (int i = 0; i < Clients.Count; i++)
                    {
                        var query = data.Where(o => o.mac == Clients[i].mac&&o.isWhiteDevice==true).ToArray();
                        if (query.Length > 0) Clients.RemoveAt(i);
                    }
                    RaisePropertyChanged("name");
                }

                public event PropertyChangedEventHandler PropertyChanged;
                public void RaisePropertyChanged(string name)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public enum DeviceType
        {
            Laptop, //笔记本
            MAC,  //MAC
            PAD, //平板
            Phone,  //电话
            TV,  //电视
            PC, //PC
            Windows, //Windows
            Android, //安卓
            Linux,  //Linux
            NAS_Server,  //网络存储器
            Printer,  //打印机
            Repeater, //网络设备
            Scanner,   //扫描仪
            Unknown_Wired,   //未知的有线设备
            Unknown_Wireless  //未知的无线设备
        }

        public class DeviceRate
        {
            public long rx { get; set; }
            public long tx { get; set; }
        }

        public class QosRuleList : List<QosRule>
        {
            public static QosRuleList Parse(string rule)
            {
                var res = new QosRuleList();
                try
                {
                    var rules = rule.Split('<');
                    foreach (var item in rules)
                    {
                        var data = item.Split('>');
                        if (data.Length == 5)
                        {
                            res.Add(new QosRule()
                            {
                                enable = (int.Parse(data[0]) == 1 ? true : false),
                                mac = data[1],
                                down = long.Parse(data[2]),
                                up = long.Parse(data[3]),
                                index = int.Parse(data[4])
                            });
                        }
                    }
                }
                catch (Exception)
                {

                }
                return res;
            }

            public int GetRuleIndex(string mac)
            {
                int index = -1;
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].mac == mac)
                        index = i;
                }
                return index;
            }

            public override string ToString()
            {
                string res = "";
                if (this != null)
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        this[i].index = i;
                        res += this[i];
                        if (i != this.Count - 1)
                            res += "<";
                    }
                }
                return res;
            }

            public bool Contains(string mac)
            {
                var query = this.Where(o => o.mac == mac).ToArray().Length;
                if (query > 0)
                    return true;
                else
                    return false;
            }

            public void AppendRule(QosRule rule)
            {
                int index = GetRuleIndex(rule.mac);
                if (index >= 0)
                {
                    this[index].enable = rule.enable;
                    this[index].up = rule.up;
                    this[index].down = rule.down;
                }
                else
                {
                    rule.index = this.Count;
                    this.Add(rule);
                }
            }
        }

        public class QosRule
        {
            public bool enable { get; set; }
            public string mac { get; set; }
            public long down { get; set; }
            public long up { get; set; }
            public int index { get; set; }

            public override string ToString()
            {
                return $"{(enable ? 1 : 0)}>{mac}>{down}>{up}>{index}";
            }
        }
    }
}