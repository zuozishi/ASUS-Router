using AsusRouterApp.Class;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
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
    public sealed partial class QrCodeDialog : ContentDialog
    {
        private byte[] qrCodeImageBmp;
        private InMemoryRandomAccessStream stream;

        private string title { get; set; } = "Title";
        private string Description { get; set; } = "Description";
        private ClientDialogBrush dialogBrush { get; set; }
        BitmapImage image { get; set; }

        public QrCodeDialog(string title,string description, QRCodeData qrData)
        {
            this.title = title;
            this.Description = description;
            this.InitializeComponent();
            dialogBrush = new ClientDialogBrush();
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrData);
            qrCodeImageBmp = qrCode.GetGraphic(20);
            LoadQrImage();
        }

        private async void LoadQrImage()
        {
            using (stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(qrCodeImageBmp);
                    await writer.StoreAsync();
                }
                image = new BitmapImage();
                await image.SetSourceAsync(stream);
            }
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
            args.Cancel = true;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (stream != null)
            {
                DataRequest request = args.Request;
                request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
                request.Data.Properties.Title = title;
                request.Data.Properties.Description = Description;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
