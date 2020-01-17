using UnityEngine;

/// <summary>
/// Component to add factory functionality to an entity (produce new gameobject)
/// </summary>
public class Factory : MonoBehaviour {
	public float reloadTime = 1f;
	[HideInInspector]
	public float reloadProgress = 0f;

	public GameObject prefab;
}
