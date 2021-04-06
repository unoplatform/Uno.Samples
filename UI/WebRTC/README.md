# WebRTC Sample for Uno - WebAssembly

This is a short sample demonstrating how to establish a WebRTC connection between 2 browsers in a Uno WebAssembly application.

1. To use it, simply compile and start the Signaling Server. Ensure it is listening on the url http://localhost:6173/.
2. Compile and start 2 instances of the WebRTC.Wasm project.
3. On the first instance, enter a room name and press <kbd>Create Room</kbd>.
4. On the second instance, select the room and press <kbd>Join</kbd>.
5. The connection will be established between the browsers and you can send a message to another instance.

## Application Flow

1. User A press <kbd>Create Room</kbd>
2. Application A is creating a _SPD Offer_ (Ice Candidate)
3. Application A is sending a `CreateRoom` request to SignalingServer through SignalR and sends the room name and the _SPD Offer_
4. Application B receives the `Rooms` message from SignalingServer using SignalR
5. User B selects the room and press <kbd>Join</kbd>
6. Application B uses the remote _SPD offer_ to create a _SPD Answer_
7. Application B is sending a `Join` request to SignalingServer through Signal and sends the room id and the _SDP Answer_
8. Application A receives the `Answer` message from SignalingServer using SignalR
9. Application A uses the _SDP Answer_ to launch the WebRTC connection between the two browsers
10. The connection is negotiated and established between the 2 browsers

## Technology Stack

This project use the following technologies:

* Uno Platform
* Aspnet Core - for the signaling server
* SignalR for communication between server and clients
* TypeScript
* WebRTC JavaScript APIs of the browser - no add-on or polyfills.

## Missing Parts

This sample is demonstrating the basic parts of WebRTC by establishing a connection using the interop between the managed code (C#) and JavaScript (here coded in TypeScript).

**STUN & TURN Servers**

STUN and TURN servers should be configured by creating a `RTCConfiguration` object and used in the `RTCPeerConnection` constructor or by calling the `.setConfiguration()` api. [Documentation on Mozilla](https://developer.mozilla.org/en-US/docs/Web/API/RTCConfiguration/iceServers).

> STUN and TURN servers are used to facilitate communication for networks with stricter NAT restrictions. The STUN server will provide help to establish a direct connection between the browsers. When it's definitely not possible because of firewall restrictions, the TURN server could be used as the proxy to transmit data from the peers.

**Media Streaming**

One of the biggest use case of WebRTC is the webcam streaming between browsers. The `.getUserMedia()` api will give access to the streamed media content and it can be sent to other peer using the `.addTrack()` api. [Documentation on Mozilla](https://developer.mozilla.org/en-US/docs/Web/API/RTCPeerConnection/addTrack).

``` javascript
// Sending the media to the other peer
const stream = await navigator.mediaDevices.getUserMedia(constraints);
for(const track of stream.getTracks())
    connection.addTrack(track, stream);
```

