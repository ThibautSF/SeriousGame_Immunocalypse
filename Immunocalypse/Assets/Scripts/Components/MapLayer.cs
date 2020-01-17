using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to mark a grid map layer
/// </summary>
public class MapLayer : MonoBehaviour {
	[Header("Flux")]
	public bool hasflux = false;
	
	public Flux flux;

	[Header("Movement")]
	public float speedBonus = 1f;
}
