using UnityEngine;

//Component to determine how long an entity take to grow between his baseSize and finalSize
public class Growth : MonoBehaviour {
	public float growthTime = 2f;
	public float growthProgress = 0f;

	public float baseSize = 0f;
	public float finalSize = 1f;
}
