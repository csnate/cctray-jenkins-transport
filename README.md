cctray-jenkins-transport
========================

An transport extension for CCTray to use with a Jenkins CI server to monitor and manage projects with authentication.  

Why?
----
Previously, our CI environment was handled with CruiseControl.NET and all developers were able to monitor and manage their projects using the awesome CCTray application supplied with CruiseControl.NET.  We decided to switch to use Jenkins instead as it was a little more robust and better maintained.  However, there were only 2 ways to monitor projects in Jenkins - use the Jenkins Tray App plugin or use the cc.xml page exposed by Jenkins with CCTray.  Neither of these solutions allowed developers to manage (force build, stop build, etc) their projects.  So I wrote a transport extension to use with Jenkins.

Benefits
--------
* You can keep your Jenkins instance locked down with any authorization scheme - no need to use the "Anyone can do anything" authorization
* You can manage any Jenkins project via CCTray - no need to use the interface or the CLI to force a build
* You can login to Jenkins server using your API token. Simply enter the API token as the password for your user account/

Drawbacks
---------
* If you are monitoring a large number of projects, this has the potential to cause a lot of network traffic.
* If you are using a username and password and you change your password, you cannot update your password (clicking Configure for the server doesn't work again).  You have to remove all projects and add the server again.

How it works
------------
Jenkins has an XML Api that can access any project/url in the system via ../api/xml.  The program makes requests to the relevant APIs with a Basic Authorization header.

Prerequisites
=============
1. This has been tested with CCTray >= 1.8.0.  Download the latest from http://sourceforge.net/projects/ccnet/files/CruiseControl.NET%20Releases/
2. This has been tested on Windows 7.  I don't know if/how this will work on a Mac/Linux environment.

Installation
============
1. Build the project locally.
2. Assuming a standard CCTray installation, create a new directory at C:\Program Files (x86)\CCTray\extensions
3. Copy the JenkinsTransport.dll assembly to this new directory.
4. Start CCTray and add a new server from File -> Settings -> Add -> Add Server
5. Select the "Using a transport extension" option.
6. Select the JenkinsTransport from the dropdown list.
7. Click the Configure Extension button.  Enter your server URL and (optionally) username and password.
8. Click Ok and you should now have acccess to your Jenkins projects.

TODOs
-----
* Support multiple Jenkins servers