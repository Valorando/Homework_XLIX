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
using System.Runtime.InteropServices.ComTypes;
using System.Net.Http;

namespace Homework_18_12_II
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private DateTime lastMessageTime;

        public Form1()
        {
            InitializeComponent();
            label2.Text = "1 - Процессоры\n2 - Материнские платы\n3 - Видеокарты\n4 - SSD/HDD накопители\n5 - ОЗУ\n6 - Блоки питания\n7 - Системы охлаждения\n8 - Корпуса";
            textBox1.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = false;
            lastMessageTime = DateTime.MinValue;

            timer1.Interval = 600000;
            timer1.Tick += timer1_Tick;
        }
    
        public async Task Main()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 8888;

            client = new TcpClient();

            try
            {
                await client.ConnectAsync(ip, port);

                string nickname = textBox2.Text;
                byte[] nicknameData = Encoding.UTF8.GetBytes(nickname);
                await client.GetStream().WriteAsync(nicknameData, 0, nicknameData.Length);

                listBox1.Items.Add("Подключено к серверу");
                textBox1.Enabled = true;
                button1.Enabled = true;
                button3.Enabled = true;
                button2.Enabled = false;
                textBox2.Enabled = false;

                timer1.Start();
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }
        }

        public async Task HandleButtonClick()
        {
            try
            {
                if ((DateTime.Now - lastMessageTime).TotalSeconds > 3)
                {
                    string command = textBox1.Text;

                    byte[] data = Encoding.UTF8.GetBytes(command);
                    await client.GetStream().WriteAsync(data, 0, data.Length);
                    listBox1.Items.Add("Сообщение отправлено на сервер");

                    timer1.Stop();
                    timer1.Start();

                    data = new byte[256];
                    int bytesRead = await client.GetStream().ReadAsync(data, 0, data.Length);
                    string response = Encoding.UTF8.GetString(data, 0, bytesRead);
                    listBox1.Items.Add("Ответ сервера:");
                    listBox1.Items.Add($"{response}");

                    lastMessageTime = DateTime.Now;
                }
                else
                {
                    listBox1.Items.Add("Подождите 3 секунды прежде чем отправлять следующий запрос");
                }
            }
            catch(Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await HandleButtonClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            client.Close();
            listBox1.Items.Add("Отключено от сервера");

            textBox1.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = false;
            button2.Enabled = true;
            textBox2.Enabled = true;

            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1.Enabled) 
            {
                client.Close();
                listBox1.Items.Add("Вы были отключены из-за бездействия");
                textBox1.Enabled = false;
                button1.Enabled = false;
                button3.Enabled = false;
                button2.Enabled = true;
                textBox2.Enabled = true;

                timer1.Stop();
            }
        }
    }
}
