using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Category
{
    Open = 0, Closed = 1, Unvisited = 2
}

public class NodeInfo
{
    public Connection connection;
    public float costSoFar;
    public Category category = Category.Unvisited;

    public NodeInfo(Connection connection, float costSoFar = 0f)
    {
        this.connection = connection;
        this.costSoFar = costSoFar;
    }
}

