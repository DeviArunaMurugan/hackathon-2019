using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Pub_publish.xaml
    /// </summary>
    public partial class Pub_publish : Page
    {
        List<String> controlList;
        public Pub_publish()
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


        private void cloneFromRepo(string currentDirectory ,string controlName ,string branch)
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

        private void PubPublish_Click(object sender, RoutedEventArgs e)
        {
            bool isWarningsError = false;
            string location = getProjectDirectory();
            if (!isWarningsError)
            {
                string[] deletingDirectories = { "test", "build", "images" };
                string[] fileExistList = { "analysis_options.yaml", "CHANGELOG.md", "pubspec.yaml", "README.md", "syncfusion_flutter_charts.iml" };
                string[] folderExistList = { "example", "lib" };
                if (!string.IsNullOrEmpty(location))
                {
                    Remove_Directories(deletingDirectories, location);
                    Remove_TestScript_Reference(location);
                    bool isFoldersExist = Folder_Exist_Check(folderExistList, location);
                    bool isFilesExist = File_Exist_Check(fileExistList, location);
                    bool isProperPub = Pubspec_Check(location, versionTextBlock.Text);
                    string command = "dartfmt -w lib";
                    RunPowerShell(location, command);
                }
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

        private bool File_Exist_Check(string[] filesList, string location)
        {
            bool isFilesExist = false;
            for (int k = 0; k < filesList.Length; k++)
            {
                if (File.Exists(location + @"\" + filesList[k])) isFilesExist = true;
                else
                {
                    isFilesExist = false;
                    System.Windows.MessageBox.Show(filesList[k] + " file doesn't exist");
                    break;
                }
            }
            return isFilesExist;
        }


        private bool Folder_Exist_Check(string[] foldersList, string location)
        {
            bool isFoldersExist = false;
            for (int k = 0; k < foldersList.Length; k++)
            {
                if (Directory.Exists(location + @"\" + foldersList[k])) isFoldersExist = true;
                else
                {
                    isFoldersExist = false;
                    System.Windows.MessageBox.Show(foldersList[k] + " folder doesn't exist");
                    break;
                }
            }
            return isFoldersExist;
        }

        private bool Pubspec_Check(string location, string version)
        {
            bool isPub = false;
            string[] controlSeperator = new string[] { "syncfusion_flutter_" };
            string pubSpec = File.ReadAllText(location + @"\pubspec.yaml", Encoding.UTF8);
            MatchCollection splittedTextCollection = Regex.Matches(pubSpec, @"name: (.*)\s*description: (.*)\sversion: (.*)");
            string[] nameList = splittedTextCollection[0].Groups[1].Value.ToString().Split(controlSeperator, StringSplitOptions.None);
            int descriptionLength = splittedTextCollection[0].Groups[2].Value.ToString().Length;
            bool isVersion = version == splittedTextCollection[0].Groups[3].Value.ToString().Trim() ? true : false;
            if (nameList[0] != "") System.Windows.MessageBox.Show("Package name doesn't contain the syncfusion_flutter_ word");
            if (descriptionLength < 60 || descriptionLength > 180) System.Windows.MessageBox.Show("Constrol description in pubspec length must be greater than 60 and less than 180");
            if (!isVersion) System.Windows.MessageBox.Show("Version doesn't match");
            if (nameList[0] == "" && (descriptionLength > 60 && descriptionLength < 180) && isVersion) isPub = true;
            else isPub = false;
            return isPub;
        }
        private void Remove_Directories(string[] deleteDirectories, string location)
        {
            for (int i = 0; i < deleteDirectories.Length; i++)
            {
                if (deleteDirectories[i] == "test" && Directory.Exists(location + @"\lib\src\" + deleteDirectories[i]))
                {
                    Directory.Delete(location + @"\lib\src\" + deleteDirectories[i], true);
                }
                else if (Directory.Exists(location + @"\" + deleteDirectories[i]))
                {
                    Directory.Delete(location + @"\" + deleteDirectories[i], true);
                }
            }
        }
        private void Remove_TestScript_Reference(string location)
        {
            string[] controlSeperator = new string[] { "syncfusion_flutter_" };
            string pubSpec = File.ReadAllText(location + @"\pubspec.yaml", Encoding.UTF8);
            MatchCollection splittedTextCollection = Regex.Matches(pubSpec, @"name: (.*)\s*description: (.*)\sversion: (.*)");
            string controlNameList = splittedTextCollection[0].Groups[0].Value.ToString();
            string[] locationList = splittedTextCollection[0].Groups[1].Value.ToString().Split(controlSeperator, StringSplitOptions.None);
            string controlName = locationList[locationList.Length - 1].ToString().Trim();
            string scriptReferenceText = File.ReadAllText(location + @"\lib\" + controlName + ".dart", Encoding.UTF8);

            if (scriptReferenceText.IndexOf("// export test scripts") > 0)
            {
                scriptReferenceText = scriptReferenceText.Remove(scriptReferenceText.IndexOf("// export test scripts"));
                File.WriteAllText(location + @"\lib\" + controlName + ".dart", scriptReferenceText);
            }
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

                if (result == System.Windows.Forms.DialogResult.OK
                    && !string.IsNullOrWhiteSpace(folderPath.SelectedPath))
                {
                    return folderPath.SelectedPath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
