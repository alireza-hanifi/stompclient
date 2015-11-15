using Stomp;
using System;

namespace Stomp.Listeners
{
  public class StompInMemoryClient : IStompClient, IComparable<IStompClient>
  {
    public Action OnClose { get; set; }

    public Guid SessionId { get; set; }

    public Action<StompMessage> OnSend { get; set; }

    public Action<StompMessage> OnMessage { get; set; }

    public void Send(StompMessage message)
    {
      if (this.OnSend == null)
        return;
      this.OnSend(message);
    }

    public void Close()
    {
    }

    public int CompareTo(IStompClient other)
    {
      return this.SessionId.CompareTo(other.SessionId);
    }

    public bool Equals(StompInMemoryClient other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      if (!object.ReferenceEquals((object) this, (object) other))
        return other.SessionId.Equals(this.SessionId);
      return true;
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj.GetType() == typeof (StompInMemoryClient))
        return this.Equals((StompInMemoryClient) obj);
      return false;
    }

    public override int GetHashCode()
    {
      return this.SessionId.GetHashCode();
    }
  }
}
