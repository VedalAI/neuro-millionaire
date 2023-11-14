using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MillionaireMOD;

[HarmonyPatch]
internal static class DisableCringeVoiceLines
{
    [HarmonyPatch(typeof(ScenarioLibrary), nameof(ScenarioLibrary.PlayPresenterScenario))]
    [HarmonyPrefix]
    private static bool PresenterLines(ScenarioLibrary.ePresenterAction _prAction)
    {
        if (_prAction is ScenarioLibrary.ePresenterAction.vous_avez_choisi or ScenarioLibrary.ePresenterAction.dernier_mot) return false;

        LOGGER.LogWarning("Playing presenter scenario: " + _prAction);
        return true;
    }

    private static readonly MethodInfo _scenarioLibrary_setRandom = AccessTools.Method(typeof(ScenarioLibrary), nameof(ScenarioLibrary.SetRandom),
        new[] {typeof(ScenarioLibrary.eCharacters), typeof(ScenarioLibrary.ePresenterAction), typeof(ScenarioLibrary.eCandidateAction)});

    private static readonly FieldInfo _scenarioBehaviour_mCandidateAction = AccessTools.Field(typeof(ScenarioBehavior), nameof(ScenarioBehavior.mCandidateAction));

    [HarmonyPatch(typeof(ScenarioBehavior), nameof(ScenarioBehavior.OnBehaviourPlay))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ContestantLines(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.Start();
        matcher.SearchForward(c => c.Calls(_scenarioLibrary_setRandom));
        matcher.Advance(-6);
        int insertPos = matcher.Pos;

        Label? label = null;
        matcher.SearchForward(c => c.Branches(out label));

        matcher.Advance(-999);
        matcher.Advance(insertPos + 1);

        matcher.InsertAndAdvance
        (
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, _scenarioBehaviour_mCandidateAction),
            new CodeInstruction(OpCodes.Call, new Func<ScenarioLibrary.eCandidateAction, bool>(ShouldPlay).Method),
            new CodeInstruction(OpCodes.Brfalse, label!.Value)
        );

        return matcher.InstructionEnumeration();

        static bool ShouldPlay(ScenarioLibrary.eCandidateAction _candidateAction)
        {
            if (_candidateAction is ScenarioLibrary.eCandidateAction.dernier_mot) return false;

            LOGGER.LogWarning("Playing candidate scenario: " + _candidateAction);
            return true;
        }
    }
}
