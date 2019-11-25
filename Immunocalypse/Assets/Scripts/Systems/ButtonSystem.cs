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

	public void startGame() {
		SceneManager.LoadScene("SampleScene");
	}



	// public void

	// Use this to update member variables when system pause.
	// Advice: avoid to update your families inside this function.
	protected override void onPause(int currentFrame) {
	}

	// Use this to update member variables when system resume.
	// Advice: avoid to update your families inside this function.
	protected override void onResume(int currentFrame){
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
	}
}
