using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AsusRouterApp.Class
{
    public class RouterAPI
    {
        public class Url
        {
            public static string Host
            {
                get
                {
                    return (string)Setting.GetSetting("host", "");
                }
                set
                {
                    Setting.SetSetting("host", value);
                }
            }
            public static string Login = Host + "/login.cgi";
            public static string CpuMemInfo = Host + "/cpu_ram_status.xml";
            public static string AppGet = Host + "/appGet.cgi";
            public static string ApplyApp = Host + "/applyapp.cgi";
        }

        public static bool HasLogin()
        {
            string auth = (string)Setting.GetSetting("auth", "");
            if (auth == null || auth.Length == 0||auth=="test")
                return false;
            else
            {
                return true;
            }
        }

        public static async Task<bool> Login(string user, string pwd)
        {
            try
            {
                if(user=="test"&&pwd=="test")
                {
                    Setting.SetSetting("auth", "test");
                    Setting.DemoAccount = true;
                    return true;
                }
                else
                    Setting.DemoAccount = false;
                Setting.SetSetting("auth", Convert.ToBase64String(Encoding.UTF8.GetBytes(user + ":" + pwd)));
                return await Login();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static async Task<bool> Login()
        {
            try
            {
                string auth = (string)Setting.GetSetting("auth","");
                if (auth == "test")
                {
                    Setting.DemoAccount = true;
                    return true;
                }
                else
                    Setting.DemoAccount = false;
                if (auth == null || auth.Length == 0) return false;
                KeyValuePair<string, string>[] header = new KeyValuePair<string, string>[1] {
                new KeyValuePair<string, string>("Authorization","Basic "+auth)
            };
                KeyValuePair<string, string>[] data = new KeyValuePair<string, string>[1] {
                new KeyValuePair<string, string>("login_authorization",auth)
            };
                var json = await Http.Post(Url.Login, data, header);
                if (json != null)
                {
                    var obj = JObject.Parse(json);
                    var asus_token = (string)obj["asus_token"];
                    Http.Cookie = "asus_token=" + asus_token;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <returns></returns>
        public static async Task<Model.Client> GetClients()
        {
            try
            {
                var json = await AppGet(@"get_clientlist(appobj);wl_sta_list_2g(appobj);wl_sta_list_5g(appobj);");
                var obj = JObject.Parse(json);
                var res = new Model.Client();
                res.clientList = new Dictionary<string, Model.Client.ClientInfo>();
                var clientlist = (JObject)obj["get_clientlist"];
                foreach (var item in clientlist)
                {
                    if(item.Key!="maclist")
                        res.clientList.Add(item.Key, JsonConvert.DeserializeObject<Model.Client.ClientInfo>(item.Value.ToString()));
                }
                res.macList = JsonConvert.DeserializeObject<List<string>>(obj["get_clientlist"]["maclist"].ToString());
                var wl5g= (JObject)obj["wl_sta_list_5g"];
                res.wl5g = new Dictionary<string, Model.Client.WlanInfo>();
                foreach (var item in wl5g)
                {
                    res.wl5g.Add(item.Key, JsonConvert.DeserializeObject<Model.Client.WlanInfo>(item.Value.ToString()));
                }
                var wl2g = (JObject)obj["wl_sta_list_2g"];
                res.wl2g = new Dictionary<string, Model.Client.WlanInfo>();
                foreach (var item in wl2g)
                {
                    res.wl2g.Add(item.Key, JsonConvert.DeserializeObject<Model.Client.WlanInfo>(item.Value.ToString()));
                }
                res.Update();
                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取CPU和RAM信息
        /// </summary>
        /// <returns></returns>
        public static async Task<Model.CpuMemInfo> GetCpuMemInfo()
        {
            try
            {
                //var info = await AppGet<Model.CpuMemInfo>(@"cpu_usage(appobj);memory_usage(appobj);");
                string json = await AppGet(@"cpu_usage(appobj);memory_usage(appobj);");
                Model.CpuMemInfo info = JsonConvert.DeserializeObject<Model.CpuMemInfo>(json);
                return info;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取网络速率
        /// </summary>
        /// <returns></returns>
        public static async Task<Model.NetRate> GetNetRate()
        {
            try
            {
                var json = await AppGet(@"netdev(appobj);");
                var obj = JObject.Parse(json);
                return JsonConvert.DeserializeObject<Model.NetRate>(obj["netdev"].ToString());
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取客户端速率
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string,Model.DeviceRate>> GetDeviceRate()
        {
            try
            {
                var json = await AppGet(@"bwdpi_status(traffic, ,realtime, )");
                var array = (JArray)JObject.Parse(json)["bwdpi_status-traffic"];
                var res = new Dictionary<string, Model.DeviceRate>();
                foreach (var item in array)
                {
                    var device = (JArray)item;
                    res.Add((string)device[0],new Model.DeviceRate() {
                        rx=long.Parse((string)device[1]),
                        tx= long.Parse((string)device[2])
                    });
                }
                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取路由器及公网信息
        /// </summary>
        /// <returns></returns>
        public static async Task<Model.WANInfo> GetWANinfo()
        {
            return await AppGet<Model.WANInfo>(@"nvram_get(model);nvram_get(daapd_friendly_name);nvram_get(0:macaddr);nvram_get(ddns_hostname_x);wanlink(status);wanlink(statusstr);wanlink(ipaddr);wanlink(dns);");
        }

        /// <summary>
        /// 更改WLAN设置
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<bool> SetWlan(Model.WLANInfo data)
        {
            string str = "{ \"wl0_radio\": \"1\", \"wl0_ssid\": \"wifi320_1\", \"wl0_closed\": \"0\", \"wl0_nmode_x\": \"0\", \"wl0_bw\": \"0\", \"wl0_auth_mode_x\": \"psk2\", \"wl0_crypto\": \"aes\", \"wl0_wpa_psk\": \"www.123123\", \"wl1_radio\": \"1\", \"wl1_ssid\": \"wifi320_5G\", \"wl1_closed\": \"0\", \"wl1_nmode_x\": \"0\", \"wl1_bw\": \"0\", \"wl1_auth_mode_x\": \"psk2\", \"wl1_crypto\": \"aes\", \"wl1_wpa_psk\": \"www.123123\", \"wl2_radio\": \"\", \"wl2_ssid\": \"\", \"wl2_closed\": \"\", \"wl2_nmode_x\": \"\", \"wl2_bw\": \"\", \"wl2_auth_mode_x\": \"\", \"wl2_crypto\": \"\", \"wl2_wpa_psk\": \"\", \"action_mode\": \"apply\" }";
            var obj = JObject.Parse(str);
            obj["wl0_radio"] = data.wl0_radio;
            obj["wl0_ssid"] = data.wl0_ssid;
            obj["wl0_wpa_psk"] = data.wl0_wpa_psk;
            obj["wl1_radio"] = data.wl1_radio;
            obj["wl1_ssid"] = data.wl1_ssid;
            obj["wl1_wpa_psk"] = data.wl1_wpa_psk;
            var res = await ApplyApp(obj.ToString());
            if (res != null)
            {
                return await RestartWlanService();
            }
            else
                return false;
        }

        /// <summary>
        /// 重启路由器
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> Reboot()
        {
            var res = await ApplyApp("{ \"action_mode\": \"apply\", \"rc_service\": \"reboot\" }");
            if (res != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 重启WLAN服务
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RestartWlanService()
        {
            var res = await ApplyApp("{ \"action_mode\": \"apply\", \"rc_service\": \"restart_wireless\" }");
            if (res != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取WLAN信息
        /// </summary>
        /// <returns></returns>
        public static async Task<Model.WLANInfo> GetWLANInfo()
        {
            return await AppGet<Model.WLANInfo>(@"nvram_get(wl0_radio);nvram_get(wl0_ssid);nvram_get(wl0_wpa_psk);nvram_get(wl1_radio);nvram_get(wl1_ssid);nvram_get(wl1_wpa_psk);");
        }

        public class FireWall
        {
            /// <summary>
            /// 获取禁用Internet设备MAC列表
            /// </summary>
            /// <returns></returns>
            public static async Task<string[]> GetBanList()
            {
                try
                {
                    var res = new List<string>();
                    var json = await AppGet(@"nvram_get(MULTIFILTER_ALL);nvram_get(MULTIFILTER_ENABLE);nvram_get(MULTIFILTER_DEVICENAME);nvram_get(MULTIFILTER_MAC);nvram_get(MULTIFILTER_MACFILTER_DAYTIME);");
                    var obj = JObject.Parse(json);
                    var str = (string)obj["MULTIFILTER_MAC"];
                    if(str.Length>0)
                    {
                        if(str.Contains("&#62"))
                        {
                            var strs = str.Replace("&#62", "|").Split('|');
                            foreach (var item in strs)
                            {
                                res.Add(item);
                            }
                        }
                        else
                        {
                            res.Add(str);
                        }
                    }
                    return res.ToArray();
                }
                catch (Exception)
                {
                    return new List<string>().ToArray();
                }
            }

            /// <summary>
            /// 重启防火墙
            /// </summary>
            /// <returns></returns>
            public static async Task<bool> RestartFireWall()
            {
                var res = await ApplyApp("{ \"action_mode\": \"apply\", \"rc_service\": \"restart_firewall;restart_qos\" }");
                if (res != null)
                    return true;
                else
                    return false;
            }

            public static async Task<bool> SetBan(string mac)
            {
                bool hasBan = false;
                string enableStr = "";
                string daytimeStr = "";
                string json = "{ \"MULTIFILTER_ALL\": \"1\", \"MULTIFILTER_ENABLE\": \"{enable}\", \"MULTIFILTER_DEVICENAME\": \"\", \"MULTIFILTER_MAC\": \"{mac}\", \"MULTIFILTER_MACFILTER_DAYTIME\": \"{daytime}\", \"action_mode\": \"apply\", \"rc_service\": \"restart_firewall\" }";
                var banList = await GetBanList();
                var query = banList.Where(o => o == mac).ToArray();
                if (query != null && query.Length > 0) hasBan = true;
                string macStr = "";
                if (banList.Length == 0)
                {
                    macStr = mac;
                    enableStr = "1";
                    daytimeStr = "<";
                }
                else
                {
                    for (int i = 0; i < banList.Length; i++)
                    {
                        if(!(hasBan && banList[i] == mac))
                        {
                            macStr += banList[i];
                            enableStr += "1";
                            daytimeStr += "<";
                            if (i != banList.Length - 1&&!hasBan)
                            {
                                macStr += ">";
                                enableStr += ">";
                                daytimeStr += ">";
                            }
                        }
                    }
                    if (!hasBan)
                    {
                        macStr += ">" + mac;
                        enableStr += ">1";
                        daytimeStr += "><";
                    }
                }
                json = json.Replace("{mac}", macStr).Replace("{enable}", enableStr).Replace("{daytime}", daytimeStr);
                var res = await ApplyApp(json);
                if (res != null)
                    return true;
                else
                    return false;
            }
        }

        public static async Task<Model.QosRuleList> GetQosRuleList()
        {
            try
            {
                string json = await AppGet(@"nvram_get(qos_bw_rulelist);");
                if (json != null)
                {
                    string data = (string)JObject.Parse(json)["qos_bw_rulelist"];
                    data = data.Replace("&#62", ">").Replace("&#60", "<");
                    var res = Model.QosRuleList.Parse(data);
                    return res;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<bool> SetQosRuleList(Model.QosRuleList qosRules)
        {
            try
            {
                string json = "{ \"qos_enable\": \"1\", \"qos_type\": \"2\", \"qos_bw_rulelist\": \"{0}\", \"action_mode\": \"apply\" }";
                json = json.Replace("{0}", qosRules.ToString());
                var res = await ApplyApp(json);
                if (res != null)
                    return await FireWall.RestartFireWall();
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<string> ApplyApp(string data)
        {
            return await Http.Post(Url.ApplyApp, PCLWebUtility.WebUtility.UrlEncode(data));
        }

        public static async Task<T> AppGet<T>(string data)
        {
            try
            {
                var json = await AppGet(data);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        public static async Task<string> AppGet(string data)
        {
            KeyValuePair<string, string>[] postdata = new KeyValuePair<string, string>[1] {
                new KeyValuePair<string, string>("hook",data)
            };
            return await Http.Post(Url.AppGet, postdata);
        }
    }
}
