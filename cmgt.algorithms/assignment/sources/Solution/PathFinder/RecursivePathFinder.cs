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

	private List<Node> DepthFirstSearch(Node current, Node target, int maxDepth, Node last = null)
	{
		// If path is found, return list containing path.
		// If no path is found return empty list

		if (current == target) return new List<Node>() { current };
		if (maxDepth <= 0) return new List<Node>();

		foreach (Node node in current.connections)
		{
			// If checked node is the previous node, skip that node.
			if (node != null && node == last) continue;

			List<Node> path = DepthFirstSearch(node, target, maxDepth - 1, current);
			if (path.Count > 0)
			{
				List<Node> finalPath = new List<Node>() { current };
				finalPath.AddRange(path);
				return finalPath;
			}
		}

		return new List<Node>();
	}
}

