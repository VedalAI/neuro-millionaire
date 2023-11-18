using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection.Emit;
using HarmonyLib;
using Newtonsoft.Json;
using WebSocketSharp;

namespace MillionaireMOD.Communication;

[HarmonyPatch]
public static class WebSocketConnection
{
    private static WebSocket _socket;

    private static string _clientId = "";
    private static string _clientSecret = "";

    public static event Action<string> OnAnswerReceived;
    public static event Action<string> OnLifelineReceived;
    public static event Action<int, int, int, int> OnAskTheAudienceResultsReceived;
    public static event Action<string> OnPhoneAFriendResultsReceived;

    public static void Initialize()
    {
        LOGGER.LogWarning("Initializing WebSocket connection");

        string path = Path.GetFullPath("socket-auth.txt");
        if (!File.Exists(path))
        {
            LOGGER.LogWarning($"File {path} is missing");
        }

        try
        {
            string[] splits = File.ReadAllLines(path);
            _clientId = splits[0];
            _clientSecret = splits[1];
        }
        catch (Exception e)
        {
            LOGGER.LogError(e);
        }

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

    [HarmonyPatch(typeof(WebSocket), nameof(WebSocket.createHandshakeRequest))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> AddCustomHeaders(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.Start();

        matcher.SearchForward(c => c.Is(OpCodes.Ldstr, "Sec-WebSocket-Version"));
        matcher.Advance(4);

        matcher.InsertAndAdvance
        (
            new CodeInstruction(OpCodes.Ldloc_1), // headers
            new CodeInstruction(OpCodes.Call, new Action<NameValueCollection>(addCustomHeaders).Method)
        );

        return matcher.InstructionEnumeration();

        static void addCustomHeaders(NameValueCollection headers)
        {
            headers["CF-Access-Client-Id"] = _clientId;
            headers["CF-Access-Client-Secret"] = _clientSecret;
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
