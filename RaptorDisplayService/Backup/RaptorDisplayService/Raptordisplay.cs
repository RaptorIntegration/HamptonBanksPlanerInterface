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
using System.Net.Sockets;

namespace RaptorComm
{
    public static class RaptorCommService
    {
        public static System.Data.SqlClient.SqlConnection connection;
        public static string connectionString;
        static Socket[] m_socWorker = new Socket[10];
        static bool[] connected = new bool[10];
        static int[] attempts = new int[10];
        public static int DisplayTime, DisplayTimetemp;
        public static int BlankTime;
        public static int DisplayType;
        //public static String szIPSelected;
        public static String szPort;
        public static int DisplayCount;
        
        static string[] szIPSelected = new string[10];
       
        static Thread DisplayThread = null;
        static Thread PollCounterThread = null;
        
        public static int DisplayThreadScanRate = 50;
        public static int PollCounterThreadScanRate = 250;
        
        public static System.IO.Ports.SerialPort serialPortDisplay = new System.IO.Ports.SerialPort();

        public static string GetConnectionString()
        {
            return ("Server=(local)\\SQLEXPRESS;Initial Catalog=RaptorWebSort;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=120");
        }
        public static void UpdateRaptorDisplayLog(string text)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("insert into RaptorDisplayLog select getdate(),'" + text + "'", connection);
                cmd.ExecuteNonQuery();
                SqlCommand cmd1 = new SqlCommand("delete from RaptorDisplayLog where id< (select max(id)-100 from RaptorDisplayLog)", connection);
                cmd1.ExecuteNonQuery();
            }
            catch (Exception ex) { UpdateRaptorDisplayLog(ex.Message); }
        }
        public static void ReadDisplaySettings()
        {
            UpdateRaptorDisplayLog("Reading Display Settings");
            /*if (null != DisplayThread)
            {
                DisplayThread.Abort();
                DisplayThread = null;
            }*/
            
            SqlCommand cmd = new SqlCommand("select * from AlarmGeneralSettings", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                szIPSelected[0] = reader["DisplayIPAddress"].ToString();
                
                //DisplayTime = Convert.ToInt16(Single.Parse(reader["DisplayTime"].ToString()) * 1000);
                //BlankTime = Convert.ToInt16(Single.Parse(reader["BlankTime"].ToString()) * 1000);
                DisplayType = int.Parse(reader["DisplayType"].ToString());
                DisplayCount = int.Parse(reader["DisplayCount"].ToString());
                if (DisplayType == 1)  //Adaptive Display
                    if (connected[0] == false && DisplayCount > 0)
                        UpdateRaptorDisplayLog("Attempting to Connect to Adaptive Display: " + szIPSelected[0]);
                szPort = reader["DisplayPortNumber"].ToString();

                if (DisplayType == 1)  //Adaptive Display
                {
                    try
                    {
                        string header;
                        Object objData;
                        byte[] byData;
                        
                        if (DisplayCount > 1)
                        {
                            int lastoctet = int.Parse(szIPSelected[0].Substring(szIPSelected[0].LastIndexOf(".") + 1));
                            for (int i = 1; i < DisplayCount; i++)
                            {
                                lastoctet = lastoctet + 1;
                                szIPSelected[i] = szIPSelected[0].Substring(0, szIPSelected[0].LastIndexOf(".") + 1) + lastoctet.ToString();
                                if (connected[i] == false)
                                {
                                    attempts[i] = attempts[i] + 1;
                                    //if (attempts[i] < 3)
                                        UpdateRaptorDisplayLog("Attempting to Connect to Adaptive Display: " + szIPSelected[i]);
                                }
                            }
                        }
                        //else
                            //if (connected[0] == false)  
                                //UpdateRaptorDisplayLog("Attempting to Connect to Adaptive Display: " + szIPSelected[0]);
                    
                        //create a new client socket ...                        
                        int alPort = System.Convert.ToInt16(szPort, 10);
                        for (int i = 0; i < DisplayCount; i++)
                        {
                            if (connected[i] == false)
                            {
                                //if (i == 0 || (i > 0 && attempts[i] < 3))
                                {
                                    m_socWorker[i] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                                    System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(szIPSelected[i]);
                                    System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, alPort);
                                    //if (m_socWorker[i].Connected == true)
                                        //m_socWorker[i].Disconnect(true);
                                    m_socWorker[i].Connect(remoteEndPoint);
                                    connected[i] = true;
                                    if (DisplayCount > 1)
                                    {
                                        int lastoctet = int.Parse(szIPSelected[0].Substring(szIPSelected[0].LastIndexOf(".") + 1));
                                        //for (int ii = 1; ii < DisplayCount; ii++)
                                        {
                                            lastoctet = lastoctet + i;
                                            szIPSelected[i] = szIPSelected[0].Substring(0, szIPSelected[0].LastIndexOf(".") + 1) + lastoctet.ToString();
                                            UpdateRaptorDisplayLog("Successfully Connected to Adaptive Display: " + szIPSelected[i]);
                                        }
                                    }
                                    else
                                        UpdateRaptorDisplayLog("Successfully Connected to Adaptive Display: " + szIPSelected[0]);
                                }
                                //else
                                   // UpdateRaptorDisplayLog("Giving up on " + szIPSelected[i]);
                            }
                        }

                        //clear sign memory
                        header = "";
                        header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1 + "Z" + "00" + (char)2 + "E";
                        header = header + "$";
                        objData = header + (char)4;
                        byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                        for (int i = 0; i < DisplayCount; i++)
                            if (DisplayCount > 0)
                                m_socWorker[i].Send(byData);
                        Thread.Sleep(4000);
                        
                        //enable speaker
                        header = "";
                        header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1;  //5 nulls and an SOH
                        header = header + "b" + "00";  //display type 4160C, all addresses
                        header = header + (char)2;  //STX
                        header = header + "E";  //command code (write text file)
                        header = header + "!00";  //file label 0                                    
                        objData = header + (char)4;  //EOT
                        byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                        for (int i = 0; i < DisplayCount; i++)
                            if (DisplayCount > 0)
                                m_socWorker[i].Send(byData);
                        
                        //allocate memory to use string function A, B and string file "1"
                        header = "";
                        header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1 + "Z" + "00" + (char)2 + "E";
                        header = header + "$AAU0084FF00BAU0084FF001BL00640000";
                        objData = header + (char)4;
                        byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                        for (int i = 0; i < DisplayCount; i++)
                            if (DisplayCount > 0)
                                m_socWorker[i].Send(byData);
                        
                        /*header = "";
                        header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1;  //5 nulls and an SOH
                        header = header + "b" + "00";  //display type 4160C, all addresses
                        header = header + (char)2;  //STX
                        header = header + "A";  //command code (write text file)
                        header = header + "A";  //file label A
                        header = header + (char)27;  //ESC to mark start of mode fields
                        header = header + "0";  //fill
                        header = header + "b";  //Hold mode
                        //header = header + (char)26 + "3" + (char)18;  //Seven high font, wide
                        header = header + (char)16 + "1";  //Call String File 1
                        objData = header + (char)4;  //EOT
                        byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                        m_socWorker.Send(byData);*/

                        //cancel priority message
                        header = "";
                        header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1 + "Z" + "00" + (char)2 + "E";
                        header = header + ")BFF00";  //this will do it (run time message cancels priority message mode)
                        objData = header + (char)4;
                        byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                        for (int i = 0; i < DisplayCount; i++)
                            if (DisplayCount > 0)
                                m_socWorker[i].Send(byData);
                        /*header = "";
                        header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1 + "b" + "00" + (char)2 + "A";
                        header = header + "0";
                        objData = header + (char)4;
                        byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                        m_socWorker.Send(byData);*/
                    }
                    catch (System.Net.Sockets.SocketException se)
                    {
                        UpdateRaptorDisplayLog(se.Message);
                        Thread.Sleep(5000);
                        ReadDisplaySettings();
                    }
                }
                else if (DisplayType == 2)  //Infomaster Display
                {
                    try
                    {
                        UpdateRaptorDisplayLog("Attempting to Connect to Infomaster Display.");
                        serialPortDisplay.BaudRate = 9600;
                        serialPortDisplay.PortName = szPort;
                        serialPortDisplay.Open();                        
                    }
                    catch (Exception ex)
                    {
                        UpdateRaptorDisplayLog(ex.Message);
                        Thread.Sleep(10000);
                        ReadDisplaySettings();
                    }
                }

                UpdateRaptorDisplayLog("Connection to Display(s) Established Successfully!");
                //start the various threads necessary
                
                if (null == DisplayThread)
                {
                    DisplayThread = new Thread(ReadMessages);
                    DisplayThread.Start();
                }
            }
            reader.Close();
           // connection.Close();    

        } 
                
        public static void Init()
        {
            connectionString = GetConnectionString();
            connection = new SqlConnection(connectionString);
            // Open the connection.
            connection.Open();
            for (int i = 0; i < 10; i++)
            {
                connected[i] = false;
                attempts[i] = 0;
            }
            UpdateRaptorDisplayLog("RaptorDisplay Service Started.");
            if (null == PollCounterThread)
            {
                PollCounterThread = new Thread(MonitorPollCounter);
                PollCounterThread.Start();
            }      
            ReadDisplaySettings();
                             
        }

        public static void MonitorPollCounter()
        {
            int PollCounter=0, oldPollCounter=0, timecounter=0;
            while (true)
            {
                try
                {
                    
                    SqlCommand cmd = new SqlCommand("select PollCounter from RaptorCommSettings with(NOLOCK)", connection);
                    SqlDataReader readerc = cmd.ExecuteReader();
                    readerc.Read();
                    PollCounter = int.Parse(readerc["PollCounter"].ToString());
                    
                    if (PollCounter == oldPollCounter)
                    {
                        Thread.Sleep(PollCounterThreadScanRate);
                        timecounter = timecounter + PollCounterThreadScanRate;
                    }
                    else
                    {
                        timecounter = 0;
                        oldPollCounter = PollCounter;
                        Thread.Sleep(PollCounterThreadScanRate);
                    }
                    if (timecounter > 3000)
                    {
                        SqlCommand cmd1 = new SqlCommand("execute UpdateStatusDataWEBSort 320,1", connection);
                        cmd1.ExecuteNonQuery();
                        
                        if (timecounter > 100000)
                            timecounter = 3000 + PollCounterThreadScanRate;
                    }
                    else
                    {
                        SqlCommand cmd2 = new SqlCommand("execute UpdateStatusDataWEBSort 320,0", connection);
                        cmd2.ExecuteNonQuery();
                    }
                    readerc.Close();
	                    
                }
                catch (Exception ex)
                {
                    UpdateRaptorDisplayLog(ex.Message);  
                }
            

                //Thread.Sleep(PollCounterThreadScanRate);
            }
        }
        
        public static void ReadMessages()
        {
            string previousalarmtext="";
            int previousalarmid=-1;
            bool previouscr = false;
            bool sentblank = true;
            bool dontbother;
            
            while (true)
            {                
                if (DisplayType == 1)  //Adaptive Display 4160
                {
                    try
                    {
                        string header;
                        Object objData;
                        byte[] byData;
                        string message;
                        
                        
                        //send text to the display using text file function

                        try
                        {
                            //UpdateRaptorDisplayLog("Reading Messages");  
                            SqlCommand cmd = new SqlCommand("selectDisplayMessages", connection);
                            SqlDataReader reader = cmd.ExecuteReader();
                        
                            while (reader.Read())
                            {
                                dontbother = false;
                                DisplayTime = Convert.ToInt16(Single.Parse(reader["DisplayTime"].ToString()) * 1000);
                                BlankTime = Convert.ToInt16(Single.Parse(reader["BlankTime"].ToString()) * 1000);
                                message = reader["DisplayText"].ToString().ToUpper();
                                
                                if (int.Parse(reader["alarmid"].ToString()) < 1000)  //alarm message
                                {
                                    header = "";
                                    header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1;  //5 nulls and an SOH
                                    header = header + "b" + "00";  //display type 4160C, all addresses
                                    header = header + (char)2;  //STX
                                    header = header + "A";  //command code (write text file)
                                    header = header + "0";  //file label 0
                                    header = header + (char)27;  //ESC to mark start of mode fields
                                    if (message.Length >= 17)
                                    {
                                        header = header + "0";  //fill
                                        header = header + "b";  //Hold mode    
                                        header = header + (char)26 + "3" + (char)18;  //Seven high font, wide    
                                    }                                
                                    else
                                    {
                                        header = header + " ";  //middle line
                                        header = header + "b";  //Hold mode    
                                        header = header + (char)26 + "9";  //15 high font
                                    }                                 
                                    if (reader["severity"].ToString() == "0")
                                        header = header + (char)28 + "2";  //Green
                                    else if (reader["severity"].ToString() == "1")
                                        header = header + (char)28 + "3";  //Amber
                                    else if (reader["severity"].ToString() == "2" || reader["severity"].ToString() == "3")
                                        header = header + (char)28 + "1";  //Red
                                    objData = header + message + (char)4;  //EOT
                                }
                                else  //default message
                                {
                                    if (previousalarmid < 1000)  //if this is transitioning from a non-default to default message, we need to cancel the priority message
                                    { 
                                        //cancel priority message, otherwise nothing else will be allowed to display
                                        header = "";
                                        header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1 + "Z" + "00" + (char)2 + "E";
                                        header = header + ")BFF00";  //this will do it (run time message cancels priority message mode)
                                        objData = header + (char)4;
                                        byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                                        for (int i = 0; i < DisplayCount; i++)
                                            if (DisplayCount > 0)
                                                m_socWorker[i].Send(byData);
                                    }
                                    //update the string file
                                    header = "";
                                    header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1;  //5 nulls and an SOH
                                    header = header + "b" + "00";  //display type 4160C, all addresses
                                    header = header + (char)2;  //STX
                                    header = header + "G";  //command code (write string file)
                                    header = header + "1";  //string file label 1
                                    if (reader["severity"].ToString() == "0")
                                        header = header + (char)28 + "2";  //Green
                                    else if (reader["severity"].ToString() == "1")
                                        header = header + (char)28 + "3";  //Amber
                                    else if (reader["severity"].ToString() == "2" || reader["severity"].ToString() == "3")
                                        header = header + (char)28 + "1";  //Red
                                    objData = header + message + (char)4;  //EOT
                                    byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                                    for (int i = 0; i < DisplayCount; i++)
                                        if (DisplayCount > 0)
                                            m_socWorker[i].Send(byData); 
                                     
                                    if (reader["AlarmCount"].ToString() == "1")  //to prevent the display from flashing, update font if this is the only default message being displayed, but only do this change the first time through
                                    {
                                        // check for transition from single to multiline or vice versa, in case user has changed this on the fly
                                        if (int.Parse(reader["alarmid"].ToString()) != previousalarmid || (int.Parse(reader["alarmid"].ToString()) == previousalarmid && message.Contains("\r") == true && previouscr == false) || (int.Parse(reader["alarmid"].ToString()) == previousalarmid && message.Contains("\r") == false && previouscr == true))
                                        {
                                            //UpdateRaptorDisplayLog("here " + reader["alarmid"].ToString() + " previous=" + previousalarmid.ToString());
                                            // form text file A message to send
                                            header = "";
                                            header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1;  //5 nulls and an SOH
                                            header = header + "b" + "00";  //display type 4160C, all addresses
                                            header = header + (char)2;  //STX
                                            header = header + "A";  //command code (write text file)
                                            header = header + "A";  //file label A
                                            header = header + (char)27;  //ESC to mark start of mode fields
                                            if (message.Length >= 17 || message.Contains("\r"))
                                            {
                                                header = header + "0";  //fill  
                                                header = header + "b";  //Hold mode                                  
                                                header = header + (char)26 + "3" + (char)18;  //Seven high font, wide  
                                                previouscr = true;                                  
                                            }
                                            else
                                            {
                                                header = header + " ";  //middle line
                                                header = header + "b";  //Hold mode                                    
                                                header = header + (char)26 + "9";  //15 high font
                                                previouscr = false; 
                                            }         
                                            header = header + (char)16 + "1";  //Call String File 1
                                            objData = header + (char)4;  //EOT                                        
                                        }
                                    }  
                                    else //otherwise, update font every time 
                                    {
                                        // form text file A message to send
                                        header = "";
                                        header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1;  //5 nulls and an SOH
                                        header = header + "b" + "00";  //display type 4160C, all addresses
                                        header = header + (char)2;  //STX
                                        header = header + "A";  //command code (write text file)
                                        header = header + "A";  //file label A
                                        header = header + (char)27;  //ESC to mark start of mode fields
                                        if (message.Length >= 17 || message.Contains("\r"))
                                        {
                                            header = header + "0";  //fill  
                                            header = header + "b";  //Hold mode                                  
                                            header = header + (char)26 + "3" + (char)18;  //Seven high font, wide  
                                            previouscr = true;                                  
                                        }
                                        else
                                        {
                                            header = header + " ";  //middle line
                                            header = header + "b";  //Hold mode                                    
                                            header = header + (char)26 + "9";  //15 high font
                                            previouscr = false; 
                                        }         
                                        header = header + (char)16 + "1";  //Call String File 1
                                        objData = header + (char)4;  //EOT                                    
                                    }      
                                    if (message.Contains("\r")) 
                                        previouscr = true;
                                    else
                                        previouscr = false;                                                           
                                    
                                } // end default message
                                
                                byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                                if (reader["DisplayText"].ToString() != previousalarmtext || sentblank == true)  //don't blank the display if the exact same message text is being displayed
                                {
                                    //check one last time to make sure alarm is still active (for the case where there are many alarms in the queue, we don't want to display messages that have disappeared in the  meantime)
                                    if (int.Parse(reader["alarmid"].ToString()) < 1000) //only do this for alarms
                                    {
                                        SqlCommand cmd11 = new SqlCommand("select x = (select count(*) from alarms where alarmid=" + reader["AlarmID"].ToString() + " and stoptime is null) + (select count(*) from alarmsprevious where alarmid=" + reader["AlarmID"].ToString() + " and stoptime is null) ", connection);
                                        SqlDataReader reader11 = cmd11.ExecuteReader();
                                        reader11.Read();
                                        //UpdateRaptorDisplayLog("test: " + reader11["x"].ToString());
                                        if (reader11["x"].ToString() == "0")
                                            dontbother = true;
                                        reader11.Close();
                                    }
                                    if (dontbother == false)
                                        for (int i = 0; i < DisplayCount; i++)
                                            if (DisplayCount > 0)
                                                m_socWorker[i].Send(byData);
                                    
                                    if (reader["severity"].ToString() == "3")  //audible alarm
                                    {
                                        header = "";
                                        header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1;  //5 nulls and an SOH
                                        header = header + "b" + "00";  //display type 4160C, all addresses
                                        header = header + (char)2;  //STX
                                        header = header + "E";  //command code (write text file)
                                        header = header + "(1";  //file label 0                                    
                                        objData = header + (char)4;  //EOT
                                        byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                                        if (dontbother == false)
                                            for (int i = 0; i < DisplayCount; i++)
                                                if (DisplayCount > 0)
                                                    m_socWorker[i].Send(byData);
                                    }
                                    if (dontbother == false)
                                    {
                                        message = message.Replace("\r", "  |  ");
                                        message = message.Replace("'", "''");
                                        //SqlCommand cmdupdate = new SqlCommand("update currentstate set currentmessage = '" + message + "'", connection);
                                        SqlCommand cmdupdate = new SqlCommand("update currentstate set currentalarmid=" + reader["AlarmID"].ToString() + ",currentmessage = '" + message + "'", connection);
                                    
                                        cmdupdate.ExecuteNonQuery();
                                        UpdateRaptorDisplayLog("Sent to the Display: ''" + message + "''");
                                    }
                                }
                                int sleepcounter=0;
                                bool higherprioritymessagefound = false;
                                if (dontbother == false)
                                {
                                    while (sleepcounter <= DisplayTime) //exit loop as soon as possible to increase response time to display new alarms or default messages
                                    {
                                        // check for a higher priority message that may have come in or if all alarms have been cleared
                                        try
                                        {
                                            SqlCommand cmd1 = new SqlCommand("selectDisplayMessagePriority " + reader["AlarmID"].ToString(), connection);
                                            SqlDataReader reader1 = cmd1.ExecuteReader();
                                            reader1.Read();
                                            if (reader1["Abort"].ToString() == "1")  //higher priority message found
                                            {
                                                UpdateRaptorDisplayLog("Higher Priority Message Detected.");
                                                previousalarmtext = reader1["HigherPriorityAlarmText"].ToString();
                                                previousalarmid = int.Parse(reader["alarmid"].ToString());
                                                reader1.Close();
                                                reader.Close();
                                                sentblank = true;
                                                higherprioritymessagefound = true;
                                                break;
                                            }
                                            reader1.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            UpdateRaptorDisplayLog(ex.Message);

                                        }
                                        sleepcounter = sleepcounter + 250;
                                        Thread.Sleep(250);
                                    }
                                }
                                if (higherprioritymessagefound)
                                    break;

                                //only blank the display if this is a non-default message or different text is being displayed
                                if (int.Parse(reader["alarmid"].ToString()) < 1000 && reader["DisplayText"].ToString() != previousalarmtext && reader["AlarmCount"].ToString() != "1") 
                                { 
                                    message=" ";
                                    header = "";
                                    header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1 + "b" + "00" + (char)2 + "A";
                                    header = header + "0" + (char)27 + " " + "b" + (char)28 + "2";
                                    header = header + (char)26 + "9";
                                    objData = header + message + (char)4;
                                    byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                                    sentblank = true;
                                    if (dontbother == false)
                                    {
                                        for (int i = 0; i < DisplayCount; i++)
                                            if (DisplayCount > 0)
                                                m_socWorker[i].Send(byData);                                    
                                        Thread.Sleep(BlankTime);
                                    }
                                }
                                else
                                {
                                    sentblank = false;
                                }
                                previousalarmtext = reader["DisplayText"].ToString();
                                previousalarmid = int.Parse(reader["alarmid"].ToString());
                            }
                            reader.Close();
                        }

                        catch (Exception ex) 
                        {
                            UpdateRaptorDisplayLog(ex.Message);
                            for (int i = 0; i < DisplayCount; i++)
                                if (DisplayCount > 0)
                                    m_socWorker[i].Disconnect(true); 
                            for (int i = 0; i < 10; i++)
                            {                               
                                connected[i] = false;
                                attempts[i] = 0;
                            }
                            ReadDisplaySettings();
                        }
                        
                    }
                    catch (System.Net.Sockets.SocketException se)
                    {
                        UpdateRaptorDisplayLog(se.Message);
                        Thread.Sleep(5000);
                        ReadDisplaySettings();
                        
                    }
                }
                else if (DisplayType == 2)   //Infomaster
                {
                    try
                    {
                        string tempChar = string.Empty;
                        string tempText = string.Empty;
                        string tempTextfinal = string.Empty;
                        int strlength;
                        int sleepcounter = 0;
                        bool higherprioritymessagefound = false;
                        string message;
                        
                        try
                        {
                            SqlCommand cmd = new SqlCommand("selectDisplayMessages", connection);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                dontbother = false;
                                DisplayTime = Convert.ToInt16(Single.Parse(reader["DisplayTime"].ToString()) * 1000);
                                BlankTime = Convert.ToInt16(Single.Parse(reader["BlankTime"].ToString()) * 1000);
                                message = reader["DisplayText"].ToString().ToUpper();
                                
                                if (int.Parse(reader["AlarmID"].ToString()) < 1000) //normal alarm, scroll the text
                                {
                                    //check one last time to make sure alarm is still active (for the case where there are many alarms in the queue, we don't want to display messages that have disappeared in the  meantime)
                                    SqlCommand cmd11 = new SqlCommand("select x = (select count(*) from alarms where alarmid=" + reader["AlarmID"].ToString() + " and stoptime is null) + (select count(*) from alarmsprevious where alarmid=" + reader["AlarmID"].ToString() + " and stoptime is null) ", connection);
                                    SqlDataReader reader11 = cmd11.ExecuteReader();
                                    reader11.Read();
                                    if (reader11["x"].ToString() == "0")
                                        dontbother = true;                                    
                                        
                                    reader11.Close();
                                    tempText = "       " + reader["DisplayText"].ToString().ToUpper() + "        ";  // adds leading and trailing spaces to the text to make the text appear scrolling
                                    strlength = tempText.Length - 8;
                                    if (dontbother == false)
                                    {
                                        serialPortDisplay.Write(tempText + (char)13);
                                        Thread.Sleep(DisplayTime / 20);
                                    }
                                    message = message.Replace("'", "''");
                                    SqlCommand cmdupdate = new SqlCommand("update currentstate set currentalarmid=" + reader["AlarmID"].ToString() + ",currentmessage = '" + message + "'", connection);
                                    cmdupdate.ExecuteNonQuery();
                                    UpdateRaptorDisplayLog("Sent to the Display: ''" + message + "''");
                                    int i = 0;
                                    while (i < strlength)
                                    {
                                        i++;
                                        tempText = tempText.Remove(0, 1);
                                        tempTextfinal = tempText.Substring(0, 8);
                                        if (dontbother == false)
                                            if (serialPortDisplay.IsOpen)
                                               serialPortDisplay.Write(tempTextfinal + (char)13);
                                        // check for a higher priority message that may have come in                            
                                        SqlCommand cmd1 = new SqlCommand("selectDisplayMessagePriority " + reader["AlarmID"].ToString(), connection);
                                        SqlDataReader reader1 = cmd1.ExecuteReader();
                                        reader1.Read();
                                        if (reader1["Abort"].ToString() == "1")
                                        {
                                            UpdateRaptorDisplayLog("Higher Priority Message Detected.");
                                            reader1.Close();
                                            reader.Close();
                                            higherprioritymessagefound = true;
                                            break;
                                        }
                                        reader1.Close();
                                        if (dontbother == false)
                                            Thread.Sleep(DisplayTime / 20);  //lowering this value with make the marquee scroll faster
                                    }
                                    
                                    if (higherprioritymessagefound)
                                        break;
                                    if (dontbother == false)
                                    {
                                        Thread.Sleep(BlankTime);
                                    }
                                }
                                else //default messages
                                {
                                    sleepcounter = 0;
                                    higherprioritymessagefound = false;
                                    if (reader["DisplayText"].ToString().Length <= 8)  //displaytext <=8 characters, don't scroll
                                    {
                                        tempText = reader["DisplayText"].ToString().PadLeft(8).ToUpper();
                                        serialPortDisplay.Write(tempText + (char)13);
                                        message = message.Replace("'", "''");

                                        SqlCommand cmdupdate = new SqlCommand("update currentstate set currentalarmid=" + reader["AlarmID"].ToString() + ",currentmessage = '" + message + "'", connection);
                                        //SqlCommand cmdupdate = new SqlCommand("update currentstate set currentmessage = '" + message + "'", connection);
                                        cmdupdate.ExecuteNonQuery();
                                        UpdateRaptorDisplayLog("Sent to the Display: ''" + message + "''");
                                        
                                        // check for a higher priority message that may have come in
                                        while (sleepcounter <= DisplayTime) //exit loop as soon as possible to increase response time to display new alarms or default messages
                                        {
                                            SqlCommand cmd1 = new SqlCommand("selectDisplayMessagePriority " + reader["AlarmID"].ToString(), connection);
                                            SqlDataReader reader1 = cmd1.ExecuteReader();
                                            reader1.Read();
                                            if (reader1["Abort"].ToString() == "1")
                                            {
                                                UpdateRaptorDisplayLog("Higher Priority Message Detected.");
                                                reader1.Close();
                                                reader.Close();
                                                higherprioritymessagefound = true;
                                                break;
                                            }
                                            reader1.Close();
                                            sleepcounter = sleepcounter + 250;
                                            Thread.Sleep(250);
                                        }
                                        if (higherprioritymessagefound)
                                            break;
                                    } //displaytext <=8 characters
                                    else  //displaytext > 8 characters, must scroll it
                                    {
                                        tempText = "       " + reader["DisplayText"].ToString().ToUpper() + "        ";  // adds leading and trailing spaces to the text to make the text appear scrolling
                                        strlength = tempText.Length - 8;
                                        serialPortDisplay.Write(tempText + (char)13);
                                        Thread.Sleep(DisplayTime / 20);
                                        message = message.Replace("'", "''");
                                        SqlCommand cmdupdate = new SqlCommand("update currentstate set currentalarmid=" + reader["AlarmID"].ToString() + ",currentmessage = '" + message + "'", connection);
                                        //SqlCommand cmdupdate = new SqlCommand("update currentstate set currentmessage = '" + message + "'", connection);
                                        cmdupdate.ExecuteNonQuery();
                                        UpdateRaptorDisplayLog("Sent to the Display: ''" + message + "''");
                                        int i = 0;
                                        while (i < strlength)
                                        {
                                            i++;
                                            tempText = tempText.Remove(0, 1);
                                            tempTextfinal = tempText.Substring(0, 8).ToUpper();
                                            if (serialPortDisplay.IsOpen)
                                                serialPortDisplay.Write(tempTextfinal + (char)13);
                                            // check for a higher priority message that may have come in                            
                                            SqlCommand cmd1 = new SqlCommand("selectDisplayMessagePriority " + reader["AlarmID"].ToString(), connection);
                                            SqlDataReader reader1 = cmd1.ExecuteReader();
                                            reader1.Read();
                                            if (reader1["Abort"].ToString() == "1")
                                            {
                                                UpdateRaptorDisplayLog("Higher Priority Message Detected.");
                                                reader1.Close();
                                                reader.Close();
                                                higherprioritymessagefound = true;
                                                break;
                                            }
                                            reader1.Close();
                                            Thread.Sleep(DisplayTime / 20);  //lowering this value with make the marquee scroll faster
                                        }
                                        if (higherprioritymessagefound)
                                            break;

                                       
                                        Thread.Sleep(BlankTime);
                                    } //displaytext > 8 characters
                                }  //default messages   
                                
                            }
                            reader.Close();
                        }

                        catch (Exception ex) { UpdateRaptorDisplayLog(ex.Message); }
                    }                         
                    catch (Exception ex)
                    {
                        UpdateRaptorDisplayLog(ex.Message);
                        Thread.Sleep(5000);
                        ReadDisplaySettings();
                    }                    
                }
                
                Thread.Sleep(DisplayThreadScanRate);
            }
        }
        
        
        public static void Close()
        {
            UpdateRaptorDisplayLog("RaptorDisplay Service Stopped.");
            if (null != DisplayThread)
            {
                DisplayThread.Abort();
                DisplayThread = null;
            }
            if (null != PollCounterThread)
            {
                PollCounterThread.Abort();
                PollCounterThread = null;
            }
            for (int i = 0; i < DisplayCount; i++)
                if (DisplayCount > 0)
                    m_socWorker[i].Disconnect(true);
            serialPortDisplay.Close();
            connection.Close();
            
            
            
        }


   }
}
