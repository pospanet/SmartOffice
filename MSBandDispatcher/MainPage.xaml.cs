using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.Band;

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
                .ContinueWith(t => ListBandsAsync(t.Result, BandsList));
        }

        private static async Task<IEnumerable<BandManager>> ListBandsAsync(
            IDictionary<IBandInfo, IBandClient> bands, ItemsControl bandsList)
        {
            List<BandManager> managerList = new List<BandManager>();
            foreach (KeyValuePair<IBandInfo, IBandClient> band in bands)
            {
                BandManager bandManager = new BandManager(band.Key, band.Value);
                await bandManager.StartReading();
                managerList.Add(bandManager);
                bandsList.Items.Add(bandManager);
            }
            return managerList;
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
                bandDictionary.Add(band, bandClient);
            }
            return bandDictionary;
        }
    }
}