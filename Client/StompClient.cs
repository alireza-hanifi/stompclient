using Stomp;
using Stomp.Client.Helpers;

namespace Stomp.Client
{
  public class StompClient
  {
    private BaseStompClient _client;

    protected Transaction Transaction { get; set; }

    public StompClient(BaseStompClient client)
    {
      this._client = client;
    }

    public StompMessage Send(string address)
    {
      return this.Send(address, "");
    }

    public StompMessage Send(string address, string message)
    {
      return this.Send(address, message, "-1");
    }

    public StompMessage Send(string address, string message, string receiptId)
    {
      if (!address.StartsWith("/"))
        address = string.Format("/{0}", (object) address);
      StompMessage msg = new StompMessage("SEND", message);
      msg.Transaction = this.Transaction;
      msg["destination"] = address;
      msg["content-type"] = "text/plain";
      if (!string.IsNullOrEmpty(receiptId) && receiptId != "-1")
        msg["receipt"] = receiptId;
      else if (receiptId != "-1")
        msg["receipt"] = this._client.GetNextReceiptId();
      this._client.Execute(msg);
      return msg;
    }

    public StompMessage Subscribe(string destination)
    {
      return this.Subscribe(destination, "-1", (string) null);
    }

    public StompMessage Subscribe(string destination, string receiptId)
    {
      return this.Subscribe(destination, receiptId, (string) null);
    }

    public StompMessage Subscribe(string destination, string receiptId, string ackHeader)
    {
      StompMessage msg = new StompMessage("SUBSCRIBE");
      msg["id"] = this._client.GetNextSubscriptionId();
      msg["destination"] = destination;
      if (!string.IsNullOrEmpty(ackHeader) && StompAckValues.IsValidAckValue(ackHeader))
        msg["ack"] = ackHeader;
      if (!string.IsNullOrEmpty(receiptId) && receiptId != "-1")
        msg["receipt"] = receiptId;
      else if (receiptId != "-1")
        msg["receipt"] = this._client.GetNextReceiptId();
      this._client.Execute(msg);
      return msg;
    }

    public StompMessage Acknowledge(string messageId)
    {
      return this.Acknowledge(messageId, "-1");
    }

    public StompMessage Acknowledge(string messageId, string receiptId)
    {
      StompMessage msg = new StompMessage("ACK");
      msg["id"] = messageId;
      msg.Transaction = this.Transaction;
      if (!string.IsNullOrEmpty(receiptId) && receiptId != "-1")
        msg["receipt"] = receiptId;
      else if (receiptId != "-1")
        msg["receipt"] = this._client.GetNextReceiptId();
      this._client.Execute(msg);
      return msg;
    }

    public StompMessage AcknowledgeNegative(string messageId)
    {
      return this.AcknowledgeNegative(messageId, "-1");
    }

    public StompMessage AcknowledgeNegative(string messageId, string receiptId)
    {
      StompMessage msg = new StompMessage("NACK");
      msg["id"] = messageId;
      msg.Transaction = this.Transaction;
      if (!string.IsNullOrEmpty(receiptId) && receiptId != "-1")
        msg["receipt"] = receiptId;
      else if (receiptId != "-1")
        msg["receipt"] = this._client.GetNextReceiptId();
      this._client.Execute(msg);
      return msg;
    }

    public StompMessage Unsubscribe(string subscribeId)
    {
      return this.Unsubscribe(subscribeId, "-1");
    }

    public StompMessage Unsubscribe(string subscribeId, string receiptId)
    {
      StompMessage msg = new StompMessage("UNSUBSCRIBE");
      if (!string.IsNullOrEmpty(receiptId) && receiptId != "-1")
        msg["receipt"] = receiptId;
      else if (receiptId != "-1")
        msg["receipt"] = this._client.GetNextReceiptId();
      msg["id"] = subscribeId;
      this._client.Execute(msg);
      return msg;
    }
  }
}
