﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using FYFY;

public class MoveSystem : FSystem {
	private Family _movingGO = FamilyManager.getFamily(
		new AllOfComponents(typeof(Move))
	);

	private GridMap myMaps;

	public static FSystem instance;

	public MoveSystem() {
		instance = this;
		SystemHolder.allSystems.Add(this);
		SystemHolder.pausableSystems.Add(this);

		Grid grid = Object.FindObjectOfType<Grid>();

		if (grid != null) {
			myMaps = grid.GetComponent<GridMap>();
		}
	}

	protected override void onProcess(int familiesUpdateCount) {

		foreach (GameObject go in _movingGO) {
			Transform tr = go.GetComponent<Transform>();
			Move mv = go.GetComponent<Move>();

			if (mv.path.Count > 0) {
				//Get next target
				Vector3 target = mv.path[0];

				//Clean target if reached
				if (myMaps != null) {
					//If we reach the tile targeted at worldpos -> consider subtarget reached
					for (int i = myMaps.myTileMaps.Count - 1; i >= 0 ; i--) {
						if (myMaps.myTileMaps[i].WorldToCell(tr.position) == myMaps.myTileMaps[i].WorldToCell(target)) {
							mv.path.RemoveAt(0);
							if (mv.path.Count == 1)
								mv.forcedTarget = false;
							break;
						}
					}
				} else {
					/*
					//Last target must be reached !

					//If we reach the tile targeted at worldpos -> allow to retarget (avoid blocked by unreachable target if we can retarget)
					//(Other cases must be retarget manualy by the player)
					for (int i = myMaps.myTileMaps.Count - 1; i >= 0 ; i--) {
						if (myMaps.myTileMaps[i].WorldToCell(tr.position) == myMaps.myTileMaps[i].WorldToCell(target)) {
							mv.forcedTarget = false;
							break;
						}
					}
					

					//If target reached
					if (tr.position == target) {
						if (mv.path.Count > 0) {
							mv.path.RemoveAt(0);
							mv.forcedTarget = false;
						}
					}
					*/
				}

				if (mv.path.Count == 0)
					continue;

				target = mv.path[0];

				printPath(go);
				
				//Look at direction
				Vector2 vectorToTarget = target - tr.position;
				Debug.DrawRay(tr.position, vectorToTarget, Color.red);
				float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
				Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
				tr.rotation = Quaternion.RotateTowards(tr.rotation, qt, Time.deltaTime * mv.speed * 100);
				
				//Move
				float modifiers = getSpeedModifiers(go, tr.position, vectorToTarget);
				tr.position = Vector2.MoveTowards(tr.position, target, mv.speed * modifiers * Time.deltaTime);

			}
		}
	}

	private TileBase getTile(Tilemap tilemap, Vector2 worldPos) {
		return tilemap.GetTile(tilemap.WorldToCell(worldPos));
	}

	private bool hasTile(Tilemap tilemap, Vector2 worldPos) {
		return tilemap.HasTile(tilemap.WorldToCell(worldPos));
	}

	private float getSpeedModifiers(GameObject go, Vector2 worldPos, Vector2 vectorToTarget) {
		float sm = 1f;

		//Get the speed modifer from map
		if (myMaps != null) {
			for (int i = myMaps.myTileMaps.Count - 1; i >= 0 ; i--) {
				if (hasTile(myMaps.myTileMaps[i], worldPos)) {
					MyTilemap tm = new MyTilemap(myMaps.myTileMaps[i], myMaps.myTileMaps[i].GetComponent<MapLayer>());

					if (tm.HasMapLayer()) {
						MapLayer layer = tm.GetMapLayer();
						
						List<Transform> points = layer.flux.fluxPosis;
						
						for (int j = 0; j < points.Count - 1; j++) {
							if (tm.IsBetween(worldPos, points[j], points[j+1])) {
								sm *= tm.GetBonusMalus(tm, vectorToTarget, points[j], points[j+1]);
								break;
							}
						}
					}

					break;
				}
			}
		}

		Move mv = go.GetComponent<Move>();

		for (int i = mv.speedModifiers.Count - 1; i >= 0; i--) {
			mv.speedModifiers[i].decrement(Time.deltaTime);
			sm *= mv.speedModifiers[i].getModifier();

			if (!mv.speedModifiers[i].isValid()) {
				mv.speedModifiers.RemoveAt(i);
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
