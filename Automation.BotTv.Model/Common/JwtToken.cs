using Newtonsoft.Json;

namespace Automation.BotTv.Model
{
    public record JwtToken
    {
        public int Id { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
