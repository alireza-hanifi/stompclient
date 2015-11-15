using Stomp.Client.Helpers;
using System.Collections.Generic;

namespace Stomp
{
  public class StompMessage
  {
    private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

    public Dictionary<string, string> Headers
    {
      get
      {
        return this._headers;
      }
    }

    public string Body { get; private set; }

    public bool UseReceipt { get; set; }

    public string ReceiptId { get; set; }

    public Transaction Transaction { get; set; }

    public HeartbeatHeader heartbeat { get; private set; }

    public string Command { get; private set; }

    public string this[string header]
    {
      get
      {
        if (!this._headers.ContainsKey(header))
          return string.Empty;
        return this._headers[header];
      }
      set
      {
        this._headers[header] = value;
      }
    }

    public StompMessage()
      : this(string.Empty, string.Empty)
    {
    }

    public StompMessage(string command)
      : this(command, string.Empty)
    {
    }

    public StompMessage(string command, string body)
      : this(command, body, new Dictionary<string, string>())
    {
    }

    internal StompMessage(string command, string body, Dictionary<string, string> headers)
    {
      this.Command = command;
      this.Body = body;
      this._headers = headers;
      if (!string.IsNullOrEmpty(body))
        this["content-length"] = body.Length.ToString();
      if (string.IsNullOrEmpty(this["heart-beat"]))
        return;
      string[] strArray = this["heart-beat"].Split(',');
      int result1;
      int result2;
      if (!int.TryParse(strArray[0], out result1) || result1 <= 0 || (int.TryParse(strArray[1], out result2) || result2 <= 0))
        return;
      this.heartbeat = new HeartbeatHeader(result1, result2);
    }
  }
}
