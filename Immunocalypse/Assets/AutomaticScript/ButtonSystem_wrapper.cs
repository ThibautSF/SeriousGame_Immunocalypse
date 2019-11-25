using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class ButtonSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void openPlayMenu()
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "openPlayMenu", null);
	}

	public void openCreditsMenu()
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "openCreditsMenu", null);
	}

	public void backToMainMenu()
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "backToMainMenu", null);
	}

	public void exitGame()
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "exitGame", null);
	}

	public void startGame()
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "startGame", null);
	}

}