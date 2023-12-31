package AuthServer;


import java.sql.*;

public class Database {
    private final String host = "jdbc:mySQL://localhost:3306/data";
    private final String admin = "root";
    private final String password = "";
    private int transID = 0;
    private Connection connection;
    private static final Database database = new Database();

    private Database() {
        try {
            connection = DriverManager.getConnection(host, admin, password);
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public static Database getDatabase() {
        return database;
    }


    public Connection getConnection() {
        if (connection == null) {
            try {
                connection = DriverManager.getConnection(host, admin, password);
                database.generateData();
            } catch (SQLException e) {
                e.printStackTrace();
            }
        }
        return connection;
    }

    public void closeConnection() {
        try {
            if (connection != null) {
                connection.close();
            }
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public void clearDatabase() {
        try {
            String query = "DELETE FROM data";
            Statement statement = connection.createStatement();
            statement.executeUpdate(query);
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public void generateData() {
        try {
            String query = """
                    insert into Student ('123456789','root123','Student1','Vietnam');
                    insert into Student ('987654321','root123','Student2','Congo');
                    insert into Transaction ('O0001','123456789',1000,'test',1);
                    insert into Transaction ('O0002','123456789',-200,'withdraw',1);
                    insert into Transaction ('I0001','123456789',-1500,'fee',0);
                    insert into Transaction ('I0002','123456789', 1000,'scholarship',1)""";
            Statement statement = connection.createStatement();
            ResultSet resultSet = statement.executeQuery(query);
        } catch (SQLException ex) {
            ex.printStackTrace();
        }
    }

    public void updateTransaction(String transactionID) throws SQLException {
        String query = "update transaction "
                + "set status =?" +
                "where ID=?";

        PreparedStatement preparedStatement = connection.prepareStatement(query);
        preparedStatement.setInt(1, 1);
        preparedStatement.setString(2, transactionID);
        preparedStatement.executeUpdate();
    }

    public String getTransactionID(char type) {
        setTransID(getTransID() + 1);
        if(type == 'O')
            return "O" + getTransID();
        else
            return "I" + getTransID();
    }

    public void addTransaction(char type, String studentID, double amount, int status) throws SQLException {
        String transactionID = getTransactionID(type);
        String query = "INSERT INTO transaction(ID, StudentID, Amount, Message, Status)"
                + "VALUES(?, ?, ?, ?, ?)";

        PreparedStatement preparedStatement = connection.prepareStatement(query);
        preparedStatement.setString(1, transactionID);
        preparedStatement.setString(2, studentID);
        preparedStatement.setDouble(3, amount);
        preparedStatement.setString(4, "");
        preparedStatement.setInt(5, status);

        preparedStatement.executeUpdate();
    }

    public String getStudentInfo(String studentID) {
        String name = "", nationality = "";
        try {
            String query = "SELECT Name, Nationality FROM student WHERE ID = " + studentID + ";";
            PreparedStatement statement = connection.prepareStatement("SELECT Name, Nationality FROM student WHERE ID = ?");
            statement.setString(1, studentID);
            ResultSet resultSet = statement.executeQuery();
            while (resultSet.next()) {
                name = resultSet.getString(1);
                nationality = resultSet.getString(2);
            }

        } catch (SQLException ex) {
            ex.printStackTrace();
        }
        return studentID + "_" + name + "_" + nationality;
    }

    public String getTransactionInfo(String studentID) {
        StringBuilder transactionInfo = new StringBuilder();
        String temp;
        String id, amount, message, status;
        try {
            String query = "SELECT ID, Amount, Message, Status FROM transaction WHERE StudentID = " + studentID + " LIMIT 10" + ";";
            PreparedStatement statement = connection.prepareStatement("SELECT ID, Amount, Message, Status FROM transaction WHERE StudentID = ? LIMIT 10");
            statement.setString(1, studentID);
            ResultSet resultSet = statement.executeQuery();
            while (resultSet.next()) {
                id = resultSet.getString(1);
                amount = resultSet.getString(2);
                message = resultSet.getString(3);
                status = resultSet.getString(4);
                temp = id + "_" + amount + "_" + message + "_" + status;
                transactionInfo.append(temp);
                transactionInfo.append("|");
            }
        } catch (SQLException ex) {
            ex.printStackTrace();
        }
        return transactionInfo.toString();
    }

    public String getDebtInfo(String studentID) {
        StringBuilder debt = new StringBuilder();
        String temp;
        String id, amount, message;
        try {
            String query = "SELECT ID, Amount, Message FROM transaction WHERE StudentID = " + studentID + " AND Status = 0" + " LIMIT 10" + ";";
            PreparedStatement statement = connection.prepareStatement("SELECT ID, Amount, Message FROM transaction WHERE StudentID = ? AND Status = 0 LIMIT 10");
            statement.setString(1, studentID);
            ResultSet resultSet = statement.executeQuery();
            while (resultSet.next()) {
                id = resultSet.getString(1);
                amount = resultSet.getString(2);
                message = resultSet.getString(3);
                temp = id + "_" + amount + "_" + message;
                debt.append(temp);
                debt.append("|");
            }
        } catch (SQLException ex) {
            ex.printStackTrace();
        }
        return debt.toString();
    }

    public String checkID(String studentID) {
        String pass = "";
        try {
            String query = "SELECT Pass FROM student WHERE ID = " + studentID + ";";
            PreparedStatement statement = connection.prepareStatement("SELECT Pass FROM student WHERE ID = ?");
            statement.setString(1, studentID);
            ResultSet resultSet = statement.executeQuery();
            while (resultSet.next()) {
                pass = resultSet.getString(1);
            }
        } catch (SQLException ex) {
            ex.printStackTrace();
        }
        if(pass.equals(""))
            return null;
        return pass;
    }

    public String checkTransaction(String transactionID) {
        String studentID = "";
        try {
            String query = "SELECT StudentID FROM transaction WHERE ID = " + transactionID + ";";
            PreparedStatement statement = connection.prepareStatement("SELECT StudentID FROM transaction WHERE ID = ?");
            statement.setString(1, transactionID);
            ResultSet resultSet = statement.executeQuery();
            while (resultSet.next()) {
                studentID = resultSet.getString(1);
            }
        } catch (SQLException ex) {
            ex.printStackTrace();
        }
        if(studentID.equals(""))
            return null;
        return studentID;
    }

    public double getAmount(String transactionID) {
        double amount = 0;
        try {
            String query = "SELECT amount FROM transaction WHERE ID = " + transactionID + ";";
            PreparedStatement statement = connection.prepareStatement("SELECT amount FROM transaction WHERE ID = ?");
            statement.setString(1, transactionID);
            ResultSet resultSet = statement.executeQuery();
            while (resultSet.next()) {
                amount = resultSet.getDouble(1);
            }
        } catch (SQLException ex) {
            ex.printStackTrace();
        }
        return amount;
    }

    public double getBalance(String studentID) {
        double balance = 0;
        try {
            String query = "SELECT SUM(Amount) FROM Transaction WHERE ID = " + studentID + " AND Status = 1" + ";";
            PreparedStatement statement = connection.prepareStatement("SELECT SUM(Amount) FROM Transaction WHERE StudentID = ? AND Status = 1");
            statement.setString(1, studentID);
            ResultSet resultSet = statement.executeQuery();
            while (resultSet.next()) {
                balance = resultSet.getDouble(1);
            }
        } catch (SQLException ex) {
            ex.printStackTrace();
        }
        return balance;
    }

    public int getTransID() {
        return transID;
    }

    public void setTransID(int transID) {
        this.transID = transID;
    }
}
