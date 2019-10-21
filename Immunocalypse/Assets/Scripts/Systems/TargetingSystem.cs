using UnityEngine;
using FYFY;

public class TargetingSystem : FSystem {
	private Family _randomMovingGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Move))
	);
	private Family _preysGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Prey))
	);

	public TargetingSystem() {
		/* */
		foreach (GameObject go in _randomMovingGO) {
			updateTarget(go);
		}
		/* */

		_randomMovingGO.addEntryCallback(updateTarget);
	}

	private void updateTarget(GameObject go) {
		Transform tr = go.GetComponent<Transform>();
		Collider2D[] colliders = Physics2D.OverlapCircleAll(tr.position, 50f);
		Predator predator = go.GetComponent<Predator>();

		if (colliders.Length > 0) {
			float minDist = Mathf.Infinity;
			GameObject closestTarget = null;
			foreach (Collider2D collider in colliders) {
				GameObject collidedGO = collider.gameObject;

				if (collidedGO == go) {
					continue;
				} else if (predator != null) {
					if (_preysGO.contains(collidedGO.GetInstanceID())) {
						Transform collidedTr = collidedGO.GetComponent<Transform>();
						Prey prey = collidedGO.GetComponent<Prey>();

						if (predator.myPreys.Contains(prey.myType)) {
							float dist = Vector3.Distance(collidedTr.position, tr.position);
							if (dist < minDist) {
								minDist = dist;
								closestTarget = collidedGO;
							}
						}
					}
				}
			}

			if (closestTarget != null) {
				updateTarget(go, closestTarget);
				return;
			}
		}

		updateTarget(go, new Vector3((Random.value - 0.5f) * 14f, (Random.value - 0.5f) * 10f));
	}

	private void updateTarget(GameObject go, Vector3 target) {
		Transform tr = go.GetComponent<Transform>();
		Move mv = go.GetComponent<Move>();

		mv.targetPosition = target;
	}

	private void updateTarget(GameObject go, GameObject target) {
		Transform tr = go.GetComponent<Transform>();
		Move mv = go.GetComponent<Move>();

		mv.targetObject = target;
	}

	protected override void onProcess(int familiesUpdateCount) {
		foreach (GameObject go in _randomMovingGO) {
			Transform tr = go.GetComponent<Transform>();
			Move mv = go.GetComponent<Move>();

			if (mv.targetObject != null) {
				mv.targetPosition = mv.targetObject.transform.position;
			}

			if (mv.targetPosition == tr.position) {
				updateTarget(go);
			}
		}
	}
}
