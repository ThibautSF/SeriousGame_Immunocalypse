using UnityEngine;
using FYFY;

public class TargetingSystem : FSystem {
	private Family _randomMovingGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Move))
	);
	/* TODO : 1 famille chasseur / 1 famille proie par GO
	private Family defaultPreyVirus = inti;
	private Family preys;
	*/

	public TargetingSystem() {
		foreach (GameObject go in _randomMovingGO) {
			//TODO associate prey familly (or create method)
			updateTarget(go);
		}

		_randomMovingGO.addEntryCallback(updateTarget);
	}

	private void updateTarget(GameObject go) {
		//TODO find target
		updateTarget(go, new Vector3((Random.value - 0.5f) * 14f, (Random.value - 0.5f) * 10f));
	}

	private void updateTarget(GameObject go, Vector3 target) {
		Transform tr = go.GetComponent<Transform>();
		Move mv = go.GetComponent<Move>();

		mv.target = target;
	}

	protected override void onProcess(int familiesUpdateCount) {
		foreach (GameObject go in _randomMovingGO) {
			Transform tr = go.GetComponent<Transform>();
			Move mv = go.GetComponent<Move>();

			Vector3 movement = Vector3.zero;

			if (mv.target.Equals(tr.position)) {
				updateTarget(go);
			}

			//Collider[] colliders = Physics.OverlapSphere(tr.position, Mathf.Infinity);
			Collider2D[] colliders = Physics2D.OverlapCircleAll(tr.position, 50f);

			if (colliders.Length > 0) {
				float minDist = Mathf.Infinity;
				Vector3 closestTarget = Vector3.zero;
				foreach (Collider2D collider in colliders) {
					GameObject collidedGO = collider.gameObject;
					if (collidedGO == go) {
						continue;
					}
					Transform collidedTr = collidedGO.GetComponent<Transform>();

					float dist = Vector3.Distance(collidedTr.position, tr.position);
					if (dist < minDist) {
						minDist = dist;
						closestTarget = collidedTr.position;
					}
				}

				if (!closestTarget.Equals(null)) {
					updateTarget(go, closestTarget);
				}
			}

		}
	}
}
