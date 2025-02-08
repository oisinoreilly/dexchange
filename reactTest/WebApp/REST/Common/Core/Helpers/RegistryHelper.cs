using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{

   
    public class RegistryHelper
    {
        // Example usage
        static string _keyPath = @"Software\dexchange";

        // Read a DWORD value
        public static int? ReadDWordValue(string valueName, int defaultvalue)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(_keyPath))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(valueName);
                        if (value != null)
                        {
                            return Convert.ToInt32(value);
                        }
                        return defaultvalue;
                    }
                }
                return defaultvalue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading DWORD: {ex.Message}");
                return defaultvalue;
            }
        }

        // Write a DWORD value
        public static bool WriteDWordValue(string valueName, int value)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(_keyPath))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, value, RegistryValueKind.DWord);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing DWORD: {ex.Message}");
                return false;
            }
        }

        // Read a string value
        public static string ReadStringValue(string valueName, string defaultvalue)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(_keyPath))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(valueName);
                        if (value == null)
                              return defaultvalue;
                        else return Convert.ToString(value);
                    }
                    return defaultvalue;
                }
                return defaultvalue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading string: {ex.Message}");
                return defaultvalue;
            }
        }

        // Write a string value
        public static bool WriteStringValue(string valueName, string value)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(_keyPath))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, value, RegistryValueKind.String);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing string: {ex.Message}");
                return false;
            }
        }
    }

}

