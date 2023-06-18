using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuNWallet.Client.Models
{
    public class Student
    {
        public string FullName { get; set; }
        public int ID { get; set; }
        public string Nationality { get; set; }
        public string School { get; set; }
        public double Balance { get; set; }
        public double Debt { get; set; }
    }
}
