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
        string buttonClickedText = "Done. Click to cancel shutdown";
        string buttonUnclickedText = "Schedule Shutdown";
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start the shutdown schedule based on the specified conditions
        /// </summary>
        void StartShutdown()
        {
            btnScheduleShutdown.Content = buttonClickedText;

            // Is a shutdown time sheculed
            if (chkStudownTime.IsChecked == true)
            {
                string inputTime = txtTime.Text;
                DateTime shutdownTime = new DateTime();

                if (DateTime.TryParse(inputTime, out shutdownTime))
                {
                    TimeSpan secondsUntilShutdown = ParseDateTimeLikeAHuman(shutdownTime);
                    string cmdArgument = $"shutdown -s -t {(int)secondsUntilShutdown.TotalSeconds}";

                    RunCommand(cmdArgument);
                }
                else
                {
                    CancelShutdown();
                }
            }
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
            btnScheduleShutdown.Content = buttonUnclickedText;

            if (chkStudownTime.IsChecked == true)
            {
                RunCommand("shutdown -a");
            }
        }

        void RunCommand(string arguments)
        {
            System.Diagnostics.Process cmd = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C {arguments}",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            cmd.Start();
            //txtOutput.Text = arguments;
        }

        /// <summary>
        /// Attempts to solve for date time formatted without specifying am or pm
        /// </summary>
        TimeSpan ParseDateTimeLikeAHuman(DateTime input)
        {
            TimeSpan output = new TimeSpan();
            output = input.Subtract(DateTime.Now);

            // check to see if it was able to parse a time in the future
            if (output.TotalMilliseconds < 0)
            {
                // try adding 12 hours incase military time wasn't used
                output = output.Add(TimeSpan.FromHours(12));

                if (output.TotalMilliseconds < 0)
                {
                    CancelShutdown();
                    return TimeSpan.MaxValue;
                }
            }
            return output;
        }

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
