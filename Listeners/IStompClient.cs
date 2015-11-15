using Stomp;
using System;

namespace Stomp.Listeners
{
  public interface IStompClient : IComparable<IStompClient>
  {
    Action OnClose { get; set; }

    Guid SessionId { get; set; }

    Action<StompMessage> OnMessage { get; set; }

    void Send(StompMessage message);

    void Close();
  }
}
