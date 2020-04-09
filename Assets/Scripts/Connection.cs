/// <summary>
/// It represents an Edge in the graph
/// </summary>
public class Connection
{
    private float cost;
    private NodeComponent from, to;

    public NodeComponent From { get => from; }
    public NodeComponent To { get => to; }
    public float Cost { get => cost; }

    public Connection(NodeComponent from, NodeComponent to, float cost)
    {
        this.cost = cost;
        this.from = from;
        this.to = to;
    }

    public bool IsOpposite(Connection conn)
    {
        return conn.from.GetInstanceID() == this.to.GetInstanceID() && conn.to.GetInstanceID() == this.From.GetInstanceID();
    }
}

