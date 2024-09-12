using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Win32;

namespace DocumentHQ.CommonConfig
{
	public static class RegistryHelper
	{
		public static string GetStringValue(string valueName, string defaultValue, bool forUser)
		{
			try
			{
				return (string)GetValue(valueName, defaultValue, forUser);
			}
			catch (InvalidCastException)
			{
				throw new Exception(
					string.Format(
						"The registry value '{0}' was expected to be a REG_SZ.",
						valueName));
			}
		}
        public static void SetStringValue(string valueName, string value, bool forUser)
        {
            try
            {
                SetValue(valueName, value, forUser);
            }
            catch (InvalidCastException)
            {
                throw new Exception(
                    string.Format(
                        "The registry value '{0}' was expected to be a REG_SZ.",
                        valueName));
            }
        }

		public static byte[] GetBinaryValue(string valueName, byte[] defaultValue, bool forUser)
		{
			try
			{
				return (byte[])GetValue(valueName, defaultValue, forUser);
			}
			catch (InvalidCastException)
			{
				throw new Exception(
					string.Format(
                        "The registry value '{0}' was expected to be a REG_BINARY.",
						valueName));
			}
		}

		public static void SetUserBinaryValue(string valueName, byte[] value)
		{
			SetUserValue(valueName, value);
		}

		public static int GetIntValue(string valueName, int defaultValue, bool forUser)
		{
			try
			{
				return (int)GetValue(valueName, defaultValue, forUser);
			}
			catch (InvalidCastException)
			{
				throw new Exception(
					string.Format(
                        "The registry value '{0}' was expected to be a REG_DWORD.",
						valueName));
			}
		}

		private static object GetValue(string valueName, object defaultValue, bool forUser)
		{
			// Get the ExSafe key. This throws an exception if the key is not present.
			RegistryKey exSafeKey = GetExSafeKey(forUser);

			try
			{
				// Read and return the value.
				return exSafeKey.GetValue(valueName, defaultValue);
			}
			finally
			{
				// Turn off the lights as we leave.
				exSafeKey.Close();
			}
		}
        private static void SetValue(string valueName, object value, bool forUser)
		{
			// Get the ExSafe key. This throws an exception if the key is not present.
			RegistryKey exSafeKey = GetExSafeKey(forUser);

			try
			{
				// Read and return the value.
                exSafeKey.SetValue(valueName, value);
			}
			finally
			{
				// Turn off the lights as we leave.
				exSafeKey.Close();
			}
		}

		private static RegistryKey GetExSafeKey(bool forUser)
		{
			RegistryKey exSafeKey;

			// First try to open the ExSafe registry key at the user level.
            if (!forUser || (exSafeKey = Registry.CurrentUser.OpenSubKey("Software\\" + CommonGlobals.ProductName, false)) == null)
			{
				// Didn't find it there so try the machine level.
                exSafeKey = Registry.LocalMachine.OpenSubKey("Software\\" + CommonGlobals.ProductName, true);
				if (exSafeKey == null)
				{
                    throw new Exception(CommonGlobals.ProductName + " has not been properly installed on this computer.");
				}
			}
			return exSafeKey;
		}

		private static void SetUserValue(string valueName, object value)
		{
			// Open the ExSafe registry key at the HKCU level.
			RegistryKey exSafeKey = Registry.CurrentUser.CreateSubKey("Software\\" + CommonGlobals.ProductName);
			if (exSafeKey == null)
			{
				throw new Exception("Could not save user settings to the registry.");
			}

			try
			{
				// Write the value.
				exSafeKey.SetValue(valueName, value);
			}
			finally
			{
				// Turn off the lights as we leave.
				exSafeKey.Close();
			}
		}
	}
}
