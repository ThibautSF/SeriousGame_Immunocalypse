using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

//https://github.com/davecusatis/A-Star-Sharp/blob/master/Astar.cs
//https://bitbucket.org/Unity-Technologies/2ddemos/src/b7a77d0313513fe8c5eaafc77bfba10e7650281c/PreviewR401/Assets/Scripts/Player/WalkableTilePathfinding.cs?at=PreviewR401&fileviewer=file-view-default
//https://stackoverflow.com/questions/6661169/finding-adjacent-neighbors-on-a-hexagonal-grid
//https://www.redblobgames.com/grids/hexagons/

public class Astar {
    private List<MyTilemap> tilemaps;

    Vector3Int[] neighbor_directions_yeven = new Vector3Int[6]{
        new Vector3Int(-1, -1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, 1, 0),
	};

    Vector3Int[] neighbor_directions_yodd = new Vector3Int[6]{
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0),
	};

    public Astar(List<MyTilemap> gs) {
        this.tilemaps = gs;
    }

    public Astar(MyTilemap g) {
        this.tilemaps = new List<MyTilemap>();
        this.tilemaps.Add(g);
    }

    public Stack<Vector3Int> FindPath(Vector2 start, Vector2 end) {
        Node startNode = GetNode(start);
        Node endNode = GetNode(end);
        
        Stack<Vector3Int> path = new Stack<Vector3Int>();

        List<Node> open = new List<Node>();
        List<Node> close = new List<Node>();
        List<Node> neighbors;
        Node current = startNode;

        // add first cell to open list
        open.Add(startNode);

        while (open.Count!=0 && !close.Exists(x => x.cell == endNode.cell)) {
            current = open[0];
            open.Remove(current);
            close.Add(current);
            neighbors = GetNeighbors(current);

            foreach (Node n in neighbors) {
                if (!close.Exists(x => x.cell == n.cell) && n.walkable) {
                    if (!open.Exists(x => x.cell == n.cell)) {
                        n.parent = current;
                        n.distance = Mathf.Abs(n.cell.x - endNode.cell.x) + Mathf.Abs(n.cell.y - endNode.cell.y);
                        n.cost = n.cost + n.parent.cost;
                        open.Add(n);
                        open = open.OrderBy(node => node.F).ToList<Node>();
                    }
                }
            }
        }

        //Target never reached
        if (!close.Exists(x => x.cell == endNode.cell)) {
            return null;
        }

        Node temp = close[close.IndexOf(current)];

        while (temp!=startNode && temp!=null) {
            path.Push(temp.cell);
            temp = temp.parent;
        }

        return path;
    }
    
    private Node GetNode(Vector2 pos) {
        float cost = Mathf.Infinity;
        Node node = null;

        for (int i = tilemaps.Count - 1; i >= 0 ; i--) {
            Tilemap map = tilemaps[i].GetTilemap();
            Vector3Int cell = map.WorldToCell(pos);

            if (map.HasTile(cell)) {
                cost = GetCost(tilemaps[i], cell);
                node = new Node(map, cell, cost);
                break;
            } else {
                node = new Node(null, cell, cost);
            }
        }

        return node;
    }

    private float GetCost(MyTilemap tilemap, Vector3Int cell) {
        float cost = Mathf.Infinity;
        
        if (tilemap.HasMapLayer()) {
            float sb = tilemap.GetMapLayer().speedBonus;
            if (sb!=0) {
                cost = 1/sb;
            }
        } else {
            cost = 1f;
        }

        return cost;
    }

    private float GetCost(MyTilemap tilemap, Vector3 directionVector, Transform pt1, Transform pt2) {
        float cost = Mathf.Infinity;

        //Vector3 vector = tilemap.GetTilemap().CellToWorld(directionVector);

        float sb = tilemap.GetBonusMalus(tilemap, directionVector, pt1, pt2);
        if (sb!=0) {
            cost = 1/sb;
        }

        return cost;
    }

    private List<Node> GetNeighbors(Node n) {
        List<Node> neighbors = new List<Node>();

        Vector3Int[] directions;

        if (n.cell.y % 2 == 0) {
            directions = neighbor_directions_yeven;
        } else {
            directions = neighbor_directions_yodd;
        }

        foreach (Vector3Int direction in directions) {
            float cost = Mathf.Infinity;
            Vector3Int tilePosition = new Vector3Int(n.cell.x + direction.x, n.cell.y + direction.y, n.cell.z + direction.z);

            for (int i = tilemaps.Count - 1; i >= 0 ; i--) {
                Tilemap map = tilemaps[i].GetTilemap();

                if (map.HasTile(tilePosition)) {
                    if (n.myMap == map && tilemaps[i].HasMapLayer()) {
                        MapLayer layer = tilemaps[i].GetMapLayer();

                        List<Transform> points = layer.flux.fluxPosis;
                        for (int j = 0; j < points.Count - 1; j++) {
                            if (tilemaps[i].IsBetween(tilemaps[i].GetTilemap().CellToWorld(n.cell), points[j], points[j+1])) {
                                Debug.Log(tilemaps[i].GetTilemap().CellToWorld(n.cell) + " is between " + points[j] + " and "+ points[j+1]);

                                Vector3 directionVector = tilemaps[i].GetTilemap().CellToWorld(tilePosition) - tilemaps[i].GetTilemap().CellToWorld(n.cell);

                                cost = GetCost(tilemaps[i], directionVector, points[j], points[j+1]);
                                break;
                            }
                        }
                    }

                    if (cost == Mathf.Infinity) {
                        cost = GetCost(tilemaps[i], tilePosition);
                    }

                    neighbors.Add(new Node(map, tilePosition, cost));
                    break;
                }
            }
        }
        
        return neighbors;
    }

    public class Node {
        public Node parent;
        public Tilemap myMap;
        public Vector3Int cell;
        public float distance;
        public float cost;
        public float F {
            get {
                if (distance != -1 && cost != -1)
                    return distance + cost;
                else
                    return -1;
            }
        }

        public bool walkable;

        public Node(Tilemap map, Vector3Int pos, float cost) {
            init(map, pos, cost);
            this.walkable = true;
        }

        public Node(Tilemap map, Vector3Int pos, float cost, bool walkable) {
           init(map, pos, cost);
           this.walkable = walkable;
        }

        private void init(Tilemap map, Vector3Int pos, float cost) {
            this.parent = null;
            this.myMap = map;
            this.cell = pos;
            this.distance = -1;
            this.cost = cost;
        }
    }
}
