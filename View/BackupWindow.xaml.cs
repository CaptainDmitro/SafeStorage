using Renci.SshNet;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SafeStorage.View
{
    /// <summary>
    /// Interaction logic for BackupWindow.xaml
    /// </summary>
    public partial class BackupWindow : Window
    {
        private SftpClient sftpClient;
        public BackupWindow(SftpClient sftpClient)
        {
            InitializeComponent();
            this.sftpClient = sftpClient;
        }

        private void Button_Backup_Click(object sender, RoutedEventArgs e)
        {
            SshClient client = new SshClient(Settings.HOST, 22, Settings.NAME, Settings.KEY);
            client.Connect();

            IDictionary<Renci.SshNet.Common.TerminalModes, uint> modes = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            modes.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);

            ShellStream shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, modes);
            var output = shellStream.Expect(new Regex(@"[$>]"));
            Console.WriteLine("O: " + output);

            shellStream.WriteLine($"sudo sh backup.sh {sftpClient.ConnectionInfo.Username}");
            output = shellStream.Expect(new Regex(@"([$#>:])"));
            Console.WriteLine("O: " + output);
            shellStream.WriteLine(Settings.KEY);
            output = shellStream.Expect(new Regex(@"([$#>:])"));
            Console.WriteLine("O: " + output);

            if (CheckBox_Backup_Server.IsChecked == true)
            {
                System.Windows.MessageBox.Show(Strings.BACKUP_SERVER_SUCCESS, Strings.NOTIFICATION);
            }
            if (CheckBox_Backup_Client.IsChecked == true)
            {
                try
                {
                    FolderBrowserDialog dlg = new FolderBrowserDialog();
                    DialogResult result = dlg.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        string downloadPath = dlg.SelectedPath + @"\";
                        string selectedFile = Settings.BACKUP_FILENAME;

                        using (var uplfileStream = System.IO.File.Create(downloadPath + selectedFile))
                        {
                            sftpClient.DownloadFile("." + selectedFile, uplfileStream);
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    System.Windows.MessageBox.Show(Strings.NO_FILE_SELECTED_DOWNLOAD, Strings.NO_FILE_SELECTED);
                }
            }

            client.Disconnect();
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Close();
        }
    }
}
