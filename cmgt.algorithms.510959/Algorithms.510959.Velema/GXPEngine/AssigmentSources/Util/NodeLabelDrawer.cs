﻿using GXPEngine;
using System;
using System.Drawing;

/**
 * Helper class that draws nodelabels for a nodegraph.
 */
class NodeLabelDrawer : Canvas
{
	static bool showedHowTo = false;
	private Font _labelFont;
	private bool _showLabels = false;
	private NodeGraph _graph = null;

	public NodeLabelDrawer(NodeGraph pNodeGraph) : base(pNodeGraph.width, pNodeGraph.height)
	{
		if (!showedHowTo)
		{
			Console.WriteLine("\n-----------------------------------------------------------------------------");
			Console.WriteLine("NodeLabelDrawer created.");
			Console.WriteLine("* L key to toggle node label display.");
			Console.WriteLine("-----------------------------------------------------------------------------");
			showedHowTo = true;
		}

		_labelFont = new Font(SystemFonts.DefaultFont.FontFamily, pNodeGraph.nodeSize * 2, FontStyle.Bold);
		_graph = pNodeGraph;
	}

	/////////////////////////////////////////////////////////////////////////////////////////
	///							Update loop
	///							

	//this has to be virtual otherwise the subclass won't pick it up
	protected virtual void Update()
	{
		//toggle label display when L is pressed
		if (Input.GetKeyDown(Key.L))
		{
			_showLabels = !_showLabels;
			graphics.Clear(Color.Transparent);
			if (_showLabels) drawLabels();
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////
	/// NodeGraph visualization helper methods

	protected virtual void drawLabels()
	{
		foreach (Node node in _graph.nodes) drawNode(node);
	}

	protected virtual void drawNode(Node pNode)
	{
		SizeF size = graphics.MeasureString(pNode.id, _labelFont);
		graphics.DrawString(pNode.id, _labelFont, Brushes.Black, pNode.location.X - size.Width - 5, pNode.location.Y - size.Height - 5);
	}

}
