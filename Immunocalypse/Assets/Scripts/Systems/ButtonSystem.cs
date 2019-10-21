using UnityEngine;
using UnityEngine.SceneManagement;
using FYFY;

public class ButtonSystem : FSystem {

	public void exitGame() {
		Application.Quit();
	}

	public void startGame() {
		SceneManager.LoadScene("SampleScene");
	}

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