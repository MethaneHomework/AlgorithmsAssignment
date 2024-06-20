internal class TiledDungeonView : TiledView
{
	Dungeon _dungeon;
	TileType _defaultTile;

	public TiledDungeonView(Dungeon pDungeon, TileType pDefaultTileType) : base(pDungeon.size.Width, pDungeon.size.Height, (int)pDungeon.scale, pDefaultTileType)
	{
		_dungeon = pDungeon;
		_defaultTile = pDefaultTileType;
	}

	protected override void generate()
	{
		placeRooms();
		placeDoors();
	}

	private void placeRooms()
	{
		foreach (Room room in _dungeon.rooms)
		{
			placeRoom(room);
		}
	}

	protected void placeRoom(Room room)
	{
		for (int x = room.area.Left; x < room.area.Right; x++)
		{
			for (int y = room.area.Top; y < room.area.Bottom; y++)
			{
				if (x == room.area.Left || x == room.area.Right - 1 || y == room.area.Top || y == room.area.Bottom - 1)
				{
					SetTileType(x, y, TileType.WALL);
				}
				else
				{
					SetTileType(x, y, TileType.GROUND);
				}
			}
		}
	}

	protected void placeDoors()
	{
		foreach (Door door in _dungeon.doors)
		{
			SetTileType(door.location.X, door.location.Y, TileType.GROUND);
		}
	}
}
