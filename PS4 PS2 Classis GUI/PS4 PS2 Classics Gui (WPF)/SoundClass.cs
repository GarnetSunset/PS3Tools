﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PS4_PS2_Classics_Gui__WPF_
{
    public class SoundClass
    {
        public static IWavePlayer waveOutDevice = new WaveOut();

        //this device is dedicated to the PS4BGM
        public static IWavePlayer PS4BGMDevice = new WaveOut();

        public enum Sound
        {
            PS4_Info_Pannel_Sound = 0,
            Navigation = 1,
            Options = 2,
            Error = 3,
            Shutdown = 4,
            PS4_Music = 5,
            Notification = 6,
            LQ =7,
            sk = 8,
            pete = 9,
            Aloy = 10,
        }

        public static string AppCommonPath()
        {
            string returnstring = "";
            if (Properties.Settings.Default.OverwriteTemp == true && Properties.Settings.Default.TempPath != string.Empty)
            {
                returnstring = Properties.Settings.Default.TempPath + @"\Ps4Tools\";
            }
            else
            {
                returnstring = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ps4Tools\";
            }
            return returnstring;
        }

        #region << New Sound Class This Will Get Sounds From The Assembly >>
        private static Stream GetResourceStream(string filename)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string resname = asm.GetName().Name + "." + filename;
            return asm.GetManifestResourceStream(resname);
        }
        private static WaveStream CreateInputStream(byte[] resource)
        {
            WaveChannel32 inputStream;

            MemoryStream ms = new MemoryStream(resource);

            WaveStream mp3Reader = new Mp3FileReader(ms);
            inputStream = new WaveChannel32(mp3Reader);


            return inputStream;
        }

        private static WaveStream CreateInputStreamAT9(byte[] resource)
        {
            WaveChannel32 inputStream;

            MemoryStream ms = new MemoryStream(resource);

            WaveStream mp3Reader = new WaveFileReader(ms);
            //inputStream = new WaveChannel32(mp3Reader);


            return mp3Reader;
        }

        #endregion << New Sound Class This Will Get Sounds From The Assembly >>

        public static void PlayPS4Sound(Sound Soundtoplay)
        {
            try
            {
                if(Properties.Settings.Default.EnableMusic == false)
                {
                    return;
                }

                switch (Soundtoplay)
                {

                    case Sound.Notification:
                        {
                            new Thread(() =>
                            {
                            //set the thread as a background worker
                            Thread.CurrentThread.IsBackground = true;

                                IWavePlayer waveOutDevice = new WaveOut();
                                WaveStream mp3file = CreateInputStream(Properties.Resources.PS4_Notification);

                                TimeSpan ts = mp3file.TotalTime;

                                waveOutDevice.Init(mp3file);

                                waveOutDevice.Volume = 0.7f;
                                waveOutDevice.Play();


                            /* run your code here */
                                Thread.Sleep(ts);
                                waveOutDevice.Dispose();
                                waveOutDevice.Stop();

                            }).Start();
                        }
                        break;
                    case Sound.Error:
                        {
                            new Thread(() =>
                            {
                            //set the thread as a background worker
                            Thread.CurrentThread.IsBackground = true;

                                IWavePlayer waveOutDevice = new WaveOut();
                                WaveStream mp3file = CreateInputStream(Properties.Resources.Ps4_Error_Sound);

                                TimeSpan ts = mp3file.TotalTime;

                                waveOutDevice.Init(mp3file);

                                waveOutDevice.Volume = 0.5f;
                                waveOutDevice.Play();


                            /* run your code here */
                                Thread.Sleep(ts);
                                waveOutDevice.Dispose();
                                waveOutDevice.Stop();

                            }).Start();
                        }
                        break;
                    case Sound.Shutdown:
                        {
                            WaveStream mp3file = CreateInputStream(Properties.Resources.PS4_Shutdown);
                            TimeSpan ts = mp3file.TotalTime;
                            waveOutDevice.Init(mp3file);

                            waveOutDevice.Volume = 0.5f;
                            waveOutDevice.Play();

                            new Thread(() =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                            /* run your code here */
                                Thread.Sleep(ts);
                                waveOutDevice.Stop();
                            //waveOutDevice.Dispose();
                        }).Start();
                        }
                        break;
                    case Sound.PS4_Info_Pannel_Sound:
                        {
                            new Thread(() =>
                            {
                            //set the thread as a background worker
                            Thread.CurrentThread.IsBackground = true;

                                IWavePlayer waveOutDevice = new WaveOut();

                                WaveStream mp3file = CreateInputStream(Properties.Resources.PS4_Notification);
                                TimeSpan ts = mp3file.TotalTime;
                                waveOutDevice.Init(mp3file);

                                waveOutDevice.Volume = 0.5f;
                                waveOutDevice.Play();

                                Thread.Sleep(ts);
                                waveOutDevice.Stop();
                            }).Start();
                            break;
                        }
                    case Sound.Options:
                        {

                            WaveStream mp3file = CreateInputStream(Properties.Resources.PS4_Options_Pannel);
                            TimeSpan ts = mp3file.TotalTime;
                            waveOutDevice.Init(mp3file);

                            waveOutDevice.Volume = 0.5f;
                            waveOutDevice.Play();

                            new Thread(() =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                            /* run your code here */
                                Thread.Sleep(ts);
                                waveOutDevice.Stop();
                            //waveOutDevice.Dispose();
                        }).Start();
                            break;
                        }
                    case Sound.Navigation:
                        {
                            new Thread(() =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                                IWavePlayer waveOutDevice = new WaveOut();

                                WaveStream mp3file = CreateInputStream(Properties.Resources.PS4_Navigation_Sound);
                                TimeSpan ts = mp3file.TotalTime;
                                waveOutDevice.Init(mp3file);

                                waveOutDevice.Volume = 0.5f;
                                waveOutDevice.Play();


                            /* run your code here */
                                Thread.Sleep(ts);
                                waveOutDevice.Stop();
                            //waveOutDevice.Dispose();
                        }).Start();


                            break;
                        }
                    case Sound.PS4_Music:
                        {

                            WaveStream mp3file = CreateInputStream(Properties.Resources.ps4BGM);

                            ////PS4BGMDevice = new AsioOut("ASIO4ALL v2");
                            ////PS4BGMDevice.Init(mp3file);
                            ////PS4BGMDevice.Play();

                            TimeSpan ts = mp3file.TotalTime;

                            PS4BGMDevice.Init(mp3file);

                            PS4BGMDevice.Volume = 0.5f;
                            PS4BGMDevice.Play();
                        }
                        break;
                    case Sound.LQ:
                        {

                            WaveStream mp3file = CreateInputStream(Properties.Resources.lq);

                            ////PS4BGMDevice = new AsioOut("ASIO4ALL v2");
                            ////PS4BGMDevice.Init(mp3file);
                            ////PS4BGMDevice.Play();

                            TimeSpan ts = mp3file.TotalTime;

                            PS4BGMDevice.Init(mp3file);

                            PS4BGMDevice.Volume = 0.5f;
                            PS4BGMDevice.Play();
                        }
                        break;
                    case Sound.sk:
                        {

                            WaveStream mp3file = CreateInputStream(Properties.Resources.dovahkiin);

                            ////PS4BGMDevice = new AsioOut("ASIO4ALL v2");
                            ////PS4BGMDevice.Init(mp3file);
                            ////PS4BGMDevice.Play();

                            TimeSpan ts = mp3file.TotalTime;

                            PS4BGMDevice.Init(mp3file);

                            PS4BGMDevice.Volume = 0.5f;
                            PS4BGMDevice.Play();
                        }
                        break;
                    case Sound.Aloy:
                        {

                            WaveStream mp3file = CreateInputStream(Properties.Resources.HZDA);

                            ////PS4BGMDevice = new AsioOut("ASIO4ALL v2");
                            ////PS4BGMDevice.Init(mp3file);
                            ////PS4BGMDevice.Play();
                            LoopStream looper = new LoopStream(mp3file);
                            TimeSpan ts = mp3file.TotalTime;

                            PS4BGMDevice.Init(looper);

                            PS4BGMDevice.Volume = 0.5f;
                            PS4BGMDevice.Play();
                            
                        }
                        break;
                    case Sound.pete:
                        {

                            WaveStream mp3file = CreateInputStream(Properties.Resources.priates);

                            ////PS4BGMDevice = new AsioOut("ASIO4ALL v2");
                            ////PS4BGMDevice.Init(mp3file);
                            ////PS4BGMDevice.Play();

                            TimeSpan ts = mp3file.TotalTime;

                            PS4BGMDevice.Init(mp3file);

                            PS4BGMDevice.Volume = 0.5f;
                            PS4BGMDevice.Play();
                        }
                        break;
                    default:
                        break;



                }
            }
            catch(Exception ex)
            {
                //encolsed the entire sound class to not crash on issue #13 
                //it will still break but atleast the application should not crash
            }
        }
    }

    /// <summary>
    /// Stream for looping playback
    /// </summary>
    public class LoopStream : WaveStream
    {
        WaveStream sourceStream;

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        /// <summary>
        /// Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }

        /// <summary>
        /// Return source stream's wave format
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        /// <summary>
        /// LoopStream simply returns
        /// </summary>
        public override long Length
        {
            get { return sourceStream.Length; }
        }

        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
