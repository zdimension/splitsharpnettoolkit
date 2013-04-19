using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Server
{
    public class WebServer
    {
        private TcpListener myListener;

        public delegate void LogSentEventHandler(object sender, string log);
        public event LogSentEventHandler LogSent;

        public void SendLog(string l)
        {
            LogSent(this, l);
            /*LogSentEventHandler temp = LogSent;
            if (temp != null)
            {
                temp(this, l);
            }*/
        }

        private int port = 14550;
        public WebServer()
        {


        }

        public bool Started = false;

        Thread th;

        

        public void Start(int port = 14550)
        {
            try
            {
                SendLog("Starting Split# Web Server v0.1...");
                this.port = port;
                myListener = new TcpListener(port);
                myListener.Start();

                th = new Thread(new ThreadStart(StartListen));
                th.Start();
                SendLog("Server started at port " + port + ".");
                Started = true;
            }
            catch (Exception e)
            {
                SendLog("An exception occured :" + e.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                SendLog("Stopping Split# Web Server v0.1...");

                myListener.Stop();

                th.Abort();
                th = null;

                SendLog("Server stopped.");
            }
            catch (Exception e)
            {
                SendLog("An exception occured :" + e.ToString());
            }
        }

        public string GetTheDefaultFileName(string sLocalDirectory)
        {
            StreamReader sr;
            String sLine = "";

            try
            {
                //Open the default.dat to find out the list
                // of default file
                sr = new StreamReader(Path.Combine(Application.StartupPath, @"conf\defpage.dat"));
                string s = sr.ReadToEnd();
                //MessageBox.Show(s);
                /*foreach (char c in s)
                {
                    MessageBox.Show("" + (byte)c);
                }*/

                /*while ((sLine = sr.ReadLine()) != null)
                {
                    //Look for the default file in the web server root folder
                    if (File.Exists(Path.Combine(sLocalDirectory, sLine)) == true)
                        break;
                }*/
                List<string> def = new List<string>();
                string a = "";
                foreach (char ss in s)
                {
                    if ((byte)ss == 13)
                    {
                        continue;
                    }
                    else if ((byte)ss == 10)
                    {
                        def.Add(a);
                        a = "";
                    }
                    else
                    {
                        a += ss;
                    }
                }
                foreach (string sss in def)
                {
                    string ss1 = sss.Replace("" + (char)10, "");
                    //MessageBox.Show(ss1);
                    if (File.Exists(Path.Combine(Application.StartupPath, "www", ss1)) == true)
                    {
                        sLine = ss1;
                        //MessageBox.Show(sLine);
                        break;
                    }
                }
            /*abc:

                goto abc;*/
            }
            catch (Exception e)
            {
                SendLog("An exception occurred : " + e.ToString());
            }
            //MessageBox.Show(Path.Combine(Application.StartupPath, "www", sLine));
            if (File.Exists(Path.Combine(Application.StartupPath, "www", sLine)) == true)
                return sLine;
            else
                return "";
        }

        public string GetLocalPath(string sDirName)
        {

            StreamReader sr;
            String sLine = "";
            String sVirtualDir = "";
            String sRealDir = "";
            int iStartPos = 0;


            sDirName.Trim();



            sDirName = sDirName.ToLower();


            try
            {
                sr = new StreamReader(Path.Combine(Application.StartupPath, @"conf\vfold.dat"));

                while ((sLine = sr.ReadLine()) != null)
                {
                    //sLine.Trim();

                    if (sLine.Length > 0)
                    {
                        iStartPos = sLine.IndexOf(" ");

                        // Convert to lowercase
                        sLine = sLine.ToLower();

                        sVirtualDir = sLine.Substring(0, iStartPos);
                        sRealDir = sLine.Substring(iStartPos + 1);

                        if (sVirtualDir == sDirName)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SendLog("An Exception Occurred : " + e.ToString());
            }

            if (sRealDir.StartsWith("%%"))
            {
                sRealDir = sRealDir.Substring(3);
                sRealDir = Path.Combine(Application.StartupPath, sRealDir);
            }

            if (sVirtualDir == sDirName)
                return sRealDir;
            else
                return "";
        }

        public string GetMimeType(string sRequestedFile)
        {


            StreamReader sr;
            String sLine = "";
            String sMimeType = "";
            String sFileExt = "";
            String sMimeExt = "";

            // Convert to lowercase
            sRequestedFile = sRequestedFile.ToLower();

            int iStartPos;

            //sFileExt = sRequestedFile.Substring(iStartPos);
            sFileExt = Path.GetExtension(sRequestedFile);

            try
            {
                //Open the Vdirs.dat to find out the list virtual directories
                sr = new StreamReader(Path.Combine(Application.StartupPath, @"conf\mime.dat"));

                while ((sLine = sr.ReadLine()) != null)
                {

                    //sLine.Trim();

                    if (sLine.Length > 0)
                    {
                        //find the separator
                        iStartPos = sLine.IndexOf(" ");

                        // Convert to lower case
                        sLine = sLine.ToLower();

                        sMimeExt = sLine.Substring(0, iStartPos);
                        sMimeType = sLine.Substring(iStartPos + 1);

                        if (sMimeExt == sFileExt)
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                SendLog("An exception occurred : " + e.ToString());
            }

            if (sMimeExt == sFileExt)
                return sMimeType;
            else
                return "";
        }

        public void SendHeader(string sHttpVersion, string sMIMEHeader,
            int iTotBytes, string sStatusCode, ref Socket mySocket)
        {

            String sBuffer = "";

            // if Mime type is not provided set default to text/html
            if (sMIMEHeader.Length == 0)
            {
                sMIMEHeader = "text/sshtml";  // Default Mime Type is text/html
            }

            sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
            sBuffer = sBuffer + "Server: cx1193719-b\r\n";
            sBuffer = sBuffer + "Content-Type: " + sMIMEHeader + "\r\n";
            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";

            Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);

            SendToBrowser(bSendData, ref mySocket);

            SendLog("Total Bytes : " + iTotBytes.ToString());

        }

        public void SendToBrowser(String sData, ref Socket mySocket, bool SplitSharp = false)
        {
            if (!SplitSharp)
            {
                SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);
            }
            else
            {
                SendToBrowser(Encoding.ASCII.GetBytes(SplitPageInterpreter.GetHTMLFromSSHTML(sData, this)), ref mySocket);
            }
        }


        public void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
        {
            int numBytes = 0;
            try
            {
                if (mySocket.Connected)
                {
                    if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1)
                        SendLog("Socket Error! Cannot send packet");
                    else
                    {
                        SendLog("Bytes send : " + numBytes);
                    }
                }
                else
                    SendLog("Connection lost.");
            }
            catch (Exception e)
            {
                SendLog("An error occurred : " + e);
            }
        }

        public void StartListen()
        {

            int iStartPos = 0;
            String sRequest;
            String sDirName;
            String sRequestedFile;
            String sErrorMessage;
            String sLocalDir;
            String sPhysicalFilePath = "";
            String sFormattedMessage = "";
            String sResponse = "";


            while (true)
            {
                //Accept a new connection
                Socket mySocket = myListener.AcceptSocket();

                SendLog("Socket type : " + mySocket.SocketType);
                if (mySocket.Connected)
                {
                    SendLog("Client Connected.\r\n==================\r\nClient IP : " + mySocket.RemoteEndPoint);


                    //make a byte array and receive data from the client 
                    Byte[] bReceive = new Byte[1024];
                    int i = mySocket.Receive(bReceive, bReceive.Length, 0);


                    //Convert Byte to String
                    string sBuffer = Encoding.ASCII.GetString(bReceive);


                    //At present we will only deal with GET type
                    if (sBuffer.Substring(0, 3) != "GET")
                    {
                        SendLog("Only 'GET' method is supported.");
                        mySocket.Close();
                        return;
                    }


                    // Look for HTTP request
                    iStartPos = sBuffer.IndexOf("HTTP", 1);


                    // Get the HTTP text and version e.g. it will return "HTTP/1.1"
                    string sHttpVersion = sBuffer.Substring(iStartPos, 8);


                    // Extract the Requested Type and Requested file/directory
                    sRequest = sBuffer.Substring(0, iStartPos - 1);


                    //Replace backslash with Forward Slash, if Any
                    sRequest.Replace("\\", "/");


                    //If file name is not supplied add forward slash to indicate 
                    //that it is a directory and then we will look for the 
                    //default file name..
                    if ((sRequest.IndexOf(".") < 1) && (!sRequest.EndsWith("/")))
                    {
                        sRequest = sRequest + "/";
                    }
                    //Extract the requested file name
                    iStartPos = sRequest.LastIndexOf("/") + 1;
                    sRequestedFile = sRequest.Substring(iStartPos);


                    //Extract The directory Name
                    sDirName = sRequest.Substring(sRequest.IndexOf("/"),
                               sRequest.LastIndexOf("/") - 3);
                    /////////////////////////////////////////////////////////////////////
                    // Identify the Physical Directory
                    /////////////////////////////////////////////////////////////////////
                    //if (sDirName == "/")
                    //    sLocalDir = sMyWebServerRoot;
                    //else
                    //{
                        //Get the Virtual Directory
                        sLocalDir = GetLocalPath(sDirName);
                    //}


                    SendLog("Directory requested : " + sLocalDir);

                    //If the physical directory does not exists then
                    // dispaly the error message
                    if (sLocalDir.Length == 0)
                    {
                        //sErrorMessage = "<H2>Error!! Requested Directory does not exists</H2><Br>";
                        sErrorMessage = File.ReadAllText(Path.Combine(Application.StartupPath, @"pages\errors\Error404.htm"));
                        //sErrorMessage = sErrorMessage + "Please check data\\Vdirs.Dat";

                        //Format The Message
                        SendHeader(sHttpVersion, "", sErrorMessage.Length,
                                   " 404 Not Found", ref mySocket);

                        //Send to the browser
                        SendToBrowser(sErrorMessage, ref mySocket);

                        mySocket.Close();

                        continue;
                    }
                    /////////////////////////////////////////////////////////////////////
                    // Identify the File Name
                    /////////////////////////////////////////////////////////////////////

                    //If The file name is not supplied then look in the default file list
                    if (sRequestedFile.Length == 0)
                    {
                        // Get the default filename
                        sRequestedFile = GetTheDefaultFileName(sLocalDir);

                        if (sRequestedFile == "")
                        {
                            sErrorMessage = File.ReadAllText(Path.Combine(Application.StartupPath, @"pages\errors\NoDefPage.htm"));
                            SendHeader(sHttpVersion, "", sErrorMessage.Length,
                                       " 404 Not Found", ref mySocket);
                            SendToBrowser(sErrorMessage, ref mySocket);

                            mySocket.Close();

                            return;

                        }
                    }
                    //////////////////////////////////////////////////
                    // Get TheMime Type
                    //////////////////////////////////////////////////

                    String sMimeType = GetMimeType(sRequestedFile);


                    //Build the physical path
                    sPhysicalFilePath = Path.Combine(sLocalDir, sRequestedFile);
                    SendLog("File requested : " + sPhysicalFilePath);
                    if (File.Exists(sPhysicalFilePath) == false)
                    {

                        sErrorMessage = File.ReadAllText(Path.Combine(Application.StartupPath, @"pages\errors\Error404.htm"));
                        SendHeader(sHttpVersion, "", sErrorMessage.Length,
                                   " 404 Not Found", ref mySocket);
                        SendToBrowser(sErrorMessage, ref mySocket);

                        SendLog(sFormattedMessage);
                    }
                    else
                    {
                        int iTotBytes = 0;

                        sResponse = "";

                        FileStream fs = new FileStream(sPhysicalFilePath,
                                        FileMode.Open, FileAccess.Read,
                          FileShare.Read);
                        // Create a reader that can read bytes from the FileStream.


                        BinaryReader reader = new BinaryReader(fs);
                        byte[] bytes = new byte[fs.Length];
                        int read;
                        while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Read from the file and write the data to the network
                            sResponse = sResponse + Encoding.ASCII.GetString(bytes, 0, read);

                            iTotBytes = iTotBytes + read;

                        }
                        reader.Close();
                        fs.Close();

                        SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                        bytes = Encoding.ASCII.GetBytes(SplitPageInterpreter.GetHTMLFromSSHTML(Encoding.ASCII.GetString(bytes), this));
                        SendToBrowser(bytes, ref mySocket);
                        //mySocket.Send(bytes, bytes.Length,0);

                    }
                    mySocket.Close();
                }
            }
        }
    }
}
