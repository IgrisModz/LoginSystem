using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Cache;
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

namespace LoginSystem.AdminPanel
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        readonly LicenseSystemEntities context = new LicenseSystemEntities();
        readonly CollectionViewSource clientsViewSource;
        public MainWindow()
        {
            InitializeComponent();
            clientsViewSource = ((CollectionViewSource)(this.FindResource("clientsViewSource")));
            DataContext = this;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Load is an extension method on IQueryable,    
            // defined in the System.Data.Entity namespace.   
            // This method enumerates the results of the query,    
            // similar to ToList but without creating a list.   
            // When used with Linq to Entities, this method    
            // creates entity objects and adds them to the context.   
            context.Clients.Load();

            // After the data is loaded, call the DbSet<T>.Local property    
            // to use the DbSet<T> as a binding source.   
            clientsViewSource.Source = context.Clients.Local;
        }

        private void DeleteCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Clients clients = (Clients)usersDataGrid.SelectedItem;
            // Create a new object because the old one  
            // is being tracked by EF now.  
            Clients c = context.Clients.First(i => i.Username == clients.Username);
            c.Username = ChangeUsernameTxt.Text;
            context.Clients.Remove(c);

            clientsViewSource.View.Refresh();
            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.
            context.SaveChanges();
        }

        private void UpdateUsernameCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Clients clients = (Clients)usersDataGrid.SelectedItem;
            // Create a new object because the old one  
            // is being tracked by EF now.  
            Clients c = context.Clients.First(i => i.Username == clients.Username);
            c.Username = ChangeUsernameTxt.Text;

            clientsViewSource.View.Refresh();
            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.
            context.SaveChanges();
        }

        private void UpdatePasswordCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Clients clients = (Clients)usersDataGrid.SelectedItem;
            // Create a new object because the old one  
            // is being tracked by EF now.  
            Clients c = context.Clients.First(i => i.Username == clients.Username);
            c.Password = ChangePasswordTxt.Text;

            clientsViewSource.View.Refresh();
            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.
            context.SaveChanges();
        }

        private void UpdateEmailCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Clients clients = (Clients)usersDataGrid.SelectedItem;
            // Create a new object because the old one  
            // is being tracked by EF now.  
            Clients c = context.Clients.First(i => i.Username == clients.Username);
            c.Email = ChangeEmailTxt.Text;

            clientsViewSource.View.Refresh();
            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.
            context.SaveChanges();
        }

        private void UpdateExpirationCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Clients clients = (Clients)usersDataGrid.SelectedItem;
            // Create a new object because the old one  
            // is being tracked by EF now.  
            Clients c = context.Clients.First(i => i.Username == clients.Username);
            DateTime date = GetDateTime();
            switch (ExpirationCmB.SelectedIndex)
            {
                case 0:
                    c.ExpirationDate = date;
                    break;
                case 1:
                    c.ExpirationDate = date.AddDays(1);
                    break;
                case 2:
                    c.ExpirationDate = date.AddDays(7);
                    break;
                case 3:
                    c.ExpirationDate = date.AddMonths(1);
                    break;
                case 4:
                    c.ExpirationDate = date.AddMonths(3);
                    break;
                case 5:
                    c.ExpirationDate = date.AddMonths(6);
                    break;
                case 6:
                    c.ExpirationDate = date.AddYears(1);
                    break;
                case 7:
                    c.ExpirationDate = DateTime.MaxValue;
                    break;
            }

            clientsViewSource.View.Refresh();
            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.
            context.SaveChanges();
        }

        private void UpdateBanCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Clients clients = (Clients)usersDataGrid.SelectedItem;
            // Create a new object because the old one  
            // is being tracked by EF now.  
            Clients c = context.Clients.First(i => i.Username == clients.Username);
            c.Banned = !clients.Banned;

            clientsViewSource.View.Refresh();
            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.
            context.SaveChanges();
        }

        private void ResetHWIDCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Clients clients = (Clients)usersDataGrid.SelectedItem;
            // Create a new object because the old one  
            // is being tracked by EF now.  
            Clients c = context.Clients.First(i => i.Username == clients.Username);
            c.HWID = string.Empty;

            clientsViewSource.View.Refresh();
            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.
            context.SaveChanges();
        }

        private void ResetIPCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Clients clients = (Clients)usersDataGrid.SelectedItem;
            // Create a new object because the old one  
            // is being tracked by EF now.  
            Clients c = context.Clients.First(i => i.Username == clients.Username);
            c.IPAddress = string.Empty;

            clientsViewSource.View.Refresh();
            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.
            context.SaveChanges();
        }

        internal static DateTime GetDateTime()
        {
            DateTime dateTime = DateTime.MinValue;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "text/html, application/xhtml+xml, */*";
            httpWebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
                dateTime = DateTime.ParseExact(response.Headers["date"], "ddd, dd MMM yyyy HH:mm:ss 'GMT'", (IFormatProvider)CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal);
            return dateTime;
        }

        private void OpenLicenseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            LicenseGenerator view = new LicenseGenerator { Owner = this };
            view.Show();
        }
    }
}
