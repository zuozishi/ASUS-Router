using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace AsusRouterApp.Class
{
    public class Setting
    {
        //人脉启动标识
        public static bool ContactStart = false;
        //商店审核h账号标识
        public static bool DemoAccount = false;
        //Toast通知启动标识
        public static bool ToastStart = false;
        //数据文件启动标识
        public static bool FileStart = false;
        public static string FileData = "";
        //调试窗口
        public static CoreApplicationView debugView;
        public static Frame debugFrame;

        public static ApplicationDataContainer localSetting = ApplicationData.Current.LocalSettings;

        public static StorageFolder localFloder = ApplicationData.Current.LocalFolder;

        public static void SetSetting(string key,string value)
        {
            localSetting.Values[key] = value;
        }

        public static object GetSetting(string key,string def)
        {
            if(localSetting.Values.ContainsKey(key))
            {
                return localSetting.Values[key];
            }
            else
            {
                localSetting.Values[key] = def;
                return def;
            }
        }

        public class Task
        {
            public static async Task<bool> isWhiteListToastTaskReg()
            {
                bool res = false;
                try
                {
                    var status = await BackgroundExecutionManager.RequestAccessAsync();
                    foreach (var cur in BackgroundTaskRegistration.AllTasks)
                    {
                        if (cur.Value.Name == "DeviceToastTask")
                        {
                            res = true;
                            return res;
                        }
                    }
                }
                catch (Exception e)
                {

                }
                return res;
            }

            /// <summary>
            /// 注册后台黑名单设备提醒服务
            /// </summary>
            public static async void RegWhiteListToastTask()
            {
                try
                {
                    var status = await BackgroundExecutionManager.RequestAccessAsync();
                    foreach (var cur in BackgroundTaskRegistration.AllTasks)
                    {
                        if (cur.Value.Name == "DeviceToastTask")
                        {
                            //cur.Value.Unregister(true);
                            return;
                        }
                    }
                    var builder = new BackgroundTaskBuilder();
                    builder.Name = "DeviceToastTask";
                    builder.TaskEntryPoint = "AsusRouterBackgroundTask.DeviceToastTask";
                    builder.SetTrigger(new TimeTrigger(15, false));
                    builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                    var task = builder.Register();
                }
                catch (Exception e)
                {

                }
            }

            /// <summary>
            ///注销后台黑名单设备提醒服务
            /// </summary>
            public static async void UnRegWhiteListToastTask()
            {
                try
                {
                    var status = await BackgroundExecutionManager.RequestAccessAsync();
                    foreach (var cur in BackgroundTaskRegistration.AllTasks)
                    {
                        if (cur.Value.Name == "DeviceToastTask")
                        {
                            cur.Value.Unregister(true);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        public class WhiteList
        {
            public static List<string> GetWhiteList()
            {
                string json = (string)GetSetting("DeviceWhiteList","[]");
                List<string> macs = JsonConvert.DeserializeObject<List<string>>(json);
                return macs;
            }

            public static bool IsWhiteDevice(string mac)
            {
                var macs = GetWhiteList();
                var query = macs.Where(o => o == mac).ToArray();
                if (query.Length > 0)
                    return true;
                else
                    return false;
            }

            public static void AddDevice(string mac)
            {
                if(!IsWhiteDevice(mac))
                {
                    var list = GetWhiteList();
                    list.Add(mac);
                    SetSetting("DeviceWhiteList", JsonConvert.SerializeObject(list));
                }
            }

            public static void RemoveDevice(string mac)
            {
                var list = GetWhiteList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == mac)
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
                SetSetting("DeviceWhiteList", JsonConvert.SerializeObject(list));
            }
        }

        public class Data
        {
            /// <summary>
            /// 导出数据
            /// </summary>
            /// <returns></returns>
            public static string Export()
            {
                var res = new DataModel();
                res.devName = DeviceName.GetDeviceName();
                res.devType = DeviceType.GetDeviceType();
                res.whiteList = WhiteList.GetWhiteList();
                return JsonConvert.SerializeObject(res);
            }

            /// <summary>
            /// 导入数据
            /// </summary>
            /// <param name="json"></param>
            public static bool Load(string json)
            {
                try
                {
                    DataModel data = JsonConvert.DeserializeObject<DataModel>(json);
                    if(data.devName!=null)
                    {
                        foreach (var mac in data.devName.Keys)
                        {
                            DeviceName.SetDeviceName(mac, data.devName[mac]);
                        }
                    }
                    if (data.devType != null)
                    {
                        foreach (var mac in data.devType.Keys)
                        {
                            DeviceType.SetDeviceType(mac, (Model.DeviceType)data.devType[mac]);
                        }
                    }
                    if (data.whiteList != null)
                    {
                        SetSetting("DeviceWhiteList", JsonConvert.SerializeObject(data.whiteList));
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static async void Share()
            {
                string json = Export();
                var file=await localFloder.CreateFileAsync("Asus Router Data.ard", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file,json);
                DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
                dataTransferManager.DataRequested += (s, args) =>{
                    DataRequest request = args.Request;
                    request.Data.Properties.Title = "Share Data File";
                    request.Data.SetStorageItems(new IStorageFile[] { file});
                };
                DataTransferManager.ShowShareUI();
            }

            public class DataModel
            {
                public Dictionary<string, string> devName { get; set; }
                public Dictionary<string, int> devType { get; set; }
                public List<string> whiteList { get; set; }
            }
        }

        public class DeviceName
        {
            public static Dictionary<string,string> GetDeviceName()
            {
                ApplicationDataContainer deviceNamesContainers = null;
                if (!localSetting.Containers.ContainsKey("decviceNames"))
                    localSetting.CreateContainer("decviceNames", ApplicationDataCreateDisposition.Always);
                else
                    deviceNamesContainers = localSetting.Containers["decviceNames"];
                var res = new Dictionary<string, string>();
                foreach (var item in deviceNamesContainers.Values.Keys)
                {
                    res.Add(item, (string)deviceNamesContainers.Values[item]);
                }
                return res;
            }

            public static string GetDeviceName(string mac, string def)
            {
                ApplicationDataContainer deviceNamesContainers = null;
                if (!localSetting.Containers.ContainsKey("decviceNames"))
                    localSetting.CreateContainer("decviceNames", ApplicationDataCreateDisposition.Always);
                else
                    deviceNamesContainers = localSetting.Containers["decviceNames"];
                if (deviceNamesContainers.Values.ContainsKey(mac))
                {
                    return (string)deviceNamesContainers.Values[mac];
                }
                else
                {
                    deviceNamesContainers.Values[mac] = def;
                    return def;
                }
            }

            public static void SetDeviceName(string mac, string name)
            {
                ApplicationDataContainer deviceNamesContainers = null;
                if (!localSetting.Containers.ContainsKey("decviceNames"))
                    localSetting.CreateContainer("decviceNames", ApplicationDataCreateDisposition.Always);
                else
                    deviceNamesContainers = localSetting.Containers["decviceNames"];
                deviceNamesContainers.Values[mac] = name;
            }

            public static bool ContainsDeviceName(string mac)
            {
                ApplicationDataContainer deviceNamesContainers = null;
                if (!localSetting.Containers.ContainsKey("decviceNames"))
                    localSetting.CreateContainer("decviceNames", ApplicationDataCreateDisposition.Always);
                else
                    deviceNamesContainers = localSetting.Containers["decviceNames"];
                return deviceNamesContainers.Values.ContainsKey(mac);
            }
        }

        public class DeviceType
        {
            public static Dictionary<string, int> GetDeviceType()
            {
                ApplicationDataContainer deviceNamesContainers = null;
                if (!localSetting.Containers.ContainsKey("decviceType"))
                    localSetting.CreateContainer("decviceType", ApplicationDataCreateDisposition.Always);
                else
                    deviceNamesContainers = localSetting.Containers["decviceType"];
                var res = new Dictionary<string, int>();
                foreach (var item in deviceNamesContainers.Values.Keys)
                {
                    res.Add(item, (int)deviceNamesContainers.Values[item]);
                }
                return res;
            }

            public static Model.DeviceType GetDeviceType(string mac, Model.DeviceType def)
            {
                ApplicationDataContainer deviceTypeContainers = null;
                if (!localSetting.Containers.ContainsKey("decviceType"))
                    localSetting.CreateContainer("decviceType", ApplicationDataCreateDisposition.Always);
                else
                    deviceTypeContainers = localSetting.Containers["decviceType"];
                if (deviceTypeContainers.Values.ContainsKey(mac))
                {
                    return (Model.DeviceType)((int)deviceTypeContainers.Values[mac]);
                }
                else
                {
                    deviceTypeContainers.Values[mac] = (int)def;
                    return def;
                }
            }

            public static void SetDeviceType(string mac, Model.DeviceType type)
            {
                ApplicationDataContainer deviceTypeContainers = null;
                if (!localSetting.Containers.ContainsKey("decviceType"))
                    localSetting.CreateContainer("decviceType", ApplicationDataCreateDisposition.Always);
                else
                    deviceTypeContainers = localSetting.Containers["decviceType"];
                deviceTypeContainers.Values[mac] = (int)type;
            }

            public static bool ContainsDeviceType(string mac)
            {
                ApplicationDataContainer deviceTypeContainers = null;
                if (!localSetting.Containers.ContainsKey("decviceType"))
                    localSetting.CreateContainer("decviceType", ApplicationDataCreateDisposition.Always);
                else
                    deviceTypeContainers = localSetting.Containers["decviceType"];
                return deviceTypeContainers.Values.ContainsKey(mac);
            }

            public static Model.DeviceType GetDefType(bool isWL,string name=null)
            {
                if(name!=null)
                {
                    if (name.ToLower().Contains("laptop"))
                    {
                        return Model.DeviceType.Laptop;
                    }
                    else if(name.ToLower().Contains("mac"))
                    {
                        return Model.DeviceType.MAC;
                    }
                    else if (name.ToLower().Contains("pad"))
                    {
                        return Model.DeviceType.PAD;
                    }
                    else if (name.ToLower().Contains("phone"))
                    {
                        return Model.DeviceType.Phone;
                    }
                    else if (name.ToLower().Contains("tv"))
                    {
                        return Model.DeviceType.TV;
                    }
                    else if (name.ToLower().Contains("pc"))
                    {
                        return Model.DeviceType.PC;
                    }
                    else if (name.ToLower().Contains("windows"))
                    {
                        return Model.DeviceType.Windows;
                    }
                    else if (name.ToLower().Contains("desktop"))
                    {
                        return Model.DeviceType.Windows;
                    }
                    else if (name.ToLower().Contains("android"))
                    {
                        return Model.DeviceType.Android;
                    }
                    else if (name.ToLower().Contains("linux"))
                    {
                        return Model.DeviceType.Linux;
                    }
                    else if (name.ToLower().Contains("ubuntu"))
                    {
                        return Model.DeviceType.Linux;
                    }
                    else if (name.ToLower().Contains("nas"))
                    {
                        return Model.DeviceType.NAS_Server;
                    }
                    else if (name.ToLower().Contains("printer"))
                    {
                        return Model.DeviceType.Printer;
                    }
                    else if (name.ToLower().Contains("scanner"))
                    {
                        return Model.DeviceType.Scanner;
                    }
                    else
                    {
                        if (isWL)
                            return Model.DeviceType.Unknown_Wireless;
                        else
                            return Model.DeviceType.Unknown_Wired;
                    }
                }
                else
                {
                    if(isWL)
                        return Model.DeviceType.Unknown_Wireless;
                    else
                        return Model.DeviceType.Unknown_Wired;
                }
            }
        }
    }
}
