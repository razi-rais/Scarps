using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace CallApiAsyncDemo
{
    class Program
    {
        static int counter = 1;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Instantiate the CancellationTokenSource.
            var cts = new CancellationTokenSource();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            var requestCount = 50000;
            await AccessTheWebAsync(cts.Token, requestCount);
            
            watch.Stop();
            Console.WriteLine("----------------");
            Console.WriteLine(string.Format("Total Requests: {0}", requestCount));
            Console.WriteLine(string.Format("Duration in seconds: ~{0}", watch.Elapsed.TotalSeconds.ToString()));
            Console.WriteLine(string.Format("Duration in minutes: ~{0}", watch.Elapsed.Minutes.ToString()));
            Console.WriteLine(string.Format("Duration in hours: ~{0}", watch.Elapsed.Hours.ToString()));
            Console.WriteLine("----------------");
            
            //await SendRequests();
        }

        public static async Task<object>  SendRequests()
        {
          var url = "https://ultimatetest1.b2clogin.com/ultimatetest1.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_signup_signin_loyality_number&client_id=18ac2afe-2c1f-4ea8-8d63-14dd50ee4f85&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login";
          HttpClient client = new HttpClient();
          
          Stopwatch watch = new Stopwatch();
          watch.Start();

            for (int i = 0; i < 10; i++)
              {
                var response = await client.GetAsync(url);
                Console.WriteLine(string.Format("Counter {0}: {1}", i, response.StatusCode) );
              }
          

           watch.Stop();
           Console.WriteLine("----------------");
           Console.WriteLine(string.Format("Duration in seconds: {0}", watch.Elapsed.TotalSeconds.ToString()));
           Console.WriteLine(string.Format("Duration in minutes: {0}", watch.Elapsed.Minutes.ToString()));
           Console.WriteLine(string.Format("Duration in hours: {0}", watch.Elapsed.Hours.ToString()));
           Console.WriteLine("----------------");

            return Task.FromResult<object>(null);
        }


        static async Task AccessTheWebAsync(CancellationToken ct, int requestCount)
        {
            HttpClient client = new HttpClient();
            var httpPostrequest = PrepareROPCRequest(client);

            // Make a list of web addresses.
            List<string> urlList = SetUpURLList(requestCount);

            // ***Create a query that, when executed, returns a collection of tasks.
            IEnumerable<Task<HttpResponseMessage>> downloadTasksQuery =
                from url in urlList select ProcessURL(url, client, httpPostrequest, ct);

            // ***Use ToList to execute the query and start the tasks.
            List<Task<HttpResponseMessage>> downloadTasks = downloadTasksQuery.ToList();

            // ***Add a loop to process the tasks one at a time until none remain.
            while (downloadTasks.Count > 0)
            {
                // Identify the first task that completes.
                Task<HttpResponseMessage> firstFinishedTask = await Task.WhenAny(downloadTasks);

                // ***Remove the selected task from the list so that you don't
                // process it more than once.
                downloadTasks.Remove(firstFinishedTask);

                // Await the completed task.
                HttpResponseMessage response = await firstFinishedTask;
                //int length = await firstFinishedTask;
                //resultsTextBox.Text += $"\r\nLength of the download:  {length}";
                //Console.WriteLine(string.Format("\r\nLength of the download: {0}", length));
                Console.WriteLine(string.Format("\r\nStatus Code of the download: {0}", response.StatusCode));
                Console.WriteLine(string.Format("\r\nResponse: {0}", response.Content.ReadAsStringAsync().Result));

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Console.WriteLine(string.Format("\r\nRetry-After: {0} seconds", response.Headers.GetValues("Retry-After").First()));

                }

            }
        }

        private static List<string> SetUpURLList(int requestCount)
        {
            //List<string> urls = new List<string>
            //{
            //    "https://msdn.microsoft.com",
            //    "https://msdn.microsoft.com/library/windows/apps/br211380.aspx",
            //    "https://msdn.microsoft.com/library/hh290136.aspx",
            //    "https://msdn.microsoft.com/library/dd470362.aspx",
            //    "https://msdn.microsoft.com/library/aa578028.aspx",
            //    "https://msdn.microsoft.com/library/ms404677.aspx",
            //    "https://msdn.microsoft.com/library/ff730837.aspx"
            //};

            List<string> urls = new List<string>();

            for(int i=0; i< requestCount; i++)
            {
                urls.Add("https://ultimatetest1.b2clogin.com/ultimatetest1.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_signup_signin_loyality_number&client_id=18ac2afe-2c1f-4ea8-8d63-14dd50ee4f85&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid&response_type=id_token&prompt=login");
            }

            return urls;
        }

        private static HttpRequestMessage PrepareROPCRequest(HttpClient client)
        {
            //var client = new HttpClient();
            client.BaseAddress = new Uri("https://ultimatetest1.b2clogin.com");
            var request = new HttpRequestMessage(HttpMethod.Post, "/ultimatetest1.onmicrosoft.com/B2C_1_ROPC/oauth2/v2.0/token");

            //var byteArray = new UTF8Encoding().GetBytes("<clientid>:<clientsecret>");
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("client_id", "ea0c1f1d-a4f1-4204-9a99-3ea98f52ef46"));
            formData.Add(new KeyValuePair<string, string>("response_type", "token id_token"));
            formData.Add(new KeyValuePair<string, string>("grant_type", "password"));
            formData.Add(new KeyValuePair<string, string>("username", "test.user9"));
            formData.Add(new KeyValuePair<string, string>("password", ""));
            formData.Add(new KeyValuePair<string, string>("scope", "openid ea0c1f1d-a4f1-4204-9a99-3ea98f52ef46 offline_access"));

            request.Content = new FormUrlEncodedContent(formData);
            return request;

        }
        static async Task<HttpResponseMessage> ProcessURL(string url, HttpClient client, HttpRequestMessage httpRequesMessaget,  CancellationToken ct)
        {
            try
            {
                // GetAsync returns a Task<HttpResponseMessage>.
                //HttpResponseMessage response = await client.GetAsync(url, ct);
                //var request = PrepareROPCRequest(client);

                client = new HttpClient();
                var request = PrepareROPCRequest(client);
                HttpResponseMessage response = await client.SendAsync(request);
                //HttpResponseMessage response = await client.GetAsync(url, ct);

                Console.WriteLine("Request Id " + counter++);
                return response;
            }
            catch (System.Exception ex) { Console.WriteLine(ex); }
            return new HttpResponseMessage(); //TODO: Add graceful return logic.

           

            // Retrieve the website contents from the HttpResponseMessage.
            //byte[] urlContents = await response.Content.ReadAsByteArrayAsync();

            //return urlContents.Length;
        }


    }

}
