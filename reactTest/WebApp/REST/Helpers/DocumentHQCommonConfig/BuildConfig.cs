using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace DExchange.CommonConfig
{
	public static class BuildConfig
	{
		public static Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}
	}
}
