using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Ugh, I'm not proud of any of this... at least it sorta works

public class AStarNavList
{

    public AStarNavList()
    {
        AStarNavNode.GenerateAllNodes();
        AStarNavNode.GenerateAllNeighbors();
    }

    public List<Vector3> BuildPath(Vector3 origin, Vector3 destination)
    {
        // Attempt LOS first
        RaycastHit2D hit = Physics2D.Raycast(origin, (destination - origin), (destination - origin).magnitude, LayerMask.GetMask("Default"));
        if (hit.collider == null)
        {
            List<Vector3> result = new List<Vector3>();
            result.Add(destination);
            return result;
        }
        Debug.DrawLine(origin, destination, Color.red, 5f);

        List<AStarNavNode> closedSet = new List<AStarNavNode>();
        List<AStarNavNode> openSet = new List<AStarNavNode>();

        AStarNavNode start = AStarNavNode.GetClosest(origin);
        AStarNavNode dest = AStarNavNode.GetClosest(destination);

        openSet.Add(start);

        Dictionary<AStarNavNode, AStarNavNode> cameFrom = new Dictionary<AStarNavNode,AStarNavNode>();

        Dictionary<AStarNavNode, float> gScores = new Dictionary<AStarNavNode, float>();
        Dictionary<AStarNavNode, float> fScores = new Dictionary<AStarNavNode, float>();
        foreach (AStarNavNode n in AStarNavNode.map.Values)
        {
            gScores[n] = float.PositiveInfinity;
            fScores[n] = float.PositiveInfinity;
        }
        gScores[start] = 0f;
        fScores[start] = CostEstimate(start, dest);

        int MAX_CYCLES = 500;
        int it = 0;
        while (openSet.Count > 0)
        {
            AStarNavNode current = GetFromSetWithLowestFScore(openSet, fScores);
            if (current == dest)
            {
                return ConstructPath(cameFrom, current, destination);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach(AStarNavNode neighbor in current.neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float possibleGScore = gScores[current] + CostEstimate(current, neighbor);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (possibleGScore >= gScores[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScores[neighbor] = possibleGScore;
                fScores[neighbor] = possibleGScore + CostEstimate(neighbor, dest);
            }
            it++;
            if (it > MAX_CYCLES)
            {
                break;
            }
        }
        Debug.LogWarning("Could not resolve path: " + closedSet.Count + " failed attempts from node " + start.position + " with " + start.neighbors.Count + " neighbors.");
        return null;
    }

    List<Vector3> ConstructPath(Dictionary<AStarNavNode, AStarNavNode> cameFrom, AStarNavNode current, Vector3 destination)
    {
        List<Vector3> totalPath = new List<Vector3>();
        totalPath.Add(current.position);
        totalPath.Add(destination);
        while (cameFrom.Keys.Contains(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current.position);
        }
        return totalPath;
    }

    AStarNavNode GetFromSetWithLowestFScore(List<AStarNavNode> set, Dictionary<AStarNavNode, float> fScores)
    {
        float lowest = float.PositiveInfinity;
        AStarNavNode cheapest = null;
        foreach(AStarNavNode node in set)
        {
            if (fScores[node] < lowest)
            {
                lowest = fScores[node];
                cheapest = node;
            }
        }
        return cheapest;
    }

    float CostEstimate(AStarNavNode node, AStarNavNode dest)
    {
        return Vector3.Distance(node.position, dest.position);
    }
}

public class AStarNavNode
{
    public const int MAP_SIZE = 40; // Maximum distance in game units from origin to build map
    public const float RESOLUTION = 0.5f; // Distance between nodes
    public const float OFFSET = 0f;
    public const bool GENERATE_NEIGHBORS_ON_START = true;
    public static Dictionary<Vector2, AStarNavNode> map = new Dictionary<Vector2, AStarNavNode>();
    public static bool generated = false;
    public static bool allNeighborsGenerated = false;

    public Vector2 position;
    public bool neighborsGenerated = false;
    public List<AStarNavNode> neighbors = new List<AStarNavNode>();

    public bool traversable = true;

    public AStarNavNode(Vector2 pos)
    {
        position = pos;
        if (map.ContainsKey(pos))
        {
            Debug.LogWarning("Overwriting AStarNavNode!");
        }
        map[pos] = this;

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.up);
        if (hit.collider != null)
        {
            traversable = false;
        }
    }

    public void GenerateNeighbors()
    {
        Vector2 up = new Vector2(0, RESOLUTION);
        Vector2 down = new Vector2(0, -RESOLUTION);
        Vector2 right = new Vector2(RESOLUTION, 0);
        Vector2 left = new Vector2(-RESOLUTION, 0);

        if (map.ContainsKey(position + up))
        {
            TestNeighbor(map[position + up]);
        }
        if (map.ContainsKey(position + down))
        {
            TestNeighbor(map[position + down]);
        }
        if (map.ContainsKey(position + right))
        {
            TestNeighbor(map[position + right]);
        }
        if (map.ContainsKey(position + left))
        {
            TestNeighbor(map[position + left]);
        }
        neighborsGenerated = true;
    }

    void TestNeighbor(AStarNavNode neighbor)
    {
        Vector3 diff = (neighbor.position - position);
        RaycastHit2D hit = Physics2D.Raycast(position, diff, diff.magnitude, LayerMask.GetMask("Default"));
        if (hit.collider != null)
        {
            // Cannot reach
            return;
        }
        else
        {
            neighbors.Add(neighbor);
        }
    }

    public static void GenerateAllNeighbors()
    {
        if (!allNeighborsGenerated)
        {
            foreach (AStarNavNode node in map.Values)
            {
                node.GenerateNeighbors();
            }
            allNeighborsGenerated = true;
        }
    }

    public static void GenerateAllNodes()
    {
        if (!generated)
        {
            for (float x = -MAP_SIZE - OFFSET; x < MAP_SIZE + OFFSET; x += RESOLUTION)
            {
                for (float y = -MAP_SIZE - OFFSET; y < MAP_SIZE + OFFSET; y += RESOLUTION)
                {
                    new AStarNavNode(new Vector2(x, y));
                }
            }
            generated = true;
        }
    }

    public static AStarNavNode GetClosest(Vector3 position)
    {
        Vector2 flattened = (Vector2)position;
        float newX = Mathf.Round(flattened.x * 2f) / 2f;
        float newY = Mathf.Round(flattened.y * 2f) / 2f;
        Vector2 newVec = new Vector2(newX, newY);

        List<Vector2> sortedKeys = map.Keys.Where(x => Vector2.Distance(x, newVec) < 2f).ToList();
        sortedKeys = sortedKeys.OrderBy(x => Vector2.Distance(x, newVec)).ToList();

        //List<Vector2> sortedKeys = map.Keys.OrderBy(x => Vector2.Distance(x, newVec)).ToList();

        if (sortedKeys.Count > 0)
        {
            return map[sortedKeys[0]];
        }
        else
        {
            return null;
        }
    }
}