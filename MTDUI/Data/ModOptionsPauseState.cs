using flanne.Core;
using MTDUI.Controllers;

namespace MTDUI.Data
{
    public class ModOptionsPauseState : GameState
    {
        public void OnClick()
        {
            owner.ChangeState<PauseState>();
        }

        public override void Enter()
        {
            ModOptionsMenuController.PauseMenuBackButton?.onClick.AddListener(OnClick);
            ModOptionsMenuController.PauseMenu?.Show();
        }

        public override void Exit()
        {
            ModOptionsMenuController.PauseMenuBackButton?.onClick.RemoveListener(OnClick);
            ModOptionsMenuController.PauseMenu?.Hide();
        }
    }
}
