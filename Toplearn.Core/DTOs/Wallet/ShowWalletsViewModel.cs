using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs.Wallet
{
    public class ShowWalletsViewModel
    {
        public int Amount { get; set; }
        public int TypeId { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsPay { get; set; }
        public string RefId { get; set; } = "-";
        public string Authority { get; set; } = "-";

    }
}