using System.Threading;

namespace Stomp.Client.Helpers
{
  internal class SequenceNumberGenerator : ISequenceNumberGenerator
  {
    private int _count;

    public int Next()
    {
      return Interlocked.Increment(ref this._count);
    }
  }
}
