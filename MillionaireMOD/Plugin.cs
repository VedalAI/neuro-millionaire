global using static MillionaireMOD.Plugin;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using MillionaireMOD.Communication;
using MillionaireMOD.Communication.Incoming;
using MillionaireMOD.Resources;
using UnityEngine;

namespace MillionaireMOD;

[BepInPlugin("MillionaireMOD", "MillionaireMOD", "1.0.0")]
public sealed class Plugin : BaseUnityPlugin
{
    // ReSharper disable once InconsistentNaming
    public static ManualLogSource LOGGER { get; private set; }
    // ReSharper disable once InconsistentNaming
    public static AudioClip LONGER_AUDIENCE_CLIP { get; private set; }

    private void Awake()
    {
        LOGGER = Logger;

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        gameObject.AddComponent<WebSocketConnection>();
        WebSocketConnection.OnLanguageReceived += ReceiveLanguage.SetLanguage;
        WebSocketConnection.OnStartReceived += ReceiveStart.StartGame;
        WebSocketConnection.OnCharacterResponseReceived += ReceiveCharacterResponse.ReceiveResponse;
        WebSocketConnection.OnAnswerReceived += ReceiveAnswer.AnswerQuestion;
        WebSocketConnection.OnLifelineReceived += ReceiveLifeline.UseLifeline;
        WebSocketConnection.OnAskTheAudienceResultsReceived += ReceiveAskTheAudienceResults.UpdateUI;
        WebSocketConnection.OnPhoneAFriendResultsReceived += ReceivePhoneAFriendResults.EndCall;

        LONGER_AUDIENCE_CLIP = ResourceManager.GetAssetBundle("assets").LoadAsset<AudioClip>("longer audience sound");
    }
}

/*
[HarmonyPatch(typeof(UIController), nameof(UIController.Update))]
public static class QuestionDump
{
    [HarmonyPostfix]
    public static void Postfix(UIController __instance)
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            for (int i = 0; i <= 1; i++)
            {
                AllQuestion pack = __instance.mQuestionsLibrary.mAllQuestion[i];

                File.WriteAllText($@"C:\Users\alexe\Desktop\Questions_{i}.txt", JsonConvert.SerializeObject(pack, Formatting.Indented));
            }
        }
    }
}
*/
