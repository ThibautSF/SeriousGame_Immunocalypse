using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

//https://github.com/davecusatis/A-Star-Sharp/blob/master/Astar.cs
//https://bitbucket.org/Unity-Technologies/2ddemos/src/b7a77d0313513fe8c5eaafc77bfba10e7650281c/PreviewR401/Assets/Scripts/Player/WalkableTilePathfinding.cs?at=PreviewR401&fileviewer=file-view-default

public class Astar {
    private List<Tilemap> tilemaps;

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

    public Astar(List<Tilemap> gs) {
        this.tilemaps = gs;
    }

    public Astar(Tilemap g) {
        this.tilemaps = new List<Tilemap>();
        this.tilemaps.Add(g);
    }

    public Stack<Vector3Int> findPath(Vector2 start, Vector2 end) {
        Vector3Int startCell = getCell(start);
        Node startNode = new Node(startCell, getCostTop(startCell));
        Vector3Int endCell = getCell(end);
        Node endNode = new Node(endCell, getCostTop(endCell));
        
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
            neighbors = getNeighbors(current);
            
            /* *
            foreach (Node n in neighbors) {
                Debug.Log(n.cell);
            }
            /* *
            Debug.Log(neighbors.Count);
            Debug.Log(current.cell.ToString() + " : " + neighbors.ToString());
            //return path;
            /* */

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

    private Vector3Int getCell(Vector2 pos) {
        return tilemaps[0].WorldToCell(pos);
    }

    private float getCostTop(Vector3Int cell) {
        float cost = Mathf.Infinity;
        for (int i = tilemaps.Count - 1; i >= 0 ; i--) {
            if (tilemaps[i].HasTile(cell)) {
                cost = getCost(tilemaps[i], cell);
                break;
            }
        }

        return cost;
    }

    private float getCost(Tilemap tilemap, Vector3Int cell) {
        float cost = Mathf.Infinity;
        
        float sb = tilemap.GetComponent<MapLayer>().speedBonus;
        if (sb!=0) {
            cost = 1/sb;
        }

        return cost;
    }

    private List<Node> getNeighbors(Node n) {
        List<Node> neighbors = new List<Node>();
        //string s = n.cell.ToString() + " : ";

        Vector3Int[] directions;

        if (n.cell.y % 2 == 0) {
            directions = neighbor_directions_yeven;
        } else {
            directions = neighbor_directions_yodd;
        }

        foreach (Vector3Int direction in directions) {
            Vector3Int tilePosition = new Vector3Int(n.cell.x + direction.x, n.cell.y + direction.y, n.cell.z + direction.z);

            for (int i = tilemaps.Count - 1; i >= 0 ; i--) {
                if (tilemaps[i].HasTile(tilePosition)) {
                    neighbors.Add(new Node(tilePosition, getCost(tilemaps[i], tilePosition)));
                    //s += tilePosition.ToString() + ", ";
                    //tilemaps[i].SetTile(tilePosition, null);
                    break;
                }
            }
        }

        //Debug.Log(s);

        return neighbors;
    }

    public class Node {
        public Node parent;
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

        public Node(Vector3Int pos, float cost) {
            init(pos, cost);
            this.walkable = true;
        }

        public Node(Vector3Int pos, float cost, bool walkable) {
           init(pos, cost);
           this.walkable = walkable;
        }

        private void init(Vector3Int pos, float cost) {
            this.parent = null;
            this.cell = pos;
            this.distance = -1;
            this.cost = cost;
        }
    }
}
