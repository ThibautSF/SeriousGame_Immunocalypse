using UnityEngine;

/// <summary>
/// Component to add energy functionality to an entity
/// </summary>
/// <remarks>
/// myUI allow values to be print on the given UI elements (slider or text)
/// </remarks>
public class Energy : MonoBehaviour {
	public float maxEnergyPoints = 100f;
	public float energyPoints = 100f;

	public UIGroup myUI;
}
