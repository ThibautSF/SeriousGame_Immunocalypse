using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MyTilemap {
    private Tilemap map;
    private MapLayer mapLayer;

    public MyTilemap(Tilemap m, MapLayer layer) {
        this.map = m;
        this.mapLayer = layer;
    }

    public Tilemap GetTilemap() {
        return map;
    }

    public bool HasMapLayer() {
        return (mapLayer != null);
    }

    public MapLayer GetMapLayer() {
        return mapLayer;
    }

    public bool IsBetween(Vector3 pos, Transform pt1, Transform pt2) {
        Vector3 a = pt1.position;
        Vector3 b = pt2.position;

        //((xb-xa)(xo-xa)+(yb-ya)(yo-ya))/((xb-xa)^2+(yb-ya)^2)
        float f = ((b.x - a.x) * (pos.x - a.x) + (b.y - a.y) * (pos.y - a.y)) / (Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2));

        return (f >= 0 && f <= 1);
    }

    public float GetBonusMalus(MyTilemap tilemap, Vector3 directionVector, Transform pt1, Transform pt2) {
        float bonusmalus = 1;

        Vector3 a = pt1.position;
        Vector3 b = pt2.position;

        //((yb-ya)(y)+(xb-xa)(x))
        float f = (b.x - a.x) * directionVector.x + (b.y - a.y) * directionVector.y;

        if (tilemap.HasMapLayer()) {
            bonusmalus = tilemap.GetMapLayer().speedBonus;

            if (f <= 0) {
                bonusmalus = tilemap.GetMapLayer().flux.speedMalus;
                Debug.Log("Le vecteur " + directionVector + " dans le flux de A(" + a + ") à B(" + b + ") est contre courant : " + f );
            } else {
                Debug.Log("Le vecteur " + directionVector + " dans le flux de A(" + a + ") à B(" + b + ") est en courant : " + f);
            }
        }

        return bonusmalus;
    }
}