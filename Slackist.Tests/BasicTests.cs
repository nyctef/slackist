using Xunit;
using System;
using System.Text;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Slackist.Utils;
using Newtonsoft.Json.Linq;

public class Tests
{
	[Fact]
	public async Task TryUsingSockets()
	{
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        var client = await StringClientWebSocket.Connect(new Uri("ws://echo.websocket.org"), token);
        await client.SendString("this is a test", token);
        var result = await client.ReceiveString(token);
        result.Should().Be("this is a test", "this is an echo server");
	}

    [Fact]
    public async Task CanConnectToSlack()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        // TODO LATER: some config system
        var slackApiToken = Environment.GetEnvironmentVariable("SLACK_TOKEN");
        slackApiToken.Should().NotBeNull("we need an API token to connect to slack");
        
        using (var webClient = new HttpClient())
        {
            var url = string.Format("https://slack.com/api/rtm.start?token={0}&simple_latest=true&no_unreads=true", slackApiToken);
            var json = await webClient.GetStringAsync(url);
            dynamic rtmStart = JObject.Parse(json);
            bool ok = rtmStart.ok;
            ok.Should().BeTrue("slack should give us a successful response");
            string wsUrl = rtmStart.url;
            var websocket = await StringClientWebSocket.Connect(new Uri(wsUrl), token);
            while (true)
            {
                var nextMessageTask = websocket.ReceiveString(token);
                if (nextMessageTask.Wait(TimeSpan.FromSeconds(10))) {
                    var messageJson = nextMessageTask.Result;
                    Console.WriteLine(messageJson);
                    dynamic message = JObject.Parse(messageJson);
                    Console.WriteLine(message.type);
                    if (message.type == "star_added" && message.item.message != null) {
                        Console.WriteLine("message starred: " + message.item.message.text);
                    }

                    continue;
                } else {
                    break;
                }
            }
        }
    }

}

