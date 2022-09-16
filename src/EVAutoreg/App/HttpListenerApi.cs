using System.Net;
using System.Text;
using static EVAutoreg.Auxiliary.PrettyPrinter;

namespace EVAutoreg.App
{
    public class HttpListenerApi
    {
        private readonly Exchange _exchange;

        public HttpListenerApi(Exchange exchange)
        {
            _exchange = exchange;
        }

        public async Task SimpleListener(string[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentNullException(nameof(prefixes));

            var listener = new HttpListener();

            foreach (var prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }

            listener.Start();

            Console.WriteLine("Listening...");



            while (true)
            {
                var context = await listener.GetContextAsync();
                var request = context.Request;


                if (request.HttpMethod == HttpMethod.Post.Method && request.Url?.AbsolutePath == "/enable/")
                {
                    var response = context.Response;

                    const string responseText = "SERVICE ENABLED";
                    var buffer = Encoding.UTF8.GetBytes(responseText);

                    response.ContentLength64 = buffer.Length;

                    var output = response.OutputStream;
                    await output.WriteAsync(buffer, 0, buffer.Length);

                    output.Close();
                    PrintNotification("Received enable request, starting service...", ConsoleColor.Blue);
                    await _exchange.StartService();

                }
                else if (request.HttpMethod == HttpMethod.Post.Method && request.Url?.AbsolutePath == "/disable/")
                {
                    var response = context.Response;

                    const string responseText = "SERVICE DISABLED";
                    var buffer = Encoding.UTF8.GetBytes(responseText);

                    response.ContentLength64 = buffer.Length;

                    var output = response.OutputStream;
                    await output.WriteAsync(buffer, 0, buffer.Length);

                    output.Close();
                    PrintNotification("Received disable request, stopping service...", ConsoleColor.Blue);
                    _exchange.IsServiceEnabled = false;
                    _exchange.StopService();
                }
            }
        }
    }
}
