using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class ButtonSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
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