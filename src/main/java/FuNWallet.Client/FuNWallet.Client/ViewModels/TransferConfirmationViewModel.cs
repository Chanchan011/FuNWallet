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
    public class TransferConfirmationViewModel: ViewModelBase
    {
        const string GREEN = "#4BE843";
        const string RED = "#F96F57";
        const string YELLOW = "#F2A713";

        private Transaction t;
        private bool ok = false;
        private string message = "Status: Waiting for verification...";
        private string statusColor = YELLOW;
        private VerificationStatus status;

        public ReactiveCommand<Unit, VerificationStatus> Confirm { get; }
        public ReactiveCommand<Unit, Unit> Back { get; }


        public Transaction Tran
        {
            get => t;
            set => this.RaiseAndSetIfChanged(ref t, value);
        }

        public VerificationStatus Status
        {
            get => status;
            set => this.RaiseAndSetIfChanged(ref status, value);
        }

        public bool Ok
        {
            get => ok;
            set => this.RaiseAndSetIfChanged(ref ok, value);
        }

        public string Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }

        public string StatusColor
        {
            get => statusColor;
            set => this.RaiseAndSetIfChanged(ref statusColor, value);
        }

        public TransferConfirmationViewModel(Transaction transaction)
        {
            t = transaction;
            status = new VerificationStatus();

            Task.Run(async () =>
            {
                await Task.Delay(2000);
                Ok = true;
                Message = "Status: Transaction approved";
                StatusColor = GREEN;
            });


            /*var okEnabled = this.WhenAnyValue(
                x => x.Status);*/

            Confirm = ReactiveCommand.Create(
                () => new VerificationStatus { Ok = Status.Ok, Message = Status.Message });
            Back = ReactiveCommand.Create(() => { });
        }
    }
}
