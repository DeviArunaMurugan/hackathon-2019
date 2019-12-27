using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Flutter_Publish_Utility
{
    /// <summary>
    /// Interaction logic for APKGeneration.xaml
    /// </summary>
    public partial class APKGeneration : Page
    {
        List<String> controlList;
        public APKGeneration()
        {
            InitializeComponent();
            controlList = new List<string>();
            controlList.Add("Core");
            controlList.Add("Chart");
            controlList.Add("Gauge");
            controlList.Add("Calender");
            combobox.ItemsSource = controlList;
            combobox.DefaultText = "Select the controls";
        }

        /// <summary>
        /// Method to generate .apk
        /// </summary>
        /// <param name="sender">The button</param>
        /// <param name="e">Routed event args</param>
        private void GenerateAPK_Click(object sender, RoutedEventArgs e)
        {
            string directory = getProjectDirectory();
            if (!string.IsNullOrEmpty(directory))
            {
                string command = "flutter analyze";
                executePowershell(directory, command);
            }
        }

        private void GitClone_Click(object sender, RoutedEventArgs e)
        {
            string controlName = string.Empty;
            string branch = string.Empty;
            List<string> selectedItems = combobox.SelectedItems.Cast<string>().ToList();
            for (int i = 0; i < selectedItems.Count; i++)
            {
                string currentDirectory = getProjectDirectory();
                if (!string.IsNullOrEmpty(currentDirectory))
                {
                    if (selectedItems[i] == "Core")
                    {
                        controlName = "core";
                    }
                    else if (selectedItems[i] == "Chart")
                    {
                        controlName = "charts";
                    }
                    else if (selectedItems[i] == "Gauge")
                    {
                        controlName = "gauges";
                    }
                    else if (selectedItems[i] == "Calender")
                    {
                        controlName = "calendar";
                    }

                    if (branchComboBox.SelectedIndex == 0)
                    {
                        branch = "development";
                    }
                    else
                    {
                        branch = "master";
                    }

                    cloneFromRepo(currentDirectory, controlName, branch);
                }
            }
        }

        private void cloneFromRepo(string currentDirectory, string controlName, string branch)
        {
            var Restoreprocess = new System.Diagnostics.Process();
            Restoreprocess.StartInfo.FileName = @"git";
            Restoreprocess.StartInfo.WorkingDirectory = currentDirectory;
            Restoreprocess.StartInfo.Arguments = @"clone " + "https://gitlab.syncfusion.com/essential-studio/flutter-" + controlName.ToString() + " -b " + branch.ToString();

            Restoreprocess.StartInfo.UseShellExecute = false;
            Restoreprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Restoreprocess.StartInfo.Verb = "runas";

            Restoreprocess.Start();
            Restoreprocess.WaitForExit();
        }

        private void executePowershell(string location, string command)
        {
            var Restoreprocess = new Process();
            Restoreprocess.StartInfo.FileName = "cmd.exe";
            Restoreprocess.StartInfo.WorkingDirectory = location;
            Restoreprocess.StartInfo.Arguments = "/c " + command;
            Restoreprocess.StartInfo.UseShellExecute = false;
            Restoreprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Restoreprocess.StartInfo.Verb = "runas";
            Restoreprocess.StartInfo.RedirectStandardOutput = true;
            Restoreprocess.Start();
            var output = Restoreprocess.StandardOutput.ReadToEnd();

            if (output.Contains("No issues found!"))
            {
                System.Windows.MessageBox.Show("Flutter Analyzed successfully");
            }
            else
            {
                int index = output.IndexOf("issues found");
                if (index == -1)
                {
                    System.Windows.MessageBox.Show("issues found");
                    return;
                }
                System.Windows.MessageBox.Show(output[index - 3] + " issues found");
            }

            Restoreprocess.WaitForExit();
            Restoreprocess.StartInfo.Arguments = "/c " + "flutter build apk";
            Restoreprocess.Start();
            output = Restoreprocess.StandardOutput.ReadToEnd();
            Restoreprocess.WaitForExit();
            if (output.Contains(@"build\app\outputs\apk\release\"))
            {
                sendAttachment(location + @"\build\app\outputs\apk\release\app-release.apk");
            }
            else
            {
                System.Windows.MessageBox.Show("Apk generation failed");
            }
        }

        private void sendAttachment(string attachment)
        {
            string to = "dharanidharan.dharmasivam@syncfusion.com";
            Microsoft.Office.Interop.Outlook.MailItem message = new Microsoft.Office.Interop.Outlook.Application().CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            message.To = to;
            message.Subject = "Testing";
            Microsoft.Office.Interop.Outlook.Attachment attachement = message.Attachments.Add(attachment);
            message.Body = "Test Mail";
            message.Send();
            System.Windows.MessageBox.Show("Message Sent to " + to);
        }


        /// <summary>
        /// To get desired project directory
        /// </summary>
        /// <returns></returns>
        private string getProjectDirectory()
        {
            using (var folderPath = new FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = folderPath.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderPath.SelectedPath))
                {
                    return folderPath.SelectedPath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private void GenerateAppBundle_Click(object sender, RoutedEventArgs e)
        {
            string directory = getProjectDirectory();
            if (!string.IsNullOrEmpty(directory))
            {
                string command = "flutter build appbundle";
                RunPowerShell(directory, command);
            }
        }

        /// <summary>
        /// Method to run the power shell
        /// </summary>
        /// <param name="location">Project Directory</param>
        /// <param name="command">Flutter command</param>
        private void RunPowerShell(string location, string command)
        {
            var Restoreprocess = new Process();
            Restoreprocess.StartInfo.FileName = "cmd.exe";
            Restoreprocess.StartInfo.WorkingDirectory = location;
            Restoreprocess.StartInfo.Arguments = "/c " + command;
            Restoreprocess.StartInfo.UseShellExecute = false;
            Restoreprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Restoreprocess.StartInfo.Verb = "runas";

            Restoreprocess.Start();
            Restoreprocess.WaitForExit(60000);
            System.Windows.MessageBox.Show("Done");
        }
    }
}
