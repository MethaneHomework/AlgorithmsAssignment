using GXPEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

internal class AStarPathfinder : PathFinder
{
	public AStarPathfinder(NodeGraph pGraph) : base(pGraph)
	{

	}

	protected override List<Node> generate(Node pFrom, Node pTo)
	{
		if (pFrom == pTo) return null;

		int nodesChecked = 0;

		Dictionary<Node, Node> explored = new Dictionary<Node, Node>();
		Dictionary<Node, float> pathCosts = new Dictionary<Node, float>();
		Dictionary<Node, float> estimatedCosts = new Dictionary<Node, float>();
		List<Node> queue = new List<Node>();

		float costEstimate = OctileDistance(pFrom, pTo);
		Console.WriteLine("Taxicab distance: {0}", TaxicabDistance(pFrom, pTo));
		Console.WriteLine("Not Chebyshev distance: {0}", OctileDistance(pFrom, pTo));

		explored.Add(pFrom, null);
		pathCosts.Add(pFrom, 0);
		estimatedCosts.Add(pFrom, costEstimate);
		queue.Add(pFrom);

		while (queue.Count > 0)
		{
			nodesChecked++;
			Node currentNode = queue[0];

			queue.Remove(currentNode);

			if (currentNode == pTo)
			{
				float finalCost = pathCosts[currentNode];
				Console.WriteLine("Final path cost: {0}", finalCost);
				Console.WriteLine("Deviation from estimate: {0}", finalCost - costEstimate);
				Debug.Assert(finalCost >= costEstimate);

				Console.WriteLine("Nodes checked: {0}", nodesChecked);
				return ConstructPath(currentNode, explored);
			}

			foreach (Node nextNode in currentNode.connections)
			{
				float heuristicCost = OctileDistance(nextNode, pTo);
				float pathCost = pathCosts[currentNode] + Distance(currentNode, nextNode);
				float totalCost = heuristicCost + pathCost;

				if (!explored.ContainsKey(nextNode))
				{
					explored.Add(nextNode, currentNode);
					pathCosts.Add(nextNode, pathCost);
					estimatedCosts.Add(nextNode, totalCost);
					queue.Add(nextNode);
				}
				else if (pathCost < pathCosts[nextNode])
				{
					explored[nextNode] = currentNode;
					pathCosts[nextNode] = pathCost;
					estimatedCosts[nextNode] = totalCost;
					if (!queue.Contains(nextNode)) queue.Add(nextNode);
				}
			}

			queue.Sort(new Comparison<Node>((a, b) =>
			{
				if (estimatedCosts[a] < estimatedCosts[b]) return -1;
				else if (estimatedCosts[a] > estimatedCosts[b]) return 1;
				else return 0;
			}));
		}

		return new List<Node>();
	}

	private List<Node> ConstructPath(Node node, Dictionary<Node, Node> explored)
	{
		List<Node> path = new List<Node>() { node };
		while (explored[node] != null)
		{
			node = explored[node];
			path.Add(node);
		}
		path.Reverse();
		return path;
	}

	protected virtual float Distance(Node from, Node to)
	{
		float xSquared = from.location.X - to.location.X;
		xSquared *= xSquared;
		float ySquared = from.location.Y - to.location.Y;
		ySquared *= ySquared;
		return Mathf.Sqrt(xSquared + ySquared);
	}
	protected virtual float TaxicabDistance(Node from, Node to)
	{
		return Mathf.Abs(from.location.X - to.location.X) + Mathf.Abs(from.location.Y - to.location.Y);
	}

	protected float OctileDistance(Node from, Node to)
	{
		float horizontal = Mathf.Abs(from.location.X - to.location.X);
		float vertical = Mathf.Abs(from.location.Y - to.location.Y);

		float shortest = Mathf.Min(horizontal, vertical);
		float longest = Mathf.Max(horizontal, vertical);

		return Mathf.Sqrt(2) * shortest + longest - shortest;
	}
}
