# Sync Daemon Protocol
Sync Daemon Protocol version 0.1

## Basic
* All tcp messages end with a CRLF

## Server
### Broadcast

* 100 -- Server Name, must contain "Symon Server"
* 101 -- Protocol Version

### SslStream

* 201 -- Hello from server, return when get hello from client
* 202 -- Ping
* 203 -- Pong

### General

#### System Call
* 300 -- System Call Mode
* 301 -- Command
* 302 -- Arguments
* 303 -- User
* 304 -- Password
	
## Client
### SslStream

* 200 -- Hello from client
* 202 -- Ping
* 203 -- Pong

## General
* 305 -- Successfully Finish System Call
