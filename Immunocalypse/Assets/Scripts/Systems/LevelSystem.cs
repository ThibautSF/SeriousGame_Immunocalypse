using UnityEngine;
using UnityEngine.Tilemaps;
using FYFY;
using System.Collections;
using System.Collections.Generic;

public class LevelSystem : FSystem {
	private bool levelInit = false;
	private string levelStatus = "Pending";

	private Family _mapSpawnerGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(MapLayer), typeof(Factory))
	);

	private Family _levelSpawnerGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(FactoryLevel))
	);

	private Family _cellsGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Cell), typeof(Health))
	);

	private Family _playerGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Player)),
		new AnyOfComponents(typeof(Health), typeof(Energy))
	);

	private Family _energizerGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Energizer))
	);

	private Family _attackersGO = FamilyManager.getFamily(
		new AnyOfComponents(typeof(FactoryLevel), typeof(Virus), typeof(Bacteria))
	);

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		if (!levelInit) {
			foreach (GameObject go in _mapSpawnerGO) {
				Factory factory = go.GetComponent<Factory>();
				Tilemap tilemap = go.GetComponent<Tilemap>();

				//factory.reloadProgress += Time.deltaTime;
				if (factory.reloadProgress >= factory.reloadTime) {
					foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
						Vector3Int localPos = new Vector3Int(pos.x, pos.y, pos.z);

						if (tilemap.HasTile(localPos)) {
							Vector3 worldPos = tilemap.CellToWorld(localPos);

							//Instantiate and bind to FYFY a new instance of antibodies drift (factory prefab)
							GameObject mySpawn = Object.Instantiate<GameObject>(factory.prefab, worldPos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
							GameObjectManager.bind(mySpawn);
						}
					}

					factory.reloadProgress = 0f;
				}
			}

			foreach (GameObject go in _playerGO) {
				Player player = go.GetComponent<Player>();

				foreach (GameObject buyable in player.levelBuyablePrefabs) {
					//Create a new visual
					GameObject myUI = Object.Instantiate<GameObject>(player.unitUIVisual, player.unitContainer.transform);
					GameObjectManager.bind(myUI);

					//Add height to container (each new element is 100px height here)
					RectTransform rt = player.unitContainer.GetComponent<RectTransform>();
					rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y + 100);

					SpriteRenderer sr = buyable.GetComponentInChildren<SpriteRenderer>();
					Info info = buyable.GetComponent<Info>();

					//Update UI image and text
					UIUnit ui = myUI.GetComponent<UIUnit>();
					ui.image.sprite = sr.sprite;
					ui.text.text = info.myName;

					ui.prefab = buyable;
				}
			}

			levelInit = true;
		} else {
			foreach (GameObject go in _playerGO) {
				Health playerHealth = go.GetComponent<Health>();
				Energy playerEnergy = go.GetComponent<Energy>();

				//Update health
				if (playerHealth != null) {
					bool init = false;

					if (playerHealth.maxHealthPoints == 0)
						init = true;
					playerHealth.healthPoints = 0;

					foreach (GameObject cellGO in _cellsGO) {
						Health cellHealth = cellGO.GetComponent<Health>();

						if (init)
							playerHealth.maxHealthPoints += cellHealth.maxHealthPoints;

						playerHealth.healthPoints += cellHealth.healthPoints;
					}
				}

				//Update energy
				if (playerEnergy != null) {
					foreach (GameObject energizerGO in _energizerGO) {
						Energizer energizer = energizerGO.GetComponent<Energizer>();

						energizer.reloadProgress += Time.deltaTime;

						if (energizer.reloadProgress >= energizer.reloadTime) {
							playerEnergy.energyPoints += energizer.recoverPoints;

							energizer.reloadProgress = 0f;
						}
					}

					//Cap at max
					if (playerEnergy.energyPoints > playerEnergy.maxEnergyPoints)
						playerEnergy.energyPoints = playerEnergy.maxEnergyPoints;

					if (playerEnergy.energyPoints < 0)
						playerEnergy.energyPoints = 0f;
				}
			}

			//Check cell spawn position of cell is dead
			foreach (GameObject go in _mapSpawnerGO) {
				Tilemap tilemap = go.GetComponent<Tilemap>();

				//Remove cell tile position when cell is dead
				foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
					Vector3Int localPos = new Vector3Int(pos.x, pos.y, pos.z);

					if (tilemap.HasTile(localPos)) {
						bool cellFound = false;
						Vector3 worldPos = tilemap.CellToWorld(localPos);

						Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, 1f, LayerMask.GetMask("Default"));

						foreach (Collider2D collider in colliders) {
							GameObject collidedGO = collider.gameObject;

							if (_cellsGO.contains(collidedGO.GetInstanceID())) {
								//There is a cell at position
								cellFound = true;
								break;
							}
						}

						if (!cellFound) {
							tilemap.SetTile(localPos, null);
						}
					}
				}
			}

			//Spawn waves
			foreach (GameObject go in _levelSpawnerGO) {
				FactoryLevel factory = go.GetComponent<FactoryLevel>();

				factory.reloadProgress += Time.deltaTime;
				
				if (factory.reloadProgress >= factory.reloadTime) {
					if (factory.currentWave < factory.waves.Count) {
						Wave wave = factory.waves[factory.currentWave];

						List<Vector3> spawnArea = TilemapUtils.getAllWorldPosition(factory.spawnArea);
						List<Vector3> targetArea = TilemapUtils.getAllWorldPosition(factory.spawnTargetArea);

						//Spawn each group of entity
						foreach (Group g in wave.groups) {
							int i = 0;

							while (i < g.nbSpawn) {
								Vector3 position = Vector3.zero;
								if (spawnArea.Count > 0)
									position = spawnArea[Random.Range(0, spawnArea.Count)];

								GameObject mySpawn = Object.Instantiate<GameObject>(g.prefab, position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
								GameObjectManager.bind(mySpawn);

								//Init first target move
								Vector3 target = Vector3.zero;
								if (targetArea.Count > 0)
									target = targetArea[Random.Range(0, targetArea.Count)];
								
								Move mv = mySpawn.GetComponent<Move>();
								if (mv != null) {
									mv.targetPosition = target;
									mv.newTargetPosition = true;
									mv.forcedTarget = true;
								}

								i++;
							}
						}

						factory.reloadTime = wave.timeBeforeNext;
						factory.reloadProgress = 0f;

						factory.currentWave += 1;
					}
				}

				//Destroy level wave factory if last wave was released (for victory condition)
				if (factory.currentWave >= factory.waves.Count) {
					GameObjectManager.unbind(go);
					Object.Destroy(go);
				}
			}

			//Check level status
			if (_attackersGO.Count == 0) {
				levelStatus = "Victory";
			} else {
				int nbHealth = 0;
				int nbHealthZero = 0;

				foreach (GameObject go in _playerGO) {
					Health playerHealth = go.GetComponent<Health>();

					if (playerHealth != null) {
						nbHealth += 1;

						if (playerHealth.healthPoints == 0)
							nbHealthZero += 1;
					}
				}

				if (nbHealthZero == nbHealth)
					levelStatus = "Defeat";
			}
		}

		switch (levelStatus) {
			case "Victory":
				Debug.Log(levelStatus);
				//TODO
				break;

			case "Defeat":
				Debug.Log(levelStatus);
				//TODO
				break;

			case "Pending":
			default:
				break;
		}
	}
}
