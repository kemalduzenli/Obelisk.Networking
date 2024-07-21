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

Obelisk's <b>Server</b> and <b>Client</b> classes both have several events to allow you to run your own code when various things happen.

For example, you'll likely want your server to spawn a player object when a client connects and destroy it again when they disconnect. You can do this by subscribing your spawn and despawn methods to the <b>OnClientConnected</b> and <b>OnClientDisconnected</b> events.

For a complete list of available events, check out the server events and client events.

### Examples

```csharp
server.OnClientConnected += (Connection conn) =>
{
    Debug.Log($"New Connection[{conn.Id}] is connected");
};
```

```csharp
server.OnClientDisconnected += (Connection conn) =>
{
    Debug.Log($"Connection[{conn.Id}] is disconnected");
};
```

## Sending Data

In order to send data over the network, it has to be converted to bytes first—you can't just send a string or an int directly. Obelisk provides the <b>Packet</b> struct to make this process really easy.

### Creating a Packet
The first step of sending a packet is to get an instance of the struct. This is done using the <b>Create</b> method, which requires the packet's an ID (packet.type) as parameters.

```csharp
Packet packet = Packet.Create(1);
```

Packet IDs are used to identify what type of packet you're sending, which allows the receiving end to determine how to properly handle it. In the example above, we set the packet ID to 1 (in practice you'd probably want to use an enum for packet IDs instead of hard-coding the number).

### Adding Data to the Packet

To write data to our packet, we can simply call the Add method for the type we want to write. For examples:

```csharp
packet.WriteInt(31);
packet.WriteFloat(31.62f);
packet.WriteBool(true);
packet.WriteByte(1);

packet.WriteString("Hello world");

packet.WriteBytes(new byte[2] { 1, 2 });
```

### Sending the Packet

Once you've added the data you want to include in your packet, it's time to send it. Clients have only one Send method, while servers have Send (which has an overload as well).

```csharp
client.Send(packet); // Sends the message to the server

server.Send(packet); // Sends the message to all connected clients
server.Send(<toClientId>, packet); // Sends the message to a specific client
```

Make sure to replace <toClientId> with the ID of the client you want to send the packet to, or who you don't want to sent the packet to if you're using the <b>Send(packet)</b> method.

## Handling the Packet

Use <b>OnPacket</b> event to detect incoming packets on Server and Clients.

### Use in Server example

```csharp
server.OnPacket += (Connection conn, Packet packet) =>
{
    //TODO
}; 
```

### Use in Client example

```csharp
server.OnPacket += (Packet packet) =>
{
    //TODO
}; 
```


