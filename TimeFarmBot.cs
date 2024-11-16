using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TimeFarm
{

    public class TimeFarmBot
    {

        public readonly TimeFarmQuery PubQuery;
        private readonly ProxyType[] PubProxy;
        private readonly string AccessToken;
        public readonly TimeFarmAccessTokenResponse UserDetail;
        public readonly bool HasError;
        public readonly string ErrorMessage;
        public readonly string IPAddress;

        public TimeFarmBot(TimeFarmQuery Query, ProxyType[] Proxy)
        {
            PubQuery = Query;
            PubProxy = Proxy;
            IPAddress = GetIP().Result;
            PubQuery.Auth = getSession().Result;
            var GetToken = TimeFarmGetToken().Result;
            if (GetToken is not null)
            {
                AccessToken = GetToken.Token;
                UserDetail = GetToken;
                HasError = false;
                ErrorMessage = "";
            }
            else
            {
                AccessToken = string.Empty;
                UserDetail = new();
                HasError = true;
                ErrorMessage = "get token failed";
            }
        }

        private async Task<string> GetIP()
        {
            HttpClient client;
            var FProxy = PubProxy.Where(x => (long)x.Index == PubQuery.Index);
            if (FProxy.Count() != 0)
            {
                if (!string.IsNullOrEmpty(FProxy.ElementAtOrDefault(0)?.Proxy))
                {
                    var handler = new HttpClientHandler() { Proxy = new WebProxy() { Address = new Uri(FProxy.ElementAtOrDefault(0)?.Proxy ?? string.Empty) } };
                    client = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 30) };
                }
                else
                    client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            }
            else
                client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await client.GetAsync($"https://httpbin.org/ip");
            }
            catch { }
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<Httpbin>(responseStream);
                    return responseJson.Origin;
                }
            }

            return "";
        }

        private async Task<string> getSession()
        {
            var vw = new TelegramMiniApp.WebView(PubQuery.API_ID, PubQuery.API_HASH, PubQuery.Name, PubQuery.Phone, "TimeFarmCryptoBot", "https://timefarm.app/");
            string url = await vw.Get_URL();
            if (!string.IsNullOrEmpty(url))
                return url.Split(new string[] { "tgWebAppData=" }, StringSplitOptions.None)[1].Split(new string[] { "&tgWebAppVersion" }, StringSplitOptions.None)[0];

            return "";
        }

        private async Task<TimeFarmAccessTokenResponse?> TimeFarmGetToken()
        {
            var TFAPI = new TimeFarmApi(0, PubQuery.Auth, PubQuery.Index, PubProxy);
            var request = new TimeFarmAccessTokenRequest() { InitData = PubQuery.Auth };
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await TFAPI.TFAPIPost("https://tg-bot-tap.laborx.io/api/v1/auth/validate-init/v2", serializedRequestContent);
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<TimeFarmAccessTokenResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<TimeFarmAccessTokenBalance?> TimeFarmBalance()
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var httpResponse = await TFAPI.TFAPIGet($"https://tg-bot-tap.laborx.io/api/v1/balance");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<TimeFarmAccessTokenBalance>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<TimeFarmFarmingInfoResponse?> TimeFarmFarmingInfo()
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var httpResponse = await TFAPI.TFAPIGet($"https://tg-bot-tap.laborx.io/api/v1/farming/info");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<TimeFarmFarmingInfoResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> TimeFarmClaimFarming()
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var httpResponse = await TFAPI.TFAPIPost("https://tg-bot-tap.laborx.io/api/v1/farming/finish", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }

        public async Task<bool> TimeFarmStartFarming()
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var httpResponse = await TFAPI.TFAPIPost("https://tg-bot-tap.laborx.io/api/v1/farming/start", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }

        public async Task<bool> TimeFarmClaimFriends()
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var httpResponse = await TFAPI.TFAPIPost("https://tg-bot-tap.laborx.io/api/v1/balance/referral/claim", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }

        public async Task<List<TimeFarmTaskResponse>?> TimeFarmTasks()
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var httpResponse = await TFAPI.TFAPIGet($"https://tg-bot-tap.laborx.io/api/v1/tasks");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<List<TimeFarmTaskResponse>>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> TimeFarmSubmitTask(string TaskID)
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var request = new TimeFarmTaskSubmitRequest() { TaskId = TaskID };
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await TFAPI.TFAPIPost("https://tg-bot-tap.laborx.io/api/v1/tasks/submissions", serializedRequestContent);
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<TimeFarmTaskSubmitResponse>(responseStream);
                    return !string.IsNullOrEmpty(responseJson?.Result?.Status);
                }
            }

            return false;
        }

        public async Task<bool> TimeFarmClaimTask(string TaskID)
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var httpResponse = await TFAPI.TFAPIPost($"https://tg-bot-tap.laborx.io/api/v1/tasks/{TaskID}/claims", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }

        public async Task<int> TimeFarmAnswer()
        {
            var client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true, NoStore = true, MaxAge = TimeSpan.FromSeconds(0d) };
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await client.GetAsync("https://raw.githubusercontent.com/glad-tidings/TimeFarmBot/refs/heads/main/question.json");
            }
            catch { }
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<TimeFarmAnswerResponse>(responseStream);
                    if (responseJson?.Expire.ToLocalTime() > DateTime.Now)
                        return await this.TimeFarmClaimDailyQuestions(responseJson.Answer);
                }
            }

            return 1;
        }

        public async Task<TimeFarmDailyQuestionsResponse?> TimeFarmDailyQuestions()
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var httpResponse = await TFAPI.TFAPIGet($"https://tg-bot-tap.laborx.io/api/v1/daily-questions");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<TimeFarmDailyQuestionsResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<int> TimeFarmClaimDailyQuestions(string answer)
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var request = new TimeFarmDailyQuestionRequest() { Answer = answer };
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await TFAPI.TFAPIPost("https://tg-bot-tap.laborx.io/api/v1/daily-questions", serializedRequestContent);
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<TimeFarmDailyQuestionResponse>(responseStream);
                    return (responseJson.IsCorrect ? 2 : 0);
                }
            }

            return 0;
        }

        public async Task<bool> TimeFarmUpgrade()
        {
            var TFAPI = new TimeFarmApi(1, AccessToken, PubQuery.Index, PubProxy);
            var httpResponse = await TFAPI.TFAPIPost($"https://tg-bot-tap.laborx.io/api/v1/me/level/upgrade", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }
    }
}