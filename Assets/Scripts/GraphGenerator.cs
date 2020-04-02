using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour
{
    /// <summary>
    /// Random seed used to generate random edges between the nodes 
    /// </summary>
    [SerializeField]
    private int randomSeed = 0;

    /// <summary>
    /// The prefab used to represent visually a node in the graph
    /// </summary>
    [SerializeField]
    private GameObject nodeGameObj;

    /// <summary>
    /// The prefab used to represent visually a connection in the graph
    /// </summary>
    [SerializeField]
    private GameObject connGameObj;

    /// <summary>
    /// The probabiliy that an edge between two node will appear
    /// </summary>
    [SerializeField]
    private float edgeProbability = .5f;

    /// <summary>
    /// x dimension of the node grid
    /// </summary>
    [SerializeField]
    private int x = 100;

    /// <summary>
    /// y dimension of the node grid
    /// </summary>
    [SerializeField]
    private int y = 100;

    /// <summary>
    /// The matrix representing the grid of nodes
    /// </summary>
    private NodeComponent[,] matrix;

    /// <summary>
    /// maximum gap between each node in the matrix
    /// </summary>
    [SerializeField]
    private float maxGap;

    /// <summary>
    /// minimum gap between each node in the matrix
    /// </summary>
    [SerializeField]
    private float minGap;

    private Graph graph = new Graph();

    void Start()
    {
        if (nodeGameObj == null) return;
        if (randomSeed == 0) randomSeed = (int)System.DateTime.Now.Ticks;
        UnityEngine.Random.InitState(randomSeed);
        matrix = CreateGrid();
        CreateConnections();
        Dijkstra.Solve(graph, matrix[0, 0], matrix[x-1, y-1]);
    }

    private NodeComponent[,] CreateGrid()
    {
        matrix = new NodeComponent[x, y];
        for(int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject nodeRepr = Instantiate(nodeGameObj);
                nodeRepr.transform.position = transform.right * UnityEngine.Random.Range(minGap, maxGap) * i +
                    transform.forward * UnityEngine.Random.Range(minGap, maxGap) * j;
                nodeRepr.transform.rotation = Quaternion.identity;
                nodeRepr.name = string.Format("Node[i = {0}, j = {1}]", i, j);
                matrix[i, j] = nodeRepr.GetComponent<NodeComponent>();
            }
        }           
        return matrix;
    }

    private float Distance(NodeComponent from, NodeComponent to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position);
    }

    private void OnDrawGizmos()
    {
        if (matrix != null && graph != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    foreach(var connection in graph.GetConnections(matrix[i, j]))
                    {
                        var from = connection.From.transform.position;
                        var to = connection.To.transform.position;
                        Gizmos.DrawLine(from, to);
                    }
                }
            }
        }
    }

    private void CreateConnections()
    {
        NodeComponent currFrom;
        List<NodeComponent> neighbors = new List<NodeComponent>();
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                currFrom = matrix[i, j];
                neighbors.Clear();
                if (i > 0) neighbors.Add(matrix[i - 1, j]);
                if (j > 0) neighbors.Add(matrix[i, j - 1]);
                if (j < y - 1) neighbors.Add(matrix[i, j + 1]);
                if (i < x - 1) neighbors.Add(matrix[i + 1, j]);
                foreach(var neighbor in neighbors)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < edgeProbability)
                    {
                        graph.AddConnection(new Connection(currFrom, neighbor, Distance(currFrom, neighbor)));
                        var connRenderer = Instantiate(connGameObj).GetComponent<LineRenderer>();
                        connRenderer.positionCount = 2;
                        //TODO: Find a way to render connections properly
                        connRenderer.SetPosition(0, currFrom.transform.position + new Vector3(0, -neighbor.transform.localScale.y / 2));
                        connRenderer.SetPosition(1, neighbor.transform.position + new Vector3(0, neighbor.transform.localScale.y / 2));                        
                    }
                }
                
            }
        }
    }

}
