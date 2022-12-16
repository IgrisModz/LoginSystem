using LoginSystem.MessageBox;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace LoginSystem.AdminPanel
{
    /// <summary>
    /// Logique d'interaction pour LicenseGenerator.xaml
    /// </summary>
    public partial class LicenseGenerator : MetroWindow
    {
        readonly LicenseSystemEntities context = new LicenseSystemEntities();
        readonly CollectionViewSource licenseClientsViewSource;
        public LicenseGenerator()
        {
            InitializeComponent();
            licenseClientsViewSource = ((CollectionViewSource)(this.FindResource("licenseClientsViewSource")));
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
            context.LicenseClients.Load();

            // After the data is loaded, call the DbSet<T>.Local property    
            // to use the DbSet<T> as a binding source.   
            licenseClientsViewSource.Source = context.LicenseClients.Local;
        }

        private void GenerateLicenseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            string resultLicense;
            resultLicense = RandomString(15).ToUpper();
            resultLicense = $"{resultLicense.Substring(0, 5)}-{resultLicense.Substring(5, 5)}-{resultLicense.Substring(10, 5)}";
            LicenseClients license = new LicenseClients
            {
                License = resultLicense,
                Project = "GF34AZ3424DS34D3DFQ3"
            };
            DateTime date = MainWindow.GetDateTime();
            switch (ExpirationCmB.SelectedIndex)
            {
                case 0:
                    license.ExpirationDate = date;
                    break;
                case 1:
                    license.ExpirationDate = date.AddDays(1);
                    break;
                case 2:
                    license.ExpirationDate = date.AddDays(7);
                    break;
                case 3:
                    license.ExpirationDate = date.AddMonths(1);
                    break;
                case 4:
                    license.ExpirationDate = date.AddMonths(3);
                    break;
                case 5:
                    license.ExpirationDate = date.AddMonths(6);
                    break;
                case 6:
                    license.ExpirationDate = date.AddYears(1);
                    break;
                case 7:
                    license.ExpirationDate = DateTime.MaxValue;
                    break;
            }
            context.LicenseClients.Add(license);
            context.SaveChanges();
            Clipboard.SetText(resultLicense);
            MetroMessageBox.Show($"Your license is {resultLicense} and has been copied to clipboard", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
