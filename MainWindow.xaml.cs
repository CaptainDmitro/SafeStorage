﻿using System;
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
                MessageBox.Show(Strings.ERR_LOGIN, Strings.ERROR_MSG);
            }
        }

        private void Button_Sign_Up_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationWindow().Show();
        }
    }
}
