using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace NBsoft.Appointment.WPF.Common
{
    static class HardwareKey
    {
        private const string EncryptionPass = "#appointment@nbasoft-2016";

        public static string GetUIK()
        {
            string result = "";
            int num = 0;
            while (true)
            {
                num++;
                if (num <= 3)
                {
                    try
                    {
                        string[] array;
                        result = GetUniqueID(out array);
                    }
                    catch
                    {
                        Thread.Sleep(2000);
                        result = "";
                        continue;
                    }
                    break;
                }
                break;
            }
            return result;
        }

        private static string GetUniqueID(out string[] Params)
        {
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Username = null;
            connectionOptions.Password = null;
            ManagementPath path = new ManagementPath(string.Format("\\\\{0}\\root\\cimv2", Environment.MachineName));
            Params = new string[6];
            string text = "";
            try
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new ManagementScope(path, connectionOptions), new ObjectQuery("Select * from Win32_OperatingSystem"));
                ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        ManagementObject managementObject = (ManagementObject)enumerator.Current;
                        PropertyData propertyData = managementObject.Properties["SerialNumber"];
                        Params[0] = ((propertyData == null || propertyData.Value == null) ? "OSSerialNumberFailed" : propertyData.Value.ToString());
                        text = Params[0];
                    }
                }
            }
            catch
            {
                text = "ERR001";
            }
            try
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new ManagementScope(path, connectionOptions), new ObjectQuery("Select * from Win32_BaseBoard"));
                ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator2 = managementObjectCollection.GetEnumerator())
                {
                    if (enumerator2.MoveNext())
                    {
                        ManagementObject managementObject2 = (ManagementObject)enumerator2.Current;
                        PropertyData propertyData2 = managementObject2.Properties["Product"];
                        PropertyData propertyData3 = managementObject2.Properties["Version"];
                        PropertyData propertyData4 = managementObject2.Properties["SerialNumber"];
                        Params[1] = ((propertyData2 == null || propertyData2.Value == null) ? "BoardProductNumberFailed" : propertyData2.Value.ToString().Trim());
                        Params[2] = ((propertyData3 == null || propertyData3.Value == null) ? "BoardVersionFailed" : propertyData3.Value.ToString().Trim());
                        Params[3] = ((propertyData4 == null || propertyData4.Value == null) ? "BoardSerialNumberFailed" : propertyData4.Value.ToString().Trim());
                        text += Params[1];
                        text += Params[2];
                        text += Params[3];
                    }
                }
            }
            catch
            {
                text += "ERR002";
            }
            try
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new ManagementScope(path, connectionOptions), new ObjectQuery("Select * from Win32_BIOS"));
                ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator3 = managementObjectCollection.GetEnumerator())
                {
                    if (enumerator3.MoveNext())
                    {
                        ManagementObject managementObject3 = (ManagementObject)enumerator3.Current;
                        PropertyData propertyData5 = managementObject3.Properties["Manufacturer"];
                        Params[4] = ((propertyData5 == null || propertyData5.Value == null) ? "BiosManufacturer Failed" : propertyData5.Value.ToString().Trim());
                        text += Params[4];
                    }
                }
            }
            catch
            {
                text += "ERR003";
            }
            try
            {
                string text2 = "";
                try
                {
                    text2 = GetSystemDrive();
                }
                catch
                {
                    text2 = "\\\\.\\PHYSICALDRIVE0";
                }
                string query = string.Format("Select * from Win32_DiskDrive WHERE DeviceId = '{0}'", text2.Replace("\\", "\\\\"));
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new ManagementScope(path, connectionOptions), new ObjectQuery(query));
                ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator4 = managementObjectCollection.GetEnumerator())
                {
                    if (enumerator4.MoveNext())
                    {
                        ManagementObject managementObject4 = (ManagementObject)enumerator4.Current;
                        PropertyData propertyData6 = managementObject4.Properties["SerialNumber"];
                        Params[5] = ((propertyData6 == null || propertyData6.Value == null) ? "HddSerialNumberFailed" : propertyData6.Value.ToString().Trim());
                        text += Params[5];
                    }
                }
            }
            catch
            {
                text += "ERR004";
            }
            if (text == "")
            {
                text = Environment.Version.ToString();
                Params = new string[]
                {
                    text
                };
            }
            connectionOptions = null;
            path = null;
            string text3 = Encrypt(text, EncryptionPass);
            int num = 0;
            long num2 = -1L;
            string text4 = text3;
            for (int i = 0; i < text4.Length; i++)
            {
                char c = text4[i];
                num += (int)((byte)c);
                num2 += (long)(num * (int)((byte)c));
            }
            string s = num2.ToString() + num.ToString();
            string text5 = long.Parse(s).ToString("X");
            int num3 = 0;
            while (text5.Length < 8)
            {
                try
                {
                    text5 += long.Parse(text5.Substring(num3++, 2)).ToString("X");
                }
                catch
                {
                    text5 += num2.ToString();
                }
            }
            return text5.Substring(text5.Length - 8, 8);
        }

        private static string GetSystemDrive()
        {
            string str = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 2);
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk WHERE DeviceID='" + str + "'"))
            {
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ManagementObject managementObject = (ManagementObject)enumerator.Current;
                        using (ManagementObjectCollection.ManagementObjectEnumerator enumerator2 = managementObject.GetRelated("Win32_DiskPartition").GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                ManagementObject managementObject2 = (ManagementObject)enumerator2.Current;
                                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator3 = managementObject2.GetRelated("Win32_DiskDrive").GetEnumerator())
                                {
                                    if (enumerator3.MoveNext())
                                    {
                                        ManagementObject managementObject3 = (ManagementObject)enumerator3.Current;
                                        return managementObject3["DeviceID"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream memoryStream = new MemoryStream();
            Rijndael rijndael = Rijndael.Create();
            rijndael.Key = Key;
            rijndael.IV = IV;
            CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(clearData, 0, clearData.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        private static string Encrypt(string clearText, string Password)
        {
            new StackTrace();
            byte[] bytes = Encoding.Unicode.GetBytes(clearText);
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(Password, new byte[]
            {
                73,
                118,
                97,
                110,
                32,
                77,
                101,
                100,
                118,
                101,
                100,
                101,
                118
            });
            byte[] inArray = Encrypt(bytes, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));
            return Convert.ToBase64String(inArray);
        }
    }
}
