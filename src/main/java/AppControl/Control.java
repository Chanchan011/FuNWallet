package AppControl;

import java.util.List;

public class Control {
    private String ID;
    private String password;
    private int balance;

    // When app runs, prompt user to input ID and password.
    // After user input the info, create a TCP request for current balance.
    public Control() {
        ID = "00000001";
        password = "root123";
    }

    public String getID() {return ID;}

    public int getBalance() {return balance;}

    // 0: Transaction was approved
    // 1: User ID was not found
    // 2: Password is incorrect
    // 3: Invalid amount
    public void createTransaction(String receiverID, int amount) throws UserNotFoundException, InvalidCredentialException, InvalidAmountException {
        int serverReply = 0;
        switch (serverReply){
            case Status.VALID_TRANSACTION:
                break;
            case Status.USER_NOT_FOUND:
                throw new UserNotFoundException("Check sender/receiver ID");
            case Status.INCORRECT_CREDENTIALS:
                throw new InvalidCredentialException("Incorrect password");
            case Status.INVALID_AMOUNT:
                throw new InvalidAmountException("Check transfer amount");
            default:
                //This should not exist but might as well
        }
    }

    public List<Transaction> getTransactionHistory() {
        return null;
    }
}
