using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The graph represented as a set pair of V,E, represented respectly
/// as nodes and connections
/// </summary>
public class Graph
{
    private Dictionary<NodeComponent, List<Connection>> data = new Dictionary<NodeComponent, List<Connection>>();
    public NodeComponent[] Nodes { get => data.Keys.ToArray(); }

    public void AddNode(NodeComponent node)
    {
        if (!data.ContainsKey(node))
            data.Add(node, new List<Connection>());
    }

    public void AddConnection(Connection connection)
    {
        AddNode(connection.From);
        AddNode(connection.To);
        if (!data[connection.From].Contains(connection))
            data[connection.From].Add(connection);
    }

    public Connection[] GetConnections(NodeComponent n)
    {
        if (!data.ContainsKey(n)) return new Connection[0];
        else return data[n].ToArray();
    }

}

