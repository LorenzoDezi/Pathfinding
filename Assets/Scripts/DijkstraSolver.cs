using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DijkstraSolver : PathfindingSolver
{
    private Connection[] path = new Connection[0];

    /// <summary>
    /// Returns the node with the minimum cost
    /// </summary>
    /// <param name="status"> as the dictionary Node -> NodeInfo </param>
    /// <param name="unvisited"> as the nodes still to visit </param>
    /// <returns></returns>
    private NodeComponent GetMinNode(List<NodeComponent> unvisited)
    {
        NodeComponent minNode = null;
        float minCost = Mathf.Infinity;
        foreach(var node in unvisited)
        {
            if(node.NodeInfo.costSoFar < minCost || minNode == null)
            {
                minNode = node;
                minCost = node.NodeInfo.costSoFar;
            }
        }
        return minNode;
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
        foreach(var node in unvisited)
           node.NodeInfo = new NodeInfo(null, node == start ? 0 : Mathf.Infinity);
        //The iteration begins
        NodeComponent currentNode = null;
        start.NodeInfo.category = Category.Open;
        open.Add(start);
        while (open.Count > 0)
        {
            currentNode = GetMinNode(open);
            currentNode.MarkAsCurrent();
            yield return new WaitForSeconds(0.5f);
            //If it is the goal node, then terminate - following the book algorithm
            if (currentNode == goal) {
                break;
            } 
            //Otherwise, gets its ungoing connection
            foreach(var connection in graph.GetConnections(currentNode))
            {
                var category = connection.To.NodeInfo.category;
                var nodeInfo = new NodeInfo(connection, connection.From.NodeInfo.costSoFar + connection.Cost);
                if (category == Category.Open) {
                    //if the current cost is less than the one of the current path, we don't need to go further
                    if (connection.To.NodeInfo.costSoFar > nodeInfo.costSoFar)
                        connection.To.NodeInfo = nodeInfo;
                } else if (category == Category.Unvisited)
                {
                    nodeInfo.category = Category.Open;
                    open.Add(connection.To);
                    connection.To.NodeInfo = nodeInfo;
                }
            }
            yield return new WaitForSeconds(0.5f);
            //We've finished looking at the connections for the current node, so add it to the closed list
            //and remove it from the open list
            var currNodeInfo = currentNode.NodeInfo;
            currNodeInfo.category = Category.Closed;
            currentNode.NodeInfo = currNodeInfo;
            open.Remove(currentNode);
            yield return new WaitForSeconds(0.5f);
        }
        //We've here if we've either found a goal, or if we've no more nodes to search, find which
        if (currentNode == goal)
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
    }
}

