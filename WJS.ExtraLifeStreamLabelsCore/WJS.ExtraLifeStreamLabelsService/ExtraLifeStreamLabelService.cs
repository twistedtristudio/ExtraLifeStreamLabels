using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WJS.DonorData;
using WJS.ParticipantData;

namespace WJS.ExtraLifeStreamLabelsService
{
    public class ExtraLifeStreamLabelService : BackgroundService
    {
        private readonly ILogger<ExtraLifeStreamLabelService> _logger;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private const string Url = "https://www.extra-life.org/api/";
        private static string _participantEndpoint;
        private string _donationsEndpoint;
        private string _teamEndpoint;
        private static ParticipantDataModel _previousParticipantData;
        private static IConfigurationRoot _config;

        private static string GetAssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public ExtraLifeStreamLabelService(ILogger<ExtraLifeStreamLabelService> logger)
        {
            _logger = logger;
            var builder = new ConfigurationBuilder().SetBasePath(GetAssemblyDirectory).AddJsonFile("appsettings.json");
            _config = builder.Build();
            _participantEndpoint =
                $"participants/{_config.GetSection("ExtraLifeData").Get<ExtraLifeData>().ParticipantId}";

            _donationsEndpoint = $"{_participantEndpoint}/donations";
            _teamEndpoint = $"teams/{_config.GetSection("ExtraLifeData").Get<ExtraLifeData>().TeamId}";
            _previousParticipantData = new ParticipantDataModel();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!Directory.Exists($"{_config.GetSection("ExtraLifeData").Get<ExtraLifeData>().StreamLabelOutputPath}"))
                Directory.CreateDirectory($"{_config.GetSection("ExtraLifeData").Get<ExtraLifeData>().StreamLabelOutputPath}");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("ExtraLifeStreamLabelService running at: {time}", DateTimeOffset.Now);

                await GetParticipantData();
                await Task.Delay(new TimeSpan(0, 0, 0, 30), stoppingToken);

                //await Task.Delay(30000, stoppingToken);
            }
        }

        private async Task GetParticipantData()
        {
            using (var clientParticipant = new HttpClient())
            {
                try
                {
                    clientParticipant.BaseAddress = new Uri(Url);
                    HttpResponseMessage responseParticipant = await clientParticipant.GetAsync(_participantEndpoint, _cts.Token);
                    if (responseParticipant.IsSuccessStatusCode)
                    {
                        using (ParticipantDataModel currentParticipantData =
                            ParticipantDataModel.FromJson(responseParticipant.Content.ReadAsStringAsync().Result))
                        {
                            if (!currentParticipantData.Equals(_previousParticipantData))
                            {
                                _previousParticipantData = currentParticipantData;
                                CreateParticipantStreamLabel(currentParticipantData);

                                await GetDonationData();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to get participant data");
                    //Logger.Error(e);
                }
            }
        }

        private static void CreateParticipantStreamLabel(ParticipantDataModel currentParticipantData)
        {
            using (FileStream progressDataStream = new FileStream($"{_config.GetSection("ExtraLifeData").Get<ExtraLifeData>().StreamLabelOutputPath}//ExtraLifeProgress.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                int percentage =
                    Convert.ToInt32(currentParticipantData.GetPercentOfGoalReached());
                string progress =
                    $"${currentParticipantData.SumDonations:N2} / ${currentParticipantData.FundraisingGoal:N2} ( {percentage}% )";
                progressDataStream.SetLength(0);
                progressDataStream.Write(new UTF8Encoding(true).GetBytes(progress), 0,
                    progress.Length);
                Console.WriteLine($"{progress}");
            }
        }

        private async Task GetDonationData()
        {
            using (HttpClient clientDonations = new HttpClient())
            {
                try
                {
                    clientDonations.BaseAddress = new Uri(Url);
                    HttpResponseMessage responseDonations =
                        await clientDonations.GetAsync(_donationsEndpoint, _cts.Token);
                    if (responseDonations.IsSuccessStatusCode)
                    {
                        CreateDonorInfoStreamLabels(responseDonations);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to get donation data");
                    //Logger.Error(e);
                }
            }
        }

        private static void CreateDonorInfoStreamLabels(HttpResponseMessage responseDonations)
        {
            DonorDataModel[] donorList =
                DonorDataModel.FromJson(responseDonations.Content.ReadAsStringAsync().Result);

            using (FileStream lastDonorData = new FileStream($"{_config.GetSection("ExtraLifeData").Get<ExtraLifeData>().StreamLabelOutputPath}//ExtraLifeMostRecentDonation.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                if (donorList.Any())
                {
                    DonorDataModel mostRecentDataModel = donorList[0];
                    string donation = $"{GetDonorName(mostRecentDataModel)}{GetDonationAmount(mostRecentDataModel)}    ";  //$"{donorName}{donationAmount}";
                    lastDonorData.SetLength(0);
                    lastDonorData.Write(new UTF8Encoding(true).GetBytes(donation), 0, donation.Length);
                    Console.WriteLine(donation);
                }
            }

            using (FileStream fullDonorListData = new FileStream(
                $"{_config.GetSection("ExtraLifeData").Get<ExtraLifeData>().StreamLabelOutputPath}//ExtraLifeFullDonorList.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                StringBuilder sb = new StringBuilder();
                string donation = string.Empty;
                fullDonorListData.SetLength(0);
                List<DonorDataModel> fullDonorList = donorList.ToList();
                foreach (var donor in fullDonorList)
                {
                    sb.Append($"{GetDonorName(donor)}{GetDonationAmount(donor)}    ");
                }

                string fullDonorData = sb.ToString();
                fullDonorListData.Write(new UTF8Encoding(true).GetBytes(fullDonorData), 0, fullDonorData.Length);
            }

            using (FileStream fullDonorListWithMessageData = new FileStream(
                $"{_config.GetSection("ExtraLifeData").Get<ExtraLifeData>().StreamLabelOutputPath}//ExtraLifeFullDonorListWithMessages.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                StringBuilder sb = new StringBuilder();
                string donation = string.Empty;
                fullDonorListWithMessageData.SetLength(0);
                List<DonorDataModel> fullDonorList = donorList.ToList();
                foreach (var donor in fullDonorList)
                {
                    sb.Append($"{GetDonorName(donor)}{GetDonationAmount(donor)}{GetDonationMessage(donor)}    ");
                }

                string fullDonorData = sb.ToString();
                fullDonorListWithMessageData.Write(new UTF8Encoding(true).GetBytes(fullDonorData), 0, fullDonorData.Length);
            }
        }

        private static string GetDonorName(DonorDataModel donorDataModel)
        {
            return donorDataModel.DisplayName ?? "Anonymous"; //Resources.AnonymousDonorName;
        }

        private static string GetDonationAmount(DonorDataModel donorDataModel)
        {
            return donorDataModel.Amount == null ? string.Empty : $": ${donorDataModel.Amount:N2}";
        }

        private static string GetDonationMessage(DonorDataModel donorDataModel)
        {
            return donorDataModel.Message == null ? string.Empty : $" Message: {donorDataModel.Message}";
        }
    }

    public class ExtraLifeData
    {
        public string ParticipantId { get; set; }
        public string TeamId { get; set; }
        public string StreamLabelOutputPath { get; set; }
    }
}
