using System;
using System.Timers;

namespace Stomp.Client.Transport
{
  public interface ITransport
  {
    Timer inHeartbeatTimer { get; set; }

    Timer outHeartbeatTimer { get; set; }

    Action OnOpen { get; set; }

    Action<string> OnMessage { get; set; }

    void Connect();

    void Send(string message);

    void Close();
  }
}
