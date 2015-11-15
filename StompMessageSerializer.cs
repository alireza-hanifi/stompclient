using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stomp
{
  public class StompMessageSerializer
  {
    public string Serialize(StompMessage message)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(message.Command + (object) '\n');
      if (message.Transaction != null)
        message["transaction"] = message.Transaction.TransactionId;
      if (message.Headers != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in message.Headers)
          stringBuilder.Append(string.Concat(new object[4]
          {
            (object) keyValuePair.Key,
            (object) ':',
            (object) keyValuePair.Value,
            (object) '\n'
          }));
      }
      stringBuilder.Append('\n');
      stringBuilder.Append(message.Body);
      stringBuilder.Append(char.MinValue);
      return stringBuilder.ToString();
    }

    public StompMessage Deserialize(string message)
    {
      StringReader stringReader = new StringReader(message);
      string command = stringReader.ReadLine();
      Dictionary<string, string> headers = new Dictionary<string, string>();
      string str = stringReader.ReadLine();
      try
      {
        for (; !string.IsNullOrEmpty(str); str = stringReader.ReadLine() ?? string.Empty)
        {
          string[] strArray = str.Split(':');
          if (strArray.Length == 2 && !headers.ContainsKey(strArray[0].Trim()))
            headers[strArray[0].Trim()] = strArray[1].Trim();
        }
      }
      catch (Exception ex)
      {
        ex.ToString();
      }
      string body = (stringReader.ReadToEnd() ?? string.Empty).TrimEnd('\r', '\n', char.MinValue);
      return new StompMessage(command, body, headers);
    }
  }
}
