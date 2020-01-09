using UnityEngine;
using UnityEngine.Tilemaps;
using FYFY;
using System.Collections;
using System.Collections.Generic;

public class LevelSystem : FSystem {
	private Family _mapSpawnerGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(MapLayer), typeof(Factory))
	);

	private Family _cellsGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Cell), typeof(Health))
	);

	private Family _playerHealthGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Player), typeof(Health))
	);

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		//Init Cells
		foreach (GameObject go in _mapSpawnerGO) {
			Factory factory = go.GetComponent<Factory>();

			//factory.reloadProgress += Time.deltaTime;
			if (factory.reloadProgress >= factory.reloadTime) {
				Tilemap tilemap = go.GetComponent<Tilemap>();
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

	}
}