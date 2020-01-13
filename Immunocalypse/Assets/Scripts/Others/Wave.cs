using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave {
	public List<Group> groups;
	public float timeBeforeNext = 1f;
}

[System.Serializable]
public class Group {
	public int nbSpawn;
	public GameObject prefab;
}
