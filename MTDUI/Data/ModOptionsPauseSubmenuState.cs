using flanne.Core;
using MTDUI.Controllers;
using UnityEngine;

namespace MTDUI.Data
{
	public class ModOptionsPauseSubmenuState : GameState
	{
		public static string CurrentSubmenu = "";
		private static RectTransform rect;
		public void OnClick()
		{
			owner.ChangeState<ModOptionsPauseState>();
		}

		public override void Enter()
		{
			if (rect == null) rect = ModOptionsMenuController.PauseModOptionsSubPanel?.GetComponent<RectTransform>();

			if (rect != null)
			{
				// rescaling has to be done upon enter due to there being multiple sub menus
				var entryCount = 1;
				foreach (var button in ModOptionsMenuController.ModOptionComponents)
				{
					button.gameObject.SetActive(button.Mod == CurrentSubmenu);
					if (button.Mod == CurrentSubmenu) entryCount++;
				}

				var currentY = 160;
				if (entryCount < 6) currentY -= (6 - entryCount) * 24;
				else if (entryCount > 6) currentY += (entryCount - 6) * 24;

				rect.sizeDelta = new Vector2(300, currentY);
			}

			ModOptionsMenuController.PauseModOptionsSubMenuBackButton?.onClick.AddListener(OnClick);
			ModOptionsMenuController.PauseModOptionsSubPanel?.Show();
		}

		public override void Exit()
		{
			ModOptionsMenuController.PauseModOptionsSubMenuBackButton?.onClick.RemoveListener(OnClick);
			ModOptionsMenuController.PauseModOptionsSubPanel?.Hide();
		}
	}
}
