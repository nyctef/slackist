using Xunit;
using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Slackist.Utils;

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
}

