using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class GoodDungeon : SufficientDungeon
{
	public GoodDungeon(Size pSize) : base(pSize) {}

	protected override void generate(int pMinimumRoomSize)
	{
		// Generate sufficient dungeon
		base.generate(pMinimumRoomSize);

		Room smallestRoom = null;
		int smallestSize = int.MaxValue;
		Room biggestRoom = null;
		int biggestSize = int.MinValue;

		foreach (Room room in rooms)
		{
			int size = room.area.Width * room.area.Height;
			if (size < smallestSize)
			{
				smallestSize = size;
				smallestRoom = room;
			}
			if (size > biggestSize)
			{
				biggestSize = size;
				biggestRoom = room;
			}
		}

		rooms.Remove(smallestRoom);
		foreach (Door door in smallestRoom.doors)
		{
			doors.Remove(door);
			if (door.roomA == smallestRoom) door.roomB.doors.Remove(door);
			else door.roomA.doors.Remove(door);
		}
		rooms.Remove(biggestRoom);
		foreach (Door door in biggestRoom.doors)
		{
			doors.Remove(door);
			if (door.roomA == biggestRoom) door.roomB.doors.Remove(door);
			else door.roomA.doors.Remove(door);
		}
	}

	protected override void drawRooms(IEnumerable<Room> pRooms, Pen pWallColor, Brush pFillColor = null)
	{
		foreach (Room room in pRooms)
		{
			Brush fillColor;
			switch (room.doors.Count)
			{
				case 0:
					fillColor = Brushes.Red;
					break;
				case 1:
					fillColor = Brushes.Orange;
					break;
				case 2:
					fillColor = Brushes.Yellow;
					break;
				case 3:
					fillColor = Brushes.Green;
					break;
				default:
					fillColor = Brushes.Purple;
					break;
			}
			drawRoom(room, pWallColor, fillColor);
		}
	}
}
