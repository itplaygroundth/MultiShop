using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SmartLib.Helpers
{
    public partial class Networking
    {
         [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, 
                                         byte[] pMacAddr, ref uint PhyAddrLen);
        public static string getIpAddress()
        {
            string ip_addr = "";

            String strHostName = Dns.GetHostName();

            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
             ip_addr= addr[2].ToString();

            return ip_addr;

        }

      

        public static void TCPStart(string ipaddr)
        {
            Int32 port = 9007;
            IPAddress localAddr = IPAddress.Parse(ipaddr);
            TcpListener server = null;
            server = new TcpListener(localAddr, port);
            server.Start();
            DoBeginAcceptSocket(server);

        }

        public static string GetLocalIP()
        {
            string _IP = null;

            // Resolves a host name or IP address to an IPHostEntry instance.
            // IPHostEntry - Provides a container class for Internet host address information. 
            System.Net.IPHostEntry _IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            // IPAddress class contains the address of a computer on an IP network. 
            foreach (System.Net.IPAddress _IPAddress in _IPHostEntry.AddressList)
            {
                // InterNetwork indicates that an IP version 4 address is expected 
                // when a Socket connects to an endpoint
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    _IP = _IPAddress.ToString();
                    return _IP;
                }
            }
            return _IP;
        }

        public static bool checkConnection(string ipaddress)
        {
            bool result = false;
            try
            {
                int timout = 800;
                int port = 1433;
                if (ipaddress.Length > 0)
                {
                    IPAddress localAddr = IPAddress.Parse(ipaddress);
                    IPEndPoint remoteEndPoint = new IPEndPoint(localAddr, port);


                    TcpClient NetworkClient = SmartPOS.Helpers.TimeOutSocket.Connect(remoteEndPoint, timout);
                    result = true;
                }
                   
                //NetworkStream networkstream = NetworkClient.GetStream();
                //StreamReader streamReader = new StreamReader(networkstream);
                //string line = streamReader.ReadLine();
                //if (!string.IsNullOrEmpty(line))
                //    txtreturneddata.Text = line;
            }
            catch (Exception ex)
            {
                //result = false;// MessageBox.Show(ex.Message);
            }
            return result;
        }


        //public static bool checkConnection(string ip)
        //{
        //    //BS.Utilities.Ping netMon = new BS.Utilities.Ping();
        //    bool result = false;
        //    try
        //    {
        //    }
        //    catch(PingException ex)
        //    {
        //        result = false;
        //    }
        //    return result;
        
        //}

        public  static bool checkConnection(string ipaddress,Byte[] macAddr,uint macAddrLen)
        {
      
            bool result=false;
     
            macAddr = new byte[6];

            //Here you can put the IP that should be checked
            IPAddress Destination = IPAddress.Parse(ipaddress);

            //Send Request and check if the host is there
            if (SendARP((int)Destination.Address, 0, macAddr, ref macAddrLen) == 0)
            {
                //SUCCESS! Igor it's alive!
                result = true;
        
            }
        
        return result;
        }

        public static List<string> GetLocalIPList()
        {
            List<string> iplist = new List<string>();
            //string _IP = null;

            // Resolves a host name or IP address to an IPHostEntry instance.
            // IPHostEntry - Provides a container class for Internet host address information. 
            System.Net.IPHostEntry _IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
           
            // IPAddress class contains the address of a computer on an IP network. 
            foreach (System.Net.IPAddress _IPAddress in _IPHostEntry.AddressList)
            {
                // InterNetwork indicates that an IP version 4 address is expected 
                // when a Socket connects to an endpoint
                //if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                //{
                // _IP = _IPAddress.ToString();
                // return _IP;
                //}
                iplist.Add( _IPAddress.ToString());
            }
            return iplist;
        }

        public static List<string> GetIPAllInterface()
        {
            List<string> iplist = new List<string>();
            //IPInterfaceProperties properties=null;
            foreach (NetworkInterface netif in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netif.NetworkInterfaceType != NetworkInterfaceType.Loopback && netif.NetworkInterfaceType != NetworkInterfaceType.Tunnel && netif.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties properties = netif.GetIPProperties();

                    foreach (IPAddressInformation unicast in properties.UnicastAddresses)
                    {

                        iplist.Add(unicast.Address.ToString());

                    }
                }
            }
            return iplist;
        }

        private static bool IsNetworkAvailable()
        {
            // only recognizes changes related to Internet adapters
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // however, this will include all adapters
                NetworkInterface[] interfaces =
                    NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface face in interfaces)
                {
                    // filter so we see only Internet adapters
                    if (face.OperationalStatus == OperationalStatus.Up)
                    {
                        if ((face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
                            (face.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                        {
                            IPv4InterfaceStatistics statistics =
                                face.GetIPv4Statistics();

                            // all testing seems to prove that once an interface
                            // comes online it has already accrued statistics for
                            // both received and sent...

                            if ((statistics.BytesReceived > 0) &&
                                (statistics.BytesSent > 0))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static string GetIPNetworkAvailable()
        {
            // only recognizes changes related to Internet adapters

            string ipaddress = "";

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // however, this will include all adapters
                NetworkInterface[] interfaces =
                    NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface face in interfaces)
                {
                    // filter so we see only Internet adapters
                    if (face.OperationalStatus == OperationalStatus.Up)
                    {
                        if ((face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
                            (face.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                        {
                            IPInterfaceProperties fp = face.GetIPProperties();
                            foreach(IPAddressInformation unicast in fp.UnicastAddresses)
                            {
                                if (IsValidIP(unicast.Address.ToString()))
                                    ipaddress=unicast.Address.ToString();
                                      
                            }

                            //IPv4InterfaceStatistics statistics =
                            //    face.GetIPv4Statistics();

                            // all testing seems to prove that once an interface
                            // comes online it has already accrued statistics for
                            // both received and sent...

                            //if ((statistics.BytesReceived > 0) &&
                            //    (statistics.BytesSent > 0))
                            //{
                            //    return true;
                            //}
                            //return fp.UnicastAddresses.
                        }
                    }
                }
            }

            return ipaddress;
        }

        public static bool IsValidIP(string addr)
        {
            //create our match pattern
            string pattern = @"^([1]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            //Regex(@"[\b\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}\b")
            //create our Regular Expression object
            pattern = @"([01]?\d\d?|2[0-4]\d|25[0-5])\." +
                         @"([01]?\d\d?|2[0-4]\d|25[0-5])\." +
                         @"([01]?\d\d?|2[0-4]\d|25[0-5])\." +
                         @"([01]?\d\d?|2[0-4]\d|25[0-5])";
            Regex check = new Regex(pattern);
            //boolean variable to hold the status
            bool valid = false;
            //check to make sure an ip address was provided
            if (addr == "")
            {
                //no address provided so return false
                valid = false;
            }
            else
            {
                //address provided so use the IsMatch Method
                //of the Regular Expression object
                valid = check.IsMatch(addr, 0);
            }
            //return the results
            return valid;
        }
        // Thread signal.
        public static ManualResetEvent clientConnected = new ManualResetEvent(false);

        // Accept one client connection asynchronously.
        public static void DoBeginAcceptSocket(TcpListener listener)
        {
            // Set the event to nonsignaled state.
            clientConnected.Reset();

            // Start to listen for connections from a client.
            //Console.WriteLine("Waiting for a connection...");

            // Accept the connection.
            // BeginAcceptSocket() creates the accepted socket.
            listener.BeginAcceptSocket(new AsyncCallback(DoAcceptSocketCallback), listener);
            // Wait until a connection is made and processed before
            // continuing.
            clientConnected.WaitOne();
        }

        // Process the client connection.
        public static void DoAcceptSocketCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;

            // End the operation and display the received data on the
            //console.
            Socket clientSocket = listener.EndAcceptSocket(ar);

            // Process the connection here. (Add the client to a
            // server table, read data, etc.)
            //Console.WriteLine("Client connected completed");

            // Signal the calling thread to continue.
            clientConnected.Set();
        }
    }

}
