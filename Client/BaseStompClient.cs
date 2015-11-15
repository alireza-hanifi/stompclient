using Stomp;
using Stomp.Client.Helpers;
using Stomp.Client.Transport;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Stomp.Client
{
  public class BaseStompClient : IDisposable
  {
    private readonly Queue<Action> _commandQueue = new Queue<Action>();
    private readonly StompMessageSerializer serializer = new StompMessageSerializer();
    private const string _transactionPrefix = "trx-";
    private const string disconnectReceiptId = "rcpt-dis";
    private readonly IDictionary<string, Action<StompMessage>> _messageConsumers;
    private readonly ITransport transport;
    private Timer outTimer;
    private Timer inTimer;
    private readonly ISequenceNumberGenerator _receiptNumberGenerator;
    private readonly ISequenceNumberGenerator _subscriptionNumberGenerator;
    private readonly ISequenceNumberGenerator _transactionNumberGenerator;

    public Action<StompMessage> OnMessage { get; set; }

    public Action<StompMessage> OnReceipt { get; set; }

    public Action<StompMessage> OnError { get; set; }

    public Action OnDisconnect { get; set; }

    public Action<StompMessage> OnConnect { get; set; }

    public Action OnTimeout { get; set; }

    public bool IsConnected { get; private set; }

    public BaseStompClient(string address)
    {
      this.transport = (ITransport) new WebTransportTransport(address);
      this.outTimer = new Timer();
      this.inTimer = new Timer();
      this.transport.outHeartbeatTimer = this.outTimer;
      this.transport.inHeartbeatTimer = this.inTimer;
      this._messageConsumers = (IDictionary<string, Action<StompMessage>>) new Dictionary<string, Action<StompMessage>>()
      {
        {
          "MESSAGE",
          (Action<StompMessage>) (msg =>
          {
            if (this.OnMessage == null)
              return;
            this.OnMessage(msg);
          })
        },
        {
          "RECEIPT",
          (Action<StompMessage>) (msg =>
          {
            if (msg["receipt-id"].Equals("rcpt-dis"))
            {
              this.transport.Close();
              this.IsConnected = false;
              if (this.OnDisconnect == null)
                return;
              this.OnDisconnect();
            }
            else
            {
              if (this.OnReceipt == null)
                return;
              this.OnReceipt(msg);
            }
          })
        },
        {
          "ERROR",
          (Action<StompMessage>) (msg =>
          {
            if (this.OnError == null)
              return;
            this.OnError(msg);
          })
        },
        {
          "CONNECTED",
          new Action<StompMessage>(this.OnStompConnected)
        }
      };
      this._receiptNumberGenerator = (ISequenceNumberGenerator) new SequenceNumberGenerator();
      this._subscriptionNumberGenerator = (ISequenceNumberGenerator) new SequenceNumberGenerator();
      this._transactionNumberGenerator = (ISequenceNumberGenerator) new SequenceNumberGenerator();
    }

    public void Connect(string host)
    {
      this.Connect(host, (string) null, (string) null, (HeartbeatHeader) null);
    }

    public void Connect(string host, string login, string passcode, HeartbeatHeader heartbeatHeader)
    {
      StompMessage connectMessage = new StompMessage("CONNECT");
      connectMessage.Headers.Add("accept-version", "1.2");
      connectMessage.Headers.Add("host", host);
      if (heartbeatHeader != null)
        connectMessage.Headers.Add("heart-beat", heartbeatHeader.ToString());
      if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(passcode))
      {
        connectMessage.Headers.Add("login", login);
        connectMessage.Headers.Add("passcode", passcode);
      }
      this.transport.OnOpen += (Action) (() => this.transport.Send(this.serializer.Serialize(connectMessage)));
      this.transport.OnMessage += (Action<string>) (msg => this.HandleMessage(this.serializer.Deserialize(msg)));
      this.transport.Connect();
    }

    public StompMessage Disconnect()
    {
      return this.Disconnect(false);
    }

    public StompMessage Disconnect(bool useReceipt)
    {
      StompMessage msg = new StompMessage("DISCONNECT");
      if (useReceipt)
        msg["receipt"] = "rcpt-dis";
      this.ExecuteWhenConnected((Action) (() =>
      {
        this.transport.Send(this.serializer.Serialize(msg));
        if (useReceipt)
          return;
        this.transport.Close();
        this.IsConnected = false;
      }));
      return msg;
    }

    public void Execute(StompMessage msg)
    {
      this.ExecuteWhenConnected((Action) (() => this.transport.Send(this.serializer.Serialize(msg))));
    }

    private void ExecuteWhenConnected(Action command)
    {
      if (this.IsConnected)
        command();
      else
        this._commandQueue.Enqueue(command);
    }

    private void SendHeartbeat(object source, ElapsedEventArgs e)
    {
      StompMessage msg = new StompMessage();
      this.ExecuteWhenConnected((Action) (() => this.transport.Send(this.serializer.Serialize(msg))));
    }

    private void ReceiveHeartbeatTimeout(object source, ElapsedEventArgs e)
    {
      this.inTimer.Stop();
      if (this.OnTimeout == null)
        return;
      this.OnTimeout();
    }

    private void OnStompConnected(StompMessage obj)
    {
      this.IsConnected = true;
      if (obj.heartbeat != null && obj.heartbeat.Outgoing > 0)
      {
        string str = obj.Headers["heart-beat"];
        this.outTimer.Elapsed += new ElapsedEventHandler(this.SendHeartbeat);
        this.outTimer.Interval = (double) obj.heartbeat.Outgoing;
        this.outTimer.Enabled = true;
        this.outTimer.Start();
      }
      if (obj.heartbeat != null && obj.heartbeat.Incoming > 0)
      {
        string str = obj.Headers["heart-beat"];
        this.inTimer.Elapsed += new ElapsedEventHandler(this.ReceiveHeartbeatTimeout);
        this.inTimer.Interval = (double) obj.heartbeat.Incoming;
        this.inTimer.Enabled = true;
        this.inTimer.Start();
      }
      foreach (Action action in this._commandQueue)
        action();
      this._commandQueue.Clear();
      if (this.OnConnect == null)
        return;
      this.OnConnect(obj);
    }

    private void HandleMessage(StompMessage message)
    {
      if (message == null || message.Command == null || !this._messageConsumers.ContainsKey(message.Command))
        return;
      this._messageConsumers[message.Command](message);
    }

    public void Dispose()
    {
      if (this.IsConnected)
        this.Disconnect();
      this._commandQueue.Clear();
    }

    public string GetNextReceiptId()
    {
      return this._receiptNumberGenerator.Next().ToString();
    }

    public string GetNextSubscriptionId()
    {
      return this._subscriptionNumberGenerator.Next().ToString();
    }

    public string GetNextTransactionId()
    {
      return "trx-" + (object) this._transactionNumberGenerator.Next();
    }
  }
}
