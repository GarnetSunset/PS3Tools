﻿using DiscUtils.Iso9660;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;

namespace PS4_PS2_Classis_GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        XmlDataDocument xmldoc = null;
        string xmlcontentid = "";


        //items needed
        string PS2ID;
        string[] elfs;
        private string[][] Apps;
        private string[][] Auths;
        private StringComparison ignore = StringComparison.InvariantCultureIgnoreCase;
        private List<string> Fws;

        private void Form1_Load(object sender, EventArgs e)
        {
            ExtractAllResources();

            LoadGp4();



            //quickly read sfo 
            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            PS2ClassicsSfo.SFO sfo = new PS2ClassicsSfo.SFO(AppCommonPath() + @"\PS2Emu\" + "param.sfo");

            //all we want to change is the Content ID which will rename the package 
            txtContentID.Text = sfo.ContentID.ToString().Trim().Substring(7, 9);

            LoadAuthDB();
        }

        public void LoadAuthDB()
        {
            if (!File.Exists(AppCommonPath() + "authinfo.txt"))
                MessageBox.Show("Can not find authinfo.txt.");
            else
            {
                Fws = new List<string>();
                List<string[]> _Apps = new List<string[]>();
                List<string[]> _Auths = new List<string[]>();
                List<string> _apps = new List<string>();
                List<string> _auths = new List<string>();
                bool app, auth, fw;
                app = auth = fw = false;
                foreach (string line in File.ReadAllLines(AppCommonPath() + "authinfo.txt"))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.Contains("[FW="))
                        {
                            if (Fws.Count > 0 && app) { MessageBox.Show("DataBase Inconsistent !"); break; }

                            string Is = @"=";
                            string Ie = "]";

                            int start = line.ToString().IndexOf(Is) + Is.Length;
                            int end = line.ToString().IndexOf(Ie, start);
                            if (end > start)
                                Fws.Add(line.ToString().Substring(start, end - start));
                            fw = true;
                            if (Fws.Count > 1)
                            {
                                _Apps.Add(_apps.ToArray());
                                _Auths.Add(_auths.ToArray());
                                _apps = new List<string>();
                                _auths = new List<string>();
                            }

                        }
                        else if (line.Contains("[Name="))
                        {
                            if (!fw)
                            {
                                if (!auth) { MessageBox.Show("DataBase Inconsistent !"); break; }
                            }

                            string Is = @"=";
                            string Ie = "]";

                            int start = line.ToString().IndexOf(Is) + Is.Length;
                            int end = line.ToString().IndexOf(Ie, start);
                            if (end > start)
                                _apps.Add(line.ToString().Substring(start, end - start));
                            auth = fw = false;
                            app = true;
                        }
                        else if (line.Contains("[Auth="))
                        {
                            if (!app)
                            {
                                if (fw) { MessageBox.Show("DataBase Inconsistent !"); break; }
                            }
                            string Is = @"=";
                            string Ie = "]";

                            int start = line.ToString().IndexOf(Is) + Is.Length;
                            int end = line.ToString().IndexOf(Ie, start);
                            if (end > start)
                                _auths.Add(line.ToString().Substring(start, end - start));
                            app = false;
                            auth = true;
                        }
                    }
                }

                _Apps.Add(_apps.ToArray());
                _Auths.Add(_auths.ToArray());

                Apps = _Apps.ToArray();
                Auths = _Auths.ToArray();
                //will need to clean this up a bit later
            }
        }

        public void LoadGp4()
        {
            //create new XML Document 
            xmldoc = new XmlDataDocument();
            //nodelist 
            XmlNodeList xmlnode;
            //setup the resource file to be extarcted
            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            //load the xml file from the base directory
            xmldoc.Load(AppCommonPath() + @"\PS2Emu\" + "PS2Classics.gp4");
            //now load the nodes
            xmlnode = xmldoc.GetElementsByTagName("volume");//volume is inside the xml
            //loop to get all info from the node list
            foreach (XmlNode xn in xmlnode)
            {
                XmlNode xNode = xn.SelectSingleNode("package");
                if (xNode != null)
                {
                    //we found the info we are looking for
                    xmlcontentid = xNode.Attributes[0].Value.ToString();//fetch the attribute
                }
            }

        }
        public void SaveGp4()
        {
            //create new XML Document 
            xmldoc = new XmlDataDocument();
            //nodelist 
            XmlNodeList xmlnode;
            //setup the resource file to be extarcted
            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            //load the xml file from the base directory
            xmldoc.Load(AppCommonPath() + @"\PS2Emu\" + "PS2Classics.gp4");
            //now load the nodes
            xmlnode = xmldoc.GetElementsByTagName("volume");//volume is inside the xml
            //loop to get all info from the node list
            foreach (XmlNode xn in xmlnode)
            {
                XmlNode xNode = xn.SelectSingleNode("package");
                if (xNode != null)
                {
                    //we found the info we are looking for
                    xNode.Attributes[0].Value = xmlcontentid;//set the attribute
                }
            }
            //xmlnode = xmldoc.GetElementsByTagName("volume_ts");
            //foreach (XmlNode item in xmlnode)
            //{
            //    item.InnerText = DateTime.Now.ToString("YYYY-MM-DD HH:mm:ss");//2018-03-21 15:37:08
            //}
            
            xmldoc.Save(AppCommonPath() + @"\PS2Emu\" + "PS2Classics.gp4");

        }

        #region << orbis_pub_cmd >>

        /*
         *orbis-pub-cmd.exe gp4_proj_create --volume_type pkg_ps4_app --storage_type bd50 --content_id IV0002-NPXS29038_00-SIMPLESHOOTINGAM --passcode GvE6xCpZxd96scOUGuLPbuLp8O800B0s simple_shooting_game.gp4 
         */

        #region << Update PS4 Project >>



        #endregion << Update PS4 Project >>

        #region << --volume_type >>

        public enum Volume_Type_PKG
        {
            //Project For a Package
            [Description("Project for an application package")]
            pkg_ps4_app,
            [Description("Project for a patch package")]
            pkg_ps4_patch,
            [Description("Project for a remaster package")]
            pkg_ps4_remaster,
            [Description("Project for an additional content package (with extra data)")]
            pkg_ps4_ac_data,
            [Description("Project for an additional content package (without extra data)")]
            pkg_ps4_ac_nodata,
            [Description("Project for a system software theme package")]
            pkg_ps4_theme

        }

        public enum Volume_Type_ISO
        {
            //Project For a Package
            [Description("Project for an ISO image file (BD, Max 25 GB)")]
            bd25,
            [Description("Project for an ISO image file (BD, Max 50 GB)")]
            bd50,
            [Description("Project for an ISO image file (BD, Max 50GB + BD, Max 25GB)")]
            bd50_25,
            [Description("Project for an ISO image file (BD, Max 50GB + BD, Max 50GB)")]
            bd50_50
        }

        //examplecode
        //Example: --volume_type pkg_ps4_app

        #endregion << --volume_type>>

        #region << _volume_ts TimeStamp>>

        /// <summary>
        /// This will return current timestamp as YYYY-MM-DD hh:mm:ss
        /// </summary>
        /// <returns>--volume_ts "2014-01-01 12:34:56"</returns>

        public string GetCurrentTimeStamp()
        {
            return "--volume_ts \"" + DateTime.Now.ToString("YYYY-MM-DD hh:mm:ss") + "\"";
        }

        //examplecode
        //Example: --volume_ts "2014-01-01 12:34:56"
        #endregion << _volume_ts TimeStamp>>

        #region << Enum List>>
        
       

        #endregion << Enum List>>

        #region << --content_id content_id>>
        /// <summary>
        /// this will set the game id
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns>--content_id " + "UP9000-" + GameID + "_00-SLUS209090000001"</returns>
        public string SetContentID(string GameID)
        {
            //set the game id
            return "--content_id " + "UP9000-" + GameID + "_00-SLUS209090000001";
        }

        //examplecode
        //Example: --content_id UP9000-CRST00001_00-SLUS209090000001
        #endregion << --content_id content_id>>

        #region <<--passcode passcode >>
        public string SetPasscode(string passcode)
        {
            return "--passcode " + passcode;
        }

        public string Use_Default_Passcode()
        {
            return "--passcode ng8II8vax3iXZU7sfI3ugo8XlebJ731o";//default 
        }

        //Example: --passcode GvE6xCpZxd96scOUGuLPbuLp8O800B0s
        #endregion <<--passcode passcode >>


        #region <<--storage_type Storage Type >>

        public enum Storage_Type_Application
        {
            //Project For a Package
            [Description("Digital and BD, Max 25 GB")]
            bd25,
            [Description("Digital and BD, Max 50 GB")]
            bd50,
            [Description("Digital and 2 BDs, Max 50GB+25GB (2 images)")]
            bd50_25,
            [Description("Digital and 2 BDs, Max 50GB+50GB (2 images)")]
            bd50_50,
            [Description("Digital only, Max 50 GB")]
            digital50
        }

        public enum Storage_Type_Patch
        {
            //Project For a Package
            [Description("Digital only, Max 25 GB")]
            digital25
        }

        public enum Storage_Type_Remaster
        {
            //Project For a Package
            [Description("Digital and BD, Max 25 GB")]
            bd25,
            [Description("Digital and BD, Max 50 GB")]
            bd50,
            [Description("Digital only, Max 25 GB")]
            digital25,
            [Description("Digital only, Max 50 GB")]
            digital50
        }

        #endregion <<--storage_type Storage Type >>

        #region << --app_type app_type >>

        public enum App_Type
        {
            //Project For a Package
            [Description("Paid Standalone Full App")]
            full,
            [Description("Upgradable App")]
            upgradable,
            [Description("Demo App")]
            demo,
            [Description("Freemium App")]
            freemium,
        }

        #endregion << --app_type app_type >>

        //we will work with an existing XML

        public string Orbis_CMD(string command, string arguments)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = AppCommonPath() + "orbis-pub-cmd-ps2.exe " + command;
            start.Arguments = arguments ;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        /// <summary>
        /// Fake Sign and spoof authentication informations.
        /// </summary>
        /// <param name="deci">The TitleIDs decimal value as a hex string.</param>
        private bool FakeSign(string deci)
        {
            string _elfs = string.Empty;
            int count = 0;
            bool err = false;

            ProcessStartInfo run = new ProcessStartInfo();
            Process call = new Process();
            call.ErrorDataReceived += Ps2_ErrorHandler;
            run.FileName = AppCommonPath() + "make_fself.exe";
            run.UseShellExecute = false;
            run.CreateNoWindow = run.RedirectStandardError = true;

            foreach (string elf in Apps[2])
            {
                string auth = Auths[2][count];
                auth = deci + auth.Substring(4, auth.Length - 4);

                _elfs = string.Empty;
                foreach (string found in elfs) { if (found.Contains(elf)) _elfs = found; }
                if (_elfs == string.Empty) { MessageBox.Show("Couldn't find: " + elf); return false; }

                run.Arguments = "--paid " + auth.Substring(0, 16).EndianSwapp() + " --auth-info " + auth + " " + _elfs + " " + _elfs.Replace(".elf", "fself").Replace(".prx", "fself");
                MessageBox.Show(run.Arguments);
                call.StartInfo = run;

                try { call.Start(); }
                catch (Exception io) { MessageBox.Show(io.ToString()); err = true; break; }

                call.BeginErrorReadLine();
                call.WaitForExit();
                count++;
            }

            if (err) return false;
            return true;
        }

        /// <summary>
        /// Error Event Handler for the make_fself.py and orbis-pub-cmd-ps2.exe Process.
        /// </summary>
        /// <param name="sendingProcess">The Process which triggered this Event.</param>
        /// <param name="outLine">The Received Data Event Arguments.</param>
        private static void Ps2_ErrorHandler(object sendingProcess, DataReceivedEventArgs outLine) {
            MessageBox.Show(outLine.Data);
        }

        #endregion << orbis_pub_cmd >>

        #region << Extract Needed Resources >>

        private string AppCommonPath()
        {
            string returnstring = "";

            returnstring = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ps4Tools\";

            return returnstring;
        }

        public void ExtractAllResources()
        {
            if (!Directory.Exists(AppCommonPath()))
            {
                Directory.CreateDirectory(AppCommonPath());
            }
            if (!Directory.Exists(AppCommonPath() + @"\PS2Emu\"))
            {
                Directory.CreateDirectory(AppCommonPath() + @"\PS2Emu\");
            }

            //copy byte files
            System.IO.File.WriteAllBytes(AppCommonPath() + @"\PS2Emu\" + "PS2Classics.gp4", Properties.Resources.PS2Classics);
            System.IO.File.WriteAllBytes(AppCommonPath() + @"\PS2Emu\" + "param.sfo", Properties.Resources.param);
            System.IO.File.WriteAllBytes(AppCommonPath() + "orbis_pub_cmd.exe", Properties.Resources.orbis_pub_cmd);
            System.IO.File.WriteAllBytes(AppCommonPath() + "PS2.zip", Properties.Resources.PS2);

            //copy images for the save process
            Properties.Resources.icon0.Save(AppCommonPath() + @"\PS2Emu\" + "icon0.png");
            Properties.Resources.icon0.Save(AppCommonPath() + @"\PS2Emu\" + "pic0.png");
            Properties.Resources.icon0.Save(AppCommonPath() + @"\PS2Emu\" + "pic1.png");

            //copy text files
            System.IO.File.WriteAllText(AppCommonPath() + @"\PS2Emu\" +  "sfo.xml", Properties.Resources.sfo);

            //extarct zip
            if(Directory.Exists(AppCommonPath() + @"\PS2\"))
            {
                DeleteDirectory(AppCommonPath() + @"\PS2\");
            }
            ZipFile.ExtractToDirectory(AppCommonPath() + "PS2.zip", AppCommonPath());
        }


        #endregion << Extract Needed Resources >>

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        private void btnISO_Click(object sender, EventArgs e)
        {
            OpenFileDialog thedialog = new OpenFileDialog();
            thedialog.Title = "Select ISO";
            thedialog.Filter = "Image File|*.iso";
            thedialog.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
            if (thedialog.ShowDialog() == DialogResult.OK)
            {
                
                string isopath = thedialog.FileName;
                txtPath.Text = isopath;
                using (FileStream isoStream = File.OpenRead(isopath))
                {
                    //use disk utils to read iso quickly
                    CDReader cd = new CDReader(isoStream, true);
                    Stream fileStream = cd.OpenFile(@"SYSTEM.CNF", FileMode.Open);
                    // Use fileStream...
                    TextReader tr = new StreamReader(fileStream);
                    string fullstring = tr.ReadToEnd();//read string to end this will read all the info we need

                    string Is = @"\";
                    string Ie = ";";

                    int start = fullstring.ToString().IndexOf(Is) + Is.Length;
                    int end = fullstring.ToString().IndexOf(Ie, start);
                    if (end > start)
                    {
                        string PS2Id = fullstring.ToString().Substring(start, end - start);

                        if (PS2Id != string.Empty)
                        {
                            PS2ID = PS2Id.Replace(".", ""); 
                            lblPS2ID.Text = "PS2 ID : " + PS2Id.Replace(".", "");
                        }
                        else
                        {
                            MessageBox.Show("Could not load PS2 ID");
                        }
                    }
                }
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void restoreBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void txtTitleId_TextChanged(object sender, EventArgs e)
        {
            lblContentName.Text = txtTitleId.Text.Trim();
        }


        public delegate void IntDelegate(int Int);

        public event IntDelegate FileCopyProgress;
        public void CopyFileWithProgress(string source, string destination)
        {
            var webClient = new WebClient();
            webClient.DownloadProgressChanged += DownloadProgress;
            webClient.DownloadFile(new Uri(source), destination);
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            if (FileCopyProgress != null)
            {
                FileCopyProgress(e.ProgressPercentage);
                progressBar1.Invoke(new Action(() => progressBar1.Value = e.ProgressPercentage));
            }

        }

        /// <summary>
        /// Check if ELFs of the base are Decrypted.
        /// </summary>
        /// <param name="path">Path to the template folder.</param>
        private bool IsElfDecrypted()
        {
            byte[] magic = new byte[4] { 0x7F, 0x45, 0x4C, 0x46, };

            foreach (string elf in elfs)
            {
                using (BinaryReader binReader = new BinaryReader(new FileStream(elf, FileMode.Open, FileAccess.Read)))
                {
                    byte[] fmagic = new byte[4];
                    binReader.Read(fmagic, 0, 4);
                    if (!magic.Contains(fmagic)) return false;
                    binReader.Close();
                }
            }
            return true;
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PS4 PKG|*.pkg";
            saveFileDialog1.Title = "Save an PS4 PKG File";
            if(DialogResult.OK != saveFileDialog1.ShowDialog())
            {
                return;
            }


            if (!Directory.Exists(AppCommonPath() + @"\Working\"))
            {
                Directory.CreateDirectory(AppCommonPath() + @"\Working\");
            }
            //first we need to build the new SFO 
            File.Copy(AppCommonPath() + @"\PS2Emu\" + "sfo.xml", AppCommonPath() + @"\Working\" + "sfo.xml",true);

            //now we need to prase it and change it 

            //create new XML Document 
            xmldoc = new XmlDataDocument();
            //nodelist 
            XmlNodeList xmlnode;
            //setup the resource file to be extarcted
            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            //load the xml file from the base directory
            xmldoc.Load(AppCommonPath() + @"\Working\" + "sfo.xml");
            //now load the nodes
            xmlnode = xmldoc.GetElementsByTagName("paramsfo");//volume is inside the xml
            //loop to get all info from the node list
            foreach (XmlNode xn in xmlnode)
            {
                XmlNode xNode = xn.SelectSingleNode("CONTENT_ID");
                XmlNodeList nodes = xmldoc.SelectNodes("//param[@key='CONTENT_ID']");
                if (nodes != null)
                {
                    xmlcontentid = "UP9000-" + txtContentID.Text.Trim() + "_00-" + PS2ID.Replace("_", "") + "0000001";
                    nodes[0].InnerText = xmlcontentid;
                }
                nodes = xmldoc.SelectNodes("//param[@key='TITLE']");
                if (nodes != null)
                {
                    nodes[0].InnerText =txtTitleId.Text.Trim();
                }
                nodes = xmldoc.SelectNodes("//param[@key='TITLE_ID']");
                if (nodes != null)
                {
                    nodes[0].InnerText = txtContentID.Text.Trim();
                }
                for (int i = 1; i < 7; i++)
                {
                    nodes = xmldoc.SelectNodes("//param[@key='SERVICE_ID_ADDCONT_ADD_"+i+"']");
                    if (nodes != null)
                    {
                        nodes[0].InnerText = string.Empty;
                    }
                }
            }
            //save this into the working folder
            xmldoc.Save(AppCommonPath() + @"\Working\" + "sfo.xml");

            SaveGp4();


            //now call orbis and create sfo
            Orbis_CMD("", "sfo_create \"" + AppCommonPath() + @"\Working\" + "sfo.xml" + "\" \"" + AppCommonPath() + @"\Working\" + "param.sfo" + "\"");

            //move SFO to main directory with locations of new images 

            File.Copy(AppCommonPath() + @"\Working\" + "param.sfo", AppCommonPath() + @"\PS2\sce_sys\param.sfo", true);
            //now move ISO
            File.Delete(AppCommonPath() + @"\PS2\image\disc01.iso");
            //CopyFileWithProgress(txtPath.Text.Trim(), AppCommonPath() + @"\PS2\image\disc01.iso");
            File.Copy(txtPath.Text.Trim(), AppCommonPath() + @"\PS2\image\disc01.iso", true);

            // Set elfs path.//this is from CFWProphet THANSK BRO
            elfs = new string[4] {
                AppCommonPath() + @"\PS2\eboot.elf",
                AppCommonPath() + @"\PS2\ps2-emu-compiler.elf",
                AppCommonPath() + @"\PS2\sce_module\libSceFios2.prx",
                AppCommonPath() + @"\PS2\sce_module\libc.prx",
            };

            //if(IsElfDecrypted() == false)
            //{
            //    return;
            //}

            //if (!FakeSign(GetDecimalBytes(PS2ID.Replace("_", "").Substring(4, 5))))
            //{
            //    MessageBox.Show("Error Signing Fake Selfs");
            //    return;
            //}

            //now create pkg 
            Orbis_CMD("", "img_create --oformat pkg \"" + AppCommonPath() + @"\PS2Emu\" + "PS2Classics.gp4\" \"" + Path.GetDirectoryName(saveFileDialog1.FileName) + "\"");
            //orbis_pub_cmd.exe img_create --skip_digest --oformat pkg C:\Users\3deEchelon\AppData\Roaming\Ps4Tools\PS2Emu\PS2Classics.gp4 C:\Users\3deEchelon\AppData\Roaming\Ps4Tools\PS2Emu\


            //now we delete the working directory
            DeleteDirectory(AppCommonPath() + @"\Working\");
            DeleteDirectory(AppCommonPath() + @"\PS2\");
        }


        /// <summary>
        /// Get the string of a byte converted decimal value.
        /// </summary>
        /// <param name="titleId">The TitleID decimal to convert.</param>
        /// <returns>A string, representing the convertet decimal byte value.</returns>
        private string GetDecimalBytes(string titleId)
        {
            byte[] titleIdBytes = Convert.ToDecimal(titleId).GetBytes();
            return BitConverter.ToString(titleIdBytes).Substring(0, 5).Replace("-", "");
        }


    }

    public static class Extensionclass
    {
        public static string EndianSwapp(this string source)
        {
            string reversed = string.Empty;
            for (int i = source.Length; i > 0; i -= 2)
            {
                if (i < 2) reversed += source.Substring(i - i, 1);
                else reversed += source.Substring(i - 2, 2);
            }
            return reversed;
        }

        public static byte[] GetBytes(this decimal dec)
        {
            //Load four 32 bit integers from the Decimal.GetBits function
            Int32[] bits = decimal.GetBits(dec);

            //Create a temporary list to hold the bytes
            List<byte> bytes = new List<byte>();

            //iterate each 32 bit integer
            foreach (Int32 i in bits) bytes.AddRange(BitConverter.GetBytes(i)); //add the bytes of the current 32bit integer to the bytes list

            //return the bytes list as an array
            return bytes.ToArray();
        }

        /// <summary>
        /// Checks a Array for existens of a other Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The Array to check for existens.</param>
        /// <returns>True if the source Array contains the Array to check for, else false.</returns>
        public static bool Contains<T>(this T[] source, T[] range)
        {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Equals(range[i]))
                {
                    if ((source.Length - i) >= range.Length)
                    {
                        int match = 1;
                        for (int j = 1; j < range.Length; j++)
                        {
                            if (source[i + j].Equals(range[j])) match++;
                            else { i += j; break; }
                        }
                        if (match == range.Length) return true;
                    }
                    else break;
                }
            }
            return false;
        }
    }



}
    