using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Toon.Serde;
using Newtonsoft.Json.Linq;
class Program
{
    static async Task Main()
    {
        // Create HttpClient with ToonHttpClientHandler
        var client = new HttpClient(new ToonHttpClientHandler(new ToonOptions { Indent = 2 }));
        client.BaseAddress = new Uri("https://httpbin.org"); // simple public echo endpoint

        var payload = new JObject
        {
            ["user"] = new JObject { ["id"] = 123, ["name"] = "Akira" },
            ["tags"] = new JArray("agent", "llm")
        };

        var jsonContent = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");

        // Put a breakpoint on the next line (inside SendAsync of ToonHttpClientHandler)
        var resp = await client.PostAsync("/post", jsonContent);
        var text = await resp.Content.ReadAsStringAsync();
        Console.WriteLine("Response status: " + resp.StatusCode);
        Console.WriteLine("Response body:\n" + text);
    }
}
