using Microsoft.VisualBasic;
using System.Text.Json;

namespace TimeFarm
{
    static class Program
    {

        private static ProxyType[]? proxies;

        static List<TimeFarmQuery>? LoadQuery()
        {
            try
            {
                var contents = File.ReadAllText(@"data.txt");
                return JsonSerializer.Deserialize<List<TimeFarmQuery>>(contents);
            }
            catch { }

            return null;
        }

        static ProxyType[]? LoadProxy()
        {
            try
            {
                var contents = File.ReadAllText(@"proxy.txt");
                return JsonSerializer.Deserialize<ProxyType[]>(contents);
            }
            catch { }

            return null;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("  _____ _                _____                    ____   ___ _____ \r\n |_   _(_)_ __ ___   ___|  ___|_ _ _ __ _ __ ___ | __ ) / _ \\_   _|\r\n   | | | | '_ ` _ \\ / _ \\ |_ / _` | '__| '_ ` _ \\|  _ \\| | | || |  \r\n   | | | | | | | | |  __/  _| (_| | |  | | | | | | |_) | |_| || |  \r\n   |_| |_|_| |_| |_|\\___|_|  \\__,_|_|  |_| |_| |_|____/ \\___/ |_|  \r\n                                                                   ");
            Console.WriteLine();
            Console.WriteLine("Github: https://github.com/glad-tidings/TimeFarmBot");
            Console.WriteLine();
            Console.Write("Select an option:\n1. Run bot\n2. Create session\n> ");
            string? opt = Console.ReadLine();

            var TimeFarmQueries = LoadQuery();
            proxies = LoadProxy();

            if (opt != null)
            {
                if (opt == "1")
                {
                    foreach (var Query in TimeFarmQueries ?? [])
                    {
                        var BotThread = new Thread(() => TimeFarmThread(Query)); BotThread.Start();
                        Thread.Sleep(60000);
                    }
                }
                else
                {
                    foreach (var Query in TimeFarmQueries ?? [])
                    {
                        if (!File.Exists(@$"sessions\{Query.Name}.session"))
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Create session for account {Query.Name} ({Query.Phone})");
                            TelegramMiniApp.WebView vw = new(Query.API_ID, Query.API_HASH, Query.Name, Query.Phone, "", "");
                            if (vw.Save_Session().Result)
                                Console.WriteLine("Session created");
                            else
                                Console.WriteLine("Create session failed");
                        }
                    }

                    Environment.Exit(0);
                }
            }
        }

        public async static void TimeFarmThread(TimeFarmQuery Query)
        {
            while (true)
            {
                var RND = new Random();

                try
                {
                    var Bot = new TimeFarmBot(Query, proxies ?? []);
                    if (!Bot.HasError)
                    {
                        Log.Show("TimeFarm", Query.Name, $"my ip '{Bot.IPAddress}'", ConsoleColor.White);
                        Log.Show("TimeFarm", Query.Name, $"synced successfully. B<{Bot.UserDetail.BalanceInfo.balance}> L<{Bot.UserDetail.Info.Level}>", ConsoleColor.Blue);

                        if (Query.Farming)
                        {
                            var farming = await Bot.TimeFarmFarmingInfo();
                            if (farming is not null)
                            {
                                if (farming.ActiveFarmingStartedAt.HasValue)
                                {
                                    if (farming.ActiveFarmingStartedAt.Value.ToLocalTime().AddHours(4d) < DateTime.Now)
                                    {
                                        bool claimFarming = await Bot.TimeFarmClaimFarming();
                                        if (claimFarming)
                                            Log.Show("TimeFarm", Query.Name, $"farming claimed", ConsoleColor.Green);
                                        else
                                            Log.Show("TimeFarm", Query.Name, $"claim farming failed", ConsoleColor.Red);

                                        Thread.Sleep(3000);

                                        bool startFarming = await Bot.TimeFarmStartFarming();
                                        if (startFarming)
                                            Log.Show("TimeFarm", Query.Name, $"new farming started", ConsoleColor.Green);
                                        else
                                            Log.Show("TimeFarm", Query.Name, $"start new farming failed", ConsoleColor.Red);

                                        Thread.Sleep(3000);
                                    }
                                }
                                else
                                {
                                    bool startFarming = await Bot.TimeFarmStartFarming();
                                    if (startFarming)
                                        Log.Show("TimeFarm", Query.Name, $"new farming started", ConsoleColor.Green);
                                    else
                                        Log.Show("TimeFarm", Query.Name, $"start new farming failed", ConsoleColor.Red);

                                    Thread.Sleep(3000);
                                }
                            }
                        }

                        if (Query.FriendBonus)
                        {
                            if (Bot.UserDetail.BalanceInfo.Referral.AvailableBalance > 0)
                            {
                                bool claimFriends = await Bot.TimeFarmClaimFriends();
                                if (claimFriends)
                                    Log.Show("TimeFarm", Query.Name, $"friends bonus claimed", ConsoleColor.Green);
                                else
                                    Log.Show("TimeFarm", Query.Name, $"claim friends bonus failed", ConsoleColor.Red);

                                Thread.Sleep(3000);
                            }
                        }

                        if (Query.DailyQuestions)
                        {
                            var question = await Bot.TimeFarmDailyQuestions();
                            if (question is not null)
                            {
                                if (question.Answer.RewardAssigned == 0)
                                {
                                    int answer = await Bot.TimeFarmAnswer();
                                    if (answer == 2)
                                        Log.Show("TimeFarm", Query.Name, $"daily question claimed", ConsoleColor.Green);
                                    else if (answer == 0)
                                        Log.Show("TimeFarm", Query.Name, $"claim daily question failed", ConsoleColor.Red);

                                    Thread.Sleep(3000);
                                }
                            }
                        }

                        if (Query.Task)
                        {
                            var tasks = await Bot.TimeFarmTasks();
                            if (tasks is not null)
                            {
                                foreach (var task in tasks.Where(x => string.IsNullOrEmpty(x.Submission.Status) & (x.Type == "TELEGRAM" | x.Type == "TWITTER" | x.Type == "YOUTUBE" | x.Type == "API_CHECK") & x.Title.EndsWith("trending apps") == false))
                                {
                                    var startTask = Bot.TimeFarmSubmitTask(task.Id);
                                    if (startTask is not null)
                                        Log.Show("TimeFarm", Query.Name, $"task '{task.Title}' started", ConsoleColor.Green);
                                    else
                                        Log.Show("TimeFarm", Query.Name, $"start task '{task.Title}' failed", ConsoleColor.Red);

                                    int eachtaskRND = RND.Next(Query.TaskSleep[0], Query.TaskSleep[1]);
                                    Thread.Sleep(eachtaskRND * 1000);
                                }

                                foreach (var task in tasks.Where(x => x.Submission.Status == "COMPLETED" & (x.Type == "TELEGRAM" | x.Type == "TWITTER" | x.Type == "YOUTUBE" | x.Type == "API_CHECK") & x.Title.EndsWith("trending apps") == false))
                                {
                                    var claimTask = Bot.TimeFarmClaimTask(task.Id);
                                    if (claimTask is not null)
                                        Log.Show("TimeFarm", Query.Name, $"task '{task.Title}' claimed", ConsoleColor.Green);
                                    else
                                        Log.Show("TimeFarm", Query.Name, $"claim task '{task.Title}' failed", ConsoleColor.Red);

                                    int eachtaskRND = RND.Next(Query.TaskSleep[0], Query.TaskSleep[1]);
                                    Thread.Sleep(eachtaskRND * 1000);
                                }
                            }
                        }

                        if (Query.Upgrade)
                        {
                            var level = Bot.UserDetail.LevelDescriptions.Where(x => (x.Level ?? "") == ((Bot.UserDetail.Info.Level + 1).ToString() ?? ""));
                            if (level.Count() == 1)
                            {
                                if (level.ElementAtOrDefault(0).Price < Bot.UserDetail.BalanceInfo.balance)
                                {
                                    bool upgrade = await Bot.TimeFarmUpgrade();
                                    if (upgrade)
                                    {
                                        Bot.UserDetail.Info.Level += 1;
                                        Log.Show("TimeFarm", Query.Name, $"level upgraded successfully", ConsoleColor.Green);
                                    }
                                    else
                                        Log.Show("TimeFarm", Query.Name, $"upgrade level failed", ConsoleColor.Red);
                                }
                            }
                        }

                        var Sync = await Bot.TimeFarmBalance();
                        if (Sync is not null)
                            Log.Show("TimeFarm", Query.Name, $"B<{Sync.balance}> L<{Bot.UserDetail.Info.Level}>", ConsoleColor.Blue);
                    }
                    else
                        Log.Show("TimeFarm", Query.Name, $"{Bot.ErrorMessage}", ConsoleColor.Red);
                }
                catch (Exception ex)
                {
                    Log.Show("TimeFarm", Query.Name, $"Error: {ex.Message}", ConsoleColor.Red);
                }

                int syncRND = 0;
                if (DateTime.Now.Hour < 8)
                    syncRND = RND.Next(Query.NightSleep[0], Query.NightSleep[1]);
                else
                    syncRND = RND.Next(Query.DaySleep[0], Query.DaySleep[1]);
                Log.Show("TimeFarm", Query.Name, $"sync sleep '{Conversion.Int(syncRND / 3600d)}h {Conversion.Int(syncRND % 3600 / 60d)}m {syncRND % 60}s'", ConsoleColor.Yellow);
                Thread.Sleep(syncRND * 1000);
            }
        }
    }
}