using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebSocketSharp;

namespace MillionaireMOD.Communication;

public static class WebSocketConnection
{
    private static WebSocket _socket;

    public static event Action<string> OnAnswerReceived;
    public static event Action<string> OnLifelineReceived;
    public static event Action<int, int, int, int> OnAskTheAudienceResultsReceived;
    public static event Action<string> OnPhoneAFriendResultsReceived;

    public static void Initialize()
    {
        LOGGER.LogWarning("Initializing WebSocket connection");
        _socket = new WebSocket("ws://localhost:8000");
        _socket.OnOpen += (_, _) => _socket.Send(new WSMessage("client"));
        _socket.OnMessage += (_, msg) => ReceiveMessage(msg);
        _socket.Connect();
    }

    public static void Send(WSMessage wsMessage)
    {
        _socket.Send(wsMessage);
    }

    private static void ReceiveMessage(MessageEventArgs msg)
    {
        try
        {
            LOGGER.LogWarning("Received message " + msg.Data);

            WSMessage wsMessage = JsonConvert.DeserializeObject<WSMessage>(msg.Data);
            switch (wsMessage.command)
            {
                case "millionaire/answer":
                    OnAnswerReceived?.Invoke(wsMessage.data["answer"].ToString());
                    break;

                case "millionaire/lifeline":
                    OnLifelineReceived?.Invoke(wsMessage.data["lifeline"].ToString());
                    break;

                case "millionaire/lifeline/ask_the_audience/results":
                    OnAskTheAudienceResultsReceived?.Invoke(
                        Convert.ToInt32(wsMessage.data["percentageA"]), Convert.ToInt32(wsMessage.data["percentageB"]),
                        Convert.ToInt32(wsMessage.data["percentageC"]), Convert.ToInt32(wsMessage.data["percentageD"])
                    );
                    break;

                case "millionaire/lifeline/phone_a_friend/results":
                    OnPhoneAFriendResultsReceived?.Invoke(wsMessage.data["result"].ToString());
                    break;
            }
        }
        catch (Exception e)
        {
            LOGGER.LogError("Received invalid message");
            LOGGER.LogError(e);
        }
    }
}


public record WSMessage(string command, Dictionary<string, object> data = null)
{
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static implicit operator string(WSMessage wsMessage)
    {
        return wsMessage.ToString();
    }
}
