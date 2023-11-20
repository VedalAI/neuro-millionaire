global using static MillionaireMOD.Plugin;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using MillionaireMOD.Communication;
using MillionaireMOD.Resources;
using UnityEngine;

namespace MillionaireMOD;

[BepInPlugin("MillionaireMOD", "MillionaireMOD", "1.0.0")]
public sealed class Plugin : BaseUnityPlugin
{
    public static ManualLogSource LOGGER { get; private set; }
    public static AudioClip LONGER_AUDIENCE_CLIP { get; private set; }

    private void Awake()
    {
        LOGGER = Logger;

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        gameObject.AddComponent<WebSocketConnection>();
        WebSocketConnection.OnAnswerReceived += ReceiveAnswer.AnswerQuestion;
        WebSocketConnection.OnLifelineReceived += ReceiveLifeline.UseLifeline;
        WebSocketConnection.OnAskTheAudienceResultsReceived += ReceiveAskTheAudienceResults.UpdateUI;
        WebSocketConnection.OnPhoneAFriendResultsReceived += ReceivePhoneAFriendResults.EndCall;

        LONGER_AUDIENCE_CLIP = ResourceManager.GetAssetBundle("assets").LoadAsset<AudioClip>("longer audience sound");
    }
}
