using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


public class HeuristicNodeInfo : NodeInfo
{
    public float estimatedTotalCost;

    public HeuristicNodeInfo(Connection connection, float estimatedCost = 0, float costSoFar = 0) 
        : base(connection, costSoFar)
    {
        this.estimatedTotalCost = estimatedCost;
    }
}
