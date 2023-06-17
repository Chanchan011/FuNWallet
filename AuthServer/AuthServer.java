package AuthServer;

import java.io.DataInputStream;
import java.net.ServerSocket;
import java.net.Socket;

public class AuthServer {
    public static void main(String[] args) {
        try (ServerSocket serverSocket = new ServerSocket(1001)) {
            Socket socket = serverSocket.accept();
            DataInputStream din = new DataInputStream(socket.getInputStream());
            String message = din.readUTF();
            System.out.println(message);
        } catch (Exception e) {
            System.out.println("Something is wrong with the socket");
        }
    }
}
