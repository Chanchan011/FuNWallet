using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipelines;
using FuNWallet.Client.Models;
using System.IO;
using Newtonsoft.Json.Linq;

namespace FuNWallet.Client
{
    public class SocketClient
    {
        private string username;
        private string password;
        private Socket clientSocket;
        private PipeReader reader;
        private PipeWriter writer;
        private string ipAddress;
        private int port;

        public SocketClient(string username, string password)
        {
            this.username = username;
            this.password = password;
            clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            var config = JObject.Parse(File.ReadAllText("appsettings.json"));
            ipAddress = config["ServerIP"].Value<string>();
            port = config["ServerPort"].Value<int>();
        }

        
        private async Task<string> SendMessageAsync(string message)
        {
            await writer.WriteAsync(Encoding.UTF8.GetBytes(message + "\n"));
            ReadResult readResult = await reader.ReadAsync().ConfigureAwait(false);
            string serverResponse = Encoding.UTF8.GetString(readResult.Buffer);
            reader.AdvanceTo(readResult.Buffer.End);
            return serverResponse.TrimEnd('\r', '\n');
        }

        public async Task<double> LoginAsync()
        {
            if (!clientSocket.Connected)
            {
                await clientSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(ipAddress), port));
                var socketConnected = clientSocket.Connected;

                if (!socketConnected || clientSocket == null)
                {
                    throw new InvalidOperationException("Failed to connect socket");
                }
                var stream = new NetworkStream(clientSocket);

                this.reader = PipeReader.Create(stream);
                this.writer = PipeWriter.Create(stream);
            }
            var response = await SendMessageAsync($"{username}_{password}");
            return double.Parse(response);
        }

        public async Task LogoutAsync()
        {
            await clientSocket.DisconnectAsync(true);
        }

        public async Task<Student> GetStudentInfo()
        {
            var response = await SendMessageAsync($"INFO_{username}");
            return Student.FromServerResponse(response);
        }

        public async Task<IEnumerable<Transaction>> GetTransactions()
        {
            var response = await SendMessageAsync($"SHOWTRANS_{username}");
            return response.Split('|').Where(x => !string.IsNullOrEmpty(x)).Select(x => Transaction.FromServerResponse(x)).Where(x => x.Approved);
        }

        public async Task<IEnumerable<Transaction>> GetPendingTransactions()
        {
            var response = await SendMessageAsync($"SHOWDEBT_{username}");
            return response.Split('|').Where(x => !string.IsNullOrEmpty(x)).Select(x => Transaction.FromServerResponse(x));
        }

        public async Task<string> CreateTransactionAsync(double amount)
        {
            string request = "";
            if (amount >= 0) { request += "ADD_"; }
            else { request += "SUB_"; }

            request += username;
            request += "_" + Math.Abs(amount).ToString();
            

            var response = await SendMessageAsync(request);
            return response;
        }

        public async Task<string> ResolveTransactionAsync(Transaction t)
        {
            string request = "PAY_" + username + "_" + t.TransactionID;
            var response = await SendMessageAsync(request);
            return response;
        }
    }
}
