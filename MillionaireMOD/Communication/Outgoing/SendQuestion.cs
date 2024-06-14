using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
internal static class SendQuestion
{
    public static string LastSentQuestion { get; set; }

    [HarmonyPatch(typeof(UIController), nameof(UIController.StartQuestionRoutine))]
    [HarmonyPatch(typeof(UIController), nameof(UIController.ShowAllQuestionAnswer))]
    [HarmonyPostfix]
    private static void Patch(UIController __instance)
    {
        if (LastSentQuestion == __instance.mQuestion.text) return;
        LastSentQuestion = __instance.mQuestion.text;

        List<string> lifelines = [];
        AnswerStep answer = GameObject.FindObjectOfType<AnswerStep>();
        if (!answer)
        {
            LOGGER.LogError("Uh oh! No AnswerStep");
        }
        else
        {
            if (!answer.mUIController.mLifelines[0].mUsed.enabled) lifelines.Add("50_50");
            if (!answer.mUIController.mLifelines[1].mUsed.enabled) lifelines.Add("phone_a_friend");
            if (!answer.mUIController.mLifelines[2].mUsed.enabled) lifelines.Add("ask_the_audience");
            if (!answer.mUIController.mLifelines[3].mUsed.enabled) lifelines.Add("flip_the_question");
        }

        Dictionary<string, object> data = new()
        {
            {"question", __instance.mQuestion.text},
            {"answerA", __instance.mAnswers[0].mAnswer.text},
            {"answerB", __instance.mAnswers[1].mAnswer.text},
            {"answerC", __instance.mAnswers[2].mAnswer.text},
            {"answerD", __instance.mAnswers[3].mAnswer.text},
            {"lifelines", lifelines}
        };

        WebSocketConnection.Send(new WsMessage("question", data));
    }

    [HarmonyPatch(typeof(UIController), nameof(UIController.HideFiftyFifty))]
    [HarmonyPostfix]
    private static void FiftyFiftyPatch(UIController __instance)
    {
        List<string> lifelines = [];
        AnswerStep answer = GameObject.FindObjectOfType<AnswerStep>();
        if (!answer)
        {
            LOGGER.LogError("Uh oh! No AnswerStep");
        }
        else
        {
            if (!answer.mUIController.mLifelines[1].mUsed.enabled) lifelines.Add("phone_a_friend");
            if (!answer.mUIController.mLifelines[2].mUsed.enabled) lifelines.Add("ask_the_audience");
            if (!answer.mUIController.mLifelines[3].mUsed.enabled) lifelines.Add("flip_the_question");
        }

        Dictionary<string, object> data = new()
        {
            {"question", __instance.mQuestion.text},
            {"lifelines", lifelines}
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

    [HarmonyPatch(typeof(UIController), nameof(UIController.Update))]
    [HarmonyPrefix]
    private static void ResendQuestion(UIController __instance)
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;

        List<string> lifelines = [];
        AnswerStep answer = GameObject.FindObjectOfType<AnswerStep>();
        if (!answer)
        {
            LOGGER.LogError("Uh oh! No AnswerStep");
        }
        else
        {
            if (!answer.mUIController.mLifelines[0].mUsed.enabled) lifelines.Add("50_50");
            if (!answer.mUIController.mLifelines[1].mUsed.enabled) lifelines.Add("phone_a_friend");
            if (!answer.mUIController.mLifelines[2].mUsed.enabled) lifelines.Add("ask_the_audience");
            if (!answer.mUIController.mLifelines[3].mUsed.enabled) lifelines.Add("flip_the_question");
        }

        Dictionary<string, object> data = new()
        {
            {"question", __instance.mQuestion.text},
            {"lifelines", lifelines}
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
