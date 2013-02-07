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

Drawbacks
---------
* Currently, you have to supplied your username and password in a config file.  This data is encoded in a Basic Authorization header on requests.
* If you are monitoring a large number of projects, this has the potential to cause a lot of network traffic.

How it works
------------
Jenkins has an XML Api that can access any project/url in the system via ../api/xml.  The program makes requests to the relevant APIs with a Basic Authorization header.

Prerequisites
=============
# This has been tested with CCTray >= 1.8.0.  Download the latest from http://sourceforge.net/projects/ccnet/files/CruiseControl.NET%20Releases/
# This has been tested on Windows 7.  I don't know if/how this will work on a Mac/Linux environment.

Installation
============
# Build the project locally.
# Assuming a standard CCTray installation, create a new directory at C:\Program Files (x86)\CCTray\extensions
# Copy the JenkinsTransport.dll and JenkinsTransport.dll.config to this new directory.
# Update the config with your values.
# Start CCTray and add a new server from File -> Settings -> Add -> Add Server
# Select the "Using a transport extension" option.
# Select the JenkinsTransport from the dropdown list.
# IMPORTANT - click the Configure Extension button.  This doesn't look like it does anything, but it will initialize the transport.
# Click Ok and you should now have acccess to your Jenkins projects.

TODOs
-----
* Jenkins users have an API token. It would be great to figure out how to use this instead of the encoded username and password in the config file
* Instead of a config file, pop a windows form to capture this same data on configure.