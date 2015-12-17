using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Slackist.Utils;
using Newtonsoft.Json;
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
        
        var watcher = new SlackStarredMessagesWatcher(slackApiToken, str => {
            Console.WriteLine("message starred: " + str);
        });

        var watchTask = watcher.Watch(token);
        watchTask.Wait(TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task CanPostTaskToTodoist()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var todoistApiToken = Environment.GetEnvironmentVariable("TODOIST_TOKEN");
        todoistApiToken.Should().NotBeNull("we need an API token to connect to Todoist");
        var api = new TodoistApi(todoistApiToken);

        await api.CreateTask("This is test item "+Guid.NewGuid(), "today", token);
    }
}

