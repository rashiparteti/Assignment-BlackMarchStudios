using System.Collections.Generic;
using UnityEngine;

public static class GridPathfinder
{
    public static readonly int gridSize = 10;

    // FindPath method to find a path from start to target using A* algorithm
    public static List<Vector3> FindPath(Vector3 start, Vector3 target, GridObstacleData obstacleData, Vector3 enemyPosition)
    {
        // Convert world positions to grid positions
        Vector3Int startGrid = WorldToGridPosition(start);
        Vector3Int targetGrid = WorldToGridPosition(target);
        Vector3Int enemyGrid = WorldToGridPosition(enemyPosition);

        // Initialize open set, dictionaries for scores, and add start node to open set
        List<Vector3Int> openSet = new List<Vector3Int> { startGrid };
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float> { { startGrid, 0 } };
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float> { { startGrid, Heuristic(startGrid, targetGrid) } };

        // A* search loop
        while (openSet.Count > 0)
        {
            // Get node in open set with lowest fScore
            Vector3Int current = GetLowestFScoreNode(openSet, fScore);

            // If current node is the target, reconstruct and return path
            if (current == targetGrid)
            {
                return ReconstructPath(cameFrom, current);
            }

            // Remove current node from open set and explore neighbors
            openSet.Remove(current);
            foreach (Vector3Int neighbor in GetNeighbors(current, obstacleData, enemyGrid))
            {
                // Calculate tentative gScore for neighbor
                float tentativeGScore = gScore[current] + Vector3Int.Distance(current, neighbor);

                // Update path if new path to neighbor is shorter or neighbor not evaluated
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Heuristic(neighbor, targetGrid);

                    // Add neighbor to open set if not already there
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // Return an empty path if no path found
        return new List<Vector3>();
    }

    // Convert world position to grid position (integer coordinates)
    private static Vector3Int WorldToGridPosition(Vector3 position)
    {
        return new Vector3Int(Mathf.RoundToInt(position.x), 0, Mathf.RoundToInt(position.z));
    }

    // Convert grid position to world position (with adjusted Y for visibility)
    private static Vector3 GridToWorldPosition(Vector3Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0.5f, gridPosition.z);
    }

    // Heuristic function (Euclidean distance) to estimate path cost
    private static float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Vector3Int.Distance(a, b);
    }

    // Get node in open set with lowest fScore
    private static Vector3Int GetLowestFScoreNode(List<Vector3Int> openSet, Dictionary<Vector3Int, float> fScore)
    {
        Vector3Int lowest = openSet[0];
        float lowestScore = fScore.ContainsKey(lowest) ? fScore[lowest] : float.MaxValue;

        // Iterate through open set to find node with lowest fScore
        foreach (var node in openSet)
        {
            float score = fScore.ContainsKey(node) ? fScore[node] : float.MaxValue;
            if (score < lowestScore)
            {
                lowest = node;
                lowestScore = score;
            }
        }

        return lowest;
    }

    // Get list of valid neighboring grid positions (not out of bounds or obstructed)
    private static List<Vector3Int> GetNeighbors(Vector3Int gridPosition, GridObstacleData obstacleData, Vector3Int enemyPosition)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        // Define possible movement directions (right, left, forward, backward)
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        // Iterate through directions to find valid neighbors
        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborPos = gridPosition + direction;
            if (IsWithinGrid(neighborPos) && !IsObstacle(neighborPos, obstacleData) && neighborPos != enemyPosition)
            {
                neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }

    // Check if grid position is within grid bounds
    private static bool IsWithinGrid(Vector3Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < gridSize && gridPosition.z >= 0 && gridPosition.z < gridSize;
    }

    // Check if grid position is obstructed by an obstacle
    private static bool IsObstacle(Vector3Int gridPosition, GridObstacleData obstacleData)
    {
        int index = gridPosition.x * gridSize + gridPosition.z;
        return obstacleData.obstacleGrid[index];
    }

    // Reconstruct path from start to current node using cameFrom dictionary
    private static List<Vector3> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3> path = new List<Vector3>();

        // Trace path backwards from current node to start
        while (cameFrom.ContainsKey(current))
        {
            path.Add(GridToWorldPosition(current));
            current = cameFrom[current];
        }

        // Reverse path to get correct order from start to end
        path.Reverse();
        return path;
    }
}
