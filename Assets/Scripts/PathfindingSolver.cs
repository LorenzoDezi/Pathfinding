using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PathfindingSolver : MonoBehaviour
{
    public abstract void Solve(Graph graph, NodeComponent start, NodeComponent goal);
}

