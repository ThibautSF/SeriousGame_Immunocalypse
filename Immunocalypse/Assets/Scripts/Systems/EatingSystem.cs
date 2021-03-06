﻿using UnityEngine;
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

	public static FSystem instance;

	public EatingSystem() {
		instance = this;
		SystemHolder.allSystems.Add(this);
		SystemHolder.pausableSystems.Add(this);
	}

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

								if (eat.infectOnKill) {
									//Debug.Log(pos);
									prey.myType = eat.newPreyTag;

									GameObjectManager.addComponent<Infected>(target, new { myParasit = go });
									GameObjectManager.addComponent<Factory>(target, new { reloadTime = eat.timerFactory, prefab = eat.prefab });
									collidedH.healthPoints = collidedH.maxHealthPoints;

									Energizer collidedE = target.GetComponent<Energizer>();
									Energizer e = go.GetComponent<Energizer>();
									if (collidedE != null) {
										if (e != null) {
											collidedE.recoverPoints = e.recoverPoints;
											collidedE.reloadTime = e.reloadTime;
										} else {
											GameObjectManager.removeComponent(collidedE);
										}
									} else {
										if (e != null) {
											GameObjectManager.addComponent<Energizer>(target, new { recoverPoints = e.recoverPoints, reloadTime = e.reloadTime });
										}
									}

									if (eat.applyColor) {
										Renderer r = target.GetComponentInChildren<Renderer>();

										if (r != null) {
											if (r is SpriteRenderer)
												((SpriteRenderer) r).color = eat.color;
											else
												r.material.color = eat.color;
										}
									}
								} else {
									GameObjectManager.unbind(target);
									Object.Destroy(target);
								}

								Move mv = go.GetComponent<Move>();
								if (mv != null) {
									mv.targetObject = null;
								}

								/* Previous functionnality of duplication on kill * /
								GameObject myDuplicate = Object.Instantiate<GameObject>(eat.prefab, pos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

								//WARNING: should not do that but looks like that even if instance come frome a prefab, it is instanciated with a Triggered2D...
								//If this line is commented, FYFY will generate an error
								Object.Destroy(myDuplicate.GetComponent<Triggered2D>());

								GameObjectManager.bind(myDuplicate);

								Move mv = myDuplicate.GetComponent<Move>();
								if (mv != null) {
									Vector2 posTarget = new Vector2(Random.Range(pos.x - 1f, pos.x + 1f), Random.Range(pos.y -1f, pos.y -1f));

									mv.targetPosition = posTarget;
									mv.forcedTarget = true;
									mv.newTargetPosition = true;
								}
								/* */
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
