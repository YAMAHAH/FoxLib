namespace FoxtableLib.Stomp.Client
{
    public enum StompCommandEnum
    {
        CONNECT,
        CONNECTED,
        SEND,
        SUBSCRIBE,
        UNSUBSCRIBE,
        ACK,
        NACK,
        BEGIN,
        COMMIT,
        ABORT,
        DISCONNECT,
        MESSAGE,
        RECEIPT,
        ERROR
    }
}