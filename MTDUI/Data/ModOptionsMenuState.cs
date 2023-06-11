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
            ModOptionsMenuController.TitleMenuBackButton?.onClick.AddListener(OnClick);
            ModOptionsMenuController.TitleMenu?.Show();
        }

        public override void Exit()
        {
            ModOptionsMenuController.TitleMenuBackButton?.onClick.RemoveListener(OnClick);
            ModOptionsMenuController.TitleMenu?.Hide();
        }
    }
}
