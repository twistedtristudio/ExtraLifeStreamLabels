using System;
using Newtonsoft.Json;

namespace WJS.DonorData
{
    public sealed partial class DonorDataModel : IDisposable
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("participantID")]
        public long ParticipantId { get; set; }

        [JsonProperty("amount")]
        public double? Amount { get; set; }

        [JsonProperty("donorID")]
        public string DonorId { get; set; }

        [JsonProperty("avatarImageURL")]
        public string AvatarImageUrl { get; set; }

        [JsonProperty("createdDateUTC")]
        public string CreatedDateUtc { get; set; }

        [JsonProperty("teamID")]
        public long TeamId { get; set; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public sealed partial class DonorDataModel
    {
        public static DonorDataModel[] FromJson(string json) => JsonConvert.DeserializeObject<DonorDataModel[]>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this DonorDataModel[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
