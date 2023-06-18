using FuNWallet.Client.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;

namespace FuNWallet.Client.ViewModels;

public class MainViewModel : ViewModelBase
{
    private Student student;
    public Student Student
    {
        get => student;
        set => this.RaiseAndSetIfChanged(ref student, value);
    }
    
    private Debt currentDebt;
    public Debt CurrentDebt
    {
        get => currentDebt;
        set => this.RaiseAndSetIfChanged(ref currentDebt, value);
    }

    public ObservableCollection<Debt> Debts { get; }
    public ObservableCollection<Transaction> Transactions { get; }
    public ReactiveCommand<Unit, Debt> PayDebt { get; }

    public MainViewModel(Student student, List<Debt> debts, List<Transaction> transactions)
    {
        this.student = student;
        PayDebt = ReactiveCommand.Create(() => CurrentDebt);
        
        foreach (var debt in debts)
        {
            debt.PayCommand = ReactiveCommand.Create(() => debt);
            debt.PayCommand.Subscribe(x => {
                CurrentDebt = x;
                PayDebt.Execute().Subscribe();
            });
        }
        
        Debts = new ObservableCollection<Debt>(debts);
        Transactions = new ObservableCollection<Transaction>(transactions);
        /*
        Task.Run(async () =>
        {
            await Task.Delay(5000);
            Debts.Add(new Debt() { Title = "Semester 3 Tuition Fees", Amount = 100.6, DueDate = DateTime.Now });
            Debts[1].PayCommand = ReactiveCommand.Create(() => Debts[1]);
            Debts[1].PayCommand.Subscribe(x => {
                CurrentDebt = x;
                PayDebt.Execute().Subscribe();
            });
            Student.Balance += 100;
        });*/
    }
}
