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

public class TodoistApi
{
    private readonly string m_ApiToken;
    public TodoistApi(string todoistApiToken)
    {
        m_ApiToken = todoistApiToken;
    }

    public async Task CreateTask(string content, string time, CancellationToken cancel)
    {
        using (var webClient = new HttpClient())
        {
            var commands = JsonConvert.SerializeObject(new [] {
                    new { type = "item_add", 
                          uuid = Guid.NewGuid(), 
                          temp_id= Guid.NewGuid(),
                          args = new {
                              content = content,
                              date_string = time
                          }
                        }
            });
            var url = "https://todoist.com/API/v6/sync";

            var response = await webClient.PostAsync(url, new FormUrlEncodedContent(new Dictionary<string, string>() {
                    { "token", m_ApiToken },
                    { "commands", commands },
            }), cancel);
        }
    }

}
