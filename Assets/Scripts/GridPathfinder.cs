using System.Collections.Generic;
using UnityEngine;

public static class GridPathfinder
{
    public static readonly int gridSize = 10;

    public static List<Vector3> FindPath(Vector3 start, Vector3 target, GridObstacleData obstacleData, Vector3 enemyPosition)
    {
        Vector3Int startGrid = WorldToGridPosition(start);
        Vector3Int targetGrid = WorldToGridPosition(target);
        Vector3Int enemyGrid = WorldToGridPosition(enemyPosition);

        if (!IsWithinGrid(startGrid) || !IsWithinGrid(targetGrid) ||
            obstacleData.obstacleGrid[startGrid.x * gridSize + startGrid.z] ||
            obstacleData.obstacleGrid[targetGrid.x * gridSize + targetGrid.z] ||
            targetGrid == enemyGrid)
        {
            return new List<Vector3>(); // Return empty path if start or target is invalid
        }

        List<Vector3Int> openSet = new List<Vector3Int> { startGrid };
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float> { { startGrid, 0 } };
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float> { { startGrid, Heuristic(startGrid, targetGrid) } };

        while (openSet.Count > 0)
        {
            Vector3Int current = GetLowestFScoreNode(openSet, fScore);
            if (current == targetGrid)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (!IsWithinGrid(neighbor) || obstacleData.obstacleGrid[neighbor.x * gridSize + neighbor.z] || neighbor == enemyGrid)
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, targetGrid);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return new List<Vector3>(); // Return empty path if no valid path found
    }

    private static bool IsWithinGrid(Vector3Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < gridSize &&
               gridPosition.z >= 0 && gridPosition.z < gridSize;
    }


    private static List<Vector3Int> GetNeighbors(Vector3Int node)
    {
        return new List<Vector3Int>
        {
            node + Vector3Int.left,
            node + Vector3Int.right,
            node + Vector3Int.forward,
            node + Vector3Int.back
        };
    }

    private static float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
    }

    private static Vector3Int GetLowestFScoreNode(List<Vector3Int> openSet, Dictionary<Vector3Int, float> fScore)
    {
        Vector3Int lowest = openSet[0];
        float lowestScore = fScore[lowest];

        foreach (Vector3Int node in openSet)
        {
            if (fScore[node] < lowestScore)
            {
                lowest = node;
                lowestScore = fScore[node];
            }
        }

        return lowest;
    }

    private static List<Vector3> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> totalPath = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }

        List<Vector3> worldPath = new List<Vector3>();
        foreach (Vector3Int gridPos in totalPath)
        {
            worldPath.Add(new Vector3(gridPos.x, 0.5f, gridPos.z));
        }

        return worldPath;
    }

    private static Vector3Int WorldToGridPosition(Vector3 position)
    {
        return new Vector3Int(Mathf.RoundToInt(position.x), 0, Mathf.RoundToInt(position.z));
    }
}
