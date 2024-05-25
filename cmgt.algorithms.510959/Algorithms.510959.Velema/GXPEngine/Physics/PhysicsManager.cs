using System;
using System.Collections.Generic;
using static GXPEngine.Physics.ACollider;

namespace GXPEngine.Physics
{
	internal class PhysicsManager
	{
		public List<ACollider> Objects => bodies;

		private readonly List<ACollider> bodies;
		private readonly List<ACollider> staticColliders;
		private readonly List<ACollider> triggerColliders;
		private readonly List<ACollider> rigidColliders;

		private bool ActiveStep;

		public PhysicsManager()
		{
			bodies = new List<ACollider>();
			staticColliders = new List<ACollider>();
			triggerColliders = new List<ACollider>();
			rigidColliders = new List<ACollider>();

			ActiveStep = false;
		}

		public void Add(ACollider obj)
		{
			if (ActiveStep)
			{
				throw new InvalidOperationException("Currently running a step, cannot add object");
			}
			else
			{
				bodies.Add(obj);

				if (obj.Behavior == ColliderType.Static) staticColliders.Add(obj);
				else if (obj.Behavior == ColliderType.Rigid) rigidColliders.Add(obj);
				else if (obj.Behavior == ColliderType.Trigger) triggerColliders.Add(obj);

				obj.BehaviorChanged += BehaviorChangeHandler;
			}
		}
		public void Remove(ACollider obj)
		{
			if (ActiveStep)
			{
				throw new InvalidOperationException("Currently running a step, cannot remove object");
			}
			else
			{
				bodies.Remove(obj);

				if (obj.Behavior == ColliderType.Static) staticColliders.Remove(obj);
				else if (obj.Behavior == ColliderType.Rigid) rigidColliders.Remove(obj);
				else if (obj.Behavior == ColliderType.Trigger) triggerColliders.Remove(obj);

				obj.BehaviorChanged -= BehaviorChangeHandler;
			}
		}
		private void ForceRemove(ACollider obj)
		{
			bodies.Remove(obj);

			if (obj.Behavior == ColliderType.Static) staticColliders.Remove(obj);
			else if (obj.Behavior == ColliderType.Rigid) rigidColliders.Remove(obj);
			else if (obj.Behavior == ColliderType.Trigger) triggerColliders.Remove(obj);

			obj.BehaviorChanged -= BehaviorChangeHandler;
			obj.Destroy();
		}

		public void Step()
		{
			for (int i = bodies.Count - 1; i >= 0; i--)
			{
				ACollider obj = bodies[i];

				if (obj.ShouldRemove)
				{
					ForceRemove(obj);
				}
			}

			ActiveStep = true;

			foreach (ACollider obj in rigidColliders)
			{
				Step(obj);
			}

			ActiveStep = false;
		}

		private void Step(ACollider obj)
		{
			if (obj.Behavior == ColliderType.Rigid)
			{
				Step_Triggers(obj);

				// Copied from ACollider
				obj.Velocity += new Vector2(0, 0.2);
				obj.Position += obj.Velocity;

				bool collided = false;
				CollisionInfo bestCol = new CollisionInfo();

				foreach (ACollider collider in bodies)
				{
					if (collider == obj || collider.Behavior == ColliderType.Trigger) continue;
					if (obj.Overlapping(collider))
					{
						CollisionInfo colInfo = obj.LastCollision;
						if (colInfo.Depth > bestCol.Depth)
						{
							bestCol = colInfo;
						}
						collided = true;
					}
				}
				if (collided)
				{
					obj.Position -= bestCol.Normal * bestCol.Depth;

					Vector2 q = Vector2.Dot(bestCol.Normal, obj.Velocity) * bestCol.Normal;
					obj.Velocity -= (2 * 0.9f) * q;
				}

				obj.Owner.Position = obj.Position;
				obj.Owner.Rotation = obj.Angle;
			}
		}
		private void Step_Triggers(ACollider obj)
		{
			foreach (ACollider trigger in triggerColliders)
			{
				if (obj.Overlapping(trigger))
				{
					// Should apply forces
					trigger.TriggerMethod(trigger, obj);
				}
			}
		}

		private void BehaviorChangeHandler(object sender, BehaviorChangeEvent args)
		{
			ColliderType oldB = args.OldBehavior;
			ColliderType newB = args.NewBehavior;

			if (oldB == ColliderType.Rigid) rigidColliders.Remove((ACollider)sender);
			else if (oldB == ColliderType.Trigger) triggerColliders.Remove((ACollider)sender);
			else if (oldB == ColliderType.Rigid) rigidColliders.Remove((ACollider)sender);

			if (newB == ColliderType.Rigid) rigidColliders.Add((ACollider)sender);
			else if (newB == ColliderType.Trigger) triggerColliders.Add((ACollider)sender);
			else if (newB == ColliderType.Rigid) rigidColliders.Add((ACollider)sender);
		}
	}
}
