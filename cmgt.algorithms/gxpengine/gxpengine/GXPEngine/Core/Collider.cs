namespace GXPEngine.Core
{
	public class Collider
	{
		public Collider()
		{
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														HitTest()
		//------------------------------------------------------------------------------------------------------------------------		
		public virtual bool HitTest(Collider other)
		{
			return false;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														HitTest()
		//------------------------------------------------------------------------------------------------------------------------		
		public virtual bool HitTestPoint(float x, float y)
		{
			return false;
		}
	}
}

