using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using FYFY;

public class ButtonSystem : FSystem {
	private Family _panelGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(ListMenu))
	);

	public static FSystem instance;

	public ButtonSystem() {
		instance = this;
		SystemHolder.allSystems.Add(this);

		Texture2D cursor = Resources.Load<Texture2D>("Pixel Cursors/Cursors/basic_01");
		Cursor.SetCursor(cursor, new Vector2(10, 5), CursorMode.Auto);
	}

	private void changeActiveMenus(string menuToSetActive) {
		List<GameObject> go = _panelGO.First().GetComponent<ListMenu>().listMenus;
		foreach (GameObject menuPanel in go) {
			if (menuPanel.name == menuToSetActive) {
				menuPanel.SetActive(true);
			} else {
				menuPanel.SetActive(false);
			}
		}
	}

	public void openMenu(string name) {
		changeActiveMenus(name);
	}

	public void exitGame() {
		Application.Quit();
	}

	public void startGame(string levelName) {
		//SceneManager.LoadScene(levelName);
		GameObjectManager.loadScene(levelName);
	}

	public void pauseResumeGame(Canvas canvas) {
		bool status = false;
		foreach (FSystem system in SystemHolder.pausableSystems) {
			system.Pause = !system.Pause;
			status = system.Pause;
		}

		if (canvas != null) {
			//canvas.gameObject.SetActive(status);
			GameObjectManager.setGameObjectState(canvas.gameObject, status);
		}
	}

	public void showHide(Canvas canvas) {
		//canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
		GameObjectManager.setGameObjectState(canvas.gameObject, !canvas.gameObject.activeSelf);
	}

	public void modInfo(Canvas canvas) {
		Texture2D cursor;

		//boolean negation due to FYFY activating the GO only next iteration ('showHide' should be called before)
		if (!canvas.gameObject.activeSelf)
			cursor = Resources.Load<Texture2D>("Pixel Cursors/Cursors/Bonus_50");
		else
			cursor = Resources.Load<Texture2D>("Pixel Cursors/Cursors/basic_01");

		Cursor.SetCursor(cursor, new Vector2(10, 5), CursorMode.Auto);
	}

	public void openURL(string url) {
		// TODO (if it was not useless for this project) : check url
		if (url != "") {
			foreach (FSystem system in SystemHolder.pausableSystems) {
				system.Pause = true;
			}

			Application.OpenURL(url);
		}
	}
}
