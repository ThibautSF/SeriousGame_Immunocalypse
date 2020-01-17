using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Component to add the factory functionality for the level
/// </summary>
/// <remarks>
/// Allow to generate waves of units
/// </remarks>
public class FactoryLevel : MonoBehaviour {
	public Tilemap spawnArea;
	public Tilemap spawnTargetArea;
	public float reloadTime = 1f;
	[HideInInspector]
	public float reloadProgress = 0f;
	public List<Wave> waves = new List<Wave>();

	[HideInInspector]
	public int currentWave = 0;
}
