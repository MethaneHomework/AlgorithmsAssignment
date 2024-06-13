using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class PathFindingAgent : NodeGraphAgent
{
	protected Node _last;
	protected Node _target;
	protected Queue<Node> _path = new Queue<Node>();
	protected PathFinder _pathFinder;

	public PathFindingAgent(NodeGraph pNodeGraph, PathFinder pPathFinder) : base(pNodeGraph)
	{
		SafeJump(pNodeGraph.nodes[Utils.Random(0, pNodeGraph.nodes.Count)]);
		_pathFinder = pPathFinder;

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
		
		if (_path.Count == 0) return;

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

