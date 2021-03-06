﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to add moving functionality to an entity
/// </summary>
public class Move : MonoBehaviour {
	public float speed = 2.5f;
	[HideInInspector]
	public List<SpeedModifier> speedModifiers = new List<SpeedModifier>();

	[HideInInspector]
	public bool newTargetPosition = false;
	[HideInInspector]
	public bool forcedTarget = false;
	[HideInInspector]
	public Vector3 targetPosition;
	[HideInInspector]
	public GameObject targetObject;
	[HideInInspector]
	public List<Vector3> path = new List<Vector3>();
}
