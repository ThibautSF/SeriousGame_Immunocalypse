using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using FYFY;

public class ButtonSystem : FSystem {

	private Family _panelGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(ListMenu))
	);

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

	public void openPlayMenu() {
		changeActiveMenus("playMenu");
	}

	public void openCreditsMenu() {
		changeActiveMenus("creditsMenu");
	}

	public void backToMainMenu() {
		changeActiveMenus("MainMenu");
	}

	public void exitGame() {
		Application.Quit();
	}

	public void startGame(string levelName) {
		//SceneManager.LoadScene(levelName);
		GameObjectManager.loadScene(levelName);
	}
}
