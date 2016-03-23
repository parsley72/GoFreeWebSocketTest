using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using WebSocketSharp;
//using WebSocketSharp.Frame;

namespace Navico
{
    enum eDataGroup
    {
        GPS = 1,
        Navigation = 2,
        Vessel = 3,
        Sonar = 4,
        Weather = 5,
        Trip = 6,
        Time = 7,
        Engine = 8,
        Transmission = 9,
        FuelTank = 10,
        FreshWaterTank = 11,
        GrayWaterTank = 12,
        LiveWellTank = 13,
        OilTank = 14,
        BlackWaterTank = 15,
        EngineRoom = 16,
        Cabin = 17,
        BaitWell = 18,
        Refrigerator = 19,
        HeatingSystem = 20,
        Freezer = 21,
        Battery = 22,
        Rudder = 23,
        TrimTab = 24,
        ACInput = 25,
        DigitalSwitching = 26,
        Other = 27,
        GPSStatus = 28,
        RouteData = 29,
        SpeedDepth = 30,
        LogTimer = 31,
        Environment = 32,
        Wind = 33,
        Pilot = 34,
        Sailing = 35,
        AcOutput = 36,
        Charger = 37,
        Inverter = 38,

        Count = 39,
        AllData = 40,
        Invalid = 255
    };

    enum eDataType
    {
        Altitude            = 1,
        PositionError       = 3,
        Hdop                = 4,
        Vdop                = 5,
        Tdop                = 6,
        Pdop                = 7,
        GeoidalSeperation   = 8,
        Cog                 = 9,
        PositionQuality     = 10,
        PositionIntegrity   = 11,
        SatsInView          = 12,
        WaasStatus          = 13,
        Bearing             = 14,
        Course              = 15,
        CourseToSteer       = 17,
        CrossTrack          = 18,
        VelocityMadeGood    = 19,
        Destination         = 20,
        DistanceToTurn      = 21,
        DistanceToDest      = 22,
        TimeToTurn          = 23,
        TimeToDest          = 24,
        EtaAtTurn           = 25,
        EtaAtDest           = 26,
        TotalDistance       = 27,
        SteerArrow          = 28,
        Odometer            = 29,
        TripDistance        = 30,
        TripTime            = 31,
        Date                = 32,
        Time                = 33,
        UtcDate             = 34,
        UtcTime             = 35,
        LocalTimeOffset     = 36,
        Heading             = 37,
        Voltage             = 38,
        CurrentSet          = 39,
        CurrentDrift        = 40,
        SpeedSog            = 41,
        SpeedWater          = 42,
        SpeedPitot          = 43,
        SpeedTripAvg        = 44,
        SpeedTripMax        = 45,
        SpeedWindApp        = 46,
        SpeedWindTrue       = 47,
        TempWater           = 48,
        TempOutside         = 49,
        TempInside          = 50,
        TempEngineRoom      = 51,
        TempMainCabin       = 52,
        TempLiveWell        = 53,
        TempBaitWell        = 54,
        TempRefrigeration   = 55,
        TempHeatingSystem   = 56,
        TempDewPoint        = 57,
        TempWindChillApp    = 58,
        TempWindChillTheoretic  = 59,
        TempHeatIndex       = 60,
        TempFreezer         = 61,
        EngineTemp          = 62,
        EngineAirTemp       = 63,
        EngineOilTemp       = 64,
        TempBattery         = 65,
        PressureAtmospheric = 66,
        EngineBoostPres     = 67,
        EngineOilPres       = 68,
        EngineWaterPres     = 69,
        EngineFuelPres      = 70,
        EngineManifoldPres  = 71,
        PressureSteam       = 72,
        PressureComprAir    = 73,
        PressureHydraulic   = 74,
        Depth               = 77,
        WaterDistance       = 78,
        EngineRpm           = 79,
        EngineTrim          = 80,
        EngineAlternatorPotential   = 81,
        EngineFuelRate      = 82,
        EnginePercentLoad   = 83,
        EnginePercentTorque = 84,
        SuzukiAlarmLevLo    = 85,
        SuzukiAlarmLevHigh  = 86,
        TankFuelLevel       = 87,
        FluidLevelFreshWater    = 88,
        FluidLevelGrayWater = 89,
        FluidLevelLiveWell  = 90,
        FluidLevelOil       = 91,
        FluidLevelBlackWater    = 92,
        TankFuelRemaining   = 93,
        FluidVolumeFreshWater   = 94,
        FluidVolumeGrayWater    = 95,
        FluidVolumeLiveWell = 96,
        FluidVolumeOil      = 97,
        FluidVolumeBlackWater   = 98,
        GenFluidVolume      = 99,
        GenTankCapacity     = 105,
        TankFuelCapacity    = 106,
        TankCapacityFreshWater  = 107,
        TankCapacityGrayWater   = 108,
        TankCapacityLiveWell    = 109,
        TankCapacityOil     = 110,
        TankCapacityBlackWater    = 111,
        TankFuelUsed        = 112,
        EngineFuelUsed      = 113,
        EngineFuelUsedTrip  = 114,
        EngineFuelUsedSeasonal  = 115,
        EngineFuelKValue    = 116,
        BatteryPotential    = 117,
        BatteryCurrent      = 118,
        TrimTab             = 119,
        RateOfTurn          = 121,
        AttitudeYaw         = 122,
        AttitudePitch       = 123,
        AttitudeRoll        = 124,
        MagneticVariation   = 125,
        Deviation           = 126,
        FuelEconomyWtr      = 127,
        FuelEconomyGps      = 128,
        FuelRangeWtr        = 130,
        FuelRangeGps        = 131,
        EngineHoursUsed     = 132,
        EngineType          = 133,
        VesselFuelRate      = 134,
        VesselFuelEconomyWtr    = 135,
        VesselFuelEconomyGps    = 136,
        VesselFuelRemaining     = 137,
        VesselFuelRangeWtr      = 138,
        VesselFuelRangeGps      = 139,
        WindAppDirection    = 140,
        WindTrueAngle       = 141,
        WindTrueDirection   = 142,
        HumidityInside      = 143,
        HumidityOutside     = 144,
        SetHumidity         = 145,
        RudderAngle         = 146,
        TransGear           = 147,
        TransOilPressure    = 148,
        TransOilTemp        = 149,
        CmdRudderAngle      = 150,
        RudderLimit         = 151,
        OffHeadingLim       = 152,
        RadiusOfTurnOrder   = 153,
        RateOfTurnOrder     = 154,
        OffTrackLim         = 155,
        LoggingTimeRemaining    = 156,
        PositionFixType     = 157,
        EngineDiscreteStatus    = 158,
        TransmissionDiscreteStatus  = 159,
        GpsBestOfFourSnr    = 160,
        GenFluidLevel       = 161,
        GenPressure         = 162,
        GenTemperature      = 163,
        InternalVoltage     = 164,
        StructureDepth      = 166,
        LoranPosition       = 167,
        VesselStatus        = 168,
        BatteryDcType       = 169,
        BatteryStateOfCharge    = 170,
        BatteryStateOfHealth    = 171,
        BatteryTimeRemaining    = 172,
        BatteryRippleVoltage    = 173,
        Ac1Acceptability    = 174,
        Ac2Acceptability    = 175,
        Ac3Acceptability    = 176,
        Ac1Voltage          = 177,
        Ac2Voltage          = 178,
        Ac3Voltage          = 179,
        Ac1Current          = 180,
        Ac2Current          = 181,
        Ac3Current          = 182,
        Ac1Frequency        = 183,
        Ac2Frequency        = 184,
        Ac3Frequency        = 185,
        Ac1BreakerSize      = 186,
        Ac2BreakerSize      = 187,
        Ac3BreakerSize      = 188,
        Ac1RealPower        = 189,
        Ac2RealPower        = 190,
        Ac3RealPower        = 191,
        Ac1ReactivePower    = 192,
        Ac2ReactivePower    = 193,
        Ac3ReactivePower    = 194,
        Ac1PowerFactor      = 195,
        Ac2PowerFactor      = 196,
        Ac3PowerFactor      = 197,
        SwitchState         = 198,
        SwitchCurrent       = 199,
        SwitchFault         = 200,
        SwitchDimLevel      = 201,
        PreviousCmdHeading  = 202,
        CmdWindAngle        = 203,
        CmdBearingOffset    = 204,
        CmdBearing          = 205,
        CmdDepthContour     = 206,
        CmdCourseChange     = 207,
        PilotDrift          = 208,
        PilotDistanceToTurn = 209,
        PilotTimeToTurn     = 210,
        PilotReferencePosition  = 211,
        DcStatus            = 212,
        AcStatus            = 213,
        SwitchVoltage       = 214,
        BatteryCapacityRemaining    = 215,
        H3000Linear1        = 217,
        H3000Linear2        = 218,
        H3000Linear3        = 219,
        BoomPosition        = 220,
        SailingCourse       = 221,
        DaggerboardPosition = 222,
        H3000Linear4        = 223,
        HeadingOnNextTack   = 224,
        KeelAngle           = 225,
        Leeway              = 226,
        MastAngle           = 227,
        TargetTrueWindAngle = 228,
        KeelTrimTab         = 229,
        RaceTimer           = 230,
        CanardAngle         = 231,
        NextLegApparentWindAngle    = 232,
        NextLegApparentWindSpeed    = 233,
        TargetBoatSpeed     = 234,
        VmgToWind           = 235,
        TimeToLaylines      = 236,
        DistanceToLaylines  = 237,
        AftDepth            = 238,
        ForeStay            = 239,
        PolarSpeed          = 240,
        PolarPerformance    = 241,
        TackingPerformance  = 242,
        WindAngleToMast     = 243,
        CanBusVoltage       = 244,
        InternalTemperature = 245,
        EngageCurrent       = 246,
        UrefVoltage         = 247,
        SupplyVoltage       = 248,
        DestinationPosition = 249,
        EngineSyncState     = 252,
        EngineGeneralMaintenance    = 253,
        EnginePercentThrottle   = 254,
        EngineSteeringAngle = 255,
        EngineBreakInReqd   = 256,
        EngineBreakInAccum  = 258,
        EngineTrimStatus    = 259,
        PilotPresent        = 260,
        Ac1OutWaveform      = 261,
        Ac2OutWaveform      = 262,
        Ac3OutWaveform      = 263,
        Ac1OutVoltage       = 264,
        Ac2OutVoltage       = 265,
        Ac3OutVoltage    = 266,
        Ac1OutCurrent    = 267,
        Ac2OutCurrent    = 268,
        Ac3OutCurrent    = 269,
        Ac1OutFrequency    = 270,
        Ac2OutFrequency    = 271,
        Ac3OutFrequency    = 272,
        Ac1OutBreakerSize    = 273,
        Ac2OutBreakerSize    = 274,
        Ac3OutBreakerSize    = 275,
        Ac1OutRealPower    = 276,
        Ac2OutRealPower    = 277,
        Ac3OutRealPower    = 278,
        Ac1OutReactivePower    = 279,
        Ac2OutReactivePower    = 280,
        Ac3OutReactivePower    = 281,
        Ac1OutPowerFactor    = 282,
        Ac2OutPowerFactor    = 283,
        Ac3OutPowerFactor    = 284,
        Ac2Status    = 285,
        Ac3Status    = 286,
        Ac1OutStatus    = 287,
        Ac2OutStatus    = 288,
        Ac3OutStatus    = 289,
        SwitchManualOverride    = 290,
        SwitchReversePolarity    = 291,
        SwitchAcsourceAvailable    = 292,
        SwitchAccontactorSystemsonstate    = 293,
        ChargerBatteryInstance    = 294,
        ChargerOperatingState    = 295,
        ChargerMode    = 296,
        ChargerEnabled    = 297,
        ChargerEqualizationPending    = 298,
        ChargerEqualizationTimeRemaining    = 299,
        InverterAcInstance    = 300,
        InverterDcInstance    = 301,
        InverterOperatingState    = 302,
        InverterEnabled    = 303,
        SailingTimeToWaypoint   = 304,
        SailingDistanceToWaypoint   = 305,
        SailingETA                  = 306,
        Latitude    = 309,
        Longitude    = 310,
    };

/*
    enum eInstanceLocation // TODO A: update this to match the enum in NOS
    {
        locPORT             = 0,
        locPORT_CENTER      = 1,
        locCENTER_PORT      = 2,
        locCENTER           = 3,
        locCENTER_STARBOARD = 4,
        locSTARBOARD_CENTER = 5,
        locSTARBOARD        = 6,
        locPRIMARY          = 7,
        locOTHER            = 8,

        NUM_LOCATIONS
    };
*/

    enum eSettingTypes
    {
        Enumeration = 1,
        Boolean     = 2,
        Number      = 3,
    };

    enum eSettingGroups
    {
        Display = 1,
        Units   = 2,
        Alarms  = 3,
        TripLog = 4,
    };

    enum eSettingIDs
    {
        BacklightLevel   = 1,
        NightMode        = 2,
        TripLog1         = 3,
        TripLog2         = 4,
//        DistanceUnits    = 5,
        // TODO A: Populate the rest of this
    };

    enum eEvents
    {
        AlarmActivate      = 1,
        AlarmDeactivate    = 2,
        AlarmAcknowledge   = 3,
        AlarmSilence       = 4,
        MOB                = 5,
        WaypointCreate     = 6,
        TripLogReset       = 7,
        ZoomIn             = 8,
        ZoomOut            = 9,
    };

    enum eVesselTypes
    {
        AIS = 1,
    };

    enum eVesselStatus
    {
        Dangerous       = 1,
        Safe            = 2,
        Acquiring       = 3,
        AcquiringFailed = 4,
        OutOfRange      = 5,
        LostOutOfRange  = 6,
    };

    public class Root
    {
//        public string key { get; set; }
    }

    public class DataList
    {
        public int groupId { get; set; }
        public List<int> list { get; set; }
    }

    public class RootDataList : Root
    {
        public DataList DataList { get; set; }
    }

    public class InstanceInfo
    {
        public int inst { get; set; }
        public int location { get; set; }
        public string str { get; set; }
    }

    public class DataInfo
    {
        public int id { get; set; }
        public string sname { get; set; }
        public string lname { get; set; }
        public string unit { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public int numInstances { get; set; }
        public List<InstanceInfo> instanceInfo { get; set; }
    }

    public class RootDataInfo : Root
    {
        public List<DataInfo> DataInfo { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public double val { get; set; }
        public string valStr { get; set; }
        public double sysVal { get; set; }
        public int inst { get; set; }
        public bool valid { get; set; }
    }

    public class RootData : Root
    {
        public List<Data> Data { get; set; }
    }

    public class SettingList
    {
        public int groupId { get; set; }
        public List<int> list { get; set; }
    }

    public class RootSettingList : Root
    {
        public SettingList SettingList { get; set; }
    }

    public class Value
    {
        public int id { get; set; }
        public string title { get; set; }
    }

    public class SettingInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public List<Value> values { get; set; }
        public double low { get; set; }
        public double high { get; set; }
        public double step { get; set; }
    }

    public class RootSettingInfo : Root
    {
        public List<SettingInfo> SettingInfo { get; set; }
    }

    public class Setting
    {
        public int id { get; set; }
        public bool value { get; set; }
    }

    public class RootSetting : Root
    {
        public List<Setting> Setting { get; set; }
    }

    public class TrafficReq
    {
        public bool subscribe { get; set; }
        public int id { get; set; }
        public int sourceType { get; set; }
    }

    public class RootTrafficReq
    {
        public TrafficReq TrafficReq { get; set; }
    }

    public class RootTrafficReg
    {
        public List<int> TrafficReg { get; set; }
    }

    public class RootTrafficUnreg
    {
        public List<int> TrafficUnreg { get; set; }
    }

    public class Dimensions
    {
        public double bow { get; set; }
        public double stern { get; set; }
        public double port { get; set; }
        public double stbd { get; set; }
    }

    public class Traffic
    {
        public string type { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public bool lost { get; set; }
        public double distance { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public double trueBearing { get; set; }
        public double cpa { get; set; }
        public int tcpa { get; set; }
        public double sog { get; set; }
        public double cog { get; set; }
        public double relativeSog { get; set; }
        public double relativeCog { get; set; }
        public string status { get; set; }
        public string eta { get; set; }
        public double draught { get; set; }
        public string destination { get; set; }
        public bool isAton { get; set; }
        public int atonType { get; set; }
        public Dimensions dimensions { get; set; }
        public bool offPosition { get; set; }

        public bool accuratePosition { get; set; }
        public int aisClass { get; set; }
        public string callsign { get; set; }
        public int imo { get; set; }
        public int sourceType { get; set; }
        public double trueHeading { get; set; }
    }

    public class RootTraffic : Root
    {
        public List<Traffic> Traffic { get; set; }
    }

    public class TrafficRemoved
    {
        public int sourceType { get; set; }
        public int id { get; set; }
    }

    public class RootTrafficRemoved
    {
        public List<TrafficRemoved> TrafficRemoved { get; set; }
    }

/*
    public struct QueueMessage
    {
        public string Summary;
        public var Class;
    }
*/

    public class DataListReq
    {
        public int group { get; set; }
    }

    public class RootDataListReq
    {
        public DataListReq DataListReq { get; set; }
    }

    public class RootDataInfoReq
    {
        public List<int> DataInfoReq { get; set; }
    }

    public class DataReq
    {
        public int id { get; set; }
        public bool repeat { get; set; }
        public int inst { get; set; }
    }

    public class RootDataReq
    {
        public List<DataReq> DataReq { get; set; }
    }

    public class UnsubscribeData
    {
        public int id { get; set; }
        public int inst { get; set; }
    }

    public class RootUnsubscribeData
    {
        public List<UnsubscribeData> UnsubscribeData { get; set; }
    }

    public class SettingListReq
    {
        public int groupId { get; set; }
    }

    public class RootSettingListReq
    {
        public List<SettingListReq> SettingListReq { get; set; }
    }

    public class RootSettingInfoReq
    {
        public List<int> SettingInfoReq { get; set; }
    }

    public class SettingReq
    {
        public List<int> ids { get; set; }
        public bool register { get; set; }
    }

    public class RootSettingReq
    {
        public SettingReq SettingReq { get; set; }
    }

    public class RootUnsubscribeSetting
    {
        public List<int> UnsubscribeSetting { get; set; }
    }

    public class RootEventReg
    {
        public List<int> EventReg { get; set; }
    }

    public class RootEventUnreg
    {
        public List<int> EventUnreg { get; set; }
    }

    public class EventSet
    {
        public int id { get; set; }
        public string name { get; set; }
        public int severity { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int alarmId { get; set; }
        public int alarmInstance { get; set; }
        public string alarmText { get; set; }
        public int inst { get; set; }
        public bool active { get; set; }
        public string wpName { get; set; }
        public string wpId { get; set; }
    }

    public class RootEventSet : Root
    {
        public List<EventSet> EventSet { get; set; }
    }

/*
    public class TrafficReq
    {
        public bool subscribe { get; set; }
        public int id { get; set; }
        public int sourceType { get; set; }
    }

    public class RootTrafficReq
    {
        public TrafficReq TrafficReq { get; set; }
    }

    public class RootTrafficReg
    {
        public List<int> TrafficReg { get; set; }
    }

    public class RootTrafficUnreg
    {
        public List<int> TrafficUnreg { get; set; }
    }

    public class TrafficRemoved
    {
        public int sourceType { get; set; }
        public int id { get; set; }
    }

    public class RootTrafficRemoved
    {
        public List<TrafficRemoved> TrafficRemoved { get; set; }
    }
*/

    public class ThreadState
    {
        public bool           Enabled      { get; set; }
        public AutoResetEvent Notification { get; private set; }

        public ThreadState()
        {
            Enabled      = true;
            Notification = new AutoResetEvent(false);
        }
    }

    public class TimeOfTick : EventArgs
    {
        private DateTime TimeNow;
        public DateTime Time
        {
            set
            {
                TimeNow = value;
            }
            get
            {
                return this.TimeNow;
            }
        }
    }

    public class GoFree : IDisposable
    {
        public delegate void TickHandler(string className, Root root);
        public event TickHandler Tick;
        
//        private Queue _msgQ;
        private WaitCallback            _notifyMsg;
        private ThreadState             _notifyMsgState;
        private TimerCallback           _sendHeartbeat;
        private Timer                   _heartbeatTimer;
        private WebSocket               _ws;

        public GoFree(string url)
        {
            _ws       = new WebSocket(url);
//            _msgQ     = Queue.Synchronized(new Queue());

            configure();
        }

        private void configure()
        {
            _ws.OnOpen += (sender, e) =>
            {
            };

            _ws.OnMessage += (sender, e) =>
            {
                switch (e.Type)
                {
                    case Opcode.Text:
                        /*var msg =*/ parseTextMessage(e.Data);
//                        _msgQ.Enqueue(msg);
                        break;
                    case Opcode.Binary:
                        break;
                    default:
                        break;
                }
            };

            _ws.OnError += (sender, e) =>
            {
//                enNfMessage("[GoFree] error", "WS: Error: " + e.Message, "notification-message-im");
            };

            _ws.OnClose += (sender, e) =>
            {
/*
                enNfMessage
                (
                    "[GoFree] disconnect",
                    String.Format("WS: Close({0}:{1}): {2}", (ushort)e.Code, e.Code, e.Reason),
                    "notification-message-im"
                );
*/
            };

            _notifyMsgState = new ThreadState();
            _notifyMsg = (state) =>
            {
                while (_notifyMsgState.Enabled /*|| _msgQ.Count > 0*/)
                {
                    Thread.Sleep(500);
/*
                    if (_msgQ.Count > 0)
                    {
                        QueueMessage msg = (QueueMessage)_msgQ.Dequeue();
                        Console.WriteLine("{0}", msg.Summary);
                    }
*/
                }

                _notifyMsgState.Notification.Set();
            };

            _sendHeartbeat = (state) =>
            {
//                var msg = createTextMessage("heartbeat", String.Empty);
//                _ws.Send(msg);
            };
        }

        private void parseTextMessage(string data)
        {
            JObject msg     = JObject.Parse(data);
            string key = "";
            foreach (var x in msg)
            {
                key = x.Key;
            }

            switch (key)
            {
                case "DataList":
                {
                    RootDataList rootDataList = JsonConvert.DeserializeObject<RootDataList>(data);
                    Tick(key, rootDataList);
                }
                break;

                case "DataInfo":
                {
                    RootDataInfo rootDataInfo = JsonConvert.DeserializeObject<RootDataInfo>(data);
                    Tick(key, rootDataInfo);
                }
                break;

                case "Data":
                {
                    RootData rootData = JsonConvert.DeserializeObject<RootData>(data);
                    Tick(key, rootData);
                }
                break;

                case "SettingList":
                {
                    RootSettingList rootSettingList = JsonConvert.DeserializeObject<RootSettingList>(data);
                    Tick(key, rootSettingList);
                }
                break;

                case "SettingInfo":
                {
                    RootSettingInfo rootSettingInfo = JsonConvert.DeserializeObject<RootSettingInfo>(data);
                    Tick(key, rootSettingInfo);
                }
                break;

                case "Setting":
                {
                    RootSetting rootSetting = JsonConvert.DeserializeObject<RootSetting>(data);
                    Tick(key, rootSetting);
                }
                break;

//                case "Vessel":
                case "Traffic":
                {
                    RootTraffic rootTraffic = JsonConvert.DeserializeObject<RootTraffic>(data);
                    Tick(key, rootTraffic);
                }
                break;

                case "EventSet":
                {
                    RootEventSet rootEventSet = JsonConvert.DeserializeObject<RootEventSet>(data);
                    Tick(key, rootEventSet);
                }
                break;

                default:
                {
                    System.Diagnostics.Debug.WriteLine("Eh? {0} {1}", key, data);
                }
                break;
            }
        }

        public void Connect()
        {
            _ws.Connect();

                ThreadPool.QueueUserWorkItem(_notifyMsg);
                _heartbeatTimer = new Timer(_sendHeartbeat, null, 30 * 1000, 30 * 1000);
        }

        public void Disconnect()
        {
                var wait = new AutoResetEvent(false);
                _heartbeatTimer.Dispose(wait);
                wait.WaitOne();

            _ws.Close();

                _notifyMsgState.Enabled = false;
                _notifyMsgState.Notification.WaitOne();
            }

        public void Dispose()
        {
            Disconnect();
        }

        public void Write(string data)
        {
            _ws.Send(data);
        }

        public void Write(FileInfo file)
        {
            throw new NotImplementedException();
        }
    }
}
