using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navico
{
    class NavicoDiscovery
    {
        public class MFDService
        {
            public MFDService(string service, uint version, uint port)
            {
                Service = service;
                Version = version;
                Port = port;
            }

            public string Service;
            public uint Version;
            public uint Port;
        }

        public class MFD
        {
            public string Name;
            public string IP;
            public string Model;
            public MFDService[] Services;
        }

        static readonly object _locker = new object();

        public static void ReceiveOldMessage(List<MFD> MFDList)
        {
#if DEBUG_MGS
            string output = string.Format("Enter ReceiveOldMessage");
            lock (_locker)
            {
                Console.WriteLine(output);
                Debug.Print(output + string.Format("\r\n"));
            }
#endif

            // Navico NSS 2.0: Multicast 239.2.1.1, port 2050
            UdpClient client = new UdpClient();
            client.Client.ReceiveTimeout = 2000;

            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 2050);
            client.Client.Bind(localEp);

            IPAddress multicastaddress = IPAddress.Parse("239.2.1.1");
            client.JoinMulticastGroup(multicastaddress);

            DateTime timeLastNewFound = DateTime.Now;
            while (timeLastNewFound.AddSeconds(2) > DateTime.Now)
            {
                Byte[] data = null;
                try
                {
                    data = client.Receive(ref localEp);
                }
                catch (Exception ex)
                {
                    break;
                }
                if (data.Length > 0)
                {
                    string strData = Encoding.ASCII.GetString(data);
                    char[] lineEnd = { '\r', '\n' };
                    strData = strData.TrimEnd(lineEnd);
                    string[] strParams = strData.Split(',');
                    if (strParams.Count() >= 4)
                    {
                        bool foundMFD = false;
                        bool foundService = false;
                        foreach (MFD mfd in MFDList)
                        {
                            if (strParams[2] == mfd.IP)
                            {
                                foundMFD = true;
                                foreach (MFDService service in mfd.Services)
                                {
                                    if (strParams[0] == service.Service)
                                    {
                                        foundService = true;
                                    }
                                }
                                break;
                            }
                        }

                        if (foundMFD)
                        {
                            if (!foundService)
                            {
                                timeLastNewFound = DateTime.Now;
                                foreach (MFD mfd in MFDList)
                                {
                                    if (strParams[2] == mfd.IP)
                                    {
                                        lock (_locker)
                                        {
                                            MFDService[] newList = new MFDService[mfd.Services.Count() + 1];
                                            for (int service = 0; service < mfd.Services.Count(); service++)
                                            {
                                                newList[service] = mfd.Services[service];
                                            }
                                            newList[mfd.Services.Count()].Service = strParams[0];
                                            newList[mfd.Services.Count()].Port = Convert.ToUInt32(strParams[3]);
                                            mfd.Services = newList;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            timeLastNewFound = DateTime.Now;
                            MFD mfd = new MFD();
                            mfd.IP = strParams[2];
                            mfd.Services = new MFDService[1];
                            mfd.Services[0] = new MFDService(strParams[0], 0, Convert.ToUInt32(strParams[3]));
#if DEBUG_MGS
                            output = string.Format("Service: {0}\tVersion: {1}\tIP: {2}\tPort: {3}", strParams[0], strParams[1], strParams[2], strParams[3]);
#endif
                            lock (_locker)
                            {
                                MFDList.Add(mfd);

#if DEBUG_MGS
                                Console.WriteLine(output);
                                Debug.Print(output + string.Format("\r\n"));
#endif
                            }
                        }
                    }
                }
            }

#if DEBUG_MGS
            output = string.Format("Exit ReceiveOldMessage");
            lock (_locker)
            {
                Console.WriteLine(output);
                Debug.Print(output + string.Format("\r\n"));
            }
#endif
        }

        public static void ReceiveNewMessage(List<MFD> MFDList)
        {
#if DEBUG_MGS
            string output = string.Format("Enter ReceiveNewMessage");
            lock (_locker)
            {
                Console.WriteLine(output);
                Debug.Print(output + string.Format("\r\n"));
            }
#endif

            // Navico NSS 2.5: Multicast 239.2.1.1, port 2052
            UdpClient client = new UdpClient();
            client.Client.ReceiveTimeout = 2000;

            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 2052);
            client.Client.Bind(localEp);

            IPAddress multicastaddress = IPAddress.Parse("239.2.1.1");
            client.JoinMulticastGroup(multicastaddress);

            DateTime timeLastNewFound = DateTime.Now;
            while (timeLastNewFound.AddSeconds(2) > DateTime.Now)
            {
                Byte[] data = null;
                try
                {
                    data = client.Receive(ref localEp);
                }
                catch (Exception ex)
                {
                    break;
                }
                if (data.Length > 0)
                {
                    string strData = Encoding.ASCII.GetString(data);
                    MFD deserializedMFD = JsonConvert.DeserializeObject<MFD>(strData);

                    bool found = false;
                    foreach (MFD mfd in MFDList)
                    {
                        if (deserializedMFD.IP == mfd.IP)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        timeLastNewFound = DateTime.Now;
#if DEBUG_MGS
                        output = string.Format("MFD: {0}\tModel: {1}\tIP: {2}", deserializedMFD.Name, deserializedMFD.Model, deserializedMFD.IP);
#endif
                        lock (_locker)
                        {
                            MFDList.Add(deserializedMFD);

#if DEBUG_MGS
                            Console.WriteLine(output);
                            Debug.Print(output);
                            for (uint service = 0; service < deserializedMFD.Services.Count(); service++)
                            {
                                output = string.Format("\tService {0}: {1}\tVersion: {2}\tPort: {3}", service, deserializedMFD.Services[service].Service, deserializedMFD.Services[service].Version, deserializedMFD.Services[service].Port);
                                Console.WriteLine(output);
                                Debug.Print(output);
                            }
#endif
                        }
                    }
                }
            }

#if DEBUG_MGS
            output = string.Format("Exit ReceiveNewMessage");
            lock (_locker)
            {
                Console.WriteLine(output);
                Debug.Print(output + string.Format("\r\n"));
            }
#endif
        }
    }
}
