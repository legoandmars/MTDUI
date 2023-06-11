using flanne.Core;
using HarmonyLib;
using MTDUI.Controllers;
using MTDUI.Data;
using UnityEngine.Events;

namespace MTDUI.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(PauseState))]
    [HarmonyPatch("Enter", MethodType.Normal)]
    internal class ModOptionsPauseButtonAddPatch
    {
        private static void Postfix(ref GameController ___owner)
        {
            if (ModOptionsMenuController.GameController == null) ModOptionsMenuController.GameController = ___owner;

            if (!ModOptionsMenuController.isPauseMenuModMenuCreated) ModOptionsMenuController.CreatePauseMenuModsMenu();
            ModOptionsMenuController.PauseMenuAddListeners();
        }
    }
}
