using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour {
	[Header("Buyable Visuals Settings")]
	public GameObject unitContainer;
	public GameObject unitUIVisual;

	[Header("Buyable Units Settings")]
	public Tilemap spawnArea;
	public List<GameObject> levelBuyablePrefabs;
}
