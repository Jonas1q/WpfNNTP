using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpfnntp
{
    
    public partial class MainWindow : Window
    {
        String serverName = "news.sunsite.dk";
        int serverPort = 119;
        private String response;

        public StreamWriter sw;
        public StreamReader sr;
        public StreamReader reader;

        private TcpClient socket;
        private NetworkStream ns;
        public MainWindow()
            {

                InitializeComponent();

                socket = new TcpClient(serverName, serverPort);
                ns = socket.GetStream();
                sr = new StreamReader(ns, Encoding.Default);
                sw = new StreamWriter(ns);
                reader = new StreamReader(ns, Encoding.UTF8);
            }



            void LoginBtn_Click(object sender, RoutedEventArgs e)
            {
                String serverName = "news.sunsite.dk";
                int serverPort = 119;

                string username = BoxUser.Text;
                string password = PasswordBox.Password;

                // Convert an input String to bytes  

                byte[] sendMessage = Encoding.UTF8.GetBytes("Hello Server, how are you today?\n");


                TcpClient socket = null;
                NetworkStream ns = null;
                StreamReader reader = null;

                try
                {
                    // (1) Create the socket that is connected to server on specified port  
                    socket = new TcpClient(serverName, serverPort);
                    Thread.Sleep(100);
                    ns = socket.GetStream();
                    Console.WriteLine("Connected to server, the stream is ready");




                    // a) Write to the server 
                    ns.Write(sendMessage, 0, sendMessage.Length);
                    Thread.Sleep(100);
                    Console.WriteLine("Sent {0} bytes to server...", sendMessage.Length);

                    ns.Flush();

                    // b) Read from the server

                    reader = new StreamReader(ns, Encoding.UTF8);
                    String? recieveMessage = reader.ReadLine();
                    Console.WriteLine("Got this message {0} back from the server", recieveMessage);
                    Thread.Sleep(100);


                    StreamReader sr = new StreamReader(ns, Encoding.Default);
                    StreamWriter sw = new StreamWriter(ns);
                    sw.AutoFlush = true;


                    Thread.Sleep(100);
                    sw.WriteLine($"authinfo user {username}");
                    Thread.Sleep(100);
                    string response = sr.ReadLine();
                    Console.WriteLine(response);

                    if (response == "500 What?")
                    {
                        sw.WriteLine($"authinfo user {username}");
                        response = sr.ReadLine();
                    }

                    if (response == "381 PASS required")
                    {
                        sw.WriteLine($"authinfo pass {password}");
                        response = sr.ReadLine();
                        Console.WriteLine(response);
                        sw.WriteLine($"authinfo pass {password}");
                        response = sr.ReadLine(); // Retrieve Response


                    }

                    if (response == "281 Ok")
                    {
                        MessageBox.Show("Connected with no problems");
                        Listbtn.IsEnabled = true;
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }

                
            }

            void Listbtn_Click(object sender, RoutedEventArgs e)
            {


                sw.WriteLine("list");
                {
                    response = sr.ReadLine();
                    OutputBox.Text += response;
                }



            }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            sw.WriteLine("quit");
            ns.Close();
            reader.Close();
            socket.Close();
        }
    }
    }


    

