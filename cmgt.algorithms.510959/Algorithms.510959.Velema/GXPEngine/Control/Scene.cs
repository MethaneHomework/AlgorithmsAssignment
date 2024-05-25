using System.Collections.Generic;
using System;

namespace GXPEngine.Control
{
	internal abstract class Scene : GameObject
	{
		public static Scene Current { get; private set; }
		protected static Stack<Scene> SceneStack = new Stack<Scene>();
		protected int Width => game.Width;
		protected int Height => game.Height;

		public virtual void Load()
		{
			//if (Current != null) throw new Exception("Cannot load scene if another scene is already running");
			Current?.Unload();
			game.AddChild(this);
			Current = this;
		}

		private void Unload()
		{
			game.RemoveChild(this);
			Current = null;
		}

		public static void Switch(Scene other)
		{
			SceneStack.Push(Current);

			Current.Unload();
			other.Load();
		}

		// Switch back to the previous scene if there was one
		public virtual void Previous()
		{
			Scene popped;
			try
			{
				popped = SceneStack.Pop();
			}
			catch
			{
				popped = null;
			}
			Unload();
			popped?.Load();
		}
	}
}
