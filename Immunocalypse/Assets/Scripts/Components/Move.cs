using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
	public float speed = 2.5f;
	[HideInInspector]
	public List<SpeedModifier> speedModifiers = new List<SpeedModifier>();

	[HideInInspector]
	public bool newTargetPosition = false;
	[HideInInspector]
	public Vector3 targetPosition;
	[HideInInspector]
	public GameObject targetObject;
	[HideInInspector]
	public List<Vector3> path = new List<Vector3>();
}
