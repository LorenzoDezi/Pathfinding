using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NodeComponent : MonoBehaviour
{
    [SerializeField]
    private Material openMat;
    [SerializeField]
    private Material closedMat;
    [SerializeField]
    private Material unvisitedMat;
    [SerializeField]
    private Material currentMat;
    [SerializeField]
    private Material pathMat;

    private MeshRenderer meshRenderer;
    private NodeInfo nodeInfo;
    private Dictionary<Category, Material> categoryMatDict = new Dictionary<Category, Material>();

    public NodeInfo NodeInfo
    {
        get => nodeInfo;
        set => SetNodeInfo(value);
    }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = unvisitedMat;
        categoryMatDict.Add(Category.Open, openMat);
        categoryMatDict.Add(Category.Closed, closedMat);
        categoryMatDict.Add(Category.Unvisited, unvisitedMat);
    }

    private void SetNodeInfo(NodeInfo newInfo)
    {
        this.nodeInfo = newInfo;
        meshRenderer.material = categoryMatDict[newInfo.category];
    }

    public void MarkAsPath()
    {
        meshRenderer.material = pathMat;
    }

    public void MarkAsCurrent()
    {
        meshRenderer.material = currentMat;
    }
}