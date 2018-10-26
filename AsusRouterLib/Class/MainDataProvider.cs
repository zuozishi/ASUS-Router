using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;

namespace AsusRouterApp.Class
{
    public class MainDataProvider
    {
        private TimeSpan updateSpan;

        private ThreadPoolTimer updateTimer;

        public Model.WANInfo wanInfo { get; set; }

        public Model.NetRate netRate { get; set; }

        public Model.CpuMemInfo cpuMemInfo { get; set; }

        public Model.Client clients { get; set; }

        public Dictionary<string, Model.DeviceRate> devRate { get; set; }

        public Model.WLANInfo wlanInfo { get; set; }

        public Model.NetSpeed netSpeed { get; set; }

        public Model.QosRuleList qosRuleList { get; set; }

        public string[] banList { get; set; }

        public MainDataProvider() { }

        public MainDataProvider(int second) {
            updateSpan = TimeSpan.FromSeconds(second);
            updateTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
                if (this.wanInfo == null) this.wanInfo = await RouterAPI.GetWANinfo();
                this.netRate = await RouterAPI.GetNetRate();
                this.cpuMemInfo = await RouterAPI.GetCpuMemInfo();
                this.clients = await RouterAPI.GetClients();
                this.devRate = await RouterAPI.GetDeviceRate();
                this.banList = await RouterAPI.FireWall.GetBanList();
                this.qosRuleList = await RouterAPI.GetQosRuleList();
                if (this.wlanInfo == null) this.wlanInfo = await RouterAPI.GetWLANInfo();
                DataUpdate?.Invoke();
            }, updateSpan);
        }

        public async Task<bool> ExportDemoData()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this);
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                savePicker.FileTypeChoices.Add("Json", new List<string>() { ".json" });
                savePicker.SuggestedFileName = "DemoData";
                var file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, json);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static async Task<MainDataProvider> GetDemoData()
        {
            try
            {
                var json = await PathIO.ReadTextAsync("ms-appx:///Assets/DemoData.json");
                var data= JsonConvert.DeserializeObject<MainDataProvider>(json);
                return data;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public delegate void DataUpdatedHandle();

        public event DataUpdatedHandle DataUpdate;
    }
}
