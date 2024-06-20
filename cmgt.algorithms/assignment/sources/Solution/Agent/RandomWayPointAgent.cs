using GXPEngine;


internal class RandomWayPointAgent : OnGraphWayPointAgent
{
	public RandomWayPointAgent(NodeGraph pNodeGraph) : base(pNodeGraph)
	{
	}

	protected override void Update()
	{
		base.Update();

		if (_targets.Count == 0)
		{
			_targets.Enqueue(_last.connections[Utils.Random(0, _last.connections.Count)]);
			_last = _targets.Peek();
		}
	}
}

