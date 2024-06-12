using System;
using UnityEngine;

namespace MillionaireMOD.Communication.Incoming;

public static class ReceiveAnswer
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

        answer.mUIController.mCurrentAnswerHighlighted = num;
        answer.lastHighlightAnswer = num;
        answer.mPreselectedAnswer = true;
        answer.mUIController.PlayerAnswer();
        answer.mAnswerSelected = true;
    }
}
