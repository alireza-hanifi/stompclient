using System;
using System.Timers;
using WebSocketSharp;

namespace Stomp.Client.Transport
{
  public class WebTransportTransport : ITransport
  {
    private readonly string _address;
    private WebSocket _webSocket;

    public Action OnOpen { get; set; }

    public Action<string> OnMessage { get; set; }

    public Timer inHeartbeatTimer { get; set; }

    public Timer outHeartbeatTimer { get; set; }

    public WebTransportTransport(string address)
    {
      this._address = address;
    }

    public void Connect()
    {
      this._webSocket = new WebSocket(this._address, new string[0]);
      this._webSocket.OnOpen += (EventHandler) ((o, e) =>
      {
        if (this.OnOpen == null)
          return;
        this.OnOpen();
      });
      this._webSocket.OnMessage += (EventHandler<MessageEventArgs>) ((o, s) =>
      {
        if (this.inHeartbeatTimer.Enabled)
        {
          this.inHeartbeatTimer.Stop();
          this.inHeartbeatTimer.Start();
        }
        if (this.OnMessage == null)
          return;
        this.OnMessage(s.Data);
      });
      this._webSocket.Connect();
    }

    public void Send(string message)
    {
      this._webSocket.Send(message);
      if (!this.outHeartbeatTimer.Enabled)
        return;
      this.outHeartbeatTimer.Stop();
      this.outHeartbeatTimer.Start();
    }

    public void Close()
    {
      this._webSocket.Close();
    }
  }
}
