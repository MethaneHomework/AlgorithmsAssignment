using GXPEngine.Physics;

namespace GXPEngine.Primitives
{
	internal class Circle : PhysicsObject
	{
		public Circle(Vector2 position, float radius)
		{
			body = new CircleCollider(position, radius, this);

			Position = position;
		}
	}
}
