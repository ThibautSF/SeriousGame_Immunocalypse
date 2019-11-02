using UnityEngine;
using UnityEngine.Tilemaps;
using FYFY;

public class MoveSystem : FSystem {
	private Family _randomMovingGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Move))
	);

	//private Grid myMaps;
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

			Vector2 movement = Vector2.zero;
			
			//Look at direction
			Vector2 vectorToTarget = mv.targetPosition - tr.position;
			Debug.DrawRay(tr.position, vectorToTarget, Color.red);
			float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
			Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
			tr.rotation = Quaternion.RotateTowards(tr.rotation, qt, Time.deltaTime * mv.speed * 100);
			
			//Move
			float modifiers = getSpeedModifier(tr.position);
			tr.position = Vector2.MoveTowards(tr.position, mv.targetPosition, mv.speed * modifiers * Time.deltaTime);
		}
	}

	private TileBase getCell(Tilemap tilemap, Vector2 worldPos) {
		return tilemap.GetTile(tilemap.WorldToCell(worldPos));
	}

	private bool hasTile(Tilemap tilemap, Vector2 worldPos) {
		return tilemap.HasTile(tilemap.WorldToCell(worldPos));
	}

	private float getSpeedModifier(Vector2 worldPos) {
		float sm = 1f;

		for (int i = myMaps.myTileMaps.Count - 1; i >= 0 ; i--) {
			if (hasTile(myMaps.myTileMaps[i], worldPos)) {
				sm *= myMaps.myTileMaps[i].GetComponent<MapLayer>().speedBonus;
				break;
			}
		}

		return sm;
	}
}
