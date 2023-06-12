using flanne.Core;
using MTDUI.Controllers;

namespace MTDUI.Data
{
    public class ModOptionsPauseState : GameState
    {
        public override void Enter()
        {
            ModOptionsMenuController.PauseMenuBackButton?.onClick.AddListener(owner.ChangeState<PauseState>);
            ModOptionsMenuController.PauseMenu?.Show();
        }

        public override void Exit()
        {
            ModOptionsMenuController.PauseMenuBackButton?.onClick.RemoveAllListeners();
            ModOptionsMenuController.PauseMenu?.Hide();
        }
    }
}
