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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
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
                new ControlWindow(client, CryptoSystem.GenerateKey(Encoding.UTF8.GetBytes(PasswordBox_Password.Password))).Show();
            }
            catch (Renci.SshNet.Common.SshAuthenticationException)
            {
                MessageBox.Show(Strings.ERR_LOGIN, Strings.ERR_MSG);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Sign_Up_Click(object sender, RoutedEventArgs e)
        {
            //new RegistrationWindow().Show();
            if (TextBox_Username.Text.Length == 0)
            {
                MessageBox.Show(Strings.LOGIN_IS_EMPTY, Strings.ERR_MSG);
            }
            else if (PasswordBox_Password.Password.Length == 0)
            {
                MessageBox.Show(Strings.PSW_IS_EMPTY, Strings.ERR_MSG);
            }
            else
            {
                SshClient client = new SshClient(Settings.HOST, 22, Settings.NAME, Settings.KEY);
                client.Connect();

                IDictionary<Renci.SshNet.Common.TerminalModes, uint> modes = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
                modes.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);

                ShellStream shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, modes);
                var output = shellStream.Expect(new Regex(@"[$>]"));

                shellStream.WriteLine($"{Settings.RUN_REG_SCRIPT} {TextBox_Username.Text} {PasswordBox_Password.Password}");
                output = shellStream.Expect(new Regex(@"([$#>:])"));
                shellStream.WriteLine(Settings.KEY);
                output = shellStream.Expect(new Regex(@"([$#>:])"));

                client.Disconnect();

                MessageBox.Show(Strings.REG_SUCCESS, Strings.NOTIFICATION);
            }
        }
    }
}
