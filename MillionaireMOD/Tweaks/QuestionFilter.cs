using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class QuestionFilter
{
    private static readonly HashSet<int> _blockedQuestions = [ 37081, 400789 ];

    [HarmonyPatch(typeof(QuestionsLibrary), nameof(QuestionsLibrary.GenerateChildQuestionsPool))]
    [HarmonyPatch(typeof(QuestionsLibrary), nameof(QuestionsLibrary.GenerateQuestionsPool))]
    [HarmonyPatch(typeof(QuestionsLibrary), nameof(QuestionsLibrary.GenerateQuestionsPoolOnline))]
    [HarmonyPostfix]
    public static void FilterQuestions(QuestionsLibrary __instance, ref int[] __result)
    {
        int length = __result.Length;
        __result = __result.Where(i => !_blockedQuestions.Contains(i)).ToArray();
        if (length != __result.Length)
            LOGGER.LogFatal("QUESTION COUNT CHANGED: " + length + " -> " + __result.Length);
        else
            LOGGER.LogMessage("Question pool unchanged: length " + length);
        LOGGER.LogDebug(string.Join(" ", __result));
    }
}
