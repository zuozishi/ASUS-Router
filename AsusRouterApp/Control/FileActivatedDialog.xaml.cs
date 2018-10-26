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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace AsusRouterApp.Control
{
    public sealed partial class FileActivatedDialog : ContentDialog
    {
        private ClientDialogBrush dialogBrush { get; set; }
        private string Detail { get; set; } = "";

        public FileActivatedDialog(bool status)
        {
            this.InitializeComponent();
            dialogBrush = new ClientDialogBrush();
            if (status)
                Detail = Utils.AppResources.GetString("LoadDataSuccess");
            else
                Detail = Utils.AppResources.GetString("LoadDataError");
        }

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
                    mainGrid = Utils.UI.GetAcrylicBrush(AcrylicBackgroundSource.HostBackdrop, Edi.UWP.Helpers.UI.GetAccentColor(), Edi.UWP.Helpers.UI.GetAccentColor(), 0.3);
                    textBrush = new SolidColorBrush(Colors.White);
                }
                else
                {
                    //主页面白色
                    mainGrid = new SolidColorBrush(Edi.UWP.Helpers.UI.GetAccentColor());
                    textBrush = new SolidColorBrush(Colors.White);
                }
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
