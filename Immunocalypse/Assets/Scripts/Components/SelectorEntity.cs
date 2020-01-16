using UnityEngine;

public class SelectorEntity : MonoBehaviour {
	public bool hasSelected = true;
	public bool isSelecting = false;
	public Vector3 mousePosition1;

	void OnGUI() {
		if(isSelecting) {
			// Create a rect from both mouse positions
			var rect = Utils.GetScreenRect(mousePosition1, Input.mousePosition);
			Utils.DrawScreenRect(rect, new Color( 0.8f, 0.8f, 0.95f, 0.25f));
			Utils.DrawScreenRectBorder(rect, 2, new Color( 0.8f, 0.8f, 0.95f));
		}
	}
}
