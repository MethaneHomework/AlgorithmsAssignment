using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;

internal class BSPNode
{
	public Rectangle Bounds;
	public BSPNode Parent;
	public BSPNode ChildA, ChildB;

	public Room Room;

	public BSPNode(Rectangle bounds, BSPNode parent = null)
	{
		Bounds = bounds;
		Parent = parent;
	}

	public bool HasChildren => ChildA != null && ChildB != null;
	public bool ContainsPoint(Point point) => Bounds.Contains(point);

	public ImmutableList<BSPNode> LeafNodes
	{
		get
		{
			if (HasChildren)
			{
				List<BSPNode> leaves = new List<BSPNode>();
				leaves.AddRange(ChildA?.LeafNodes);
				leaves.AddRange(ChildB?.LeafNodes);
				return leaves.ToImmutableList();
			}
			else return new List<BSPNode>() { this }.ToImmutableList();
		}
	}
	public ImmutableList<Rectangle> LeafBounds
	{
		get
		{
			if (HasChildren)
			{
				List<Rectangle> bounds = new List<Rectangle>();
				bounds.AddRange(ChildA?.LeafBounds);
				bounds.AddRange(ChildB?.LeafBounds);
				return bounds.ToImmutableList();
			}
			else return new List<Rectangle>() { Bounds }.ToImmutableList();
		}
	}

	public BSPNode FindPoint(Point point)
	{
		if (ContainsPoint(point))
		{
			if (HasChildren)
			{
				// Check children
				if (ChildA != null && ChildA.ContainsPoint(point))
				{
					// A contains the point, search A
					return ChildA.FindPoint(point);
				}
				else if (ChildB != null && ChildB.ContainsPoint(point))
				{
					// B contains the point, search B
					return ChildB.FindPoint(point);
				}
				else
				{
					// A special case, the point is contained in the tree but not in the leaf nodes.
					// This results from the way the splits are made, sibling nodes get 1 tiles space between them, these are considered walls during generation.
					return null;
				}
			}
			else
			{
				// This is a leaf node and it contains the point
				return this;
			}
		}
		else return null;
	}

	// Node is split into left and right node
	private void SplitHorizontal(int x)
	{
		if (x < Bounds.Left) throw new Exception();
		Rectangle boundsA = new Rectangle(Bounds.Location, new Size(x - Bounds.Left - 1, Bounds.Height));
		Rectangle boundsB = new Rectangle(x, Bounds.Y, Bounds.Width - boundsA.Width - 1, Bounds.Height);

		ChildA = new BSPNode(boundsA, this);
		ChildB = new BSPNode(boundsB, this);
	}
	private void SplitVertical(int y)
	{
		Rectangle boundsA = new Rectangle(Bounds.Location, new Size(Bounds.Width, y - Bounds.Top - 1));
		Rectangle boundsB = new Rectangle(Bounds.X, y, Bounds.Width, Bounds.Height - boundsA.Height - 1);

		ChildA = new BSPNode(boundsA, this);
		ChildB = new BSPNode(boundsB, this);
	}
	public bool Split(int pMinRoomSize, Random rng)
	{
		int minBounds = pMinRoomSize * 2 + 3;
		if (Bounds.Width > Bounds.Height && Bounds.Width > minBounds)
		{
			// Split horizontally
			int x = rng.Next(Bounds.Left + pMinRoomSize + 1, Bounds.Right - pMinRoomSize - 1);
			SplitHorizontal(x);
			return true;
		}
		else if (Bounds.Height > minBounds)
		{
			// Split vertically
			int y = rng.Next(Bounds.Top + pMinRoomSize + 1, Bounds.Bottom - pMinRoomSize - 1);
			SplitVertical(y);
			return true;
		}
		else return false;
	}
	public void SplitRecursive(int desiredDepth, int pMinRoomSize, Random rng)
	{
		if (Split(pMinRoomSize, rng) && desiredDepth != 0)
		{
			// Split successful and depth has not been reached
			ChildA.SplitRecursive(desiredDepth - 1, pMinRoomSize, rng);
			ChildB.SplitRecursive(desiredDepth - 1, pMinRoomSize, rng);
		}
	}

	public BSPNode Sibling
	{
		get
		{
			if (Parent != null)
			{
				return Parent.ChildA == this ? Parent.ChildB : Parent.ChildA;
			}
			else return null;
		}
	}
	public ImmutableList<(BSPNode a, BSPNode b)> SiblingPairs
	{
		get
		{
			if (HasChildren)
			{
				List<(BSPNode a, BSPNode b)> pairs = new List<(BSPNode a, BSPNode b)>() { (ChildA, ChildB) };
				pairs.AddRange(ChildA?.SiblingPairs);
				pairs.AddRange(ChildB?.SiblingPairs);
				return pairs.ToImmutableList();
			}
			else return new List<(BSPNode a, BSPNode b)>().ToImmutableList();
		}
	}
}
