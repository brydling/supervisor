# supervisor
## About
This is a tool for starting and stopping processes on multiple computers from a single GUI. This repository is for the GUI application written in Visual C# and includes project files for Visual Studio Express. The server, running in one instance on every computer where processes shall be started and stopped, is in a different project called [local_supervisor](http://github.com/brydling/local_supervisor).
## Usage
The example configuration files included (hosts.ini and processes.ini) shall be put in the same directory as the supervisor .exe-file and are configured to work with the example configuration file for local_supervisor. Start the two programs on the same computer and test. I think the configuration files are pretty self-explanatory.

The supervisor GUI is a window without a frame that can be moved around by clicking and dragging in the gray areas of the window, where there are no graphical elements. The position is saved in the Windows registry to let the window pop up in the same position the next time supervisor is started. Use Alt+F4 to close the window.

Every process configured in processes.ini gets its own square label showing the process' name and the name of the host on which the process is configured. If the label background is red it means that the process is stopped, if it is green it means that the process is started and if it is gray it means there is no connection between supervisor.exe and the local_supervisor.exe handling that process.

To start and stop processes, click on the big red/green label for the process that you want to start or stop. The Kill-button can be used to aggressively kill the process if it has crashed, and the "Start minimized"-checkbox can be used to start the process with the window minimized.
