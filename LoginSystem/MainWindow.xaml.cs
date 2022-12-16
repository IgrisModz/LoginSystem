using LoginSystem.API;
using MahApps.Metro.Controls;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LoginSystem.MessageBox;

namespace LoginSystem
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocalAuth.auth.Connect(UsernameTxt.Text, PasswordTxt.Password))
                MetroMessageBox.Show($"Welcome {UsernameTxt.Text} to LoginSystem", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void LoginBtn_KeyUp(object sender, KeyEventArgs e)
        {
            if (UsernameTxt.Text != string.Empty && PasswordTxt.Password != string.Empty)
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    if (LocalAuth.auth.Connect(UsernameTxt.Text, PasswordTxt.Password))
                        MetroMessageBox.Show("Login Succeded!", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow register = new RegisterWindow { Owner = this };
            Hide();
            register.ShowDialog();
            Show();
        }
    }
}
