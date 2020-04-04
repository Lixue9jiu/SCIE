using Engine;
using System;
using Engine.Graphics;
using Engine.Input;

namespace Game
{
    public class TelescopeCamera : BasePerspectiveCamera
    {
		//public ComponentMiner ComponentMiner;
		public TelescopeCamera(View view) : base(view)
        {
        }

        public override bool UsesMovementControls
        {
            get { return true; }
        }

        public override bool IsEntityControlEnabled
        {
            get { return true; }
        }
		
		public override void Activate(Camera previousCamera)
        {
            m_angles = new Vector2(0f, (float)Math.Asin(previousCamera.ViewDirection.Y));
            m_angles.X = (float)Math.Acos(previousCamera.ViewDirection.X / Math.Cos(m_angles.Y));
            if (previousCamera.ViewDirection.Z > 0)
            {
                m_angles.X = -m_angles.X;
            }
            SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
			//ComponentMiner = base.Entity.FindComponent<ComponentMiner>(throwOnError: true);
		}

        public override void Update(float dt)
        {
            ComponentPlayer componentPlayer = View.PlayerData.ComponentPlayer;
            if (componentPlayer == null || View.Target == null)
            {
                return;
            }
            ComponentInput componentInput = componentPlayer.ComponentInput;
            Vector3 cameraSneakMove = componentInput.PlayerInput.CameraSneakMove;
            Vector2 cameraLook = componentInput.PlayerInput.CameraLook;
			ComponentMiner componentMiner = componentPlayer.ComponentMiner;
			int num2 = componentMiner.ActiveBlockValue;
			if (num2 != ItemBlock.IdTable["Telescope"])
			{
				var view5 = componentMiner.ComponentPlayer.View;
				view5.ActiveCamera = view5.FindCamera<FppCamera>(true);
				return;
			}
			m_angles.X = MathUtils.NormalizeAngle(m_angles.X - 4f * cameraLook.X * dt + 0.5f * cameraSneakMove.X * dt);
            m_angles.Y = MathUtils.Clamp(MathUtils.NormalizeAngle(m_angles.Y + 4f * cameraLook.Y * dt), MathUtils.DegToRad(-45f), MathUtils.DegToRad(80f));
            m_distance = MathUtils.Clamp(m_distance + 50f * cameraSneakMove.Z * dt, 2f, 100f);
            var v = Vector3.Transform(new Vector3(m_distance, 0f, 0f), Matrix.CreateFromYawPitchRoll(m_angles.X, 0f, m_angles.Y));
            Vector3 vector = View.Target.ComponentBody.BoundingBox.Center()+ new Vector3(0f,0.5f,0f);
            Vector3 vector2 = vector + v;
            if (Vector3.Distance(vector2, m_position) < 100f)
            {
                Vector3 v2 = vector2 - m_position;
                float s = MathUtils.Saturate(10f * dt);
                m_position += s * v2;
            }
            else
            {
                m_position = vector2;
            }
            Vector3 vector3 = m_position - vector;
            float? num = null;
            var vector4 = Vector3.Normalize(Vector3.Cross(vector3, Vector3.UnitY));
            var v3 = Vector3.Normalize(Vector3.Cross(vector3, vector4));
            for (int i = 0; i <= 0; i++)
            {
                for (int j = 0; j <= 0; j++)
                {
                    Vector3 v4 = 0.5f * (vector4 * i + v3 * j);
                    Vector3 v5 = vector + v4;
                    Vector3 end = v5 + vector3 + Vector3.Normalize(vector3) * 0.5f;
                    TerrainRaycastResult? terrainRaycastResult = View.SubsystemViews.SubsystemTerrain.Raycast(v5, end, false, true, (value, distance) => !(Terrain.ExtractContents(value)==0));
                    if (terrainRaycastResult != null)
                    {
                        num = num != null ? MathUtils.Min(num.Value, terrainRaycastResult.Value.Distance) : terrainRaycastResult.Value.Distance;
                    }
                }
            }
            Vector3 vector6 = num != null ? vector + Vector3.Normalize(vector3) * MathUtils.Max(num.Value - 0.5f, 0.2f) : vector + vector3;
            SetupPerspectiveCamera(vector6 + new Vector3(0f, 0.15f, 0f), vector6 - vector, Vector3.UnitY);
        }

        public Vector3 m_position;
        public Vector2 m_angles = new Vector2(0f, MathUtils.DegToRad(30f));
        public float m_distance = 6f;
    }


	public class TelescopeCamera2 : BasePerspectiveCamera
	{
		public TelescopeCamera2(View view) : base(view)
		{
		}

		public override bool UsesMovementControls
		{
			get { return true; }
		}

		public override bool IsEntityControlEnabled
		{
			get { return true; }
		}

		public override void Activate(Camera previousCamera)
		{
			m_angles = new Vector2(0f, (float)Math.Asin(previousCamera.ViewDirection.Y));
			m_angles.X = (float)Math.Acos(previousCamera.ViewDirection.X / Math.Cos(m_angles.Y));
			if (previousCamera.ViewDirection.Z > 0)
			{
				m_angles.X = -m_angles.X;
			}
			SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		//public new Vector3 Dir
		//{
		//	return m_direction;
		//}

		public override void Update(float dt)
		{
			ComponentPlayer componentPlayer = View.PlayerData.ComponentPlayer;
			if (componentPlayer == null || View.Target == null)
			{
				return;
			}
			ComponentInput componentInput = componentPlayer.ComponentInput;
			//componentPlayer.
			//int num = Terrain.ExtractContents(ComponentMiner.ActiveBlockValue);
			ComponentMiner componentMiner = componentPlayer.ComponentMiner;
			int num2 = Terrain.ExtractContents(componentMiner.ActiveBlockValue);
			if (num2 != Musket4Block.Index)
			{
				var view5 = componentMiner.ComponentPlayer.View;
				view5.ActiveCamera = view5.FindCamera<FppCamera>(true);
				return;
			}
			Vector3 cameraSneakMove = componentInput.PlayerInput.CameraSneakMove;
			Vector2 cameraLook = componentInput.PlayerInput.CameraLook;
			m_angles.X = MathUtils.NormalizeAngle(m_angles.X - 4f * cameraLook.X * dt + 0.5f * cameraSneakMove.X * dt);
			m_angles.Y = MathUtils.Clamp(MathUtils.NormalizeAngle(m_angles.Y + 4f * cameraLook.Y * dt), MathUtils.DegToRad(-45f), MathUtils.DegToRad(80f));
			m_distance = MathUtils.Clamp(m_distance + 50f * cameraSneakMove.Z * dt, 2f, 100f);
			var v = Vector3.Transform(new Vector3(m_distance, 0f, 0f), Matrix.CreateFromYawPitchRoll(m_angles.X, 0f, m_angles.Y));
			Vector3 vector = View.Target.ComponentBody.BoundingBox.Center() + new Vector3(0f, 0.5f, 0f);
			Vector3 vector2 = vector + v;
			if (Vector3.Distance(vector2, m_position) < 100f)
			{
				Vector3 v2 = vector2 - m_position;
				float s = MathUtils.Saturate(10f * dt);
				m_position += s * v2;
			}
			else
			{
				m_position = vector2;
			}
			Vector3 vector3 = m_position - vector;
			float? num = null;
			var vector4 = Vector3.Normalize(Vector3.Cross(vector3, Vector3.UnitY));
			var v3 = Vector3.Normalize(Vector3.Cross(vector3, vector4));
			for (int i = 0; i <= 0; i++)
			{
				for (int j = 0; j <= 0; j++)
				{
					Vector3 v4 = 0.5f * (vector4 * i + v3 * j);
					Vector3 vector5 = vector + v4;
					Vector3 end = vector5 + vector3 + Vector3.Normalize(vector3) * 0.5f;
					TerrainRaycastResult? terrainRaycastResult = View.SubsystemViews.SubsystemTerrain.Raycast(vector5, end, false, true, (value, distance) => !(Terrain.ExtractContents(value) == 0));
					if (terrainRaycastResult != null)
					{
						num = (num != null) ? MathUtils.Min(num.Value, terrainRaycastResult.Value.Distance) : terrainRaycastResult.Value.Distance;
					}
				}
			}
			Vector3 vector6 = num != null ? vector + Vector3.Normalize(vector3) * MathUtils.Max(num.Value - 0.5f, 0.2f) : vector + vector3;
			m_direction = Vector3.Normalize(vector6 - vector) * m_distance;
			SetupPerspectiveCamera(vector6 + new Vector3(0f, 0.15f, 0f), vector6 - vector, Vector3.UnitY);
		}

		public Vector3 m_position;
		public static Vector3 m_direction;
		public Vector2 m_angles = new Vector2(0f, MathUtils.DegToRad(30f));
		public float m_distance = 10f;
	}




	

    public class DebugCamera2 : DebugCamera
	{

		public DebugCamera2(View view)
			: base(view)
		{
			ctor1?.Invoke(this, view);
		}
		public override bool UsesMovementControls
		{
			get { return true; }
		}

		public override bool IsEntityControlEnabled
		{
			get { return true; }
		}
		public override void Activate(Camera previousCamera)
		{
			Action<DebugCamera2, Camera> activate = Activate1;
			if (activate != null)
			{
				activate(this, previousCamera);
				return;
			}
			m_position = previousCamera.ViewPosition;
			m_direction = previousCamera.ViewDirection;
			m_position.Y = 120;
			m_direction.X = 0;
			m_direction.Z = 0;
			m_direction.Y = -1;
			SetupPerspectiveCamera(m_position, m_direction, Vector3.UnitY);
		}

		public override void Update(float dt)
		{
			Action<DebugCamera2, float> update = Update1;
			if (update != null)
			{
				update(this, dt);
				return;
			}
			Vector3 zero = Vector3.Zero;
			ComponentPlayer componentPlayer = View.PlayerData.ComponentPlayer;
			//componentPlayer.ComponentInput.
			
			if (componentPlayer.ComponentInput.m_componentGui.m_sneakButtonWidget.IsChecked)
			{
				if (componentPlayer.ComponentInput.m_playerInput.CameraSneakMove.Z< 0)
				{
					zero.Y = -1f;
				}
				if (componentPlayer.ComponentInput.m_playerInput.CameraSneakMove.Z> 0 && m_position.Y<2000)
				{
					zero.Y = 1f;
				}
			}else
			{
				if (componentPlayer.ComponentInput.m_playerInput.CameraSneakMove.X < 0)
				{
					zero.X = -1f;
				}
				if (componentPlayer.ComponentInput.m_playerInput.CameraSneakMove.X > 0)
				{
					zero.X = 1f;
				}
				if (componentPlayer.ComponentInput.m_playerInput.CameraSneakMove.Z > 0)
				{
					zero.Z = 1f;
				}
				if (componentPlayer.ComponentInput.m_playerInput.CameraSneakMove.Z < 0)
				{
					zero.Z = -1f;
				}
			}
			Vector2 vector = 0.03f * new Vector2(Mouse.MouseMovement.X, -Mouse.MouseMovement.Y);
			
			Vector3 direction = m_direction;
			Vector3 unitY = Vector3.UnitY;
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(direction, unitY));
			float num2 = 18f;
			
			
			Vector3 zero2 = Vector3.Zero;
			zero2 += num2 * zero.X * vector2;
			zero2 += num2 * zero.Y * unitY;
			zero2 += num2 * zero.Z * new Vector3(-MathUtils.Sqrt(1-vector2.X* vector2.X),0f, -MathUtils.Sqrt(1 - vector2.Z * vector2.Z));
			TerrainRaycastResult? terrainRaycastResult = View.SubsystemViews.SubsystemTerrain.Raycast(m_position, m_position + 1f * zero2, false, true, (value, distance) => !(Terrain.ExtractContents(value) == 0));
			if (terrainRaycastResult == null)
			{
				m_position += zero2 * dt;
			}
			
			//m_direction = Vector3.Transform(m_direction, Matrix.CreateFromAxisAngle(unitY, -4f * vector.X * dt));
			//m_direction = Vector3.Transform(m_direction, Matrix.CreateFromAxisAngle(vector2, 4f * vector.Y * dt));
			SetupPerspectiveCamera(m_position, m_direction, Vector3.UnitY);
			Vector2 v = base.View.GameWidget.GameViewWidget.ActualSize / 2f;
			FlatBatch2D flatBatch2D = WidgetsManager.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None);
			int count = flatBatch2D.LineVertices.Count;
			flatBatch2D.QueueLine(v - new Vector2(5f, 0f), v + new Vector2(5f, 0f), 0f, Color.White);
			flatBatch2D.QueueLine(v - new Vector2(0f, 5f), v + new Vector2(0f, 5f), 0f, Color.White);
			flatBatch2D.TransformLines(WidgetMatrix, count);
		}
	}

}