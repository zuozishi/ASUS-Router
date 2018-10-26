using AsusRouterApp.Class;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace AsusRouterBackgroundTask
{
    public sealed class DeviceToastTask : IBackgroundTask
    {
        List<Model.Client.ClientInfo> blackDevs;
        string[] banList;

        async void IBackgroundTask.Run(IBackgroundTaskInstance taskInstance)
        {
            var _taskDeferral = taskInstance.GetDeferral();
            await PopToast();
            _taskDeferral.Complete();
        }

        private async Task PopToast()
        {
            // Generate the toast notification content and pop the toast
            var loginRes = await RouterAPI.Login();
            if (!loginRes) return;
            var clients = await RouterAPI.GetClients();
            banList = await RouterAPI.FireWall.GetBanList();
            if (clients == null) return;
            if (clients.NotInBlackDevices.Count == 0) return;
            clients.Update();
            blackDevs = clients.NotInBlackDevices.Where(o=>banList.Contains(o.mac)==false).ToList();
            if (blackDevs.Count == 0) return;
            ToastContent content = GenerateToastContent();
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

        private ToastContent GenerateToastContent()
        {
            var toast = new ToastContent()
            {
                Launch = "action=deviceToastEvent&eventId=1983",
                Scenario = ToastScenario.Reminder,
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                            {
                                new AdaptiveText(){
                                    Text = $"有{blackDevs.Count}个非白名单设备连入路由器"
                                }
                            }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                        {
                            new ToastButton("进入APP查看","Select"),
                            new ToastButtonDismiss("忽略")
                        }
                }
            };
           
            if(blackDevs.Count<=2)
            {
                foreach (var item in blackDevs)
                {
                    toast.Visual.BindingGeneric.Children.Add(new AdaptiveText() { Text = item.name });
                }
            }
            else
            {
                toast.Visual.BindingGeneric.Children.Add(new AdaptiveText() { Text = blackDevs[0].name });
                toast.Visual.BindingGeneric.Children.Add(new AdaptiveText() { Text = blackDevs[1].name + " 忽略显示"+(blackDevs.Count - 1).ToString()+"个设备" });
            }
            return toast;
        }
    }
}
