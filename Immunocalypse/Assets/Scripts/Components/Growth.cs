using UnityEngine;

/// <summary>
/// Component to add growing functionality to an entity
/// </summary>
/// <remarks>
/// Grow between his baseSize and finalSize
/// Also need Life entity
/// </remarks>
public class Growth : MonoBehaviour {
	public bool hasGrewUp = false;
	
	public float growthTime = 2f;
	public float growthProgress = 0f;

	public float baseSize = 0f;
	public float finalSize = 1f;
}
