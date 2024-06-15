using System;
using System.Collections;
using UnityEngine;

namespace MillionaireMOD.Communication.Incoming;

internal static class ReceiveLifeline
{
    public static void UseLifeline(string lifelineStr)
    {
        int num = lifelineStr switch
        {
            "50_50" => 0,
            "phone_a_friend" => 1,
            "ask_the_audience" => 2,
            "flip_the_question" => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(lifelineStr), lifelineStr, null)
        };

        AnswerStep answer = GameObject.FindObjectOfType<AnswerStep>();
        if (!answer) return;

        if (answer.mUIController.mLifelines[num].mUsed.enabled) return;

        answer.StartCoroutine(coroutine());

        return;

        IEnumerator coroutine()
        {
            while (SoloGameplay.sInstance.mGameState != eGameState.ANSWER) yield return null;

            answer.mUIController.mCurrentLifelineHighlighted = num;
            answer.mUIController.LifelineSelected();
            answer.mLifelineSelected = true;
        }
    }
}
