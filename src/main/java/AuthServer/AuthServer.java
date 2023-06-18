package AuthServer;

import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;
import java.sql.SQLException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class AuthServer {
    private static final int PORT = 1234;

    private static final ExecutorService executor = Executors.newFixedThreadPool(10);
    public static void main(String[] args) {
        try {
            ServerSocket serverSocket = new ServerSocket(PORT);
            while (true) {
                Socket clientSocket = serverSocket.accept();
                executor.execute(new AuthServer.ClientHandler(clientSocket));
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private static class ClientHandler implements Runnable {
        private final Socket studentSocket;
        private final Database database = Database.getDatabase();
        private boolean login = false, quit = false;
        double balance = 0;
        private PrintWriter studentWriter;

        public ClientHandler(Socket studentSocket) {
            this.studentSocket = studentSocket;
        }

        public void run() {
            try {
                BufferedReader studentReader = new BufferedReader(new InputStreamReader(studentSocket.getInputStream()));
                studentWriter = new PrintWriter(studentSocket.getOutputStream(), true);

                String message;
                while ((message = studentReader.readLine()) != null) {
                    if(quit) {
                        break;
                    }
                    if(!login) {
                        if (message.equalsIgnoreCase("QUIT")) {
                            quit = true;
                        }
                        else {
                            String[] splitString = message.split("_");
                            String ID = splitString[0];
                            String password = splitString[1];
                            if (database.checkID(ID).equals(null)) {
                                studentWriter.println("Wrong ID");
                            } else if (!database.checkID(ID).equals(password)) {
                                studentWriter.println("Wrong password");
                            } else {
                                login = true;
                                balance = database.getBalance(ID);
                                studentWriter.println(balance);
                            }
                        }
                    } else {
                        if(message.equalsIgnoreCase("LOGOUT")) {
                            login = false;
                        }
                        else if (message.contains("INFO")){
                            String[] splitString = message.split("_");
                            String studentID = splitString[1];
                            studentWriter.println(database.getStudentInfo(studentID));
                        }
                        else if (message.contains("SHOWTRANS")) {
                            String[] splitString = message.split("_");
                            String studentID = splitString[1];
                            studentWriter.println(database.getTransactionInfo(studentID));
                        }
                        else if (message.contains("SHOWDEBT")) {
                            String[] splitString = message.split("_");
                            String studentID = splitString[1];
                            studentWriter.println(database.getDebtInfo(studentID));
                        }
                        else if (message.contains("ADD")) {
                            String[] splitString = message.split("_");
                            String studentID = splitString[1];
                            double amount = Double.parseDouble(splitString[2]);
                            database.updateTransaction('O', studentID, amount, 1);
                            balance = balance + amount;
                            studentWriter.println(balance);
                        }
                        else if(message.contains("SUB")) {
                            String[] splitString = message.split("_");
                            String studentID = splitString[1];
                            double amount = Double.parseDouble(splitString[2]);
                            if(balance >= amount) {
                                database.updateTransaction('O', studentID, -1 * amount, 1);
                                balance = balance - amount;
                                studentWriter.println(balance);
                            }
                            else {
                                studentWriter.println("Error! You don't have enough money.");
                            }
                        }
                        else if(message.contains("PAY")) {
                            String[] splitString = message.split("_");
                            String studentID = splitString[1];
                            String transactionID = splitString[2];
                            String compareID = database.checkTransaction(transactionID);
                            if(compareID == null) {
                                studentWriter.println("404 Not found");
                            }
                            else if (!compareID.equals(studentID)) {
                                studentWriter.println("Error occur");
                            }
                            else {
                                double amount = database.getAmount(transactionID);
                                if(balance >= amount) {
                                    database.updateTransaction('I', studentID, -1 * amount, 1);
                                    balance = balance - amount;
                                    studentWriter.println(balance);
                                }
                                else {
                                    studentWriter.println("Error! You don't have enough money.");
                                }
                            }
                        }
                    }
                }
                studentSocket.close();
            } catch (IOException | SQLException e) {
                throw new RuntimeException(e);
            }
        }
    }
}


