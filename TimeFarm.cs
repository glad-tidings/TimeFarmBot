using System.Text.Json.Serialization;

namespace TimeFarm
{
    public class TimeFarmQuery
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string API_ID { get; set; } = string.Empty;
        public string API_HASH { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool Farming { get; set; }
        public bool FriendBonus { get; set; }
        public bool Task { get; set; }
        public int[]? TaskSleep { get; set; }
        public bool DailyQuestions { get; set; }
        public bool Upgrade { get; set; }
        public int[]? DaySleep { get; set; }
        public int[]? NightSleep { get; set; }
    }

    public class TimeFarmAccessTokenRequest
    {
        [JsonPropertyName("initData")]
        public string InitData { get; set; } = string.Empty;
    }

    public class TimeFarmAccessTokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
        [JsonPropertyName("flaggedByAdmin")]
        public bool FlaggedByAdmin { get; set; }
        [JsonPropertyName("isPartner")]
        public bool IsPartner { get; set; }
        [JsonPropertyName("info")]
        public TimeFarmAccessTokenInfo? Info { get; set; }
        [JsonPropertyName("levelDescriptions")]
        public List<TimeFarmAccessTokenLevelDescriptions>? LevelDescriptions { get; set; }
        [JsonPropertyName("balanceInfo")]
        public TimeFarmAccessTokenBalance? BalanceInfo { get; set; }
        [JsonPropertyName("serverTime")]
        public DateTime ServerTime { get; set; }
    }

    public class TimeFarmAccessTokenInfo
    {
        [JsonPropertyName("onboardingCompleted")]
        public bool OnboardingCompleted { get; set; }
        [JsonPropertyName("level")]
        public int Level { get; set; }
    }

    public class TimeFarmAccessTokenLevelDescriptions
    {
        [JsonPropertyName("level")]
        public string Level { get; set; } = string.Empty;
        [JsonPropertyName("price")]
        public int Price { get; set; }
    }

    public class TimeFarmAccessTokenBalance
    {
        [JsonPropertyName("balance")]
        public int balance { get; set; }
        [JsonPropertyName("referral")]
        public TimeFarmAccessTokenBalanceReferral Referral { get; set; } = new TimeFarmAccessTokenBalanceReferral();
        [JsonPropertyName("autofarm")]
        public bool AutoFarm { get; set; }
    }

    public class TimeFarmAccessTokenBalanceReferral
    {
        [JsonPropertyName("availableBalance")]
        public int AvailableBalance { get; set; }
        [JsonPropertyName("claimedBalance")]
        public int ClaimedBalance { get; set; }
    }

    public class TimeFarmFarmingInfoResponse
    {
        [JsonPropertyName("balance")]
        public string Balance { get; set; } = string.Empty;
        [JsonPropertyName("activeFarmingStartedAt")]
        public DateTime? ActiveFarmingStartedAt { get; set; }
        [JsonPropertyName("farmingDurationInSec")]
        public int FarmingDurationInSec { get; set; }
        [JsonPropertyName("farmingReward")]
        public int FarmingReward { get; set; }
    }

    public class TimeFarmTaskResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("reward")]
        public int Reward { get; set; }
        [JsonPropertyName("submission")]
        public TimeFarmTaskSubmission Submission { get; set; } = new TimeFarmTaskSubmission();
    }

    public class TimeFarmTaskSubmission
    {
        [JsonPropertyName("reward")]
        public int Reward { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    public class TimeFarmTaskSubmitRequest
    {
        [JsonPropertyName("taskId")]
        public string TaskId { get; set; } = string.Empty;
    }

    public class TimeFarmTaskSubmitResponse
    {
        [JsonPropertyName("result")]
        public TimeFarmTaskSubmitResult? Result { get; set; }
    }

    public class TimeFarmTaskSubmitResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    public class TimeFarmAnswerResponse
    {
        [JsonPropertyName("expire")]
        public DateTime Expire { get; set; }
        [JsonPropertyName("answer")]
        public string Answer { get; set; } = string.Empty;
    }

    public class TimeFarmDailyQuestionsResponse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;
        [JsonPropertyName("reward")]
        public int Reward { get; set; }
        [JsonPropertyName("answer")]
        public TimeFarmDailyQuestionsAnswer Answer { get; set; } = new TimeFarmDailyQuestionsAnswer();
    }

    public class TimeFarmDailyQuestionsAnswer
    {
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
        [JsonPropertyName("isCorrect")]
        public bool IsCorrect { get; set; }
        [JsonPropertyName("rewardAssigned")]
        public int RewardAssigned { get; set; }
    }

    public class TimeFarmDailyQuestionRequest
    {
        [JsonPropertyName("answer")]
        public string Answer { get; set; } = string.Empty;
    }

    public class TimeFarmDailyQuestionResponse
    {
        [JsonPropertyName("isCorrect")]
        public bool IsCorrect { get; set; }
    }

    public class ProxyType
    {
        [JsonPropertyName("Index")]
        public int Index { get; set; }
        [JsonPropertyName("Proxy")]
        public string Proxy { get; set; } = string.Empty;
    }

    public class Httpbin
    {
        [JsonPropertyName("origin")]
        public string Origin { get; set; } = string.Empty;
    }
}