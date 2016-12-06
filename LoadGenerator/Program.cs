using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoadGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Waiting for API to Startup");
            System.Threading.Thread.Sleep(3000);
            bool quit;
            do
            {
                Console.Write("Run Asyncronously (Y/N)?");
                var runAsyncronously = Console.ReadLine().ToLower() == "y";
                var tasks = Enumerable.Range(0, 200).Select(num => CallApi(num, runAsyncronously)).ToArray();
                Task.WaitAll(tasks);
                Console.Write("Finished. Quit (Y/N)?");
                quit = Console.ReadLine().ToLower() == "y";
            } while (!quit);
        }

        public static async Task CallApi(int number, bool runAsyncronously)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            string result;
            try
            {
                //var url = runAsyncronously ? "http://localhost:60383/api/SearchBingAsync" : "http://localhost:60383/api/SearchBingSync";
                var url = runAsyncronously ? "http://localhost:5000/api/SearchBingAsync?random=" : "http://localhost:5000/api/SearchBingSync?random=";

                using (var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(60)
                })
                using (var response = await client.GetAsync(url + Guid.NewGuid()))
                using (var content = response.Content)
                {
                    var downloadedContent = await content.ReadAsStringAsync();
                    result = "Success!";
                }
            }
            catch (Exception)
            {
                result = "Failure!";
            }
            stopWatch.Stop();
            Console.WriteLine($"{number} - {result} - {stopWatch.ElapsedMilliseconds}ms");
        }
    }
}
