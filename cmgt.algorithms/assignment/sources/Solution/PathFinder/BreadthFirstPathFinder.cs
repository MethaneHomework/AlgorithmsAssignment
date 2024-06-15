using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BreadthFirstPathFinder : PathFinder
{
	Dictionary<Node, Node> _explored = new Dictionary<Node, Node>();
	Queue<Node> _queue = new Queue<Node>();

	public BreadthFirstPathFinder(NodeGraph pGraph) : base(pGraph) { }

	protected override List<Node> generate(Node pFrom, Node pTo)
	{
		// Empty collections
		_explored.Clear();
		_queue.Clear();
		
		// mark pFrom as explored with no previous node since it's the start of the path.
		_queue.Enqueue(pFrom);
		_explored.Add(pFrom, null);

		while (_queue.Count > 0)
		{
			Node node = _queue.Dequeue();
			if (node != pTo)
			{
				foreach (Node connection in node.connections)
				{
					if (!_explored.ContainsKey(connection))
					{
						_queue.Enqueue(connection);
						_explored.Add(connection, node);
					}
				}
			}
			else
			{
				List<Node> path = new List<Node>();
				Node current = node;
				
				// If current is pFrom then the path is complete.
				while (current != null)
				{
					path.Add(current);
					current = _explored[current];
				}
				// The list is created as path from pTo to pFrom so it has to be reversed first
				path.Reverse();
				return path;
			}
		}
		// No path found
		return new List<Node>();
	}
}
