package AppControl;

public class Transaction {
    private String transactionID;
    private String studentID;
    private double amount;
    private String message;
    private boolean status;

    public Transaction(String input) {
        String[] data = input.split("_");
        transactionID = data[0];
        studentID = data[1];
        amount = Integer.parseInt(data[2]);
        message = data[3];
        status = (data[4] == "1");
    }

    public double getAmount() {
        return amount;
    }

    public String getStudentID() {
        return studentID;
    }

    public String getTransactionID() {
        return transactionID;
    }

    public String getMessage() {
        return message;
    }
}
