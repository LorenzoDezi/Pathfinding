using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NodeComponent : MonoBehaviour
{
    /// <summary>
    /// Dijkstra is currently analyzing the connections of this node
    /// </summary>
    [SerializeField]
    private Material currentMat;
    /// <summary>
    /// Dijkstra is currently analyzing the node as a connection of the current one
    /// </summary>
    [SerializeField]
    private Material connMat;
    private Material prevMat;
    /// <summary>
    /// The node is still not visited by dijkstra
    /// </summary>
    [SerializeField]
    private Material unvisitedMat;
    /// <summary>
    /// The node is visited by dijkstra
    /// </summary>
    [SerializeField]
    private Material visitedMat;
    /// <summary>
    /// The node is a part of the path towards the destination
    /// </summary>
    [SerializeField]
    private Material pathMat;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = unvisitedMat;
    }

    public void Visit()
    {
        meshRenderer.material = visitedMat;
    }

    public void SetAsCurrent()
    {
        meshRenderer.material = currentMat;
    }

    public void ReleaseAsConnection()
    {
        if(prevMat != null)
            meshRenderer.material = prevMat;
    }

    public void MarkAsPath()
    {
        meshRenderer.material = pathMat;
    }

    internal void SetAsConnection()
    {
        prevMat = meshRenderer.material;
        meshRenderer.material = connMat;
    }
}
