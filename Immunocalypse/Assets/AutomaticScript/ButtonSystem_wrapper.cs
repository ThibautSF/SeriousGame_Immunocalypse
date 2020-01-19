using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class ButtonSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void openMenu(System.String name)
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "openMenu", name);
	}

	public void exitGame()
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "exitGame", null);
	}

	public void startGame(System.String levelName)
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "startGame", levelName);
	}

	public void pauseResumeGame(UnityEngine.Canvas canvas)
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "pauseResumeGame", canvas);
	}

	public void showHide(UnityEngine.Canvas canvas)
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "showHide", canvas);
	}

	public void modInfo(UnityEngine.Canvas canvas)
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "modInfo", canvas);
	}

	public void openURL(System.String url)
	{
		MainLoop.callAppropriateSystemMethod ("ButtonSystem", "openURL", url);
	}

}