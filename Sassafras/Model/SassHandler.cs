using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Sassafras
{
    public static class SassHandler
    {
        // CONSTANTS

        private static string FileLinksPath = "./file_watchers.json";
        private static double OutputDelay = 10;


        // PROPERTIES

        private static List<Process> WatchProcs;
        private static bool IsRunning;

        private static Timer OutputTimer;
        private static Timer ErrorTimer;
        private static string LastOutputMessage;
        private static string LastErrorMessage;
        public static EventHandler OnOutput;
        public static EventHandler OnError;
        public static List<string> AllOutputLines;
        public static List<string> OutputLines;
        public static List<string> ErrorLines;

        public static List<SassFile> AllSassFiles = new List<SassFile>();


        // METHODS

        private static void OutputReceived(object sender, DataReceivedEventArgs e)
        {
            if (LastOutputMessage == null) LastOutputMessage = e.Data;
            else LastOutputMessage += "\r\n" + e.Data;
            //Set timer for grouping output lines together
            if (OutputTimer == null || !OutputTimer.Enabled)
            {
                OutputTimer = new Timer();
                OutputTimer.Interval = OutputDelay;
                OutputTimer.Elapsed += OutputTimer_Elapsed;
                OutputTimer.Start();
            }
            //Reset timer
            OutputTimer.Stop();
            OutputTimer.Start();
        }


        private static void ErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (LastErrorMessage == null) LastErrorMessage = e.Data;
            else LastErrorMessage += "\r\n" + e.Data;
            //Set timer for grouping output lines together
            if (ErrorTimer == null || !ErrorTimer.Enabled)
            {
                ErrorTimer = new Timer();
                ErrorTimer.Interval = OutputDelay;
                ErrorTimer.Elapsed += ErrorTimer_Elapsed;
                ErrorTimer.Start();
            }
            //Reset timer
            ErrorTimer.Stop();
            ErrorTimer.Start();
        }


        private static void OutputTimer_Elapsed(object sender, EventArgs e)
        {
            OutputTimer.Stop();
            if (OutputLines == null) OutputLines = new List<string>();
            if (AllOutputLines == null) AllOutputLines = new List<string>();
            OutputLines.Add(LastOutputMessage);
            AllOutputLines.Add(LastOutputMessage);
            LastOutputMessage = null;
            OnOutput?.Invoke(sender, e);
        }


        private static void ErrorTimer_Elapsed(object sender, EventArgs e)
        {
            ErrorTimer.Stop();
            if (ErrorLines == null) ErrorLines = new List<string>();
            if (AllOutputLines == null) AllOutputLines = new List<string>();
            ErrorLines.Add(LastErrorMessage);
            AllOutputLines.Add(LastErrorMessage);
            LastErrorMessage = null;
            OnError?.Invoke(sender, e);
        }


        public static void Begin()
        {
            if (IsRunning) return;
            string applicationPath = Properties.Settings.Default.SassFilePath;
            //Validate application path
            if (string.IsNullOrEmpty(applicationPath)) return;
            if (!File.Exists(applicationPath))
            {
                //TODO: log or display error for non-exising Sass application path
                return;
            }
            //Get all valid Sass files to run
            if (AllSassFiles == null) return;
            List<SassFile> ValidSassFiles = AllSassFiles.Where(x => x.IsValid).ToList();
            if (ValidSassFiles.Count <= 0) return;
            //Run a process for each file watch
            WatchProcs = new List<Process>();
            foreach (SassFile fileWatch in ValidSassFiles)
            {
                //Setup Sass application process
                Process cmd = new Process();
                WatchProcs.Add(cmd);
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                //Run main application process
                cmd.Start();
                //Set output reading
                cmd.OutputDataReceived += OutputReceived;
                cmd.ErrorDataReceived += ErrorReceived;
                cmd.BeginOutputReadLine();
                cmd.BeginErrorReadLine();
                //Change to the right directory
                FileInfo applicationFile = new FileInfo(applicationPath);
                string applicationDrive = Path.GetPathRoot(applicationPath);
                cmd.StandardInput.WriteLine(applicationDrive.Replace("\\", ""));
                cmd.StandardInput.WriteLine($"cd \"{applicationFile.DirectoryName}\"");
                //Set the watches on each Sass file
                cmd.StandardInput.WriteLine($"sass --watch \"{fileWatch.InputFilePath}\" \"{fileWatch.OutputFilePath}\"");
                //Close input
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
            }
            IsRunning = true;
        }


        public static void Stop()
        {
            if (!IsRunning) return;
            foreach (Process cmd in WatchProcs)
            {
                cmd.CancelOutputRead();
                cmd.CancelErrorRead();
                cmd.Kill();
                cmd.Dispose();
            }
            WatchProcs = null;
            IsRunning = false;
        }


        public static void Restart()
        {
            Stop();
            Begin();
        }


        public static void SaveFileLinks()
        {
            List<SassFile> FilesToSave = new List<SassFile>();
            if (AllSassFiles != null)
            {
                //Remove empty records from saving
                FilesToSave = AllSassFiles.Where(x => !string.IsNullOrEmpty(x.InputFilePath) || !string.IsNullOrEmpty(x.OutputFilePath)).ToList();
            }
            //Save file
            string encodedFileLinks = Tools.SerialiseData(FilesToSave);
            File.WriteAllText(FileLinksPath, encodedFileLinks);
        }


        public static void LoadFileLinks()
        {
            if (!File.Exists(FileLinksPath)) return;
            string encodedFileLinks = File.ReadAllText(FileLinksPath);
            List<SassFile> decodedFileLinks = Tools.DeserialiseData<List<SassFile>>(encodedFileLinks);
            AllSassFiles = decodedFileLinks;
        }


    }
}
