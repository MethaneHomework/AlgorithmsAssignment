using GXPEngine.Physics;

namespace GXPEngine.Primitives
{
	internal class Box : PhysicsObject
	{
		public Box(Vector2 position, Vector2 size, float angle)
		{
			body = new OBCollider(position, size, angle, this);

			Position = position;
			Rotation = angle;
		}
	}
}
