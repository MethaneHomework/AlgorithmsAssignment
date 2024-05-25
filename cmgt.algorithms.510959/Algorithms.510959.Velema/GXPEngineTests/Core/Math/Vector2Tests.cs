using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using static GXPEngine.Mathf;

namespace Tests
{
	[TestClass()]
	public class Vector2Tests
	{
		[TestCategory("Functionality test")]
		[TestMethod()]
		public void Vector2Test()
		{
			Vector2 vector = new Vector2(3, 4);
			Assert.AreEqual(3, vector.X);
			Assert.AreEqual(4, vector.Y);

			vector = Vector2.Zero;
			Assert.AreEqual(0, vector.X);
			Assert.AreEqual(0, vector.Y);

			vector = Vector2.PositiveX;
			Assert.AreEqual(1, vector.X);
			Assert.AreEqual(0, vector.Y);

			vector = Vector2.PositiveY;
			Assert.AreEqual(0, vector.X);
			Assert.AreEqual(1, vector.Y);
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void LengthSquaredTest()
		{
			Vector2 v = new Vector2(3, 4);
			Assert.AreEqual(25f, v.LengthSquared(), 0.0001f);
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void LengthTest()
		{
			Vector2 v = new Vector2(3, 4);
			Assert.AreEqual(5f, v.Length(), 0.0001f);
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void NormalizeTest()
		{
			Vector2 v = new Vector2(3, 4);
			v.Normalize();
			Vector2 expected = new Vector2(3 / 5f, 4 / 5f);
			Assert.AreEqual(expected, v);
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void NormalizedTest()
		{
			Vector2 v = new Vector2(3, 4);
			Vector2 n = v.Normalized();
			Vector2 expected = new Vector2(3 / 5f, 4 / 5f);
			Assert.AreEqual(expected, n);
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void SetXYTest()
		{
			Vector2 v = Vector2.Zero;
			Assert.AreEqual(new Vector2(0, 0), v);
			v.XY = (3f, 8f);
			Assert.AreEqual(new Vector2(3, 8), v);
			v.XY = (-18, 2);
			Assert.AreEqual(new Vector2(-18f, 2f), v);
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void Deg2RadTest()
		{
			Assert.AreEqual(PI, Vector2.Deg2Rad(180), 0.0001f);
			Assert.AreEqual(PI / 4f, Vector2.Deg2Rad(45), 0.0001f);
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void Rad2DegTest()
		{
			Assert.AreEqual(180, Vector2.Rad2Deg(PI), 0.0001f);
			Assert.AreEqual(45, Vector2.Rad2Deg(PI / 4f), 0.0001f);
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void GetRadiansTest()
		{
			Vector2 v = new Vector2(1, 1);
			Assert.AreEqual(PI / 4f, v.GetRadians());
			v = Vector2.PositiveY;
			Assert.AreEqual(PI / 2f, v.GetRadians());
			v = -Vector2.PositiveX;
			Assert.AreEqual(-PI, v.GetRadians());
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void GetDegreesTest()
		{
			Vector2 v = new Vector2(1, 1);
			Assert.AreEqual(45, v.GetDegrees(), 0.0001f);
			v = Vector2.PositiveY;
			Assert.AreEqual(90, v.GetDegrees(), 0.0001f);
			v = -Vector2.PositiveX;
			Assert.AreEqual(-180, v.GetDegrees(), 0.0001f);
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void SetRadiansTest()
		{
			Vector2 v = Vector2.PositiveY;
			v.SetRadians(PI);
			Assert.IsTrue(new Vector2(-1, 0).Equals(v, 0.0001f));
			Assert.AreEqual(1, v.Length());
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void SetDegreesTest()
		{
			Vector2 v = Vector2.PositiveY;
			v.SetDegrees(180);
			Assert.IsTrue(new Vector2(-1, 0).Equals(v, 0.0001f));
			Assert.AreEqual(1, v.Length());
			v.SetDegrees(-180);
			Assert.IsTrue(new Vector2(-1, 0).Equals(v, 0.0001f));
			Assert.AreEqual(1, v.Length());
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void GetUnitVectorRadTest()
		{
			Vector2 v = Vector2.GetUnitVectorRad(PI / 4f);
			Vector2 expected = new Vector2(1, 1).Normalized();
			Assert.IsTrue(v.Equals(expected, 0.0001f));
			v = Vector2.GetUnitVectorRad(5f * PI / 4f);
			expected *= -1;
			Assert.IsTrue(v.Equals(expected, 0.0001f));
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void GetUnitVectorDegTest()
		{
			Vector2 v = Vector2.GetUnitVectorDeg(45f);
			Vector2 expected = new Vector2(1, 1).Normalized();
			Assert.IsTrue(v.Equals(expected, 0.0001f));

			v = Vector2.GetUnitVectorDeg(225f);
			expected *= -1;
			Assert.IsTrue(v.Equals(expected, 0.0001f));
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void RandomUnitVecTest()
		{
			Vector2 a = Vector2.RandomUnitVec();
			Vector2 b = Vector2.RandomUnitVec();
			Assert.AreNotEqual(a, b);
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void RotatedRadTest()
		{
			Vector2 v = new Vector2(1, 1).Normalized();
			v = v.RotatedRad(-PI / 4f);
			Assert.IsTrue(v.Equals(Vector2.PositiveX, 0.0001f));
			v = v.RotatedRad(PI);
			Assert.IsTrue(v.Equals(-Vector2.PositiveX, 0.0001f));
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void RotatedDegTest()
		{
			Vector2 v = new Vector2(1, 1).Normalized();
			v = v.RotatedDeg(-45f);
			Assert.IsTrue(v.Equals(Vector2.PositiveX, 0.0001f));
			v = v.RotatedDeg(180f);
			Assert.IsTrue(v.Equals(-Vector2.PositiveX, 0.0001f));
			v = v.RotatedDeg(180f);
			Assert.IsTrue(v.Equals(Vector2.PositiveX, 0.0001f));
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void RotateVectorsRadTest()
		{
			Vector2[] vectors =
			{
				Vector2.PositiveX,
				Vector2.PositiveY,
				new Vector2(1, 1).Normalized(),
			};

			Vector2[] expected = new Vector2[vectors.Length];
			for (int i = 0; i < vectors.Length; i++)
			{
				Vector2 v = vectors[i];
				Vector2 r = v.RotatedRad(PI / 4f);
				expected[i] = r;
			}

			Vector2[] results = Vector2.RotateVectorsRad(vectors, PI / 4f);

			for (int i = 0; i < results.Length; i++)
			{
				Vector2 e = expected[i];
				Vector2 r = results[i];
				Assert.IsTrue(e.Equals(r, 0.0001f));
			}
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void RotateVectorsDegTest()
		{
			Vector2[] vectors =
			{
				Vector2.PositiveX,
				Vector2.PositiveY,
				new Vector2(1, 1).Normalized(),
			};

			Vector2[] expected = new Vector2[vectors.Length];
			for (int i = 0; i < vectors.Length; i++)
			{
				Vector2 v = vectors[i];
				Vector2 r = v.RotatedDeg(PI / 4f);
				expected[i] = r;
			}

			Vector2[] results = Vector2.RotateVectorsDeg(vectors, PI / 4f);

			for (int i = 0; i < results.Length; i++)
			{
				Vector2 e = expected[i];
				Vector2 r = results[i];
				Assert.IsTrue(e.Equals(r, 0.0001f));
			}
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void RotateAroundRadiansTest()
		{
			Vector2 a = new Vector2(4, 3);
			Vector2 b = new Vector2(3, 5);
			Vector2 expected = new Vector2(2, 2);
			Vector2 result = b.RotateAroundRadians(a, PI / 2f);
			Assert.IsTrue(expected.Equals(result, 0.0001f));
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void RotateAroundDegreesTest()
		{
			Vector2 a = new Vector2(4, 3);
			Vector2 b = new Vector2(3, 5);
			Vector2 expected = new Vector2(2, 2);
			Vector2 result = b.RotateAroundDegrees(a, 90);
			Assert.IsTrue(expected.Equals(result, 0.0001f));
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void LerpTest()
		{
			Vector2 a = Vector2.PositiveX;
			Vector2 b = Vector2.PositiveY;
			Vector2 result = Vector2.Lerp(a, b, 0.5f).Normalized();
			Vector2 expected = new Vector2(1, 1).Normalized();
			Assert.IsTrue(result.Equals(expected, 0.0001f));
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void DotTest()
		{
			Vector2 a = Vector2.PositiveX;
			Vector2 b = Vector2.PositiveY;
			Assert.AreEqual(0, Vector2.Dot(a, b));
			Assert.AreEqual(1, Vector2.Dot(a, a));
			Assert.AreEqual(1, Vector2.Dot(b, b));
			Assert.AreEqual(-1, Vector2.Dot(-a, a));
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void DotTest1()
		{
			Vector2 a = Vector2.PositiveX;
			Vector2 b = Vector2.PositiveY;
			Assert.AreEqual(0, a.Dot(b));
			Assert.AreEqual(1, a.Dot(a));
			Assert.AreEqual(1, b.Dot(b));
			Assert.AreEqual(-1, (-a).Dot(a));
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void NormalTest()
		{
			Vector2 v = Vector2.PositiveX * 5;
			Vector2 result = v.Normal();
			Vector2 expected = Vector2.PositiveY;
			Assert.IsTrue(result.Equals(expected, 0.0001f));
		}

		[TestCategory("Functionality test")]
		[TestMethod()]
		public void ToStringTest()
		{
			Vector2 v = new Vector2(3, 4);
			string result = v.ToString();
			string expected = "(3; 4)";
			Assert.AreEqual(expected, result);
		}

		[TestCategory("Benchmark")]
		[TestMethod()]
		public void AdditionBM()
		{
			const int n = 100000;
			Stopwatch sw = new Stopwatch();
			Random r = new Random();

			float[] floatsA = new float[n];
			float[] floatsB = new float[n];
			float[] floatsR = new float[n];

			for (int i = 0; i < n; i++)
			{
				floatsA[i] = (float)r.NextDouble();
				floatsB[i] = (float)r.NextDouble();
			}

			sw.Start();
			for (int i = 0; i < n; i++)
			{
				floatsR[i] = floatsA[i] + floatsB[i];
			}
			int elapsedTime = (int)sw.ElapsedTicks;
			sw.Stop();

			Debug.WriteLine("Addition of {0} floats took {1} ticks", n, elapsedTime);
			sw.Reset();

			Vector2[] vecsA = new Vector2[n];
			Vector2[] vecsB = new Vector2[n];
			Vector2[] vecsR = new Vector2[n];

			for (int i = 0; i < n; i++)
			{
				vecsA[i] = Vector2.RandomUnitVec() * (float)r.NextDouble() * 10;
				vecsB[i] = Vector2.RandomUnitVec() * (float)r.NextDouble() * 10;
			}

			sw.Start();
			for (int i = 0; i < n; i++)
			{
				vecsR[i] = vecsA[i] + vecsB[i];
			}
			elapsedTime = (int)sw.ElapsedTicks;
			sw.Stop();

			Debug.WriteLine("Addition of {0} Vector2 instances took {1} ticks", n, elapsedTime);

			Assert.IsTrue(true);
		}

		[TestCategory("Benchmark")]
		[TestMethod()]
		public void RotationBM()
		{
			const int n = 10000;
			const int p = 20000;

			Stopwatch sw = new Stopwatch();
			Random r = new Random();

			Vector2[] vecsBatch = new Vector2[n];
			Vector2[] vecsBatchR = new Vector2[n];

			float radians;
			float degrees;

			float[] radianTimes = new float[p];
			float[] degreeTimes = new float[p];
			float averageRadianTime;
			float averageDegreeTime;

			for (int i = 0; i < n; i++)
			{
				vecsBatch[i] = Vector2.RandomUnitVec() * (float)r.NextDouble() * 10;

				vecsBatchR[i] = Vector2.Zero;
			}

			for (int i = 0; i < p; i++)
			{
				radians = (float)r.NextDouble() * 2 * PI;
				sw.Restart();
				vecsBatch = Vector2.RotateVectorsRad(vecsBatch, radians);
				radianTimes[i] = sw.ElapsedTicks;
				GC.Collect(0, GCCollectionMode.Forced);

				degrees = (float)r.NextDouble() * 2 * PI;
				sw.Restart();
				vecsBatch = Vector2.RotateVectorsDeg(vecsBatch, degrees);
				degreeTimes[i] = sw.ElapsedTicks;
				GC.Collect(0, GCCollectionMode.Forced);
			}

			averageRadianTime = radianTimes.Sum() / radianTimes.Count();
			averageDegreeTime = degreeTimes.Sum() / degreeTimes.Count();

			Debug.WriteLine("Average time taken by radian batch rotation of {0} vectors: {1} ticks (Sample size {2}).", n, averageRadianTime, p);
			Debug.WriteLine("Average time taken by degree batch rotation of {0} vectors: {1} ticks (Sample size {2}).", n, averageDegreeTime, p);
		}
	}
}