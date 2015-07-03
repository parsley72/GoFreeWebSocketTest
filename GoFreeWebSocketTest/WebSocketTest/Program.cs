using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text;
using System.ComponentModel;

using WebSocketSharp;
//using WebSocketSharp.Frame;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Navico;

namespace WebSocketTest
{
    class Program
    {
#if VS_2012
        static public void DbgOutput(string message,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Debug.Print(sourceFilePath + "(" + sourceLineNumber + "): " + message);
            Console.WriteLine(message);
        }
#else // !VS_2012
        static public void DbgOutput(string message)
        {
            Debug.Print(message);
            Console.WriteLine(message);
        }
#endif // VS_2012

        private sealed class Options
        {
            [Option('v', null, HelpText = "Print details during execution.")]
            public bool Verbose { get; set; }

            [Option('m', "model", HelpText = "Specify model to search for.")]
            public string Model { get; set; }

            [Option('i', "ip", HelpText = "Specify ip address to search for.")]
            public string IP { get; set; }

            [Option('p', "port", HelpText = "Specify port address to search for.")]
            public string Port { get; set; }

            [Option("RequestingAListOfDataIDs", DefaultValue = true, HelpText = "Requesting A List Of Data IDs.")]
            public bool RequestingAListOfDataIDs { get; set; }

            [Option("RequestFullInformationAboutADataValue", DefaultValue = true, HelpText = "Request Full Information About A Data Value.")]
            public bool RequestFullInformationAboutADataValue { get; set; }

            [Option("RequestingAValue", DefaultValue = true, HelpText = "Requesting A Value.")]
            public bool RequestingAValue { get; set; }

            [Option("RequestASettingsGroup", DefaultValue = true, HelpText = "Request A Settings Group.")]
            public bool RequestASettingsGroup { get; set; }

            [Option("RequestSettingInformation", DefaultValue = true, HelpText = "Request Setting Information.")]
            public bool RequestSettingInformation { get; set; }

            [Option("RequestAListOfSettings", DefaultValue = true, HelpText = "Request A List Of Settings.")]
            public bool RequestAListOfSettings { get; set; }

            [Option("RegisterToReceiveAnEvent", DefaultValue = true, HelpText = "Register To Receive An Event.")]
            public bool RegisterToReceiveAnEvent { get; set; }

            [Option("MOBEvent", DefaultValue = true, HelpText = "MOB Event.")]
            public bool MOBEvent { get; set; }

            [Option("CreateAWaypoint", DefaultValue = true, HelpText = "Create A Waypoint.")]
            public bool CreateAWaypoint { get; set; }

            [Option("ActivateSilenceAcknowledgeDeactivateAnAlarm", DefaultValue = true, HelpText = "Activate Silence Acknowledge Deactivate An Alarm.")]
            public bool ActivateSilenceAcknowledgeDeactivateAnAlarm { get; set; }

            [Option("ResetATripLog", DefaultValue = true, HelpText = "Reset A Trip Log.")]
            public bool ResetATripLog { get; set; }

            [Option("RegisterForVesselsId", DefaultValue = true, HelpText = "Register For Vessels Id.")]
            public bool RegisterForVesselsId { get; set; }

            [Option("RegisterForVesselsType", DefaultValue = true, HelpText = "Register For Vessels Type.")]
            public bool RegisterForVesselsType { get; set; }

/*
            #region Specialized Option Attribute
            [ValueList(typeof(List<string>))]
            [DefaultValue(null)]
            public IList<string> DefinitionFiles { get; set; }

            [OptionList("o", "operators", Separator = ';', HelpText = "Operators included in processing (+;-;...)." +
                " Separate each operator with a semicolon." + " Do not include spaces between operators and separator.")]
            [DefaultValue(null)]
            public IList<string> AllowedOperators { get; set; }
            #endregion
*/

            #region Help Screen

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }

/*
            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this, delegate(HelpText current) {
                    if (this.LastPostParsingState.Errors.Count > 0)
                    {
                        var errors = current.RenderParsingErrorsText(this, 2); // indent with two spaces
                        if (!string.IsNullOrEmpty(errors))
                        {
                            current.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
                            current.AddPreOptionsLine(errors);
                        }
                    }
                });
            }
*/
/*
            [HelpOption]
            public string GetUsage()
            {
                var help = new HelpText { Heading = Program._headingInfo,
                    Copyright = new CopyrightInfo("Giacomo Stelluti Scala", 2005, 2012),
                    AdditionalNewLineAfterOption = true,
                    AddDashesToOption = true
                };
                this.HandleParsingErrorsInHelp(help);
                help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
                help.AddPreOptionsLine("the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
                help.AddPreOptionsLine("Usage: SampleApp -rMyData.in -wMyData.out --calculate");
                help.AddPreOptionsLine(string.Format("       SampleApp -rMyData.in -i -j{0} file0.def file1.def", 9.7));
                help.AddPreOptionsLine("       SampleApp -rMath.xml -wReport.bin -o *;/;+;-");
                help.AddOptions(this);

                return help;
            }

            private void HandleParsingErrorsInHelp(HelpText help)
            {
                if (this.LastPostParsingState.Errors.Count > 0)
                {
                    var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
                    if (!string.IsNullOrEmpty(errors))
                    {
                        help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
                        help.AddPreOptionsLine(errors);
                    }
                }
            }
*/
            #endregion
        }
        
        public class Listener
        {
            private GoFree _goFree;

            public Listener(GoFree goFree)
            {
                _goFree = goFree;
            }

            public void Subscribe(GoFree goFree)
            {
                goFree.Tick += new GoFree.TickHandler(HeardIt);
            }

            private void HeardIt(string className, Root root)
            {
//                System.Console.WriteLine("HEARD IT");

                switch (className)
                {
                    case "DataList":
                        {
                            RootDataList rootDataList = root as RootDataList;

//                            RootDataInfoReq rootDataInfoReq = new RootDataInfoReq();
//                            rootDataInfoReq.DataInfoReq = new List<int>();

                            string output = string.Format("\tDataList:");
                            DbgOutput(output);
                            output = "";
                            foreach (int n in rootDataList.DataList.list)
                            {
                                output += string.Format("{0} ", n);
//                                rootDataInfoReq.DataInfoReq.Add(n);
                            }
                            DbgOutput(output);

//                            string data;
//                            data = JsonConvert.SerializeObject(rootDataInfoReq);
//                            _goFree.Write(data);
                        }
                        break;

                    case "DataInfo":
                        {
                            RootDataInfo rootDataInfo = root as RootDataInfo;

                            DbgOutput("\tDataInfo:");
                            foreach (DataInfo dataInfo in rootDataInfo.DataInfo)
                            {
                                DbgOutput(string.Format("\t\tid: {0}, sname: {1}, lname: {2}, unit: {3}, min: {4}, max: {5}, numInstance: {6}", dataInfo.id, dataInfo.sname, dataInfo.lname, dataInfo.unit, dataInfo.min, dataInfo.max, dataInfo.numInstances));
                                if (dataInfo.instanceInfo != null)
                                {
                                    foreach (InstanceInfo instanceInfo in dataInfo.instanceInfo)
                                    {
                                        DbgOutput(string.Format("\t\tinst: {0}, location: {1}, str: {2}", instanceInfo.inst, instanceInfo.location, instanceInfo.str));
                                    }
                                }
                            }
                        }
                        break;

                    case "Data":
                        {
                            RootData rootData = root as RootData;

                            DbgOutput("\tData:");
                            foreach (Data data in rootData.Data)
                            {
                                DbgOutput(string.Format("\t\tid: {0}, val: {1}, valStr: {2}, sysVal: {3}, inst: {4}, valid: {5}", data.id, data.val, data.valStr, data.sysVal, data.inst, data.valid));
                            }
                        }
                        break;

                    case "SettingList":
                        {
                            RootSettingList rootSettingList = root as RootSettingList;

                            DbgOutput(string.Format("\tSettingList:\r\n\tgroupId: {0}", rootSettingList.SettingList.groupId));
                            foreach (int setting in rootSettingList.SettingList.list)
                            {
                                DbgOutput(string.Format("\t\tsetting: {0}", setting));
                            }
                        }
                        break;

                    case "SettingInfo":
                        {
                            RootSettingInfo rootSettingInfo = root as RootSettingInfo;

                            DbgOutput("\tSettingInfo:");
                            foreach (SettingInfo settingInfo in rootSettingInfo.SettingInfo)
                            {
                                DbgOutput(string.Format("\t\tid: {0}, name: {1}, type: {2}, low: {3}, high: {4}, step: {5}", settingInfo.id, settingInfo.name, settingInfo.type, settingInfo.low, settingInfo.high, settingInfo.step));
                                if (settingInfo.values != null)
                                {
                                    foreach (Value value in settingInfo.values)
                                    {
                                        DbgOutput(string.Format("\t\t\tid: {0}, title: {1}", value.id, value.title));
                                    }
                                }
                            }
                        }
                        break;

                    case "Setting":
                        {
                            RootSetting rootSetting = root as RootSetting;

                            DbgOutput("\tSetting:");
                            foreach (Setting setting in rootSetting.Setting)
                            {
                                DbgOutput(string.Format("\t\tid: {0}, value: {1}", setting.id, setting.value));
                            }
                        }
                        break;

                    case "Traffic":
                        {
                            RootTraffic rootTraffic = root as RootTraffic;

                            DbgOutput("\tTraffic:");
                            foreach (Traffic traffic in rootTraffic.Traffic)
                            {
                                DbgOutput(string.Format("\t\ttype: {0}, id: {1}, name: {2}, lost: {3}, distance: {4}, lat: {5}, lon: {6}, trueBearing: {7}, cpa: {8}, tcpa: {9}, sog: {10}, cog: {11}, relativesog: {12}, relativecog: {13}, status: {14}",
                                    traffic.type, traffic.id, traffic.name, traffic.lost, traffic.distance, traffic.lat, traffic.lon, traffic.trueBearing, traffic.cpa, traffic.tcpa, traffic.sog, traffic.cog, traffic.relativeSog, traffic.relativeCog, traffic.status));
                            }
                        }
                        break;

                    case "EventSet":
                        {
                            RootEventSet rootEventSet = root as RootEventSet;

                            DbgOutput("\tEventSet:");
                            foreach (EventSet eventSet in rootEventSet.EventSet)
                            {
                                DbgOutput(string.Format("\t\tid: {0}, name: {1}, severity: {2}, latitude: {3}, longitude: {4}, alarmId: {5}, alarmInstance: {6}, alarmText: {7}, inst: {8}, active: {9}, wpName: {10}, wpId: {11}",
                                    eventSet.id, eventSet.name, eventSet.severity, eventSet.latitude, eventSet.longitude, eventSet.alarmId, eventSet, eventSet.alarmInstance, eventSet.alarmText, eventSet.inst, eventSet.active, eventSet.wpName, eventSet.wpId));
                            }
                        }
                        break;

                    default:
                        {
                            DbgOutput(string.Format("Eh? {0}", className));
                        }
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            var options = new Options();
            CommandLine.ParserSettings parserSettings = new ParserSettings();
            parserSettings.HelpWriter = Console.Error;
            CommandLine.Parser parser = new Parser(parserSettings);
            if (parser.ParseArguments(args, options))
            {
                if (options.Verbose)
                {
                    Console.WriteLine(options.Model);
                    Console.WriteLine(options.IP);
                    Console.WriteLine(options.Port);
                }
            }
            else
            {
                Environment.Exit(1);
            }

            string output;
/*
            string test1 = "{\"SettingInfo\":[{\"id\":40, \"name\":\"Boat Type\", \"type\":1, \"values\":[{\"id\":0, \"title\":\"Sailing\"}, {\"id\":1,\"title\":\"Fishing\"}, {\"id\":2, \"title\":\"Planing\" }]}]}";
            RootSettingInfo rootSettingInfo1 = JsonConvert.DeserializeObject<RootSettingInfo>(test1);

            string test2 = "{\"SettingInfo\":[{\"id\":1,\"name\":\"Backlight Level\",\"type\":2,\"low\":0,\"high\":10}]}";
            RootSettingInfo rootSettingInfo2 = JsonConvert.DeserializeObject<RootSettingInfo>(test2);

            string test3 = "{\"SettingInfo\":[{\"id\":2,\"name\":\"Night Mode\",\"type\":3}]}";
            RootSettingInfo rootSettingInfo3 = JsonConvert.DeserializeObject<RootSettingInfo>(test3);
*/

            Navico.NavicoDiscovery.MFD websocketMFD = null;

            if ((options.IP == null) && (options.Port == null))
            {
                List<Navico.NavicoDiscovery.MFD> MFDList = new List<Navico.NavicoDiscovery.MFD>();

                Thread newThread;
                newThread = new Thread(() => Navico.NavicoDiscovery.ReceiveNewMessage(MFDList));
                newThread.IsBackground = true;
                newThread.Start();

                newThread.Join();

                foreach (Navico.NavicoDiscovery.MFD mfd in MFDList)
                {
                    if (options.Model != null)
                    {
//                        if (string.Compare(mfd.Model, options.Model, true) != 0)
                        if (mfd.Model.IndexOf(options.Model) != 0)
                        {
                            continue;
                        }
                    }
                    if (options.IP != null)
                    {
//                        if (string.Compare(mfd.IP, options.IP, true) != 0)
                        if (mfd.IP.IndexOf(options.IP) != 0)
                        {
                            continue;
                        }
                    }

                    DbgOutput(string.Format("MFD: {0}\tModel: {1}\tIP: {2}", mfd.Name, mfd.Model, mfd.IP));
                    foreach (Navico.NavicoDiscovery.MFDService service in mfd.Services)
                    {
                        DbgOutput(string.Format("\tService: {0}\tVersion: {1}\tPort: {2}", service.Service, service.Version, service.Port));

                        if ((service.Service == "navico-nav-ws") /*|| (service.Service == "navico-navdata-websocket")*/)
                        {
                            if (websocketMFD == null)
                            {
                                websocketMFD = mfd;
                                websocketMFD.Services = null;
                                websocketMFD.Services = new NavicoDiscovery.MFDService[1];
                                websocketMFD.Services[0] = service;
                            }
                        }
                    }
                }

                if (websocketMFD == null)
                {
                    return;
                }
            }
            else
            {
                websocketMFD = new NavicoDiscovery.MFD();
                websocketMFD.IP = options.IP;
                websocketMFD.Services = new NavicoDiscovery.MFDService[1];

                uint port = 2053;
                if (!options.Port.IsNullOrEmpty())
                {
                    port = Convert.ToUInt32(options.Port);
                }
                websocketMFD.Services[0] = new NavicoDiscovery.MFDService("navico-nav-ws", 0, port);
            }

            DbgOutput(string.Format("Connect to {0}:{1}", websocketMFD.IP, websocketMFD.Services[0].Port));

            string wsURL;
            wsURL = string.Format("ws://{0}:{1}", websocketMFD.IP, websocketMFD.Services[0].Port);
            //wsURL = string.Format("ws://172.28.29.224:2053");
            using (GoFree streamer = new GoFree(wsURL))
            {
                Listener l = new Listener(streamer);
                l.Subscribe(streamer);
                streamer.Connect();

                Thread.Sleep(500);
                Console.WriteLine("\nType \"exit\" to exit.\n");

                string data;

                if (options.RequestingAListOfDataIDs)
                {
                    DbgOutput("Requesting a list of data IDs");

//                    data = "{\"DataListReq\":{\"group\":1}}";
//                    streamer.Write(data);

                    RootDataListReq rootDataListReq = new RootDataListReq();
                    rootDataListReq.DataListReq = new DataListReq();

                    foreach (eDataGroup dataGroup in System.Enum.GetValues(typeof(eDataGroup)))
                    {
                        rootDataListReq.DataListReq.group = (int)dataGroup;
                        data = JsonConvert.SerializeObject(rootDataListReq);
                        streamer.Write(data);
                    }

                    Thread.Sleep(1000);
                }

                if (options.RequestFullInformationAboutADataValue)
                {
                    DbgOutput("Request full information about a data value");

//                    data = "{\"DataInfoReq\":[40]}";
//                    streamer.Write(data);

                    foreach (eDataType dataType in System.Enum.GetValues(typeof(eDataType)))
                    {
                        RootDataInfoReq rootDataInfoReq = new RootDataInfoReq();
                        rootDataInfoReq.DataInfoReq = new List<int>();
                        rootDataInfoReq.DataInfoReq.Add((int)dataType);
                        data = JsonConvert.SerializeObject(rootDataInfoReq);
                        streamer.Write(data);
                    }

                    Thread.Sleep(1000);
                }

                if (options.RequestingAValue)
                {
                    DbgOutput("Requesting a value");

//                    data = "{\"DataReq\":[{\"id\":1,\"repeat\":false,\"inst\":0}]}";
//                    streamer.Write(data);

                    foreach (eDataType dataType in System.Enum.GetValues(typeof(eDataType)))
                    {
                        RootDataReq rootDataReq = new RootDataReq();
                        rootDataReq.DataReq = new List<DataReq>();
                        DataReq dataReq = new DataReq();
                        dataReq.id = (int)dataType;
                        rootDataReq.DataReq.Add(dataReq);
                        data = JsonConvert.SerializeObject(rootDataReq);
                        streamer.Write(data);
                    }

                    Thread.Sleep(1000);
                }

                if (options.RequestASettingsGroup)
                {
                    DbgOutput("Request a settings group");

//                    data = "{\"SettingListReq\":[{\"group\":1}]}";
//                    streamer.Write(data);

                    foreach (eDataGroup dataGroup in System.Enum.GetValues(typeof(eDataGroup)))
                    {
                        RootSettingListReq rootSettingListReq = new RootSettingListReq();
                        rootSettingListReq.SettingListReq = new List<SettingListReq>();
                        SettingListReq settingListReq = new SettingListReq();
                        settingListReq.groupId = (int)dataGroup;
                        rootSettingListReq.SettingListReq.Add(settingListReq);
                        data = JsonConvert.SerializeObject(rootSettingListReq);
                        streamer.Write(data);
                    }

                    Thread.Sleep(1000);
                }

                if (options.RequestSettingInformation)
                {
                    DbgOutput("Request setting information");

//                    data = "{\"SettingInfoReq\":[1]}";
//                    streamer.Write(data);

                    foreach (eSettingIDs setting in System.Enum.GetValues(typeof(eSettingIDs)))
                    {
                        RootSettingInfoReq rootSettingInfoReq = new RootSettingInfoReq();
                        rootSettingInfoReq.SettingInfoReq = new List<int>();
                        rootSettingInfoReq.SettingInfoReq.Add((int)setting);
                        data = JsonConvert.SerializeObject(rootSettingInfoReq);
                        streamer.Write(data);
                    }

                    Thread.Sleep(1000);
                }

                if (options.RequestAListOfSettings)
                {
                    DbgOutput("Request a list of settings");

//                    data = "{\"SettingReq\":{\"ids\":[1,2,3],\"register\":false}}";
//                    streamer.Write(data);

                    foreach (eSettingIDs setting in System.Enum.GetValues(typeof(eSettingIDs)))
                    {
                        RootSettingReq rootSettingReq = new RootSettingReq();
                        rootSettingReq.SettingReq = new SettingReq();
                        rootSettingReq.SettingReq.ids = new List<int>();
                        rootSettingReq.SettingReq.ids.Add((int)setting);
                        data = JsonConvert.SerializeObject(rootSettingReq);
                        streamer.Write(data);
                    }

                    Thread.Sleep(1000);
                }

                if (options.RegisterToReceiveAnEvent)
                {
                    DbgOutput("Register to receive an event");

//                    data = "{\"EventReg\":[1,2]}";
//                    streamer.Write(data);

                    foreach (eEvents eventValue in System.Enum.GetValues(typeof(eEvents)))
                    {
                        RootEventReg rootEventReg = new RootEventReg();
                        rootEventReg.EventReg = new List<int>();
                        rootEventReg.EventReg.Add((int)eventValue);
                        data = JsonConvert.SerializeObject(rootEventReg);
                        streamer.Write(data);
                    }

                    Thread.Sleep(1000);
                }

/*
                if (true)
                {
                    DbgOutput("Set event");

                    foreach (eEvents eventValue in System.Enum.GetValues(typeof(eEvents)))
                    {
                        RootEventSet rootEventSet = new RootEventSet();
                        rootEventSet.EventSet = new List<EventSet>();
                        EventSet eventSet = new EventSet();
                        eventSet.id = (int)eventValue;
                        eventSet.
                        rootEventSet.EventSet.Add(eventSet);
                        data = JsonConvert.SerializeObject(rootEventSet);
                        streamer.Write(data);
                    }

                    Thread.Sleep(1000);
                }
*/

                if (options.MOBEvent)
                {
                    DbgOutput("MOB event");

                    RootEventSet rootEventSet = new RootEventSet();
                    rootEventSet.EventSet = new List<EventSet>();
                    EventSet eventSet = new EventSet();
                    eventSet.id = (int)eEvents.MOB;
                    eventSet.name = "MOB";
                    eventSet.active = true;
                    rootEventSet.EventSet.Add(eventSet);
                    data = JsonConvert.SerializeObject(rootEventSet);
                    streamer.Write(data);

                    Thread.Sleep(1000);
                }

                if (options.CreateAWaypoint)
                {
                    DbgOutput("Create a waypoint");

                    RootEventSet rootEventSet = new RootEventSet();
                    rootEventSet.EventSet = new List<EventSet>();
                    EventSet eventSet = new EventSet();
                    eventSet.id = (int)eEvents.WaypointCreate;
                    eventSet.latitude = 50.9892;
                    eventSet.longitude = -1.4975;
                    eventSet.wpName = "Waypoint1";
                    rootEventSet.EventSet.Add(eventSet);
                    data = JsonConvert.SerializeObject(rootEventSet);
                    streamer.Write(data);

                    Thread.Sleep(1000);
                }

                if (options.ActivateSilenceAcknowledgeDeactivateAnAlarm)
                {
                    DbgOutput("Activate/Silence/Acknowledge/Deactivate an alarm");

                    RootEventSet rootEventSet = new RootEventSet();
                    rootEventSet.EventSet = new List<EventSet>();
                    EventSet eventSet = new EventSet();
//                    eventSet.id = (int)eEvents.AlarmActivate;
                    eventSet.id = (int)eEvents.AlarmSilence;
//                    eventSet.id = (int)eEvents.AlarmAcknowledge;
//                    eventSet.id = (int)eEvents.AlarmDeactivate;
                    eventSet.alarmId = 5;
                    eventSet.alarmText = "Low Speed";
                    eventSet.severity = 1;
                    rootEventSet.EventSet.Add(eventSet);
                    data = JsonConvert.SerializeObject(rootEventSet);
                    streamer.Write(data);

                    Thread.Sleep(1000);
                }

                if (options.ResetATripLog)
                {
                    DbgOutput("Reset a trip log");

                    RootEventSet rootEventSet = new RootEventSet();
                    rootEventSet.EventSet = new List<EventSet>();
                    EventSet eventSet = new EventSet();
                    eventSet.id = (int)eEvents.TripLogReset;
                    eventSet.inst = 0;
                    rootEventSet.EventSet.Add(eventSet);
                    data = JsonConvert.SerializeObject(rootEventSet);
                    streamer.Write(data);

                    Thread.Sleep(1000);
                }

                if (options.RegisterForVesselsId)
                {
                    DbgOutput("Register for vessels (id)");

//                    data = "{\"TrafficReq\":{\"subscribe\":true,\"id\":12345,\"sourceType\":0}}";
//                    streamer.Write(data);

                    RootTrafficReq rootTrafficReq = new RootTrafficReq();
                    rootTrafficReq.TrafficReq = new TrafficReq();
                    rootTrafficReq.TrafficReq.subscribe = true;
                    rootTrafficReq.TrafficReq.id = 12345;
                    rootTrafficReq.TrafficReq.sourceType = 0;
                    data = JsonConvert.SerializeObject(rootTrafficReq);
                    streamer.Write(data);

                    Thread.Sleep(1000);
                }

                if (options.RegisterForVesselsType)
                {
                    DbgOutput("Register for vessels (type)");

//                    data = "{\"TrafficReg\":[0,2]}";
//                    streamer.Write(data);

                    RootTrafficReg rootTrafficReg = new RootTrafficReg();
                    rootTrafficReg.TrafficReg = new List<int>();
                    rootTrafficReg.TrafficReg.Add(0);
                    rootTrafficReg.TrafficReg.Add(2);
                    data = JsonConvert.SerializeObject(rootTrafficReg);
                    streamer.Write(data);
                }

                while (true)
                {
                    Thread.Sleep(500);

                    Console.Write("> ");
                    data = Console.ReadLine();
                    if (data == "exit")
                    {
                        break;
                    }

                    streamer.Write(data);
                }
            }
        }
    }
}
