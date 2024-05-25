using System;
using System.Collections.Generic;

namespace GXPEngine.Physics
{
	/// <summary>
	/// Ghost colliders can move but don't collide
	/// Static colliders are not affected by physics but do affect other colliders
	/// </summary>

	internal abstract class ACollider
	{
		public GameObject Owner;
		public bool ShouldRemove = false;

		public Vector2 Position
		{
			get => _position;
			set
			{
				_position = value;
				Invalidate();
			}
		}
		Vector2 _position;

		public Vector2 Velocity;
		public float Angle
		{
			get => _angle;
			set
			{
				_angle = value;
				Invalidate();
			}
		}
		private float _angle;

		public bool IsColliding;
		public CollisionInfo LastCollision
		{
			get => _lastCol;
			set
			{
				_lastCol = value;
				OnCollision?.Invoke(this, (this, _lastCol.Other));
			}
		}
		private CollisionInfo _lastCol;
		public EventHandler<(ACollider col, ACollider other)> OnCollision;

		public ColliderType Behavior
		{
			get => _behavior;
			set
			{
				BehaviorChanged?.Invoke(this, new BehaviorChangeEvent(_behavior, value));
				_behavior = value;
			}
		}
		private ColliderType _behavior;
		public EventHandler<BehaviorChangeEvent> BehaviorChanged;

		public struct BehaviorChangeEvent
		{
			public ColliderType OldBehavior;
			public ColliderType NewBehavior;

			public BehaviorChangeEvent(ColliderType oldB, ColliderType newB)
			{
				OldBehavior = oldB; NewBehavior = newB;
			}
		}

		public enum ColliderType
		{
			Rigid,
			Trigger,
			Static,
		}

		// Properties for triggers
		public delegate void TriggerAction(ACollider trigger, ACollider other);
		public TriggerAction TriggerMethod = null;

		public ACollider(PhysicsObject owner)
		{
			Owner = owner;
			Behavior = ColliderType.Static;
		}

		public abstract bool Overlapping(ACollider other);
		public abstract (float min, float max) MinMaxBounds(Vector2 axis);
		public abstract float RayCast(Ray ray);
		public abstract Vector2 NormalAt(Vector2 point);

		public void Step(ICollection<ACollider> colliderList)
		{
			if (Behavior != ColliderType.Static)
			{
				Position += Velocity;

				bool collided = false;
				CollisionInfo bestCol = new CollisionInfo();

				foreach (ACollider collider in colliderList)
				{
					if (collider == this || collider.Behavior == ColliderType.Trigger) continue;
					if (Overlapping(collider))
					{
						CollisionInfo colInfo = LastCollision;
						if (colInfo.Depth > bestCol.Depth)
						{
							bestCol = colInfo;
						}
						collided = true;
					}
				}
				if (collided)
				{
					Position -= bestCol.Normal * bestCol.Depth;

					Vector2 q = Vector2.Dot(bestCol.Normal, Velocity) * bestCol.Normal;
					Velocity -= (2 * 0.9f) * q;
				}

				Owner.Position = Position;
			}
		}
		protected abstract void Invalidate();
		public EventHandler OnDestroy = null;
		public void Destroy() => OnDestroy?.Invoke(this, new EventArgs());
	}
}
