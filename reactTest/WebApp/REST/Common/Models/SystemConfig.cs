using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class SystemConfig
    {
        public string BankId { get; set; }
        public string BankDisplayName { get; set; }
        public string BankIcon { get; set; }
        public string CorporateId { get; set; }
        public string CorporateName { get; set; }
        public string CorporateIcon { get; set; }
        List<string> UserConfigs { get; set; }
    }
}
