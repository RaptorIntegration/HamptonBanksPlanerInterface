using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Diagnostics;
using System.ServiceProcess;
using Logix;


namespace RaptorComm
{
    public static class RaptorCommService
    {
        public static System.Data.SqlClient.SqlConnection connection;
        public static string connectionString;
        static PeerMessage peerMsg = new PeerMessage();
        static Tag cpuState = new Tag("$CPU_STATE", Logix.Tag.ATOMIC.OBJECT);
        static Array stateCPU;

        static Thread LugThread = null;
        static Thread BinDataFromPLC = null;
        static Thread BinDataToPLC = null;
        static Thread StatusDataFromPLC = null;
        static Thread StatusDataVariableFromPLC = null;
        static Thread SortDataToPLC = null;
        static Thread ProductDataToPLC = null;
        static Thread ThicknessDataToPLC = null;
        static Thread WidthDataToPLC = null;
        static Thread LengthDataToPLC = null;
        static Thread MoistureDataToPLC = null;
        static Thread GradeDataToPLC = null;
        static Thread PLCStateThread = null;
        static Thread PLCPollThread = null;
        static Thread TimingDataToPLC = null;
        static Thread DiagnosticsDataToPLC = null;
        static Thread ParameterDataToPLC = null;
        static Thread DrivesDataToPLC = null;
        static Thread DataRequestsThread = null;
        public static int LugThreadScanRate = 50;
        public static int PLCStateScanRate = 2000;
        public static int PLCPollScanRate = 1000;
        public static int BinDataFromPLCScanRate = 50;
        public static int StatusDataFromPLCScanRate = 10;
        public static int StatusDataVariableFromPLCScanRate = 10;
        public static int BinDataToPLCScanRate = 10;
        public static int SortDataToPLCScanRate = 10;
        public static int ProductDataToPLCScanRate = 10;
        public static int ThicknessDataToPLCScanRate = 10;
        public static int WidthDataToPLCScanRate = 10;
        public static int LengthDataToPLCScanRate = 10;
        public static int MoistureDataToPLCScanRate = 10;
        public static int GradeDataToPLCScanRate = 10;
        public static int TimingDataToPLCScanRate = 10;
        public static int DiagnosticDataToPLCScanRate = 10;
        public static int ParameterDataToPLCScanRate = 10;
        public static int DrivesDataToPLCScanRate = 10;
        public static int DataRequestsScanRate = 10;
        public static int DataRequests = 0;
        
        ///////////////////////////////////////
        // initialize the Controller class
        static Logix.Controller MyPLCState = new Logix.Controller();
        static Logix.Controller MyPLC = new Logix.Controller();
        static Logix.Controller MyPollPLC = new Logix.Controller();
        static Logix.Controller MyPLCBin1 = new Logix.Controller();
        static Logix.Controller MyPLCBin = new Logix.Controller();
        static Logix.Controller MyPLCStatus = new Logix.Controller();
        static Logix.Controller MyPLCStatus1 = new Logix.Controller();
        static Logix.Controller MyPLCSort = new Logix.Controller();
        static Logix.Controller MyPLCProduct = new Logix.Controller();
        static Logix.Controller MyPLCThickness = new Logix.Controller();
        static Logix.Controller MyPLCWidth = new Logix.Controller();
        static Logix.Controller MyPLCLength = new Logix.Controller();
        static Logix.Controller MyPLCMoisture = new Logix.Controller();
        static Logix.Controller MyPLCGrade = new Logix.Controller();
        static Logix.Controller MyPLCTiming = new Logix.Controller();
        static Logix.Controller MyPLCParameter = new Logix.Controller();
        static Logix.Controller MyPLCDiagnostics = new Logix.Controller();
        static Logix.Controller MyPLCDrives = new Logix.Controller();
        static Logix.Controller MyPLCMisc = new Logix.Controller();
        ///////////////////////////////////////
        // initialize TagGroup class
        static Logix.TagGroup LugUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup MiscUDTGroup = new Logix.TagGroup();
        public static int LugIndex = 0;
        public static int LugIndexPLC = 0, oldLugIndexPLC = 0;
        public static int LastFrameStart = 0;
        public static int PLCCounter = 0;
        public static int WEBSortCounter = 0;
        public static int oldWEBSortCounter = 0;

        static Logix.TagGroup BinUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup BinSendUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup SortSendUDTGroup = new Logix.TagGroup();
        public static int BinIndex = 0;
        public static int BinIndexPLC = 0, oldBinIndexPLC = 0;
        public static int LastBinFrameStart = 0;
        public static int PLCBinCounter = 0;
        public static int WEBSortBinCounter = 0;
        public static int oldWEBSortBinCounter = 0;

        static Logix.TagGroup StatusUDTGroup = new Logix.TagGroup();
        public static int StatusIndex = 0;
        public static int StatusIndexPLC = 0, oldStatusIndexPLC = 0;
        public static int PLCStatusCounter = 0;
        public static int WEBSortStatusCounter = 0;
        public static int oldWEBSortStatusCounter = 0;

        static Logix.TagGroup ThicknessSendUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup ProductUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup WidthSendUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup LengthSendUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup MoistureSendUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup GradeSendUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup DiagnosticSendUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup DriveSendUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup PollUDTGroup = new Logix.TagGroup();
        static Logix.TagGroup PollUDTGroup1 = new Logix.TagGroup();
        static Logix.TagGroup PollUDTGroup2 = new Logix.TagGroup();
        static Logix.TagGroup DateTimeUDTGroup = new Logix.TagGroup();
        
        
        ///////////////////////////////////////
        // initialize tag classes /////////////        
        static Logix.Tag lugindex = new Logix.Tag("Program:WEBSort.TableIndex", Logix.Tag.ATOMIC.INT);
        static Logix.Tag binindex = new Logix.Tag("Program:WEBSort.BayTableIndex", Logix.Tag.ATOMIC.INT);
        static Logix.Tag statusindex = new Logix.Tag("Program:Utility.SystemStatusTableIndex", Logix.Tag.ATOMIC.INT);
        static Logix.Tag pollcount = new Logix.Tag("WEBSortPollCount", Logix.Tag.ATOMIC.DINT);
        static Logix.Tag lugrate = new Logix.Tag("WEBSortLPM", Logix.Tag.ATOMIC.DINT);
        static Logix.Tag PlanerSpeed = new Logix.Tag("PlanerSpeed", Logix.Tag.ATOMIC.DINT);
        static Logix.Tag ProductLength = new Logix.Tag("ProductLength", Logix.Tag.ATOMIC.DINT);
        static Logix.Tag activemessage = new Logix.Tag("ActiveMessage", Logix.Tag.ATOMIC.STRING);
        static Logix.Tag WEBSortRefresh = new Logix.Tag("Program:WEBSort.WEBSortRefresh", Logix.Tag.ATOMIC.BOOL);
        static Logix.Tag ShiftReset = new Logix.Tag("WEBSortShiftEnd", Logix.Tag.ATOMIC.BOOL);
        static Logix.Tag RunThick = new Logix.Tag("WEBSortRunThick", Logix.Tag.ATOMIC.REAL);
        static Logix.Tag RunWidth = new Logix.Tag("WEBSortRunWidth", Logix.Tag.ATOMIC.REAL);
        static Logix.Tag Recipeid = new Logix.Tag("WEBSortRecipeid", Logix.Tag.ATOMIC.DINT);

        static Logix.Tag InventoryNewdata = new Logix.Tag("WEBSortTagDataReady", Logix.Tag.ATOMIC.BOOL);
        static Logix.Tag InventoryProductID = new Logix.Tag("WEBSortTagID", Logix.Tag.ATOMIC.DINT);
        
        static Logix.Tag NoSpareBays = new Logix.Tag("NoSpareBays", Logix.Tag.ATOMIC.BOOL);
        static Logix.Tag Volume = new Logix.Tag("Program:WEBSort.ShiftFBM", Logix.Tag.ATOMIC.REAL);
        static Logix.Tag Pieces = new Logix.Tag("Program:WEBSort.ShiftPieceCount", Logix.Tag.ATOMIC.DINT);
        static Logix.Tag Year = new Logix.Tag("WEBSortTimeTable[0]", Logix.Tag.ATOMIC.INT);
        static Logix.Tag Month = new Logix.Tag("WEBSortTimeTable[1]", Logix.Tag.ATOMIC.INT);
        static Logix.Tag Day = new Logix.Tag("WEBSortTimeTable[2]", Logix.Tag.ATOMIC.INT);
        static Logix.Tag Hour = new Logix.Tag("WEBSortTimeTable[3]", Logix.Tag.ATOMIC.INT);
        static Logix.Tag Minutes = new Logix.Tag("WEBSortTimeTable[4]", Logix.Tag.ATOMIC.INT);
        static Logix.Tag Seconds = new Logix.Tag("WEBSortTimeTable[5]", Logix.Tag.ATOMIC.INT);
        static Logix.Tag EncoderPosition = new Logix.Tag("EncoderPositionS", Logix.Tag.ATOMIC.DINT);
        static Logix.Tag EncoderActual = new Logix.Tag("SorterEncoder.ActualPosition", Logix.Tag.ATOMIC.REAL);
        static Logix.Tag SkipEncoderPosition = new Logix.Tag("SkipEncoderPosition", Logix.Tag.ATOMIC.DINT);
        static Logix.Tag SortEditTrigger = new Logix.Tag("Program:WEBSort.SortEditTrigger", Logix.Tag.ATOMIC.BOOL);
        static Logix.Tag BayEditTrigger = new Logix.Tag("Program:WEBSort.BayEditTrigger", Logix.Tag.ATOMIC.BOOL);
        static Logix.Tag PACSerialNumber = new Logix.Tag("Program:WEBSort.PAC", Logix.Tag.ATOMIC.DINT);

        
        public struct LUG
        {
            public Int16 FrameStart;
            public Int16 LugNum;
            public UInt32 TrackNum;
            public UInt16 pad2;
            public Int16 BayNum;
            public Int16 ProductID;
            public Int16 LengthID;
            public Int16 PLCGradeIDx;
            public Int16 PLCSpeciesIDx;
            public Int16 GradeId;
            public Int16 SpeciedsD;
            public Single ThickActual;
            public Single WidthActual;
            public Single LengthIn;
            public Single LengthOut;
            public Single Fence;
            public Single MoistureActual;
            public int MoistureID;
            public int Saws;
            public Byte NET;
            public Byte FET;
            public Byte CN2;
            public Byte GraderID;
            public int BayCount;
            public Single Volume;
            public int PieceCount;
            public int Flags;
            public int Devices;
            public Byte boolAck;
            public Byte pad1;
            public Int16 FrameEnd;
        };
        public struct BIN
        {
            public Int16 FrameStart;
            public Int16 BayNum;
            public Int16 Count;
            public Int16 SortXRef;
            //public Int16 LinkID;
            //public Int16 Pad;
            public Byte BinFlags0;
            public Byte BinFlags1;          
            public Int16 FrameEnd;
        };
        
        public unsafe struct STRING_TYPE
        {
          public int Len;
          public fixed byte Data[20];
        };
        public struct PRODUCT
        {
            public unsafe fixed uint Product[30];
        };
        public struct PRODUCTCHILD
        {
            public unsafe fixed uint ProductChild[3];
        };
        public struct BINREADWRITE
        {
            public STRING_TYPE Name;
            
            public int PkgSize;
            public Single Count;            
            public PRODUCT ProductArray;            
            public uint LengthMap;
            public uint MoistureMap;
            public int Stamps;
            public int Sprays;
            public Single PctFull;
            public Byte SecProdID;
            public Byte SecSize;
            public Byte SecCount;
            public Byte Pad;
            public unsafe fixed int StackerLath[10];
            public Int16 SortXRef;
            public Int16 BinFlags;
            
        };
        public struct SORTREADWRITE
        {
            public STRING_TYPE Name;

            public Int16 PkgSize;
            public Byte PkgsPerSort;
            public Byte OrderCount;
            public PRODUCT ProductArray;
            public uint LengthMap;
            public uint MoistureMap;
            public int Stamps;
            public int Sprays;
            public Byte SecProdID;
            public Byte SecSize;
            public Int16 Pad;
            public Int16 Zone1;
            public Int16 Zone2;
            public int SortFlags;
            

        };
        public struct TIMINGREADWRITE
        {
            public unsafe fixed int TimingData[10];

        };
        
        /// Encoded_Type (aka Abbreviated Data Type) a series 
        /// of bytes from the PLC that is used to reference 
        /// Data Types.  You'll need this value to write Structured Types
        public static byte[] Encoded_TypeCode = null;
        //static UInt16 UDT_TypeCode = 0;

        /// The Tag.Value class returns Structure Data Types as a 
        /// byte[].  We'll call this rawUDT to pass to DTEncoding class
        public static byte[] rawUDT = null;

        /// Boolean (bit) values in structured types are either packed in 8-bit or 32-bit values.
        /// The DTEncoding.ToBoolArray unpacks bits
        public static bool[] bit;
        public static bool[] bit1;
        public static bool[] bit2;
        
        //////////////////////////////////////////
        /// Instance of the NET.LOGIX DTEncoding class
        static DTEncoding udtEnc = new DTEncoding();
        static DTEncoding udtBinEnc = new DTEncoding();


        public static void ResetCounters()
        {
            try
            {
                MyPLC.ReadTag(lugindex);
            }
            catch 
            {
                UpdateRaptorCommLog("Error reading lugindex tag: " + MyPLC.ErrorString);
            }
            LugIndexPLC = Convert.ToInt16(lugindex.Value);
            LugIndex = LugIndexPLC;
            PLCCounter = LugIndex;
            WEBSortCounter = PLCCounter;
            oldLugIndexPLC = LugIndexPLC;
            oldWEBSortCounter = WEBSortCounter-1;

        }
        public static void ResetBinCounters()
        {
            try
            {
                MyPLCBin1.ReadTag(binindex);
            }
            catch
            {
                UpdateRaptorCommLog("Error reading binindex tag: " + MyPLCBin1.ErrorString);
            }
            BinIndexPLC = Convert.ToInt16(binindex.Value);
            BinIndex = BinIndexPLC;
            PLCBinCounter = BinIndex;
            WEBSortBinCounter = PLCBinCounter;
            oldBinIndexPLC = BinIndexPLC;
            oldWEBSortBinCounter = WEBSortBinCounter - 1;
            
        }
        public static void ResetStatusCounters()
        {
            try
            {
                MyPLCStatus.ReadTag(statusindex);
            }
            catch 
            {
                UpdateRaptorCommLog("Error reading statusindex tag: " + MyPLCStatus.ErrorString);
            }
            StatusIndexPLC = Convert.ToInt16(statusindex.Value);
            StatusIndex = StatusIndexPLC;
            PLCStatusCounter = StatusIndex;
            WEBSortStatusCounter = PLCStatusCounter;
            oldStatusIndexPLC = StatusIndexPLC;
            oldWEBSortStatusCounter = WEBSortStatusCounter - 1;

        }
        public static void ReadPLCSettings()
        {
            try{
                
                SqlCommand cmd = new SqlCommand("select * from RaptorCommSettings", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    MyPLC.IPAddress = reader["PLCIPAddress"].ToString();
                    MyPLC.Path = reader["PLCProcessorSlot"].ToString();
                    MyPLC.Timeout = int.Parse(reader["PLCTimeout"].ToString());
                    LugThreadScanRate = int.Parse(reader["LugScanRate"].ToString());
                }
                reader.Close();
                }
            catch(Exception ex){
                UpdateRaptorCommLog(ex.Message);
            }
            MyPollPLC.IPAddress = MyPLC.IPAddress;
            MyPollPLC.Path = MyPLC.Path;
            MyPollPLC.Timeout = MyPLC.Timeout;
            MyPLCState.IPAddress = MyPLC.IPAddress;
            MyPLCState.Path = MyPLC.Path;
            MyPLCState.Timeout = MyPLC.Timeout;
            MyPLCBin.IPAddress = MyPLC.IPAddress;
            MyPLCBin.Path = MyPLC.Path;
            MyPLCBin.Timeout = MyPLC.Timeout;
            MyPLCBin1.IPAddress = MyPLC.IPAddress;
            MyPLCBin1.Path = MyPLC.Path;
            MyPLCBin1.Timeout = MyPLC.Timeout;
            MyPLCSort.IPAddress = MyPLC.IPAddress;
            MyPLCSort.Path = MyPLC.Path;
            MyPLCSort.Timeout = MyPLC.Timeout;
            MyPLCProduct.IPAddress = MyPLC.IPAddress;
            MyPLCProduct.Path = MyPLC.Path;
            MyPLCProduct.Timeout = MyPLC.Timeout;
            MyPLCThickness.IPAddress = MyPLC.IPAddress;
            MyPLCThickness.Path = MyPLC.Path;
            MyPLCThickness.Timeout = MyPLC.Timeout;
            MyPLCWidth.IPAddress = MyPLC.IPAddress;
            MyPLCWidth.Path = MyPLC.Path;
            MyPLCWidth.Timeout = MyPLC.Timeout;
            MyPLCParameter.IPAddress = MyPLC.IPAddress;
            MyPLCParameter.Timeout = MyPLC.Timeout;
            MyPLCParameter.Path = MyPLC.Path;
            MyPLCLength.IPAddress = MyPLC.IPAddress;
            MyPLCLength.Path = MyPLC.Path;
            MyPLCLength.Timeout = MyPLC.Timeout;
            MyPLCMoisture.IPAddress = MyPLC.IPAddress;
            MyPLCMoisture.Path = MyPLC.Path;
            MyPLCMoisture.Timeout = MyPLC.Timeout;
            MyPLCGrade.IPAddress = MyPLC.IPAddress;
            MyPLCGrade.Path = MyPLC.Path;
            MyPLCGrade.Timeout = MyPLC.Timeout;
            MyPLCTiming.IPAddress = MyPLC.IPAddress;
            MyPLCTiming.Timeout = MyPLC.Timeout;
            MyPLCTiming.Path = MyPLC.Path;
            MyPLCDiagnostics.IPAddress = MyPLC.IPAddress;
            MyPLCDiagnostics.Path = MyPLC.Path;
            MyPLCDiagnostics.Timeout = MyPLC.Timeout;
            MyPLCDrives.IPAddress = MyPLC.IPAddress;
            MyPLCDrives.Path = MyPLC.Path;
            MyPLCDrives.Timeout = MyPLC.Timeout;
            MyPLCMisc.IPAddress = MyPLC.IPAddress;
            MyPLCMisc.Path = MyPLC.Path;
            MyPLCMisc.Timeout = MyPLC.Timeout;
            MyPLCStatus.IPAddress = MyPLC.IPAddress;
            MyPLCStatus.Path = MyPLC.Path;
            MyPLCStatus.Timeout = MyPLC.Timeout;
            MyPLCStatus1.IPAddress = MyPLC.IPAddress;
            MyPLCStatus1.Path = MyPLC.Path;
            MyPLCStatus1.Timeout = MyPLC.Timeout;

        }
       

        public static string GetConnectionString()
        {
            return ("Server=(local)\\SQLEXPRESS;Initial Catalog=RaptorWebSort;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=120");
        }

        public static void UpdateRaptorCommLog(string text)
        {
            try{
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                SqlCommand cmd = new SqlCommand("insert into RaptorCommLog select getdate(),'" + text + "'", connection);
                cmd.ExecuteNonQuery();
                SqlCommand cmd1 = new SqlCommand("delete from RaptorCommLog where id< (select max(id)-1000 from RaptorCommLog)", connection);
                cmd1.ExecuteNonQuery();
            }
            catch{}
        }

        static void peerMsg_Received(object sender, EventArgs e)
        {
            try
            {
                //////////////////////////////////////////////////
                // since tag_changed is being called from the plcUpdate
                // thread, we need to ceated a delegate to handle the UI
                //if (InvokeRequired)
                    //Invoke(new MsgReceivedDelegate(MsgReceived), new object[] { (MessageEventArgs)e });
                //else
                    MsgReceived((MessageEventArgs)e);
            }
            catch (System.Exception ex)
            {
                UpdateRaptorCommLog(ex.Message);
            }
            //throw new Exception("The method or operation is not implemented.");
        }

        private unsafe static void MsgReceived(MessageEventArgs e)
        {
            DTEncoding udtBinEnc = new DTEncoding();
            try{
                // convert unsolicited message value to STRUCT_A UDT
                BINREADWRITE binreadwrite = (BINREADWRITE)udtBinEnc.ToType((byte[])peerMsg.Value, typeof(BINREADWRITE));
                ASCIIEncoding enc = new ASCIIEncoding();
                Byte[] asciidata = new byte[20];
                for (int i = 0; i < binreadwrite.Name.Len; i++)
                    asciidata[i] = binreadwrite.Name.Data[i];
                String Name = enc.GetString(asciidata, 0, binreadwrite.Name.Len);
        
                //UpdateRaptorCommLog("Unsolicited message received from " + peerMsg.IPSender + ", " + peerMsg.ItemName + ", " + peerMsg.Value.ToString());
                UpdateRaptorCommLog("Unsolicited message received from " + peerMsg.IPSender + ", " + peerMsg.ItemName + ", " + Name);
            }
            catch(Exception ex){
                UpdateRaptorCommLog(ex.Message);
            }
            
            /*ListViewItem msgItem = listView.Items.Add(peerMsg.IPSender);
            msgItem.SubItems.Add(peerMsg.Timestamp.ToString());
            msgItem.SubItems.Add(peerMsg.Length.ToString());
            msgItem.SubItems.Add(peerMsg.ItemName);
            msgItem.SubItems.Add(peerMsg.NetType.ToString());
            msgItem.SubItems.Add(ItemValues());*/
        }
        public static void InitializeConnectionsAndThreads()
        {
            UpdateRaptorCommLog("Opening Connections to PLC");
            while (MyPLC.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Main: " + MyPLC.ErrorString);
                ReadPLCSettings();
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCState.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("PLC State: " + MyPLCState.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCBin.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Bin: " + MyPLCBin.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCBin1.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Bin Poll: " + MyPLCBin1.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCSort.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Sort: " + MyPLCSort.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPollPLC.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Poll: " + MyPollPLC.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCProduct.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Product: " + MyPLCProduct.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCThickness.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Thickness: " + MyPLCThickness.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCWidth.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Width: " + MyPLCWidth.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCLength.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Length: " + MyPLCLength.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCMoisture.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Moisture: " + MyPLCMoisture.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCGrade.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Grade: " + MyPLCGrade.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCTiming.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Timing: " + MyPLCTiming.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCDiagnostics.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Diagnostics: " + MyPLCDiagnostics.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCDrives.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Drives: " + MyPLCDrives.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCMisc.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Misc: " + MyPLCMisc.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCStatus.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Status: " + MyPLCStatus.ErrorString);
                //return;
            }
            Thread.Sleep(50);
            while (MyPLCStatus1.Connect() != ResultCode.E_SUCCESS)
            {
                UpdateRaptorCommLog("Status Variable: " + MyPLCStatus1.ErrorString);
                //return;
            }
            UpdateRaptorCommLog("Connection to PLC Established Successfully!");
            //start the various threads necessary
            UpdateRaptorCommLog("Starting Threads");
            if (null == PLCStateThread)
            {
                PLCStateThread = new Thread(ReadPLCState);
                PLCStateThread.Start();
            }
            if (null == PLCPollThread)
            {
                PLCPollThread = new Thread(PLCPoll);
                PLCPollThread.Priority = ThreadPriority.Highest;
                PLCPollThread.Start();
            }
            if (null == LugThread)
            {
                LugThread = new Thread(ReadLugs);
                LugThread.Priority = ThreadPriority.Highest;
                LugThread.Start();
            }
            if (null == BinDataFromPLC)
            {
                BinDataFromPLC = new Thread(BinDataSentFromPLC);
                BinDataFromPLC.Start();
            }
            if (null == StatusDataFromPLC)
            {
                StatusDataFromPLC = new Thread(StatusDataSentFromPLC);
                //StatusDataFromPLC.Priority = ThreadPriority.Highest;
                StatusDataFromPLC.Start();
            }
            if (null == StatusDataVariableFromPLC)
            {
                StatusDataVariableFromPLC = new Thread(StatusDataVariableSentFromPLC);
                StatusDataVariableFromPLC.Priority = ThreadPriority.Highest;
                StatusDataVariableFromPLC.Start();
            }
            if (null == BinDataToPLC)
            {
                BinDataToPLC = new Thread(BinDataSentToPLC);
                BinDataToPLC.Start();
            }
            if (null == SortDataToPLC)
            {
                SortDataToPLC = new Thread(SortDataSentToPLC);
                SortDataToPLC.Start();
            }
            if (null == ProductDataToPLC)
            {
                ProductDataToPLC = new Thread(ProductDataSentToPLC);
                ProductDataToPLC.Start();
            }
            if (null == ThicknessDataToPLC)
            {
                ThicknessDataToPLC = new Thread(ThicknessDataSentToPLC);
                ThicknessDataToPLC.Start();
            }
            if (null == WidthDataToPLC)
            {
                WidthDataToPLC = new Thread(WidthDataSentToPLC);
                WidthDataToPLC.Start();
            }
            if (null == LengthDataToPLC)
            {
                LengthDataToPLC = new Thread(LengthDataSentToPLC);
                LengthDataToPLC.Start();
            }
            if (null == MoistureDataToPLC)
            {
                MoistureDataToPLC = new Thread(MoistureDataSentToPLC);
                MoistureDataToPLC.Start();
            }
            if (null == GradeDataToPLC)
            {
                GradeDataToPLC = new Thread(GradeDataSentToPLC);
                GradeDataToPLC.Start();
            }
            if (null == TimingDataToPLC)
            {
                TimingDataToPLC = new Thread(TimingDataSentToPLC);
                TimingDataToPLC.Start();
            }
            if (null == DiagnosticsDataToPLC)
            {
                DiagnosticsDataToPLC = new Thread(DiagnosticDataSentToPLC);
                DiagnosticsDataToPLC.Start();
            }
            if (null == DrivesDataToPLC)
            {
                DrivesDataToPLC = new Thread(DrivesDataSentToPLC);
                DrivesDataToPLC.Start();
            }
            if (null == ParameterDataToPLC)
            {
                ParameterDataToPLC = new Thread(ParameterDataSentToPLC);
                ParameterDataToPLC.Start();
            }
            if (null == DataRequestsThread)
            {
                DataRequestsThread = new Thread(ReadDataRequests);
                DataRequestsThread.Start();
            }
        }
        public static void Init()
        {
            connectionString = GetConnectionString();
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            UpdateRaptorCommLog("RaptorComm Service Start Request.");
            
            peerMsg.Received += new EventHandler(peerMsg_Received);
            peerMsg.IPAddressNIC = "";
            peerMsg.Protocol = PeerMessage.MSGPROTOCOL.CIP;
            peerMsg.Listen();
            ReadPLCSettings();
            
            InitializeConnectionsAndThreads();
            
        }

        public static void ReadDataRequests()
        {
            //DataRequests: 
            //1 = Bay
            //2 = Sort
            //4 = Product
            //8 = Grade
            //16 = Thickness
            //32 = Width
            //64 = Length
            //128 = Diagnostic
            //256 = Timing
            //512 = Moisture
            //1024 = Drives
            //2048 = PET Length
            //4096 = Grader Test
            //16384 = Clear volume and piece count
            //32768 = Run Thick and Width
            //65536 = End of Shift
            //131072 = Sort Edit Trigger
            //262144 = Bay Edit Trigger
            //524288 = Read Shift Piece Production from PLC
            //1048576 = Parameters
            //2097152 = Cut in Two Override
            //33554432 = Jag Priority bay list
            while (true)
            {
                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    //if any of the threads happen to die, restart them                
                    if (null == PLCStateThread)
                    {
                        UpdateRaptorCommLog("Restarting Lug Thread");
                        PLCStateThread = new Thread(ReadPLCState);
                        PLCStateThread.Start();
                    }
                    if (null == PLCPollThread)
                    {
                        UpdateRaptorCommLog("Restarting Poll Thread");
                        PLCPollThread = new Thread(PLCPoll);
                        PLCPollThread.Priority = ThreadPriority.Highest;
                        PLCPollThread.Start();
                    }
                    if (null == LugThread)
                    {
                        UpdateRaptorCommLog("Restarting PLC State Thread");
                        LugThread = new Thread(ReadLugs);
                        LugThread.Priority = ThreadPriority.Highest;
                        LugThread.Start();
                    }
                    if (null == BinDataFromPLC)
                    {
                        UpdateRaptorCommLog("Restarting Bin Thread");
                        BinDataFromPLC = new Thread(BinDataSentFromPLC);
                        BinDataFromPLC.Start();
                    }
                    if (null == StatusDataFromPLC)
                    {
                        UpdateRaptorCommLog("Restarting Status Thread");
                        StatusDataFromPLC = new Thread(StatusDataSentFromPLC);
                        StatusDataFromPLC.Start();
                    }
                    if (null == StatusDataVariableFromPLC)
                    {
                        UpdateRaptorCommLog("Restarting Status Variable Thread");
                        StatusDataVariableFromPLC = new Thread(StatusDataVariableSentFromPLC);
                        StatusDataVariableFromPLC.Priority = ThreadPriority.Highest;
                        StatusDataVariableFromPLC.Start();
                    }
                    if (null == BinDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Bin Read/Write Thread");
                        BinDataToPLC = new Thread(BinDataSentToPLC);
                        BinDataToPLC.Start();
                    }
                    if (null == SortDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Sort Thread");
                        SortDataToPLC = new Thread(SortDataSentToPLC);
                        SortDataToPLC.Start();
                    }
                    if (null == ProductDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Product Thread");
                        ProductDataToPLC = new Thread(ProductDataSentToPLC);
                        ProductDataToPLC.Start();
                    }
                    if (null == ThicknessDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Thickness Thread");
                        ThicknessDataToPLC = new Thread(ThicknessDataSentToPLC);
                        ThicknessDataToPLC.Start();
                    }
                    if (null == WidthDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Width Thread");
                        WidthDataToPLC = new Thread(WidthDataSentToPLC);
                        WidthDataToPLC.Start();
                    }
                    if (null == LengthDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Length Thread");
                        LengthDataToPLC = new Thread(LengthDataSentToPLC);
                        LengthDataToPLC.Start();
                    }
                    if (null == MoistureDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Moisture Thread");
                        MoistureDataToPLC = new Thread(MoistureDataSentToPLC);
                        MoistureDataToPLC.Start();
                    }
                    if (null == GradeDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Grade Thread");
                        GradeDataToPLC = new Thread(GradeDataSentToPLC);
                        GradeDataToPLC.Start();
                    }
                    if (null == TimingDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Timing Thread");
                        TimingDataToPLC = new Thread(TimingDataSentToPLC);
                        TimingDataToPLC.Start();
                    }
                    if (null == DiagnosticsDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Diagnostics Thread");
                        DiagnosticsDataToPLC = new Thread(DiagnosticDataSentToPLC);
                        DiagnosticsDataToPLC.Start();
                    }
                    if (null == DrivesDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Drives Thread");
                        DrivesDataToPLC = new Thread(DrivesDataSentToPLC);
                        DrivesDataToPLC.Start();
                    }
                    if (null == ParameterDataToPLC)
                    {
                        UpdateRaptorCommLog("Restarting Parameter Thread");
                        ParameterDataToPLC = new Thread(ParameterDataSentToPLC);
                        ParameterDataToPLC.Start();
                    }
                    
                    if (!MyPLCMisc.IsConnected)
                    {
                        while (MyPLCMisc.Connect() != ResultCode.E_SUCCESS)
                        {
                            UpdateRaptorCommLog("Main: " + MyPLCMisc.ErrorString);
                            //return;
                        }
                        UpdateRaptorCommLog("Main Connection to PLC Re-established Successfully!");
                    }
                    SqlCommand cmd = new SqlCommand("select DataRequests from RaptorCommSettings", connection);
                    SqlDataReader reader = cmd.ExecuteReader(); 
                    reader.Read();
                    DataRequests = int.Parse(reader["DataRequests"].ToString());  
                    reader.Close();
                    if ((DataRequests & 16384) == 16384)  //clear volume and piece count in the PLC
                    {
                        Volume.Value = "0";
                        Pieces.Value = "0";
                        if (MyPLCMisc.IsConnected)
                        {
                            if (MyPLCMisc.WriteTag(Volume) != Logix.ResultCode.E_SUCCESS)
                            {
                                UpdateRaptorCommLog("Volume Tag Write: " + MyPLCMisc.ErrorString);
                                Thread.Sleep(10000);
                            }
                            if (MyPLCMisc.WriteTag(Pieces) != Logix.ResultCode.E_SUCCESS)
                            {
                                UpdateRaptorCommLog("Pieces Tag Write: " + MyPLCMisc.ErrorString);
                                Thread.Sleep(10000);
                            }                    
                        }
                        SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-16384 where (datarequests & 16384)=16384", connection);
                        cmd1.ExecuteNonQuery();
                    }
                    if ((DataRequests & 65536) == 65536)  //end of shift, send time, and then end of shift bit
                    {
                        if (MyPLCMisc.IsConnected)
                        {                           
                            DateTimeUDTGroup.Tags.Clear();
                            DateTimeUDTGroup.Clear();
                            DateTimeUDTGroup.AddTag(Year);
                            DateTimeUDTGroup.AddTag(Month);
                            DateTimeUDTGroup.AddTag(Day);
                            DateTimeUDTGroup.AddTag(Hour);
                            DateTimeUDTGroup.AddTag(Minutes);
                            DateTimeUDTGroup.AddTag(Seconds);

                            Year.Value = System.DateTime.Now.Year.ToString();
                            Month.Value = System.DateTime.Now.Month.ToString();
                            Day.Value = System.DateTime.Now.Day.ToString();
                            Hour.Value = System.DateTime.Now.Hour.ToString();
                            Minutes.Value = System.DateTime.Now.Minute.ToString();
                            Seconds.Value = System.DateTime.Now.Second.ToString();
                            MyPLCMisc.GroupWrite(DateTimeUDTGroup);
                            Thread.Sleep(1000);

                            if (ResultCode.QUAL_GOOD != Year.QualityCode)
                            {
                                UpdateRaptorCommLog("Date/Time Write Error: " + Year.ErrorString);
                                Thread.Sleep(10000);
                            }
                            
                            //send end of shift flag
                            ShiftReset.Value = true;
                            if (MyPLCMisc.WriteTag(ShiftReset) != Logix.ResultCode.E_SUCCESS)
                            {
                                UpdateRaptorCommLog("Reset Tag Write: " + MyPLCMisc.ErrorString);
                                Thread.Sleep(10000);
                            }
                        }
                        SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-65536 where (datarequests & 65536)=65536", connection);
                        cmd1.ExecuteNonQuery();
                    }
                    if ((DataRequests & 32768) == 32768)  //run thick and width
                    {
                        SqlCommand cmdtw = new SqlCommand("selectRunParameters", connection);
                        SqlDataReader readertw = cmdtw.ExecuteReader();
                    
                        readertw.Read();
                        
                        RunThick.Value = readertw["RunThickness"].ToString();
                        RunWidth.Value = readertw["RunWidth"].ToString();
                        Recipeid.Value = readertw["Recipeid"].ToString();
                        if (MyPLCMisc.IsConnected)
                        {
                            if (MyPLCMisc.WriteTag(RunThick) != Logix.ResultCode.E_SUCCESS)
                            {
                                UpdateRaptorCommLog("RunThick Tag Write: " + MyPLCMisc.ErrorString);
                                Thread.Sleep(10000);
                            }
                            if (MyPLCMisc.WriteTag(RunWidth) != Logix.ResultCode.E_SUCCESS)
                            {
                                UpdateRaptorCommLog("RunWidth Tag Write: " + MyPLCMisc.ErrorString);
                                Thread.Sleep(10000);
                            }
                            if (MyPLCMisc.WriteTag(Recipeid) != Logix.ResultCode.E_SUCCESS)
                            {
                                UpdateRaptorCommLog("Recipeid Tag Write: " + MyPLCMisc.ErrorString);
                                Thread.Sleep(10000);
                            }
                            
                        }
                        readertw.Close();
                        SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-32768 where (datarequests & 32768)=32768", connection);
                        cmd1.ExecuteNonQuery();
                    }
                    if ((DataRequests & 131072) == 131072)  //sort edit trigger
                    {
                        if (MyPLCMisc.IsConnected)
                        {
                            SortEditTrigger.Value = true;
                            if (MyPLCMisc.WriteTag(SortEditTrigger) != Logix.ResultCode.E_SUCCESS)
                            {
                                UpdateRaptorCommLog("Sort Edit Trigger: " + MyPLCMisc.ErrorString);
                                Thread.Sleep(10000);
                            }
                            
                        }
                        SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-131072 where (datarequests & 131072)=131072", connection);
                        cmd1.ExecuteNonQuery();
                    }
                    if ((DataRequests & 262144) == 262144)  //bay edit trigger
                    {
                        if (MyPLCMisc.IsConnected)
                        {
                            
                            BayEditTrigger.Value = true;
                            if (MyPLCMisc.WriteTag(BayEditTrigger) != Logix.ResultCode.E_SUCCESS)
                            {
                                UpdateRaptorCommLog("Bay Edit Trigger: " + MyPLCMisc.ErrorString);
                                Thread.Sleep(10000);
                            }
                        }
                        SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-262144 where (datarequests & 262144)=262144", connection);
                        cmd1.ExecuteNonQuery();
                    }
                    if ((DataRequests & 524288) == 524288)  //read piece production
                    {
                        if (MyPLCMisc.IsConnected)
                        {
                            SqlCommand command0 = new SqlCommand("truncate table ProductionBoards", connection);
                            command0.ExecuteNonQuery();
                            for (int product = 1; product <= 95; product++)
                                for (int length = 1; length <= 19;length++ )
                                {
                                    Tag myTag = new Tag("Program:WEBSort.ShiftProductCount[" + product.ToString() + "].Count[" + length.ToString() + "]", Logix.Tag.ATOMIC.DINT);
                                    if (MyPLCMisc.ReadTag(myTag) != Logix.ResultCode.E_SUCCESS)
                                    {
                                        UpdateRaptorCommLog("Piece Count Read: " + MyPLCMisc.ErrorString);
                                        Thread.Sleep(10000);
                                    }
                                    else  //store the piece count data
                                    {
                                        SqlCommand command = new SqlCommand("execute UpdatePLCProductionData " + product.ToString() + "," + length.ToString() + "," + myTag.Value.ToString(), connection);
                                        command.ExecuteNonQuery();
                                    }
                                }
                        }
                        SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-524288 where (datarequests & 524288)=524288", connection);
                        cmd1.ExecuteNonQuery();
                    }
                    if ((DataRequests & 2097152) == 2097152)  //cut in two overrides
                    {
                        int i = 0;
                        Tag myTag, myTag1, myTag2, myTag3, myTag4;
                        if (MyPLCMisc.IsConnected)
                        {
                            SqlCommand cmdre = new SqlCommand("selectCutInTwoOverrides", connection);
                            SqlDataReader readerre = cmdre.ExecuteReader();

                            while (readerre.Read())
                            {
                                myTag = new Tag("websortCN2Override[" + readerre["gradeid"].ToString() + "].Parent", Logix.Tag.ATOMIC.DINT);
                                myTag.Value = readerre["parent"].ToString();
                                myTag1 = new Tag("websortCN2Override[" + readerre["gradeid"].ToString() + "].Child", Logix.Tag.ATOMIC.DINT);
                                myTag1.Value = readerre["child"].ToString();
                                myTag2 = new Tag("websortCN2Override[" + readerre["gradeid"].ToString() + "].Length", Logix.Tag.ATOMIC.DINT);
                                myTag2.Value = readerre["lengthnominal"].ToString();
                                myTag3 = new Tag("websortCN2Override[" + readerre["gradeid"].ToString() + "].CN2", Logix.Tag.ATOMIC.DINT);
                                myTag3.Value = readerre["CN2"].ToString();
                                myTag4 = new Tag("websortCN2Override[" + readerre["gradeid"].ToString() + "].Order", Logix.Tag.ATOMIC.DINT);
                                myTag4.Value = readerre["Orders"].ToString();

                                MiscUDTGroup.Tags.Clear();
                                MiscUDTGroup.Clear();
                                MiscUDTGroup.AddTag(myTag);
                                MiscUDTGroup.AddTag(myTag1);
                                MiscUDTGroup.AddTag(myTag2);
                                MiscUDTGroup.AddTag(myTag3);
                                MiscUDTGroup.AddTag(myTag4);
                                if (MyPLCMisc.GroupWrite(MiscUDTGroup) != Logix.ResultCode.E_SUCCESS)
                                {
                                    UpdateRaptorCommLog("Cut In Two Overrides: " + MyPLCMisc.ErrorString);
                                    Thread.Sleep(1000);
                                    break;
                                }
                                i++;
                            }
                            readerre.Close();

                        }
                        SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-2097152 where (datarequests & 2097152)=2097152", connection);
                        cmd1.ExecuteNonQuery();
                    }
                    if ((DataRequests & 33554432) == 33554432)  //jag priority list
                    {
                        int i = 1;
                        Tag myTag;
                        if (MyPLCMisc.IsConnected)
                        {
                            SqlCommand cmdprime = new SqlCommand("select * from jagpriorities", connection);
                            SqlDataReader readerprime = cmdprime.ExecuteReader();

                            while (readerprime.Read())
                            {
                                myTag = new Tag("Program:WEBSort.JagPriority[" + i.ToString() + "]", Logix.Tag.ATOMIC.DINT);
                                myTag.Value = readerprime["priority"].ToString();

                                if (MyPLCMisc.WriteTag(myTag) != Logix.ResultCode.E_SUCCESS)
                                {
                                    UpdateRaptorCommLog("Jag Priorities: " + MyPLCMisc.ErrorString);
                                    Thread.Sleep(1000);
                                    break;
                                }
                                i++;
                            }
                            readerprime.Close();
                           
                        }
                        SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set datarequests = datarequests-33554432 where (datarequests & 33554432)=33554432", connection);
                        cmd1.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    UpdateRaptorCommLog(ex.Message);
                }

                Thread.Sleep(DataRequestsScanRate);
            }
        }
        public static void ReadPLCState()
        {
            while (true)
            {
                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    if (!MyPLCState.IsConnected)
                    {
                        while (MyPLCState.Connect() != ResultCode.E_SUCCESS)
                        {
                            UpdateRaptorCommLog("PLC State: " + MyPLCState.ErrorString);
                            //return;
                        }
                        UpdateRaptorCommLog("PLC State Connection to PLC Re-established Successfully!");
                    }
                    
                    if (MyPLCState.IsConnected)
                    {
                        if (Logix.ResultCode.E_SUCCESS == MyPLCState.ReadTag(cpuState))
                        {
                            stateCPU = cpuState.Value as Array;
                            SqlCommand command = new SqlCommand("update RaptorCommSettings set PLCLED = " + stateCPU.GetValue(0).ToString() + ",PLCFault=" + stateCPU.GetValue(1).ToString() + ",PLCKeySwitch=" + stateCPU.GetValue(2).ToString(), connection);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    UpdateRaptorCommLog("Error reading PLCState: " + MyPLCState.ErrorString);
                }               
                
                Thread.Sleep(PLCStateScanRate);
            }
        }

        public static void StatusDataSentFromPLC()
        {
            // initialize counters
            ResetStatusCounters();

            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCStatus.IsConnected)
                {
                    while (MyPLCStatus.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Status: " + MyPLCStatus.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Status Connection to PLC Re-established Successfully!");
                }
                Tag myTag = new Tag("SystemStatus[0]", Logix.Tag.ATOMIC.DINT, 15);                                
                ///////////////////////////////////////
                // update the group
                try
                {
                    if (MyPLCStatus.IsConnected && oldWEBSortStatusCounter != WEBSortStatusCounter)
                    {
                        //UpdateRaptorCommLog(WEBSortStatusCounter.ToString() + "," + oldWEBSortStatusCounter.ToString());
                        if (MyPLCStatus.ReadTag(myTag) != Logix.ResultCode.E_SUCCESS)
                        {
                            UpdateRaptorCommLog("Error reading Status Tag: " + MyPLCStatus.ErrorString);
                            Thread.Sleep(10000);
                        }
                        else
                        {                          
                            //process data in database
                            Array theData = (Array)myTag.Value;
                            try
                            {
                                SqlCommand command = new SqlCommand("execute UpdateStatusData " + Convert.ToString(theData.GetValue(0)) + "," + Convert.ToString(theData.GetValue(1)) + "," + Convert.ToString(theData.GetValue(2)) + "," + Convert.ToString(theData.GetValue(3)) + "," + Convert.ToString(theData.GetValue(4)) + "," + Convert.ToString(theData.GetValue(5)) + "," + Convert.ToString(theData.GetValue(6)) + "," + Convert.ToString(theData.GetValue(7)) + "," + Convert.ToString(theData.GetValue(8)) + "," + Convert.ToString(theData.GetValue(9)) + "," + Convert.ToString(theData.GetValue(10)) + "," + Convert.ToString(theData.GetValue(11)) + "," + Convert.ToString(theData.GetValue(12)) + "," + Convert.ToString(theData.GetValue(13)) + "," + Convert.ToString(theData.GetValue(14)), connection);
                                command.ExecuteNonQuery();
                                //Thread.Sleep(20);
                            }
                            catch(Exception ex){
                                UpdateRaptorCommLog(ex.Message);
                            }

                            oldWEBSortStatusCounter = WEBSortStatusCounter;                 
                        }
                    }

                }
                catch (Exception ex)
                {
                    UpdateRaptorCommLog("Error reading Status Tag: " + ex.Message);
                    //return;
                }

                try
                {
                    if (MyPLCStatus.IsConnected)
                        MyPLCStatus.ReadTag(statusindex);
                    //UpdateRaptorCommLog(Statusindex.Value.ToString());
                }
                catch 
                {
                    UpdateRaptorCommLog("Error reading statusindex tag: " + MyPLCStatus.ErrorString);
                }

                StatusIndex = Convert.ToInt16(statusindex.Value);

                if (StatusIndex > WEBSortStatusCounter)  //PLC had advanced the Status index pointer
                {
                    if (StatusIndex - WEBSortStatusCounter == 1)
                        WEBSortStatusCounter = StatusIndex;
                    else
                    {
                        WEBSortStatusCounter++;
                        if (WEBSortStatusCounter == 20)
                            WEBSortStatusCounter = 0;
                    }
                }
                else if (StatusIndex < WEBSortStatusCounter)  //StatusIndex has rolled over
                {
                    if (StatusIndex - (WEBSortStatusCounter - 20) == 1)
                        WEBSortStatusCounter = StatusIndex;
                    else
                    {
                        WEBSortStatusCounter++;
                        if (WEBSortStatusCounter == 20)
                            WEBSortStatusCounter = 0;
                    }
                }                
                Thread.Sleep(StatusDataFromPLCScanRate);
            }
        }

        public static void StatusDataVariableSentFromPLC()
        {
            int Succeeded;
            // initialize counters
            ResetStatusCounters();

            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCStatus1.IsConnected)
                {
                    while (MyPLCStatus1.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Status: " + MyPLCStatus1.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Status Variable Connection to PLC Re-established Successfully!");
                }

                try
                {
                    SqlCommand cmd = new SqlCommand("select * from DataRequestsAlarmData with(nolock) where processed = 0 order by id", connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        //data request exists, process whether this a read or write request
                        Succeeded = 1;
                        int AlarmID = int.Parse(reader["AlarmID"].ToString());

                        Tag AlarmData = new Tag("StatusMsg[" + AlarmID.ToString() + "]", Logix.Tag.ATOMIC.DINT);

                        try
                        {
                            if (MyPLCStatus1.IsConnected)
                                MyPLCStatus1.ReadTag(AlarmData);
                        }
                        catch
                        {
                            UpdateRaptorCommLog("Error reading Status Msg Tag: " + MyPLCStatus1.ErrorString);
                        }
                        if (ResultCode.QUAL_GOOD == AlarmData.QualityCode)
                        {
                            //write results into the database
                            try
                            {
                                SqlCommand cmd0 = new SqlCommand("update datarequestsAlarmData set AlarmData=" + AlarmData.Value.ToString() + " where id=" + reader["id"].ToString(), connection);
                                cmd0.ExecuteNonQuery();
                                SqlCommand cmd1 = new SqlCommand("update AlarmSettings set Data = " + AlarmData.Value.ToString() + " where AlarmID=" + reader["Alarmid"].ToString(), connection);
                                cmd1.ExecuteNonQuery();
                                //SqlCommand cmd2 = new SqlCommand("insert into test1 select getdate(),'hello'", connection);
                                //cmd2.ExecuteNonQuery();

                            }
                            catch (Exception ex)
                            {
                                UpdateRaptorCommLog("Error updating Status Msg data in database: " + ex.Message);
                            }
                        }
                        else
                        {
                            UpdateRaptorCommLog("Status Msg Tag Read: " + AlarmData.ErrorString);
                            Succeeded = 0;
                            Thread.Sleep(10000);
                        }

                        //mark data request as processed
                        if (Succeeded == 1)
                        {
                            SqlCommand cmd11 = new SqlCommand("update datarequestsAlarmData set processed = 1 where id=" + reader["id"].ToString(), connection);
                            cmd11.ExecuteNonQuery();
                        }
                        SqlCommand cmd12 = new SqlCommand("delete from datarequestsAlarmData where id <= (select max(id)-100 from datarequestsAlarmData)", connection);
                        cmd12.ExecuteNonQuery();


                    }
                    reader.Close();
                }
                catch { }
                
                Thread.Sleep(StatusDataVariableFromPLCScanRate);
            }
        }
        
        public static void ReadLugs()
        {
            // initialize counters
            ResetCounters();

            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLC.IsConnected)
                {
                    while (MyPLC.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("PLC State: " + MyPLC.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Lug Connection to PLC Re-established Successfully!");
                }
                
                Tag myUDT = new Tag("WEBSortLugTable[" + WEBSortCounter + "]",Logix.Tag.ATOMIC.OBJECT);
                Tag Ack = new Tag("WEBSortLugTable[" + WEBSortCounter + "].Acknowledge", Logix.Tag.ATOMIC.BOOL);
                /*Tag FrameStart = new Tag("WEBSortLugTable[" + LugIndex + "].FrameStart", Logix.Tag.ATOMIC.INT);
                Tag FrameEnd = new Tag("WEBSortLugTable[" + LugIndex + "].FrameEnd", Logix.Tag.ATOMIC.INT);
                Tag Ack = new Tag("WEBSortLugTable[" + LugIndex + "].Acknowledge", Logix.Tag.ATOMIC.BOOL);
                Tag LugNum = new Tag("WEBSortLugTable[" + LugIndex + "].LugNum", Logix.Tag.ATOMIC.INT);
                Tag TrackNum = new Tag("WEBSortLugTable[" + LugIndex + "].TrackNum", Logix.Tag.ATOMIC.INT);
                Tag BayNum = new Tag("WEBSortLugTable[" + LugIndex + "].BayNum", Logix.Tag.ATOMIC.INT);
                Tag ProductID = new Tag("WEBSortLugTable[" + LugIndex + "].ProductID", Logix.Tag.ATOMIC.INT);
                Tag LengthID = new Tag("WEBSortLugTable[" + LugIndex + "].LengthID", Logix.Tag.ATOMIC.INT);
                Tag ThickActual = new Tag("WEBSortLugTable[" + LugIndex + "].ThickActual", Logix.Tag.ATOMIC.REAL);
                Tag WidthActual = new Tag("WEBSortLugTable[" + LugIndex + "].WidthActual", Logix.Tag.ATOMIC.REAL);
                Tag LengthIn = new Tag("WEBSortLugTable[" + LugIndex + "].LengthIn", Logix.Tag.ATOMIC.REAL);
                Tag Fence = new Tag("WEBSortLugTable[" + LugIndex + "].Fence", Logix.Tag.ATOMIC.REAL);
                Tag Saws = new Tag("WEBSortLugTable[" + LugIndex + "].Saws", Logix.Tag.ATOMIC.DINT);
                Tag NET = new Tag("WEBSortLugTable[" + LugIndex + "].NET", Logix.Tag.ATOMIC.SINT);
                Tag FET = new Tag("WEBSortLugTable[" + LugIndex + "].FET", Logix.Tag.ATOMIC.SINT);
                Tag CN2 = new Tag("WEBSortLugTable[" + LugIndex + "].CN2", Logix.Tag.ATOMIC.SINT);
                Tag BayCount = new Tag("WEBSortLugTable[" + LugIndex + "].BayCount", Logix.Tag.ATOMIC.DINT);
                Tag Volume = new Tag("WEBSortLugTable[" + LugIndex + "].Volume", Logix.Tag.ATOMIC.REAL);
                Tag PieceCount = new Tag("WEBSortLugTable[" + LugIndex + "].PieceCount", Logix.Tag.ATOMIC.DINT);
                Tag Flags = new Tag("WEBSortLugTable[" + LugIndex + "].Flags", Logix.Tag.ATOMIC.DINT);
                Tag Devices = new Tag("WEBSortLugTable[" + LugIndex + "].Devices", Logix.Tag.ATOMIC.DINT);

                LugUDTGroup.Tags.Clear();
                LugUDTGroup.Clear();
                LugUDTGroup.AddTag(FrameStart);
                LugUDTGroup.AddTag(LugNum);
                LugUDTGroup.AddTag(TrackNum);
                LugUDTGroup.AddTag(BayNum);
                LugUDTGroup.AddTag(ProductID);
                LugUDTGroup.AddTag(LengthID);
                LugUDTGroup.AddTag(ThickActual);
                LugUDTGroup.AddTag(WidthActual);
                LugUDTGroup.AddTag(LengthIn);
                LugUDTGroup.AddTag(Fence);
                LugUDTGroup.AddTag(Saws);
                LugUDTGroup.AddTag(NET);
                LugUDTGroup.AddTag(FET);
                LugUDTGroup.AddTag(CN2);
                LugUDTGroup.AddTag(BayCount);
                LugUDTGroup.AddTag(Volume);
                LugUDTGroup.AddTag(PieceCount);
                LugUDTGroup.AddTag(Flags);
                LugUDTGroup.AddTag(Devices);
                LugUDTGroup.AddTag(Ack);
                LugUDTGroup.AddTag(FrameEnd);*/
                ///////////////////////////////////////
                // update the group
                try
                {
                    if (MyPLC.IsConnected && WEBSortCounter != oldWEBSortCounter)
                    {
                        if (MyPLC.ReadTag(myUDT) != Logix.ResultCode.E_SUCCESS)
                        {
                            UpdateRaptorCommLog("Error reading Lug UDT: " + MyPLC.ErrorString);
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            LUG lug = (LUG)udtEnc.ToType(myUDT, typeof(LUG));
                            
                            // the individual bit elements
                            bit = udtEnc.ToBoolArray(lug.boolAck);
                            //check PLC for new lug data
                            //if (WEBSortCounter <= PLCCounter)
                                if (Convert.ToInt16(lug.FrameStart) == Convert.ToInt16(lug.FrameEnd))
                                    if (Convert.ToInt16(lug.FrameStart) != LastFrameStart)
                                        if (bit[0] == false)
                                        {
                                            //UpdateRaptorCommLog(lug.BayCount.ToString() + "," + lug.Volume.ToString() + "," + lug.PieceCount.ToString() + "," + lug.Flags.ToString());
                                            //store data in database
                                            SqlCommand command = new SqlCommand("execute UpdateLugData " + lug.FrameStart.ToString() + "," + lug.LugNum.ToString() + "," + lug.TrackNum.ToString() + "," + lug.BayNum.ToString() + "," + lug.ProductID.ToString() + "," + lug.LengthID.ToString() + "," + lug.PLCGradeIDx.ToString() + "," + lug.ThickActual.ToString() + "," + lug.WidthActual.ToString() + "," + lug.LengthIn.ToString() + "," + lug.Fence.ToString() + "," + lug.GraderID + "," + lug.Saws.ToString() + "," + lug.NET.ToString() + "," + lug.FET.ToString() + "," + lug.CN2.ToString() + "," + lug.BayCount.ToString() + "," + lug.Volume.ToString() + "," + lug.PieceCount.ToString() + "," + lug.Flags.ToString() + "," + lug.Devices.ToString() + ",'" + bit[0].ToString() + "'," + lug.FrameEnd.ToString(), connection);
                                            command.ExecuteNonQuery();

                                            LastFrameStart = Convert.ToInt16(lug.FrameStart);

                                            //acknowledge new lug data
                                            Ack.Value = "True";
                                            try
                                            {
                                                MyPLC.WriteTag(Ack);
                                                SqlCommand commandack = new SqlCommand("update DataReceivedLug set Ack = 'True' where FrameStart= " + lug.FrameStart.ToString(), connection);
                                                commandack.ExecuteNonQuery();

                                            }
                                            catch 
                                            {
                                                UpdateRaptorCommLog("Error writing Lug Acknowledge: " + MyPLC.ErrorString);

                                            }
                                            oldWEBSortCounter = WEBSortCounter;
                                            /*LugIndex++;
                                            WEBSortCounter++;
                                            if (LugIndex == 20)
                                                LugIndex = 0;*/
                                        }
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    UpdateRaptorCommLog("Error reading Lug UDT: " + ex.Message);
                    //return;
                }
                               
                //if (MyPLC.ErrorCode == 0)
                //if (ResultCode.QUAL_GOOD == lug.FrameStart.QualityCode && ResultCode.QUAL_GOOD == FrameEnd.QualityCode)
                
                /*else
                {
                    UpdateRaptorCommLog("Lug UDT Read: " + MyPLC.ErrorString);                    
                    Thread.Sleep(10000);
                }*/
                try
                {
                    if (MyPLC.IsConnected)
                        MyPLC.ReadTag(lugindex);
                }
                catch 
                {
                    UpdateRaptorCommLog("Error reading lugindex tag: " + MyPLC.ErrorString);
                }
                LugIndex = Convert.ToInt16(lugindex.Value);
                if (LugIndex > WEBSortCounter)  //PLC had advanced the bin index pointer
                {
                    if (LugIndex - WEBSortCounter == 1)
                        WEBSortCounter = LugIndex;
                    else
                    {
                        WEBSortCounter++;
                        if (WEBSortCounter >= 20)
                            WEBSortCounter = 0;
                    }
                }
                else if (LugIndex < WEBSortCounter)  //BinIndex has rolled over
                {
                    if (LugIndex - (WEBSortCounter - 20) == 1)
                        WEBSortCounter = LugIndex;
                    else
                    {
                        WEBSortCounter++;
                        if (WEBSortCounter >= 20)
                            WEBSortCounter = 0;
                    }
                }
                /*if (LugIndexPLC != oldLugIndexPLC)
                {
                    //if PLC lug index is too different than the previous, assume a temporary comm problem and re-sync  
                    if (Math.Abs(LugIndexPLC - oldLugIndexPLC) > 15)
                    {
                        ResetCounters();
                    }
                    else
                    {   //update counters
                        if (LugIndexPLC > oldLugIndexPLC)
                            PLCCounter = PLCCounter + (LugIndexPLC - oldLugIndexPLC);
                        else //account for rollover
                            PLCCounter = PLCCounter + ((20 - oldLugIndexPLC) + (LugIndexPLC));
                    }
                }
                oldLugIndexPLC = LugIndexPLC;*/
                //oldWEBSortCounter = WEBSortCounter;

                Thread.Sleep(LugThreadScanRate);
            }
        }
        public static void BinDataSentFromPLC()
        {
            // initialize counters
            ResetBinCounters();
            
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCBin1.IsConnected)
                {
                    while (MyPLCBin1.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Bin: " + MyPLCBin1.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Bin Connection to PLC Re-established Successfully!");
                }

                Tag myUDT = new Tag("WEBSortBayTable[" + WEBSortBinCounter + "]", Logix.Tag.ATOMIC.OBJECT);
                Tag Ack = new Tag("WEBSortBayTable[" + WEBSortBinCounter + "].Acknowledge", Logix.Tag.ATOMIC.BOOL);
                
                ///////////////////////////////////////
                // update the group
                try
                {
                    //UpdateRaptorCommLog("WEBSortBinCounter: " + WEBSortBinCounter + ", oldWEBSortBinCounter: " + oldWEBSortBinCounter);
                    if (MyPLCBin1.IsConnected && WEBSortBinCounter != oldWEBSortBinCounter)
                    {
                        if (MyPLCBin1.ReadTag(myUDT) != Logix.ResultCode.E_SUCCESS)
                        {
                            UpdateRaptorCommLog("Error reading Bin uDT: " + MyPLCBin1.ErrorString);
                            Thread.Sleep(10000);                        
                        }
                        else
                        {
                            BIN bin = (BIN)udtEnc.ToType(myUDT, typeof(BIN));                            
                                        
                            int BinStatus=0;

                            // the individual bit elements
                            bit = udtEnc.ToBoolArray(bin.BinFlags0);
                            bit1 = udtEnc.ToBoolArray(bin.BinFlags1);
                            if (bit[0] == true)
                                BinStatus = 1;
                            else if (bit[1] == true)
                                BinStatus = 0;
                            else if (bit[2] == true)
                                BinStatus = 3;
                            else if (bit[3] == true)
                                BinStatus = 2;
                            else if (bit[4] == true)
                                BinStatus = 2;
                            else if (bit[7] == true)
                                BinStatus = 4;
                            else if (bit1[0] == true)
                                BinStatus = 5;

                            //UpdateRaptorCommLog("WEBSortBinCounter=" + WEBSortBinCounter + ", PLCBinCounter=" + PLCBinCounter);
                            //check PLC for new bin data
                            //if (WEBSortBinCounter <= PLCBinCounter)
                                if (Convert.ToInt16(bin.FrameStart) == Convert.ToInt16(bin.FrameEnd))
                                    if (Convert.ToInt16(bin.FrameStart) != LastBinFrameStart)
                                        //if (Ack.Value.ToString() == "False")
                                        //if ((bin.BinFlags1 & 2) != 2)  //false
                                        if (bit1[1] == false)  //false
                                        {
                                            //UpdateRaptorCommLog("bit7=" + (bin.BinFlags0 & 128).ToString() + ", BinStatus=" + bin.BinFlags0.ToString());
                                            //process data in database
                                            SqlCommand command = new SqlCommand("execute UpdateBinData " + bin.FrameStart.ToString() + "," + bin.BayNum.ToString() + ",'',0," + bin.Count.ToString() + ",0,0,0,'" + bit[6].ToString() + "'," + BinStatus.ToString() + ",0,0,'" + bit[5].ToString() + "'," + bin.SortXRef.ToString() + ",0,0,0,0,0,0,0,'" + bit[7].ToString() + "'," + bin.FrameEnd.ToString(), connection);
                                            command.ExecuteNonQuery();
                                            LastBinFrameStart = Convert.ToInt16(bin.FrameStart);

                                            //acknowledge new lug data
                                            Ack.Value = "True";
                                            try
                                            {
                                                MyPLCBin1.WriteTag(Ack);
                                                SqlCommand commandack = new SqlCommand("update DataReceivedBin set Ack='True' where FrameStart=" + bin.FrameStart.ToString(), connection);
                                                commandack.ExecuteNonQuery();
                                            }
                                            catch
                                            {
                                                UpdateRaptorCommLog("Error writing Bin Acknowledge: " + MyPLCBin1.ErrorString);
                                            }
                                            oldWEBSortBinCounter = WEBSortBinCounter;

                                      /*      if (BinStatus == 0)   //if bin is being reset, zero out all products for this bin number, to clean things up
                                            {
                                                int bb;
                                                bb = bin.BayNum;
                                                if (bb >= 32)
                                                    bb = bb - 32;
                                                else if (bb >= 64)
                                                    bb = bb - 64;
                                                else if (bb >= 96)
                                                    bb = bb - 96;
                                                else if (bb >= 128)
                                                    bb = bb - 128;
                                                else if (bb >= 160)
                                                    bb = bb - 160;
                                                UpdateRaptorCommLog("Zeroing out bin products");
                                                        
                                                for (int i = 1; i <= 191; i++)
                                                {
                                                    Tag binlocation0 = new Tag("ActiveProductList[" + i + "].BayLocation[0]." + bb.ToString(), Logix.Tag.ATOMIC.DINT);
                                                    Tag binlocation1 = new Tag("ActiveProductList[" + i + "].BayLocation[1]." + bb.ToString(), Logix.Tag.ATOMIC.DINT);
                                                    Tag binlocation2 = new Tag("ActiveProductList[" + i + "].BayLocation[2]." + bb.ToString(), Logix.Tag.ATOMIC.DINT);
                                                    Tag binlocation3 = new Tag("ActiveProductList[" + i + "].BayLocation[3]." + bb.ToString(), Logix.Tag.ATOMIC.DINT);
                                                    Tag binlocation4 = new Tag("ActiveProductList[" + i + "].BayLocation[4]." + bb.ToString(), Logix.Tag.ATOMIC.DINT);
                                                    Tag binlocation5 = new Tag("ActiveProductList[" + i + "].BayLocation[5]." + bb.ToString(), Logix.Tag.ATOMIC.DINT);
                                                    binlocation0.Value = 0;
                                                    binlocation1.Value = 0;
                                                    binlocation2.Value = 0;
                                                    binlocation3.Value = 0;
                                                    binlocation4.Value = 0;
                                                    binlocation5.Value = 0;

                                                    ProductUDTGroup.Tags.Clear();
                                                    ProductUDTGroup.Clear();
                                                    if (bin.BayNum < 32)
                                                    {
                                                        //UpdateRaptorCommLog("Zeroing out bin products, product " + i.ToString() + ", bin " + bin.BayNum.ToString() + " binlocation0: " + binlocation0.Name);
                                                        ProductUDTGroup.AddTag(binlocation0);
                                                    }
                                                    else if (bin.BayNum < 64)
                                                    {
                                                        //UpdateRaptorCommLog("Zeroing out bin products, product " + i.ToString() + ", bin " + bin.BayNum.ToString() + " binlocation1: " + binlocation1.Name);
                                                        ProductUDTGroup.AddTag(binlocation1);
                                                    }
                                                    else if (bin.BayNum < 96)
                                                    {
                                                        //UpdateRaptorCommLog("Zeroing out bin products, product " + i.ToString() + ", bin " + bin.BayNum.ToString() + " binlocation2: " + binlocation2.Name);
                                                        ProductUDTGroup.AddTag(binlocation2);
                                                    }
                                                    else if (bin.BayNum < 128)
                                                    {
                                                        //UpdateRaptorCommLog("Zeroing out bin products, product " + i.ToString() + ", bin " + bin.BayNum.ToString() + " binlocation3: " + binlocation3.Name);
                                                        ProductUDTGroup.AddTag(binlocation3);
                                                    }
                                                    else if (bin.BayNum < 160)
                                                    {
                                                        //UpdateRaptorCommLog("Zeroing out bin products, product " + i.ToString() + ", bin " + bin.BayNum.ToString() + " binlocation4: " + binlocation4.Name);
                                                        ProductUDTGroup.AddTag(binlocation4);
                                                    }
                                                    else if (bin.BayNum < 192)
                                                    {
                                                        //UpdateRaptorCommLog("Zeroing out bin products, product " + i.ToString() + ", bin " + bin.BayNum.ToString() + " binlocation5: " + binlocation5.Name);
                                                        ProductUDTGroup.AddTag(binlocation5);
                                                    }
                                                    try
                                                    {
                                                        if (MyPLCBin1.IsConnected)
                                                        {
                                                            if (MyPLCBin1.GroupWrite(ProductUDTGroup) != Logix.ResultCode.E_SUCCESS)
                                                            {
                                                                UpdateRaptorCommLog("Error Zeroing out bin products: " + MyPLCBin1.ErrorString);
                                                                //Succeeded = 0;
                                                                Thread.Sleep(1000);
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        UpdateRaptorCommLog("Error Zeroing out bin products: " + ex.Message);
                                                    }
                                                }
                                            }*/
                                            //BinIndex++;
                                            //WEBSortBinCounter++;
                                            //if (WEBSortBinCounter == 20)
                                                //WEBSortBinCounter = 0;
                                            
                                            //UpdateRaptorCommLog("BinIndex=" + BinIndex + ", WEBSortBinCounter=" + WEBSortBinCounter);
                
                                            //if (BinIndex == 20)
                                                //BinIndex = 0;
                                            
                                        }
                        }
                    }
                        
                }
                catch (Exception ex)
                {
                    UpdateRaptorCommLog("Error reading Bin UdT: " + ex.Message);
                    //return;
                }
                                
                try
                {
                    if (MyPLCBin1.IsConnected)
                        MyPLCBin1.ReadTag(binindex);
                    else
                        UpdateRaptorCommLog("MyPLCBin1 is not connected");
                }
                catch 
                {
                    UpdateRaptorCommLog("Error reading binindex tag: " + MyPLCBin1.ErrorString);
                }
                
                BinIndex = Convert.ToInt16(binindex.Value);
                
                
                if (BinIndex > WEBSortBinCounter)  //PLC had advanced the bin index pointer
                {
                    if (BinIndex - WEBSortBinCounter == 1)
                        WEBSortBinCounter = BinIndex;
                    else
                    {
                        WEBSortBinCounter++;
                        if (WEBSortBinCounter >= 20)
                            WEBSortBinCounter = 0;
                    }
                }
                else if (BinIndex < WEBSortBinCounter)  //BinIndex has rolled over
                {
                    if (BinIndex - (WEBSortBinCounter-20)  == 1)
                        WEBSortBinCounter = BinIndex;
                    else
                    {
                        WEBSortBinCounter++;
                        if (WEBSortBinCounter >= 20)
                            WEBSortBinCounter = 0;
                    }
                }
                    
                   
                /*if (BinIndexPLC != oldBinIndexPLC)
                {
                    //if PLC lug index is too different than the previous, assume a temporary comm problem and re-sync  
                    if (Math.Abs(BinIndexPLC - oldBinIndexPLC) > 15)
                    {
                        ResetBinCounters();
                    }
                    else
                    {   //update counters
                        if (BinIndexPLC > oldBinIndexPLC)
                        {
                            //UpdateRaptorCommLog("Incrementing PLCBinCounter");
                            PLCBinCounter = PLCBinCounter + (BinIndexPLC - oldBinIndexPLC);
                        }
                        else //account for rollover
                        {
                            //UpdateRaptorCommLog("Incrementing PLCBinCounter (1)");
                            PLCBinCounter = PLCBinCounter + ((20 - oldBinIndexPLC) + (BinIndexPLC));
                        }
                    }
                }
                oldBinIndexPLC = BinIndexPLC;*/
                //UpdateRaptorCommLog("WEBSortBinCounter=" + WEBSortBinCounter + ", PLCBinCounter=" + PLCBinCounter);
                
                Thread.Sleep(BinDataFromPLCScanRate);
            }
        }
        public unsafe static void BinDataSentToPLC()
        {
            int Succeeded;
            DTEncoding dtEncBin = new DTEncoding();
            DTEncoding dtEncBin1 = new DTEncoding();
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCBin.IsConnected)
                {
                    while (MyPLCBin.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Bin Read/Write " + MyPLCBin.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Bin Read/Write Connection to PLC Re-established Successfully!");
                }
                
                if ((DataRequests & 1) == 1)
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try{
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsBin with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();                

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            Int16 BinID = Int16.Parse(reader["BinID"].ToString());
                            String BinName = reader["BinLabel"].ToString();
                            Int16 BinPkgSize = Int16.Parse(reader["BinSize"].ToString());
                            Single BinCount = Single.Parse(reader["BinCount"].ToString());
                            bool BinRdmWidthFlag = bool.Parse(reader["RW"].ToString());
                            Byte BinStatus = Byte.Parse(reader["BinStatus"].ToString());
                            int BinStamps = int.Parse(reader["BinStamps"].ToString());
                            int BinSprays = int.Parse(reader["BinSprays"].ToString());
                            bool BinTrimFlag = bool.Parse(reader["TrimFlag"].ToString());
                            Int16 BinSortXRef = Int16.Parse(reader["SortID"].ToString());
                            uint ProductMap0 = uint.Parse(reader["ProductMap0"].ToString());
                            uint ProductMap1 = uint.Parse(reader["ProductMap1"].ToString());
                            uint ProductMap2 = uint.Parse(reader["ProductMap2"].ToString());
                            uint ProductMap3 = uint.Parse(reader["ProductMap3"].ToString());
                            uint ProductMap4 = uint.Parse(reader["ProductMap4"].ToString());
                            uint ProductMap5 = uint.Parse(reader["ProductMap5"].ToString());
                            uint ProductMap0Old = uint.Parse(reader["ProductMap0Old"].ToString());
                            uint ProductMap1Old = uint.Parse(reader["ProductMap1Old"].ToString());
                            uint ProductMap2Old = uint.Parse(reader["ProductMap2Old"].ToString());
                            uint ProductMap3Old = uint.Parse(reader["ProductMap3Old"].ToString());
                            uint ProductMap4Old = uint.Parse(reader["ProductMap4Old"].ToString());
                            uint ProductMap5Old = uint.Parse(reader["ProductMap5Old"].ToString());
                            Byte SecProdID = Byte.Parse(reader["SecProdID"].ToString());
                            Byte SecSize = Byte.Parse(reader["SecSize"].ToString());
                            Byte SecCount = Byte.Parse(reader["SecCount"].ToString());
                            uint LengthMap = uint.Parse(reader["LengthMap"].ToString());
                            int ProductsOnly = int.Parse(reader["ProductsOnly"].ToString());
                            
                            
                            if (Write == true) //writing data to the PLC
                            {          
                                if (ProductsOnly == 3)
                                {
                                    WEBSortRefresh.Value = true;
                                    if (MyPLCBin.IsConnected)
                                    {
                                        if (MyPLCBin.WriteTag(WEBSortRefresh) != Logix.ResultCode.E_SUCCESS)
                                        {
                                            UpdateRaptorCommLog("Bin Tag Write WEBSortRefresh: " + MyPLCBin.ErrorString);
                                            Succeeded = 0;
                                            Thread.Sleep(10000);
                                        }
                                    }                                   
                                }                  
                                if (ProductsOnly == 0 || ProductsOnly == 2 || ProductsOnly == 3)  //write the entire UDT
                                {
                                    // encode BINREADWRITE for writing  
                                    Tag udtRead = new Tag("WebSortBay[" + BinID + "]",Logix.Tag.ATOMIC.OBJECT);
                                    Tag myUDT = new Tag("WebSortBay[" + BinID + "]", Logix.Tag.ATOMIC.OBJECT);
                                    myUDT.DataType = Logix.Tag.ATOMIC.OBJECT;

                                    MyPLCBin.ReadTag(udtRead);
                                    // get Abbreviated Type Code byte array
                                    UInt16 UDT_TypeCode = 0;
                                    byte[] typeCode = dtEncBin.GetDataTypeCode(udtRead);
                                    UDT_TypeCode = BitConverter.ToUInt16(typeCode, 0);
                                    BINREADWRITE binreadwrite = (BINREADWRITE)dtEncBin.ToType(udtRead, typeof(BINREADWRITE));

                                    binreadwrite.BinFlags = 0;
                                    if (BinStatus == 0)
                                    {
                                        binreadwrite.BinFlags += 2;  //Spare
                                        if (MyPLCBin.IsConnected)
                                        {
                                            NoSpareBays.Value = false;
                                            if (MyPLCBin.WriteTag(NoSpareBays) != Logix.ResultCode.E_SUCCESS)
                                            {
                                                UpdateRaptorCommLog("No Spare Bays Flag Write BinDataSentToPLC: " + NoSpareBays.ErrorString);
                                                Succeeded = 0;
                                                Thread.Sleep(10000);
                                            }
                                            //update spare bin count in PLC for the sort this bin was using
                                            SqlCommand cmd1 = new SqlCommand("declare @zone1start int,@zone1stop int,@binid int,@sortxref int select @binid=" + BinID + " select @sortxref=(select sortid from Bins where BinID=@binid) if @sortxref>0 begin select @zone1start=(select zone1start from Sorts where SortID=(select SortID from Bins where BinID=@binid) and recipeid=(select recipeid from recipes where online=1)) select @zone1stop=(select zone1stop from Sorts where SortID=(select SortID from Bins where BinID=@binid) and recipeid=(select recipeid from recipes where online=1))	if (@binid between @zone1start and @zone1stop)	select sparebincount1=(select count(*)+1 from bins where binstatus=0 and bins.binid between @zone1start and @zone1stop),sortxref=@sortxref	else select sparebincount1=99,sortxref=@sortxref end else select sparebincount1=999,sortxref=@sortxref ", connection);
                                            SqlDataReader reader1 = cmd1.ExecuteReader();
                                            reader1.Read();
                                            int spare1 = int.Parse(reader1["sparebincount1"].ToString());
                                            int sortxref = int.Parse(reader1["sortxref"].ToString());
                                            reader1.Close();
                                            SqlCommand cmd2 = new SqlCommand("declare @zone2start int,@zone2stop int,@binid int,@sortxref int select @binid=" + BinID + " select @sortxref=(select sortid from Bins where BinID=@binid) if @sortxref>0 begin select @zone2start=(select zone2start from Sorts where SortID=(select SortID from Bins where BinID=@binid) and recipeid=(select recipeid from recipes where online=1)) select @zone2stop=(select zone2stop from Sorts where SortID=(select SortID from Bins where BinID=@binid) and recipeid=(select recipeid from recipes where online=1))	if (@binid between @zone2start and @zone2stop)	select sparebincount2=(select count(*)+1 from bins where binstatus=0 and bins.binid between @zone2start and @zone2stop),sortxref=@sortxref	else select sparebincount2=99,sortxref=@sortxref end else select sparebincount2=999,sortxref=@sortxref ", connection);
                                            SqlDataReader reader2 = cmd2.ExecuteReader();
                                            reader2.Read();
                                            int spare2 = int.Parse(reader2["sparebincount2"].ToString());
                                            reader2.Close();
                                            if (spare1 < 99) spare2 = 99;
                                            if (spare2 < 99) spare1 = 99;
                                            if (spare1 != 99 || spare2 != 99)
                                            if (sortxref > 0)
                                            {
                                                Tag SpareBin1Count = new Tag("SpareBays[" + sortxref + "].Zone1Count", Logix.Tag.ATOMIC.DINT);
                                                Tag SpareBin2Count = new Tag("SpareBays[" + sortxref + "].Zone2Count", Logix.Tag.ATOMIC.DINT);
                                                BinSendUDTGroup.Tags.Clear();
                                                BinSendUDTGroup.Clear();
                                                BinSendUDTGroup.AddTag(SpareBin1Count);
                                                BinSendUDTGroup.AddTag(SpareBin2Count);

                                                SpareBin1Count.Value = spare1;
                                                SpareBin2Count.Value = spare2;
                                                if (MyPLCBin.GroupWrite(BinSendUDTGroup) != Logix.ResultCode.E_SUCCESS)
                                                {
                                                    UpdateRaptorCommLog("Bin Group Write Spare Bays BinDataSentToPLC: " + MyPLCBin.ErrorString);
                                                    Succeeded = 0;
                                                    Thread.Sleep(10000);
                                                }  
                                            }

                                        }
                                    }
                                    else if (BinStatus == 1)
                                        binreadwrite.BinFlags += 1;  //Active
                                    else if (BinStatus == 2)
                                        binreadwrite.BinFlags += 24;  //Full Count and Full Bay
                                    else if (BinStatus == 3)
                                    {
                                        binreadwrite.BinFlags += 4;   //Disabled
                                        
                                    }
                                    else if (BinStatus == 4)
                                        binreadwrite.BinFlags += 128;   //Reject
                                    if (BinStatus == 5)
                                    {
                                        binreadwrite.BinFlags += 256;   //Virtual
                                        if (BinPkgSize > 0)
                                            binreadwrite.BinFlags += 1;  //don't lose the active status on the virtual if it has been allocated

                                    }

                                    if (BinTrimFlag == true)
                                        binreadwrite.BinFlags += 32;
                                    if (BinRdmWidthFlag == true)
                                        binreadwrite.BinFlags += 64;
                                    
                                    binreadwrite.ProductArray.Product[0] = ProductMap0;
                                    binreadwrite.ProductArray.Product[1] = ProductMap1;
                                    binreadwrite.ProductArray.Product[2] = ProductMap2;
                                    binreadwrite.ProductArray.Product[3] = ProductMap3;
                                    binreadwrite.ProductArray.Product[4] = ProductMap4;
                                    binreadwrite.ProductArray.Product[5] = ProductMap5;
                                    binreadwrite.LengthMap = LengthMap;
                                    
                                    ASCIIEncoding enc = new ASCIIEncoding();
                                    Byte[] asciidata = new byte[20];                            
                                    asciidata = enc.GetBytes(BinName);
                                    for (int i = 0; i < asciidata.Length; i++)
                                        binreadwrite.Name.Data[i] = asciidata[i];

                                    binreadwrite.Name.Len = BinName.Length;                            
                                    binreadwrite.PkgSize = BinPkgSize;
                                    binreadwrite.Count = BinCount;
                                    binreadwrite.Stamps = BinStamps;
                                    binreadwrite.Sprays = BinSprays;
                                    //binreadwrite.SortXRef = BinSortXRef;
                                    binreadwrite.SecProdID = SecProdID;
                                    binreadwrite.SecSize = SecSize;
                                    binreadwrite.SecCount = SecCount;
                                                                        
                                    myUDT.Value = dtEncBin.FromType(binreadwrite);
                                    // set data type code
                                    dtEncBin.SetDataTypeCode(typeCode, myUDT);

                                    try
                                    {
                                        if (MyPLCBin.IsConnected)
                                        {
                                            if (MyPLCBin.WriteTag(myUDT) != Logix.ResultCode.E_SUCCESS)
                                            {
                                                UpdateRaptorCommLog("Bin UDT Write BinDataSentToPLC: " + myUDT.ErrorString);
                                                Succeeded = 0;
                                                Thread.Sleep(10000);
                                            }                               
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error writing BinSendUDTGroup: " + ex.Message);
                                    }                                
                                }
                                else if (ProductsOnly == 1) //write products tag only
                                {
                                    Tag Product0 = new Tag("WebSortBay[" + BinID + "].Product[0]", Logix.Tag.ATOMIC.DINT);
                                    Tag Product1 = new Tag("WebSortBay[" + BinID + "].Product[1]", Logix.Tag.ATOMIC.DINT);
                                    Tag Product2 = new Tag("WebSortBay[" + BinID + "].Product[2]", Logix.Tag.ATOMIC.DINT);
                                    Tag Product3 = new Tag("WebSortBay[" + BinID + "].Product[3]", Logix.Tag.ATOMIC.DINT);
                                    Tag Product4 = new Tag("WebSortBay[" + BinID + "].Product[4]", Logix.Tag.ATOMIC.DINT);
                                    Tag Product5 = new Tag("WebSortBay[" + BinID + "].Product[5]", Logix.Tag.ATOMIC.DINT);
                                    Tag LengthMap0 = new Tag("WebSortBay[" + BinID + "].Length", Logix.Tag.ATOMIC.DINT);

                                    BinSendUDTGroup.Tags.Clear();
                                    BinSendUDTGroup.Clear();
                                    BinSendUDTGroup.AddTag(Product0);
                                    BinSendUDTGroup.AddTag(Product1);
                                    BinSendUDTGroup.AddTag(Product2);
                                    BinSendUDTGroup.AddTag(Product3);
                                    BinSendUDTGroup.AddTag(Product4);
                                    BinSendUDTGroup.AddTag(Product5);
                                    BinSendUDTGroup.AddTag(LengthMap0);

                                    Product0.Value = ProductMap0;
                                    Product1.Value = ProductMap1;
                                    Product2.Value = ProductMap2;
                                    Product3.Value = ProductMap3;
                                    Product4.Value = ProductMap4;
                                    Product5.Value = ProductMap5;
                                    LengthMap0.Value = LengthMap;

                                    try
                                    {
                                        if (MyPLCBin.IsConnected)
                                        {
                                            if (MyPLCBin.GroupWrite(BinSendUDTGroup) != Logix.ResultCode.E_SUCCESS)
                                            {
                                                UpdateRaptorCommLog("Bin Group Write BinDataSentToPLC: " + MyPLCBin.ErrorString);
                                                Succeeded = 0;
                                                Thread.Sleep(10000);
                                            }                                          
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error writing BinSendUDTGroup: " + ex.Message);
                                    }
                                    
                                }
                                else if (ProductsOnly == 4) //write count,size and status only (from sorter view screen)
                                {
                                    Tag Stamps = new Tag("WebSortBay[" + BinID + "].Stamps", Logix.Tag.ATOMIC.DINT);
                                    Tag SortXRef = new Tag("WebSortBay[" + BinID + "].SortXRef", Logix.Tag.ATOMIC.INT);
                                    Tag Count = new Tag("WebSortBay[" + BinID + "].Count", Logix.Tag.ATOMIC.INT);
                                    Tag Size = new Tag("WebSortBay[" + BinID + "].PkgSize", Logix.Tag.ATOMIC.INT);
                                    Tag Spare = new Tag("WebSortBay[" + BinID + "].Spare", Logix.Tag.ATOMIC.BOOL);
                                    Tag SecProd = new Tag("WebSortBay[" + BinID + "].SecID", Logix.Tag.ATOMIC.SINT);
                                    Tag SecCo = new Tag("WebSortBay[" + BinID + "].SecCount", Logix.Tag.ATOMIC.SINT);
                                    Tag SecSi = new Tag("WebSortBay[" + BinID + "].SecSize", Logix.Tag.ATOMIC.SINT);
                                    Tag FullCount = new Tag("WebSortBay[" + BinID + "].FullCount", Logix.Tag.ATOMIC.BOOL);
                                    Tag FullBay = new Tag("WebSortBay[" + BinID + "].FullBay", Logix.Tag.ATOMIC.BOOL);
                                    Tag Active = new Tag("WebSortBay[" + BinID + "].Active", Logix.Tag.ATOMIC.BOOL);
                                    Tag Disabled = new Tag("WebSortBay[" + BinID + "].Disabled", Logix.Tag.ATOMIC.BOOL);
                                    Tag LengthMap0 = new Tag("WebSortBay[" + BinID + "].LengthMap", Logix.Tag.ATOMIC.DINT);
                                    bool bSpare = false, bFullBay = false, bFullCount = false, bActive = false, bDisabled=false;

                                    if (BinStatus == 0)
                                    {
                                        bSpare = true;
                                        // encode BINREADWRITE for writing  
                                        Tag udtRead = new Tag("WebSortBay[" + BinID + "]", Logix.Tag.ATOMIC.OBJECT);
                                        Tag myUDT = new Tag("WebSortBay[" + BinID + "]", Logix.Tag.ATOMIC.OBJECT);
                                        myUDT.DataType = Logix.Tag.ATOMIC.OBJECT;

                                        MyPLCBin.ReadTag(udtRead);
                                        // get Abbreviated Type Code byte array
                                        UInt16 UDT_TypeCode = 0;
                                        byte[] typeCode = dtEncBin.GetDataTypeCode(udtRead);
                                        UDT_TypeCode = BitConverter.ToUInt16(typeCode, 0);
                                        BINREADWRITE binreadwrite = (BINREADWRITE)dtEncBin.ToType(udtRead, typeof(BINREADWRITE));

                                       
                                        binreadwrite.BinFlags = 0;
                                        
                                        binreadwrite.BinFlags += 2;  //Spare
                                        if (MyPLCBin.IsConnected)
                                        {
                                            NoSpareBays.Value = false;
                                            if (MyPLCBin.WriteTag(NoSpareBays) != Logix.ResultCode.E_SUCCESS)
                                            {
                                                UpdateRaptorCommLog("No Spare Bays Flag Write BinDataSentToPLC: " + NoSpareBays.ErrorString);
                                                Succeeded = 0;
                                                Thread.Sleep(10000);
                                            }
                                            //update spare bin count in PLC for the sort this bin was using
                                            SqlCommand cmd1 = new SqlCommand("declare @zone1start int,@zone1stop int,@binid int,@sortxref int select @binid=" + BinID + " select @sortxref=(select sortid from Bins where BinID=@binid) if @sortxref>0 begin select @zone1start=(select zone1start from Sorts where SortID=(select SortID from Bins where BinID=@binid) and recipeid=(select recipeid from recipes where online=1)) select @zone1stop=(select zone1stop from Sorts where SortID=(select SortID from Bins where BinID=@binid) and recipeid=(select recipeid from recipes where online=1))	if (@binid between @zone1start and @zone1stop)	select sparebincount1=(select count(*)+1 from bins where binstatus=0 and bins.binid between @zone1start and @zone1stop),sortxref=@sortxref	else select sparebincount1=99,sortxref=@sortxref end else select sparebincount1=999,sortxref=@sortxref ", connection);
                                            SqlDataReader reader1 = cmd1.ExecuteReader();
                                            reader1.Read();
                                            int spare1 = int.Parse(reader1["sparebincount1"].ToString());
                                            int sortxref = int.Parse(reader1["sortxref"].ToString());
                                            reader1.Close();
                                            SqlCommand cmd2 = new SqlCommand("declare @zone2start int,@zone2stop int,@binid int,@sortxref int select @binid=" + BinID + " select @sortxref=(select sortid from Bins where BinID=@binid) if @sortxref>0 begin select @zone2start=(select zone2start from Sorts where SortID=(select SortID from Bins where BinID=@binid) and recipeid=(select recipeid from recipes where online=1)) select @zone2stop=(select zone2stop from Sorts where SortID=(select SortID from Bins where BinID=@binid) and recipeid=(select recipeid from recipes where online=1))	if (@binid between @zone2start and @zone2stop)	select sparebincount2=(select count(*)+1 from bins where binstatus=0 and bins.binid between @zone2start and @zone2stop),sortxref=@sortxref	else select sparebincount2=99,sortxref=@sortxref end else select sparebincount2=999,sortxref=@sortxref ", connection);
                                            SqlDataReader reader2 = cmd2.ExecuteReader();
                                            reader2.Read();
                                            int spare2 = int.Parse(reader2["sparebincount2"].ToString());
                                            reader2.Close();
                                            if (spare1 < 99) spare2 = 99;
                                            if (spare2 < 99) spare1 = 99;
                                            if (spare1 != 99 || spare2 != 99)
                                                if (sortxref > 0)
                                                {
                                                    Tag SpareBin1Count = new Tag("SpareBays[" + sortxref + "].Zone1Count", Logix.Tag.ATOMIC.DINT);
                                                    Tag SpareBin2Count = new Tag("SpareBays[" + sortxref + "].Zone2Count", Logix.Tag.ATOMIC.DINT);
                                                    BinSendUDTGroup.Tags.Clear();
                                                    BinSendUDTGroup.Clear();
                                                    BinSendUDTGroup.AddTag(SpareBin1Count);
                                                    BinSendUDTGroup.AddTag(SpareBin2Count);

                                                    SpareBin1Count.Value = spare1;
                                                    SpareBin2Count.Value = spare2;
                                                    if (MyPLCBin.GroupWrite(BinSendUDTGroup) != Logix.ResultCode.E_SUCCESS)
                                                    {
                                                        UpdateRaptorCommLog("Bin Group Write Spare Bays BinDataSentToPLC: " + MyPLCBin.ErrorString);
                                                        Succeeded = 0;
                                                        Thread.Sleep(10000);
                                                    }
                                                }

                                        }                                

                                        if (BinTrimFlag == true)
                                            binreadwrite.BinFlags += 32;
                                        if (BinRdmWidthFlag == true)
                                            binreadwrite.BinFlags += 64;

                                        binreadwrite.ProductArray.Product[0] = ProductMap0;
                                        binreadwrite.ProductArray.Product[1] = ProductMap1;
                                        binreadwrite.ProductArray.Product[2] = ProductMap2;
                                        binreadwrite.ProductArray.Product[3] = ProductMap3;
                                        binreadwrite.ProductArray.Product[4] = ProductMap4;
                                        binreadwrite.ProductArray.Product[5] = ProductMap5;
                                        binreadwrite.LengthMap = LengthMap;

                                        ASCIIEncoding enc = new ASCIIEncoding();
                                        Byte[] asciidata = new byte[20];
                                        asciidata = enc.GetBytes(BinName);
                                        for (int i = 0; i < asciidata.Length; i++)
                                            binreadwrite.Name.Data[i] = asciidata[i];

                                        binreadwrite.Name.Len = BinName.Length;
                                        binreadwrite.PkgSize = BinPkgSize;
                                        binreadwrite.Count = BinCount;
                                        binreadwrite.Stamps = BinStamps;
                                        binreadwrite.Sprays = BinSprays;
                                        binreadwrite.SortXRef = BinSortXRef;
                                        binreadwrite.SecProdID = SecProdID;
                                        binreadwrite.SecSize = SecSize;
                                        binreadwrite.SecCount = SecCount;
                                        
                                        myUDT.Value = dtEncBin.FromType(binreadwrite);
                                        // set data type code
                                        dtEncBin.SetDataTypeCode(typeCode, myUDT);

                                        try
                                        {
                                            if (MyPLCBin.IsConnected)
                                            {
                                                if (MyPLCBin.WriteTag(myUDT) != Logix.ResultCode.E_SUCCESS)
                                                {
                                                    UpdateRaptorCommLog("Bin UDT Write BinDataSentToPLC: " + myUDT.ErrorString);
                                                    Succeeded = 0;
                                                    Thread.Sleep(10000);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            UpdateRaptorCommLog("Error writing BinSendUDTGroup: " + ex.Message);
                                        }    
                                    }
                                    else
                                    {
                                        
                                        if (BinStatus == 1)
                                            bActive = true;
                                        else if (BinStatus == 2)
                                        {
                                            bFullBay = true;
                                            bFullCount = true;
                                        }
                                        else if (BinStatus == 3)
                                        {
                                            bDisabled = true;
                                           
                                        }

                                        Spare.Value = bSpare;
                                        Active.Value = bActive;
                                        FullBay.Value = bFullBay;
                                        FullCount.Value = bFullCount;
                                        Disabled.Value = bDisabled;
                                        Count.Value = BinCount;
                                        Size.Value = BinPkgSize;
                                        Stamps.Value = BinStamps;
                                        SecProd.Value = SecProd;
                                        SecSi.Value = SecSize;
                                        SecCo.Value = SecCo;
                                        
                                        BinSendUDTGroup.Tags.Clear();
                                        BinSendUDTGroup.Clear();
                                        BinSendUDTGroup.AddTag(Count);
                                        BinSendUDTGroup.AddTag(Stamps);
                                        BinSendUDTGroup.AddTag(Size);
                                        BinSendUDTGroup.AddTag(Spare);
                                        //BinSendUDTGroup.AddTag(SecProd);
                                        //BinSendUDTGroup.AddTag(SecSi);
                                        //BinSendUDTGroup.AddTag(SecCo);
                                        BinSendUDTGroup.AddTag(Active);
                                        BinSendUDTGroup.AddTag(FullCount);
                                        BinSendUDTGroup.AddTag(FullBay);
                                        BinSendUDTGroup.AddTag(Disabled);

                                        try
                                        {
                                            if (MyPLCBin.IsConnected)
                                            {
                                                if (MyPLCBin.GroupWrite(BinSendUDTGroup) != Logix.ResultCode.E_SUCCESS)
                                                {
                                                    UpdateRaptorCommLog("Bin Group Write BinDataSentToPLC: " + MyPLCBin.ErrorString);
                                                    Succeeded = 0;
                                                    Thread.Sleep(10000);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            UpdateRaptorCommLog("Error writing BinSendUDTGroup: " + ex.Message);
                                        }
                                    }

                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag myUDT = new Tag("WEBSortBay[" + BinID + "]", Logix.Tag.ATOMIC.OBJECT);
                                /*Tag Name = new Tag("WebSortBay[" + BinID + "].Name", Logix.Tag.ATOMIC.STRING);
                                Tag PkgSize = new Tag("WebSortBay[" + BinID + "].PkgSize", Logix.Tag.ATOMIC.INT);
                                Tag Count = new Tag("WebSortBay[" + BinID + "].Count", Logix.Tag.ATOMIC.INT);
                                Tag RdmWidthFlag = new Tag("WebSortBay[" + BinID + "].RdmWidthFlag", Logix.Tag.ATOMIC.BOOL);
                                Tag Status = new Tag("WebSortBay[" + BinID + "].Status", Logix.Tag.ATOMIC.SINT);
                                Tag Stamps = new Tag("WebSortBay[" + BinID + "].Stamps", Logix.Tag.ATOMIC.DINT);
                                Tag Sprays = new Tag("WebSortBay[" + BinID + "].Sprays", Logix.Tag.ATOMIC.DINT);
                                Tag TrimFlag = new Tag("WebSortBay[" + BinID + "].TrimFlag", Logix.Tag.ATOMIC.BOOL);
                                Tag SortXRef = new Tag("WebSortBay[" + BinID + "].SortXRef", Logix.Tag.ATOMIC.INT);

                                BinSendUDTGroup.Tags.Clear();
                                BinSendUDTGroup.Clear();
                                BinSendUDTGroup.AddTag(Name);
                                BinSendUDTGroup.AddTag(PkgSize);
                                BinSendUDTGroup.AddTag(Count);
                                BinSendUDTGroup.AddTag(RdmWidthFlag);
                                BinSendUDTGroup.AddTag(Status);
                                BinSendUDTGroup.AddTag(Stamps);
                                BinSendUDTGroup.AddTag(Sprays);
                                BinSendUDTGroup.AddTag(TrimFlag);
                                BinSendUDTGroup.AddTag(SortXRef);*/

                                if (MyPLCBin.IsConnected)
                                {
                                    if (MyPLCBin.ReadTag(myUDT) != Logix.ResultCode.E_SUCCESS)
                                    {
                                        UpdateRaptorCommLog("Error reading Bin UDt: " + MyPLCBin.ErrorString);
                                        Succeeded = 0;
                                        Thread.Sleep(10000);
                                    }
                                    else
                                    {
                                        BINREADWRITE binreadwrite = (BINREADWRITE)dtEncBin.ToType(myUDT, typeof(BINREADWRITE));
                                
                                        //write results into the database
                                        try
                                        {     
                                            // the individual bit elements
                                            //bit = dtEncBin.ToBoolArray(binreadwrite.RdmWidthFlag);
                                            //bit1 = dtEncBin.ToBoolArray(binreadwrite.TrimFlag); 
                                            if ((binreadwrite.BinFlags & 2) == 2)  //spare
                                                BinStatus=0;
                                            if ((binreadwrite.BinFlags & 1) == 1)  //active
                                                BinStatus = 1;
                                            if ((binreadwrite.BinFlags & 8) == 8)  //full at count
                                                BinStatus = 2;
                                            if ((binreadwrite.BinFlags & 16) == 16) //full at bay
                                                BinStatus = 2;
                                            if ((binreadwrite.BinFlags & 4) == 4)  //disabled
                                                BinStatus = 3;
                                            if ((binreadwrite.BinFlags & 128) == 128)  //Reject
                                                BinStatus = 4;
                                            if ((binreadwrite.BinFlags & 256) == 256)  //Virtual  
                                                BinStatus = 5;                               

                                            if ((binreadwrite.BinFlags & 32) == 32)
                                                BinTrimFlag = true;
                                            if ((binreadwrite.BinFlags & 64) == 64)
                                                BinRdmWidthFlag = true;
                                            ASCIIEncoding enc = new ASCIIEncoding();
                                            Byte[] asciidata = new byte[20];
                                            for (int i = 0; i < binreadwrite.Name.Len; i++)
                                                asciidata[i] = binreadwrite.Name.Data[i];
                                            String Name = enc.GetString(asciidata,0,binreadwrite.Name.Len);
                                            //UpdateRaptorCommLog("test: " + binreadwrite.Status.ToString());
                                            SqlCommand cmd0 = new SqlCommand("update datarequestsbin set binlabel='" + Name.Replace("'", "''") + "',binsize=" + binreadwrite.PkgSize.ToString() + ",bincount=" + binreadwrite.Count.ToString() + ",secprodid=" + binreadwrite.SecProdID.ToString() + ",secsize=" + binreadwrite.SecSize.ToString() + ",seccount=" + binreadwrite.SecCount.ToString() + ",RW='" + BinRdmWidthFlag.ToString() + "',BinStatus=" + BinStatus.ToString() + ",BinStamps=" + binreadwrite.Stamps.ToString() + ",BinSprays=" + binreadwrite.Sprays.ToString() + ",TrimFlag='" + BinTrimFlag.ToString() + "',SortID=" + binreadwrite.SortXRef.ToString() + ",ProductMap0=" + binreadwrite.ProductArray.Product[0].ToString() + ",ProductMap1=" + binreadwrite.ProductArray.Product[1].ToString() + ",ProductMap2=" + binreadwrite.ProductArray.Product[2].ToString() + ",LengthMap=" + binreadwrite.LengthMap.ToString() + " where id=" + reader["id"].ToString(), connection);
                                            cmd0.ExecuteNonQuery();
                                            SqlCommand cmd1 = new SqlCommand("execute UpdateBinData 0," + BinID.ToString() + ",'" + Name.Replace("'", "''") + "'," + binreadwrite.PkgSize.ToString() + "," + binreadwrite.Count.ToString() + "," + binreadwrite.SecProdID.ToString() + "," + binreadwrite.SecSize.ToString() + "," + binreadwrite.SecCount.ToString() + ",'" + BinRdmWidthFlag.ToString() + "'," + BinStatus.ToString() + "," + binreadwrite.Stamps.ToString() + "," + binreadwrite.Sprays.ToString() + ",'" + BinTrimFlag.ToString() + "'," + binreadwrite.SortXRef.ToString() + "," + binreadwrite.ProductArray.Product[0].ToString() + "," + binreadwrite.ProductArray.Product[1].ToString() + "," + binreadwrite.ProductArray.Product[2].ToString() + "," + binreadwrite.ProductArray.Product[3].ToString() + "," + binreadwrite.ProductArray.Product[4].ToString() + "," + binreadwrite.ProductArray.Product[5].ToString() + "," + binreadwrite.LengthMap.ToString() + ",0,1", connection);
                                            cmd1.ExecuteNonQuery();
                                            
                                        }
                                        catch (Exception ex)
                                        {
                                            UpdateRaptorCommLog("Error updating bins data in database: " + ex.Message);
                                            Succeeded = 0;
                                        }
                                    }
                                
                            }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsBin set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsBin where id <= (select max(id)-500 from DataRequestsBin)", connection);
                            cmd12.ExecuteNonQuery();
                            
                            //process bin by product data to the PLC, but only after successfully processing the rest of the bin data write
                            if (Write == true) //writing data to the PLC
                            {            
                                uint oldbit=0,newbit=0,BayOn=0;
                                bool writetoplc;
                                         
                                if (ProductsOnly == 1 || ProductsOnly == 2 || ProductsOnly == 3 || (ProductsOnly == 4 && BinStatus == 0))  //write the product by bin data to the PLC
                                {
                                    SqlCommand cmd1 = new SqlCommand("select NumProducts from WEBSortSetup", connection);
                                    SqlDataReader reader1 = cmd1.ExecuteReader();
                                    reader1.Read();
                                    int NumProducts = int.Parse(reader1["NumProducts"].ToString());
                                    reader1.Close();
                                    
                                    for (int i=1; i<=NumProducts; i++)
                                    {
                                        BayOn=0;
                                        writetoplc = false;
                                        if (i<32)
                                        {
                                            oldbit = (ProductMap0Old & Convert.ToUInt32(Math.Pow(2,Convert.ToDouble(i)))) / Convert.ToUInt32(Math.Pow(2,Convert.ToDouble(i)));
                                            newbit = (ProductMap0 & Convert.ToUInt32(Math.Pow(2,Convert.ToDouble(i)))) / Convert.ToUInt32(Math.Pow(2,Convert.ToDouble(i)));
                                        }
                                        else if (i < 64)
                                        {
                                            oldbit = (ProductMap1Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i-32)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i-32)));
                                            newbit = (ProductMap1 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i-32)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i-32)));
                                        }
                                        else if (i < 96)
                                        {
                                            oldbit = (ProductMap2Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 64)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 64)));
                                            newbit = (ProductMap2 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 64)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 64)));
                                        }
                                        else if (i < 128)
                                        {
                                            oldbit = (ProductMap3Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 96)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 96)));
                                            newbit = (ProductMap3 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 96)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 96)));
                                        }
                                        else if (i < 160)
                                        {
                                            oldbit = (ProductMap4Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 128)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 128)));
                                            newbit = (ProductMap4 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 128)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 128)));
                                        }
                                        else if (i < 192)
                                        {
                                            oldbit = (ProductMap5Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 160)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 160)));
                                            newbit = (ProductMap5 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 160)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 160)));
                                        }
                                        if (oldbit == 1 && newbit == 0) //product has been removed
                                        {
                                            BayOn = 0;                                        
                                            writetoplc = true;                                         
                                        }                                        
                                        else if (oldbit == 0 && newbit == 1) //product has been added
                                        {
                                            BayOn=1;                                        
                                            writetoplc = true;                                        
                                        }
                                        else if (oldbit == 1 && newbit == 1) //send it anyway to keep synced
                                        {
                                            BayOn = 1;
                                            writetoplc = true;
                                        }
                                        
                                        if (writetoplc)
                                        {
                                            //now write tag to PLC
                                            Tag BayLocation = new Tag("ActiveProductList[" + i + "].BayLocation[" + (BinID/32).ToString() + "]." + (BinID & 31).ToString(), Logix.Tag.ATOMIC.DINT);
                                            
                                            UpdateRaptorCommLog(BayLocation.Name + "," + BayOn.ToString());
                                            BayLocation.Value = BayOn; 
                                            
                                            try
                                            {
                                                if (MyPLCBin.IsConnected)
                                                {
                                                    if (MyPLCBin.WriteTag(BayLocation) != Logix.ResultCode.E_SUCCESS)
                                                    {
                                                        UpdateRaptorCommLog("Bin Group Write Bin by Product Map: " + MyPLCBin.ErrorString);
                                                        Succeeded = 0;
                                                        Thread.Sleep(10000);
                                                    }
                                                    System.Threading.Thread.Sleep(10);
                                                }
                                                
                                            }
                                            catch (Exception ex)
                                            {
                                                UpdateRaptorCommLog("Error writing BinSendUDTGroup - Bin by Product Map: " + ex.Message);
                                            }
                                        }
                                    } //NumProducts
                                    
                                }
                            }
                        }
                        reader.Close();
                    }
                    catch(Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsBin table: " + ex.Message);   
                    }           
                }
                Thread.Sleep(BinDataToPLCScanRate);
            }
        }

        public unsafe static void SortDataSentToPLC()
        {
            int Succeeded;
            DTEncoding dtEncSort = new DTEncoding();
            DTEncoding dtEncSort1 = new DTEncoding();
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                if (!MyPLCSort.IsConnected)
                {
                    while (MyPLCSort.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Sort " + MyPLCSort.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Sort Connection to PLC Re-established Successfully!");
                }
                
                if ((DataRequests & 2) == 2)
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsSort with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            Int16 SortID = Int16.Parse(reader["SortID"].ToString());
                            String SortName = reader["SortLabel"].ToString();
                            Int16 SortPkgSize = Int16.Parse(reader["SortSize"].ToString());
                            Int16 Zone1 = Int16.Parse(reader["Zone1"].ToString());
                            Int16 Zone2 = Int16.Parse(reader["Zone2"].ToString());
                            bool SortRdmWidthFlag = bool.Parse(reader["RW"].ToString());
                            bool Active = bool.Parse(reader["Active"].ToString());
                            int SortStamps = int.Parse(reader["SortStamps"].ToString());
                            int SortSprays = int.Parse(reader["SortSprays"].ToString());
                            bool SortTrimFlag = bool.Parse(reader["TrimFlag"].ToString());
                            Byte SortOrderCount = Byte.Parse(reader["OrderCount"].ToString());
                            Byte PkgsPerSort = Byte.Parse(reader["PkgsPerSort"].ToString());
                            Byte SecProdID = Byte.Parse(reader["SecProdID"].ToString());
                            Byte SecSize = Byte.Parse(reader["SecSize"].ToString());
                            uint ProductMap0 = uint.Parse(reader["ProductMap0"].ToString());
                            uint ProductMap1 = uint.Parse(reader["ProductMap1"].ToString());
                            uint ProductMap2 = uint.Parse(reader["ProductMap2"].ToString());
                            uint ProductMap3 = uint.Parse(reader["ProductMap3"].ToString());
                            uint ProductMap4 = uint.Parse(reader["ProductMap4"].ToString());
                            uint ProductMap5 = uint.Parse(reader["ProductMap5"].ToString());
                            uint ProductMap0c = uint.Parse(reader["ProductMap0c"].ToString());
                            uint ProductMap1c = uint.Parse(reader["ProductMap1c"].ToString());
                            uint ProductMap2c = uint.Parse(reader["ProductMap2c"].ToString());
                            uint ProductMap0Old = uint.Parse(reader["ProductMap0Old"].ToString());
                            uint ProductMap1Old = uint.Parse(reader["ProductMap1Old"].ToString());
                            uint ProductMap2Old = uint.Parse(reader["ProductMap2Old"].ToString());
                            uint ProductMap3Old = uint.Parse(reader["ProductMap3Old"].ToString());
                            uint ProductMap4Old = uint.Parse(reader["ProductMap4Old"].ToString());
                            uint ProductMap5Old = uint.Parse(reader["ProductMap5Old"].ToString());
                            uint LengthMap = uint.Parse(reader["LengthMap"].ToString());
                            uint LengthMapc = uint.Parse(reader["LengthMapc"].ToString());
                            int ProductsOnly = int.Parse(reader["ProductsOnly"].ToString());
                           

                            if (Write == true) //writing data to the PLC
                            {
                                if (ProductsOnly == 0 || ProductsOnly == 2)  //write the entire UDT
                                {
                                    // encode SORTREADWRITE for writing  
                                    Tag udtRead = new Tag("WebSortTable[" + SortID + "]",Logix.Tag.ATOMIC.OBJECT);
                                    Tag myUDT = new Tag("WebSortTable[" + SortID + "]",Logix.Tag.ATOMIC.OBJECT);
                                    myUDT.DataType = Logix.Tag.ATOMIC.OBJECT;

                                    MyPLCSort.ReadTag(udtRead);
                                    // get Abbreviated Type Code byte array
                                    UInt16 UDT_TypeCode = 0;
                                    byte[] typeCode = dtEncSort.GetDataTypeCode(udtRead);
                                    UDT_TypeCode = BitConverter.ToUInt16(typeCode, 0);
                                    SORTREADWRITE Sortreadwrite = (SORTREADWRITE)dtEncSort.ToType(udtRead, typeof(SORTREADWRITE));

                                    Sortreadwrite.SortFlags = 0;
                                    
                                    if (Active == true)
                                        Sortreadwrite.SortFlags += 4;
                                    if (SortTrimFlag == true)
                                        Sortreadwrite.SortFlags += 1;
                                    if (SortRdmWidthFlag == true)
                                        Sortreadwrite.SortFlags += 2;

                                    Sortreadwrite.ProductArray.Product[0] = ProductMap0;
                                    Sortreadwrite.ProductArray.Product[1] = ProductMap1;
                                    Sortreadwrite.ProductArray.Product[2] = ProductMap2;
                                    Sortreadwrite.ProductArray.Product[3] = ProductMap3;
                                    Sortreadwrite.ProductArray.Product[4] = ProductMap4;
                                    Sortreadwrite.ProductArray.Product[5] = ProductMap5;
                                    Sortreadwrite.LengthMap = LengthMap;
                                    
                                    
                                    ASCIIEncoding enc = new ASCIIEncoding();
                                    Byte[] asciidata = new byte[20];
                                    asciidata = enc.GetBytes(SortName);
                                    for (int i = 0; i < asciidata.Length; i++)
                                        Sortreadwrite.Name.Data[i] = asciidata[i];

                                    Sortreadwrite.Name.Len = SortName.Length;
                                    Sortreadwrite.PkgSize = SortPkgSize;
                                    Sortreadwrite.Stamps = SortStamps;
                                    Sortreadwrite.Sprays = SortSprays;
                                    Sortreadwrite.OrderCount = SortOrderCount;
                                    Sortreadwrite.PkgsPerSort = PkgsPerSort;
                                    Sortreadwrite.Zone1 = Zone1;
                                    Sortreadwrite.Zone2 = Zone2;
                                    Sortreadwrite.SecProdID = SecProdID;
                                    Sortreadwrite.SecSize = SecSize;
                                    
                                    myUDT.Value = dtEncSort.FromType(Sortreadwrite);
                                    // set data type code
                                    dtEncSort.SetDataTypeCode(typeCode, myUDT);

                                    try
                                    {
                                        if (MyPLCSort.IsConnected)
                                        {
                                            if (MyPLCSort.WriteTag(myUDT) != Logix.ResultCode.E_SUCCESS)
                                            {
                                                UpdateRaptorCommLog("Sort UDT Write SortDataSentToPLC: " + myUDT.ErrorString);
                                                Succeeded = 0;
                                                Thread.Sleep(10000);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error writing SortSendUDTGroup: " + ex.Message);
                                    }
                                }
                                else if (ProductsOnly == 1) //write products tag only
                                {
                                    Tag Product0 = new Tag("WebSortTable[" + SortID + "].Product[0]", Logix.Tag.ATOMIC.DINT);
                                    Tag Product1 = new Tag("WebSortTable[" + SortID + "].Product[1]", Logix.Tag.ATOMIC.DINT);
                                    Tag Product2 = new Tag("WebSortTable[" + SortID + "].Product[2]", Logix.Tag.ATOMIC.DINT);
                                    Tag Product3 = new Tag("WebSortTable[" + SortID + "].Product[3]", Logix.Tag.ATOMIC.DINT);
                                    Tag Product4 = new Tag("WebSortTable[" + SortID + "].Product[4]", Logix.Tag.ATOMIC.DINT);
                                    Tag Product5 = new Tag("WebSortTable[" + SortID + "].Product[5]", Logix.Tag.ATOMIC.DINT);
                                    Tag LengthMap0 = new Tag("WebSortTable[" + SortID + "].Length", Logix.Tag.ATOMIC.DINT);

                                    //Tag Product0c = new Tag("WebSortTable[" + SortID + "].ProductChild[0]", Logix.Tag.ATOMIC.DINT);
                                    //Tag Product1c = new Tag("WebSortTable[" + SortID + "].ProductChild[1]", Logix.Tag.ATOMIC.DINT);
                                    //Tag Product2c = new Tag("WebSortTable[" + SortID + "].ProductChild[2]", Logix.Tag.ATOMIC.DINT);
                                    //Tag LengthMap0c = new Tag("WebSortTable[" + SortID + "].LengthChild", Logix.Tag.ATOMIC.DINT);

                                    SortSendUDTGroup.Tags.Clear();
                                    SortSendUDTGroup.Clear();
                                    SortSendUDTGroup.AddTag(Product0);
                                    SortSendUDTGroup.AddTag(Product1);
                                    SortSendUDTGroup.AddTag(Product2);
                                    SortSendUDTGroup.AddTag(Product3);
                                    SortSendUDTGroup.AddTag(Product4);
                                    SortSendUDTGroup.AddTag(Product5);
                                    SortSendUDTGroup.AddTag(LengthMap0);
                                    //SortSendUDTGroup.AddTag(Product0c);
                                    //SortSendUDTGroup.AddTag(Product1c);
                                    //SortSendUDTGroup.AddTag(Product2c);
                                    //SortSendUDTGroup.AddTag(LengthMap0c);

                                    Product0.Value = ProductMap0;
                                    Product1.Value = ProductMap1;
                                    Product2.Value = ProductMap2;
                                    Product3.Value = ProductMap3;
                                    Product4.Value = ProductMap4;
                                    Product5.Value = ProductMap5;
                                    LengthMap0.Value = LengthMap;
                                    //Product0c.Value = ProductMap0c;
                                    //Product1c.Value = ProductMap1c;
                                    //Product2c.Value = ProductMap2c;
                                    //LengthMap0c.Value = LengthMapc;

                                    try
                                    {
                                        if (MyPLCSort.IsConnected)
                                        {
                                            if (MyPLCSort.GroupWrite(SortSendUDTGroup) != Logix.ResultCode.E_SUCCESS)
                                            {
                                                UpdateRaptorCommLog("Sort Group Write SortDataSentToPLC: " + MyPLCSort.ErrorString);
                                                Succeeded = 0;
                                                Thread.Sleep(10000);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error writing SortSendUDTGroup: " + ex.Message);
                                    }

                                }
                                else if (ProductsOnly == 4) //write sort stamp tag only
                                {
                                    Tag sortstamp = new Tag("WebSortTable[" + SortID + "].Stamps", Logix.Tag.ATOMIC.DINT);
                                    
                                    SortSendUDTGroup.Tags.Clear();
                                    SortSendUDTGroup.Clear();
                                    SortSendUDTGroup.AddTag(sortstamp);

                                    sortstamp.Value = SortStamps;                                    

                                    try
                                    {
                                        if (MyPLCSort.IsConnected)
                                        {
                                            if (MyPLCSort.GroupWrite(SortSendUDTGroup) != Logix.ResultCode.E_SUCCESS)
                                            {
                                                UpdateRaptorCommLog("Sort Group Write SortDataSentToPLC: " + MyPLCSort.ErrorString);
                                                Succeeded = 0;
                                                Thread.Sleep(10000);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error writing SortSendUDTGroup: " + ex.Message);
                                    }

                                }
 
                            }
                            else //reading data from the PLC
                            {
                                Tag myUDT = new Tag("WEBSortTable[" + SortID + "]", Logix.Tag.ATOMIC.OBJECT);                            

                                if (MyPLCSort.IsConnected)
                                {
                                    if (MyPLCSort.ReadTag(myUDT) != Logix.ResultCode.E_SUCCESS)
                                    {
                                        UpdateRaptorCommLog("Error reading Sort UDT: " + MyPLCSort.ErrorString);
                                        Succeeded = 0;
                                        Thread.Sleep(10000);
                                    }
                                    else
                                    {
                                        SORTREADWRITE Sortreadwrite = (SORTREADWRITE)dtEncSort.ToType(myUDT, typeof(SORTREADWRITE));

                                        //write results into the database
                                        try
                                        {
                                            // the individual bit elements
                                            //bit = dtEncSort.ToBoolArray(Sortreadwrite.RdmWidthFlag);
                                            //bit1 = dtEncSort.ToBoolArray(Sortreadwrite.TrimFlag); 
                                            if ((Sortreadwrite.SortFlags & 1) == 1)
                                                SortTrimFlag = true;
                                            if ((Sortreadwrite.SortFlags & 2) == 2)
                                                SortRdmWidthFlag = true;
                                            if ((Sortreadwrite.SortFlags & 4) == 4)
                                                Active = true;
                                            ASCIIEncoding enc = new ASCIIEncoding();
                                            Byte[] asciidata = new byte[20];
                                            for (int i = 0; i < Sortreadwrite.Name.Len; i++)
                                                asciidata[i] = Sortreadwrite.Name.Data[i];
                                            String Name = enc.GetString(asciidata, 0, Sortreadwrite.Name.Len);
                                            SqlCommand cmd0 = new SqlCommand("update datarequestsSort set Sortlabel='" + Name.Replace("'", "''") + "',Sortsize=" + Sortreadwrite.PkgSize.ToString() + ",pkgspersort=" + Sortreadwrite.PkgsPerSort.ToString() + ",secprodid=" + Sortreadwrite.SecProdID.ToString() + ",secsize=" + Sortreadwrite.SecSize.ToString() + ",RW='" + SortRdmWidthFlag.ToString() + "',ordercount=" + Sortreadwrite.OrderCount.ToString() + ",SortStamps=" + Sortreadwrite.Stamps.ToString() + ",SortSprays=" + Sortreadwrite.Sprays.ToString() + ",TrimFlag='" + SortTrimFlag.ToString() + "',Zone1=" + Sortreadwrite.Zone1.ToString() + ",Zone2=" + Sortreadwrite.Zone2.ToString() + ",ProductMap0=" + Sortreadwrite.ProductArray.Product[0].ToString() + ",ProductMap1=" + Sortreadwrite.ProductArray.Product[1].ToString() + ",ProductMap2=" + Sortreadwrite.ProductArray.Product[2].ToString() + ",LengthMap=" + Sortreadwrite.LengthMap.ToString() + ",active='" + Active.ToString() + "' where id=" + reader["id"].ToString(), connection);
                                            cmd0.ExecuteNonQuery();
                                            SqlCommand cmd1 = new SqlCommand("execute UpdateSortData " + SortID.ToString() + ",'" + Name.Replace("'", "''") + "'," + Sortreadwrite.PkgSize.ToString() + "," + Sortreadwrite.SecProdID.ToString() + "," + Sortreadwrite.SecSize.ToString() + ",'" + SortRdmWidthFlag.ToString() + "'," + Sortreadwrite.PkgsPerSort + "," + Sortreadwrite.OrderCount + "," + Sortreadwrite.Stamps.ToString() + "," + Sortreadwrite.Sprays.ToString() + ",'" + SortTrimFlag.ToString() + "'," + Sortreadwrite.Zone1.ToString() + "," + Sortreadwrite.Zone2.ToString() + ",'" + Active + "'," + Sortreadwrite.ProductArray.Product[0].ToString() + "," + Sortreadwrite.ProductArray.Product[1].ToString() + "," + Sortreadwrite.ProductArray.Product[2].ToString() + "," + Sortreadwrite.LengthMap.ToString(), connection);
                                            cmd1.ExecuteNonQuery();

                                        }
                                        catch (Exception ex)
                                        {
                                            UpdateRaptorCommLog("Error updating Sorts data in database: " + ex.Message);
                                            Succeeded = 0;
                                        }
                                    }

                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsSort set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsSort where id <= (select max(id)-100 from DataRequestsSort)", connection);
                            cmd12.ExecuteNonQuery();

                            //process Sort by product data to the PLC, but only after successfully processing the rest of the Sort data write
                            if (Write == true) //writing data to the PLC
                            {
                                uint oldbit = 0, newbit = 0, BayOn = 0;
                                bool writetoplc;

                                if (ProductsOnly == 1 || ProductsOnly == 2)  //write the product by Sort data to the PLC
                                {
                                    SqlCommand cmd1 = new SqlCommand("select NumProducts from WEBSortSetup", connection);
                                    SqlDataReader reader1 = cmd1.ExecuteReader();
                                    reader1.Read();
                                    int NumProducts = int.Parse(reader1["NumProducts"].ToString());
                                    reader1.Close();
                                    SqlCommand cmd1p = new SqlCommand("select NumProducts=(select max(prodid) from products)", connection);
                                    SqlDataReader reader1p = cmd1p.ExecuteReader();
                                    reader1p.Read();
                                    int NumProductsp = int.Parse(reader1p["NumProducts"].ToString());
                                    reader1p.Close();

                                    SqlCommand cmdc = new SqlCommand("select activeproductlistzero from websortsetup", connection);
                                    SqlDataReader readerc = cmdc.ExecuteReader();
                                    readerc.Read();
                                    if (readerc["activeproductlistzero"].ToString() == "1" & SortID == 1)
                                    {
                                        for (int i = 1; i <= NumProductsp; i++)
                                        {
                                            //if (processed[i] == false)//if this is the first time this product has been encountered, zero out all of the sort locations for this product to start with a clean slate
                                            {
                                                //processed[i] = true;
                                                Tag sort0 = new Tag("ActiveProductList[" + i + "].SortLocation[0]", Logix.Tag.ATOMIC.DINT);
                                                Tag sort1 = new Tag("ActiveProductList[" + i + "].SortLocation[1]", Logix.Tag.ATOMIC.DINT);
                                                Tag sort2 = new Tag("ActiveProductList[" + i + "].SortLocation[2]", Logix.Tag.ATOMIC.DINT);
                                                Tag sort3 = new Tag("ActiveProductList[" + i + "].SortLocation[3]", Logix.Tag.ATOMIC.DINT);
                                                Tag sort4 = new Tag("ActiveProductList[" + i + "].SortLocation[4]", Logix.Tag.ATOMIC.DINT);
                                                Tag sort5 = new Tag("ActiveProductList[" + i + "].SortLocation[5]", Logix.Tag.ATOMIC.DINT);
                                                sort0.Value = 0;
                                                sort1.Value = 0;
                                                sort2.Value = 0;
                                                sort3.Value = 0;
                                                sort4.Value = 0;
                                                sort5.Value = 0;
                                                ProductUDTGroup.Tags.Clear();
                                                ProductUDTGroup.Clear();
                                                ProductUDTGroup.AddTag(sort0);
                                                ProductUDTGroup.AddTag(sort1);
                                                ProductUDTGroup.AddTag(sort2);
                                                ProductUDTGroup.AddTag(sort3);
                                                ProductUDTGroup.AddTag(sort4);
                                                ProductUDTGroup.AddTag(sort5);
                                                UpdateRaptorCommLog("Zeroing out product " + i.ToString());
                                                try
                                                {
                                                    if (MyPLCSort.IsConnected)
                                                    {
                                                        if (MyPLCSort.GroupWrite(ProductUDTGroup) != Logix.ResultCode.E_SUCCESS)
                                                        {
                                                            UpdateRaptorCommLog("Sort Group Write Sort by Product Map1: " + MyPLCSort.ErrorString);
                                                            Succeeded = 0;
                                                            Thread.Sleep(1000);
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    UpdateRaptorCommLog("Error writing SortSendUDTGroup - Sort by Product Map1: " + ex.Message);
                                                }
                                            }
                                        }
                                        SqlCommand cmd011a = new SqlCommand("update websortsetup set activeproductlistzero=0", connection);
                                        cmd011a.ExecuteNonQuery();
                                    }
                                    readerc.Close();
                                    

                                    for (int i = 1; i <= NumProducts; i++)
                                    {
                                        BayOn = 0;
                                        writetoplc = false;
                                         
                                        if (i < 32)
                                        {
                                            oldbit = (ProductMap0Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i)));
                                            newbit = (ProductMap0 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i)));
                                        }
                                        else if (i < 64)
                                        {
                                            oldbit = (ProductMap1Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 32)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 32)));
                                            newbit = (ProductMap1 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 32)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 32)));
                                        }
                                        else if (i < 96)
                                        {
                                            oldbit = (ProductMap2Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 64)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 64)));
                                            newbit = (ProductMap2 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 64)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 64)));
                                        }
                                        else if (i < 128)
                                        {
                                            oldbit = (ProductMap3Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 96)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 96)));
                                            newbit = (ProductMap3 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 96)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 96)));
                                        }
                                        else if (i < 160)
                                        {
                                            oldbit = (ProductMap4Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 128)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 128)));
                                            newbit = (ProductMap4 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 128)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 128)));
                                        }
                                        else if (i < 192)
                                        {
                                            oldbit = (ProductMap5Old & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 160)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 160)));
                                            newbit = (ProductMap5 & Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 160)))) / Convert.ToUInt32(Math.Pow(2, Convert.ToDouble(i - 160)));
                                        }
                                        if (oldbit == 1 && newbit == 0) //product has been removed
                                        {
                                            BayOn = 0;
                                            writetoplc = true;
                                        }
                                        else if (oldbit == 0 && newbit == 1) //product has been added
                                        {
                                            BayOn = 1;
                                            writetoplc = true;
                                        }
                                        else if (oldbit == 1 && newbit == 1) //send it anyway to keep synced
                                        {
                                            BayOn = 1;
                                            writetoplc = true;
                                        }
                                        /*else if (oldbit == 0 && newbit == 0) //send it anyway to keep synced
                                        {
                                            BayOn = 0;
                                            writetoplc = true;
                                        }*/
                                        if (SecProdID > 0 && i == SecProdID)
                                            writetoplc = false;
                                        if (writetoplc)
                                        {
                                            //now write tag to PLC
                                            Tag BayLocation = new Tag("ActiveProductList[" + i + "].SortLocation[" + (SortID / 32).ToString() + "]." + (SortID & 31).ToString(), Logix.Tag.ATOMIC.DINT);

                                            UpdateRaptorCommLog(BayLocation.Name + "," + BayOn.ToString());
                                            BayLocation.Value = BayOn;

                                            try
                                            {
                                                if (MyPLCSort.IsConnected)
                                                {
                                                    if (MyPLCSort.WriteTag(BayLocation) != Logix.ResultCode.E_SUCCESS)
                                                    {
                                                        UpdateRaptorCommLog("Sort Group Write Sort by Product Map: " + MyPLCSort.ErrorString);
                                                        Succeeded = 0;
                                                        Thread.Sleep(10000);
                                                    }
                                                    System.Threading.Thread.Sleep(10);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                UpdateRaptorCommLog("Error writing SortSendUDTGroup - Sort by Product Map: " + ex.Message);
                                            }
                                            
                                        }
                                    } //NumProducts

                                }
                            }
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsSort table: " + ex.Message);
                    }                    
                }
                Thread.Sleep(SortDataToPLCScanRate);
            }
        }

        public static void ProductDataSentToPLC()
        {
            int Succeeded;
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCProduct.IsConnected)
                {
                    while (MyPLCProduct.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Product " + MyPLCProduct.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Product Connection to PLC Re-established Successfully!");
                }
                
                if ((DataRequests & 4) == 4)
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsProduct with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int ProductID = int.Parse(reader["ProductID"].ToString());
                            bool PActive = bool.Parse(reader["Active"].ToString());
                            uint PThickID = uint.Parse(reader["ThicknessID"].ToString());
                            uint PWidthID = uint.Parse(reader["WidthID"].ToString());
                            uint PGradeID = uint.Parse(reader["GradeID"].ToString());
                            uint PMoistureID = uint.Parse(reader["MoistureID"].ToString());
                            uint PSpecID = uint.Parse(reader["SpecID"].ToString());
                            uint PSpecialX = uint.Parse(reader["SpecialX"].ToString());
                            uint PSpecialY = uint.Parse(reader["SpecialY"].ToString());
                            uint PSpecialZ = uint.Parse(reader["SpecialY"].ToString());
                            //uint PSixteen = uint.Parse(reader["Sixteen"].ToString());
                            //uint PTwenty = uint.Parse(reader["Twenty"].ToString());
                            uint ProductMap;

                            //ProductMap = PSpecialZ + (PSpecialY << 4) + (PSpecialX << 8) + (PMoistureID << 12) + (PSpecID << 16) + (PGradeID << 20) + (PWidthID << 24) + (PThickID << 28);
                            ProductMap = PSpecialZ + (PSpecialY << 4) + (PSpecialX << 8) + (PMoistureID << 12) + (PGradeID << 16) + (PWidthID << 24) + (PThickID << 28); 

                            if (Write == true) //writing data to the PLC
                            {
                                Tag Product = new Tag("WebSortProductList[" + ProductID + "]", Logix.Tag.ATOMIC.DINT);
                                //Tag ProductSpecial = new Tag("Program:WEBSort.LengthOverride[" + ProductID + "].Sixteen", Logix.Tag.ATOMIC.DINT);
                                //Tag ProductSpecial1 = new Tag("Program:WEBSort.LengthOverride[" + ProductID + "].Twenty", Logix.Tag.ATOMIC.DINT);
                                
                                //ProductUDTGroup.Tags.Clear();
                                //ProductUDTGroup.Clear();
                                //ProductUDTGroup.AddTag(ProductSpecial);
                                //ProductUDTGroup.AddTag(ProductSpecial1);

                                //ProductSpecial.Value = PSixteen.ToString();
                                //ProductSpecial1.Value = PTwenty.ToString();
                                Product.Value = ProductMap;
                                
                                try
                                {
                                    if (MyPLCProduct.IsConnected)
                                    {
                                        MyPLCProduct.WriteTag(Product);
                                        //MyPLCProduct.GroupWrite(ProductUDTGroup);
                                    }
                                }
                                catch 
                                {
                                    UpdateRaptorCommLog("Error writing Product: " + MyPLCProduct.ErrorString);
                                }
                                ///if (ResultCode.QUAL_GOOD != test.QualityCode)
                                if (ResultCode.QUAL_GOOD != Product.QualityCode)
                                {
                                    UpdateRaptorCommLog("Product Write: " + Product.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag Product = new Tag("WebSortProductList[" + ProductID + "]", Logix.Tag.ATOMIC.DINT);
                                //Tag ProductSpecial = new Tag("Program:WEBSort.LengthOverride[" + ProductID + "].Sixteen", Logix.Tag.ATOMIC.DINT);
                                //Tag ProductSpecial1 = new Tag("Program:WEBSort.LengthOverride[" + ProductID + "].Twenty", Logix.Tag.ATOMIC.DINT);
                                int ThicknessID,WidthID,GradeID,MoistureID,SpecID,SpecialX,SpecialY,SpecialZ;

                                ProductUDTGroup.Tags.Clear();
                                ProductUDTGroup.Clear();
                                //ProductUDTGroup.AddTag(ProductSpecial);
                                //ProductUDTGroup.AddTag(ProductSpecial1);                                       
                                try
                                {
                                    if (MyPLCProduct.IsConnected)
                                    {
                                        MyPLCProduct.ReadTag(Product);
                                        //MyPLCProduct.GroupRead(ProductUDTGroup);
                                    }
                                }
                                catch 
                                {
                                    UpdateRaptorCommLog("Error reading Product: " + MyPLCProduct.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == Product.QualityCode)
                                {
                                    SpecialZ = Convert.ToInt32(Product.Value) & 15;
                                    SpecialY = (Convert.ToInt32(Product.Value) >> 4) & 15;
                                    SpecialX = (Convert.ToInt32(Product.Value) >> 8) & 15;
                                    MoistureID = (Convert.ToInt32(Product.Value) >> 12) & 15;
                                    SpecID = (Convert.ToInt32(Product.Value) >> 16) & 15;
                                    GradeID = (Convert.ToInt32(Product.Value) >> 20) & 15;
                                    WidthID = (Convert.ToInt32(Product.Value) >> 24) & 15;
                                    ThicknessID = (Convert.ToInt32(Product.Value) >> 28) & 15;
                                    
                                    //UpdateRaptorCommLog(Product.Value.ToString());
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsProduct set ThicknessID=" + ThicknessID.ToString() + ",WidthID=" + WidthID.ToString() + ",GradeID=" + GradeID.ToString() + ",SpecID=" + SpecID.ToString() + ",MoistureID=" + MoistureID.ToString() + ",SpecialX=" + SpecialX.ToString() + ",SpecialY=" + SpecialY.ToString() + ",SpecialZ=" + SpecialZ.ToString() + " where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("if (select deleted from products where prodid=" + ProductID + ")=0 or (select deleted from products where prodid=" + ProductID + ") is null begin delete from products where prodid=" + ProductID + " insert into Products select " + ProductID + ",0,1,convert(varchar,thicknessnominal) + ' x ' + convert(varchar,widthnominal)," + GradeID.ToString() + "," + MoistureID.ToString() + "," + SpecID.ToString() + ",thicknessnominal,thicknessminimum,thicknessmaximum,widthnominal,widthminimum,widthmaximum," + ThicknessID.ToString() + "," + WidthID.ToString() + " from thickness,width where thicknessnominal>0 and widthnominal>0 and thickness.id=" + ThicknessID + " and width.id=" + WidthID + " end", connection);
                                        //UpdateRaptorCommLog(cmd1.CommandText);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating Products data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Product Read: " + Product.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsProduct set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsProduct where id <= (select max(id)-100 from DataRequestsProduct)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();
                    }
                    catch(Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsProduct table: " + ex.Message);   
                    }                   
                }
                Thread.Sleep(ProductDataToPLCScanRate);
            }
        }

        public static void ThicknessDataSentToPLC()
        {
            int Succeeded;
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCThickness.IsConnected)
                {
                    while (MyPLCThickness.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Thickness " + MyPLCThickness.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Thickness Connection to PLC Re-established Successfully!");
                }
                
                if ((DataRequests & 16) == 16)
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsThickness with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int ThicknessID = int.Parse(reader["ThickID"].ToString());
                            double PThickMin = double.Parse(reader["ThickMin"].ToString());
                            double PThickMax = double.Parse(reader["ThickMax"].ToString());
                            double PThickNom = double.Parse(reader["ThickNom"].ToString());
                            
                            if (Write == true) //writing data to the PLC
                            {
                                Tag ThickMin = new Tag("WebSortThicknessTable[0,0," + ThicknessID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag ThickMax = new Tag("WebSortThicknessTable[0,1," + ThicknessID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag ThickNom = new Tag("WebSortThicknessTable[1,0," + ThicknessID + "]", Logix.Tag.ATOMIC.REAL);
                                
                                ThicknessSendUDTGroup.Tags.Clear();
                                ThicknessSendUDTGroup.Clear();
                                ThicknessSendUDTGroup.AddTag(ThickMin);
                                ThicknessSendUDTGroup.AddTag(ThickMax);
                                ThicknessSendUDTGroup.AddTag(ThickNom);
                                
                                ThickMin.Value = PThickMin.ToString();
                                ThickMax.Value = PThickMax.ToString();
                                ThickNom.Value = PThickNom.ToString();
                                
                                try
                                {
                                    if (MyPLCThickness.IsConnected)
                                        MyPLCThickness.GroupWrite(ThicknessSendUDTGroup);
                                        
                                }
                                catch 
                                {
                                    UpdateRaptorCommLog("Error writing ThicknessSendUDTGroup: " + MyPLCThickness.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != ThickMin.QualityCode)
                                {
                                    UpdateRaptorCommLog("Thickness Group Write ThicknessDataSentToPLC: " + ThickMin.ErrorString);
                                    
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag ThickMin = new Tag("WebSortThicknessTable[0,0," + ThicknessID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag ThickMax = new Tag("WebSortThicknessTable[0,1," + ThicknessID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag ThickNom = new Tag("WebSortThicknessTable[1,0," + ThicknessID + "]", Logix.Tag.ATOMIC.REAL);

                                ThicknessSendUDTGroup.Tags.Clear();
                                ThicknessSendUDTGroup.Clear();
                                ThicknessSendUDTGroup.AddTag(ThickMin);
                                ThicknessSendUDTGroup.AddTag(ThickMax);
                                ThicknessSendUDTGroup.AddTag(ThickNom);
                                
                                try
                                {
                                    if (MyPLCThickness.IsConnected)
                                        MyPLCThickness.GroupRead(ThicknessSendUDTGroup);
                                }
                                catch 
                                {
                                    UpdateRaptorCommLog("Error reading ThicknessSendUDTGroup: " + MyPLCThickness.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == ThickMin.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsThickness set ThickMin=" + ThickMin.Value.ToString() + ",ThickMax=" + ThickMax.Value.ToString() + ",ThickNom=" + ThickNom.Value.ToString() + " where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("delete from thickness where id=" + ThicknessID + " insert into Thickness select " + ThicknessID + "," + ThickNom.Value.ToString() + "," + ThickMin.Value.ToString() + "," + ThickMax.Value.ToString(), connection);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating Thickness data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Thickness Group Read ThicknessDataSentToPLC: " + ThickMin.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsThickness set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsThickness where id <= (select max(id)-100 from DataRequestsThickness)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsThickness table: " + ex.Message);
                    }                    
                }
                Thread.Sleep(ThicknessDataToPLCScanRate);
            }
        }

        public static void WidthDataSentToPLC()
        {
            int Succeeded;
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCWidth.IsConnected)
                {
                    while (MyPLCWidth.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Width " + MyPLCWidth.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Width Connection to PLC Re-established Successfully!");
                }
                
                if ((DataRequests & 32) == 32)
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsWidth with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int WidthID = int.Parse(reader["WidthID"].ToString());
                            double PWidthMin = double.Parse(reader["WidthMin"].ToString());
                            double PWidthMax = double.Parse(reader["WidthMax"].ToString());
                            double PWidthNom = double.Parse(reader["WidthNom"].ToString());

                            if (Write == true) //writing data to the PLC
                            {
                                Tag WidthMin = new Tag("WebSortWidthTable[0,0," + WidthID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag WidthMax = new Tag("WebSortWidthTable[0,1," + WidthID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag WidthNom = new Tag("WebSortWidthTable[1,0," + WidthID + "]", Logix.Tag.ATOMIC.REAL);

                                WidthSendUDTGroup.Tags.Clear();
                                WidthSendUDTGroup.Clear();
                                WidthSendUDTGroup.AddTag(WidthMin);
                                WidthSendUDTGroup.AddTag(WidthMax);
                                WidthSendUDTGroup.AddTag(WidthNom);

                                WidthMin.Value = PWidthMin.ToString();
                                WidthMax.Value = PWidthMax.ToString();
                                WidthNom.Value = PWidthNom.ToString();

                                try
                                {
                                    if (MyPLCWidth.IsConnected)
                                        MyPLCWidth.GroupWrite(WidthSendUDTGroup);

                                }
                                catch 
                                {
                                    UpdateRaptorCommLog("Error writing WidthSendUDTGroup: " + MyPLCWidth.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != WidthMin.QualityCode)
                                {
                                    UpdateRaptorCommLog("Width Group Write WidthDataSentToPLC: " + WidthMin.ErrorString);

                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag WidthMin = new Tag("WebSortWidthTable[0,0," + WidthID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag WidthMax = new Tag("WebSortWidthTable[0,1," + WidthID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag WidthNom = new Tag("WebSortWidthTable[1,0," + WidthID + "]", Logix.Tag.ATOMIC.REAL);

                                WidthSendUDTGroup.Tags.Clear();
                                WidthSendUDTGroup.Clear();
                                WidthSendUDTGroup.AddTag(WidthMin);
                                WidthSendUDTGroup.AddTag(WidthMax);
                                WidthSendUDTGroup.AddTag(WidthNom);

                                try
                                {
                                    if (MyPLCWidth.IsConnected)
                                        MyPLCWidth.GroupRead(WidthSendUDTGroup);
                                }
                                catch 
                                {
                                    UpdateRaptorCommLog("Error reading WidthSendUDTGroup: " + MyPLCWidth.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == WidthMin.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsWidth set WidthMin=" + WidthMin.Value.ToString() + ",WidthMax=" + WidthMax.Value.ToString() + ",WidthNom=" + WidthNom.Value.ToString() + " where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("delete from width where id=" + WidthID + " insert into Width select " + WidthID + "," + WidthNom.Value.ToString() + "," + WidthMin.Value.ToString() + "," + WidthMax.Value.ToString(), connection);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating Width data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Width Group Read WidthDataSentToPLC: " + WidthMin.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsWidth set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsWidth where id <= (select max(id)-100 from DataRequestsWidth)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsWidth table: " + ex.Message);
                    }                    
                }
                Thread.Sleep(WidthDataToPLCScanRate);
            }
        }

        public static void LengthDataSentToPLC()
        {
            int Succeeded;
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCLength.IsConnected)
                {
                    while (MyPLCLength.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Length " + MyPLCLength.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Length Connection to PLC Re-established Successfully!");
                }

                if ((DataRequests & 64) == 64)
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsLength with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int LengthID = int.Parse(reader["LengthID"].ToString());
                            double PLengthMin = double.Parse(reader["LengthNom"].ToString());
                            double PLengthMax = double.Parse(reader["LengthNom"].ToString());
                            double PLengthNomInches = double.Parse(reader["LengthNom"].ToString());
                            double PLengthNomFeet = (PLengthNomInches / 12);


                            if (Write == true) //writing data to the PLC
                            {
                                Tag LengthMin = new Tag("WebSortLengthTable[0,0," + LengthID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag LengthMax = new Tag("WebSortLengthTable[0,1," + LengthID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag LengthNomFeet = new Tag("WebSortLengthTable[1,0," + LengthID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag LengthNomInches = new Tag("WebSortLengthTable[1,1," + LengthID + "]", Logix.Tag.ATOMIC.REAL);

                                LengthSendUDTGroup.Tags.Clear();
                                LengthSendUDTGroup.Clear();
                                LengthSendUDTGroup.AddTag(LengthMin);
                                LengthSendUDTGroup.AddTag(LengthMax);
                                LengthSendUDTGroup.AddTag(LengthNomFeet);
                                LengthSendUDTGroup.AddTag(LengthNomInches);

                                LengthMin.Value = PLengthMin.ToString();
                                LengthMax.Value = PLengthMax.ToString();
                                LengthNomFeet.Value = PLengthNomFeet.ToString();
                                LengthNomInches.Value = PLengthNomInches.ToString();

                                try
                                {
                                    if (MyPLCLength.IsConnected)
                                        MyPLCLength.GroupWrite(LengthSendUDTGroup);

                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error writing LengthSendUDTGroup: " + MyPLCLength.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != LengthMin.QualityCode)
                                {
                                    UpdateRaptorCommLog("Length Group Write LengthDataSentToPLC: " + LengthMin.ErrorString);

                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag LengthMin = new Tag("WebSortLengthTable[0,0," + LengthID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag LengthMax = new Tag("WebSortLengthTable[0,1," + LengthID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag LengthNomFeet = new Tag("WebSortLengthTable[1,0," + LengthID + "]", Logix.Tag.ATOMIC.REAL);
                                Tag LengthNomInches = new Tag("WebSortLengthTable[1,1," + LengthID + "]", Logix.Tag.ATOMIC.REAL);

                                LengthSendUDTGroup.Tags.Clear();
                                LengthSendUDTGroup.Clear();
                                LengthSendUDTGroup.AddTag(LengthMin);
                                LengthSendUDTGroup.AddTag(LengthMax);
                                LengthSendUDTGroup.AddTag(LengthNomFeet);
                                LengthSendUDTGroup.AddTag(LengthNomInches);

                                try
                                {
                                    if (MyPLCLength.IsConnected)
                                        MyPLCLength.GroupRead(LengthSendUDTGroup);
                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error reading LengthSendUDTGroup: " + MyPLCLength.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == LengthMin.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsLength set LengthMin=" + LengthMin.Value.ToString() + ",LengthMax=" + LengthMax.Value.ToString() + ",LengthNom=" + LengthNomInches.Value.ToString() + " where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("delete from Lengths where id=" + LengthID + " insert into Lengths select " + LengthID + ",'" + Convert.ToInt32(LengthNomFeet.Value).ToString() + "'''," + LengthNomInches.Value.ToString() + "," + LengthMin.Value.ToString() + "," + LengthMax.Value.ToString(), connection);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating Length data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Length Group Read LengthDataSentToPLC: " + LengthMin.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsLength set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsLength where id <= (select max(id)-100 from DataRequestsLength)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsLength table: " + ex.Message);
                    }
                }
                if ((DataRequests & 2048) == 2048)
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsPETLength with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int PETLengthID = int.Parse(reader["PETLengthID"].ToString());
                            int PSawIndex = int.Parse(reader["SawIndex"].ToString());
                            double PLengthNom = double.Parse(reader["LengthNom"].ToString());
                            double PPETPosition = double.Parse(reader["PETPosition"].ToString());


                            if (Write == true) //writing data to the PLC
                            {
                                Tag SawIndex = new Tag("WebSortPETTable[" + PETLengthID + "].SawIndex", Logix.Tag.ATOMIC.DINT);
                                Tag LengthNom = new Tag("WebSortPETTable[" + PETLengthID + "].Length", Logix.Tag.ATOMIC.REAL);
                                Tag PETPosition = new Tag("WebSortPETTable[" + PETLengthID + "].PETPosition", Logix.Tag.ATOMIC.REAL);

                                LengthSendUDTGroup.Tags.Clear();
                                LengthSendUDTGroup.Clear();
                                LengthSendUDTGroup.AddTag(SawIndex);
                                LengthSendUDTGroup.AddTag(LengthNom);
                                LengthSendUDTGroup.AddTag(PETPosition);

                                SawIndex.Value = PSawIndex.ToString();
                                LengthNom.Value = PLengthNom.ToString();
                                PETPosition.Value = PPETPosition.ToString();

                                try
                                {
                                    if (MyPLCLength.IsConnected)
                                        MyPLCLength.GroupWrite(LengthSendUDTGroup);

                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error writing LengthSendUDTGroup: " + MyPLCLength.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != SawIndex.QualityCode)
                                {
                                    UpdateRaptorCommLog("Length Group Write LengthDataSentToPLC: " + SawIndex.ErrorString);

                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag SawIndex = new Tag("WebSortPETTable[" + PETLengthID + "].SawIndex", Logix.Tag.ATOMIC.DINT);
                                Tag LengthNom = new Tag("WebSortPETTable[" + PETLengthID + "].Length", Logix.Tag.ATOMIC.REAL);
                                Tag PETPosition = new Tag("WebSortPETTable[" + PETLengthID + "].PETPosition", Logix.Tag.ATOMIC.REAL);

                                LengthSendUDTGroup.Tags.Clear();
                                LengthSendUDTGroup.Clear();
                                LengthSendUDTGroup.AddTag(SawIndex);
                                LengthSendUDTGroup.AddTag(LengthNom);
                                LengthSendUDTGroup.AddTag(PETPosition);

                                try
                                {
                                    if (MyPLCLength.IsConnected)
                                        MyPLCLength.GroupRead(LengthSendUDTGroup);
                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error reading PETLengthSendUDTGroup: " + MyPLCLength.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == SawIndex.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsPETLength set SawIndex=" + SawIndex.Value.ToString() + ",LengthNom=" + LengthNom.Value.ToString() + ",PETPosition=" + PETPosition.Value.ToString() + " where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("delete from PETLengths where id=" + PETLengthID + " insert into PETLengths select " + PETLengthID + ",'" + (LengthNom.Value).ToString() + "\"'," + SawIndex.Value.ToString() + "," + LengthNom.Value.ToString() + "," + PETPosition.Value.ToString(), connection);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating PETLength data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("PETLength Group Read LengthDataSentToPLC: " + SawIndex.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsPETLength set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsPETLength where id <= (select max(id)-100 from DataRequestsPETLength)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsPETLength table: " + ex.Message);
                    }
                }
                Thread.Sleep(LengthDataToPLCScanRate);
            }
        }

        public static void MoistureDataSentToPLC()
        {
            int Succeeded;
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCMoisture.IsConnected)
                {
                    while (MyPLCMoisture.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Moisture " + MyPLCMoisture.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Moisture Connection to PLC Re-established Successfully!");
                }

                if ((DataRequests & 512) == 512)
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsMoisture with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int MoistureID = int.Parse(reader["MoistureID"].ToString());
                            int PMoistureMin = int.Parse(reader["MoistureMin"].ToString());
                            int PMoistureMax = int.Parse(reader["MoistureMax"].ToString());
                            

                            if (Write == true) //writing data to the PLC
                            {
                                Tag MoistureMin = new Tag("WebSortMoistureTable[0,0," + MoistureID + "]", Logix.Tag.ATOMIC.INT);
                                Tag MoistureMax = new Tag("WebSortMoistureTable[0,1," + MoistureID + "]", Logix.Tag.ATOMIC.INT);
                                
                                MoistureSendUDTGroup.Tags.Clear();
                                MoistureSendUDTGroup.Clear();
                                MoistureSendUDTGroup.AddTag(MoistureMin);
                                MoistureSendUDTGroup.AddTag(MoistureMax);
                               
                                MoistureMin.Value = PMoistureMin.ToString();
                                MoistureMax.Value = PMoistureMax.ToString();
                                
                                try
                                {
                                    if (MyPLCMoisture.IsConnected)
                                        MyPLCMoisture.GroupWrite(MoistureSendUDTGroup);

                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error writing MoistureSendUDTGroup: " + MyPLCMoisture.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != MoistureMin.QualityCode)
                                {
                                    UpdateRaptorCommLog("Moisture Group Write MoistureDataSentToPLC: " + MoistureMin.ErrorString);

                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag MoistureMin = new Tag("WebSortMoistureTable[0,0," + MoistureID + "]", Logix.Tag.ATOMIC.INT);
                                Tag MoistureMax = new Tag("WebSortMoistureTable[0,1," + MoistureID + "]", Logix.Tag.ATOMIC.INT);
                                
                                MoistureSendUDTGroup.Tags.Clear();
                                MoistureSendUDTGroup.Clear();
                                MoistureSendUDTGroup.AddTag(MoistureMin);
                                MoistureSendUDTGroup.AddTag(MoistureMax);
                                
                                try
                                {
                                    if (MyPLCMoisture.IsConnected)
                                        MyPLCMoisture.GroupRead(MoistureSendUDTGroup);
                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error reading MoistureSendUDTGroup: " + MyPLCMoisture.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == MoistureMin.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsMoisture set MoistureMin=" + MoistureMin.Value.ToString() + ",MoistureMax=" + MoistureMax.Value.ToString()+ " where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("delete from Moistures where id=" + MoistureID + " insert into Moistures select " + MoistureID + "," + MoistureMin.Value.ToString() + "," + MoistureMax.Value.ToString(), connection);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating Moisture data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Moisture Group Read MoistureDataSentToPLC: " + MoistureMin.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsMoisture set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsMoisture where id <= (select max(id)-100 from DataRequestsMoisture)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsMoisture table: " + ex.Message);
                    }
                }
                
                Thread.Sleep(MoistureDataToPLCScanRate);
            }
        }
        public static void GradeDataSentToPLC()
        {
            int Succeeded;
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCGrade.IsConnected)
                {
                    while (MyPLCGrade.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Grade " + MyPLCGrade.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Grade Connection to PLC Re-established Successfully!");
                }

                if ((DataRequests & 8) == 8)  //grade map
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsGrade with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int GradeID = int.Parse(reader["GradeID"].ToString());
                            int gGradeIDX = int.Parse(reader["GradeIDX"].ToString());
                            int gGradestamps = int.Parse(reader["GradeStamps"].ToString());
                            
                            if (Write == true) //writing data to the PLC
                            {
                                Tag GradeIDX = new Tag("WebSortGradeTable[" + GradeID + "].Button", Logix.Tag.ATOMIC.INT);
                                Tag GradeStamps = new Tag("WebSortGradeTable[" + GradeID + "].Stamps", Logix.Tag.ATOMIC.DINT);
                                
                                GradeSendUDTGroup.Tags.Clear();
                                GradeSendUDTGroup.Clear();
                                GradeSendUDTGroup.AddTag(GradeIDX);
                                GradeSendUDTGroup.AddTag(GradeStamps);

                                GradeIDX.Value = gGradeIDX.ToString();
                                GradeStamps.Value = gGradestamps.ToString();
                                
                                try
                                {
                                    if (MyPLCGrade.IsConnected)
                                        MyPLCGrade.GroupWrite(GradeSendUDTGroup);

                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error writing GradeSendUDTGroup: " + MyPLCGrade.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != GradeIDX.QualityCode)
                                {
                                    UpdateRaptorCommLog("Grade Write GradeDataSentToPLC: " + GradeIDX.ErrorString);

                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag GradeIDX = new Tag("WebSortGradeTable[" + GradeID + "].Button", Logix.Tag.ATOMIC.INT);
                                Tag GradeStamps = new Tag("WebSortGradeTable[" + GradeID + "].Stamps", Logix.Tag.ATOMIC.DINT);

                                GradeSendUDTGroup.Tags.Clear();
                                GradeSendUDTGroup.Clear();
                                GradeSendUDTGroup.AddTag(GradeIDX);
                                GradeSendUDTGroup.AddTag(GradeStamps);
                                
                                try
                                {
                                    if (MyPLCGrade.IsConnected)
                                        MyPLCGrade.GroupRead(GradeSendUDTGroup);
                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error reading GradeSendUDTGroup: " + MyPLCGrade.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == GradeIDX.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsGrade set GradeIDX=" + GradeIDX.Value.ToString() + ",GradeStamps=" + GradeStamps.Value.ToString() + " where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("update GradeMatrix set WEBSortGradeID = " + GradeIDX.Value.ToString() + ",GradeStamps=" + GradeStamps.Value.ToString() + " where recipeid=(select recipeid from recipes where online=1) and PLCGradeID=" + GradeID, connection);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating Grade data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Grade Read GradeDataSentToPLC: " + GradeIDX.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsGrade set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsGrade where id <= (select max(id)-100 from DataRequestsGrade)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsGrade table: " + ex.Message);
                    }
                }
                else if ((DataRequests & 4096) == 4096)  //grader test
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsGradertest with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int gGraderMap = int.Parse(reader["Graders"].ToString());
                            int gGradeMap = int.Parse(reader["Grades"].ToString());
                            int gThicknessMap = int.Parse(reader["Thickness"].ToString());
                            int gWidthMap = int.Parse(reader["Width"].ToString());
                            int gLengthMap = int.Parse(reader["Lengths"].ToString());
                            Int16 gBayID = Int16.Parse(reader["BayID"].ToString());
                            Int16 gSampleSize = Int16.Parse(reader["SampleSize"].ToString());
                            Int16 gInterval = Int16.Parse(reader["Interval"].ToString());
                            Int16 gSamplesRemaining = Int16.Parse(reader["SamplesRemaining"].ToString());
                            bool gActive = bool.Parse(reader["Active"].ToString());
                            bool gStamp = bool.Parse(reader["Stamp"].ToString());
                            bool gTrim = bool.Parse(reader["Trim"].ToString());

                            if (Write == true) //writing data to the PLC
                            {
                                Tag GraderMap = new Tag("GradeCheck.GraderMap", Logix.Tag.ATOMIC.DINT);
                                Tag GradeMap = new Tag("GradeCheck.GradeMap", Logix.Tag.ATOMIC.DINT);
                                Tag ThicknessMap = new Tag("GradeCheck.ThicknessMap", Logix.Tag.ATOMIC.DINT);
                                Tag WidthMap = new Tag("GradeCheck.WidthMap", Logix.Tag.ATOMIC.DINT);
                                Tag LengthMap = new Tag("GradeCheck.LengthMap", Logix.Tag.ATOMIC.DINT);
                                Tag BayID = new Tag("GradeCheck.BayID", Logix.Tag.ATOMIC.SINT);
                                Tag SampleSize = new Tag("GradeCheck.SampleSize", Logix.Tag.ATOMIC.INT);
                                Tag Interval = new Tag("GradeCheck.Interval", Logix.Tag.ATOMIC.SINT);
                                Tag SamplesRemaining = new Tag("GradeCheck.SamplesRemaining", Logix.Tag.ATOMIC.INT);
                                Tag Active = new Tag("GradeCheck.TestActive", Logix.Tag.ATOMIC.BOOL);
                                Tag Stamp = new Tag("GradeCheck.StampTestBoards", Logix.Tag.ATOMIC.BOOL);
                                Tag Trim = new Tag("GradeCheck.TrimTestBoards", Logix.Tag.ATOMIC.BOOL);


                                GradeSendUDTGroup.Tags.Clear();
                                GradeSendUDTGroup.Clear();
                                GradeSendUDTGroup.AddTag(GraderMap);
                                GradeSendUDTGroup.AddTag(GradeMap);
                                GradeSendUDTGroup.AddTag(ThicknessMap);
                                GradeSendUDTGroup.AddTag(WidthMap);
                                GradeSendUDTGroup.AddTag(LengthMap);
                                GradeSendUDTGroup.AddTag(BayID);
                                GradeSendUDTGroup.AddTag(SampleSize);
                                GradeSendUDTGroup.AddTag(Interval);
                                GradeSendUDTGroup.AddTag(SamplesRemaining);
                                GradeSendUDTGroup.AddTag(Active);
                                GradeSendUDTGroup.AddTag(Stamp);
                                GradeSendUDTGroup.AddTag(Trim);

                                GraderMap.Value = gGraderMap.ToString();
                                GradeMap.Value = gGradeMap.ToString();
                                ThicknessMap.Value = gThicknessMap.ToString();
                                WidthMap.Value = gWidthMap.ToString();
                                LengthMap.Value = gLengthMap.ToString();
                                BayID.Value = gBayID.ToString();
                                SampleSize.Value = gSampleSize.ToString();
                                Interval.Value = gInterval.ToString();
                                SamplesRemaining.Value = gSamplesRemaining.ToString();
                                Active.Value = gActive.ToString();
                                Stamp.Value = gStamp.ToString();
                                Trim.Value = gTrim.ToString();

                                try
                                {
                                    if (MyPLCGrade.IsConnected)
                                        MyPLCGrade.GroupWrite(GradeSendUDTGroup);

                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error writing Grader Test GradeSendUDTGroup: " + MyPLCGrade.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != GraderMap.QualityCode)
                                {
                                    UpdateRaptorCommLog("Grade Write Grader Test GradeDataSentToPLC: " + GraderMap.ErrorString);

                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag GraderMap = new Tag("GradeCheck.GraderMap", Logix.Tag.ATOMIC.DINT);
                                Tag GradeMap = new Tag("GradeCheck.GradeMap", Logix.Tag.ATOMIC.DINT);
                                Tag ThicknessMap = new Tag("GradeCheck.ThicknessMap", Logix.Tag.ATOMIC.DINT);
                                Tag WidthMap = new Tag("GradeCheck.WidthMap", Logix.Tag.ATOMIC.DINT);
                                Tag LengthMap = new Tag("GradeCheck.LengthMap", Logix.Tag.ATOMIC.DINT);
                                Tag BayID = new Tag("GradeCheck.BayID", Logix.Tag.ATOMIC.INT);
                                Tag SampleSize = new Tag("GradeCheck.SampleSize", Logix.Tag.ATOMIC.INT);
                                Tag Interval = new Tag("GradeCheck.Interval", Logix.Tag.ATOMIC.SINT);
                                Tag SamplesRemaining = new Tag("GradeCheck.SamplesRemaining", Logix.Tag.ATOMIC.INT);
                                Tag Active = new Tag("GradeCheck.TestActive", Logix.Tag.ATOMIC.BOOL);
                                Tag Stamp = new Tag("GradeCheck.StampTestBoards", Logix.Tag.ATOMIC.BOOL);
                                Tag Trim = new Tag("GradeCheck.TrimTestBoards", Logix.Tag.ATOMIC.BOOL);

                                GradeSendUDTGroup.Tags.Clear();
                                GradeSendUDTGroup.Clear();
                                GradeSendUDTGroup.AddTag(GraderMap);
                                GradeSendUDTGroup.AddTag(GradeMap);
                                GradeSendUDTGroup.AddTag(ThicknessMap);
                                GradeSendUDTGroup.AddTag(WidthMap);
                                GradeSendUDTGroup.AddTag(LengthMap);
                                GradeSendUDTGroup.AddTag(BayID);
                                GradeSendUDTGroup.AddTag(SampleSize);
                                GradeSendUDTGroup.AddTag(Interval);
                                GradeSendUDTGroup.AddTag(SamplesRemaining);
                                GradeSendUDTGroup.AddTag(Active);
                                GradeSendUDTGroup.AddTag(Stamp);
                                GradeSendUDTGroup.AddTag(Trim);

                                try
                                {
                                    if (MyPLCGrade.IsConnected)
                                        MyPLCGrade.GroupRead(GradeSendUDTGroup);
                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error reading Grader Test GradeSendUDTGroup: " + MyPLCGrade.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == GraderMap.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsGradertest set Graders=" + GraderMap.Value.ToString() + ",Thickness=" + ThicknessMap.Value.ToString() + ",Width=" + WidthMap.Value.ToString() + ",Grades=" + GradeMap.Value.ToString() + ",Lengths=" + LengthMap.Value.ToString() + ",BayID=" + BayID.Value.ToString() + ",SampleSize=" + SampleSize.Value.ToString() + ",SamplesRemaining=" + SamplesRemaining.Value.ToString() + ",Interval=" + Interval.Value.ToString() + ",Active='" + Active.Value.ToString() + "',Stamp='" + Stamp.Value.ToString() + "',Trim='" + Trim.Value.ToString() + "' where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("update Gradertest set Graders=" + GraderMap.Value.ToString() + ",Thickness=" + ThicknessMap.Value.ToString() + ",Width=" + WidthMap.Value.ToString() + ",Grades=" + GradeMap.Value.ToString() + ",Lengths=" + LengthMap.Value.ToString() + ",BayID=" + BayID.Value.ToString() + ",SampleSize=" + SampleSize.Value.ToString() + ",SamplesRemaining=" + SamplesRemaining.Value.ToString() + ",Interval=" + Interval.Value.ToString() + ",Active='" + Active.Value.ToString() + "',Stamp='" + Stamp.Value.ToString() + "',Trim='" + Trim.Value.ToString() + "'", connection);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating Grader Test data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Read Grader Test GradeDataSentToPLC: " + GraderMap.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsGradertest set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsGradertest where id <= (select max(id)-100 from DataRequestsGradertest)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsGradertest table: " + ex.Message);
                    }
                }
                /*else if ((DataRequests & 262144) == 262144)  //msr sample
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsMSRtest with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int gTestID = int.Parse(reader["TestID"].ToString());
                            int gGradeMap = int.Parse(reader["Grades"].ToString());
                            int gLengthMap = int.Parse(reader["Lengths"].ToString());
                            Int16 gSampleSize = Int16.Parse(reader["SampleSize"].ToString());
                            Int16 gInterval = Int16.Parse(reader["Interval"].ToString());
                            Int16 gSamplesRemaining = Int16.Parse(reader["SamplesRemaining"].ToString());
                            bool gActive = bool.Parse(reader["Active"].ToString());
                            bool gStamp = bool.Parse(reader["Stamp"].ToString());
                            bool gTrim = bool.Parse(reader["Trim"].ToString());

                            if (Write == true) //writing data to the PLC
                            {
                                Tag GradeMap = new Tag("MSRCheck" + gTestID.ToString() + ".GradeMap", Logix.Tag.ATOMIC.DINT);
                                Tag LengthMap = new Tag("MSRCheck" + gTestID.ToString() + ".LengthMap", Logix.Tag.ATOMIC.DINT);
                                Tag SampleSize = new Tag("MSRCheck" + gTestID.ToString() + ".SampleSize", Logix.Tag.ATOMIC.INT);
                                Tag Interval = new Tag("MSRCheck" + gTestID.ToString() + ".Interval", Logix.Tag.ATOMIC.SINT);
                                Tag SamplesRemaining = new Tag("MSRCheck" + gTestID.ToString() + ".SamplesRemaining", Logix.Tag.ATOMIC.INT);
                                Tag Active = new Tag("MSRCheck" + gTestID.ToString() + ".TestActive", Logix.Tag.ATOMIC.BOOL);
                                Tag Stamp = new Tag("MSRCheck" + gTestID.ToString() + ".StampTestBoards", Logix.Tag.ATOMIC.BOOL);
                                Tag Trim = new Tag("MSRCheck" + gTestID.ToString() + ".TrimTestBoards", Logix.Tag.ATOMIC.BOOL);


                                GradeSendUDTGroup.Tags.Clear();
                                GradeSendUDTGroup.Clear();
                                GradeSendUDTGroup.AddTag(GradeMap);
                                GradeSendUDTGroup.AddTag(LengthMap);
                                GradeSendUDTGroup.AddTag(SampleSize);
                                GradeSendUDTGroup.AddTag(Interval);
                                GradeSendUDTGroup.AddTag(SamplesRemaining);
                                GradeSendUDTGroup.AddTag(Active);
                                GradeSendUDTGroup.AddTag(Stamp);
                                GradeSendUDTGroup.AddTag(Trim);

                                GradeMap.Value = gGradeMap.ToString();
                                LengthMap.Value = gLengthMap.ToString();
                                SampleSize.Value = gSampleSize.ToString();
                                Interval.Value = gInterval.ToString();
                                SamplesRemaining.Value = gSamplesRemaining.ToString();
                                Active.Value = gActive.ToString();
                                Stamp.Value = gStamp.ToString();
                                Trim.Value = gTrim.ToString();

                                try
                                {
                                    if (MyPLCGrade.IsConnected)
                                        MyPLCGrade.GroupWrite(GradeSendUDTGroup);

                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error writing MSR Test GradeSendUDTGroup: " + MyPLCGrade.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != GradeMap.QualityCode)
                                {
                                    UpdateRaptorCommLog("Grade Write MSR Test GradeDataSentToPLC: " + GradeMap.ErrorString);

                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag GradeMap = new Tag("MSRCheck" + gTestID.ToString() + ".GradeMap", Logix.Tag.ATOMIC.DINT);
                                Tag LengthMap = new Tag("MSRCheck" + gTestID.ToString() + ".LengthMap", Logix.Tag.ATOMIC.DINT);
                                Tag SampleSize = new Tag("MSRCheck" + gTestID.ToString() + ".SampleSize", Logix.Tag.ATOMIC.INT);
                                Tag Interval = new Tag("MSRCheck" + gTestID.ToString() + ".Interval", Logix.Tag.ATOMIC.SINT);
                                Tag SamplesRemaining = new Tag("MSRCheck" + gTestID.ToString() + ".SamplesRemaining", Logix.Tag.ATOMIC.INT);
                                Tag Active = new Tag("MSRCheck" + gTestID.ToString() + ".TestActive", Logix.Tag.ATOMIC.BOOL);
                                Tag Stamp = new Tag("MSRCheck" + gTestID.ToString() + ".StampTestBoards", Logix.Tag.ATOMIC.BOOL);
                                Tag Trim = new Tag("MSRCheck" + gTestID.ToString() + ".TrimTestBoards", Logix.Tag.ATOMIC.BOOL);

                                GradeSendUDTGroup.Tags.Clear();
                                GradeSendUDTGroup.Clear();
                                GradeSendUDTGroup.AddTag(GradeMap);
                                GradeSendUDTGroup.AddTag(LengthMap);
                                GradeSendUDTGroup.AddTag(SampleSize);
                                GradeSendUDTGroup.AddTag(Interval);
                                GradeSendUDTGroup.AddTag(SamplesRemaining);
                                GradeSendUDTGroup.AddTag(Active);
                                GradeSendUDTGroup.AddTag(Stamp);
                                GradeSendUDTGroup.AddTag(Trim);

                                try
                                {
                                    if (MyPLCGrade.IsConnected)
                                        MyPLCGrade.GroupRead(GradeSendUDTGroup);
                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error reading MSR Test GradeSendUDTGroup: " + MyPLCGrade.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == GradeMap.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsMSRtest set Grades=" + GradeMap.Value.ToString() + ",Lengths=" + LengthMap.Value.ToString() + ",SampleSize=" + SampleSize.Value.ToString() + ",SamplesRemaining=" + SamplesRemaining.Value.ToString() + ",Interval=" + Interval.Value.ToString() + ",Active='" + Active.Value.ToString() + "',Stamp='" + Stamp.Value.ToString() + "',Trim='" + Trim.Value.ToString() + "' where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("update MSRtest set Grades=" + GradeMap.Value.ToString() + ",Lengths=" + LengthMap.Value.ToString() + ",SampleSize=" + SampleSize.Value.ToString() + ",SamplesRemaining=" + SamplesRemaining.Value.ToString() + ",Interval=" + Interval.Value.ToString() + ",Active='" + Active.Value.ToString() + "',Stamp='" + Stamp.Value.ToString() + "',Trim='" + Trim.Value.ToString() + "' where id=" + gTestID.ToString(), connection);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating MSR Test data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Read MSR Test GradeDataSentToPLC: " + GradeMap.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsMSRtest set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsMSRtest where id <= (select max(id)-100 from DataRequestsMSRtest)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsMSRtest table: " + ex.Message);
                    }
                }*/
                Thread.Sleep(GradeDataToPLCScanRate);
            }
        }

        public static void DiagnosticDataSentToPLC()
        {
            int Succeeded;
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCDiagnostics.IsConnected)
                {
                    while (MyPLCDiagnostics.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Diagnostics " + MyPLCDiagnostics.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Diagnostics Connection to PLC Re-established Successfully!");
                }

                if ((DataRequests & 128) == 128)
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        //Device Tests
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsDiagnostic with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int DiagnosticID = int.Parse(reader["DiagnosticID"].ToString());
                            int dDiagnosticMap = int.Parse(reader["DiagnosticMap"].ToString());
                            int dParameter = int.Parse(reader["Parameter"].ToString());
                            
                            
                            if (Write == true) //writing data to the PLC
                            {
                                Tag DiagnosticMap = new Tag("WebSortMachine.Test", Logix.Tag.ATOMIC.DINT);
                                Tag DiagnosticParameter = new Tag("WebSortMachine.TestVariable[" + DiagnosticID.ToString() + "]", Logix.Tag.ATOMIC.DINT);
                                

                                DiagnosticSendUDTGroup.Tags.Clear();
                                DiagnosticSendUDTGroup.Clear();
                                DiagnosticSendUDTGroup.AddTag(DiagnosticMap);
                                DiagnosticSendUDTGroup.AddTag(DiagnosticParameter);
                                
                                DiagnosticMap.Value = dDiagnosticMap.ToString();
                                DiagnosticParameter.Value = dParameter.ToString();
                                
                                try
                                {
                                    if (MyPLCDiagnostics.IsConnected)
                                        MyPLCDiagnostics.GroupWrite(DiagnosticSendUDTGroup);

                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error writing DiagnosticSendUDTGroup: " + MyPLCDiagnostics.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != DiagnosticMap.QualityCode)
                                {
                                    UpdateRaptorCommLog("Diagnostic Group Write DiagnosticDataSentToPLC: " + DiagnosticMap.ErrorString);

                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag DiagnosticMap = new Tag("WebSortMachine.Test", Logix.Tag.ATOMIC.DINT);
                                Tag DiagnosticState = new Tag("WebSortMachine.Test." + DiagnosticID.ToString(), Logix.Tag.ATOMIC.BOOL);
                                Tag DiagnosticParameter = new Tag("WebSortMachine.TestVariable[" + DiagnosticID.ToString() + "]", Logix.Tag.ATOMIC.DINT);
                                
                                DiagnosticSendUDTGroup.Tags.Clear();
                                DiagnosticSendUDTGroup.Clear();
                                DiagnosticSendUDTGroup.AddTag(DiagnosticState);
                                DiagnosticSendUDTGroup.AddTag(DiagnosticMap);
                                DiagnosticSendUDTGroup.AddTag(DiagnosticParameter);
                                
                                try
                                {
                                    if (MyPLCDiagnostics.IsConnected)
                                        MyPLCDiagnostics.GroupRead(DiagnosticSendUDTGroup);
                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error reading DiagnosticSendUDTGroup: " + MyPLCDiagnostics.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == DiagnosticMap.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsDiagnostic set DiagnosticID = " + DiagnosticID + ",DiagnosticMap=" + DiagnosticMap.Value.ToString() + ",Parameter=" + DiagnosticParameter.Value.ToString() + " where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("update Diagnostictests set diagnosticon='" + DiagnosticState.Value.ToString() + "', parameter=" + DiagnosticParameter.Value.ToString() + " where deviceid=" + DiagnosticID, connection);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating Diagnostic data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Diagnostic Group Read DiagnosticDataSentToPLC: " + DiagnosticMap.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsDiagnostic set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsDiagnostic where id <= (select max(id)-100 from DataRequestsDiagnostic)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();

                        //Device Modes
                        SqlCommand cmdd = new SqlCommand("select * from DataRequestsDiagnostic1 with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader readerd = cmdd.ExecuteReader();

                        while (readerd.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(readerd["Write"].ToString());
                            int DiagnosticID = int.Parse(readerd["DiagnosticID"].ToString());
                            int dDiagnosticMap = int.Parse(readerd["DiagnosticMap"].ToString());
                            int dParameter = int.Parse(readerd["Parameter"].ToString());


                            if (Write == true) //writing data to the PLC
                            {
                                Tag DiagnosticMap = new Tag("WebSortMachine.Device", Logix.Tag.ATOMIC.DINT);
                                Tag DiagnosticParameter = new Tag("WebSortMachine.DeviceVariable[" + DiagnosticID.ToString() + "]", Logix.Tag.ATOMIC.DINT);


                                DiagnosticSendUDTGroup.Tags.Clear();
                                DiagnosticSendUDTGroup.Clear();
                                DiagnosticSendUDTGroup.AddTag(DiagnosticMap);
                                DiagnosticSendUDTGroup.AddTag(DiagnosticParameter);

                                DiagnosticMap.Value = dDiagnosticMap.ToString();
                                DiagnosticParameter.Value = dParameter.ToString();

                                try
                                {
                                    if (MyPLCDiagnostics.IsConnected)
                                        MyPLCDiagnostics.GroupWrite(DiagnosticSendUDTGroup);

                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error writing Diagnostic1SendUDTGroup: " + MyPLCDiagnostics.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != DiagnosticMap.QualityCode)
                                {
                                    UpdateRaptorCommLog("Diagnostic Group Write Diagnostic1DataSentToPLC: " + DiagnosticMap.ErrorString);

                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag DiagnosticMap = new Tag("WebSortMachine.Device", Logix.Tag.ATOMIC.DINT);
                                Tag DiagnosticState = new Tag("WebSortMachine.Device." + DiagnosticID.ToString(), Logix.Tag.ATOMIC.BOOL);
                                Tag DiagnosticParameter = new Tag("WebSortMachine.DeviceVariable[" + DiagnosticID.ToString() + "]", Logix.Tag.ATOMIC.DINT);

                                DiagnosticSendUDTGroup.Tags.Clear();
                                DiagnosticSendUDTGroup.Clear();
                                DiagnosticSendUDTGroup.AddTag(DiagnosticState);
                                DiagnosticSendUDTGroup.AddTag(DiagnosticMap);
                                DiagnosticSendUDTGroup.AddTag(DiagnosticParameter);

                                try
                                {
                                    if (MyPLCDiagnostics.IsConnected)
                                        MyPLCDiagnostics.GroupRead(DiagnosticSendUDTGroup);
                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error reading Diagnostic1SendUDTGroup: " + MyPLCDiagnostics.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == DiagnosticMap.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsDiagnostic1 set DiagnosticID = " + DiagnosticID + ",DiagnosticMap=" + DiagnosticMap.Value.ToString() + ",Parameter=" + DiagnosticParameter.Value.ToString() + " where id=" + readerd["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmd1 = new SqlCommand("update Diagnostictests1 set diagnosticon='" + DiagnosticState.Value.ToString() + "', parameter=" + DiagnosticParameter.Value.ToString() + " where deviceid=" + DiagnosticID, connection);
                                        cmd1.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating Diagnostic1 data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Diagnostic Group Read Diagnostic1DataSentToPLC: " + DiagnosticMap.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsDiagnostic1 set processed = 1 where id=" + readerd["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsDiagnostic1 where id <= (select max(id)-100 from DataRequestsDiagnostic1)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        readerd.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsDiagnostic table: " + ex.Message);
                    }
                }
                Thread.Sleep(DiagnosticDataToPLCScanRate);
            }
        }

        public static void DrivesDataSentToPLC()
        {
            int Succeeded;
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCDrives.IsConnected)
                {
                    while (MyPLCDrives.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Drives " + MyPLCDrives.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Drives Connection to PLC Re-established Successfully!");
                }

                if ((DataRequests & 1024) == 1024)
                {
                    //poll database for data requests from WEBSort, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsDrive with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int DriveID = int.Parse(reader["DriveID"].ToString());
                            int Command = int.Parse(reader["Command"].ToString());
                            int MasterLink = int.Parse(reader["MasterLink"].ToString());
                            int MaxSpeed = int.Parse(reader["MaxSpeed"].ToString());
                            double Scale = double.Parse(reader["Scale"].ToString());
                            double SpeedMultiplier = double.Parse(reader["SpeedMultiplier"].ToString());
                            bool Slave = Boolean.Parse(reader["Slave"].ToString());
                            bool Master = Boolean.Parse(reader["Master"].ToString());
                            bool Independent = Boolean.Parse(reader["Independent"].ToString());
                            bool Lineal = Boolean.Parse(reader["Lineal"].ToString());
                            bool Transverse = Boolean.Parse(reader["Transverse"].ToString());
                            bool Lugged = Boolean.Parse(reader["Lugged"].ToString());
                            bool Custom = Boolean.Parse(reader["Custom"].ToString());

                            if (Write == true) //writing data to the PLC
                            {
                                Tag dCommand = new Tag("WebSortDrive[" + DriveID + "].Command", Logix.Tag.ATOMIC.DINT);
                                Tag dMasterLink = new Tag("WebSortDrive[" + DriveID + "].MasterLink", Logix.Tag.ATOMIC.DINT);
                                Tag dMaxSpeed = new Tag("WebSortDrive[" + DriveID + "].MaxSpeed", Logix.Tag.ATOMIC.DINT);
                                Tag dScale = new Tag("WebSortDrive[" + DriveID + "].Scale", Logix.Tag.ATOMIC.REAL);
                                Tag dSpeedMultiplier = new Tag("WebSortDrive[" + DriveID + "].SpeedMult", Logix.Tag.ATOMIC.REAL);
                                Tag dSlave = new Tag("WebSortDrive[" + DriveID + "].Slave", Logix.Tag.ATOMIC.BOOL);
                                Tag dMaster = new Tag("WebSortDrive[" + DriveID + "].Master", Logix.Tag.ATOMIC.BOOL);
                                Tag dIndependent = new Tag("WebSortDrive[" + DriveID + "].Independent", Logix.Tag.ATOMIC.BOOL);
                                Tag dLineal = new Tag("WebSortDrive[" + DriveID + "].Lineal", Logix.Tag.ATOMIC.BOOL);
                                Tag dTransverse = new Tag("WebSortDrive[" + DriveID + "].Transverse", Logix.Tag.ATOMIC.BOOL);
                                Tag dLugged = new Tag("WebSortDrive[" + DriveID + "].Lugged", Logix.Tag.ATOMIC.BOOL);
                                Tag dCustom = new Tag("WebSortDrive[" + DriveID + "].Custom", Logix.Tag.ATOMIC.BOOL);

                                DriveSendUDTGroup.Tags.Clear();
                                DriveSendUDTGroup.Clear();
                                DriveSendUDTGroup.AddTag(dCommand);
                                DriveSendUDTGroup.AddTag(dMasterLink);
                                DriveSendUDTGroup.AddTag(dMaxSpeed);
                                DriveSendUDTGroup.AddTag(dScale);
                                DriveSendUDTGroup.AddTag(dSpeedMultiplier);
                                DriveSendUDTGroup.AddTag(dSlave);
                                DriveSendUDTGroup.AddTag(dMaster);
                                DriveSendUDTGroup.AddTag(dIndependent);
                                DriveSendUDTGroup.AddTag(dLineal);
                                DriveSendUDTGroup.AddTag(dTransverse);
                                DriveSendUDTGroup.AddTag(dLugged);
                                DriveSendUDTGroup.AddTag(dCustom);

                                dCommand.Value = Command.ToString();
                                dMasterLink.Value = MasterLink.ToString();
                                dMaxSpeed.Value = MaxSpeed.ToString();
                                dScale.Value = Scale.ToString();
                                dSpeedMultiplier.Value = SpeedMultiplier.ToString();
                                dSlave.Value = Slave.ToString();
                                dMaster.Value = Master.ToString();
                                dIndependent.Value = Independent.ToString();
                                dLineal.Value = Lineal.ToString();
                                dTransverse.Value = Transverse.ToString();
                                dLugged.Value = Lugged.ToString();
                                dCustom.Value = Custom.ToString();

                                try
                                {
                                    if (MyPLCDrives.IsConnected)
                                        MyPLCDrives.GroupWrite(DriveSendUDTGroup);

                                }
                                catch 
                                {
                                    UpdateRaptorCommLog("Error writing DriveSendUDTGroup: " + MyPLCDrives.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD != dCommand.QualityCode)
                                {
                                    UpdateRaptorCommLog("Drive Group Write DriveDataSentToPLC: " + dCommand.ErrorString);

                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag dCommand = new Tag("WebSortDrive[" + DriveID + "].Command", Logix.Tag.ATOMIC.DINT);
                                Tag dActual = new Tag("WebSortDrive[" + DriveID + "].Actual", Logix.Tag.ATOMIC.DINT);
                                Tag dMasterLink = new Tag("WebSortDrive[" + DriveID + "].MasterLink", Logix.Tag.ATOMIC.DINT);
                                Tag dMaxSpeed = new Tag("WebSortDrive[" + DriveID + "].MaxSpeed", Logix.Tag.ATOMIC.DINT);
                                Tag dScale = new Tag("WebSortDrive[" + DriveID + "].Scale", Logix.Tag.ATOMIC.REAL);
                                Tag dSpeedMultiplier = new Tag("WebSortDrive[" + DriveID + "].SpeedMult", Logix.Tag.ATOMIC.REAL);
                                Tag dSlave = new Tag("WebSortDrive[" + DriveID + "].Slave", Logix.Tag.ATOMIC.BOOL);
                                Tag dMaster = new Tag("WebSortDrive[" + DriveID + "].Master", Logix.Tag.ATOMIC.BOOL);
                                Tag dIndependent = new Tag("WebSortDrive[" + DriveID + "].Independent", Logix.Tag.ATOMIC.BOOL);
                                Tag dLineal = new Tag("WebSortDrive[" + DriveID + "].Lineal", Logix.Tag.ATOMIC.BOOL);
                                Tag dTransverse = new Tag("WebSortDrive[" + DriveID + "].Transverse", Logix.Tag.ATOMIC.BOOL);
                                Tag dLugged = new Tag("WebSortDrive[" + DriveID + "].Lugged", Logix.Tag.ATOMIC.BOOL);
                                Tag dCustom = new Tag("WebSortDrive[" + DriveID + "].Custom", Logix.Tag.ATOMIC.BOOL);


                                DriveSendUDTGroup.Tags.Clear();
                                DriveSendUDTGroup.Clear();
                                DriveSendUDTGroup.AddTag(PlanerSpeed);
                                DriveSendUDTGroup.AddTag(ProductLength);
                                try
                                {
                                    if (MyPLCDrives.IsConnected)
                                        MyPLCDrives.GroupRead(DriveSendUDTGroup);
                                }
                                catch
                                {
                                    UpdateRaptorCommLog("Error reading PlanerSpeed and ProductLength: " + MyPLCDrives.ErrorString);
                                }
                                //SqlCommand cmd03 = new SqlCommand("update DriveCurrentState set planerspeed=" + PlanerSpeed.Value.ToString() + ",ProductLength=" + ProductLength.Value.ToString(), connection);
                                //SqlCommand cmd03 = new SqlCommand("updateDriveCurrentState " + PlanerSpeed.Value.ToString() + "," + ProductLength.Value.ToString(), connection);
                                //cmd03.ExecuteNonQuery();

                                DriveSendUDTGroup.Tags.Clear();
                                DriveSendUDTGroup.Clear();
                                DriveSendUDTGroup.AddTag(dCommand);
                                DriveSendUDTGroup.AddTag(dActual);
                                DriveSendUDTGroup.AddTag(dMasterLink);
                                DriveSendUDTGroup.AddTag(dMaxSpeed);
                                DriveSendUDTGroup.AddTag(dScale);
                                DriveSendUDTGroup.AddTag(dSpeedMultiplier);
                                DriveSendUDTGroup.AddTag(dSlave);
                                DriveSendUDTGroup.AddTag(dMaster);
                                DriveSendUDTGroup.AddTag(dIndependent);
                                DriveSendUDTGroup.AddTag(dLineal);
                                DriveSendUDTGroup.AddTag(dTransverse);
                                DriveSendUDTGroup.AddTag(dLugged);
                                DriveSendUDTGroup.AddTag(dCustom);
                                try
                                {
                                    if (MyPLCDrives.IsConnected)
                                        MyPLCDrives.GroupRead(DriveSendUDTGroup);
                                }
                                catch 
                                {
                                    UpdateRaptorCommLog("Error reading DriveSendUDTGroup: " + MyPLCDrives.ErrorString);
                                }
                                if (ResultCode.QUAL_GOOD == dCommand.QualityCode)
                                {
                                    //write results into the database
                                    try
                                    {
                                        int LengthID;
                                        SqlCommand cmd0 = new SqlCommand("update datarequestsDrive set command=" + dCommand.Value.ToString() + ",Actual=" + dActual.Value.ToString() + ",MasterLink=" + dMasterLink.Value.ToString() + ",MaxSpeed=" + dMaxSpeed.Value.ToString() + ",Scale=" + dScale.Value.ToString() + ",SpeedMultiplier=" + dSpeedMultiplier.Value.ToString() + ",Slave='" + dSlave.Value.ToString() + "',Master='" + dMaster.Value.ToString() + "',Independent='" + dIndependent.Value.ToString() + "',Lineal='" + dLineal.Value.ToString() + "',Transverse='" + dTransverse.Value.ToString() + "',Lugged='" + dLugged.Value.ToString() + "',Custom='" + dCustom.Value.ToString() + "' where id=" + reader["id"].ToString(), connection);
                                        cmd0.ExecuteNonQuery();
                                        SqlCommand cmdmode = new SqlCommand("select mode from drivecurrentstate", connection);
                                        SqlDataReader readermode = cmdmode.ExecuteReader();
                                        readermode.Read();
                                        if (readermode["mode"].ToString() == "1")  //single length setup
                                            LengthID = 1;
                                        else
                                        {
                                            SqlCommand cmdlength = new SqlCommand("select lengthid from lengths where lengthnominal=(select productlength*12 from drivecurrentstate)", connection);
                                            SqlDataReader readerlength = cmdlength.ExecuteReader();
                                            readerlength.Read();
                                            LengthID = int.Parse(readerlength["LengthID"].ToString());
                                            readerlength.Close();
                                        }
                                        readermode.Close();
                                        SqlCommand cmd1 = new SqlCommand("update Drives set Command=" + dCommand.Value.ToString() + ",Actual=" + dActual.Value.ToString() + ",Length" + LengthID.ToString() + "Multiplier=" + dSpeedMultiplier.Value.ToString() + " where DriveID = " + DriveID  + " and RecipeID = (select min(recipeid) from recipes where online=1)", connection);
                                        //UpdateRaptorCommLog("test: " + cmd1.CommandText);
                                        cmd1.ExecuteNonQuery();
                                        int Type, Configuration;
                                        if (dIndependent.Value.ToString() == "True")
                                            Type = -1;
                                        else if (dMaster.Value.ToString() == "True")
                                            Type = 0;
                                        else
                                            Type = int.Parse(dMasterLink.Value.ToString());
                                        if (dLineal.Value.ToString() == "True")
                                            Configuration = 0;
                                        else if (dTransverse.Value.ToString() == "True")
                                            Configuration = 1;
                                        else if (dLugged.Value.ToString() == "True")
                                            Configuration = 2;
                                        else
                                            Configuration = 3;
                                        
                                        SqlCommand cmd2 = new SqlCommand("update DriveSettings set maxspeed=" + dMaxSpeed.Value.ToString() + ",gearingactual=" + dScale.Value.ToString() + ",Type=" + Type + ",configuration=" + Configuration + " where DriveID=" + DriveID, connection);
                                        cmd2.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateRaptorCommLog("Error updating Drive data in database: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    UpdateRaptorCommLog("Drive Group Read DriveDataSentToPLC: " + dCommand.ErrorString);
                                    Succeeded = 0;
                                    Thread.Sleep(10000);
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsDrive set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsDrive where id <= (select max(id)-100 from DataRequestsDrive)", connection);
                            cmd12.ExecuteNonQuery();


                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsDrive table: " + ex.Message);
                    }
                }
                Thread.Sleep(DrivesDataToPLCScanRate);
            }
        }

        public unsafe static void TimingDataSentToPLC()
        {
            int Succeeded;
            DTEncoding dtEncTiming = new DTEncoding();
            DTEncoding dtEncTiming1 = new DTEncoding();
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCTiming.IsConnected)
                {
                    while (MyPLCTiming.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Timing " + MyPLCTiming.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Timing Connection to PLC Re-established Successfully!");
                }

                if ((DataRequests & 256) == 256)
                {
                    //poll database for data requests from WEBTiming, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsTiming with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            int TimingID = int.Parse(reader["PLCID"].ToString());
                            bool Write = bool.Parse(reader["Write"].ToString());
                            int TimingData1 = int.Parse(reader["Item1Value"].ToString());
                            int TimingData2 = int.Parse(reader["Item2Value"].ToString());
                            int TimingData3 = int.Parse(reader["Item3Value"].ToString());
                            int TimingData4 = int.Parse(reader["Item4Value"].ToString());
                            int TimingData5 = int.Parse(reader["Item5Value"].ToString());
                            int TimingData6 = int.Parse(reader["Item6Value"].ToString());
                            int TimingData7 = int.Parse(reader["Item7Value"].ToString());
                            int TimingData8 = int.Parse(reader["Item8Value"].ToString());
                            int TimingData9 = int.Parse(reader["Item9Value"].ToString());
                            int TimingData10 = int.Parse(reader["Item10Value"].ToString());                            

                            if (Write == true) //writing data to the PLC
                            {
                                // encode TimingREADWRITE for writing  
                                Tag udtRead = new Tag("WebSortTiming[" + TimingID + "]", Logix.Tag.ATOMIC.OBJECT);
                                Tag myUDT = new Tag("WebSortTiming[" + TimingID + "]",Logix.Tag.ATOMIC.OBJECT);
                                myUDT.DataType = Logix.Tag.ATOMIC.OBJECT;

                                MyPLCTiming.ReadTag(udtRead);
                                // get Abbreviated Type Code byte array
                                UInt16 UDT_TypeCode = 0;
                                byte[] typeCode = dtEncTiming.GetDataTypeCode(udtRead);
                                UDT_TypeCode = BitConverter.ToUInt16(typeCode, 0);
                                TIMINGREADWRITE Timingreadwrite = (TIMINGREADWRITE)dtEncTiming.ToType(udtRead, typeof(TIMINGREADWRITE));

                                Timingreadwrite.TimingData[0] = TimingData1;
                                Timingreadwrite.TimingData[1] = TimingData2;
                                Timingreadwrite.TimingData[2] = TimingData3;
                                Timingreadwrite.TimingData[3] = TimingData4;
                                Timingreadwrite.TimingData[4] = TimingData5;
                                Timingreadwrite.TimingData[5] = TimingData6;
                                Timingreadwrite.TimingData[6] = TimingData7;
                                Timingreadwrite.TimingData[7] = TimingData8;
                                Timingreadwrite.TimingData[8] = TimingData9;
                                Timingreadwrite.TimingData[9] = TimingData10;                                                               

                                myUDT.Value = dtEncTiming.FromType(Timingreadwrite);
                                // set data type code
                                dtEncTiming.SetDataTypeCode(typeCode, myUDT);

                                try
                                {
                                    if (MyPLCTiming.IsConnected)
                                    {
                                        if (MyPLCTiming.WriteTag(myUDT) != Logix.ResultCode.E_SUCCESS)
                                        {
                                            UpdateRaptorCommLog("Timing UDT Write TimingDataSentToPLC: " + myUDT.ErrorString);
                                            Succeeded = 0;
                                            Thread.Sleep(10000);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    UpdateRaptorCommLog("Error writing TimingSendUDTGroup: " + ex.Message);
                                }                                
                            }
                            else //reading data from the PLC
                            {
                                Tag myUDT = new Tag("WEBSortTiming[" + TimingID + "]", Logix.Tag.ATOMIC.OBJECT);

                                if (MyPLCTiming.IsConnected)
                                {
                                    if (MyPLCTiming.ReadTag(myUDT) != Logix.ResultCode.E_SUCCESS)
                                    {
                                        UpdateRaptorCommLog("Error reading Timing UDT: " + MyPLCTiming.ErrorString);
                                        Succeeded = 0;
                                        Thread.Sleep(10000);
                                    }
                                    else
                                    {
                                        TIMINGREADWRITE Timingreadwrite = (TIMINGREADWRITE)dtEncTiming.ToType(myUDT, typeof(TIMINGREADWRITE));

                                        //write results into the database
                                        try
                                        {
                                            SqlCommand cmd0 = new SqlCommand("update datarequestsTiming set Item1Value=" + Timingreadwrite.TimingData[0] + ",Item2Value=" + Timingreadwrite.TimingData[1] + ",Item3Value=" + Timingreadwrite.TimingData[2] + ",Item4Value=" + Timingreadwrite.TimingData[3] + ",Item5Value=" + Timingreadwrite.TimingData[4] + ",Item6Value=" + Timingreadwrite.TimingData[5] + ",Item7Value=" + Timingreadwrite.TimingData[6] + ",Item8Value=" + Timingreadwrite.TimingData[7] + ",Item9Value=" + Timingreadwrite.TimingData[8] + ",Item10Value=" + Timingreadwrite.TimingData[9] + " where id=" + reader["id"].ToString(), connection);
                                            cmd0.ExecuteNonQuery();
                                            SqlCommand cmd1 = new SqlCommand("UpdateTimingData " + TimingID + "," + Timingreadwrite.TimingData[0] + "," + Timingreadwrite.TimingData[1] + "," + Timingreadwrite.TimingData[2] + "," + Timingreadwrite.TimingData[3] + "," + Timingreadwrite.TimingData[4] + "," + Timingreadwrite.TimingData[5] + "," + Timingreadwrite.TimingData[6] + "," + Timingreadwrite.TimingData[7] + "," + Timingreadwrite.TimingData[8] + "," + Timingreadwrite.TimingData[9], connection);
                                            cmd1.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            UpdateRaptorCommLog("Error updating Timings data in database: " + ex.Message);
                                            Succeeded = 0;
                                        }
                                    }
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsTiming set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsTiming where id <= (select max(id)-100 from DataRequestsTiming)", connection);
                            cmd12.ExecuteNonQuery();

                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsTiming table: " + ex.Message);
                    }
                }
                Thread.Sleep(TimingDataToPLCScanRate);
            }
        }

        public unsafe static void ParameterDataSentToPLC()
        {
            int Succeeded;
            DTEncoding dtEncTiming = new DTEncoding();
            DTEncoding dtEncTiming1 = new DTEncoding();
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                if (!MyPLCParameter.IsConnected)
                {
                    while (MyPLCParameter.Connect() != ResultCode.E_SUCCESS)
                    {
                        UpdateRaptorCommLog("Parameters " + MyPLCParameter.ErrorString);
                        //return;
                    }
                    UpdateRaptorCommLog("Timing Connection to PLC Re-established Successfully!");
                }

                if ((DataRequests & 1048576) == 1048576)
                {
                    //poll database for data requests from WEBTiming, either writing data to PLC or reading data from PLC
                    try
                    {
                        SqlCommand cmd = new SqlCommand("select * from DataRequestsParameters with(nolock) where processed = 0 order by id", connection);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            //data request exists, process whether this a read or write request
                            Succeeded = 1;
                            int ParameterID = int.Parse(reader["PLCID"].ToString());
                            bool Write = bool.Parse(reader["Write"].ToString());
                            float TimingData1 = float.Parse(reader["Item1Value"].ToString());


                            if (Write == true) //writing data to the PLC
                            {
                                Tag Parameter = new Tag("WEBSortParameters[" + ParameterID + "]", Logix.Tag.ATOMIC.REAL);
                                Parameter.Value = TimingData1;

                                try
                                {
                                    if (MyPLCParameter.IsConnected)
                                    {
                                        if (MyPLCParameter.WriteTag(Parameter) != Logix.ResultCode.E_SUCCESS)
                                        {
                                            UpdateRaptorCommLog("Write Parameter To PLC: " + Parameter.ErrorString);
                                            Succeeded = 0;
                                            Thread.Sleep(1000);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    UpdateRaptorCommLog("Error writing Parameter: " + ex.Message);
                                }
                            }
                            else //reading data from the PLC
                            {
                                Tag Parameter = new Tag("WEBSortParameters[" + ParameterID + "]", Logix.Tag.ATOMIC.REAL);
                                Parameter.Value = TimingData1;

                                if (MyPLCParameter.IsConnected)
                                {
                                    if (MyPLCParameter.ReadTag(Parameter) != Logix.ResultCode.E_SUCCESS)
                                    {
                                        UpdateRaptorCommLog("Error reading Parameter tag: " + MyPLCParameter.ErrorString);
                                        Succeeded = 0;
                                        Thread.Sleep(1000);
                                    }
                                    else
                                    {
                                        //write results into the database
                                        try
                                        {
                                            SqlCommand cmd0 = new SqlCommand("update datarequestsParameters set Item1Value=" + Parameter.Value + " where id=" + reader["id"].ToString(), connection);
                                            cmd0.ExecuteNonQuery();
                                            SqlCommand cmd1 = new SqlCommand("UpdateParameterData " + ParameterID + "," + Parameter.Value, connection);
                                            cmd1.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            UpdateRaptorCommLog("Error updating Parameters data in database: " + ex.Message);
                                            Succeeded = 0;
                                        }
                                    }
                                }
                            }
                            //mark data request as processed
                            if (Succeeded == 1)
                            {
                                SqlCommand cmd11 = new SqlCommand("update DataRequestsParameters set processed = 1 where id=" + reader["id"].ToString(), connection);
                                cmd11.ExecuteNonQuery();
                            }
                            SqlCommand cmd12 = new SqlCommand("delete from DataRequestsParameters where id <= (select max(id)-100 from DataRequestsParameters)", connection);
                            cmd12.ExecuteNonQuery();

                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorCommLog("Error reading DataRequestsParameters table: " + ex.Message);
                    }
                }
                Thread.Sleep(TimingDataToPLCScanRate);
            }
        }

        public static void PLCPoll()
        {
            Boolean Firstscan = true;
            string AActiveMessage;
                      
            
            while (true)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                
                /*if (!MyPollPLC.IsConnected)
                    while (MyPollPLC.Connect() != ResultCode.E_SUCCESS)
                        UpdateRaptorCommLog("Poll: " + MyPollPLC.ErrorString);*/
                try
                {
                    if (MyPollPLC.IsConnected)
                    {
                        PollUDTGroup.Tags.Clear();
                        PollUDTGroup.Clear();
                        PollUDTGroup.AddTag(pollcount);
                        PollUDTGroup.AddTag(lugrate);
                        PollUDTGroup.AddTag(EncoderActual);
                        //PollUDTGroup.AddTag(SkipEncoderPosition);
                        PollUDTGroup.AddTag(PACSerialNumber);                                       
       

                        MyPollPLC.GroupRead(PollUDTGroup);
                       
                        
                        
                    }                   
                }
                catch 
                {
                    UpdateRaptorCommLog("Error reading pollcount tag: " + MyPollPLC.ErrorString);
                }
                if (MyPollPLC.ErrorCode != 0 || !MyPollPLC.IsConnected)
                {
                    UpdateRaptorCommLog("Connection Lost. Attempting Re-connect to PLC");
                    
                    try{
                        CloseThreads(1);
                        CloseConnections();
                        ReadPLCSettings();
                        ResetCounters();
                        ResetBinCounters();
                        InitializeConnectionsAndThreads();               
                        
                    }
                    catch { }
                }
                if (ResultCode.QUAL_GOOD == pollcount.QualityCode)
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("update CurrentState set currentlpm = " + lugrate.Value.ToString() + ",mainencoderactual=" + EncoderActual.Value.ToString() /*+ ",Skipencoderposition=" + SkipEncoderPosition.Value.ToString()*/, connection);
                        cmd.ExecuteNonQuery();
                        SqlCommand cmd1 = new SqlCommand("update RaptorCommSettings set PollCounter = " + pollcount.Value.ToString(), connection);
                        cmd1.ExecuteNonQuery();
                        //SqlCommand cmd1a = new SqlCommand("updateDriveCurrentState " + PlanerSpeed.Value.ToString() + "," + ProductLength.Value.ToString() , connection);
                        //cmd1a.ExecuteNonQuery();
                        SqlCommand cmd2 = new SqlCommand("update WEBSortSetup set WEBSortProductKeyCurrent = " + PACSerialNumber.Value.ToString(), connection);
                        cmd2.ExecuteNonQuery();

                        if (Firstscan)
                        {
                            //check to see if the serial number entered in the database matches the processor
                            SqlCommand cmd3 = new SqlCommand("select WEBSortProductKeyCurrent=(WEBSortProductKeyCurrent / POWER(2,16) + WEBSortProductKeyCurrent & 65535) + 2487,WEBSortProductKeyEncrypted from WEBSortSetup", connection);
                            SqlDataReader reader = cmd3.ExecuteReader();
                            reader.Read();
                            if (reader["WEBSortProductKeyCurrent"].ToString() != reader["WEBSortProductKeyEncrypted"].ToString())
                            {
                                SqlCommand cmd4 = new SqlCommand("execute UpdateStatusDataWEBSort 484,1", connection);
                                cmd4.ExecuteNonQuery();
                                UpdateRaptorCommLog("RaptorComm Service Stop Request.");

                                peerMsg.ShutDown();

                                CloseThreads(0);
                                ///////////////////////////////
                                /// disconnect the PLC     /// 
                                CloseConnections();
                                connection.Close();
                                RaptorCommService.Close();
                                ServiceController service = new ServiceController("RaptorComm");
                                service.Stop();
                            }
                            else
                            {
                                SqlCommand cmd5 = new SqlCommand("execute UpdateStatusDataWEBSort 484,0", connection);
                                cmd5.ExecuteNonQuery();
                            }
                            reader.Close();
                            Firstscan = false;
                        }
                        
                        //if (Convert.ToInt32(pollcount.Value) % 10 == 0)
                            //UpdateRaptorCommLog("Poll Counter: " + pollcount.Value.ToString());
                        pollcount.Value = (Convert.ToInt32(pollcount.Value) + 1).ToString();
                    
                        MyPollPLC.WriteTag(pollcount);

                      /*  SqlCommand cmd3m = new SqlCommand("select currentmessage from currentstate", connection);
                        SqlDataReader readerm = cmd3m.ExecuteReader();
                        readerm.Read();
                        AActiveMessage = "";
                        AActiveMessage = readerm["currentmessage"].ToString();
                        activemessage.Value = "";
                        activemessage.Length = AActiveMessage.Length;
                        activemessage.Value = AActiveMessage.ToString();
                        MyPollPLC.WriteTag(activemessage);
                        readerm.Close();*/
                    }
                    catch 
                    {
                        UpdateRaptorCommLog("Error writing pollcount tag: " + MyPollPLC.ErrorString);
                    }
                }
                else
                {
                    //UpdateRaptorCommLog("Pollcounter: "  + pollcount.ErrorString);  
                    Thread.Sleep(1000);
                }

                Thread.Sleep(PLCPollScanRate);
            }
        }
        public static void CloseConnections()
        {
            UpdateRaptorCommLog("Closing Connections");
            if (MyPLC.IsConnected)
                MyPLC.Disconnect();
            if (MyPLCState.IsConnected)
                MyPLCState.Disconnect();
            if (MyPollPLC.IsConnected)
                MyPollPLC.Disconnect();
            if (MyPLCBin.IsConnected)
                MyPLCBin.Disconnect();
            if (MyPLCBin1.IsConnected)
                MyPLCBin1.Disconnect();
            if (MyPLCStatus.IsConnected)
                MyPLCStatus.Disconnect();
            if (MyPLCStatus1.IsConnected)
                MyPLCStatus1.Disconnect();
            if (MyPLCSort.IsConnected)
                MyPLCSort.Disconnect();
            if (MyPLCThickness.IsConnected)
                MyPLCThickness.Disconnect();
            if (MyPLCWidth.IsConnected)
                MyPLCWidth.Disconnect();
            if (MyPLCLength.IsConnected)
                MyPLCLength.Disconnect();
            if (MyPLCMoisture.IsConnected)
                MyPLCMoisture.Disconnect();
            if (MyPLCGrade.IsConnected)
                MyPLCGrade.Disconnect();
            if (MyPLCProduct.IsConnected)
                MyPLCProduct.Disconnect();
            if (MyPLCTiming.IsConnected)
                MyPLCTiming.Disconnect();
            if (MyPLCDiagnostics.IsConnected)
                MyPLCDiagnostics.Disconnect();
            if (MyPLCMisc.IsConnected)
                MyPLCMisc.Disconnect();
            if (MyPLCParameter.IsConnected)
                MyPLCParameter.Disconnect();
            
        }
        public static void CloseThreads(int Flag)
        {
            UpdateRaptorCommLog("Closing Threads");
            if (null != DataRequestsThread)
            {
                DataRequestsThread.Abort();
                DataRequestsThread = null;
            }
            if (Flag != 1)
            {
                if (null != PLCPollThread)
                {
                    PLCPollThread.Abort();
                    PLCPollThread = null;
                }
            }
            if (null != LugThread)
            {
                LugThread.Abort();
                LugThread = null;
            }
            if (null != BinDataToPLC)
            {
                BinDataToPLC.Abort();
                BinDataToPLC = null;
            }
            if (null != SortDataToPLC)
            {
                SortDataToPLC.Abort();
                SortDataToPLC = null;
            }
            if (null != BinDataFromPLC)
            {
                BinDataFromPLC.Abort();
                BinDataFromPLC = null;
            }
            if (null != StatusDataFromPLC)
            {
                StatusDataFromPLC.Abort();
                StatusDataFromPLC = null;
            }
            if (null != StatusDataVariableFromPLC)
            {
                StatusDataVariableFromPLC.Abort();
                StatusDataVariableFromPLC = null;
            }
            if (null != ParameterDataToPLC)
            {
                ParameterDataToPLC.Abort();
                ParameterDataToPLC = null;
            }
            if (null != ProductDataToPLC)
            {
                ProductDataToPLC.Abort();
                ProductDataToPLC = null;
            }
            if (null != ThicknessDataToPLC)
            {
                ThicknessDataToPLC.Abort();
                ThicknessDataToPLC = null;
            }
            if (null != WidthDataToPLC)
            {
                WidthDataToPLC.Abort();
                WidthDataToPLC = null;
            }
            if (null != LengthDataToPLC)
            {
                LengthDataToPLC.Abort();
                LengthDataToPLC = null;
            }
            if (null != MoistureDataToPLC)
            {
                MoistureDataToPLC.Abort();
                MoistureDataToPLC = null;
            }
            if (null != GradeDataToPLC)
            {
                GradeDataToPLC.Abort();
                GradeDataToPLC = null;
            }
            if (null != TimingDataToPLC)
            {
                TimingDataToPLC.Abort();
                TimingDataToPLC = null;
            }
            if (null != DiagnosticsDataToPLC)
            {
                DiagnosticsDataToPLC.Abort();
                DiagnosticsDataToPLC = null;
            }
            if (null != DrivesDataToPLC)
            {
                DrivesDataToPLC.Abort();
                DrivesDataToPLC = null;
            }

            if (null != PLCStateThread)
            {
                PLCStateThread.Abort();
                PLCStateThread = null;
            }
        }
        public static void Close()
        {
            UpdateRaptorCommLog("RaptorComm Service Stop Request.");
            
            peerMsg.ShutDown();
            
            CloseThreads(0);
            ///////////////////////////////
            /// disconnect the PLC     /// 
            CloseConnections();
            connection.Close();
            
            
        }


   }
}
