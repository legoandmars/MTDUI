using flanne.TitleScreen;
using MTDUI.Controllers;

namespace MTDUI.Data
{
    public class ModOptionsMenuState : TitleScreenState
    {
        public override void Enter()
        {
            ModOptionsMenuController.TitleMenuBackButton?.onClick.AddListener(owner.ChangeState<TitleMainMenuState>);
            ModOptionsMenuController.TitleMenu?.Show();
        }

        public override void Exit()
        {
            ModOptionsMenuController.TitleMenuBackButton?.onClick.RemoveAllListeners();
            ModOptionsMenuController.TitleMenu?.Hide();
        }
    }
}
