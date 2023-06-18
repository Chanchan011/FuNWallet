using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace FuNWallet.Client.Models
{
    public class Debt
    {
        public string Title { get; set; }
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        public ReactiveCommand<Unit, Debt> PayCommand { get; set; }
    }
}
