using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection.Emit;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WebSocketSharp;

namespace MillionaireMOD.Communication;

[HarmonyPatch]
public class WebSocketConnection : MonoBehaviour
{
    private static WebSocket _socket;

    private static string _socketUrl = "";
    private static string _clientId = "";
    private static string _clientSecret = "";

    public static event Action<string> OnLanguageReceived;
    public static event Action<string, string[]> OnStartReceived;
    public static event Action<bool> OnCharacterResponseReceived;
    public static event Action<string> OnAnswerReceived;
    public static event Action<string> OnLifelineReceived;
    public static event Action<int, int, int, int> OnAskTheAudienceResultsReceived;
    public static event Action<string> OnPhoneAFriendResultsReceived;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
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
            _socketUrl = splits[0];
            _clientId = splits[1];
            _clientSecret = splits[2];
        }
        catch (Exception e)
        {
            LOGGER.LogError(e);
        }

        StartWs();
    }

    private IEnumerator Reconnect()
    {
        yield return new WaitForSeconds(3);
        StartWs();
    }

    private void StartWs()
    {
        try
        {
            if (_socket?.ReadyState is WebSocketState.Open or WebSocketState.Connecting) _socket?.Close();
        }
        catch
        {
            // ignored
        }

        _socket = new WebSocket(_socketUrl);
        _socket.OnOpen += (_, _) => _socket.Send(new WsMessage("client"));
        _socket.OnMessage += (_, msg) => ReceiveMessage(msg);
        _socket.OnError += (_, e) =>
        {
            LOGGER.LogError("Websocket connection has encountered an error!");
            LOGGER.LogError(e.Message);
            LOGGER.LogError(e.Exception);
            StartCoroutine(Reconnect());
        };
        _socket.OnClose += (_, _) =>
        {
            LOGGER.LogError("Websocket connection has been closed!");
            StartCoroutine(Reconnect());
        };
        _socket.Connect();
    }

    public static void Send(WsMessage wsMessage)
    {
        _socket.Send(wsMessage);
    }

    private static void ReceiveMessage(MessageEventArgs msg)
    {
        try
        {
            LOGGER.LogWarning("Received message " + msg.Data);

            WsMessage wsMessage = JsonConvert.DeserializeObject<WsMessage>(msg.Data);
            switch (wsMessage.Command)
            {
                case "language":
                    OnLanguageReceived?.Invoke(wsMessage.Data["language"].ToString());
                    break;

                case "start":
                    JArray categories = (JArray) wsMessage.Data["categories"];
                    OnStartReceived?.Invoke(wsMessage.Data["difficulty"].ToString(), categories.ToObject<string[]>());
                    break;

                case "character/response":
                    OnCharacterResponseReceived?.Invoke((bool) wsMessage.Data["accept"]);
                    break;

                case "answer":
                    OnAnswerReceived?.Invoke(wsMessage.Data["answer"].ToString());
                    break;

                case "lifeline":
                    OnLifelineReceived?.Invoke(wsMessage.Data["lifeline"].ToString());
                    break;

                case "lifeline/ask_the_audience/results":
                    OnAskTheAudienceResultsReceived?.Invoke(
                        Convert.ToInt32(wsMessage.Data["percentageA"]), Convert.ToInt32(wsMessage.Data["percentageB"]),
                        Convert.ToInt32(wsMessage.Data["percentageC"]), Convert.ToInt32(wsMessage.Data["percentageD"])
                    );
                    break;

                case "lifeline/phone_a_friend/results":
                    OnPhoneAFriendResultsReceived?.Invoke(wsMessage.Data["result"].ToString());
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

public record struct WsMessage(string Command, Dictionary<string, object> Data = null)
{
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static implicit operator string(WsMessage wsMessage)
    {
        return wsMessage.ToString();
    }
}
