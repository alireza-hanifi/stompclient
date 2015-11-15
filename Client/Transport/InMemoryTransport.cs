using Stomp;
using Stomp.Listeners;
using System;
using System.Timers;

namespace Stomp.Client.Transport
{
  public class InMemoryTransport : ITransport
  {
    private readonly StompMessageSerializer _serializer = new StompMessageSerializer();
    private readonly StompInMemoryClient _client = new StompInMemoryClient();
    private readonly StompInMemoryListener _stompInMemoryListener;

    public Timer inHeartbeatTimer { get; set; }

    public Timer outHeartbeatTimer { get; set; }

    public Action OnOpen { get; set; }

    public Action<string> OnMessage { get; set; }

    public InMemoryTransport(StompInMemoryListener stompInMemoryListener)
    {
      this._stompInMemoryListener = stompInMemoryListener;
    }

    public void Connect()
    {
      this._client.OnSend += (Action<StompMessage>) (m => this.OnMessage(this._serializer.Serialize(m)));
      if (this._stompInMemoryListener.OnConnect != null)
        this._stompInMemoryListener.OnConnect((IStompClient) this._client);
      if (this.OnOpen == null)
        return;
      this.OnOpen();
    }

    public void Send(string message)
    {
      if (this._client.OnMessage == null)
        return;
      this._client.OnMessage(this._serializer.Deserialize(message));
    }

    public void Close()
    {
    }
  }
}
