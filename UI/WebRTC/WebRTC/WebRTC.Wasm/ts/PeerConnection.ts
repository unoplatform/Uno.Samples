namespace Uno.WebRTC {

    export class PeerConnection {

        private constructor(private connection: RTCPeerConnection, private element: HTMLElement, private dataChannel?: RTCDataChannel|undefined) {
            connection.addEventListener("icecandidate", msg => this.onIceCandidate(msg));
            if (dataChannel) {
                this.setDataChannel(dataChannel);
            }
        }

        private setDataChannel(dataChannel: RTCDataChannel) {
            if (this.dataChannel && this.dataChannel != dataChannel) {
                this.dataChannel.close();
            }
            this.dataChannel = dataChannel;

            dataChannel.addEventListener("open", msg => this.onChannelOpened(msg));
            dataChannel.addEventListener("message", msg => this.onChannelMessage(msg));
            dataChannel.addEventListener("error", msg => this.onChannelError(msg));
            dataChannel.addEventListener("close", msg => this.onChannelClosed(msg));
        }

        private onChannelOpened(event: Event) {
            this.raiseEvent("Opened");
        }

        private onChannelMessage(event: MessageEvent) {
            this.raiseEvent("Message", event.data);
        }

        private onChannelError(event: RTCErrorEvent) {
            this.raiseEvent("Error", event.error.errorDetail);
        }

        private onChannelClosed(event: Event) {
            this.raiseEvent("Closed");
        }

        private onIceCandidate(event: RTCPeerConnectionIceEvent) {
            this.raiseEvent("IceCandidate", this.connection.localDescription);
        }

        private raiseEvent(eventName: string, detail: any|undefined = undefined) {
            if (detail) {
                const eventToManagedCode = new CustomEvent(eventName, { detail: detail });
                this.element.dispatchEvent(eventToManagedCode);
            } else {
                const eventToManagedCode = new Event(eventName);
                this.element.dispatchEvent(eventToManagedCode);
            }
        }

        public async SetAnswer(spdAnswer: RTCSessionDescriptionInit) {
            await this.connection.setRemoteDescription(spdAnswer);
        }

        public async SendMessage(message: string): Promise<void> {
            if (this.dataChannel) {
                this.dataChannel.send(message);
            }
        }

        public Close(): void {
            this.dataChannel.close();
            this.connection.close();
        }

        public static async CreateInitiator(channelName: string, element: HTMLElement): Promise<PeerConnection> {

            const connection = new RTCPeerConnection();
            const dataChannel = connection.createDataChannel(channelName);
            const peerConnection = new PeerConnection(connection, element, dataChannel);

            const offer = await connection.createOffer();
            connection.setLocalDescription(offer);

            return peerConnection;
        }

        public static async CreateRemote(element: HTMLElement, iceOffer: RTCSessionDescriptionInit): Promise<PeerConnection> {

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
}