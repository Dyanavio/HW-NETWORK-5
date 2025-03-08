using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.WebRequestMethods;

namespace HW_NETWORK_5
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 5050;

            Server server = new Server(ip, port);
            await server.StartAsync();
        }
    }
}
