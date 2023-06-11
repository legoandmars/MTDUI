using flanne.Core;
using MTDUI.Controllers;
using UnityEngine;

namespace MTDUI.Data
{
    public class ModOptionsPauseSubmenuState : GameState
    {
        public static string CurrentSubmenu = "";
        private static RectTransform? rect;

        public override void Enter()
        {
            if (ModOptionsMenuController.PauseSubmenu == null || ModOptionsMenuController.PauseSubmenuBackButton == null)
            {
                Debug.LogError("No panel present");
                return;
            }

            if (rect == null) rect = ModOptionsMenuController.PauseSubmenu.GetComponent<RectTransform>();

            // Rescaling based on number of entry to show
            var entryCount = 1;
            foreach (var button in ModOptionsMenuController.PauseMenuModOptionComponents)
            {
                button.gameObject.SetActive(button.Mod == CurrentSubmenu);
                if (button.Mod == CurrentSubmenu) entryCount++;
            }

            var buttonSize = ModOptionsMenuController.PauseSubmenuBackButton.GetComponent<RectTransform>().sizeDelta.y;
            var verticalPadding = 40;
            rect.sizeDelta = new Vector2(300, verticalPadding + buttonSize * entryCount);

            var buttonListRect = ModOptionsMenuController.PauseSubmenu.transform.GetChild(0).GetComponent<RectTransform>();
            buttonListRect.position = Vector3.zero;
            buttonListRect.sizeDelta = new Vector2(100, entryCount * buttonSize);

            ModOptionsMenuController.PauseSubmenuBackButton?.onClick.AddListener(owner.ChangeState<ModOptionsPauseState>);
            ModOptionsMenuController.PauseSubmenu?.Show();
        }

        public override void Exit()
        {
            ModOptionsMenuController.PauseSubmenuBackButton?.onClick.RemoveAllListeners();
            ModOptionsMenuController.PauseSubmenu?.Hide();
        }
    }
}
