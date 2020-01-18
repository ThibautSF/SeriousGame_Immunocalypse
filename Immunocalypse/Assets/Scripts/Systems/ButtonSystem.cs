﻿using UnityEngine;
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

	public void pauseResumeGame() {
		foreach (FSystem system in SystemHolder.pausableSystems) {
			system.Pause = !system.Pause;
		}
	}

	public void showHide(Canvas canvas) {
		canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
	}

	public void modInfo (Canvas canvas) {
		Texture2D cursor;

		if (canvas.gameObject.activeSelf)
			cursor = Resources.Load<Texture2D>("Pixel Cursors/Cursors/Bonus_50");
		else
			cursor = Resources.Load<Texture2D>("Pixel Cursors/Cursors/basic_01");

		Cursor.SetCursor(cursor, new Vector2(10, 5), CursorMode.Auto);
	}
}
