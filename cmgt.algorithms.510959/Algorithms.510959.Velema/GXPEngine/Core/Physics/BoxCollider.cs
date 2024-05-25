using System;

namespace GXPEngine.Core
{
	public class BoxCollider : Collider
	{
		private readonly Sprite _owner;

		//------------------------------------------------------------------------------------------------------------------------
		//														BoxCollider()
		//------------------------------------------------------------------------------------------------------------------------		
		public BoxCollider(Sprite owner)
		{
			_owner = owner;
		}


		//------------------------------------------------------------------------------------------------------------------------
		//														HitTest()
		//------------------------------------------------------------------------------------------------------------------------		
		public override bool HitTest(Collider other)
		{
			if (other is BoxCollider collider)
			{
				Vector2[] c = _owner.GetExtents();
				if (c == null) return false;
				Vector2[] d = collider._owner.GetExtents();
				if (d == null) return false;
				if (!AreaOverlap(c, d)) return false;
				return AreaOverlap(d, c);
			}
			else
			{
				return false;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														HitTest()
		//------------------------------------------------------------------------------------------------------------------------		
		public override bool HitTestPoint(float x, float y)
		{
			Vector2[] c = _owner.GetExtents();
			if (c == null) return false;
			Vector2 p = new Vector2(x, y);
			return PointOverlapsArea(p, c);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														areaOverlap()
		//------------------------------------------------------------------------------------------------------------------------
		private bool AreaOverlap(Vector2[] c, Vector2[] d)
		{
			// normal 1:
			float ny = c[1].X - c[0].X;
			float nx = c[0].Y - c[1].Y;
			// own 'depth' in direction of this normal:
			float dx = c[3].X - c[0].X;
			float dy = c[3].Y - c[0].Y;
			float dot = (dy * ny + dx * nx);

			if (dot == 0.0f) dot = 1.0f;

			float t, minT, maxT;

			t = ((d[0].X - c[0].X) * nx + (d[0].Y - c[0].Y) * ny) / dot;
			maxT = t; minT = t;

			t = ((d[1].X - c[0].X) * nx + (d[1].Y - c[0].Y) * ny) / dot;
			minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

			t = ((d[2].X - c[0].X) * nx + (d[2].Y - c[0].Y) * ny) / dot;
			minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

			t = ((d[3].X - c[0].X) * nx + (d[3].Y - c[0].Y) * ny) / dot;
			minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

			if ((minT >= 1) || (maxT <= 0)) return false;

			// second normal:
			ny = dx;
			nx = -dy;
			dx = c[1].X - c[0].X;
			dy = c[1].Y - c[0].Y;
			dot = (dy * ny + dx * nx);

			if (dot == 0.0f) dot = 1.0f;

			t = ((d[0].X - c[0].X) * nx + (d[0].Y - c[0].Y) * ny) / dot;
			maxT = t; minT = t;

			t = ((d[1].X - c[0].X) * nx + (d[1].Y - c[0].Y) * ny) / dot;
			minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

			t = ((d[2].X - c[0].X) * nx + (d[2].Y - c[0].Y) * ny) / dot;
			minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

			t = ((d[3].X - c[0].X) * nx + (d[3].Y - c[0].Y) * ny) / dot;
			minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

			if ((minT >= 1) || (maxT <= 0)) return false;

			return true;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														pointOverlapsArea()
		//------------------------------------------------------------------------------------------------------------------------
		//ie. for hittestpoint and mousedown/up/out/over
		private bool PointOverlapsArea(Vector2 p, Vector2[] c)
		{
			float dx1 = c[1].X - c[0].X;
			float dy1 = c[1].Y - c[0].Y;
			float dx2 = c[3].X - c[0].X;
			float dy2 = c[3].Y - c[0].Y;
			// first: take delta1 as normal:
			float dot = dy2 * dx1 - dx2 * dy1;

			float t;

			t = ((p.Y - c[0].Y) * dx1 - (p.X - c[0].X) * dy1) / dot;
			if ((t > 1) || (t < 0)) return false;

			// next: take delta2 as normal:
			dot = -dot;

			t = ((p.Y - c[0].Y) * dx2 - (p.X - c[0].X) * dy2) / dot;

			if ((t > 1) || (t < 0)) return false;

			return true;
		}

		public override float TimeOfImpact(Collider other, float vx, float vy, out Vector2 normal)
		{
			normal = new Vector2();
			if (other is BoxCollider collider)
			{
				Vector2[] c = _owner.GetExtents();
				if (c == null) return float.MaxValue;
				Vector2[] d = collider._owner.GetExtents();
				if (d == null) return float.MaxValue;

				float maxTOI = float.MinValue;
				float minTOE = float.MaxValue;
				// normals of this vs points of other:
				float nx = -c[0].Y + c[1].Y;
				float ny = -c[1].X + c[0].X;
				if (UpdateImpactExitTime(
						c[0].X, c[0].Y, nx, ny,
						c[3].X - c[0].X, c[3].Y - c[0].Y, d, -vx, -vy, ref maxTOI, ref minTOE))
				{
					normal.X = nx;
					normal.Y = ny;
				}
				if (minTOE <= maxTOI || minTOE <= 0)
					return float.MaxValue;
				nx = c[0].Y - c[3].Y;
				ny = c[3].X - c[0].X;
				if (UpdateImpactExitTime(
						c[0].X, c[0].Y, nx, ny,
						c[1].X - c[0].X, c[1].Y - c[0].Y, d, -vx, -vy, ref maxTOI, ref minTOE))
				{
					normal.X = nx;
					normal.Y = ny;
				}
				if (minTOE <= maxTOI || minTOE <= 0)
					return float.MaxValue;

				// normals of other vs points of this:
				nx = -d[0].Y + d[1].Y;
				ny = -d[1].X + d[0].X;
				if (UpdateImpactExitTime(
						d[0].X, d[0].Y, nx, ny,
						d[3].X - d[0].X, d[3].Y - d[0].Y, c, vx, vy, ref maxTOI, ref minTOE))
				{
					normal.X = nx;
					normal.Y = ny;
				}
				if (minTOE <= maxTOI || minTOE <= 0)
					return float.MaxValue;

				nx = d[0].Y - d[3].Y;
				ny = d[3].X - d[0].X;
				if (UpdateImpactExitTime(
						d[0].X, d[0].Y, nx, ny,
						d[1].X - d[0].X, d[1].Y - d[0].Y, c, vx, vy, ref maxTOI, ref minTOE))
				{
					normal.X = nx;
					normal.Y = ny;
				}
				if (minTOE <= maxTOI || minTOE <= 0)
					return float.MaxValue;
				// normalize the normal when there's an actual collision:
				float nLen = Mathf.Sqrt(normal.X * normal.X + normal.Y * normal.Y);
				normal.X /= nLen;
				normal.Y /= nLen;
				if (normal.X * vx + normal.Y * vy > 0)
				{
					normal.X *= -1;
					normal.Y *= -1;
				}
				if (maxTOI >= 0)
					return maxTOI;
				// remaining case: maxTOI is negative, minTOE is positive. => currently overlapping
				if (Mathf.Abs(maxTOI) < Mathf.Abs(minTOE)) // only return collision if going towards deeper overlap!
					return 0;
				return float.MaxValue;
			}
			else
			{
				return float.MaxValue;
			}
		}


		// cx,cy: corner point of body 1
		// nx,ny: current normal (not necessarily normalized)
		// dx,dy: body vector of body 1 that gives the max depth along this normal
		// d: points of body 2
		// vx,vy: relative velocity of body 2 w. resp. to body 1
		// TOI/TOE: time of impact/exit. Updated when we find better values along this normal.
		//
		// Returns true if the TOI is updated.
		private bool UpdateImpactExitTime(float cx, float cy, float nx, float ny, float dx, float dy, Vector2[] d, float vx, float vy, ref float maxTOI, ref float minTOE)
		{
			float dot = (dy * ny + dx * nx);

			if (dot == 0.0f) dot = 1.0f; // hm

			float t, minT, maxT;

			t = ((d[0].X - cx) * nx + (d[0].Y - cy) * ny) / dot;
			maxT = t; minT = t;

			t = ((d[1].X - cx) * nx + (d[1].Y - cy) * ny) / dot;
			minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

			t = ((d[2].X - cx) * nx + (d[2].Y - cy) * ny) / dot;
			minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

			t = ((d[3].X - cx) * nx + (d[3].Y - cy) * ny) / dot;
			minT = Math.Min(minT, t); maxT = Math.Max(maxT, t);

			// relative velocity:
			float vp = (vx * nx + vy * ny) / dot;

			if (Mathf.Abs(vp) < 0.0001f)
			{
				if (minT >= 1 || maxT < 0)
				{ // no overlap in this direction, ever.
					minTOE = float.MinValue;
					maxTOI = float.MaxValue;
					return true;
				}
			}
			else
			{
				float TOI, TOE;
				if (vp > 0)
				{
					TOI = -maxT / vp;
					TOE = (1 - minT) / vp;
				}
				else
				{
					TOE = -maxT / vp;
					TOI = (1 - minT) / vp;
				}
				if (TOE < minTOE)
				{
					minTOE = TOE;
				}
				if (TOI > maxTOI)
				{
					maxTOI = TOI;
					return true;
				}
			}
			return false;
		}


		public override Collision GetCollisionInfo(Collider other)
		{
			float penetrationDepth = float.MaxValue;
			Vector2 normal = new Vector2();
			Vector2 point = new Vector2();
			if (other is BoxCollider collider)
			{
				//Console.WriteLine ("\n\n===== Computing collision data:\n");
				Vector2[] c = _owner.GetExtents();
				if (c == null) return null;
				Vector2[] d = collider._owner.GetExtents();
				if (d == null) return null;

				//Console.WriteLine ("\nSide vectors of this:\n {0},{1} and {2},{3}",
				//	c[1].x-c[0].x,c[1].y-c[0].y,c[3].x-c[0].x,c[3].y-c[0].y
				//);

				// normals of this vs points of other:
				float nx = -c[0].Y + c[1].Y;
				float ny = -c[1].X + c[0].X;
				if (!UpdateCollisionPoint(
						c[0].X, c[0].Y, nx, ny,
						c[3].X - c[0].X, c[3].Y - c[0].Y, d,
						true, ref penetrationDepth, ref normal, ref point))
					return null;

				nx = c[0].Y - c[3].Y;
				ny = c[3].X - c[0].X;
				if (!UpdateCollisionPoint(
					c[0].X, c[0].Y, nx, ny,
					c[1].X - c[0].X, c[1].Y - c[0].Y, d,
					true, ref penetrationDepth, ref normal, ref point))
					return null;

				//Console.WriteLine ("\nSide vectors of other:\n {0},{1} and {2},{3}",
				//	d[1].x-d[0].x,d[1].y-d[0].y,d[3].x-d[0].x,d[3].y-d[0].y
				//);
				// normals of other vs points of this:
				nx = -d[0].Y + d[1].Y;
				ny = -d[1].X + d[0].X;
				if (!UpdateCollisionPoint(
					d[0].X, d[0].Y, nx, ny,
					d[3].X - d[0].X, d[3].Y - d[0].Y, c,
					false, ref penetrationDepth, ref normal, ref point))
					return null;

				nx = d[0].Y - d[3].Y;
				ny = d[3].X - d[0].X;
				if (!UpdateCollisionPoint(
					d[0].X, d[0].Y, nx, ny,
					d[1].X - d[0].X, d[1].Y - d[0].Y, c,
					false, ref penetrationDepth, ref normal, ref point))
					return null;
				/*
				if (convertToParentSpace && _owner.parent!=null) {
					normal = _owner.parent.InverseTransformPoint (normal.x, normal.y);
					float nLen = Mathf.Sqrt (normal.x * normal.x + normal.y * normal.y);
					normal.x /= nLen;
					normal.y /= nLen;

					point = _owner.parent.InverseTransformPoint (point.x, point.y);
				}
				*/
				return new Collision(_owner, collider._owner, normal, point, penetrationDepth);
			}
			else
			{
				return null;
			}
		}

		private bool UpdateCollisionPoint(
			float cx, float cy, float nx, float ny, float dx, float dy, Vector2[] d, bool invertNormal,
			ref float minPenetrationDepth, ref Vector2 normal, ref Vector2 point
		)
		{
			float dot = (dy * ny + dx * nx);

			if (dot == 0.0f) dot = 1.0f; // hm

			Vector2 argMin = new Vector2();
			Vector2 argMax = new Vector2();
			float minT = float.MaxValue;
			float maxT = float.MinValue;
			for (int i = 0; i < d.Length; i++)
			{
				float t = ((d[i].X - cx) * nx + (d[i].Y - cy) * ny) / dot;
				if (t < minT)
				{
					minT = t;
					argMin = d[i];
				}
				if (t > maxT)
				{
					maxT = t;
					argMax = d[i];
				}
			}
			// Two cases where no collision:
			if (maxT < 0)
				return false;
			if (minT > 1)
				return false;
			bool updateNormal = false;
			float lenD = Mathf.Sqrt(dx * dx + dy * dy);

			//Console.WriteLine ("\n  considering normal: {0},{1}\n  minT, maxT: {2},{3}\n  intersection candidates: {4},{5}",
			//	nx,ny,minT,maxT,(1-minT)*lenD,maxT*lenD
			//);
			if (lenD == 0)
				lenD = 1; // hm
			if (maxT * lenD < minPenetrationDepth)
			{
				minPenetrationDepth = maxT * lenD;
				updateNormal = true;
				point = argMax;
			}
			if ((1 - minT) * lenD < minPenetrationDepth)
			{
				minPenetrationDepth = (1 - minT) * lenD;
				updateNormal = true;
				point = argMin;
				invertNormal = !invertNormal;
			}
			if (updateNormal)
			{
				float len = invertNormal ? -Mathf.Sqrt(nx * nx + ny * ny) : Mathf.Sqrt(nx * nx + ny * ny);
				normal.X = nx / len;
				normal.Y = ny / len;
				//Console.WriteLine ("NEW BEST");
			}
			else
			{
				//Console.WriteLine ("NO UPDATE");
			}
			//Console.WriteLine (" (check:) best normal: "+normal);
			return true;
		}
	}
}


