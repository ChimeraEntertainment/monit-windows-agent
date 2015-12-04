> monit-windows-agent is a project that aims to reimplement a Windows [Monit](http://www.mmonit.com "Monit") Agent from scratch, with the primary goal to push status reports to [M/Monit](http://www.mmonit.com "M/Monit").


# Current Project Status

**As of Dec the 4th 2015 I have pushed an update which is basically a rewrite of the C++ monit source code in C#. Extended functionality is given, we're using it in production since 2 months. The documentation below is outdated and will be updated soon!**


(We need your help, see <a href="#please-contribute">Please contribute</a>.)

## What already works

 - Agent is **already running** on command line as well as a Windows Service
 - Agent **client is already reporting system status** like CPU, HDD, Memory usage to M/Monit
 - Agent **server is running** with implemented responses to /\_status?format=xml, /\_getid and /\_ping requests
 - Current configuration takes place in a proprietary XML file

## Screenshots
- M/Monit view of the windows agent:
![monit-windows-agent Screenshot from M/Monit](img/monit-windows-agent-mmonit-1.png)

- Running monit windows service:
![monit-windows-agent Windows Service Screenshot](img/monit-windows-agent-service-1.png)

## What is still to do?

- **Alerting**: Agent must be made configurable for alerts
- Pending design decision if there should be a **monitrc parser** implemented in .NET, or if the configuration should be done *proprietary** (XML file etc)
- **Research**
  - Which data and behaviour does M/Monit expect from its Monit clients? 
 - Specific (TODO: should be made an Issue): If the "controlfile" directive from the monitrc configuration is mandatorily needed by the M/Monit server, and if yes, what it does with it. That could limit our approach to utilize a proprietary configuration for the Windows Agent.
- **Agent Server must respond to /_doAction and other vital web requests** and execute the according action
- Integration of **Swap Memory** status for Windows
- Setting up a proper community and getting some contributors (see below) ;)

# Please contribute!
- I'm not going to achieve all the goals alone
- Not only developers are needed, but also folks who can provide insights into the API and internals of M/Monit.
- You are a pro in parser generation (like with [Bison](http://www.gnu.org/software/bison/), [grammar files](http://dinosaur.compilertools.net/bison/bison_6.html), and it's piece of cake for you to generate a parser [from this (monitrc grammar)](https://bitbucket.org/tildeslash/monit/src/HEAD/src/p.y?at=master)?

## How does developing the Monit Windows Agent work?

- Programmed in C# / .NET 3.5
- Using Monit source code as a (documentation) basis
 - Monit Source Code is here: [https://bitbucket.org/tildeslash/monit/src](https://bitbucket.org/tildeslash/monit/src "https://bitbucket.org/tildeslash/monit/src")
- M/Monit source code is not available
- Using "sniffed" XML (and [source code](https://bitbucket.org/tildeslash/monit/src/HEAD/src/xml.c?at=master) as the basis for the xml format (near to "reverse engineering" because no XSD available)
- Using <a href="https://github.com/pvginkel/NHttp">NHTTP</a> for the web server implementation
- The xml data classes are created by Visual Studio 2013's "Paste XML as class" - functionality. You have to switch to .NET 4.5 framework to gain access to that functionality. After pasting, revert back to .NET 3.5.

## Development Goals / Paradigms
1. Reverse, Learn, Document
2. Get it running. Follow the <a href="http://en.wikipedia.org/wiki/KISS_principle#In_software_development">KISS principle</a> first.
3. Refactoring to patterns - make it reusable.
