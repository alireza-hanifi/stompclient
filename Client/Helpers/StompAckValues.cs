using System;
using System.Collections.Generic;

namespace Stomp.Client.Helpers
{
  public static class StompAckValues
  {
    private static readonly ICollection<string> _ackValidValues = (ICollection<string>) new HashSet<string>()
    {
      "auto",
      "client-individual",
      "client"
    };
    public const string AckAutoValue = "auto";
    public const string AckClientValue = "client";
    public const string AckClientIndividualValue = "client-individual";

    public static bool IsValidAckValue(string ackValue)
    {
      return StompAckValues._ackValidValues.Contains(ackValue);
    }

    public static void ThrowIfInvalidAckValue(string ackValue)
    {
      if (!StompAckValues._ackValidValues.Contains(ackValue))
        throw new ArgumentException(string.Format("ACK header value MUST be: '{1}', '{2}' or '{3}'", (object) "auto", (object) "client-individual", (object) "client"));
    }
  }
}
