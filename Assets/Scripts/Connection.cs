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
}

