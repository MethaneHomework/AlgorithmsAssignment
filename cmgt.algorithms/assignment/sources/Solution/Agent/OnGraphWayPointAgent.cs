using GXPEngine;
using System.Collections.Generic;

internal class OnGraphWayPointAgent : NodeGraphAgent
{
	protected Node _last;
	protected Queue<Node> _targets = new Queue<Node>();

	public OnGraphWayPointAgent(NodeGraph pNodeGraph) : base(pNodeGraph)
	{
		if (pNodeGraph.nodes.Count > 0)
		{
			//_last = pNodeGraph.nodes[Utils.Random(0, pNodeGraph.nodes.Count)];
			SafeJump(pNodeGraph.nodes[Utils.Random(0, pNodeGraph.nodes.Count)]);
		}

		pNodeGraph.OnNodeLeftClicked += OnNodeLeftClicked;
		pNodeGraph.OnNodeShiftLeftClicked += OnNodeShiftLeftClicked;
	}

	protected virtual void OnNodeLeftClicked(Node node)
	{
		if (_last.connections.Contains(node))
		{
			_targets.Enqueue(node);
			_last = node;
		}
	}
	protected virtual void OnNodeShiftLeftClicked(Node node)
	{
		SafeJump(node);
	}

	protected override void Update()
	{
		if (_targets.Count == 0) return;

		// Move towards the first node in the queue
		if (moveTowardsNode(_targets.Peek()))
		{
			// Remove node from queue if we reach it
			_targets.Dequeue();
		}
	}

	protected void SafeJump(Node pNode)
	{
		jumpToNode(pNode);
		_targets.Clear();
		_last = pNode;
	}
}

