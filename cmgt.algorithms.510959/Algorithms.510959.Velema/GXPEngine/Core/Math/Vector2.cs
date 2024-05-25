using System;
using static GXPEngine.Mathf; // Allows using Mathf functions

/// <summary>
/// Struct <c>Vector2</c> contains data and functionality of a 2-dimensional vector.
/// </summary>
public struct Vector2 : IEquatable<Vector2>
{
	// --------------------------------------------------------
	//		Fields
	// --------------------------------------------------------

	private static Random Random = new Random(); // Random object since the MathF implementation tends to be annoying
	public float X;
	public float Y;


	// --------------------------------------------------------
	//		Properties
	// --------------------------------------------------------

	public (float x, float y) XY
	{
		get => (X, Y);
		set
		{
			X = value.x;
			Y = value.y;
		}
	}

	// --------------------------------------------------------
	//		Constructors
	// --------------------------------------------------------

	// Standard constructor
	public Vector2(double x = 0, double y = 0)
	{
		X = (float)x;
		Y = (float)y;
	}
	// Standard vectors as properties
	public static Vector2 Zero => new Vector2(0, 0);
	public static Vector2 PositiveX => new Vector2(1, 0);
	public static Vector2 PositiveY => new Vector2(0, 1);

	// ========================================================
	//
	//      Methods - Week 1
	//        - Mostly math operators
	//
	// ========================================================

	// --------------------------------------------------------
	//		Math operators
	// --------------------------------------------------------
	// Adds two vectors together (component-wise addition)
	public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);
	// Negates a vector so it will point in the opposite direction
	public static Vector2 operator -(Vector2 v) => new Vector2(-v.X, -v.Y);
	// Subtracts a vector
	public static Vector2 operator -(Vector2 a, Vector2 b) => a + -b;
	// Scales vector by scalar value
	public static Vector2 operator *(Vector2 v, float s) => new Vector2(v.X * s, v.Y * s);
	// Scales vector by scalar value
	public static Vector2 operator *(float s, Vector2 v) => v * s;
	// Divides the components of a vector by a scalar value, will do a zero check to prevent strange behavior due to infinity and NaN values.
	public static Vector2 operator /(Vector2 v, float s)
	{
		if (s == 0) throw new DivideByZeroException();
		return v * (1 / s);
	}

	public static bool operator ==(Vector2 left, Vector2 right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Vector2 left, Vector2 right)
	{
		return !(left == right);
	}

	// --------------------------------------------------------
	//		Length related calculations and manipulations
	// --------------------------------------------------------
	// Calculate the squared length/magnitude of the vector
	public float LengthSquared() => (X * X) + (Y * Y);
	// Calculate the length (magnitude) of the instance
	public float Length() => Sqrt(LengthSquared());
	// Normalized the vector instance so the length will be 1

	public void Normalize()
	{
		if (X == 0 && Y == 0) return;
		this /= Length();
	}
	// Creates a normalized (unit length) copy of vector instance
	public Vector2 Normalized()
	{
		float length = Length();
		if (length == 0) return new Vector2();
		return new Vector2(X, Y) / length;
	}
	// Sets x and y component of vector instance
	public void SetXY(float x, float y) => this = new Vector2(x, y);

	// ========================================================
	//
	//      Methods - Week 2
	//        - Mostly angles and trigonometry
	//
	// ========================================================

	// --------------------------------------------------------
	//		Angle conversion
	// --------------------------------------------------------
	// Converts degree angle to radian angle
	public static float Deg2Rad(float degrees) => degrees * PI / 180;
	// Converts radian angle to degree angle
	public static float Rad2Deg(float radians) => radians * 180 / PI;

	// --------------------------------------------------------
	//		Angle calculation
	// --------------------------------------------------------
	// Calculates the angle of the vector
	public float GetRadians() => Atan2(Y, X);
	// Calculates the angle of the vector
	public float GetDegrees() => Rad2Deg(Atan2(Y, X));

	// --------------------------------------------------------
	//		Angle setting
	// --------------------------------------------------------
	// Sets the angle of this instance
	public void SetRadians(float radians) => this = new Vector2(Cos(radians), Sin(radians)) * Length();
	// Sets the angle of this instance
	public void SetDegrees(float degrees) => SetRadians(Deg2Rad(degrees));

	// --------------------------------------------------------
	//		Unit vector construction
	// --------------------------------------------------------
	// Create a unit vector in the specified direction
	public static Vector2 GetUnitVectorRad(float radians) => new Vector2(Cos(radians), Sin(radians));
	// Create a unit vector in the specified direction
	public static Vector2 GetUnitVectorDeg(float degrees) => GetUnitVectorRad(Deg2Rad(degrees));

	// Create a unit vector in a random direction
	public static Vector2 RandomUnitVec() => GetUnitVectorDeg(Random.Next(360));

	// --------------------------------------------------------
	//		Rotation
	// --------------------------------------------------------
	// Creates a rotated copy of this vector instance
	public Vector2 RotatedRad(float angle)
	{
		float sin = Sin(angle);
		float cos = Cos(angle);
		float xComp = cos * X - sin * Y;
		float yComp = sin * X + cos * Y;
		return new Vector2(xComp, yComp);
	}
	// Creates a rotated copy of this vector instance
	public Vector2 RotatedDeg(float angle) => RotatedRad(Deg2Rad(angle));

	// --------------------------------------------------------
	//		Batch rotation
	// --------------------------------------------------------
	// Batch rotation for increased speed
	public static Vector2[] RotateVectorsRad(Vector2[] vecs, float angle)
	{
		angle = 1 * angle;
		float cosA = Cos(angle);
		float sinA = Sin(angle);

		Vector2[] result = new Vector2[vecs.Length];
		for (int i = 0; i < vecs.Length; i++)
		{
			float x = vecs[i].X;
			float y = vecs[i].Y;

			result[i] = new Vector2(cosA * x - sinA * y, sinA * x + cosA * y);
		}

		return result;
	}
	// Batch rotation for increased speed
	public static Vector2[] RotateVectorsDeg(Vector2[] vecs, float angle)
	{
		angle = Deg2Rad(angle);
		float cosA = Cos(angle);
		float sinA = Sin(angle);

		Vector2[] result = new Vector2[vecs.Length];
		for (int i = 0; i < vecs.Length; i++)
		{
			float x = vecs[i].X;
			float y = vecs[i].Y;

			result[i] = new Vector2(cosA * x - sinA * y, sinA * x + cosA * y);
		}

		return result;
	}

	// --------------------------------------------------------
	//		Rotation around points
	// --------------------------------------------------------
	// Create a copy of vector instance that is rotated around a point
	public Vector2 RotateAroundRadians(Vector2 point, float radians)
	{
		Vector2 translated = this - point;

		float sin = Sin(radians);
		float cos = Cos(radians);

		float xComp = cos * translated.X - sin * translated.Y;
		float yComp = sin * translated.X + cos * translated.Y;

		return new Vector2(xComp, yComp) + point;
	}
	// Create a copy of vector instance that is rotated around a point
	public Vector2 RotateAroundDegrees(Vector2 point, float degrees) => RotateAroundRadians(point, Deg2Rad(degrees));

	// --------------------------------------------------------
	//		Interpolation
	// --------------------------------------------------------
	// Linear interpolation function for vectors
	public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => a + ((b - a) * t);
	// The Lerp function, mathematically, is defined as lerp(a, b, t) = a + (b — a) * t

	// ========================================================
	//
	//      Methods - Week 4
	//        - "Advanced" methods
	//
	// ========================================================

	// --------------------------------------------------------
	//		Dot product
	// --------------------------------------------------------
	// Calculates dot product (scalar product) of two vectors
	public static float Dot(Vector2 a, Vector2 b) => (a.X * b.X) + (a.Y * b.Y);
	// Calculates dot product of this vector instance with another vector
	public float Dot(Vector2 other) => Dot(this, other);

	// --------------------------------------------------------
	//		Normal construction
	// --------------------------------------------------------
	// Creates a left hand normal for vector instance.
	public Vector2 Normal() => new Vector2(-Y, X).Normalized();

	public override string ToString() => string.Format("({0}; {1})", X, Y);

	public override bool Equals(object obj) => obj is Vector2 vector && Equals(vector);
	public bool Equals(Vector2 other) => (X == other.X) && (Y == other.Y);

	public bool Equals(Vector2 other, float delta)
	{
		if (X == other.X && Y == other.Y) return true;
		float xDif = Abs(X - other.X);
		float yDif = Abs(Y - other.Y);
		if (xDif <= delta && yDif <= delta) return true;
		return false;
	}

	public override int GetHashCode()
	{
		int hashCode = 1861411795;
		hashCode = hashCode * -1521134295 + X.GetHashCode();
		hashCode = hashCode * -1521134295 + Y.GetHashCode();
		return hashCode;
	}
}