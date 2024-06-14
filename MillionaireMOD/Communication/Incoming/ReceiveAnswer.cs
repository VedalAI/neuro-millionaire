using System;
using System.Collections;
using UnityEngine;

namespace MillionaireMOD.Communication.Incoming;

internal static class ReceiveAnswer
{
    public static void AnswerQuestion(string answerStr)
    {
        int num = answerStr switch
        {
            "A" => 0,
            "B" => 1,
            "C" => 2,
            "D" => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(answerStr), answerStr, null)
        };

        AnswerStep answer = GameObject.FindObjectOfType<AnswerStep>();
        if (!answer) return;

        answer.StartCoroutine(coroutine());

        return;

        IEnumerator coroutine()
        {
            while (SoloGameplay.sInstance.mGameState != eGameState.ANSWER) yield return null;

            answer.mUIController.mCurrentAnswerHighlighted = num;
            answer.lastHighlightAnswer = num;
            answer.mPreselectedAnswer = true;
            answer.mUIController.PlayerAnswer();
            answer.mAnswerSelected = true;
        }
    }
}
