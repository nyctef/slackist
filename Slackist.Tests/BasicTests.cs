using Xunit;
using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

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

/// <summary>code based on https://msdn.microsoft.com/en-us/magazine/jj863133.aspx</summary>
public class StringClientWebSocket : IDisposable
{
    private readonly ClientWebSocket m_WebSocket;
    private readonly UTF8Encoding m_Encoding;

    private StringClientWebSocket(ClientWebSocket webSocket)
    {
        m_WebSocket = webSocket;
        m_Encoding = new UTF8Encoding(false);
    }

    public static async Task<StringClientWebSocket> Connect(Uri wsUrl, CancellationToken cancellationToken)
    {
        var webSocket = new ClientWebSocket();
        await webSocket.ConnectAsync(wsUrl, cancellationToken);
        return new StringClientWebSocket(webSocket);
    }

    public void Dispose()
    {
        if (m_WebSocket != null) m_WebSocket.Dispose();
    }

    public async Task SendString(string str, CancellationToken cancellationToken)
    {
        var bytes = m_Encoding.GetBytes(str);
        await m_WebSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
    }

    public async Task<string> ReceiveString(CancellationToken cancellationToken)
    {
        var buffer = new byte[50*1024];
        while (true)
        {
            var segment = new ArraySegment<byte>(buffer);
            var result =
                await m_WebSocket.ReceiveAsync(segment, cancellationToken);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await m_WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK",
                    cancellationToken);
                return null;
            }
            if (result.MessageType == WebSocketMessageType.Binary)
            {
                await m_WebSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType,
                    "I don't do binary", cancellationToken);
                return null;
            }
            int count = result.Count;
            while (!result.EndOfMessage)
            {
                if (count >= buffer.Length)
                {
                    await m_WebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                        "That's too long", cancellationToken);
                    return null;
                }
                segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                result = await m_WebSocket.ReceiveAsync(segment, cancellationToken);
                count += result.Count;
            }
            var message = new UTF8Encoding(false).GetString(buffer, 0, count);
            return message;
        }
    }
}
