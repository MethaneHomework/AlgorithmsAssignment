using System;
using static GXPEngine.Mathf;

namespace GXPEngine
{
	internal static class MathUtils
	{
		// Calculate the discriminant to use in quadratic formula
		public static float GetDiscriminant(float a, float b, float c) => (b * b) - (4 * a * c);

		// Calculate the roots given a, b and the discriminant of a quadratic equation (ax^2 + bx + c)
		// NOTE: Check if discriminant is not less than 0
		public static (float rootA, float rootB) GetRoots(float discriminant, float a, float b)
		{
			if (discriminant < 0) throw new ArgumentOutOfRangeException("Discriminant cannot be less than 0 when using this method");
			float sqrtDiscriminant = Sqrt(discriminant);
			float rootA = (-b - sqrtDiscriminant) / (2 * a);
			float rootB = (-b + sqrtDiscriminant) / (2 * a);
			return (rootA, rootB);
		}

		public static float RadToDeg(float radians) => radians * 180f / PI;
		public static float DegToRad(float degrees) => degrees * PI / 180f;
	}
}
