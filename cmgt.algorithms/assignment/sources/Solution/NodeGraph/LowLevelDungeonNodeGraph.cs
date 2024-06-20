using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

internal class LowLevelDungeonNodeGraph : SampleDungeonNodeGraph
{
	public LowLevelDungeonNodeGraph(Dungeon pDungeon) : base(pDungeon)
	{
		_dungeon = pDungeon;
	}

	protected override void generate()
	{
		Node[,] nodesPosition = new Node[_dungeon.size.Width, _dungeon.size.Width];
		
		foreach (Room room in _dungeon.rooms)
		{
			PlaceRoomNodes(room, nodesPosition);
		}
		foreach (Door door in _dungeon.doors)
		{
			PlaceDoorNode(door, nodesPosition);
		}

		Hashtable visitedNodes = new Hashtable();
		Queue<Node> fillQueue = new Queue<Node>();
		fillQueue.Enqueue(nodes[0]);
		visitedNodes.Add(nodes[0], null);

		while (fillQueue.Count > 0)
		{
			Node node = fillQueue.Dequeue();

			Point nodePoint = getDungeonPoint(node.location);
			Debug.Assert(node == nodesPosition[nodePoint.X, nodePoint.Y]);

			CheckNode(node, nodesPosition, fillQueue, visitedNodes,  0, -1);	// top node
			CheckNode(node, nodesPosition, fillQueue, visitedNodes, -1,  0);	// left node
			CheckNode(node, nodesPosition, fillQueue, visitedNodes,  0,  1);	// right node
			CheckNode(node, nodesPosition, fillQueue, visitedNodes,  1,  0);	// bottom node
		}
	}

	private void CheckNode(Node node, Node[,] nodePositionArray, Queue<Node> queue, Hashtable visited, int dX, int dY)
	{
		Point nodePoint = getDungeonPoint(node.location);
		Node otherNode = nodePositionArray[nodePoint.X + dX, nodePoint.Y + dY];
		if (otherNode == null) return;

		AddConnection(node, otherNode);
		if (!visited.ContainsKey(otherNode))
		{
			visited.Add(otherNode, null);
			queue.Enqueue(otherNode);
		}
	}

	private void PlaceRoomNodes(Room room, Node[,] nodePositionArray)
	{
		Rectangle roomInternal = room.internalArea;
		for (int i = roomInternal.Left; i < roomInternal.Right; i++)
		{
			for (int j = roomInternal.Top; j < roomInternal.Bottom; j++)
			{
				Node node = NodeFromDungeonPoint(new Point(i, j));
				nodes.Add(node);
				nodePositionArray[i, j] = node;
			}
		}
	}

	private void PlaceDoorNode(Door door, Node[,] nodesPositionArray)
	{
		Node node = NodeFromDungeonPoint(door.location);
		nodes.Add(node);
		nodesPositionArray[door.location.X, door.location.Y] = node;
	}

	private Node NodeFromDungeonPoint(Point point) => new Node(getPointCenter(point));

	public override string ToString()
	{
		return base.ToString();
	}
}
