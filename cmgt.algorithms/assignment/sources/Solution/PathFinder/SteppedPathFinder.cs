using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;

internal abstract class SteppedPathFinder : PathFinder
{
	protected List<Node> _path;
	protected bool _isSearching;
	public bool IsSearching => _isSearching;

	public EventHandler<List<Node>> FoundPath;

	public SteppedPathFinder(NodeGraph pGraph) : base(pGraph)
	{
		pGraph.OnNodeShiftLeftClicked += (n) =>
		{
			Cancel();
		};
		pGraph.OnNodeShiftRightClicked += (n) =>
		{
			Cancel();
		};
	}

	protected override List<Node> generate(Node pFrom, Node pTo)
	{
		if (step()) return _lastCalculatedPath;

		else return null;
	}

	public bool Step()
	{
		if (!_isSearching) return false;
		if (step())
		{
			_isSearching = false;
			_lastCalculatedPath = _path;
			for (int i = 0; i < _path.Count - 1; i++)
			{
				Node a = _path[i];
				Node b = _path[i + 1];
				drawConnection(a, b);
			}
			//drawNodes(_path, Brushes.Yellow);

			FoundPath.Invoke(this, _path);
			return true;
		}
		return false;
	}
	protected abstract bool step();

	protected override void handleInput()
	{
		if (Input.GetKeyDown(Key.C))
		{
			//clear everything
			graphics.Clear(Color.Transparent);
			_startNode = _endNode = null;
			_lastCalculatedPath = null;
		}
		if (Input.GetKeyDown(Key.G))
		{
			if (!IsSearching && _startNode != null && _endNode != null) Generate(_startNode, _endNode);
			else Step();
		}
	}
	public void Cancel() => _isSearching = false;
}
