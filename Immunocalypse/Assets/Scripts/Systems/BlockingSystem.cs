using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;

public class BlockingSystem : FSystem {
	private Family _triggeredGO = FamilyManager.getFamily(
		new AllOfComponents(
			typeof(Triggered2D),
			typeof(Predator),
			typeof(Block)
		)
	);
	
	private Family _preysGO = FamilyManager.getFamily(
		new AllOfComponents(
			typeof(Prey),
			typeof(Move)
		)
	);

	public static FSystem instance;

	public BlockingSystem() {
		instance = this;
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		foreach (GameObject go in _triggeredGO) {
			Transform tr = go.GetComponent<Transform>();
			Triggered2D t2d = go.GetComponent<Triggered2D>();
			Predator predator = go.GetComponent<Predator>();
			Block block = go.GetComponent<Block>();

			block.timeSinceLastEffect += Time.deltaTime;
			bool effectApplied = false;

			//Debug.Log(go.name + " " + t2d.Targets.Length);

			foreach (GameObject target in t2d.Targets) {
				if (_preysGO.contains(target.GetInstanceID())) {
					Prey prey = target.GetComponent<Prey>();
					
					if (predator.myPreys.Contains(prey.myType)) {
						//TODO slow the target
						Move mv = target.GetComponent<Move>();
						Transform collidedTr = target.GetComponent<Transform>();

						float dist = Vector3.Distance(collidedTr.position, tr.position);

						if (block.timeSinceLastEffect >= block.effectFreq) {
							//float f = (float) 0.5f * (1/dist);
							float f = 0.5f;
							mv.speedModifiers.Add(new SpeedModifier(f, false, 2f));
							effectApplied = true;
						}
					}
				}
			}

			if (effectApplied) {
				block.timeSinceLastEffect = 0f;
			}
		}
	}
}