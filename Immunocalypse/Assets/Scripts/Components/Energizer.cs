using UnityEngine;

//Component to determine how long an entity live
public class Energizer : MonoBehaviour {
	public float recoverPoints = 0f;
	public float reloadTime = 1f;
	[HideInInspector]
	public float reloadProgress = 0f;
}
