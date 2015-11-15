using System;

namespace Stomp.Listeners
{
  public class StompInMemoryListener : IStompListener
  {
    public Action<IStompClient> OnConnect { get; set; }

    public void Start()
    {
    }

    public void Stop()
    {
    }
  }
}
