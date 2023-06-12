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
            if (!ModOptionsMenuController.isPauseMenuModMenuCreated) ModOptionsMenuController.CreatePauseMenuModsMenu(___owner);
            ModOptionsMenuController.PauseModOptionsButton?.onClick.AddListener(new UnityAction(___owner.ChangeState<ModOptionsPauseState>));
        }
    }
}
