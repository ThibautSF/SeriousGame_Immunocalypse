using UnityEngine;

/// <summary>
/// Component to add eating functionality (in fact damaging)
/// </summary>
/// <remarks>
/// Need a Predator component alongside
/// Also need TriggerSensitive (same for targets)
/// Potential target need Prey and Health
/// </remarks>
public class Eat : MonoBehaviour {
	//Damage per second
	public float damage = 10;
	public float timeSinceLastEffect = 1f;
	public float effectFreq = 1f;

	public bool infectOnKill = false;
	public float timerFactory = 0f;
	public string newPreyTag = "";
	public bool applyColor = false;
	public Color color;
	public GameObject prefab;
}
