namespace CsTools.HttpRequest;

public class SseClient
{
    public SseClient(string url)
    {
        this.url = url;
        client.Timeout = TimeSpan.FromSeconds(5);
        Loop();   
    }

    async void Loop()
    {
        while (true)
        {
            try 
            {
                using var sr = new StreamReader(await client.GetStreamAsync(url));
                while (!sr.EndOfStream)
                {
                    var msg = await sr.ReadLineAsync();
                    Console.WriteLine(msg);
                }
            }
            catch (Exception e)
            {
                await Task.Delay(client.Timeout);
            }
        }
    }

    readonly HttpClient client = new();
    readonly string url;
}