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
        public static int DisplayTime, DisplayTimetemp;
        public static int BlankTime;
        public static int DisplayType;
        //public static String szIPSelected;
        public static String szPort;
        public static int DisplayCount;
        static string[] szIPSelected = new string[10];
       
        static Thread DisplayThread = null;
         
        public static int DisplayThreadScanRate = 50;
        
        //public static System.IO.Ports.SerialPort serialPortDisplay = new System.IO.Ports.SerialPort();

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
            UpdateRaptorDisplayLog("Reading Infeed Display Settings");
            SqlCommand cmd = new SqlCommand("select * from AlarmGeneralSettingsInfeed", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                szIPSelected[0] = reader["DisplayIPAddress"].ToString();
                UpdateRaptorDisplayLog("Connecting to: " + szIPSelected[0]);
                szPort = reader["DisplayPortNumber"].ToString();
                //DisplayTime = Convert.ToInt16(Single.Parse(reader["DisplayTime"].ToString()) * 1000);
                //BlankTime = Convert.ToInt16(Single.Parse(reader["BlankTime"].ToString()) * 1000);
                DisplayCount = int.Parse(reader["DisplayCount"].ToString());
                try
                {
                    string header;
                    Object objData;
                    byte[] byData;
                                                                    
                    UpdateRaptorDisplayLog("Attempting to connect to Infeed Adaptive Display.");
                    //create a new client socket ...                        
                    int alPort = System.Convert.ToInt16(szPort, 10);
                    for (int i = 0; i < DisplayCount; i++)
                    {
                        if (connected[i] == false)
                        {
                            m_socWorker[i] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                            System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(szIPSelected[i]);
                            System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, alPort);
                            m_socWorker[i].Connect(remoteEndPoint);
                            connected[i] = true;
                        }
                    }

                    //clear sign memory
                    header = "";
                    header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1 + "Z" + "00" + (char)2 + "E";
                    header = header + "$";
                    objData = header + (char)4;
                    byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                    for (int i = 0; i < DisplayCount; i++)
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
                     m_socWorker[i].Send(byData);
                    
                    //allocate memory to use string function A, B and string file "1"
                    header = "";
                    header = header + (char)0 + (char)0 + (char)0 + (char)0 + (char)0 + (char)1 + "Z" + "00" + (char)2 + "E";
                    header = header + "$AAU0084FF00BAU0084FF001BL00640000";
                    objData = header + (char)4;
                    byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                    for (int i = 0; i < DisplayCount; i++)
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

                UpdateRaptorDisplayLog("Connection to Infeed Display Established Successfully!");
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
            UpdateRaptorDisplayLog("RaptorDisplayInfeed Service Started.");
               
            ReadDisplaySettings();                             
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
                        SqlCommand cmd = new SqlCommand("selectDisplayMessagesInfeed", connection);
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
                               for (int i = 0; i < DisplayCount; i++)
                                    m_socWorker[i].Send(byData);                                
                               
                                message = message.Replace("\r", "  |  ");
                                message = message.Replace("'", "''");
                                UpdateRaptorDisplayLog("Sent to the Infeed Display: ''" + message + "''");                                
                            }
                            int sleepcounter=0;
                            bool higherprioritymessagefound = false;
                            while (sleepcounter <= DisplayTime) //exit loop as soon as possible to increase response time to display new alarms or default messages
                            {
                                // check for a higher priority message that may have come in or if all alarms have been cleared
                                try
                                {
                                    SqlCommand cmd1 = new SqlCommand("selectDisplayMessagePriorityInfeed " + reader["AlarmID"].ToString(), connection);
                                    SqlDataReader reader1 = cmd1.ExecuteReader();
                                    reader1.Read();
                                    if (reader1["Abort"].ToString() == "1")  //higher priority message found
                                    {
                                        UpdateRaptorDisplayLog("Higher Priority Infeed Message Detected.");
                                        previousalarmtext = reader1["HigherPriorityAlarmText"].ToString();
                                        reader1.Close();
                                        reader.Close();
                                        sentblank = true;
                                        higherprioritymessagefound = true;
                                        break;
                                    }
                                    reader1.Close();
                                }
                                catch(Exception ex)
                                {
                                    UpdateRaptorDisplayLog(ex.Message);
                                }
                                sleepcounter = sleepcounter + 250;
                                Thread.Sleep(250);
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
                            m_socWorker[i].Disconnect(true);
                        for (int i = 0; i < 10; i++)
                        {
                            connected[i] = false;
                        
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
                
                Thread.Sleep(DisplayThreadScanRate);
            }
        }        
        
        public static void Close()
        {
            UpdateRaptorDisplayLog("RaptorDisplayInfeed Service Stopped.");
            if (null != DisplayThread)
            {
                DisplayThread.Abort();
                DisplayThread = null;
            }
            
            m_socWorker[0].Disconnect(true);
            connection.Close();            
            
        }


   }
}
