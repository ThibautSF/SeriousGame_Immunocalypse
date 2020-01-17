using UnityEngine;

/// <summary>
/// Component to add energy production functionality
/// </summary>
/// <remarks>
/// recover can be positive or negative
/// </remarks>
public class Energizer : MonoBehaviour {
	public float recoverPoints = 0f;
	public float reloadTime = 1f;
	[HideInInspector]
	public float reloadProgress = 0f;
}
