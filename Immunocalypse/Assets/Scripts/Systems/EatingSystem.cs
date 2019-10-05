using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;

public class EatingSystem : FSystem {
	private Family _triggeredGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Triggered2D))
	);

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		foreach (GameObject go in _triggeredGO) {
			Triggered2D t2d = go.GetComponent<Triggered2D>();

			foreach (GameObject target in t2d.Targets) {
				GameObjectManager.unbind(target);
				Object.Destroy(target);
			}
		}
	}
}