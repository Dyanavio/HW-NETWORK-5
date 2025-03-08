using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        private TcpClient client;
        private readonly string ip;
        private readonly int port;

        public Client(string ip, int port)
        {
            client = new();
            this.ip = ip;
            this.port = port;
        }
        public async Task StartAsync()
        {
            await client.ConnectAsync(IPAddress.Parse(ip), port);
            await HandleAsync();
        }

        private async Task HandleAsync()
        {
            try
            {
                await using NetworkStream stream = client.GetStream();

                while(true)
                {
                    Console.Write("Enter the city('exit' to exit): ");
                    string? city = Console.ReadLine()?.ToLower().Trim();

                    if (string.IsNullOrEmpty(city))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("City cannot be empty");
                        Console.ResetColor();
                        continue;
                    }
                    byte[] buffer = Encoding.UTF8.GetBytes(city);
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                     
                    if (city == "exit") break;

                    buffer = new byte[1024];
                    int size = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, size);

                    if(response.Contains("404")) Console.ForegroundColor = ConsoleColor.Red;
                    else Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(response);
                    Console.ResetColor();
                }
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }
    }
}
