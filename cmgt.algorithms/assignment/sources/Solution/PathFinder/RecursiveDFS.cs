﻿using System;
using System.Collections.Generic;
using Random = System.Random;

internal class RecursiveDFS : PathFinder
{
	int nodesChecked;

	public RecursiveDFS(NodeGraph pGraph) : base(pGraph)
	{

	}

	protected override List<Node> generate(Node pFrom, Node pTo)
	{
		nodesChecked = 0;

		HashSet<Node> visited = new HashSet<Node>();
		List<Node> path = DepthFirstSearch(pFrom, pTo, 1000, visited);
		Console.WriteLine("DFS checked {0} nodes", nodesChecked);
		return path;
	}

	private List<Node> DepthFirstSearch(Node current, Node target, int maxDepth, HashSet<Node> visited)
	{
		// If path is found, return list containing path.
		// If no path is found return empty list

		nodesChecked++;
		if (current == target) return new List<Node>() { current };
		if (maxDepth <= 0) return new List<Node>();

		foreach (Node node in current.connections)
		{
			// If checked node is the previous node, skip that node.
			if (node != null && visited.Contains(node)) continue;

			visited.Add(node);
			List<Node> path = DepthFirstSearch(node, target, maxDepth - 1, visited);

			if (path.Count > 0)
			{
				path.Insert(0, current);
				return path;
			}
		}

		return new List<Node>();
	}

}

