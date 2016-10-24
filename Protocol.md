# Sync Daemon Protocol
Sync Daemon Protocol version 0.1

## Basic
* All tcp messages end with a CRLF

## Server
### Broadcast

* 100 -- Server Name, must contain "Symon Server"
* 101 -- Protocol Version

### SslStream

* 201 -- Hello from server
	
## Client
### SslStream

* 200 -- Hello from client
