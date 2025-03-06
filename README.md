# Elite Dangerous Journal File WebSocket Server

A server that monitors your Elite Dangerous journal files and sends the entries over WebSockets making them available to other apps on your network.

## Setup

 * Requires the .NET 7 Runtime
 * Only supports running on Windows at this time
 * If your journal files are not in the default location, you will need to add a "JournalLocation" entry to your appsettings.json file.

## Usage 
Run with `dotnet run`. Websocket is available at ws://localhost:5275/ws

When a journal file is created or changed, the [JSON Lines](https://jsonlines.org/) data will be sent over the websocket. More than one entry may be sent at a time.   

There is a barebones testing html page available at http://localhost:5275/  Press the connect button to connect to the websocket and then when a journal file changes, the entries will be displayed on the page.