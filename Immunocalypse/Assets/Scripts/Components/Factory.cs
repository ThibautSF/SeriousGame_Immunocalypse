using UnityEngine;

public class Factory : MonoBehaviour {
	public float reloadTime = 1f;
	[HideInInspector]
	public float reloadProgress = 0f;

	public GameObject prefab;
}
