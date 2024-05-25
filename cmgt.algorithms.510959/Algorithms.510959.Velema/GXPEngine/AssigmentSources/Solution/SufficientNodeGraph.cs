using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.AssigmentSources.Solution
{
	internal class SufficientNodeGraph : NodeGraph
	{
		Dungeon dungeon;

		public SufficientNodeGraph(Dungeon pDungeon) : base((int)(pDungeon.size.Width * pDungeon.scale), (int)(pDungeon.size.Height * pDungeon.scale), (int)pDungeon.scale / 3)
		{
			dungeon = pDungeon;

			OnNodeLeftClicked = (node) =>
			{
				Console.WriteLine(node + " connects to:");
				for (int i = 0; i < node.connections.Count - 1; i++)
				{
					Node n = node.connections[i];
					Console.WriteLine("\u251C {0}", n);
				}
				Console.WriteLine("\u2517 {0}", node.connections[node.connections.Count - 1]);

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
				Point point = new Point( 
					(int)( (door.location.X + 0.5f) * dungeon.scale ), 
					(int)( (door.location.Y + 0.5f) * dungeon.scale )
				);
				Node doorNode = new Node(point);

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
