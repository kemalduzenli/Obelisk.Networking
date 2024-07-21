<h1 align="center">Obelisk Networking</h1>

Obelisk Networking is a lightweight C# networking library primarily designed for use in multiplayer games. It can be used in Unity as well as in other .NET environments such as console applications.

It provides functionality for establishing connections and sending data back and forth, leaving it up to you to decide what data you want to send and when. This is ideal if you like to be in control of your code and know what's going on under the hood.

## Getting Started

# Starting a Server

To start a server, we need to create a new Server instance and then call its Start method, which takes in the port we want it to run on and the maximum number of clients we want to allow to be connected at any given time. You'll likely want to run this code as soon as your server application starts up.
