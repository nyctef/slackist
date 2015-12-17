using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Slackist.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class SlackStarredMessagesWatcher
{
    private readonly string m_ApiToken;
    private readonly Action<string> m_OnStarredMessage;
    public SlackStarredMessagesWatcher(string slackApiToken, Action<string> onStarredMessage)
    {
        m_ApiToken = slackApiToken;
        m_OnStarredMessage = onStarredMessage;
    }

    public async Task Watch(CancellationToken cancel)
    {
        using (var webClient = new HttpClient())
        {
            var url = string.Format("https://slack.com/api/rtm.start?token={0}&simple_latest=true&no_unreads=true", m_ApiToken);
            var json = await webClient.GetStringAsync(url);
            dynamic rtmStart = JObject.Parse(json);
            bool ok = rtmStart.ok;
            if (!ok) throw new Exception("Error connecting to slack rtm.start: "+json);
            string wsUrl = rtmStart.url;
            var websocket = await StringClientWebSocket.Connect(new Uri(wsUrl), cancel);
            while (true)
            {
                var messageJson = await websocket.ReceiveString(cancel);
                Console.WriteLine(messageJson);
                dynamic message = JObject.Parse(messageJson);
                Console.WriteLine(message.type);
                if (message.type == "star_added" && message.item.message != null) {
                    m_OnStarredMessage((string)message.item.message.text);
                }
            }
        }
    }
}

