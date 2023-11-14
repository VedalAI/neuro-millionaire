using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MillionaireMOD;

[HarmonyPatch]
public static class PreventAudienceResults
{
    private static readonly MethodInfo _uiController_StartPublicCilinder = AccessTools.Method(typeof(UIController), nameof(UIController.StartPublicCilinder));
    private static readonly MethodInfo _uiController_StartStopPublicParticleAnim = AccessTools.Method(typeof(UIController), nameof(UIController.StartStopPublicParticleAnim));

    [HarmonyPatch(typeof(CtrlUIBehavior), nameof(CtrlUIBehavior.OnBehaviourPlay))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.Start();

        matcher.SearchForward(c => c.Calls(_uiController_StartStopPublicParticleAnim));
        matcher.Advance(1);
        matcher.SearchForward(c => c.Calls(_uiController_StartStopPublicParticleAnim));
        matcher.Set(OpCodes.Nop, null);
        matcher.Advance(-1);
        matcher.Set(OpCodes.Nop, null);
        matcher.Advance(-1);
        matcher.Set(OpCodes.Nop, null);

        matcher.SearchForward(c => c.Calls(_uiController_StartPublicCilinder));
        matcher.Set(OpCodes.Nop, null);
        matcher.Advance(-1);
        matcher.Set(OpCodes.Nop, null);

        return matcher.InstructionEnumeration();
    }
}
