using UnityEngine;

public class Move : MonoBehaviour {
	public float speed = 2.5f;
	[HideInInspector]
	public float speedManger = 1f;
	[HideInInspector]
	public Vector3 targetPosition;
	[HideInInspector]
	public GameObject targetObject;
}
