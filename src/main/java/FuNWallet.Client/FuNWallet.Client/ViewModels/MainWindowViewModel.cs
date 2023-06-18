using FuNWallet.Client.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace FuNWallet.Client.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        ViewModelBase currentView;
        public MainWindowViewModel()
        {
            currentView = new LoginViewModel();
        }
        
        public ViewModelBase CurrentView
        {
            get => currentView;
            private set => this.RaiseAndSetIfChanged(ref currentView, value);
        }

        public MainViewModel MainView { get; private set; }

        public void NavigateToTransferView(Debt debt)
        {
            var tvm = new TransferViewModel();
            tvm.Amount = debt.Amount;
            tvm.RecipientID = "FUN University";

            Observable.Merge(
                tvm.Send,
                tvm.Back.Select(_ => (Transaction)null))
                .Take(1)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        NavigateToTransferConfirmationView(model, tvm);
                    }

                    else 
                        CurrentView = MainView;
                });
            CurrentView = tvm;
        }

        public void NavigateToTransferConfirmationView(Transaction t, TransferViewModel tvm)
        {
            var tcvm = new TransferConfirmationViewModel(t);
            Observable.Merge(
                tcvm.Confirm,
                tcvm.Back.Select(_ => (VerificationStatus)null))
                .Take(1)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        NavigateToTransferSuccessView();
                    }

                    else
                    {
                        Observable.Merge(
                            tvm.Send,
                            tvm.Back.Select(_ => (Transaction)null))
                            .Take(1)
                            .Subscribe(model =>
                            {
                                if (model != null)
                                {
                                    NavigateToTransferConfirmationView(model, tvm);
                                }

                                else
                                    CurrentView = MainView;
                         });
                        CurrentView = tvm;
                    };
                });
            CurrentView = tcvm;
        }
        public void NavigateToTransferSuccessView()
        {
            var tcvm = new TransactionSuccessViewModel();
            tcvm.Back.Subscribe(x => CurrentView = MainView);
            CurrentView = tcvm;
        }

        public void Login()
        {
            var studentID = (CurrentView as LoginViewModel).StudentID;
            var password = (CurrentView as LoginViewModel).Password;

            MainView = new MainViewModel(new Student()
            {
                FullName = "Pham Dam Quan Jr",
                School = "UEB",
                ID = 21012122,
                Balance = 1000,
                Debt = 500.11,
                Nationality = "Vietnamese"
            });
            MainView.PayDebt.Subscribe(x =>
            {
                if (x != null)
                {
                    NavigateToTransferView(x);
                }
            });
            CurrentView = MainView;
        }

        public void Logout()
        {
            CurrentView = new LoginViewModel();
            MainView = null;
        }
    }
}
