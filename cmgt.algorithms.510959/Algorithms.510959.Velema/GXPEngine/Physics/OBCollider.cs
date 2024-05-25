using static GXPEngine.Mathf;

namespace GXPEngine.Physics
{
	/// <summary>
	/// Oriented Box collider. Specifically not called OBB because it doesn't have bounding box functionality as of yet (if ever).
	/// </summary>
	internal partial class OBCollider : ACollider
	{
		// Size field can't change, this to prevent unpredictable behavior.
		public readonly Vector2 Size;

		// Corners property
		public Vector2[] Corners
		{
			get
			{
				if (!_cornersValid)
				{
					_corners = Vector2.RotateVectorsDeg(baseCorners, Angle);
					_cornersValid = true;
				}
				return _corners;
			}
		}
		private readonly Vector2[] baseCorners;
		private Vector2[] _corners;
		bool _cornersValid = false;

		// Normals property
		public Vector2[] Normals
		{
			get
			{
				if (!_normalsValid)
				{
					_normals = Vector2.RotateVectorsDeg(baseNormals, Angle);
					_normalsValid = true;
				}
				return _normals;
			}
		}
		private readonly Vector2[] baseNormals;
		private Vector2[] _normals;
		private bool _normalsValid = false;

		// ===================================
		// Overlap checks
		// ===================================
		public OBCollider(Vector2 position, Vector2 size, float angle, PhysicsObject owner) : base(owner)
		{
			Position = position;
			Size = size;
			Angle = angle;

			LastCollision = new CollisionInfo();

			// Construct corner offsets
			float hWidth = Size.X / 2f;
			float hHeight = Size.Y / 2f;

			Vector2 cornerA = new Vector2(hWidth, hHeight);
			Vector2 cornerB = new Vector2(-hWidth, hHeight);
			Vector2 cornerC = new Vector2(-hWidth, -hHeight);
			Vector2 cornerD = new Vector2(hWidth, -hHeight);
			baseCorners = new Vector2[4] { cornerA, cornerB, cornerC, cornerD };

			// Construct normals
			// NOTE: Normals for a box are very easy but for studying purposes the normals are defined by the edges instead. This will be changed later.
			Vector2 normalA = (cornerA - cornerB).Normal();
			Vector2 normalB = (cornerB - cornerC).Normal();
			Vector2 normalC = (cornerC - cornerD).Normal();
			Vector2 normalD = (cornerD - cornerA).Normal();
			baseNormals = new Vector2[4] { normalA, normalB, normalC, normalD };
		}
		public override bool Overlapping(ACollider other)
		{
			bool result = false;
			if (other is OBCollider) result = Overlapping(other as OBCollider);
			else if (other is CircleCollider) result = Overlapping(other as CircleCollider);

			return result;
		}
		// Overlap test specifically for other boxes
		private bool Overlapping(OBCollider other)
		{
			CollisionInfo bestCol = null;

			// Axis checks for this box
			for (int i = 0; i < Normals.Length; i++)
			{
				Vector2 norm = Normals[i];

				bool overlaps = OverlappingOnAxis(norm, other, out CollisionInfo col);
				if (bestCol == null || col.Depth < bestCol.Depth) bestCol = col;
				if (!overlaps) return false;
			}
			// Axis checks for the other box
			for (int i = 0; i < other.Normals.Length; i++)
			{
				Vector2 norm = other.Normals[i];

				bool overlaps = other.OverlappingOnAxis(norm, this, out CollisionInfo col);
				if (bestCol == null || col.Depth < bestCol.Depth) bestCol = col;
				if (!overlaps) return false;
			}
			bestCol.Normal *= -1;
			bestCol.Other = other;
			LastCollision = bestCol;
			return true;
		}
		private bool Overlapping(CircleCollider other)
		{
			CollisionInfo bestCol = null;

			for (int i = 0; i < Normals.Length; i++)
			{
				Vector2 norm = Normals[i];

				bool overlaps = OverlappingOnAxis(norm, other, out CollisionInfo col);
				if (bestCol == null || col.Depth < bestCol.Depth) bestCol = col;
				if (!overlaps) return false;
			}

			Vector2 circleAxis = (other.Position - Position).Normalized();
			bool circleOverlaps = OverlappingOnAxis(circleAxis, other, out CollisionInfo circleCol);
			if (bestCol == null || circleCol.Depth < bestCol.Depth) bestCol = circleCol;
			if (!circleOverlaps) return false;

			bestCol.Normal *= -1;
			LastCollision = bestCol;
			return true;
		}
		// Checks if the box overlaps with other box along a given axis
		public bool OverlappingOnAxis(Vector2 axis, ACollider other, out CollisionInfo colInfo)
		{
			float pDepth;
			Vector2 normal;
			bool collides;

			axis.Normal();

			(float pMinThis, float pMaxThis) = MinMaxBounds(axis);
			(float pMinOther, float pMaxOther) = other.MinMaxBounds(axis);

			if (pMinThis < pMinOther && pMaxThis > pMinOther)
			{
				// "this" is overlapping on the left(negative) side of "other"
				normal = axis;
				pDepth = pMaxThis - pMinOther;
				collides = true;
			}
			else if (pMinOther < pMinThis && pMaxOther > pMinThis)
			{
				// "this" is overlapping on the right(positive) side of "other"
				normal = -axis;
				pDepth = pMaxOther - pMinThis;
				collides = true;
			}
			else
			{
				normal = new Vector2();
				pDepth = 0;
				collides = false;
			}

			// Exception for same angle boxes
			if (other is OBCollider && Angle == other.Angle)
			{
				normal = -normal;
			}

			colInfo = new CollisionInfo(normal, pDepth);
			return collides;
		}
		/// <summary>
		/// Gets the minimum and maximum points along a given axis
		/// </summary>
		/// <param name="axis">Axis to compare the points on</param>
		/// <returns>A tuple containing the min and max vectors and projected values</returns>
		public override (float min, float max) MinMaxBounds(Vector2 axis)
		{
			float pMin = float.PositiveInfinity;
			float pMax = float.NegativeInfinity;

			for (int i = 0; i < Corners.Length; i++)
			{
				Vector2 v = Corners[i] + Position;
				float p = Vector2.Dot(v, axis);

				if (p < pMin)
				{
					pMin = p;
				}
				if (p > pMax)
				{
					pMax = p;
				}
			}

			return (pMin, pMax);
		}


		// =====================================================
		// Raycast and normal calculation
		// =====================================================
		public override float RayCast(Ray ray)
		{
			// Steps for raycasting
			// Translate origin and box so that box is at origin.
			// Rotate direction and box so that box angle is 0
			// Check raycast as if box is AABB

			// Since the size gives the measurements of the box we don't have to rotate the box
			Vector2 rOrigin = ray.Origin - Position;
			rOrigin = rOrigin.RotatedDeg(-Angle);
			Vector2 rDirection = ray.Direction.RotatedDeg(-Angle);

			// Horizontal check
			float tA = (-Size.X / 2f - rOrigin.X) / rDirection.X;
			float tB = (Size.X / 2f - rOrigin.X) / rDirection.X;
			float tNearHori = Min(tA, tB);
			float tFarHori = Max(tA, tB);
			if (tFarHori < 0) return -1;

			// Vertical check
			tA = (-Size.Y / 2f - rOrigin.Y) / rDirection.Y;
			tB = (Size.Y / 2f - rOrigin.Y) / rDirection.Y;
			float tNearVert = Min(tA, tB);
			float tFarVert = Max(tA, tB);
			if (tFarVert < 0) return -1;

			float min = Max(tNearHori, tNearVert);
			float max = Min(tFarHori, tFarVert);

			if (min < max)
			{
				return min;
			}
			else return float.NegativeInfinity;
		}
		public override Vector2 NormalAt(Vector2 point)
		{
			point = (point - Position).RotatedDeg(-Angle);
			point.X *= Size.Y / 2f;
			point.Y *= Size.X / 2f;
			point = point.RotatedDeg(-45);
			bool right = point.X > 0;
			bool top = point.Y > 0;

			if (right && top) return Normals[0];
			else if (!right && top) return Normals[1];
			else if (!right && !top) return Normals[2];
			else return Normals[3];
		}

		// =====================================================
		// Debugging methods
		// =====================================================
		public void DrawNormals(EasyDraw ed)
		{
			for (int i = 0; i < baseNormals.Length; i++)
			{
				Vector2 a = Position + (Corners[i] + Corners[(i + 1) % Corners.Length]) / 2f;
				Vector2 b = a + Normals[i] * 50;
				ed.Line(a.X, a.Y, b.X, b.Y);
			}
		}
		public void DrawCorners(EasyDraw ed)
		{
			foreach (Vector2 corner in Corners)
			{
				Vector2 a = Position + corner;
				ed.Ellipse(a.X, a.Y, 10, 10);
			}
		}
		public void DrawOverlapOnAxis(EasyDraw ed, Vector2 axis)
		{
			axis.Normalize();
			var (min, max) = MinMaxBounds(axis);

			//axis.y = -axis.y;

			Vector2 a = axis * min;
			Vector2 b = axis * max;
			ed.Line(a.X, a.Y, b.X, b.Y);
		}

		protected override void Invalidate()
		{
			_cornersValid = false;
			_normalsValid = false;
		}
	}
}
