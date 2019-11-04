using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using FYFY;

public class MoveSystem : FSystem {
	private Family _randomMovingGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Move))
	);

	private GridMap myMaps;

	public MoveSystem() {
		Grid grid = Object.FindObjectOfType<Grid>();

		if (grid != null) {
			myMaps = grid.GetComponent<GridMap>();
		}
	}

	protected override void onProcess(int familiesUpdateCount) {

		foreach (GameObject go in _randomMovingGO) {
			Transform tr = go.GetComponent<Transform>();
			Move mv = go.GetComponent<Move>();

			//Get next target
			Vector3 target = mv.path[0];
			printPath(go);
			
			//Look at direction
			Vector2 vectorToTarget = target - tr.position;
			Debug.DrawRay(tr.position, vectorToTarget, Color.red);
			float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
			Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
			tr.rotation = Quaternion.RotateTowards(tr.rotation, qt, Time.deltaTime * mv.speed * 100);
			
			//Move
			float modifiers = getSpeedModifier(tr.position);
			tr.position = Vector2.MoveTowards(tr.position, target, mv.speed * modifiers * Time.deltaTime);

			//Clean target if reached
			if (tr.position == target) {
				if (mv.path.Count > 0) {
					mv.path.RemoveAt(0);
				}
			}
		}
	}

	private TileBase getTile(Tilemap tilemap, Vector2 worldPos) {
		return tilemap.GetTile(tilemap.WorldToCell(worldPos));
	}

	private bool hasTile(Tilemap tilemap, Vector2 worldPos) {
		return tilemap.HasTile(tilemap.WorldToCell(worldPos));
	}

	private float getSpeedModifier(Vector2 worldPos) {
		float sm = 1f;

		if (myMaps != null) {
			for (int i = myMaps.myTileMaps.Count - 1; i >= 0 ; i--) {
				if (hasTile(myMaps.myTileMaps[i], worldPos)) {
					sm *= myMaps.myTileMaps[i].GetComponent<MapLayer>().speedBonus;
					break;
				}
			}
		}

		return sm;
	}

	private void printPath(GameObject go) {
		Transform tr = go.GetComponent<Transform>();
		Move mv = go.GetComponent<Move>();

		Vector3 lastStep = tr.position;
		for (int i = 0; i < mv.path.Count; i++){
			Debug.DrawLine(lastStep, mv.path[i], (i%2==0) ? Color.green : Color.white);
					
			lastStep = mv.path[i];
		}
	}
}
