var Uno;
(function (Uno) {
    var WebRTC;
    (function (WebRTC) {
        class PeerConnection {
            constructor(connection, element, dataChannel) {
                this.connection = connection;
                this.element = element;
                this.dataChannel = dataChannel;
                connection.addEventListener("icecandidate", msg => this.onIceCandidate(msg));
                if (dataChannel) {
                    this.setDataChannel(dataChannel);
                }
            }
            setDataChannel(dataChannel) {
                if (this.dataChannel && this.dataChannel != dataChannel) {
                    this.dataChannel.close();
                }
                this.dataChannel = dataChannel;
                dataChannel.addEventListener("open", msg => this.onChannelOpened(msg));
                dataChannel.addEventListener("message", msg => this.onChannelMessage(msg));
                dataChannel.addEventListener("error", msg => this.onChannelError(msg));
                dataChannel.addEventListener("close", msg => this.onChannelClosed(msg));
            }
            onChannelOpened(event) {
                this.raiseEvent("Opened");
            }
            onChannelMessage(event) {
                this.raiseEvent("Message", event.data);
            }
            onChannelError(event) {
                this.raiseEvent("Error", event.error.errorDetail);
            }
            onChannelClosed(event) {
                this.raiseEvent("Closed");
            }
            onIceCandidate(event) {
                this.raiseEvent("IceCandidate", this.connection.localDescription);
            }
            raiseEvent(eventName, detail = undefined) {
                if (detail) {
                    const eventToManagedCode = new CustomEvent(eventName, { detail: detail });
                    this.element.dispatchEvent(eventToManagedCode);
                }
                else {
                    const eventToManagedCode = new Event(eventName);
                    this.element.dispatchEvent(eventToManagedCode);
                }
            }
            async SetAnswer(spdAnswer) {
                await this.connection.setRemoteDescription(spdAnswer);
            }
            async SendMessage(message) {
                if (this.dataChannel) {
                    this.dataChannel.send(message);
                }
            }
            Close() {
                this.dataChannel.close();
                this.connection.close();
            }
            static async CreateInitiator(channelName, element) {
                const connection = new RTCPeerConnection();
                const dataChannel = connection.createDataChannel(channelName);
                const peerConnection = new PeerConnection(connection, element, dataChannel);
                const offer = await connection.createOffer();
                connection.setLocalDescription(offer);
                return peerConnection;
            }
            static async CreateRemote(element, iceOffer) {
                const connection = new RTCPeerConnection();
                const peerConnection = new PeerConnection(connection, element);
                connection.ondatachannel = msg => {
                    const dataChannel = msg.channel;
                    connection.ondatachannel = null;
                    peerConnection.setDataChannel(dataChannel);
                };
                await connection.setRemoteDescription(iceOffer);
                const answer = await connection.createAnswer();
                await connection.setLocalDescription(answer);
                return peerConnection;
            }
        }
        WebRTC.PeerConnection = PeerConnection;
    })(WebRTC = Uno.WebRTC || (Uno.WebRTC = {}));
})(Uno || (Uno = {}));
//# sourceMappingURL=uno-webrtc.js.map