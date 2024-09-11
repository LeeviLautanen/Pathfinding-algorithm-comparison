using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    private Node[,] nodes;
    private int gridSize;
    private TilemapManager tilemapManager;

    public Astar(Node[,] nodes, TilemapManager tilemapManager)
    {
        this.nodes = nodes;
        gridSize = nodes.GetLength(0);

        this.tilemapManager = tilemapManager;
    }

    public List<Node> FindPath(Node start, Node end)
    {
        Heap openSet = new(gridSize);
        HashSet<Node> closedSet = new();

        start.gCost = 0;
        start.hCost = GetDistance(start, end);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);
            tilemapManager.AddClosedTile(currentNode);

            if (currentNode == end)
            {
                Debug.Log(closedSet.Count + openSet.Count + " nodes visited");
                return RetracePath(start, end);
            }

            for (int x = currentNode.x - 1; x <= currentNode.x + 1; x++)
            {
                for (int y = currentNode.y - 1; y <= currentNode.y + 1; y++)
                {
                    if (x == currentNode.x && y == currentNode.y) continue; // Not a neighbor
                    if (x < 0 || x >= gridSize || y < 0 || y >= gridSize) continue; // Outside grid bounds

                    Node neighbor = nodes[x, y];

                    if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                    int newGCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newGCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newGCost;
                        neighbor.hCost = GetDistance(neighbor, end);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                            tilemapManager.AddOpenTile(neighbor);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
        }

        return null;
    }

    public IEnumerator FindPathVisual(Node start, Node end)
    {
        Heap openSet = new(gridSize);
        HashSet<Node> closedSet = new();

        start.gCost = 0;
        start.hCost = GetDistance(start, end);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            tilemapManager.AddClosedTile(currentNode);

            if (currentNode == end)
            {
                Debug.Log("path found");
                yield break;
            }

            for (int x = currentNode.x - 1; x <= currentNode.x + 1; x++)
            {
                for (int y = currentNode.y - 1; y <= currentNode.y + 1; y++)
                {
                    if (x == currentNode.x && y == currentNode.y) continue; // Not a neighbor
                    if (x < 0 || x >= gridSize || y < 0 || y >= gridSize) continue; // Outside grid bounds

                    Node neighbor = nodes[x, y];

                    if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                    int newGCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newGCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newGCost;
                        neighbor.hCost = GetDistance(neighbor, end);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            Debug.Log($"{neighbor.x}, {neighbor.y} added to openset");
                            openSet.Add(neighbor);
                            tilemapManager.AddOpenTile(neighbor);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
            Debug.Log("Count: " + openSet.Count);
            yield return new WaitForSecondsRealtime(.2f);
        }

        Debug.LogWarning("No path found!");
        yield break;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Add(startNode);

        return path;
    }

    private int GetManhattanDistance(Node nodeA, Node nodeB)
    {
        return Math.Abs(nodeA.x - nodeB.x) + Mathf.Abs(nodeA.y - nodeB.y);
    }

    private int GetOctileDistance(Node nodeA, Node nodeB)
    {
        int distX = Math.Abs(nodeA.x - nodeB.x);
        int distY = Math.Abs(nodeA.y - nodeB.y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);

        return 14 * distX + 10 * (distY - distX);
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Math.Abs(nodeA.x - nodeB.x);
        int distY = Math.Abs(nodeA.y - nodeB.y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);

        return 14 * distX + 10 * (distY - distX);
    }
}
