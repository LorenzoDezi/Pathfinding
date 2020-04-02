using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{


    private class NodeInfo {
        public Connection connection;
        public float costSoFar;

        public NodeInfo(Connection connection, float costSoFar = 0f)
        {
            this.connection = connection;
            this.costSoFar = costSoFar;
        }

    };

    private static Dijkstra dijkstra;
    private Connection[] path = new Connection[0];

    private void Awake()
    {
        if (dijkstra == null)
            dijkstra = this;
        else
            Destroy(this);
    }

    /// <summary>
    /// Returns the node with the minimum cost
    /// </summary>
    /// <param name="status"> as the dictionary Node -> NodeInfo </param>
    /// <param name="unvisited"> as the nodes still to visit </param>
    /// <returns></returns>
    private static NodeComponent GetMinNode(Dictionary<NodeComponent, NodeInfo> status, List<NodeComponent> unvisited)
    {
        NodeComponent minNode = null;
        float minCost = Mathf.Infinity;
        foreach(var node in unvisited)
        {
            if(status[node].costSoFar < minCost || minNode == null)
            {
                minNode = node;
                minCost = status[node].costSoFar;
            }
        }
        return minNode;
    }

    public static void Solve(Graph graph, NodeComponent start, NodeComponent goal)
    {
        dijkstra.StartCoroutine(SolveCoroutine(graph, start, goal));
    }


    public static IEnumerator SolveCoroutine(Graph graph, NodeComponent start, NodeComponent goal)
    {
        var unvisited = new List<NodeComponent>(graph.Nodes);
        if (unvisited.Count == 0)
            yield break;
        //Initializing open and closed data structures
        //The visited node list
        var visited = new List<NodeComponent>();
        //The status dictionary with all informations about node
        var status = new Dictionary<NodeComponent, NodeInfo>();
        foreach(var node in unvisited)
            status.Add(node, new NodeInfo(null, node == start ? 0 : Mathf.Infinity));

        //The iteration begins
        NodeComponent currentNode = start;
        Connection[] currentConnections;
        while (unvisited.Count > 0)
        {
            currentNode = GetMinNode(status, unvisited);
            if (status[currentNode].costSoFar == Mathf.Infinity) {
                break; //the graph is partitioned
            }
            //BUG: sometimes cost and connection are not set for a node. Find out why
            currentNode.SetAsCurrent();
            yield return new WaitForSeconds(0.5f);
            //If it is the goal node, then terminate
            if (currentNode == goal) {
                break;
            } 
            //Otherwise, gets its ungoing connection
            currentConnections = graph.GetConnections(currentNode);
            foreach(var connection in currentConnections)
            {
                if (!visited.Contains(connection.To)) {
                    var nodeInfo = new NodeInfo(connection, status[connection.From].costSoFar + connection.Cost);
                    //if the current cost is less than the one of the current path, we don't need to go further
                    if (status[connection.To].costSoFar > nodeInfo.costSoFar)
                        status[connection.To] = nodeInfo;
                }
            }
            //We've finished looking at the connections for the current node, so add it to the closed list
            //and remove it from the open list
            unvisited.Remove(currentNode);
            visited.Add(currentNode);
            currentNode.Visit();
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
                connections.Add(status[currentNode].connection);
                Debug.Log(currentNode.name + "=> status: " + status[currentNode].connection);
                currentNode = status[currentNode].connection.From;
            }
            currentNode.MarkAsPath();
            //Reversing the path
            connections.Reverse();
            dijkstra.path = connections.ToArray();
        }
    }
}

