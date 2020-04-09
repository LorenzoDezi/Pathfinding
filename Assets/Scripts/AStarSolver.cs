using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum HeuristicType
{
    Euclidean = 0
}

public delegate float AStarHeuristic(NodeComponent start, NodeComponent goal);

public class AStarSolver : PathfindingSolver
{
    private Connection[] path = new Connection[0];

    //Heuristic setup
    [SerializeField]
    private HeuristicType heuristicType;
    private Dictionary<HeuristicType, AStarHeuristic> heuristicFuncDict 
        = new Dictionary<HeuristicType, AStarHeuristic>();

    private void Awake()
    {
        heuristicFuncDict.Add(HeuristicType.Euclidean, 
            (start, goal) => { return (goal.transform.position - start.transform.position).magnitude; });
    }

    public override void Solve(Graph graph, NodeComponent start, NodeComponent goal)
    {
        StartCoroutine(SolveCoroutine(graph, start, goal));
    }

    public IEnumerator SolveCoroutine(Graph graph, NodeComponent start, NodeComponent goal)
    {
        var unvisited = new List<NodeComponent>(graph.Nodes);
        if (unvisited.Count == 0)
            yield break;
        //Initializing open data structure
        //Using a proerty of nodeInfo we don't need a close list, avoiding searching
        var open = new List<NodeComponent>();
        //Every node will start with an infinity cost except the start node
        foreach (var node in unvisited)
        {
            bool isStart = node == start;
            node.NodeInfo = new HeuristicNodeInfo(null, 
                isStart ? heuristicFuncDict[heuristicType](start, goal) : Mathf.Infinity, 
                isStart ? 0 : Mathf.Infinity);
        }
        //The iteration begins
        NodeComponent currentNode = null;
        start.NodeInfo.category = Category.Open;
        open.Add(start);
        while (open.Count > 0)
        {
            currentNode = GetMinNode(open);
            currentNode.MarkAsCurrent();
            yield return new WaitForSeconds(timeToWait);
            //If it is the goal node, then terminate - following the book algorithm
            if (currentNode == goal)
                break;
            //Otherwise, gets its ungoing connection
            foreach (var connection in graph.GetConnections(currentNode))
                ProcessConnection(goal, open, currentNode, connection);
            yield return new WaitForSeconds(timeToWait);
            //We've finished looking at the connections for the current node, so add it to the closed list
            //and remove it from the open list
            var currNodeInfo = currentNode.NodeInfo;
            currNodeInfo.category = Category.Closed;
            currentNode.NodeInfo = currNodeInfo;
            open.Remove(currentNode);
            yield return new WaitForSeconds(timeToWait);
        }
        //We've here if we've either found a goal, or if we've no more nodes to search, find which
        if (currentNode == goal)
            CalculatePath(start, currentNode);
    }

    private void CalculatePath(NodeComponent start, NodeComponent currentNode)
    {
        //Traversing the path from goal to start
        List<Connection> connections = new List<Connection>();
        while (currentNode != start)
        {
            currentNode.MarkAsPath();
            connections.Add(currentNode.NodeInfo.connection);
            currentNode = currentNode.NodeInfo.connection.From;
        }
        currentNode.MarkAsPath();
        //Reversing the path
        connections.Reverse();
        path = connections.ToArray();
    }

    private void ProcessConnection(NodeComponent goal, List<NodeComponent> open, 
        NodeComponent fromNode, Connection connection)
    {
        var toNode = connection.To;
        var toNodeInfo = toNode.NodeInfo as HeuristicNodeInfo;
        HeuristicNodeInfo newNodeInfo = new HeuristicNodeInfo(
            connection, Mathf.Infinity, fromNode.NodeInfo.costSoFar + connection.Cost);
        float endNodeHeuristic;
        if (toNodeInfo.category == Category.Closed)
        {
            //If the node is closed, we need to update its cost
            //only when it is greater then the one of the current path
            //we also need to add the node again on the open list, because we should
            //update its neighbors
            if (toNodeInfo.costSoFar > newNodeInfo.costSoFar)
            {
                newNodeInfo.category = Category.Open;
                open.Add(toNode);
                toNode.NodeInfo = GetUpdateNodeInfo(toNodeInfo, newNodeInfo);
            }
        }
        else if (toNodeInfo.category == Category.Open)
        {
            //If the node is opened, we should update its cost
            //only when it is greater then the one of the current path
            if (toNodeInfo.costSoFar > newNodeInfo.costSoFar)
                toNode.NodeInfo = GetUpdateNodeInfo(toNodeInfo, newNodeInfo);
        }
        else
        {
            //the node is unvisited: we calculate its heuristic and add it 
            //to the open list
            endNodeHeuristic = heuristicFuncDict[heuristicType](toNode, goal);
            newNodeInfo.estimatedTotalCost = newNodeInfo.costSoFar + endNodeHeuristic;
            newNodeInfo.category = Category.Open;
            toNode.NodeInfo = newNodeInfo;
            open.Add(toNode);
        }
    }

    private HeuristicNodeInfo GetUpdateNodeInfo(HeuristicNodeInfo oldNodeInfo, HeuristicNodeInfo newNodeInfo)
    {
        newNodeInfo.estimatedTotalCost = newNodeInfo.costSoFar +
                                    (oldNodeInfo.estimatedTotalCost - oldNodeInfo.costSoFar); //the heuristic is already calculated
        return newNodeInfo;
    }

    private NodeComponent GetMinNode(List<NodeComponent> open)
    {
        NodeComponent candidate = null;
        HeuristicNodeInfo currNodeInfo = null;
        float minCost = float.MaxValue;
        foreach(var node in open)
        {
            currNodeInfo = node.NodeInfo as HeuristicNodeInfo;
            if(candidate == null || minCost > currNodeInfo.estimatedTotalCost)
            {
                candidate = node;
                minCost = currNodeInfo.estimatedTotalCost;
            }
        }
        Debug.Log(minCost);
        return candidate;
    }
}
