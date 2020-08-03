using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sassafras
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        // PROPERTIES

        private List<SassFile> AllSassFiles
        {
            get { return SassHandler.AllSassFiles; }
            set { SassHandler.AllSassFiles = value; }
        }


        // CONSTRUCTOR

        public SettingsWindow()
        {
            InitializeComponent();
            //Display existing settings
            SyncSettingsInputs();
            //Add Sass file selectors
            if (AllSassFiles.Count == 0)
            {
                AddSassFile();
            }
            else
            {
                foreach (SassFile fileLink in AllSassFiles)
                {
                    AddSassFile(fileLink);
                }
            }
            //Set log event handlers
            SassHandler.OnOutput += SassHandler_Output;
            SassHandler.OnError += SassHandler_Output;
            //Initialise the console tab
            UpdateSassConsole();
        }


        // METHODS

        private void SyncSettingsInputs()
        {
            TxtSassPath.Text = Properties.Settings.Default.SassFilePath;
            TxtDefaultInput.Text = Properties.Settings.Default.DefaultInputDirectory;
            TxtDefaultOutput.Text = Properties.Settings.Default.DefaultOutputDirectory;
        }


        private void AddSassFile(SassFile fileLink = null)
        {
            //Add a new Sass File to the list
            if (fileLink == null)
            {
                fileLink = new SassFile();
                AllSassFiles.Add(fileLink);
            }
            //Create a new input control for the Sass File
            SassFileControl fileControl = new SassFileControl(fileLink);
            fileControl.OnRemove += BtnRemoveFile_Click;
            fileControl.OnUpdate += FileUpdated;
            FileLinkList.Children.Add(fileControl);
        }


        private void RemoveSassFile(SassFileControl fileControl)
        {
            AllSassFiles.Remove(fileControl.FileLink);
            FileLinkList.Children.Remove(fileControl);
            SassHandler.SaveFileLinks();
            SassHandler.Restart();
            //Prevent the last file being removed
            if (AllSassFiles.Count == 0)
            {
                AddSassFile();
            }
        }


        private void UpdateSassConsole()
        {
            if (SassHandler.AllOutputLines == null || SassHandler.AllOutputLines.Count <= 0)
            {
                TxtOutputLog.Text = "";
            }
            else
            {
                TxtOutputLog.Text = string.Join("\r\n", SassHandler.AllOutputLines);
                TxtOutputLog.ScrollToEnd();
            }
        }


        // EVENT HANDLERS

        private void BtnBrowseSass_Click(object sender, RoutedEventArgs e)
        {
            string selectedFilePath = FilePick.OpenFileSelector(Properties.Settings.Default.SassFilePath);
            if (selectedFilePath != null)
            {
                Properties.Settings.Default.SassFilePath = selectedFilePath;
                Properties.Settings.Default.Save();
                SyncSettingsInputs();
                //Attempt to restart Sass using new path
                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    SassHandler.Restart();
                }
            }
        }


        private void BtnBrowseDefaultInput_Click(object sender, RoutedEventArgs e)
        {
            string selectedFilePath = FilePick.OpenDirectorySelector(Properties.Settings.Default.DefaultInputDirectory);
            if (selectedFilePath != null)
            {
                Properties.Settings.Default.DefaultInputDirectory = selectedFilePath;
                Properties.Settings.Default.Save();
                SyncSettingsInputs();
            }
        }


        private void BtnBrowseDefaultOutput_Click(object sender, RoutedEventArgs e)
        {
            string selectedFilePath = FilePick.OpenDirectorySelector(Properties.Settings.Default.DefaultOutputDirectory);
            if (selectedFilePath != null)
            {
                Properties.Settings.Default.DefaultOutputDirectory = selectedFilePath;
                Properties.Settings.Default.Save();
                SyncSettingsInputs();
            }
        }


        private void BtnAddFile_Click(object sender, RoutedEventArgs e)
        {
            //Check that the last Sass File in the list is not empty
            if (AllSassFiles != null && AllSassFiles.Count > 0)
            {
                SassFile lastSassFile = AllSassFiles.Last();
                if (lastSassFile == null || string.IsNullOrEmpty(lastSassFile.InputFilePath) || string.IsNullOrEmpty(lastSassFile.OutputFilePath))
                {
                    return;
                }
            }
            AddSassFile();
        }


        private void BtnRemoveFile_Click(object sender, EventArgs e)
        {
            SassFileControl removeControl = (SassFileControl)sender;
            MessageBoxResult userResp = MessageBox.Show("Really remove this Sass file from the watch list?", "Are you sure?", MessageBoxButton.OKCancel);
            if (userResp == MessageBoxResult.OK || userResp == MessageBoxResult.Yes)
            {
                RemoveSassFile(removeControl);
            }
        }


        private void FileUpdated(object sender, EventArgs e)
        {
            SassHandler.SaveFileLinks();
            SassHandler.Restart(); //Could this be too much?
        }


        //private void BtnSaveFileLinks_Click(object sender, RoutedEventArgs e)
        //{
        //    SassHandler.SaveFileLinks();
        //    SassHandler.Restart();
        //}


        private void SassHandler_Output(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateSassConsole();
            });
        }


    }
}
