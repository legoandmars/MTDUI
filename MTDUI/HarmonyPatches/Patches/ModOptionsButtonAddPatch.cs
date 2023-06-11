using flanne.TitleScreen;
using HarmonyLib;
using MTDUI.Controllers;
using MTDUI.Data;
using UnityEngine.Events;

namespace MTDUI.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(TitleMainMenuState))]
    [HarmonyPatch("Enter", MethodType.Normal)]
    internal class ModOptionsButtonAddPatch
    {
        private static void Postfix(ref TitleScreenController ___owner)
        {
            if (!ModOptionsMenuController.isTitleScreenModMenuCreated) ModOptionsMenuController.CreateTitleScreenModsMenu(___owner);
            ModOptionsMenuController.ModOptionsButton?.onClick.AddListener(new UnityAction(___owner.ChangeState<ModOptionsMenuState>));
        }
    }
}
