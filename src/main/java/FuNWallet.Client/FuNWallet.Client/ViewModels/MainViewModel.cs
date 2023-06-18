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
    public ReactiveCommand<Unit, Debt> PayDebt { get; }

    public MainViewModel(Student student)
    {
        this.student = student;
        var debts = new List<Debt>() { new Debt() { Title = "Semester 2 Tuition Fees", Amount = 100.2, DueDate = DateTime.Now } };
        PayDebt = ReactiveCommand.Create(() => CurrentDebt);
        
        debts[0].PayCommand = ReactiveCommand.Create(() => debts[0]);
        debts[0].PayCommand.Subscribe(x => { 
            CurrentDebt = x; 
            PayDebt.Execute().Subscribe(); 
        });

       
        Debts = new ObservableCollection<Debt>(debts);
        
        Task.Run(async () =>
        {
            await Task.Delay(5000);
            Debts.Add(new Debt() { Title = "Semester 3 Tuition Fees", Amount = 100.2, DueDate = DateTime.Now });
            Debts[1].PayCommand = ReactiveCommand.Create(() => Debts[1]);
            Debts[1].PayCommand.Subscribe(x => {
                CurrentDebt = x;
                PayDebt.Execute().Subscribe();
            });
            Student.Balance += 100;
        });
    }
}
