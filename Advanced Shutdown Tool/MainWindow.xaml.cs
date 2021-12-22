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

namespace Advanced_Shutdown_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Global Variables
        readonly string buttonClickedText = "Done. Click to cancel shutdown";
        readonly string buttonUnclickedText = "Schedule Shutdown";
        public MainWindow()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            dateTimePicker.DefaultValue = DateTime.Now.AddHours(1);
            dateTimePicker.DisplayDefaultValueOnEmptyText = true;
            dateTimePicker.Format = Xceed.Wpf.Toolkit.DateTimeFormat.ShortTime;
        }

        /// <summary>
        /// Start the shutdown schedule based on the specified conditions
        /// </summary>
        void StartShutdown()
        {
            // show the user the shutdown has been scheduled by changing the button text
            btnScheduleShutdown.Content = buttonClickedText;

            // Shutdown based on date/time
            if (chkStudownTime.IsChecked == true)
            {
                // get the time from the picker
                DateTime? selectedTime = dateTimePicker.Value;

                // ensure the time is valid and in the future
                if (selectedTime.HasValue && (DateTime)selectedTime.Value > DateTime.Now)
                {
                    // get the seconds until the picked time and send the command to command prompt
                    TimeSpan secondsUntilShutdown = ((DateTime)selectedTime).Subtract(DateTime.Now);
                    string cmdArgument = $"shutdown -s -t {(int)secondsUntilShutdown.TotalSeconds}";
                    RunCommand(cmdArgument);
                }
                // picked time is invalid or in the past
                else
                {
                    CancelShutdown();
                }
            }
            // no shutdown condition checkbox is checked
            else
            {
                CancelShutdown();
            }
        }

        /// <summary>
        /// Stops the scheduled shutdown
        /// </summary>
        void CancelShutdown()
        {
            // revert the button text to it's default
            btnScheduleShutdown.Content = buttonUnclickedText;

            // see if the user wanted a time-based shutdown and cancel it via command prompt
            if (chkStudownTime.IsChecked == true)
            {
                RunCommand("shutdown -a");
            }
        }

        void RunCommand(string arguments)
        {
            // start a new command prompt process
            System.Diagnostics.Process cmd = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    // starts command prompt without showing the UI
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C {arguments}", // needs to start with '/C' before regular commands can be input
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            cmd.Start();
        }

        /// <summary>
        /// Button for starting a shutdown schedule
        /// </summary>
        private void btnScheduleShutdown_Click(object sender, RoutedEventArgs e)
        {
            // is shutdown to be scheduled?
            // if true, shutdown is to be scheduled
            if (btnScheduleShutdown.Content.ToString() != buttonClickedText)
            {
                StartShutdown();
            }
            // false, shutdown is to be cancelled
            else
            {
                CancelShutdown();
            }
        }
    }
}
