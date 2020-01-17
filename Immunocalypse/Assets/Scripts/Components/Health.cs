using UnityEngine;

/// <summary>
/// Component to add health functionality to an entity
/// </summary>
/// <remarks>
/// myUI allow values to be print on the given UI elements (slider or text)
/// </remarks>
public class Health : MonoBehaviour {
	public float maxHealthPoints = 100f;
	public float healthPoints = 100f;

	public UIGroup myUI;
}
