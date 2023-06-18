using FuNWallet.Client.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace FuNWallet.Client.ViewModels
{
    public class WithdrawViewModel: ViewModelBase
    {
        public ReactiveCommand<Unit, Transaction> Send { get; }
        public ReactiveCommand<Unit, Unit> Back { get; }

        private bool notdone = true;
        public bool NotDone
        {
            get => notdone;
            set => this.RaiseAndSetIfChanged(ref notdone, value);
        }

        private double amount;
        public double Amount
        {
            get => amount;
            set => this.RaiseAndSetIfChanged(ref amount, value);
        }

        private string message = "";
        public string Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }

        public WithdrawViewModel()
        {
            var okEnabled = this.WhenAnyValue(
                x => x.Amount,
                x => x > 0);

            Send = ReactiveCommand.Create(
                () => new Transaction { RecipientID = "FUN University", Amount = Amount, Message = $"Withdrawed {Amount}" },
                okEnabled);
            Back = ReactiveCommand.Create(() => { });
        }
    }
}
