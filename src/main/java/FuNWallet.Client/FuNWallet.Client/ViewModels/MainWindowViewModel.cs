using Avalonia.Platform;
using FuNWallet.Client.Models;
using FuNWallet.Client.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        SocketClient client;

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

        public void NavigateToTopUpView()
        {
            var tvm = new TopupViewModel();
            tvm.Send.Subscribe(async model =>
            {
                if (model != null)
                {
                    string res = await client.CreateTransactionAsync(model.Amount);
                    if (double.TryParse(res, out _))
                    {
                        tvm.Message = $"Successfully added {model.Amount} to your account!";
                        tvm.NotDone = false;
                        MainView.Student.Balance += model.Amount;
                    }
                    else
                    {
                        tvm.Message = res;
                    }
                }
            });
            tvm.Back.Subscribe(x => CurrentView = MainView);
            CurrentView = tvm;
        }

        public void NavigateToWithdrawView()
        {
            var tvm = new WithdrawViewModel();
            tvm.Send.Subscribe(async model =>
            {
                if (model != null)
                {
                    string res = await client.CreateTransactionAsync(-model.Amount);
                    if (double.TryParse(res, out _))
                    {
                        tvm.Message = $"Successfully withdrawed {model.Amount} from your account!";
                        tvm.NotDone = false;
                        MainView.Student.Balance -= model.Amount;
                    }
                    else
                    {
                        tvm.Message = res;
                    }
                }
            });
            tvm.Back.Subscribe(x => CurrentView = MainView);
            CurrentView = tvm;
        }

        public void NavigateToTransferView(Debt debt)
        {
            var tvm = new TransferViewModel();
            tvm.Amount = debt.Amount;
            tvm.Message = debt.Title;
            tvm.TransactionID = debt.TransactionID;
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
                        _ = VerifyTransaction(model);
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
                        NavigateToTransferSuccessView(MainView.Student.Balance - model.Amount);
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
                                    _ = VerifyTransaction(model);
                                }

                                else
                                    CurrentView = MainView;
                         });
                        CurrentView = tvm;
                    };
                });
            CurrentView = tcvm;
        }
        public void NavigateToTransferSuccessView(double balance)
        {
            var tcvm = new TransactionSuccessViewModel();
            tcvm.Back.Subscribe(async x => await ReturnToMainView(balance));
            CurrentView = tcvm;
        }

        private async Task VerifyTransaction(Transaction transaction)
        {
            var res = await client.ResolveTransactionAsync(transaction);
            if (double.TryParse(res, out var balance))
            {
                var tcvm = CurrentView as TransferConfirmationViewModel;
                tcvm.Ok = true;
                tcvm.Message = "Status: Transaction approved";
                tcvm.StatusColor = TransferConfirmationViewModel.GREEN;
            }
            else
            {
                var tcvm = CurrentView as TransferConfirmationViewModel;
                tcvm.Ok = false;
                tcvm.Message = res;
                tcvm.StatusColor = TransferConfirmationViewModel.RED;
            }
        }

        public async Task Login()
        {
            try
            {
                var studentID = (CurrentView as LoginViewModel).StudentID;
                var password = (CurrentView as LoginViewModel).Password;
                client = new SocketClient(studentID, password);
                double balance = await client.LoginAsync();
                if (balance < 0)
                    // Handle later
                    return;
                var student = await client.GetStudentInfo();
                student.Balance = balance;
                student.School = "FUN University";
                var debts = (await client.GetPendingTransactions()).Select(x => new Debt() { Title = x.Message, Amount = Math.Abs(x.Amount), DueDate = DateTime.Today, TransactionID = x.TransactionID });
                var recentTransactions = await client.GetTransactions();
                student.Debt = debts.Sum(x => x.Amount);

                MainView = new MainViewModel(student, debts.ToList(), recentTransactions.ToList());
                MainView.PayDebt.Subscribe(x =>
                {
                    if (x != null)
                    {
                        NavigateToTransferView(x);
                    }
                });
                CurrentView = MainView;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task Logout()
        {
            await client.LogoutAsync();
            CurrentView = new LoginViewModel();
            MainView = null;
        }

        public async Task ReturnToMainView(double balance)
        {
            var student = await client.GetStudentInfo();
            student.Balance = balance;
            student.School = "FUN University";
            var debts = (await client.GetPendingTransactions()).Select(x => new Debt() { Title = x.Message, Amount = Math.Abs(x.Amount), DueDate = DateTime.Today, TransactionID = x.TransactionID });
            var recentTransactions = await client.GetTransactions();
            student.Debt = debts.Sum(x => x.Amount);

            MainView = new MainViewModel(student, debts.ToList(), recentTransactions.ToList());
            MainView.PayDebt.Subscribe(x =>
            {
                if (x != null)
                {
                    NavigateToTransferView(x);
                }
            });
            CurrentView = MainView;
        }
    }
}
