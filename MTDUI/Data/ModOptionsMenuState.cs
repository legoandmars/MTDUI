using flanne.TitleScreen;
using MTDUI.Controllers;

namespace MTDUI.Data
{
    public class ModOptionsMenuState : TitleScreenState
    {
        public void OnClick()
        {
            owner.ChangeState<TitleMainMenuState>();
        }

        public override void Enter()
        {
            ModOptionsMenuController.ModOptionsBackButton?.onClick.AddListener(OnClick);
            ModOptionsMenuController.ModOptionsMenu?.Show();
        }

        public override void Exit()
        {
            ModOptionsMenuController.ModOptionsBackButton?.onClick.RemoveListener(OnClick);
            ModOptionsMenuController.ModOptionsMenu?.Hide();
        }
    }
}
