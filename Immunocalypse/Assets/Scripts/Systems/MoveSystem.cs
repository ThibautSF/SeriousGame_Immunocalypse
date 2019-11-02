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
			myMaps.GetComponent<GridMap>();
		}
	}

	protected override void onProcess(int familiesUpdateCount) {

		foreach (GameObject go in _randomMovingGO) {
			Transform tr = go.GetComponent<Transform>();
			Move mv = go.GetComponent<Move>();

			Vector3 movement = Vector3.zero;

			tr.position = Vector3.MoveTowards(tr.position, mv.targetPosition, mv.speed * Time.deltaTime);

			/*
			Vector3 newDir = Vector3.RotateTowards(tr.forward, mv.target, mv.speed * Time.deltaTime, 0.0f);
			newDir = new Vector3(newDir.x, 0, newDir.z);
			Debug.DrawRay(tr.position, newDir, Color.red);
			tr.rotation = Quaternion.LookRotation(newDir);
			*/

			Vector3 vectorToTarget = mv.targetPosition - tr.position;
			Debug.DrawRay(tr.position, vectorToTarget, Color.red);

			float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
			Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
			tr.rotation = Quaternion.RotateTowards(tr.rotation, qt, Time.deltaTime * mv.speed * 100);

		}
	}

	private TileBase getCell(Tilemap tilemap, Vector2 worldPos) {
		return tilemap.GetTile(tilemap.WorldToCell(worldPos));
	}

	private bool hasTile(Tilemap tilemap, Vector2 worldPos) {
		return tilemap.HasTile(tilemap.WorldToCell(worldPos));
	}
}
