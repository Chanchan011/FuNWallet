using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FuNWallet.Client.Models
{
    public class Transaction
    {
        public string RecipientID { get; set; } = "FUN University";
        public double Amount { get; set; }
        public string Message { get; set; }
        public string TransactionID { get; set; }
        public string StudentID { get; set; }
        public bool Approved { get; set; }

        public string Status => Approved ? "Approved" : "Unapproved";

        public static Transaction FromServerResponse(string response)
        {
            string[] data = response.Split("_");
            var transactionID = data[0];
            var amount = int.Parse(data[1]);
            var message = data[2];
            bool status = false;
            if (data.Length > 3)
                status = (data[3] == "1");

            return new Transaction
            {
                Amount = amount,
                StudentID = "",
                TransactionID = transactionID,
                Approved = status,
                Message = message
            };
        }
    }
}
