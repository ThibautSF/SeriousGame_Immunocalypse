using UnityEngine;

/// <summary>
/// Component to add blocking functionality
/// </summary>
/// <remarks>
/// Need a Predator component alongside
/// Also need TriggerSensitive (same for targets)
/// Potential target need Prey and Health
/// </remarks>
public class Block : MonoBehaviour {
	public float timeSinceLastEffect = 1f;
	public float effectFreq = 1f;
}
