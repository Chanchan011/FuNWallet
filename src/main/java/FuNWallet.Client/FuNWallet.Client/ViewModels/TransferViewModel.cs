using Avalonia;
using FuNWallet.Client.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuNWallet.Client.ViewModels
{
    public class TransferViewModel: ViewModelBase
    {
        public ReactiveCommand<Unit, Transaction> Send { get; }
        public ReactiveCommand<Unit, Unit> Back { get; }

        private string recipientID;
        private double amount;
        private string message;
        private string transactionID;

        public TransferViewModel()
        {
            var okEnabled = this.WhenAnyValue(
                x => x.RecipientID,
                x => !string.IsNullOrWhiteSpace(x));
            
            Send = ReactiveCommand.Create(
                () => new Transaction { RecipientID = RecipientID, Amount = Amount, Message = Message, TransactionID = this.TransactionID },
                okEnabled);
            Back = ReactiveCommand.Create(() => { });
        }

        public string RecipientID
        {
            get => recipientID;
            set => this.RaiseAndSetIfChanged(ref recipientID, value);
        }

        public double Amount
        {
            get => amount;
            set => this.RaiseAndSetIfChanged(ref amount, value);
        }

        public string Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }

        public string TransactionID
        {
            get => transactionID;
            set => this.RaiseAndSetIfChanged(ref transactionID, value);
        }
    }
}
