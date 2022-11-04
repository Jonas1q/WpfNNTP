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


        void EstablishConnection(string serverName, int serverPort)
        {
            this.serverName = serverName;
            this.serverPort = serverPort;
            socket = new TcpClient(serverName, serverPort);
            ns = socket.GetStream();
        }



            void LoginBtn_Click(object sender, RoutedEventArgs e)
            {
               

                string username = BoxUser.Text;
                string password = PasswordBox.Password;


                try
                {
                    EstablishConnection(this.serverName, this.serverPort);
                    // (1) Create the socket that is connected to server on specified port  
                    reader = new StreamReader(ns, Encoding.UTF8);
                    //Thread.Sleep(100);

                    sw.AutoFlush = true;


                    Thread.Sleep(100);
                    sw.WriteLine($"authinfo user {username}");
                    Thread.Sleep(100);
                    response = sr.ReadLine();
                    Console.WriteLine(response);
                    sw.WriteLine($"authinfo user {username}");
                    response = sr.ReadLine();
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
                sw.AutoFlush = true;
                sw.WriteLine("list");
                while (sr.ReadLine() != ".")
                {
                response = sr.ReadLine();
                Console.WriteLine(response);
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


    

