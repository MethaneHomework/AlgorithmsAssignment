namespace GXPEngine
{
	internal struct Ray
	{
		public Vector2 Origin;
		public Vector2 Direction;

		public Ray(Vector2 origin, Vector2 direction)
		{
			Origin = origin;
			Direction = direction.Normalized();
		}
		public static Ray Horizontal => new Ray(new Vector2(), new Vector2(1, 0));
		public static Ray Vertical => new Ray(new Vector2(), new Vector2(0, 1));

		public Vector2 At(float t) => Origin + t * Direction;
	}
}
