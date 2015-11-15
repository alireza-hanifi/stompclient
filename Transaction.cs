using Stomp.Client;

namespace Stomp
{
  public class Transaction : StompClient
  {
    private BaseStompClient _Client;

    public string TransactionId { get; private set; }

    public Transaction(BaseStompClient client)
      : base(client)
    {
      this._Client = client;
      this.TransactionId = this._Client.GetNextTransactionId();
      this.Transaction = this;
    }

    public Transaction(BaseStompClient client, string TransactionId)
      : base(client)
    {
      this._Client = client;
      this.TransactionId = TransactionId;
      this.Transaction = this;
    }

    public StompMessage Begin()
    {
      return this.Begin("-1");
    }

    public StompMessage Begin(string receiptId)
    {
      StompMessage msg = new StompMessage("BEGIN");
      msg["transaction"] = this.TransactionId;
      if (!string.IsNullOrEmpty(receiptId) && receiptId != "-1")
        msg["receipt"] = receiptId;
      else if (receiptId != "-1")
        msg["receipt"] = this._Client.GetNextReceiptId();
      this._Client.Execute(msg);
      return msg;
    }

    public StompMessage Commit()
    {
      return this.Commit("-1");
    }

    public StompMessage Commit(string receiptId)
    {
      StompMessage msg = new StompMessage("COMMIT");
      msg["transaction"] = this.TransactionId;
      if (!string.IsNullOrEmpty(receiptId) && receiptId != "-1")
        msg["receipt"] = receiptId;
      else if (receiptId != "-1")
        msg["receipt"] = this._Client.GetNextReceiptId();
      this._Client.Execute(msg);
      return msg;
    }

    public StompMessage Abort()
    {
      return this.Abort("-1");
    }

    public StompMessage Abort(string receiptId)
    {
      StompMessage msg = new StompMessage("ABORT");
      msg["transaction"] = this.TransactionId;
      if (!string.IsNullOrEmpty(receiptId) && receiptId != "-1")
        msg["receipt"] = receiptId;
      else if (receiptId != "-1")
        msg["receipt"] = this._Client.GetNextReceiptId();
      this._Client.Execute(msg);
      return msg;
    }
  }
}
