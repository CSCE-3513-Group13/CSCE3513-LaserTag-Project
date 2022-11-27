# CSCE3513 LaserTag Project

## To run the application:

You can either directly run the .exe files in the specified folders (windows only):

#2:CSCE3513-LaserTag-Project-master\CSCE3513 LaserTag Project\bin\Debug\CSCE3513 LaserTag Project.exe

(Windows might throw unsafe warning since these were compiled on my machine)

OR

you can download and install MS visual studio (include .NET desktop development) and open the CSCE 4513 HW.sln and compile & run the project.
Link: https://visualstudio.microsoft.com/vs/community/

This was written in c# .net 4.8 and only works on windows machines.


## To use the application:

This application requires you to launch it atleast twice and click the server, and client respecitvely. In our case, the server is the master/controlling node where you launch and start the game, change configs etc. The client window lets you login with your ID or create a new user to be added to the SQL database.

When a client joins, it automatically gets assigned a team to autobalance them (Red or Blue). When ready, on the server window, you can either click the start or stop game buttons, or use the designated function keys. The client window then automatically switches to the game action window where colored logs will appear for damage delt. You can also connect up many different client windows if you want. On run, the game traffic simulator automatically runs upon start.

The score, and game feed will be displayed on both the server and client.



## Issues:

Things to note is that there is no heartbeat or callback system in place to determine if the client has left. Therefore to reset the game in its entirety, you must close all windows and restart.



