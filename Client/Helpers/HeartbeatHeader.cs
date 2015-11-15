namespace Stomp.Client.Helpers
{
  public class HeartbeatHeader
  {
    public int Outgoing { get; private set; }

    public int Incoming { get; private set; }

    public HeartbeatHeader(int outgoing, int incoming)
    {
      this.Outgoing = outgoing;
      this.Incoming = incoming;
    }

    public override string ToString()
    {
      return (string) (object) this.Outgoing + (object) ":" + (string) (object) this.Incoming;
    }
  }
}
