﻿using UnityEngine;
using UnityEngine.UI;
using FYFY;

public class UISystem : FSystem {
	private Family _healthUIGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Health), typeof(UIPrint))
	);

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
		foreach (GameObject go in _healthUIGO) {
			Health health = go.GetComponent<Health>();
			UIPrint ui = go.GetComponent<UIPrint>();

			if (health.myUI.mySlider != null) {
				updateSlider(health.myUI.mySlider, health.healthPoints, health.maxHealthPoints);
			}
		}
	}

	private void updateSlider(Slider slider, float value, float maxValue) {
		slider.value = (float) value / maxValue;
	}
}