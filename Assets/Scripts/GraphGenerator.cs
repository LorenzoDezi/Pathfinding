﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

//Graph generator state variables. The Graph generator solves
//the path only when the start and the end nodes are chosen
public enum GraphGenState
{
    NoNodeChosen = 0, StartChosen = 1, EndChosen = 2
}

public class GraphGenStateChangedEvent : UnityEvent<GraphGenState> { }

public class GraphGenerator : MonoBehaviour
{
    
    private GraphGenState state = GraphGenState.NoNodeChosen;
    private delegate void NodeChosenDelegate(NodeComponent node);
    private Dictionary<GraphGenState, NodeChosenDelegate> stateOnClickDict = 
        new Dictionary<GraphGenState, NodeChosenDelegate>();

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
    private NodeComponent start;
    private NodeComponent end;

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
    private PathfindingSolver solver;
    private Camera mainCamera;

    public GraphGenStateChangedEvent StateChangedEvent = new GraphGenStateChangedEvent();

    void Start()
    {
        if (nodeGameObj == null) return;
        if (randomSeed == 0) randomSeed = (int)System.DateTime.Now.Ticks;
        UnityEngine.Random.InitState(randomSeed);
        matrix = CreateGrid();
        CreateConnections();
        solver = GetComponent<PathfindingSolver>();
        mainCamera = Camera.main;
        stateOnClickDict.Add(GraphGenState.NoNodeChosen, (node) => {
            start = node;
            state = GraphGenState.StartChosen;
            StateChangedEvent.Invoke(state);
        });
        stateOnClickDict.Add(GraphGenState.StartChosen, (node) => {
            end = node;
            state = GraphGenState.EndChosen;
            StateChangedEvent.Invoke(state);
            Solve();
        });
    }

    private void Solve()
    {
        if (solver != null && start != null && end != null)
            solver.Solve(graph, start, end);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && state != GraphGenState.EndChosen)
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if(Physics.Raycast(ray.origin, ray.direction, out info, Mathf.Infinity))
            {
                var nodeComp = info.collider.GetComponent<NodeComponent>();
                if (nodeComp != null)
                    stateOnClickDict[state](nodeComp);
            }
        }
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
                        var connection = new Connection(currFrom, neighbor, Distance(currFrom, neighbor));
                        //Offset used to render differently the second connection between two nodes
                        float yOffset = 0f;
                        if (graph.GetConnections(connection.To).Any(c => c.To == connection.From))
                            yOffset = 0.15f;
                        graph.AddConnection(connection);
                        RenderConnection(currFrom, neighbor, yOffset);
                    } else
                        graph.AddNode(currFrom);
                }
                
            }
        }
    }

    private void RenderConnection(NodeComponent currFrom, NodeComponent neighbor, float yOffset)
    {
        var connRenderer = Instantiate(connGameObj).GetComponent<LineRenderer>();
        connRenderer.positionCount = 2;
        connRenderer.SetPosition(0, currFrom.transform.position + new Vector3(0f, yOffset, 0f));
        connRenderer.SetPosition(1, neighbor.transform.position + new Vector3(0f, yOffset, 0f));
    }
}
