using System;
using Newtonsoft.Json;

namespace WJS.ParticipantData
{
    public sealed partial class ParticipantDataModel : IDisposable
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("fundraisingGoal")]
        public double FundraisingGoal { get; set; }

        [JsonProperty("participantID")]
        public long ParticipantId { get; set; }

        [JsonProperty("teamName")]
        public string TeamName { get; set; }

        [JsonProperty("eventName")]
        public string EventName { get; set; }

        [JsonProperty("avatarImageURL")]
        public string AvatarImageUrl { get; set; }

        [JsonProperty("createdDateUTC")]
        public string CreatedDateUtc { get; set; }

        [JsonProperty("eventID")]
        public long EventId { get; set; }

        [JsonProperty("sumDonations")]
        public double SumDonations { get; set; }

        [JsonProperty("teamID")]
        public long TeamId { get; set; }

        [JsonProperty("isTeamCaptain")]
        public bool IsTeamCaptain { get; set; }

        [JsonProperty("numDonations")]
        public long NumDonations { get; set; }

        public double GetPercentOfGoalReached()
        {
            return SumDonations / FundraisingGoal * 100;
        }

        public bool Equals(ParticipantDataModel other)
        {
            return other != null && other.GetHashCode() == GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ParticipantDataModel data)
                return Equals(data);
            return false;
        }

        public override int GetHashCode()
        {
            var hashcode = 159487263;
            hashcode = hashcode * -147852369 + NumDonations.GetHashCode() + FundraisingGoal.GetHashCode() + SumDonations.GetHashCode();
            return hashcode;
        }

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

    public sealed partial class ParticipantDataModel
    {
        public static ParticipantDataModel FromJson(string json) => JsonConvert.DeserializeObject<ParticipantDataModel>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ParticipantDataModel self) => JsonConvert.SerializeObject(self, Converter.Settings);
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
