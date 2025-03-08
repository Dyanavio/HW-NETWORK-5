using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 5050;

            Client client = new Client(ip, port);
            await client.StartAsync();

            Console.ReadKey();
        }
    }
}
