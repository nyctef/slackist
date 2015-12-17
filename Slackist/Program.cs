using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Slackist
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Usage: slackist [slack_api_token] [todoist_api_token]");
                Environment.Exit(1);
            }

            var slackApiToken = args[0];
            var todoistApiToken = args[1];

            var cts = new CancellationTokenSource();
            var cancel = cts.Token;

            var todoistApi = new TodoistApi(todoistApiToken);
            var watcher = new SlackStarredMessagesWatcher(slackApiToken, async message => {
                Console.WriteLine("Message starred: " + message);
                await todoistApi.CreateTask(message, "today", cancel);
            });

            await watcher.Watch(cancel);
        }
    }
}
