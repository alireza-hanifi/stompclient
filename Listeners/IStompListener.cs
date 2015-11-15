using System;

namespace Stomp.Listeners
{
  public interface IStompListener
  {
    Action<IStompClient> OnConnect { get; set; }

    void Start();

    void Stop();
  }
}
