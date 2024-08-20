using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

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
        __result = __result.Where(i => !_blockedQuestions.Contains(i)).ToArray();
    }
}
