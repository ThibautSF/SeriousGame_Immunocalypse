using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapUtils {
	public static List<Vector3> getAllWorldPosition(Tilemap tilemap) {
		List<Vector3> tileWorldLocations = new List<Vector3>();
		
		foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
			Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
			Vector3 place = tilemap.CellToWorld(localPlace);
			if (tilemap.HasTile(localPlace)) {
				tileWorldLocations.Add(place);
			}
		}

		return tileWorldLocations;
	}
}
