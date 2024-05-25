﻿using System;
using System.Collections.Generic;
using System.Drawing;

internal class Area
{
	public Rectangle Rectangle;
	public Size Size => Rectangle.Size;
	public Point Location => Rectangle.Location;

	public int Width => Rectangle.Size.Width;
	public int Height => Rectangle.Size.Height;
	public int X => Rectangle.Location.X;
	public int Y => Rectangle.Location.Y;

	public Area(Rectangle rectangle, Area parent = null, Area childA = null, Area childB = null)
	{
		Rectangle = rectangle;
		Parent = parent;
		ChildA = childA;
		ChildB = childB;
	}

	// Parent area that contains this area
	public Area Parent;
	// Child areas "Sub areas"
	public Area ChildA;
	public Area ChildB;
	// Get the sibling of this area
	public Area Sibling
	{
		get
		{
			if (Parent == null) return null;
			else if (Parent.ChildA == this) return Parent.ChildB;
			else return Parent.ChildA;
		}
	}
	// Get leaves (nodes without children) under this area.
	public List<Area> Leaves
	{
		get
		{
			if (ChildA == null || ChildB == null)
			{
				// If any child is not assigned the area is regarded as a leaf and returns a new list containing itself;
				return new List<Area> { this };
			}
			else
			{
				List<Area> leaves = new List<Area>();
				// Recursively get leaves of child areas and append leaves to the list
				ChildA.Leaves.ForEach((area) => leaves.Add(area));
				ChildB.Leaves.ForEach((area) => leaves.Add(area));
				return leaves;
			}
		}
	}
	// Get all child pairs under this area
	public List<(Area a, Area b)> Pairs
	{
		get
		{
			List<(Area a, Area b)> pairs = new List<(Area a, Area b)>();

			if (!(ChildA == null || ChildB == null))
			{
				pairs.Add((ChildA, ChildB));
				ChildA.Pairs.ForEach((pair) => pairs.Add(pair));
				ChildB.Pairs.ForEach((pair) => pairs.Add(pair));
			}

			return pairs;
		}
	}

	// =================================================
	//		Methods
	// =================================================
	public void Split(int minSize, int maxDepth, Random rng, int depth = 0)
	{
		SplitMode splitMode;
		(bool horizontal, bool vertical) = CanSplit(minSize);

		// Sets the splitmode according to how an area can be split
		if (horizontal && vertical)
		{
			// Split along longest dimension or random if they are equal
			if (Width > Height) splitMode = SplitMode.horizontal;
			else if (Height > Width) splitMode = SplitMode.vertical;
			else
			{
				// Pick random axis to split
				int axis = rng.Next() % 2;
				if (axis == 0) splitMode = SplitMode.horizontal;
				else splitMode = SplitMode.vertical;
			}
		}
		else if (horizontal) splitMode = SplitMode.horizontal;
		else if (vertical) splitMode = SplitMode.vertical;
		else return; // Area cannot be split

		Rectangle rectA;
		Rectangle rectB;
		if (splitMode == SplitMode.horizontal)
		{
			// Split horizontally
			int splitVal = rng.Next(minSize, Width - minSize);
			rectA = new Rectangle(Rectangle.Left, Rectangle.Top, splitVal, Rectangle.Height);
			rectB = new Rectangle(Rectangle.Left + splitVal - 1, Rectangle.Top, Rectangle.Width - splitVal + 1, Rectangle.Height);
		}
		else
		{
			// Split vertically
			int splitVal = rng.Next(minSize, Height - minSize);
			rectA = new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, splitVal);
			rectB = new Rectangle(Rectangle.Left, Rectangle.Top + splitVal - 1, Rectangle.Width, Rectangle.Height - splitVal + 1);
		}

		ChildA = new Area(rectA, this);
		ChildB = new Area(rectB, this);

		if (depth < maxDepth)
		{
			ChildA.Split(minSize, maxDepth, rng, depth + 1);
			ChildB.Split(minSize, maxDepth, rng, depth + 1);
		}
	}

	// Enum for better readability of splitcode
	protected enum SplitMode
	{
		vertical,
		horizontal,
	}

	// Checks if an area is big enough to be split in either direction
	protected (bool horizontal, bool vertical) CanSplit(int minSize)
	{
		minSize = 2 * minSize + 1;
		return (Size.Width > minSize, Size.Height > minSize);
	}
}

/*
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Dungeons
{
	// Areas form a BSP and thus can be split into smaller areas
	internal class Area : BSPNode
	{
		private const int MIN_AREA_SIZE = Dungeon.MIN_AREA_SIZE;
		private const int MIN_ROOM_SIZE = Dungeon.MIN_ROOM_SIZE;

		public Rectangle Rectangle;
		public int Width => Size.Width;
		public int Height => Size.Height;
		public Size Size => Rectangle.Size;

		public Area(Rectangle rectangle, Area parent = null)
		{
			Rectangle = rectangle;
			Parent = parent;
		}
		public void Split(Random rng)
		{
			(bool horizontal, bool vertical) = CanSplit();

			Rectangle areaA;
			Rectangle areaB;
			SplitMode splitMode;

			// Sets the splitmode according to how an area can be split
			if (horizontal && vertical)
			{
				// Split along longest dimension or random if they are equal
				if (Width > Height) splitMode = SplitMode.horizontal;
				else if (Height > Width) splitMode = SplitMode.vertical;
				else
				{
					// Pick random axis to split
					int axis = rng.Next() % 2;
					if (axis == 0) splitMode = SplitMode.horizontal;
					else splitMode = SplitMode.vertical;
				}
			}
			else if (horizontal) splitMode = SplitMode.horizontal;
			else if (vertical) splitMode = SplitMode.vertical;
			else return; // Area cannot be split

			if (splitMode == SplitMode.horizontal)
			{
				// Split horizontally
				int splitVal = rng.Next(MIN_AREA_SIZE, Size.Width - MIN_AREA_SIZE);
				areaA = new Rectangle(Rectangle.Left, Rectangle.Top, splitVal, Rectangle.Height);
				areaB = new Rectangle(Rectangle.Left + splitVal - 1, Rectangle.Top, Rectangle.Width - splitVal + 1, Rectangle.Height);
			}
			else
			{
				// Split vertically
				int splitVal = rng.Next(MIN_AREA_SIZE, Size.Height - MIN_AREA_SIZE);
				areaA = new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, splitVal);
				areaB = new Rectangle(Rectangle.Left, Rectangle.Top + splitVal - 1, Rectangle.Width, Rectangle.Height - splitVal + 1);
			}

			ChildA = new Area(areaA, this);
			ChildB = new Area(areaB, this);

			return;
		}
		public void Split(int maxDepth, Random rng)
		{
			int depth = 0;
			while (depth < maxDepth)
			{
				foreach (Area leaf in this[depth].Cast<Area>())
				{
					leaf.Split(rng);
				}
				depth++;
			}
		}

		protected enum SplitMode
		{
			vertical,
			horizontal,
		}
		// Checks if an area is big enough to be split in either direction
		protected (bool horizontal, bool vertical) CanSplit()
		{
			int minSize = 2 * Dungeon.MIN_AREA_SIZE + 1;
			return (Size.Width > minSize, Size.Height > minSize);
		}
	}
}
*/