using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using FYFY;

public class TargetingSystem : FSystem {
	private Family _movingGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Move))
	);
	private Family _preysGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Prey))
	);
	private Family _selectableGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(SelectableEntity))
	);

	private Grid grid;
	private GridMap myMaps;

	public TargetingSystem() {
		grid = Object.FindObjectOfType<Grid>();

		if (grid != null) {
			myMaps = grid.GetComponent<GridMap>();
		}
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
		Collider2D c = grid.GetComponent<Collider2D>();

		Bounds b = c.bounds;

		Vector2 newpos = new Vector2(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));

		updateTarget(go, newpos);
	}

	private void updateTarget(GameObject go, Vector3 target) {
		Transform tr = go.GetComponent<Transform>();
		Move mv = go.GetComponent<Move>();

		mv.targetPosition = target;
		mv.newTargetPosition = true;
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
				//Debug.DrawLine(lastCellWorldPos, cellWorldPos, (i%2==0) ? Color.green : Color.white);
						
				lastCellWorldPos = cellWorldPos;
				s += " -> " + cell.ToString();
				i++;
			}

			if (mv.path.Count > 0)
				mv.path.RemoveAt(mv.path.Count -1);

			s += " | [" + myMaps.myTileMaps[0].WorldToCell(mv.targetPosition).ToString() + "]";

			//Debug.Log(s);
		}

		mv.path.Add(mv.targetPosition);
	}

	protected override void onProcess(int familiesUpdateCount) {
		foreach (GameObject go in _movingGO) {
			Transform tr = go.GetComponent<Transform>();
			Move mv = go.GetComponent<Move>();

			GameObject targetSeeked = null;
			if (!mv.forcedTarget) {
				//Seek new target only if we don't have a forced order to do !
				Debug.Log(go.name + " " + mv.forcedTarget);
				targetSeeked = seekTarget(go);
			}

			bool newTargetSeeked = false;
			if (mv.targetObject != targetSeeked) {
				mv.targetObject = targetSeeked;
				newTargetSeeked = true;
			}

			if (mv.targetObject != null && (mv.path.Count == 0 || newTargetSeeked)) {
				mv.targetPosition = mv.targetObject.transform.position;
				updatePath(go);
			} else if ((mv.path.Count == 0 && mv.newTargetPosition) || mv.newTargetPosition) {
				updatePath(go);
				mv.newTargetPosition = false;
			} else {
				/*
				if (mv.targetPosition == tr.position) {
					updateTarget(go);
					updatePath(go);
				}
				*/
				Debug.Log(mv.path.Count);
				if (mv.path.Count == 0 && !_selectableGO.contains(go.GetInstanceID()))
					updateTarget(go);
			}
		}
	}
}
