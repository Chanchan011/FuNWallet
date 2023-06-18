# FuNWallet
## Introduction
This is a proof of concept for our e-wallet that specialized in cross-border money transfer. This application is changed to crater to students studying aboard
## App Preview
Building an entire blockchain, generally speaking, is a time-consuming and a very resource intenstive task. Given the scope of our current project and the short deadline given; we have decided to simply the application for demonstration purposes.

There are 2 parts to this project: The client application and the server. The server in this case plays the role of the blockchain in proccessing user requests and approving transactions made by the client. The client provides the user with an user-friendly GUI which should help them navigate their tasks fairly easily.

FuNWallet allows its users to withdrawl or deposit money to and from the wallet (which is currently being presented by a placeholder). Furthermore, it allows education institution to create tution fee requests that students could pay when they have enough in their balance. This application could also double as an application to manage students information, and can be updated as such should the users demand.
### Prerequisite
For Client:
+ Something
+ Something Something
+ A healthy appatite for bread :bread:

For Server:
+ An SQL data base, preferably MySQL
+ Java JDK version 11 or higher
+ Lots of cheese, preferably mozarella :cheese:
### Server Installation Process
For testing purposes, it is best to utilize a virtual machine to set up the server. However, if resources are limited, the server should be able to run on the same machine as the client. The detailed installtion process is as followed
+ **Installing Prerequisite:** A database is neccessary in order to manage user data. Therefore, SQL installation is a must. Plain MySQL should work, however, in order to ease management, feel free to install assisting programs such as XAMPP or Workbench.
+ **Create database:** A demo script is provided in order to create a simple database for the proof of concept. However, if the situation demands, the database can be redesign to better suit the user needs. After configuring MySQL and creating the database, ***REMEMBER TO UPDATE /src/main/java/AuthSever/Database.java to match that of the database you just created***
+ **Compile and run:** Compile and run /src/main/java/AuthServer.java. Whether you compile it via an IDE or via commandline should not matter. You can also manually change the port on which this application runs in the aforemnetioned file
+ **Verify Operation:** After finish compiling and running the application successfully, move on to Client installation process. In order for the application to run smoothly, the client ***MUST*** be able to ping the Server and vice versa. Any errors relating to logging in is most likely the result of networking.
### Client Installation Process
