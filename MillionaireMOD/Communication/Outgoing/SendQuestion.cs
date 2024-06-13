using System.Collections.Generic;
using HarmonyLib;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
internal static class SendQuestion
{
    private static string _lastQuestion;

    [HarmonyPatch(typeof(UIController), nameof(UIController.StartQuestionRoutine))]
    [HarmonyPatch(typeof(UIController), nameof(UIController.ShowAllQuestionAnswer))]
    [HarmonyPostfix]
    private static void Patch(UIController __instance)
    {
        if (_lastQuestion == __instance.mQuestion.text) return;
        _lastQuestion = __instance.mQuestion.text;

        Dictionary<string, object> data = new()
        {
            {"question", __instance.mQuestion.text},
            {"answerA", __instance.mAnswers[0].mAnswer.text},
            {"answerB", __instance.mAnswers[1].mAnswer.text},
            {"answerC", __instance.mAnswers[2].mAnswer.text},
            {"answerD", __instance.mAnswers[3].mAnswer.text},
        };

        WebSocketConnection.Send(new WsMessage("question", data));
    }

    [HarmonyPatch(typeof(UIController), nameof(UIController.HideFiftyFifty))]
    [HarmonyPostfix]
    private static void FiftyFiftyPatch(UIController __instance)
    {
        Dictionary<string, object> data = new()
        {
            {"question", __instance.mQuestion.text},
        };

        for (int i = 0; i < 4; i++)
        {
            if (__instance.mAnswers[i].mAnswer.color.a == 0) continue;

            data[i switch
            {
                0 => "answerA",
                1 => "answerB",
                2 => "answerC",
                _ => "answerD",
            }] = __instance.mAnswers[i].mAnswer.text;
        }

        WebSocketConnection.Send(new WsMessage("question", data));
    }
}
