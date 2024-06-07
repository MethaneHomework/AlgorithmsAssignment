using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GXPEngine.Mathf;

internal class ExcellentDungeon : GoodDungeon
{
	public ExcellentDungeon(Size pSize) : base(pSize) { }

	protected override void generate(int pMinimumRoomSize)
	{
		ConnectionCountColor = false;
		RemoveMinMax = false;

		base.generate(pMinimumRoomSize);

		ResizeRooms(pMinimumRoomSize);
	}

	protected void ResizeRooms(int pMinimumRoomSize)
	{
		foreach (Room room in rooms)
		{
			ResizeRoom(room, pMinimumRoomSize);
		}
	}
	protected void ResizeRoom(Room room, int pMinRoomSize)
	{
		int width = rng.Next(Max(room.area.Width - pMinRoomSize, pMinRoomSize), room.area.Width);
		int height = rng.Next(Max(room.area.Height - pMinRoomSize, pMinRoomSize), room.area.Height);
		int x = rng.Next(room.area.Left, room.area.Right - width);
		int y = rng.Next(room.area.Top, room.area.Bottom - height);
		room.area = new Rectangle(x, y, width, height);
	}
	protected void ReplaceDoors()
	{
		foreach (Door door in doors)
		{
			ReplaceDoor(door);
		}
	}
	protected void ReplaceDoor(Door door)
	{
		Rectangle rectA = door.roomA.area;
		Rectangle rectB = door.roomB.area;
		if (door.horizontal)
		{
			int minY = Max(rectA.Top, rectB.Top);
			int maxY = Min(rectA.Bottom, rectB.Bottom);
			door.location.Y = rng.Next(minY, maxY);
		}
		else
		{
			int minX = Max(rectA.Left, rectB.Left);
			int maxX = Min(rectA.Right, rectB.Right);
			door.location.X = rng.Next(minX, maxX);
		}
	}

	protected override void draw()
	{
		//graphics.Clear(Color.FromArgb(200, 200, 200, 200));
		if (ConnectionCountColor)
		{
			drawRooms(rooms, wallPen);
		}
		else
		{
			drawRooms(rooms, wallPen, Brushes.Transparent);
		}
		drawDoors(doors, doorPen);
	}
	protected override void drawDoor(Door pDoor, Pen pColor)
	{
		if (pDoor.horizontal) {
			graphics.DrawRectangle(pColor, pDoor.location.X - 2, pDoor.location.Y, 2.5f, 0.5f);
		}
		else
		{
			graphics.DrawRectangle(pColor, pDoor.location.X, pDoor.location.Y -2, 0.5f, 2.5f);
		}
	}
}
