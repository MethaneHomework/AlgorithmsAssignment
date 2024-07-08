using GXPEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using static GXPEngine.Mathf;

internal class AStarStepped : SteppedPathFinder
{
	List<Node> expanded;
	Node current;

	Dictionary<Node, Node> explored;
	Dictionary<Node, float> gScore;
	Dictionary<Node, float> fScore;
	List<Node> frontier;

	public delegate float HeuristicDelegate(Node from, Node to);
	public HeuristicDelegate Heuristic;

	public AStarStepped(SampleDungeonNodeGraph pGraph) : base(pGraph)
	{
		_isSearching = false;

		_path = new List<Node>();
		expanded = new List<Node>();

		explored = new Dictionary<Node, Node>();
		gScore = new Dictionary<Node, float>();
		fScore = new Dictionary<Node, float>();
		frontier = new List<Node>();


	}

	new void Update()
	{
		base.Update();
		if (Input.GetKeyDown(Key.S)) Step();
	}

	protected override bool step()
	{
		if (!_isSearching)
		{
			graphics.Clear(Color.Transparent);
			_isSearching = true;

			expanded.Clear();
			explored.Clear();

			fScore.Clear();
			gScore.Clear();
			frontier.Clear();

			// Add first node to list
			expanded.Add(_startNode);
			explored.Add(_startNode, null);
			gScore.Add(_startNode, 0);
			float heuristicScore = Heuristic(_startNode, _endNode);
			fScore.Add(_startNode, heuristicScore);
			frontier.Add(_startNode);

			current = _startNode;
		}
		else
		{
			// Draw recently added nodes.
			// Note: This causes the purple color to be overwritten after only one frame.
			drawNodes(expanded, Brushes.SlateGray);
			expanded.Clear();

			drawNode(current, Brushes.DarkSlateGray);

			// Find the node with the lowest score
			float lowestScore = float.PositiveInfinity;
			foreach (Node node in frontier)
			{
				float score = fScore[node];
				if (score < lowestScore)
				{
					lowestScore = score;
					current = node;
				}
			}
			frontier.Remove(current);

			// Draw active node
			drawNode(current, Brushes.Turquoise);

			// Found a path.
			if (current == _endNode)
			{
				ConstructPath(current, explored);
				return true;
			}

			foreach (Node neighbor in current.connections)
			{
				if (!explored.ContainsKey(neighbor))
				{
					// Add the node since it has not been searched
					explored.Add(neighbor, current);
					
					gScore.Add(neighbor, float.PositiveInfinity);
					fScore.Add(neighbor, float.PositiveInfinity);
					frontier.Add(neighbor);
					expanded.Add(neighbor);
				}
				
				// The node has been searched but is this a better path?
				float gScoreTentative = gScore[current] + EuclideanDistance(current, neighbor);
				if (gScoreTentative < gScore[neighbor])
				{
					explored[neighbor] = current;

					gScore[neighbor] = gScoreTentative;
					fScore[neighbor] = gScoreTentative + Heuristic(neighbor, _endNode);

					if (!frontier.Contains(neighbor)) frontier.Add(neighbor);
					
					// Draw nodes where a better path was found.
					drawNode(neighbor, Brushes.LimeGreen);
				}
				
			}

			//SortQueue();
		}

		drawNodes(expanded, Brushes.MediumPurple);
		return false;
	}

	protected void ConstructPath(Node last, Dictionary<Node, Node> explored)
	{
		_path.Clear();
		Node current = last;

		// If current is pFrom then the path is complete.
		while (current != null)
		{
			_path.Add(current);
			current = explored[current];
		}
		// The list is created as path from pTo to pFrom so it has to be reversed first
		_path.Reverse();
	}

	public float OctileDistance(Node from, Node to)
	{
		SampleDungeonNodeGraph sampleNodeGraph = _nodeGraph as SampleDungeonNodeGraph;

		Point fromPoint = sampleNodeGraph.GetDungeonPoint(from.location);
		Point toPoint = sampleNodeGraph.GetDungeonPoint(to.location);

		int deltaX = Abs(fromPoint.X - toPoint.X);
		int deltaY = Abs(fromPoint.Y - toPoint.Y);

		int shortest = Min(deltaX, deltaY);
		int longest = Max(deltaX, deltaY);

		//const float sqrt2 = 1.4142f;
		//return sqrt2 * shortest + (longest - shortest);

		const float sqrt2min1 = 0.4142f;
		return sqrt2min1 * shortest + longest;
	}
	public float EuclideanDistance(Node from, Node to)
	{
		SampleDungeonNodeGraph sampleNodeGraph = _nodeGraph as SampleDungeonNodeGraph;

		Point fromPoint = sampleNodeGraph.GetDungeonPoint(from.location);
		Point toPoint = sampleNodeGraph.GetDungeonPoint(to.location);

		int deltaX = Abs(fromPoint.X - toPoint.X);
		int deltaY = Abs(fromPoint.Y - toPoint.Y);

		return Sqrt(deltaX * deltaX + deltaY * deltaY);
	}
	public float ManhattanDistance(Node from, Node to)
	{
		SampleDungeonNodeGraph sampleNodeGraph = _nodeGraph as SampleDungeonNodeGraph;

		Point fromPoint = sampleNodeGraph.GetDungeonPoint(from.location);
		Point toPoint = sampleNodeGraph.GetDungeonPoint(to.location);

		return Abs(fromPoint.X - toPoint.X) + Abs(fromPoint.Y - toPoint.Y);
	}
}
