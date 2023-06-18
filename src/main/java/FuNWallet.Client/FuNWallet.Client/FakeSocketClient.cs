using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipelines;
using FuNWallet.Client.Models;

namespace FuNWallet.Client
{
    public class FakeSocketClient
    {
        const int PORT = 8087;
        private string username;
        private string password;

        public FakeSocketClient(string username, string password)
        {
            this.username = username;
            this.password = password;
        }


        private async Task<string> SendMessageAsync(string message)
        {
            using var clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            await clientSocket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, PORT));
            var stream = new NetworkStream(clientSocket);
            var socketConnected = clientSocket.Connected;

            if (!socketConnected || clientSocket == null)
            {
                throw new InvalidOperationException("Failed to connect socket");
            }
            var reader = PipeReader.Create(stream);
            var writer = PipeWriter.Create(stream);

            await writer.WriteAsync(Encoding.UTF8.GetBytes(message));
            ReadResult readResult = await reader.ReadAsync().ConfigureAwait(false);
            return Encoding.UTF8.GetString(readResult.Buffer);
        }

        public Task<double> LoginAsync()
        {
            return Task.FromResult(100.0);
        }

        public Task<Student> GetStudentInfo()
        {
            return Task.FromResult(new Student()
            {
                ID = "21012122",
                FullName = "Pham Dam Quan",
                Nationality = "Vietnamese"
            });
        }

        public Task<IEnumerable<Transaction>> GetTransactions()
        {
            return Task.FromResult(new List<Transaction>() { new Transaction() {
                StudentID = "21012122",
                Amount = 100, 
                Approved = true,
                Message = "Transaction #1",
                TransactionID = "1"
            } }.Select(x => x));
        }

        public Task<IEnumerable<Transaction>> GetPendingTransactions()
        {
            return Task.FromResult(new List<Transaction>() { new Transaction() {
                StudentID = "21012122",
                Amount = 100,
                Approved = false,
                Message = "Semester #2 Tuition Fee",
                TransactionID = "1"
            } }.Select(x => x));
        }

        public async Task<double> CreateTransactionAsync(double amount)
        {
            return 500;
        }

        public async Task<double> ResolveTransactionAsync(Transaction t)
        {
            await Task.Delay(2000);
            string request = "PAY" + username + t.TransactionID;
            var response = "1";
            return double.Parse(response);
        }
    }
}
