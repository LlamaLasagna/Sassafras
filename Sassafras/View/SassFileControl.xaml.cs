using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sassafras
{
    /// <summary>
    /// Interaction logic for SassFileControl.xaml
    /// </summary>
    public partial class SassFileControl : UserControl
    {
        // PROPERTIES

        public SassFile FileLink { private set; get; }

        public EventHandler OnRemove;
        public EventHandler OnUpdate;


        // CONSTRUCTOR

        public SassFileControl(SassFile fileLink = null)
        {
            InitializeComponent();
            //Set the Sass file link
            FileLink = fileLink;
            if (FileLink == null)
            {
                FileLink = new SassFile();
            }
            //Set the file selector values
            SyncInputs();
        }


        // METHODS

        private void SyncInputs()
        {
            TxtInput.Text = FileLink.InputFilePath;
            TxtOutput.Text = FileLink.OutputFilePath;
            //Set default output value
            if (
                !string.IsNullOrEmpty(FileLink.InputFilePath) && string.IsNullOrEmpty(FileLink.OutputFilePath) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.DefaultOutputDirectory)
            )
            {
                string inputFileName = System.IO.Path.GetFileNameWithoutExtension(FileLink.InputFilePath);
                FileLink.OutputFilePath = Properties.Settings.Default.DefaultOutputDirectory + "\\" + inputFileName + ".css";
                SyncInputs();
            }
        }


        // EVENT HANDLERS

        private void BtnBrowseInput_Click(object sender, RoutedEventArgs e)
        {
            string defaultPath = FileLink.InputFilePath;
            if (string.IsNullOrEmpty(defaultPath) && !string.IsNullOrEmpty(Properties.Settings.Default.DefaultInputDirectory))
            {
                defaultPath = Properties.Settings.Default.DefaultInputDirectory;
            }
            string selectedFilePath = FilePick.OpenFileSelector(defaultPath);
            if (selectedFilePath != null)
            {
                FileLink.InputFilePath = selectedFilePath;
                SyncInputs();
            }
            OnUpdate?.Invoke(this, e);
        }


        private void BtnBrowseOutput_Click(object sender, RoutedEventArgs e)
        {
            string defaultPath = FileLink.OutputFilePath;
            if (string.IsNullOrEmpty(defaultPath) && !string.IsNullOrEmpty(Properties.Settings.Default.DefaultOutputDirectory))
            {
                defaultPath = Properties.Settings.Default.DefaultOutputDirectory;
            }
            string selectedFilePath = FilePick.OpenFileSelector(FileLink.OutputFilePath, false);
            if (selectedFilePath != null)
            {
                FileLink.OutputFilePath = selectedFilePath;
                SyncInputs();
            }
            OnUpdate?.Invoke(this, e);
        }


        private void BtnRemove_Click(object sender, MouseButtonEventArgs e)
        {
            OnRemove?.Invoke(this, e);
        }


    }
}
