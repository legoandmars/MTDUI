using flanne.Core;
using MTDUI.Controllers;
using UnityEngine;

namespace MTDUI.Data
{
    public class ModOptionsPauseSubmenuState : GameState
    {
        public static string CurrentSubmenu = "";
        private static RectTransform? rect;
        public void OnClick()
        {
            owner.ChangeState<ModOptionsPauseState>();
        }

        public override void Enter()
        {
            if (ModOptionsMenuController.PauseModOptionsSubmenuMenu == null || ModOptionsMenuController.PauseModOptionsSubMenuBackButton == null)
            {
                Debug.LogError("No panel present");
                return;
            }

            if (rect == null) rect = ModOptionsMenuController.PauseModOptionsSubmenuMenu.GetComponent<RectTransform>();

            // Rescaling based on number of entry to show
            var entryCount = 1;
            foreach (var button in ModOptionsMenuController.PauseMenuModOptionComponents)
            {
                button.gameObject.SetActive(button.Mod == CurrentSubmenu);
                if (button.Mod == CurrentSubmenu) entryCount++;
            }

            var buttonSize = ModOptionsMenuController.PauseModOptionsSubMenuBackButton.GetComponent<RectTransform>().sizeDelta.y;
            var verticalPadding = 40;
            rect.sizeDelta = new Vector2(300, verticalPadding + buttonSize * entryCount);

            var buttonListRect = ModOptionsMenuController.PauseModOptionsSubmenuMenu.transform.GetChild(0).GetComponent<RectTransform>();
            buttonListRect.position = Vector3.zero;
            buttonListRect.sizeDelta = new Vector2(100, entryCount * buttonSize);

            ModOptionsMenuController.PauseModOptionsSubMenuBackButton?.onClick.AddListener(OnClick);
            ModOptionsMenuController.PauseModOptionsSubmenuMenu?.Show();
        }

        public override void Exit()
        {
            ModOptionsMenuController.PauseModOptionsSubMenuBackButton?.onClick.RemoveListener(OnClick);
            ModOptionsMenuController.PauseModOptionsSubmenuMenu?.Hide();
        }
    }
}
