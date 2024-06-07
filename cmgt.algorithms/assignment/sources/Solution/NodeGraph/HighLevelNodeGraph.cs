using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class HighLevelNodeGraph : SampleNodeGraph
{
	public HighLevelNodeGraph(Dungeon dungeon) : base(dungeon)
	{
		_dungeon = dungeon;
	}

	protected override void generate()
	{
		Dictionary<Room, Node> roomNodeDict = new Dictionary<Room, Node>();

		foreach (Room room in _dungeon.rooms)
		{
			Node node = new Node(getRoomCenter(room));
			nodes.Add(node);
			roomNodeDict.Add(room, node);
		}
		foreach (Door door in _dungeon.doors)
		{
			Node node = new Node(getDoorCenter(door));
			nodes.Add(node);
			AddConnection(node, roomNodeDict[door.roomA]);
			AddConnection(node, roomNodeDict[door.roomB]);
		}
	}
}
