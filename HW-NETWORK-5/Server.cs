using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HW_NETWORK_5
{
    class Server
    {
        private TcpListener listener;
        private bool isRunning;
        private readonly string api = "93892e75c2862dba407daefd51461555";
        private string baseUrl = "https://api.openweathermap.org/data/2.5/weather?q=";
        public Server(string ip, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
        }
        public async Task StartAsync()
        {
            try
            {
                isRunning = true;
                listener.Start();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Server started . . .");
                Console.ResetColor();

                while(isRunning)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    HandleClient(client);
                }
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }

        private async void HandleClient(TcpClient client)
        {
            await using NetworkStream stream = client.GetStream();
            try
            {
                while(true)
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int size = await stream.ReadAsync(buffer, 0, buffer.Length);
                        string? inquiry = Encoding.UTF8.GetString(buffer, 0, size);

                        Console.WriteLine($"Received: {inquiry}");
                        if (inquiry.Trim().ToLower() == "exit")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Exiting");
                            break;
                        }
                        string httpRequest = baseUrl + $"{inquiry}&appid={api}&units=metric";

                        using HttpClient httpClient = new HttpClient();
                        string data = await httpClient.GetStringAsync(httpRequest);
                        JsonNode? node = JsonNode.Parse(data);

                        inquiry = Capitalize(inquiry);
                        string response = $"{inquiry}: {node?["main"]?["temp"]?.ToString()} °C";
                        buffer = Encoding.UTF8.GetBytes(response);
                        await stream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    catch(Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(e.Message);
                        Console.ResetColor();

                        string response = e.Message;
                        byte[] buffer = Encoding.UTF8.GetBytes(response);
                        await stream.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
            }
            finally
            {
                client.Close();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Client disconnected");
                Console.ResetColor();
            }
        }
        public void Stop()
        {
            isRunning = false;
            listener.Stop();
            listener.Dispose();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Server stopped . . .");
            Console.ResetColor();
        }
        private string? Capitalize(string? str)
        {
            if (str == null) ArgumentNullException.ThrowIfNull(str);
            if (str[0] >= 65 && str[0] <= 90) return str;
            char[] chars = str.ToCharArray();
            chars[0] = (char)(chars[0] - 32);
            return new string(chars);
        }
    }
}
