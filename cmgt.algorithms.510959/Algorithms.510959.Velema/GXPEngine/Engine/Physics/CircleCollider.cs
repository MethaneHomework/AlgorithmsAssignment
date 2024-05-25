using static GXPEngine.Mathf;

namespace GXPEngine.Physics
{
	internal class CircleCollider : ACollider
	{
		public float Radius;

		public CircleCollider(Vector2 center, float radius, PhysicsObject owner) : base(owner)
		{
			Position = center;
			Radius = radius;
			LastCollision = new CollisionInfo();
		}

		public override (float min, float max) MinMaxBounds(Vector2 axis)
		{
			float a = Vector2.Dot(Position, axis);
			return (a - Radius, a + Radius);
		}

		public override Vector2 NormalAt(Vector2 point)
		{
			return (point - Position).Normalized();
		}

		public override bool Overlapping(ACollider other)
		{
			if (other is CircleCollider) return Overlapping(other as CircleCollider);
			else if (other is OBCollider) return Overlapping(other as OBCollider);
			else return false;
		}
		private bool Overlapping(OBCollider other)
		{
			CollisionInfo bestCol = null;

			for (int i = 0; i < other.Normals.Length; i++)
			{
				Vector2 norm = other.Normals[i];

				bool overlaps = other.OverlappingOnAxis(norm, this, out CollisionInfo col);
				if (bestCol == null || col.Depth < bestCol.Depth) bestCol = col;
				if (!overlaps) return false;
			}

			Vector2 circleAxis = (other.Position - Position).Normalized();
			bool circleOverlaps = other.OverlappingOnAxis(circleAxis, this, out CollisionInfo circleCol);
			if (bestCol == null || circleCol.Depth < bestCol.Depth) bestCol = circleCol;
			if (!circleOverlaps) return false;

			bestCol.Normal = -bestCol.Normal;
			LastCollision = bestCol;
			return true;
		}
		private bool Overlapping(CircleCollider other)
		{
			float distance = (Position - other.Position).Length() - Radius - other.Radius;
			if (distance <= 0)
			{
				CollisionInfo col = new CollisionInfo
				{
					Normal = -(Position - other.Position).Normalized(),
					Depth = -distance,
					Other = other
				};
				LastCollision = col;
				return true;
			}
			else return false;
		}

		public override float RayCast(Ray ray)
		{
			Vector2 relative = Position - ray.Origin;

			float a = Vector2.Dot(ray.Direction, ray.Direction);
			float b = Vector2.Dot(-2 * ray.Direction, relative);
			float c = Vector2.Dot(relative, relative) - Radius * Radius;
			float D = MathUtils.GetDiscriminant(a, b, c);
			if (D < 0) return float.PositiveInfinity;

			// Get roots
			(float rootA, float rootB) = MathUtils.GetRoots(D, a, b);
			float t = Min(rootA, rootB);

			if (t < 0)
			{
				// Earliest root is invalid, check other root
				t = Max(rootA, rootB);
				if (t < 0)
				{
					return float.PositiveInfinity;
				}
			}

			return t;
		}

		protected override void Invalidate()
		{

		}
	}
}
