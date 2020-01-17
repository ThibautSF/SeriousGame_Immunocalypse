using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;

public class EatingSystem : FSystem {
	private Family _triggeredGO = FamilyManager.getFamily(
		new AllOfComponents(
			typeof(Triggered2D),
			typeof(Predator),
			typeof(Eat)
		)
	);
	
	private Family _preysGO = FamilyManager.getFamily(
		new AllOfComponents(
			typeof(Prey),
			typeof(Health)
		)
	);

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		foreach (GameObject go in _triggeredGO) {
			Triggered2D t2d = go.GetComponent<Triggered2D>();
			Predator predator = go.GetComponent<Predator>();
			Eat eat = go.GetComponent<Eat>();

			eat.timeSinceLastEffect += Time.deltaTime;
			bool effectApplied = false;

			foreach (GameObject target in t2d.Targets) {
				if (_preysGO.contains(target.GetInstanceID())) {
					Prey prey = target.GetComponent<Prey>();
					
					if (predator.myPreys.Contains(prey.myType)) {
						//"eat" the target
						Transform collidedTr = target.GetComponent<Transform>();

						Health collidedH = target.GetComponent<Health>();

						if (eat.timeSinceLastEffect >= eat.effectFreq) {
							collidedH.healthPoints -= eat.damage;

							if (collidedH.healthPoints <= 0) {
								Vector3 pos = target.transform.position;
								GameObjectManager.unbind(target);
								Object.Destroy(target);

								if (eat.duplicateOnKill) {
									//Debug.Log(pos);
									GameObject myDuplicate = Object.Instantiate<GameObject>(eat.prefab, pos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

									//WARNING: should not do that but looks like that even if instance come frome a prefab, it is instanciated with a Triggered2D...
									//If this line is commented, FYFY will generate an error
									Object.Destroy(myDuplicate.GetComponent<Triggered2D>());

									GameObjectManager.bind(myDuplicate);

									Move mv = myDuplicate.GetComponent<Move>();
									if (mv != null) {
										//Vector3 posTarget = new Vector3((pos.x - 0.5f) * 5f, (pos.y - 0.5f) * 5f);
										Vector2 posTarget = new Vector2(Random.Range(pos.x - 1f, pos.x + 1f), Random.Range(pos.y -1f, pos.y -1f));

										mv.targetPosition = posTarget;
										mv.forcedTarget = true;
										mv.newTargetPosition = true;
									}
								}
							}

							effectApplied = true;
						}
					}
				}
			}

			if (effectApplied) {
				eat.timeSinceLastEffect = 0f;
			}
		}
	}
}
