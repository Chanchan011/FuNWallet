using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FuNWallet.Client.Models
{
    public class Student
    {
        public string FullName { get; set; }
        public string ID { get; set; }
        public string Nationality { get; set; }
        public string School { get; set; } = "FUN University";
        public double Balance { get; set; }
        public double Debt { get; set; }

        public static Student FromServerResponse(string response)
        {
            string[] data = response.Split("_");
            var studentID = data[0];
            var name = data[1];
            var nationality = data[2];

            return new Student
            {
                ID = studentID,
                FullName = name,
                Nationality = nationality
            };
        }
    }
}
