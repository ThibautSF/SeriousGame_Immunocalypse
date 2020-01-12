using UnityEngine;
using UnityEngine.Tilemaps;
using FYFY;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelSystem : FSystem {
	private bool levelInit = false;
	private string levelStatus = "Pending";

	private Family _mapSpawnerGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(MapLayer), typeof(Factory))
	);

	private Family _cellsGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Cell), typeof(Health))
	);

	private Family _playerHealthGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Player), typeof(Health))
	);

	private Family _attackersGO = FamilyManager.getFamily(
		new AnyOfComponents(typeof(Virus), typeof(Bacteria))
	);

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		if (!levelInit) {
			foreach (GameObject go in _mapSpawnerGO) {
				Factory factory = go.GetComponent<Factory>();
				Tilemap tilemap = go.GetComponent<Tilemap>();

				//factory.reloadProgress += Time.deltaTime;
				if (factory.reloadProgress >= factory.reloadTime) {
					List<Vector3> allTiles = new List<Vector3>();

					for (int n = tilemap.cellBounds.xMin; n < tilemap.cellBounds.xMax; n++) {
						for (int p = tilemap.cellBounds.yMin; p < tilemap.cellBounds.yMax; p++) {
							Vector3Int localPos = new Vector3Int(n, p, (int) tilemap.transform.position.y);

							if (tilemap.HasTile(localPos)) {
								Vector3 worldPos = tilemap.CellToWorld(localPos);

								//Instantiate and bind to FYFY a new instance of antibodies drift (factory prefab)
								GameObject mySpawn = Object.Instantiate<GameObject>(factory.prefab, worldPos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
								GameObjectManager.bind(mySpawn);
							}
						}
					}

					factory.reloadProgress = 0f;
				}
			}

			levelInit = true;
		} else {
			//Update health
			foreach (GameObject go in _playerHealthGO) {
				bool init = false;
				Health playerHealth = go.GetComponent<Health>();

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

			foreach (GameObject go in _mapSpawnerGO) {
				Tilemap tilemap = go.GetComponent<Tilemap>();

				//Remove cell tile position when cell is dead
				for (int n = tilemap.cellBounds.xMin; n < tilemap.cellBounds.xMax; n++) {
					for (int p = tilemap.cellBounds.yMin; p < tilemap.cellBounds.yMax; p++) {
						Vector3Int localPos = new Vector3Int(n, p, (int) tilemap.transform.position.y);

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
			}

			if (_attackersGO.Count == 0) {
				levelStatus = "Victory";
			} else {
				int nb = 0;

				foreach (GameObject go in _playerHealthGO) {
					Health playerHealth = go.GetComponent<Health>();

					if (playerHealth.healthPoints == 0)
						nb += 1;
				}

				if (nb == _playerHealthGO.Count)
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
