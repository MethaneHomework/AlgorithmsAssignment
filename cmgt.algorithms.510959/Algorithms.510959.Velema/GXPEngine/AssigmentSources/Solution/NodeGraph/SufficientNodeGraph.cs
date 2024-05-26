using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace GXPEngine.AssigmentSources.Solution
{
	internal class SufficientNodeGraph : NodeGraph
	{
		private readonly Dungeon dungeon;

		public SufficientNodeGraph(Dungeon pDungeon) : base((int)(pDungeon.size.Width * pDungeon.scale), (int)(pDungeon.size.Height * pDungeon.scale), (int)pDungeon.scale / 3)
		{
			dungeon = pDungeon;

			OnNodeLeftClicked = (node) =>
			{
				if (node.connections.Count > 0)
				{
					Debug.WriteLineIf(SufficientDungeon.informativeOutput.Enabled, 
						node + " connects to:");
					for (int i = 0; i < node.connections.Count - 1; i++)
					{
						Node n = node.connections[i];
						Debug.WriteLineIf(SufficientDungeon.informativeOutput.Enabled,
							$"\u251C{n}");
					}
					Debug.WriteLineIf(SufficientDungeon.informativeOutput.Enabled, $"\u2514{node.connections[node.connections.Count - 1]}");
				}
				else
				{
					Debug.WriteLineIf(SufficientDungeon.informativeOutput.Enabled, node + " has no connections.");
				}
			};
		}

		protected override void generate()
		{
			Dictionary<int, Node> roomNodes = new Dictionary<int, Node>();

			foreach (Room room in dungeon.rooms)
			{
				Point point = new Point( 
					(int)( (room.area.Left + room.area.Right + 0.5) / 2 * dungeon.scale ), 
					(int)( (room.area.Top + room.area.Bottom + 0.5) / 2 * dungeon.scale )
				);

				Node roomNode = new Node(point);
				roomNodes.Add(room.ID, roomNode);
				nodes.Add(roomNode);
			}
			foreach (Door door in dungeon.doors)
			{
				//Point point = new Point( 
				//	(int)( (door.location.X + 0.5f) * dungeon.scale ),
				//	(int)( (door.location.Y + 0.5f) * dungeon.scale )
				//);
				Node doorNode = new Node(door, dungeon.scale);

				Node roomA = roomNodes[door.roomA.ID];
				roomA.connections.Add(doorNode);
				doorNode.connections.Add(roomA);

				Node roomB = roomNodes[door.roomB.ID];
				roomB.connections.Add(doorNode);
				doorNode.connections.Add(roomB);

				nodes.Add(doorNode);
			}
		}
	}
}
