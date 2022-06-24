using flanne.Core;
using HarmonyLib;
using MTDUI.Controllers;
using MTDUI.Data;
using UnityEngine.Events;

namespace MTDUI.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(PauseState))]
    [HarmonyPatch("Exit", MethodType.Normal)]
    internal class ModOptionsPauseButtonRemovePatch
    {
        private static void Postfix(PauseState __instance)
        {
            if (ModOptionsMenuController.GameController == null) ModOptionsMenuController.GameController = Traverse.Create(__instance).Field("owner").GetValue() as GameController;

            ModOptionsMenuController.CreateModOptionsButton(OptionsMenuType.PauseMenu);
            ModOptionsMenuController.CreateModOptionsPanel(OptionsMenuType.PauseMenu);

            ModOptionsMenuController.PauseModOptionsButton?.onClick.AddListener(new UnityAction(ModOptionsMenuController.OnPauseModOptionsClick));
        }
    }
}
