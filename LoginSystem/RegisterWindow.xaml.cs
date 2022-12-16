using LoginSystem.API;
using MahApps.Metro.Controls;
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
using LoginSystem.MessageBox;

namespace LoginSystem
{
    /// <summary>
    /// Logique d'interaction pour Register.xaml
    /// </summary>
    public partial class RegisterWindow : MetroWindow
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            if(LocalAuth.auth.Register(UsernameTxt.Text, PasswordTxt.Password, ConfirmPasswordTxt.Password, EmailTxt.Text, LicenseTxt.Text))
                MetroMessageBox.Show("Register Success", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void RegisterBtn_KeyUp(object sender, KeyEventArgs e)
        {
            if (UsernameTxt.Text != string.Empty && PasswordTxt.Password != string.Empty && ConfirmPasswordTxt.Password != string.Empty && EmailTxt.Text != string.Empty && LicenseTxt.Text != string.Empty)
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    if (LocalAuth.auth.Register(UsernameTxt.Text, PasswordTxt.Password, ConfirmPasswordTxt.Password, EmailTxt.Text, LicenseTxt.Text))
                    {
                        Close();
                        MetroMessageBox.Show("Register Success", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
            }
        }
    }
}
