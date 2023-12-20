using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using System.Net.Http;

namespace Homework_18_12
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private Dictionary<TcpClient, string> nicknames = new Dictionary<TcpClient, string>();


        public Form1()
        {
            InitializeComponent();
            Main();
        }

        public async Task Main()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 8888;

            try
            {
                server = new TcpListener(ip, port);
                server.Start();
                listBox1.Items.Add($"[{DateTime.Now}]: Сервер запущен");

                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    _ = Task.Run(() => ProcessClient(client));
                }
            }
            catch(Exception ex)
            {
                listBox1.Items.Add($"[{DateTime.Now}]: {ex.Message}");
            }
        }

        public async Task ProcessClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                byte[] nicknameData = new byte[256];
                int nicknameBytesRead = await stream.ReadAsync(nicknameData, 0, nicknameData.Length);
                string nickname = Encoding.UTF8.GetString(nicknameData, 0, nicknameBytesRead);

                nicknames.Add(client, nickname);

                listBox1.Items.Add($"[{DateTime.Now}]: {nickname} подключился к серверу");

                while (client.Connected)
                    {
                    byte[] data = new byte[256];
                    int bytesRead = await stream.ReadAsync(data, 0, data.Length);

                    if (bytesRead == 0)
                    {
                        listBox1.Items.Add($"[{DateTime.Now}]: {nickname} отключился от сервера");
                        nicknames.Remove(client);
                        break;
                    }

                    string message = Encoding.UTF8.GetString(data, 0, bytesRead);
                    listBox1.Items.Add($"[{DateTime.Now}]: {nickname} отправил запрос на категорию: {message}");

                    if (message == "1")
                    {
                        
                        byte[] response = Encoding.UTF8.GetBytes("Intel Core I9 - 23281 грн");
                        await stream.WriteAsync(response, 0, response.Length);
                    }
                    else if (message == "2")
                    {
                        byte[] response = Encoding.UTF8.GetBytes("ASRock B760M Pro - 6502 грн");
                        await stream.WriteAsync(response, 0, response.Length);
                    }
                    else if (message == "3")
                    {
                        byte[] response = Encoding.UTF8.GetBytes("Zotac RTX 3070 - 25776 грн");
                        await stream.WriteAsync(response, 0, response.Length);
                    }
                    else if (message == "4")
                    {
                        byte[] response = Encoding.UTF8.GetBytes("SSD Kingston a400 480GB - 1349 грн");
                        await stream.WriteAsync(response, 0, response.Length);
                    }
                    else if (message == "5")
                    {
                        byte[] response = Encoding.UTF8.GetBytes("Kingston FURY 5600 DDR5 16GB - 3275 грн");
                        await stream.WriteAsync(response, 0, response.Length);
                    }
                    else if (message == "6")
                    {
                        byte[] response = Encoding.UTF8.GetBytes("GIGABYTE 650W - 2399 грн");
                        await stream.WriteAsync(response, 0, response.Length);
                    }
                    else if (message == "7")
                    {
                        byte[] response = Encoding.UTF8.GetBytes("Куллерное Intel Socket 1700 Bulk - 140 грн");
                        await stream.WriteAsync(response, 0, response.Length);
                    }
                    else if (message == "8")
                    {
                        byte[] response = Encoding.UTF8.GetBytes("Chieftec Gaming Hunter - 3148 грн");
                        await stream.WriteAsync(response, 0, response.Length);
                    }
                    else
                    {
                        byte[] response = Encoding.UTF8.GetBytes("Сообщение не распознано");
                        await stream.WriteAsync(response, 0, response.Length);
                    }

                    listBox1.Items.Add($"[{DateTime.Now}]: Сервер отправил ответ {nickname}");
                }
            }
            catch(Exception ex)
            {
                listBox1.Items.Add($"[{DateTime.Now}]: {ex.Message}");
            }
        }
    }
}
