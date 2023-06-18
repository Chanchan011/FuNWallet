using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace FuNWallet.Client.ViewModels
{
    public class TransactionSuccessViewModel: ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> Back { get; }

        public TransactionSuccessViewModel()
        {
            Back = ReactiveCommand.Create(() => { });
        }
    }
}
