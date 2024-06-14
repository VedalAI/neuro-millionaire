using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MillionaireMOD.Behaviour;

[HarmonyPatch]
internal static class PreventAudienceResults
{
    private static readonly MethodInfo _uiControllerStartPublicCilinder = AccessTools.Method(typeof(UIController), nameof(UIController.StartPublicCilinder));
    private static readonly MethodInfo _uiControllerStartStopPublicParticleAnim = AccessTools.Method(typeof(UIController), nameof(UIController.StartStopPublicParticleAnim));

    [HarmonyPatch(typeof(CtrlUIBehavior), nameof(CtrlUIBehavior.OnBehaviourPlay))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.Start();

        matcher.SearchForward(c => c.Calls(_uiControllerStartStopPublicParticleAnim));
        matcher.Advance(1);
        matcher.SearchForward(c => c.Calls(_uiControllerStartStopPublicParticleAnim));
        matcher.Set(OpCodes.Nop, null);
        matcher.Advance(-1);
        matcher.Set(OpCodes.Nop, null);
        matcher.Advance(-1);
        matcher.Set(OpCodes.Nop, null);

        matcher.SearchForward(c => c.Calls(_uiControllerStartPublicCilinder));
        matcher.Set(OpCodes.Nop, null);
        matcher.Advance(-1);
        matcher.Set(OpCodes.Nop, null);

        return matcher.InstructionEnumeration();
    }
}
