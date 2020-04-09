using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PathfindingSolver : MonoBehaviour
{
    /// <summary>
    /// How much time the simulation should wait at each step of the algorithm?
    /// </summary>
    [SerializeField]
    protected float timeToWait = 0.5f;
    public abstract void Solve(Graph graph, NodeComponent start, NodeComponent goal);
}

