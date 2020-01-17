using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;

public class FactorySystem : FSystem {
	private Family _factoryGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Factory)),
		new NoneOfComponents(typeof(MapLayer))
	);

	// Use this to update member variables when system pause. 
	// Advice: avoid to update your families inside this function.
	protected override void onPause(int currentFrame) {
	}

	// Use this to update member variables when system resume.
	// Advice: avoid to update your families inside this function.
	protected override void onResume(int currentFrame) {
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		foreach (GameObject go in _factoryGO) {
			Transform tr = go.GetComponent<Transform>();
			Factory[] factories = go.GetComponents<Factory>();

			foreach (Factory factory in factories) {
				factory.reloadProgress += Time.deltaTime;

				if (factory.reloadProgress >= factory.reloadTime) {
					//Instantiate and bind to FYFY a new instance of antibodies drift (factory prefab)
					//factory.prefab.GetType();
					GameObject mySpawn = Object.Instantiate<GameObject>(factory.prefab, tr.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
					GameObjectManager.bind(mySpawn);

					//Set some parameters of the antibodies drift
					Growth gr = mySpawn.GetComponent<Growth>();
					if (gr != null) {
						mySpawn.GetComponent<Growth>().baseSize = tr.localScale.x * 0.5f;
						float f = mySpawn.GetComponent<Growth>().baseSize;
					}

					//Object spawned get predator values of parent (if both have the component)
					Predator predatorFactory = go.GetComponent<Predator>();
					Predator predatorSpawn = mySpawn.GetComponent<Predator>();

					if (predatorFactory != null && predatorSpawn != null) {
						predatorSpawn.myPreys = predatorFactory.myPreys;
					}

					//Reinit factory if object spawned have a factory
					Factory[] spawnFactories = mySpawn.GetComponents<Factory>();
					foreach (Factory spawnFactory in spawnFactories) {
						spawnFactory.reloadProgress = 0f;
					}

					Move mv = mySpawn.GetComponent<Move>();
					if (mv != null) {
						Vector2 posTarget = new Vector2(Random.Range(tr.position.x - 1f, tr.position.x + 1f), Random.Range(tr.position.y -1f, tr.position.y -1f));

						mv.targetPosition = posTarget;
						mv.targetObject = null;
						mv.newTargetPosition = true;
						mv.forcedTarget = true;
					}

					/*
					WARNING: should not do that but looks like that even if instance come frome a prefab, it is instanciated with a Triggered2D...
					Looks like to come from cases where an GO store it's own prefab, but instead store himself, 
					so we don't instantiate a prefab but a copy of the object
					
					If this line is commented, FYFY will generate an error
					*/
					Object.Destroy(mySpawn.GetComponent<Triggered2D>());

					//mySpawn.GetComponent<Transform>().localScale = new Vector3(0f, 0f, 0f);
					//mySpawn.SetActive(true);

					factory.reloadProgress = 0f;
				}
			}
		}
	}
}
