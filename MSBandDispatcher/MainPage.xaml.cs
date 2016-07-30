using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Microsoft.Band;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pospa.NET.SmartOffice.MSBandDispatcher
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            InitBandsAsync()
                .ContinueWith(t => ListBandsAsync(t.Result, BandList)
                    .ContinueWith(t2 => EnableTourAsync().ContinueWith(t3=>Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            lblDeviceName.Text = string.Format(lblDeviceName.Text, BandManager.GroupId);
                        }))));
        }

        private async Task EnableTourAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                btnGo.Content = "Start Tour";
                btnGo.IsEnabled = true;
            });
        }

        private async Task ListBandsAsync(
            IDictionary<IBandInfo, IBandClient> bands, ItemsControl bandsList)
        {
            List<BandManager> managerList = bands.Select(band => new BandManager(band.Key, band.Value)).ToList();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { bandsList.ItemsSource = managerList; });

        }

        private static async Task<IDictionary<IBandInfo, IBandClient>> InitBandsAsync()
        {
            Dictionary<IBandInfo, IBandClient> bandDictionary = new Dictionary<IBandInfo, IBandClient>();
            IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
            foreach (IBandInfo band in pairedBands)
            {
                IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(band);
                if (bandClient.SensorManager.HeartRate.GetCurrentUserConsent() != UserConsent.Granted)
                {
                    await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                }
                if (bandClient.SensorManager.Pedometer.GetCurrentUserConsent() != UserConsent.Granted)
                {
                    await bandClient.SensorManager.Pedometer.RequestUserConsentAsync();
                }
                await BandManager.RegisterTile(bandClient);
                bandDictionary.Add(band, bandClient);
            }
            return bandDictionary;
        }

        private async void btnGo_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (btnGo.Tag == null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    btnGo.IsEnabled = false;
                });
                IEnumerable<BandManager> bandManagers = (IEnumerable<BandManager>) BandList.ItemsSource;
                if (bandManagers == null) return;
                foreach (BandManager bandManager in bandManagers)
                {
                    await bandManager.StartReadingAsync();
                }
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    btnGo.Content = "Stop Tour";
                    btnGo.IsEnabled = true;
                });
                btnGo.Tag = new object();
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    btnGo.IsEnabled = false;
                });
                IEnumerable<BandManager> bandManagers = (IEnumerable<BandManager>)BandList.ItemsSource;
                if (bandManagers == null) return;
                foreach (BandManager bandManager in bandManagers)
                {
                    await bandManager.StopReadingAsync();
                    await bandManager.ShowIdAsync();
                    await SaveDataToAzureAsync(bandManager.GuestId, BandManager.GroupId, bandManager.StepCount);
                }
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    btnGo.Content = "Start";
                    btnGo.IsEnabled = true;
                }); btnGo.Tag = null;
            }
        }

        private static async Task SaveDataToAzureAsync(string name, string group, long steps)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Configuration.StorageAccount.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("GuidedTourData");
            await table.CreateIfNotExistsAsync();
            AttendeeData data = new AttendeeData(name, group, steps);
            TableOperation insertOperation = TableOperation.InsertOrReplace(data);
            await table.ExecuteAsync(insertOperation);
        }
    }
}