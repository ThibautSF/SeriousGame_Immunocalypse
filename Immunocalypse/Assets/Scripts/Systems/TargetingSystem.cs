using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using FYFY;

public class TargetingSystem : FSystem {
	private Family _randomMovingGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Move))
	);
	private Family _preysGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Prey))
	);
	private GridMap myMaps;

	public TargetingSystem() {
		Grid grid = Object.FindObjectOfType<Grid>();

		if (grid != null) {
			myMaps = grid.GetComponent<GridMap>();
		}

		/* */
		foreach (GameObject go in _randomMovingGO) {
			updateTarget(go);
			updatePath(go);
		}
		/* */

		_randomMovingGO.addEntryCallback(updateTarget);
	}

	private GameObject seekTarget(GameObject go) {
		Transform tr = go.GetComponent<Transform>();
		Collider2D[] colliders = Physics2D.OverlapCircleAll(tr.position, 50f);
		Predator predator = go.GetComponent<Predator>();
		GameObject closestTarget = null;

		if (colliders.Length > 0) {
			float minDist = Mathf.Infinity;
			minDist = 5f;
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
		}

		return closestTarget;
	}

	private void updateTarget(GameObject go) {
		//TODO clean code duplicated in seekTarget 
		Transform tr = go.GetComponent<Transform>();
		Collider2D[] colliders = Physics2D.OverlapCircleAll(tr.position, 50f);
		Predator predator = go.GetComponent<Predator>();

		if (colliders.Length > 0) {
			float minDist = Mathf.Infinity;
			minDist = 5f;
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

	private void updatePath(GameObject go) {
		Transform tr = go.GetComponent<Transform>();
		Move mv = go.GetComponent<Move>();

		mv.path = new List<Vector3>();
		
		if (myMaps != null) {
			List<MyTilemap> maps = new List<MyTilemap>();

			foreach (Tilemap tm in myMaps.myTileMaps) {
				maps.Add(new MyTilemap(tm, tm.GetComponent<MapLayer>()));
			}

			Astar a = new Astar(maps);
			Stack<Vector3Int> path = a.FindPath(tr.position, mv.targetPosition);
			
			Vector3 lastCellWorldPos = tr.position;
			string s = "[" + myMaps.myTileMaps[0].WorldToCell(lastCellWorldPos).ToString() + "]";

			int i = 0;
			foreach (Vector3Int cell in path) {
				Vector3 cellWorldPos = myMaps.myTileMaps[0].CellToWorld(cell);
				mv.path.Add(cellWorldPos);
				Debug.DrawLine(lastCellWorldPos, cellWorldPos, (i%2==0) ? Color.green : Color.white);
						
				lastCellWorldPos = cellWorldPos;
				s += " -> " + cell.ToString();
				i++;
			}

			s += " | [" + myMaps.myTileMaps[0].WorldToCell(mv.targetPosition).ToString() + "]";

			Debug.Log(s);
		}

		mv.path.Add(mv.targetPosition);
	}

	protected override void onProcess(int familiesUpdateCount) {
		foreach (GameObject go in _randomMovingGO) {
			Transform tr = go.GetComponent<Transform>();
			Move mv = go.GetComponent<Move>();

			GameObject targetSeeked = seekTarget(go);
			if (mv.targetObject == null || mv.targetObject != seekTarget(go)) {
				mv.targetObject = targetSeeked;
			}

			if (mv.targetObject != null) {
				mv.targetPosition = mv.targetObject.transform.position;
				updatePath(go);
			}

			if (mv.targetPosition == tr.position) {
				updateTarget(go);
				updatePath(go);
			}
		}
	}
}
