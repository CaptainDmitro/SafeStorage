using Renci.SshNet;
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
using System.IO;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SafeStorage
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        private SftpClient client;
        private byte[] key;
        private CryptoSystem cs;
        public ControlWindow(SftpClient client, byte[] key)
        {
            InitializeComponent();

            this.client = client;
            this.key = key;
            cs = new CryptoSystem();
            Init();
        }

        ~ControlWindow()
        {
            client.Disconnect();
        }

        private void Init()
        {
            client.ChangeDirectory("/upload");
            UpdateFileList();
        }

        private void UpdateFileList()
        {
            var fileList = client.ListDirectory(client.WorkingDirectory).Select(s => s.Name).OrderBy(s => s).Skip(2).ToList();
            ListBox_Files.ItemsSource = fileList;
        }

        private void Button_Download_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                DialogResult result = dlg.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string downloadPath = dlg.SelectedPath + @"\";
                    string selectedFile = ListBox_Files.SelectedItem.ToString();

                    using (var uplfileStream = System.IO.File.Create(downloadPath + selectedFile))
                    {
                        client.DownloadFile(selectedFile, uplfileStream);
                    }

                    cs.Decrypt(downloadPath + selectedFile, key);
                    File.Delete(downloadPath + selectedFile);
                }
            } 
            catch (NullReferenceException)
            {
                MessageBox.Show("Select a file from the list to download", "No file selected");
            }

            UpdateFileList();
        }

        private void Button_Upload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string fullPath = dlg.FileName;
                string fileName = System.IO.Path.GetFileName(dlg.FileName);
                
                cs.Encrypt(fullPath, key);
                fullPath += Settings.ENCRYPTION_EXTENSION;
                fileName += Settings.ENCRYPTION_EXTENSION;

                using (var uplfileStream = System.IO.File.OpenRead(fullPath))
                {
                    client.UploadFile(uplfileStream, fileName, true);
                }

                File.Delete(fullPath);
            }

            UpdateFileList();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedFile = ListBox_Files.SelectedItem.ToString();
                client.DeleteFile(selectedFile);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Select a file from the list to delete", "No file selected");
            }

            UpdateFileList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
            System.Windows.Application.Current.Shutdown();
        }
    }
}
