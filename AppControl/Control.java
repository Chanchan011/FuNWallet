package AppControl;

import java.io.DataInputStream;
import java.io.DataOutputStream;
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

    public Control() throws InvalidCredentialException{
        String userID = "";
        String password = "";
        try (Socket socket = new Socket(HOST, PORT)) {
            Scanner input = new Scanner(System.in);
            //Placeholder getting user input. Need to be changed to fit UI
            System.out.println("Enter user ID:");
            userID = input.next();
            System.out.println("Enter password:");
            password = input.next();
            //
            DataOutputStream request = new DataOutputStream(socket.getOutputStream());
            DataInputStream reply = new DataInputStream(socket.getInputStream());
            request.writeUTF(userID + "_" + password);
            double serverReply = Double.parseDouble(reply.readUTF());
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
        try (Socket socket = new Socket(HOST, PORT)){
            DataOutputStream dout = new DataOutputStream(socket.getOutputStream());
            dout.writeUTF("INFO_" + ID);
            DataInputStream din = new DataInputStream(socket.getInputStream());
            String serverReply = din.readUTF();
            return new Student(serverReply);
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
        return null;
    }

    public List<Transaction> getTransactions() {
        try (Socket socket = new Socket(HOST, PORT)) {
            List<Transaction> list = new ArrayList<Transaction>();
            DataOutputStream dout = new DataOutputStream(socket.getOutputStream());
            String request = "SHOWTRANS_" + ID;
            dout.writeUTF(request);
            DataInputStream din = new DataInputStream(socket.getInputStream());
            String serverReply = din.readUTF();
            String[] rawData = serverReply.split("|");
            for (int i = 0; i < rawData.length; i++) {
                list.add(new Transaction(rawData[i]));
            }
            return list;
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
        return null;
    }

    public List<Transaction> getPendingTransactions() {
        try (Socket socket = new Socket(HOST, PORT)) {
            List<Transaction> list = new ArrayList<Transaction>();
            DataOutputStream dout = new DataOutputStream(socket.getOutputStream());
            String request = "SHOWDEBT_" + ID;
            dout.writeUTF(request);
            DataInputStream din = new DataInputStream(socket.getInputStream());
            String serverReply = din.readUTF();
            String[] rawData = serverReply.split("|");
            for (int i = 0; i < rawData.length; i++) {
                list.add(new Transaction(rawData[i]));
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
            DataOutputStream dout = new DataOutputStream(socket.getOutputStream());
            DataInputStream din = new DataInputStream(socket.getInputStream());
            dout.writeUTF(request);
            String serverReply = din.readUTF();
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
            DataOutputStream dout = new DataOutputStream(socket.getOutputStream());
            DataInputStream din = new DataInputStream(socket.getInputStream());
            dout.writeUTF(request);
            String reply = din.readUTF();
            if (reply == "1") {
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
