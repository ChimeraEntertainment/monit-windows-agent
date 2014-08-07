> monit-windows-agent is a project that aims to re-implement a Windows [Monit](http://www.mmonit.com "Monit") Agent from scratch, with the primary goal to push status reports to [M/Monit](http://www.mmonit.com "M/Monit").


# Current Project Status

## What already works:

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
- Pending design decision if there should be a **monitrc parser** implemented in .NET, or if the configuration should be done **proprietary** (XML file etc)
 - **Research** if the "controlfile" directive from the monitrc configuration is needed by the M/Monit server, and if yes, what it does with it. That could limit our approach to utilize a proprietary configuration for the Windows Agent.
- **Agent Server must respond to /_doAction** - request and execute the according action
- Integration of **Swap Memory** status
- Setting up a proper community and getting some contributors (see below) ;)

## Yeah, but where's the code?

### SHOW YOUR INTEREST!

- Before taking the time to put the code on GitHub and also to maintain it afterwards, I want to draw some attention on it
- I want to get some feedback if the Monit community really wants to have this Windows Client, and if some of you are willing to contribute.
- If no one cares, or if there's no demand, I won't publish this anytime soon.
- Use the issue tracker to show your demand: [Demand Hub Issue](https://github.com/derFunk/monit-windows-agent/issues/1 "Demand Hub")

# Looking for contributors!
- You are a pro in parser generation (like with [Bison](http://www.gnu.org/software/bison/), [grammar files](http://dinosaur.compilertools.net/bison/bison_6.html), and it's piece of cake for you to generate a parser [from this (monitrc grammar)](https://bitbucket.org/tildeslash/monit/src/HEAD/src/p.y?at=master)? Then yell at [Demand Hub Issue](https://github.com/derFunk/monit-windows-agent/issues/1 "Demand Hub")
- If you want to contribute in any other way: Wave at [Demand Hub Issue](https://github.com/derFunk/monit-windows-agent/issues/1 "Demand Hub")
- You can contribute some insights into the internals of M/Monit? Sneak at [Demand Hub Issue](https://github.com/derFunk/monit-windows-agent/issues/1 "Demand Hub")

## How does developing the Monit Windows Agent work?

- Programmed in C#
- Using Monit source code as a (documentation) basis (no re-use of operative code of course)
 - Monit Source Code is here: [https://bitbucket.org/tildeslash/monit/src](https://bitbucket.org/tildeslash/monit/src "https://bitbucket.org/tildeslash/monit/src")
- M/Monit source code is not available
- Using "sniffed" XML (and [source code](https://bitbucket.org/tildeslash/monit/src/HEAD/src/xml.c?at=master) as the basis for the xml format (near to "reverse engineering" because no XSD available)