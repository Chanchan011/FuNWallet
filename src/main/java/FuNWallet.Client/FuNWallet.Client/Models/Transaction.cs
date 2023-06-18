using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuNWallet.Client.Models
{
    public class Transaction
    {
        public string RecipientID { get; set; }
        public double Amount { get; set; }
        public string Message { get; set; }
    }
}
