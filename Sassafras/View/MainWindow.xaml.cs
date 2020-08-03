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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sassafras
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // PROPERTIES

        private TaskTrayIcon TrayIcon;


        // CONSTRUCTOR

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                //Initialise task tray icon
                TrayIcon = new TaskTrayIcon();
                TrayIcon.AddMenuItem("Settings", TrayIcon_Settings);
                TrayIcon.AddMenuItem("Restart", TrayIcon_Restart);
                TrayIcon.AddMenuItem("Exit", TrayIcon_Exit);
            }
            catch (Exception ex)
            {
                HandleCriticalError(ex, "Error initialising application.");
            }

            try
            {
                //Initialise Sass Handler
                SassHandler.OnError += SassHandler_Error;
                SassHandler.LoadFileLinks();
                SassHandler.Begin();
            }
            catch (Exception ex)
            {
                HandleError(ex, "Error starting Sass");
            }
        }


        // METHODS

        public void HandleError(Exception ex, string userMessage = null)
        {
            //Log and display the error message
            try
            {
                Tools.LogError(ex);
            }
            catch (Exception)
            {
                //Error logging error. There is no hope!
            }

            if (userMessage == null)
            {
                userMessage = ex.Message;
            }
            else
            {
                userMessage += "\r\n \r\nException message:\r\n" + ex.Message;
            }
            MessageBox.Show(userMessage, "Sassafras Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        public void HandleCriticalError(Exception ex, string userMessage = null)
        {
            //Handle the exception and shut down the application
            HandleError(ex, userMessage);
            CloseApplication();
        }


        public void HandleSassError()
        {
            Focus();
            string errorMessage = SassHandler.ErrorLines.Last();
            MessageBox.Show("An error occurred with Sass:\r\n\r\n" + errorMessage, "Sassafras Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private void CloseApplication()
        {
            //Ensure Sass has stopped before closing
            SassHandler.Stop();
            //Ensure the task tray icon is disposed before closing
            TrayIcon.Close();
            //Shut down the entire application
            Application.Current.Shutdown();
        }


        private void OpenSettingsWindow()
        {
            SettingsWindow winSettings = new SettingsWindow();
            winSettings.Show();
        }


        private void RestartSass()
        {
            try
            {
                SassHandler.Restart();
            }
            catch (Exception ex)
            {
                HandleError(ex, "Error starting Sass");
            }
        }


        // EVENT HANDLERS

        private void Window_Closed(object sender, EventArgs e)
        {
            CloseApplication();
        }


        private void TrayIcon_Exit(object sender, EventArgs e)
        {
            CloseApplication();
        }


        private void TrayIcon_Settings(object sender, EventArgs e)
        {
            OpenSettingsWindow();
        }


        private void TrayIcon_Restart(object sender, EventArgs e)
        {
            RestartSass();
        }


        private void SassHandler_Error(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                HandleSassError();
            });
        }


    }
}
