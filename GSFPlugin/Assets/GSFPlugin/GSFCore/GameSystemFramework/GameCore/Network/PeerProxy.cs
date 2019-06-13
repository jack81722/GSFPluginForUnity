using System.Collections;
using System.Collections.Generic;

public abstract class PeerProxy
{
    protected ISerializer serializer;

    public delegate void ReceiveHandler(object obj);
    public ReceiveHandler OnReceiveEvent;

    public PeerProxy(ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public abstract void Send(object obj);

}

public interface ISerializer
{
    byte[] Serialize(object obj);
    object Deserialize(byte[] dgram);
}
