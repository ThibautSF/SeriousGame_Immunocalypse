using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Component to mark and represent the player
/// </summary>
/// <remarks>
/// Sould be at least one ( multiple could be unstable)
/// </remarks>
public class Player : MonoBehaviour {
	[Header("Buyable Visuals Settings")]
	public GameObject unitContainer;
	public GameObject unitUIVisual;

	[Header("Buyable Units Settings")]
	public Tilemap spawnArea;
	public List<GameObject> levelBuyablePrefabs;
}
