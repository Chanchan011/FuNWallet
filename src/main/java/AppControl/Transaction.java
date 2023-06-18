package AppControl;

public class Transaction {
    private int senderID;
    private int receiverID;
    private int amount;

    public Transaction(int senderID, int receiverID, int amount) {
        this.senderID = senderID;
        this.receiverID = receiverID;
        this.amount = amount;
    }
}
