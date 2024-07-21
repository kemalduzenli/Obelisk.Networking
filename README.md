<h1 align="center">Obelisk Networking</h1>

Obelisk Networking is a lightweight C# networking library primarily designed for use in multiplayer games. It can be used in Unity as well as in other .NET environments such as console applications.

[![Made in Türkiye](https://img.shields.io/badge/made%20in-t%C3%BCrkiye-white.svg?labelColor=red)]()

It provides functionality for establishing connections and sending data back and forth, leaving it up to you to decide what data you want to send and when. This is ideal if you like to be in control of your code and know what's going on under the hood.

# Getting Started

## Starting a Server

To start a server, we need to create a new Server instance and then call its Start method, which takes in the port we want it to run on and the maximum number of clients we want to allow to be connected at any given time. You'll likely want to run this code as soon as your server application starts up.

```csharp
Server server = new Server();
server.Start(7777, 10);
```

In order for the server to be able to accept connections and process messages, we need to call its Update method on a regular basis. In Unity, this can be done using the provided FixedUpdate method.

```csharp
private void FixedUpdate()
{
    server.Update();
}
```

## Connecting a Client

The process of connecting a client is quite similar. First we create a new Client instance and then we call its Connect method, which expects a host address as the parameter.

Riptide's default transport requires host addresses to consist of an IP address and a port number, separated by a :. Since we're running the server and the client on the same computer right now, we'll use 127.0.0.1 (also known as localhost) as the IP.

```csharp
Client client = new Client();
client.Connect(7777, "127.0.0.1");
```

Finally, we need to call the client's Update method on a regular basis, just like we did with the server.

```csharp
private void FixedUpdate()
{
    client.Update();
}
```

## Hooking Into Events

Obelisk's Server and Client classes both have several events to allow you to run your own code when various things happen.

For example, you'll likely want your server to spawn a player object when a client connects and destroy it again when they disconnect. You can do this by subscribing your spawn and despawn methods to the OnClientConnected and ClientDisconnected events.

The Client class's most useful events are probably the ConnectionFailed and Disconnected events, which come in handy for things like returning the player to the main menu when their connection attempt fails or they're disconnected.

For a complete list of available events, check out the server events and client events.
