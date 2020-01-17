using UnityEngine;

public class Eat : MonoBehaviour {
	//Damage per second
	public float damage = 10;
	public float timeSinceLastEffect = 1f;
	public float effectFreq = 1f;

	public bool duplicateOnKill = false;
	public GameObject prefab;
}
