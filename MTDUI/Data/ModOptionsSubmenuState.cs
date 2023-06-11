using flanne.TitleScreen;
using MTDUI.Controllers;

namespace MTDUI.Data
{
    public class ModOptionsSubmenuState : TitleScreenState
    {
        public static string CurrentSubmenu = "";

        public override void Enter()
        {
            foreach (var button in ModOptionsMenuController.TitleScreenModOptionComponents)
            {
                button.gameObject.SetActive(button.Mod == CurrentSubmenu);
            }

            ModOptionsMenuController.TitleSubmenuBackButton?.onClick.AddListener(owner.ChangeState<ModOptionsMenuState>);
            ModOptionsMenuController.TitleSubmenu?.Show();
        }

        public override void Exit()
        {
            ModOptionsMenuController.TitleSubmenuBackButton?.onClick.RemoveAllListeners();
            ModOptionsMenuController.TitleSubmenu?.Hide();
        }
    }
}
