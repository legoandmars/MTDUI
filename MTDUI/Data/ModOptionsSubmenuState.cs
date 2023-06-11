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
            foreach (var button in ModOptionsMenuController.ModOptionComponents)
            {
                button.gameObject.SetActive(button.Mod == CurrentSubmenu);
            }

            ModOptionsMenuController.ModOptionsSubMenuBackButton?.onClick.AddListener(OnClick);
            ModOptionsMenuController.ModOptionsSubmenuMenu?.Show();
        }

        public override void Exit()
        {
            ModOptionsMenuController.ModOptionsSubMenuBackButton?.onClick.RemoveListener(OnClick);
            ModOptionsMenuController.ModOptionsSubmenuMenu?.Hide();
        }
    }
}
