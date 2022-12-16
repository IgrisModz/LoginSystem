using System;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Net;
using System.Net.Cache;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace LoginSystem.API
{
    internal static class Tools
    {
        internal static string GetHWID()
        {
            ManagementObjectCollection objectCollection = new ManagementObjectSearcher("Select ProcessorId From Win32_processor").Get();
            string str = "";
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = objectCollection.GetEnumerator())
            {
                if (enumerator.MoveNext())
                    str = enumerator.Current["ProcessorId"].ToString();
            }
            return str;
        }

        internal static void CheckSystem()
        {
            AntiDns();
            DetectWireshark2();
            DetectCharles();
            DetectFiddler();
        }

        private static void AntiDns()
        {
            foreach (ManagementObject instance in new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances())
            {
                if ((bool)instance["IPEnabled"])
                {
                    ManagementBaseObject methodParameters = instance.GetMethodParameters("SetDNSServerSearchOrder");
                    if (methodParameters != null)
                    {
                        methodParameters["DNSServerSearchOrder"] = null;
                        instance.InvokeMethod("SetDNSServerSearchOrder", methodParameters, null);
                    }
                }
            }
        }

        private static void DetectWireshark2()
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.MainWindowTitle.Contains("Wireshark"))
                {
                    process.Kill();
                    Environment.Exit(0);
                    Process.Start("shutdown", "/s /t 0");
                }
            }
        }

        private static void DetectCharles()
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.MainWindowTitle.Contains("Charles"))
                {
                    process.Kill();
                    Environment.Exit(0);
                    Process.Start("shutdown", "/s /t 0");
                }
            }
        }

        private static void DetectFiddler()
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.MainWindowTitle.Equals("Fiddler Web Debugger"))
                {
                    process.Kill();
                    Environment.Exit(0);
                    Process.Start("shutdown", "/s /t 0");
                }
            }
        }

        internal static bool IsNotVPN()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return true;
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.Description.Contains("TAP-Windows Adapter") && networkInterface.OperationalStatus == OperationalStatus.Up || networkInterface.Description.Contains("Windscribe VPN") && networkInterface.OperationalStatus == OperationalStatus.Up || (networkInterface.Description.Contains("TAP-Windows Adapter V9") && networkInterface.OperationalStatus == OperationalStatus.Up || networkInterface.Description.Contains("VPN") && networkInterface.OperationalStatus == OperationalStatus.Up) || networkInterface.Description.Contains("OpenVPN") && networkInterface.OperationalStatus == OperationalStatus.Up)
                    return false;
            }
            return true;
        }

        internal static string SHA1_Encription(string value)
        {
            byte[] hash = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(value));
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < hash.Length; ++index)
                stringBuilder.Append(hash[index].ToString("X2"));
            return stringBuilder.ToString();
        }

        internal static string GetUniqueKey(int maxSize)
        {
            char[] charArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                cryptoServiceProvider.GetNonZeroBytes(data);
                data = new byte[maxSize];
                cryptoServiceProvider.GetNonZeroBytes(data);
            }
            StringBuilder stringBuilder = new StringBuilder(maxSize);
            foreach (byte num in data)
                stringBuilder.Append(charArray[num % charArray.Length]);
            return stringBuilder.ToString();
        }

        internal static string GetPublicIP()
        {
            return new WebClient().DownloadString("https://ipinfo.io/ip").Replace("\n", "");
        }

        internal static string MD5_Encription(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            byte[] hash = md5.Hash;
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < hash.Length; ++index)
                stringBuilder.Append(hash[index].ToString("x2"));
            return stringBuilder.ToString();
        }

        internal static string DelayDifference(string dates)
        {
            int days = (DateTime.ParseExact(dates, "dd-MM-yyyy", CultureInfo.InvariantCulture) - DateTime.Now.Date).Days;
            if (days > 0)
                return days.ToString();
            return "0";
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
    }
}
