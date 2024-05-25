using System.Collections.Generic;
using System.Drawing;

/**
 * This class represents (the data for) a Room, at this moment only a rectangle in the dungeon.
 */
class Room
{
	public Rectangle area;

	// Additions to original
	public readonly List<Door> doors;

	public readonly int ID;
	private static int lastID = 0;

	public Room(Rectangle pArea)
	{
		area = pArea;
		doors = new List<Door>();
		ID = lastID++;
	}

	//TODO: Implement a toString method for debugging?
	//Return information about the type of object and it's data
	//eg Room: (x, y, width, height)
	public override string ToString()
	{
		return 
			$"Room {ID}:\n" +
			$"  Location: ({area.Location})\n" +
			$"  Size: ({area.Size})";
	}
}
