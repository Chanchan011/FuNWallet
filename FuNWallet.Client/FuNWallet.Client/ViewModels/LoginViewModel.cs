using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuNWallet.Client.ViewModels
{
    public class LoginViewModel: ViewModelBase
    {
        private string studentID = "";
        private string password = "";

        public string StudentID
        {
            get => studentID;
            set => this.RaiseAndSetIfChanged(ref studentID, value);
        }

        public string Password
        {
            get => password;
            set => this.RaiseAndSetIfChanged(ref password, value);
        }

        public void Login()
        {

        }
    }
}
