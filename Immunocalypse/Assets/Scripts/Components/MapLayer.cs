using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLayer : MonoBehaviour {
	[Header("Flux")]
	public bool hasflux = false;

	public List<Vector3> flux;

	[Header("Movement")]
	public float speedBonus = 1f;
}