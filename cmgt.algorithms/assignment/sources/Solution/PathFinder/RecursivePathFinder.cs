using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class RecursivePathFinder : PathFinder
{
	public RecursivePathFinder(NodeGraph pGraph) : base(pGraph)
	{

	}

	protected override List<Node> generate(Node pFrom, Node pTo)
	{
		return DepthFirstSearch(pFrom, pTo, 50);
	}

	private List<Node> DepthFirstSearch(Node pFrom, Node pTo, int maxDepth, Node pLastNode = null) {
		foreach (Node node in pFrom.connections)
		{
			if (node == pTo)
			{
				// Checked connection is target, return path
				return new List<Node>{pFrom, node};
			}
			else if (maxDepth > 0)
			{
				List<Node> pathOrNull = DepthFirstSearch(node, pTo, maxDepth - 1);

				if (pathOrNull != null && pathOrNull.Count != 0)
				{
					// Path was found, 
					List<Node> path = new List<Node>() { pFrom };
					path.AddRange(pathOrNull);
					return path;
				}
			}
		}

		// Did not return in loop, no path found
		return new List<Node>();
	}

	// DFS
	/*
	Depth first search
	Check 

	*/
}

