using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace LoginSystem.API
{
    public sealed class Auth
    {
        private readonly string adminKey;
        private IDbCommand Command { get; set; }
        public static bool IsConnected { get; private set; }


        private Auth()
        {
        }

        public Auth(string adminKey)
        {
            if (!DataBase.Instance.AdminKeyExist(adminKey))
                return;
            this.adminKey = adminKey;
        }

        ~Auth()
        {
            if (Connection.Instance != null)
                Connection.Instance.Dispose();
        }

        public bool Connect(string username, string password)
        {
            IsConnected = false;
            string hwid = string.Empty, ip = string.Empty;
            int id = 0;
            try
            {
                if (!string.IsNullOrEmpty(adminKey) && !string.IsNullOrWhiteSpace(adminKey))
                {
                    if (Tools.IsNotVPN())
                    {
                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrWhiteSpace(username) &&
                            !string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
                        {
                            if (Regex.IsMatch(username, "^[a-zA-Z]"))
                            {
                                Command = new SqlCommand("SELECT * FROM Clients WHERE Username=@username AND Password=@password", Connection.Instance);
                                Command.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar) { Value = username });
                                Command.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar) { Value = password });
                                Connection.Instance.Open();
                                IDataReader reader = Command.ExecuteReader();
                                if (reader.Read())
                                {
                                    if (reader.GetString(5) == Tools.GetHWID() || reader.GetString(5) == string.Empty)
                                    {
                                        if (reader.GetDateTime(7) > Tools.GetDateTime())
                                        {
                                            if (!reader.GetBoolean(9))
                                            {
                                                id = reader.GetInt32(0);
                                                hwid = reader.GetString(5);
                                                ip = reader.GetString(6);
                                                IsConnected = true;
                                            }
                                            else
                                                MessageBox.Show("This account is banned!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                        else
                                            MessageBox.Show("This account has expired!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                        MessageBox.Show("This computer isn't recognized!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                else
                                    MessageBox.Show("Invalid username or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                reader.Close();
                                Command.Dispose();
                                if (IsConnected)
                                {
                                    if (hwid == string.Empty)
                                    {
                                        Command = new SqlCommand("UPDATE Clients SET HWID=@hwid WHERE Id=@id", Connection.Instance);
                                        Command.Parameters.Add(new SqlParameter("@hwid", SqlDbType.VarChar) { Value = Tools.GetHWID() });
                                        Command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
                                        Command.ExecuteNonQuery();
                                        Command.Dispose();
                                    }
                                    if (ip == string.Empty)
                                    {
                                        Command = new SqlCommand("UPDATE Clients SET IPAddress=@ip WHERE Id=@id", Connection.Instance);
                                        Command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar) { Value = Tools.GetPublicIP() });
                                        Command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
                                        Command.ExecuteNonQuery();
                                        Command.Dispose();
                                    }
                                }
                                Connection.Instance.Close();
                            }
                            else
                                MessageBox.Show("The username must begin with a letter!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            string result = string.Empty;
                            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
                                result += "Username%";
                            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
                                result += "Password%";
                            result = result.Replace("%", " and ");
                            result = $"{result.First().ToString().ToUpper()}{result.Substring(1).ToLower()}";
                            MessageBox.Show($"{result}is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                        MessageBox.Show("Please disable your VPN!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("Please enter a key!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return IsConnected;
        }

        public bool Register(string username, string password, string confirmPassword, string email, string license)
        {
            bool state = false;
            try
            {
                if (!string.IsNullOrEmpty(adminKey) && !string.IsNullOrWhiteSpace(adminKey))
                {
                    if (Tools.IsNotVPN())
                    {
                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrWhiteSpace(username) &&
                            !string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password) &&
                            !string.IsNullOrEmpty(confirmPassword) && !string.IsNullOrWhiteSpace(confirmPassword) &&
                            !string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email) &&
                            !string.IsNullOrEmpty(license) && !string.IsNullOrWhiteSpace(license))
                        {
                            if (Regex.IsMatch(username, "^[a-zA-Z]"))
                            {
                                if (Regex.IsMatch(username, @"^[a-zA-Z][a-zA-Z0-9_\-]+$"))
                                {
                                    if (username.Length >= 4)
                                    {
                                        if (username.Length <= 32)
                                        {
                                            if (password == confirmPassword)
                                            {
                                                if (Regex.IsMatch(email, @"^[a-zA-Z]([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                                                {
                                                    if (Regex.IsMatch(license, @"^([a-zA-Z0-9]{5}\-){2}([a-zA-Z0-9]{5})$"))
                                                    {
                                                        if (!DataBase.Instance.UserExist(username))
                                                        {
                                                            if (!DataBase.Instance.EmailExist(email))
                                                            {
                                                                if (DataBase.Instance.LicenseExist(license))
                                                                {
                                                                    DataBase.Instance.AddClient(adminKey, username, password, email, Tools.GetPublicIP(), Tools.GetHWID(), license);
                                                                    state = true;
                                                                }
                                                                else
                                                                    MessageBox.Show("This license is invalid!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                            }
                                                            else
                                                                MessageBox.Show("This email is already used!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                        }
                                                        else
                                                            MessageBox.Show("This username is already used!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                    }
                                                    else
                                                        MessageBox.Show("The format of the license is invalid!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                }
                                                else
                                                    MessageBox.Show("The format of the email is invalid!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                            }
                                            else
                                                MessageBox.Show("Password and confirm password isn't the same!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                        else
                                            MessageBox.Show("The username must contain a maximum of 32 letters!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                        MessageBox.Show("The username must contain at least 4 letters!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                else
                                    MessageBox.Show("The format of the username is invalid!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                                MessageBox.Show("The username must begin with a letter!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            string result = string.Empty;
                            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
                                result += "Username%";
                            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
                                result += "Password%";
                            if (string.IsNullOrEmpty(confirmPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                                result += "Confirm password%";
                            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                                result += "Email%";
                            if (string.IsNullOrEmpty(license) || string.IsNullOrWhiteSpace(license))
                                result += "Licence%";
                            result += "is empty!";
                            result = result.Replace("%", ", ").Replace(", is", " is");
                            result = $"{result.First().ToString().ToUpper()}{result.Substring(1).ToLower()}";
                            MessageBox.Show($"{result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                        MessageBox.Show("Please disable your VPN!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("Please enter a key!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return state;
        }
    }
}
