using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Win32;

namespace DocumentHQ.CommonConfig
{
	public static class CommonGlobals
	{
        public static string ProductName = "DocumentHQ";
        public static string RegistryKey = "HKEY_LOCAL_MACHINE\\Software\\DocumentHQ";

        public static string GlobalIdentifier = "666666666";

        public static string GroupsCollectionName = "groups";
        public static string RolesCollectionName = "roles";
        public static string AccountTypesCollectionName = "accounttypes";
        public static string CorporatesCollectionName = "corporates";
        public static string BanksCollectionName = "banks";
        public static string AccountsCollectionName = "accounts";
        public static string DocumentsCollectionName = "documents";
        public static string DocumentContentsCollectionName = "DocumentContents";
        public static string UserAuthCollectionName = "UserAuth";
        public static string UserConfigCollectionName = "UserConfig";
        public static string RolesChildName = "Roles";

        public static string IdFieldName = "Id";
        public static string UserIdFieldName = "UserId";
        public static string ParentID = "ParentID";

        public static string serverUrl = "http://localhost";
        //public static string serverUrl = "http://ec2-54-202-32-215.us-west-2.compute.amazonaws.com";

    }
}
