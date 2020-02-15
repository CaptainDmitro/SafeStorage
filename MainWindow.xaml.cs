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
using Renci.SshNet;


namespace SafeStorage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Fun()
        {
            var connectionInfo = new ConnectionInfo(Settings.HOST,
                                        "testu",
                                        new PasswordAuthenticationMethod("testu", "2"));
            using (var client = new SftpClient(connectionInfo))
            {
                client.Connect();
                client.ChangeDirectory("/upload");
                string fileName = "C:\\Users\\Dmitry\\Downloads\\test.txt";
                using (var uplfileStream = System.IO.File.OpenRead(fileName))
                {
                    client.UploadFile(uplfileStream, fileName, true);
                }
                client.Disconnect();
            }
        }

        private void Button_Sign_In_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var connectionInfo = new ConnectionInfo(Settings.HOST,
                                            TextBox_Username.Text,
                                            new PasswordAuthenticationMethod(TextBox_Username.Text, PasswordBox_Password.Password));
                var client = new SftpClient(connectionInfo);
                client.Connect();
                Hide();
                new ControlWindow(client, CryptoSystem.GenerateFirstKey(PasswordBox_Password.Password).Substring(0, 8)).Show();
            }
            catch (Renci.SshNet.Common.SshAuthenticationException)
            {
                MessageBox.Show("Wrong login or password!", "Authentication error");
            }
        }

    }
}
