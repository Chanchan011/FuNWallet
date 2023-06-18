package AppControl;

import java.io.*;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

public class Control {
    private static final String HOST = "localhost";
    private static final int PORT = 1234;
    private String ID;
    private String pass;
    private double balance;

    public Control() throws InvalidCredentialException {
        String userID;
        String password;
        try (Socket socket = new Socket(HOST, PORT)) {
            Scanner input = new Scanner(System.in);
            //Placeholder getting user input. Need to be changed to fit UI
            System.out.println("Enter user ID:");
            userID = input.next();
            System.out.println("Enter password:");
            password = input.next();
            //
            PrintWriter serverWriter = new PrintWriter(socket.getOutputStream(), true);
            BufferedReader serverReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
            serverWriter.write(userID + "_" + password);
            double serverReply = Double.parseDouble(serverReader.readLine());
            if (serverReply == -1) {
                throw new InvalidCredentialException("ID or password ish wrong.");
            } else {
                balance = serverReply;
            }
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
    }

    public Student getStudentInfo() {
        try (Socket socket = new Socket(HOST, PORT)) {
            PrintWriter serverWriter = new PrintWriter(socket.getOutputStream(), true);
            serverWriter.write("INFO_" + ID);
            BufferedReader serverReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
            String serverReply = serverReader.readLine();
            return new Student(serverReply);
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
        return null;
    }

    public List<Transaction> getTransactions() {
        try (Socket socket = new Socket(HOST, PORT)) {
            List<Transaction> list = new ArrayList<>();
            PrintWriter serverWriter = new PrintWriter(socket.getOutputStream(), true);
            String request = "SHOWTRANS_" + ID;
            serverWriter.write(request);
            BufferedReader serverReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
            String serverReply = serverReader.readLine();
            String[] rawData = serverReply.split("\\|");
            for (String rawDatum : rawData) {
                list.add(new Transaction(rawDatum));
            }
            return list;
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
        return null;
    }

    public List<Transaction> getPendingTransactions() {
        try (Socket socket = new Socket(HOST, PORT)) {
            List<Transaction> list = new ArrayList<>();
            PrintWriter serverWriter = new PrintWriter(socket.getOutputStream(), true);
            String request = "SHOWDEBT_" + ID;
            serverWriter.write(request);
            BufferedReader serverReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
            String serverReply = serverReader.readLine();
            String[] rawData = serverReply.split("\\|");
            for (String rawDatum : rawData) {
                list.add(new Transaction(rawDatum));
            }
            return list;
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
        return null;
    }

    public Transaction createTransaction(double amount) throws InvalidAmountException{
        try (Socket socket = new Socket(HOST, PORT)) {
            String request = "";
            if (amount >= 0) {request += "ADD_";}
            else {request += "SUB_";}
            request += Double.toString(Math.abs(amount));
            request += "_" + ID;
            PrintWriter serverWriter = new PrintWriter(socket.getOutputStream(), true);
            BufferedReader serverReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
            serverWriter.write(request);
            String serverReply = serverReader.readLine();
            balance = Double.parseDouble(serverReply);
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
        return null;
    }

    //return true if successful, false if failed
    public boolean resolveTransaction(Transaction transaction) {
        try (Socket socket = new Socket(HOST, PORT)){
            String request = "PAY" + ID + transaction.getTransactionID();
            PrintWriter serverWriter = new PrintWriter(socket.getOutputStream(), true);
            BufferedReader serverReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
            serverWriter.write(request);
            String reply = serverReader.readLine();
            if (reply.equals("1")) {
                balance += transaction.getAmount();
            } else {
                throw new InvalidCredentialException("Broke");
            }
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
        return true;
    }
}
