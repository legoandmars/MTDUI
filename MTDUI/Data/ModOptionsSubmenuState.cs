using flanne.TitleScreen;
using MTDUI.Controllers;

namespace MTDUI.Data
{
    public class ModOptionsSubmenuState : TitleScreenState
    {
        public static string CurrentSubmenu = "";
        public void OnClick()
        {
            owner.ChangeState<ModOptionsMenuState>();
        }

        public override void Enter()
        {
            foreach (var button in ModOptionsMenuController.TitleScreenModOptionComponents)
            {
                button.gameObject.SetActive(button.Mod == CurrentSubmenu);
            }

            ModOptionsMenuController.TitleSubmenuBackButton?.onClick.AddListener(OnClick);
            ModOptionsMenuController.TitleSubmenu?.Show();
        }

        public override void Exit()
        {
            ModOptionsMenuController.TitleSubmenuBackButton?.onClick.RemoveListener(OnClick);
            ModOptionsMenuController.TitleSubmenu?.Hide();
        }
    }
}
