using UnityEngine;

/// <summary>
/// Component to add selection functionality to an entity
/// </summary>
public class SelectableEntity : MonoBehaviour {
	public bool controlable = true;
	public bool isSelected = false;
	public GameObject selectionVisual;
}
