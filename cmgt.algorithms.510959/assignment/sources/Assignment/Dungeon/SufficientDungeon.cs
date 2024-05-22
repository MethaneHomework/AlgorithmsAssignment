using GXPEngine;
using System;
using System.Drawing;


internal class SufficientDungeon : Dungeon
{
	private const int seed = 2;
	private Random rnd;

	public SufficientDungeon(Size pSize) : base(pSize) { }

	protected override void generate(int pMinimumRoomSize)
	{
		rnd = new Random();

		Rectangle main = new Rectangle(1, 1, size.Width - 2, size.Height - 2);
		Rectangle a;
		Rectangle b;

		(a, b) = SplitRectangle(main, rnd);

		rooms.Add(new Room(a));
		rooms.Add(new Room(b));

		// Picture of assignment 1.1 clearly shows a BSP generated dungeon
		//
		// Create a rectangle that is the size of the dungeon
		// Pick a random axis (x or y)
		// Pick a random number that lies along the axis and fits in the rectangle
		// Create two new rectangles, one using the chosen point as it's limit and the other using that point + 1
		// Repeat for every rectangle until you have enough rooms


	}

	private (Rectangle a, Rectangle b) SplitRectangle(Rectangle rect, Random rnd)
	{
		// Pick random axis
		int axisPick = rnd.Next() % 2;

		Rectangle rectA = new Rectangle();
		Rectangle rectB = new Rectangle();

		if (axisPick == 0)
		{
			// Split horizontally
			rectA.Location = rect.Location;
			rectA.Height = rect.Height;
			rectB.Location = rect.Location;
			rectB.Height = rect.Height;

			int splitValue = rnd.Next(rect.Width);
			rectA.Width = splitValue;
			rectB.X += splitValue + 1;
			rectB.Width = rect.Width - splitValue - 1;
		}
		else
		{
			// Split vertically
			rectA.Location = rect.Location;
			rectA.Width = rect.Width;
			rectB.Location = rect.Location;
			rectB.Width = rect.Width;

			int splitValue = rnd.Next(rect.Height);
			rectA.Height = splitValue;
			rectB.Y += splitValue + 1;
			rectB.Height = rect.Width - splitValue - 1;
		}

		return (rectA, rectB);
	}
}

internal class RectangleBSPNode
{
	public Rectangle Self;
	public RectangleBSPNode ChildA;
	public RectangleBSPNode ChildB;

	public RectangleBSPNode(Rectangle rect, Random rnd)
	{
		Self = rect;

		// Pick random axis
		int axisPick = rnd.Next() % 2;

		Rectangle leafA = new Rectangle();
		Rectangle leafB = new Rectangle();

		if (axisPick == 0)
		{
			// Split horizontally
			leafA.Location = Self.Location;
			leafA.Height = Self.Height;
			leafB.Location = Self.Location;
			leafB.Height = Self.Height;

			int splitValue = rnd.Next(Self.Width);
			leafA.Width = splitValue;
			leafB.X += splitValue + 1;
			leafB.Width = Self.Width - splitValue - 1;
		}
		else
		{
			// Split vertically
			leafA.Location = Self.Location;
			leafA.Width = Self.Width;
			leafB.Location = Self.Location;
			leafB.Width = Self.Width;

			int splitValue = rnd.Next(Self.Height);
			leafA.Height = splitValue;
			leafB.Y += splitValue + 1;
			leafB.Height = Self.Width - splitValue - 1;
		}
	}
}

