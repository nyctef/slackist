using Xunit;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

public class Tests
{
	[Fact]
	public async Task TryUsingSockets()
	{
		var client = new ClientWebSocket();
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        await client.ConnectAsync(new Uri("ws://echo.websocket.org"), token);
	}
}
