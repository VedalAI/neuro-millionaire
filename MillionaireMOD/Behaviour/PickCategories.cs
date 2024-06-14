using System.Collections;
using HarmonyLib;
using MillionaireMOD.Communication.Incoming;
using UnityEngine;

namespace MillionaireMOD.Behaviour;

[HarmonyPatch]
internal static class PickCategories
{
    [HarmonyPatch(typeof(PackSelection), nameof(PackSelection.PacksRoutine))]
    private static void Prefix(PackSelection __instance, ref IEnumerator __result)
    {
        if (Input.GetKey(KeyCode.LeftShift)) return;

        __instance.StartCoroutine(coroutine());
        return;

        IEnumerator coroutine()
        {
            for (int index = 0; index < __instance.mAllPacks.Length; index++)
            {
                SimplePackHandler pack = __instance.mAllPacks[index];
                bool packSelected = ReceiveStart.LastCategories.Contains(pack.mDataValue);

                if (pack.mSelected != packSelected)
                {
                    __instance.mSelected = index;
                    __instance.HighlightSelected();
                    yield return new WaitForSeconds(0.1f);
                    if (packSelected)
                    {
                        pack.Activate();
                        __instance.mSelected++;
                    }
                    else
                    {
                        pack.DeActivate();
                        __instance.mSelected--;
                    }
                }
            }

            __instance.Save();

            yield return new WaitForSeconds(0.5f);

            AudioDirector.PlayUISound("AK_Event_UI_Generic_Select");
            __instance.mInsideDone = true;
        }
    }
}
