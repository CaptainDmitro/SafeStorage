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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SafeStorage
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_Username.Text.Length == 0)
            {
                MessageBox.Show(Strings.LOGIN_IS_EMPTY, Strings.ERR_MSG);
            }
            else if(PasswordBox_Password.Password.Length == 0 || PasswordBox_Password_Repeat.Password.Length == 0)
            {
                MessageBox.Show(Strings.PSW_IS_EMPTY, Strings.ERR_MSG);
            }
            else if (!PasswordBox_Password.Password.Equals(PasswordBox_Password_Repeat.Password))
            {
                MessageBox.Show(Strings.PSW_NOT_EQUAL, Strings.ERR_MSG);
            }
            else
            {
                SshClient client = new SshClient(Settings.HOST, 22, Settings.NAME, Settings.KEY);
                client.Connect();

                IDictionary<Renci.SshNet.Common.TerminalModes, uint> modes = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
                modes.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);

                ShellStream shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, modes);
                var output = shellStream.Expect(new Regex(@"[$>]"));

                shellStream.WriteLine($"sudo sh addUser.sh {TextBox_Username.Text} {PasswordBox_Password.Password}");
                output = shellStream.Expect(new Regex(@"([$#>:])"));
                shellStream.WriteLine(Settings.KEY);
                output = shellStream.Expect(new Regex(@"([$#>:])"));

                client.Disconnect();

                MessageBox.Show(Strings.REG_SUCCESS, Strings.NOTIFICATION);
                Close();
            }
        }
    }
}
