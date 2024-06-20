using GXPEngine;
using System;
using System.Collections.Generic;


internal class PathFindingAgent : NodeGraphAgent
{
	protected Node _last;
	protected Node _target;
	protected NodeGraph _graph;
	protected Queue<Node> _path = new Queue<Node>();
	protected PathFinder _pathFinder;

	public bool Wandering = false;

	public PathFindingAgent(NodeGraph pNodeGraph, PathFinder pPathFinder) : base(pNodeGraph)
	{
		SafeJump(pNodeGraph.nodes[Utils.Random(0, pNodeGraph.nodes.Count)]);
		_pathFinder = pPathFinder;
		_graph = pNodeGraph;

		pNodeGraph.OnNodeShiftLeftClicked += node =>
		{
			SafeJump(node);
		};
		pNodeGraph.OnNodeShiftRightClicked += node =>
		{
			_target = node;
		};
	}

	protected override void Update()
	{
		if (Input.GetKeyDown(Key.G) && _target != null)
		{
			List<Node> pathOrEmpty = _pathFinder.Generate(_last, _target);
			if (pathOrEmpty != null && pathOrEmpty.Count > 0)
			{
				_last = _target;
				pathOrEmpty.ForEach(x => _path.Enqueue(x));
			}
		}
		if (Input.GetKeyDown(Key.W))
		{
			Wandering = !Wandering;
			if (Wandering) Console.WriteLine("Agent is now wandering.");
			else Console.WriteLine("Agent stopped wandering.");
		}

		if (_path.Count == 0)
		{
			if (Wandering)
			{
				_target = _graph.nodes[Utils.Random(0, _graph.nodes.Count)];
				List<Node> pathOrEmpty = _pathFinder.Generate(_last, _target);
				if (pathOrEmpty != null && pathOrEmpty.Count > 0)
				{
					_last = _target;
					pathOrEmpty.ForEach(x => _path.Enqueue(x));
				}
			}
			return;
		}

		if (moveTowardsNode(_path.Peek()))
		{
			_path.Dequeue();
		}
	}

	private void SetTarget(Node node)
	{
		_target = node;
	}

	private void SafeJump(Node node)
	{
		jumpToNode(node);
		_last = node;
		_path.Clear();
	}
}

