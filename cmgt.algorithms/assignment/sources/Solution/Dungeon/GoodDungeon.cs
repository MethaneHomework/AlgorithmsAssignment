using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class GoodDungeon : SufficientDungeon
{
	public bool RemoveMinMax = true;
	public bool ConnectionCountColor = true;

	public GoodDungeon(Size pSize) : base(pSize) { }

	protected override void generate(int pMinimumRoomSize)
	{
		base.generate(pMinimumRoomSize);

		if (RemoveMinMax) RemoveMinMaxRooms();
	}

	private void RemoveMinMaxRooms()
	{
		int minArea = int.MaxValue;
		Room minRoom = null;

		int maxArea = int.MinValue;
		Room maxRoom = null;

		foreach (Room room in rooms)
		{
			int area = room.area.Size.Width * room.area.Size.Height;
			if (area < minArea)
			{
				minArea = area;
				minRoom = room;
			}
			if (area > maxArea)
			{
				maxArea = area;
				maxRoom = room;
			}
		}

		RemoveRoom(minRoom);
		RemoveRoom(maxRoom);
	}
	protected void RemoveRoom(Room room)
	{
		rooms.Remove(room);
		foreach (Door door in room.doors)
		{
			doors.Remove(door);
			bool _ = door.roomA != room ? door.roomA.doors.Remove(door) : door.roomB.doors.Remove(door);
		}
	}

	protected override void drawRooms(IEnumerable<Room> pRooms, Pen pWallColor, Brush pFillColor = null)
	{
		if (pFillColor != null)
		{
			base.drawRooms(pRooms, pWallColor, pFillColor);
			return;
		}

		foreach (Room room in pRooms)
		{
			Brush fillColor;
			switch (room.doors.Count)
			{
				case 0:
					// red
					fillColor = Brushes.Red;
					break;
				case 1:
					// orange
					fillColor = Brushes.Orange;
					break;
				case 2:
					// yellow
					fillColor = Brushes.Yellow;
					break;
				case 3:
					// green
					fillColor = Brushes.Green;
					break;
				default:
					// purple
					fillColor = Brushes.Purple;
					break;
			}

			drawRoom(room, pWallColor, fillColor);
		}
	}
}
