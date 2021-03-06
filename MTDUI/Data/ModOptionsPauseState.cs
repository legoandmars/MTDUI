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
			ModOptionsMenuController.PauseModOptionsBackButton?.onClick.AddListener(OnClick);
			ModOptionsMenuController.PauseModOptionsPanel?.Show();
		}

		public override void Exit()
		{
			ModOptionsMenuController.PauseModOptionsBackButton?.onClick.RemoveListener(OnClick);
			ModOptionsMenuController.PauseModOptionsPanel?.Hide();
		}
	}
}
