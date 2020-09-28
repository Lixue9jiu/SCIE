using Engine;
using System.Collections.Generic;
using System.Linq;


using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{

	public class ComponentNPlayer : ComponentPlayer, IUpdateable
	{
		public double m_time2 = 0f;

		public sealed class c__DisplayClass55_0
		{
			public MovingBlocksRaycastResult? movingBlocksResult;

			public Vector3 position;

			public Vector3 direction;

			public Vector3 creaturePosition;

			public float reach;

			public bool PickTerrainForDigging_b__0(int value, float distance)
			{
				if (movingBlocksResult.HasValue && distance > movingBlocksResult.Value.Distance)
				{
					return false;
				}
				if (Vector3.Distance(position + distance * direction, creaturePosition) <= reach)
				{
					return !(Terrain.ExtractContents(value)==GlassBlock.Index);
				}
				return false;
			}
		}

		public sealed class c__DisplayClass58_0
		{
			public Vector3 position;

			public Vector3 direction;

			public Vector3 creaturePosition;

			public float reach;

			public ComponentMiner __this;

			public bool PickBody_b__0(ComponentBody body, float distance)
			{
				if (Vector3.Distance(position + distance * direction, creaturePosition) <= reach && body.Entity != __this.Entity && !body.IsChildOfBody(__this.ComponentCreature.ComponentBody) && !__this.ComponentCreature.ComponentBody.IsChildOfBody(body))
				{
					return true;
				}
				return false;
			}
		}

		public ComponentCreature ComponentCreature
		{
			get
			{
				return ComponentCreature_;
			}
			set
			{
				ComponentCreature_ = value;
			}
		}
		public ComponentCreature ComponentCreature_;

		public BodyRaycastResult? PickBody2(Vector3 position, Vector3 direction, float distance)
		{
			var c__DisplayClass58_ = new c__DisplayClass58_0
			{
				position = position,
				direction = direction,
				__this = ComponentMiner,
				reach = distance
			};
			c__DisplayClass58_.direction = Vector3.Normalize(c__DisplayClass58_.direction);
			c__DisplayClass58_.creaturePosition = ComponentCreature.ComponentCreatureModel.EyePosition;
			Vector3 end = c__DisplayClass58_.position + c__DisplayClass58_.direction * distance;
			return Utils.SubsystemBodies.Raycast(c__DisplayClass58_.position, end, 0.35f, c__DisplayClass58_.PickBody_b__0);
		}

		public TerrainRaycastResult? PickTerrainForDigging2(Vector3 position, Vector3 direction, float distance)
		{
			var c__DisplayClass55_ = new c__DisplayClass55_0
			{
				position = position,
				direction = direction,
				reach = distance
			};
			c__DisplayClass55_.direction = Vector3.Normalize(c__DisplayClass55_.direction);
			c__DisplayClass55_.creaturePosition = ComponentCreature.ComponentCreatureModel.EyePosition;
			Vector3 end = c__DisplayClass55_.position + c__DisplayClass55_.direction * distance;
			c__DisplayClass55_.movingBlocksResult = m_subsystemMovingBlocks.Raycast(c__DisplayClass55_.position, end, extendToFillCells: true);
			return m_subsystemTerrain.Raycast(c__DisplayClass55_.position, end, true, true, c__DisplayClass55_.PickTerrainForDigging_b__0);
		}
		public SubsystemMovingBlocks m_subsystemMovingBlocks;
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			ComponentCreature = Entity.FindComponent<ComponentCreature>(throwOnError: true);
			m_subsystemMovingBlocks = Project.FindSubsystem<SubsystemMovingBlocks>(throwOnError: true);
		}

		public int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num = ((num & -3841) | ((damage & 0xF) << 8));
			return Terrain.ReplaceData(value, num);
		}
		public int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) >> 8) & 0xF;
		}

		public new void Update(float dt)
		{
			PlayerInput playerInput = ComponentInput.PlayerInput;
			if (ComponentInput.IsControlledByTouch && m_aimDirection.HasValue)
				playerInput.Look = Vector2.Zero;
			if (ComponentMiner.Inventory != null)
			{
				ComponentMiner.Inventory.ActiveSlotIndex = MathUtils.Clamp(ComponentMiner.Inventory.ActiveSlotIndex + playerInput.ScrollInventory, 0, 5);
				if (playerInput.SelectInventorySlot.HasValue)
					ComponentMiner.Inventory.ActiveSlotIndex = MathUtils.Clamp(playerInput.SelectInventorySlot.Value, 0, 5);
			}
			//	if (m_subsystemTime.PeriodicGameTimeEvent(0.5, 0))
			//	{
			//		ReadOnlyList<int> readOnlyList = ComponentClothing.GetClothes(ClothingSlot.Head);
			//		if (readOnlyList.Count > 0 && ClothingBlock.GetClothingData(Terrain.ExtractData(readOnlyList[readOnlyList.Count - 1])).DisplayName == Utils.Get("潜水头盔"))
			//		{
			//			if (ComponentBody.ImmersionFluidBlock != null && ComponentBody.ImmersionFluidBlock.BlockIndex == RottenMeatBlock.Index && ComponentBody.ImmersionDepth > 0.8f)
			//				ComponentScreenOverlays.BlackoutFactor = 1f;
			//			ComponentHealth.Air = 1f;
			//		}
			
			ComponentMount mount = ComponentRider.Mount;
			if (mount != null)
			{
				var componentSteedBehavior = mount.Entity.FindComponent<ComponentSteedBehavior>();
				if (componentSteedBehavior != null)
				{
					if (playerInput.Move.Z > 0.5f && !m_speedOrderBlocked)
					{
						if (PlayerData.PlayerClass == PlayerClass.Male)
							m_subsystemAudio.PlayRandomSound("Audio/Creatures/MaleYellFast", 0.75f, 0f, ComponentBody.Position, 2f, false);
						else
							m_subsystemAudio.PlayRandomSound("Audio/Creatures/FemaleYellFast", 0.75f, 0f, ComponentBody.Position, 2f, false);
						componentSteedBehavior.SpeedOrder = 1;
						m_speedOrderBlocked = true;
					}
					else if (playerInput.Move.Z < -0.5f && !m_speedOrderBlocked)
					{
						if (PlayerData.PlayerClass == PlayerClass.Male)
							m_subsystemAudio.PlayRandomSound("Audio/Creatures/MaleYellSlow", 0.75f, 0f, ComponentBody.Position, 2f, false);
						else
							m_subsystemAudio.PlayRandomSound("Audio/Creatures/FemaleYellSlow", 0.75f, 0f, ComponentBody.Position, 2f, false);
						componentSteedBehavior.SpeedOrder = -1;
						m_speedOrderBlocked = true;
					}
					else if (MathUtils.Abs(playerInput.Move.Z) <= 0.25f)
						m_speedOrderBlocked = false;
					componentSteedBehavior.TurnOrder = playerInput.Move.X;
					componentSteedBehavior.JumpOrder = playerInput.Jump ? 1 : 0;
					ComponentLocomotion.LookOrder = new Vector2(playerInput.Look.X, 0f);
				}
				else
				{
					var componentBoat = mount.Entity.FindComponent<ComponentBoat>();
					if (componentBoat != null || mount.Entity.FindComponent<ComponentBoatI>() != null)
					{
						if (componentBoat != null)
						{
							componentBoat.TurnOrder = playerInput.Move.X;
							componentBoat.MoveOrder = playerInput.Move.Z;
							ComponentLocomotion.LookOrder = new Vector2(playerInput.Look.X, 0f);
						}
						else
							ComponentLocomotion.LookOrder = playerInput.Look;
						ComponentCreatureModel.RowLeftOrder = playerInput.Move.X < -0.2f || playerInput.Move.Z > 0.2f;
						ComponentCreatureModel.RowRightOrder = playerInput.Move.X > 0.2f || playerInput.Move.Z > 0.2f;
					}
					ComponentBody p = mount.ComponentBody;
					while (p.ParentBody != null)
					{
						p = p.ParentBody;
					}
					var c = p.Entity.FindComponent<ComponentLocomotion>();
					if (c != null)
					{
						if (playerInput.ToggleSneak)
						{
							ComponentMount componentMount = ComponentNGui.FindNearestMount(p.Entity, p);
							if (componentMount != null)
							{
								p.Entity.FindComponent<ComponentRider>()?.StartMounting(componentMount);
							}
						}
						c.WalkOrder = playerInput.Move.XZ;
						c.FlyOrder = new Vector3(0f, playerInput.Move.Y, 0f);
						c.TurnOrder = playerInput.Look * new Vector2(1f, 0f);
						c.JumpOrder = playerInput.Jump ? 1 : 0;
						c.LookOrder = playerInput.Look;
						if (playerInput.ToggleCreativeFly)
							c.IsCreativeFlyEnabled = !c.IsCreativeFlyEnabled;
					}
				}
			}
			else
			{
				ComponentLocomotion.WalkOrder = (ComponentLocomotion.WalkOrder ?? Vector2.Zero) + (ComponentBody.IsSneaking ? (0.66f * new Vector2(playerInput.SneakMove.X, playerInput.SneakMove.Z)) : new Vector2(playerInput.Move.X, playerInput.Move.Z));
				ComponentLocomotion.FlyOrder = new Vector3(0f, playerInput.Move.Y, 0f);
				ComponentLocomotion.TurnOrder = playerInput.Look * new Vector2(1f, 0f);
				ComponentLocomotion.JumpOrder = MathUtils.Max(playerInput.Jump ? 1 : 0, ComponentLocomotion.JumpOrder);
			}
			ComponentLocomotion.LookOrder += playerInput.Look * (SettingsManager.FlipVerticalAxis ? new Vector2(0f, -1f) : new Vector2(0f, 1f));
			bool flag = false;
			ReadOnlyList<int> readOnlyList2 = ComponentClothing.GetClothes(ClothingSlot.Torso);
			//if (readOnlyList2.Count > 0 && ClothingBlock.GetClothingData(Terrain.ExtractData(readOnlyList[readOnlyList2.Count - 1])).DisplayName == Utils.Get("防热服"))
			//{
			//if (ComponentBody.ImmersionFluidBlock != null && ComponentBody.ImmersionFluidBlock.BlockIndex == RottenMeatBlock.Index && ComponentBody.ImmersionDepth > 0.8f)
			//	ComponentScreenOverlays.BlackoutFactor = 1f;
			//	ComponentHealth.m_componentOnFire.m_fireDuration = 0f;
			//ComponentHealth.m_componentPlayer.ComponentVitalStats.m_lastTemperature = 10f;
			//this.
			//	ComponentVitalStats.m_temperature = 8f;
			//}
			//	}
			if (readOnlyList2.Count > 0 && ClothingBlock.GetClothingData(Terrain.ExtractData(readOnlyList2[readOnlyList2.Count - 1])).DisplayName == Utils.Get("喷气背包"))
			{
				if (playerInput.Jump && this.ComponentBody.m_position.Y < 200)
					this.ComponentBody.m_velocity += new Vector3(0f, 10f, 0f);
				if (playerInput.Move.Z > 0.5f)
				{
					Vector3 right = ComponentBody.Matrix.Right;
					Vector3 vector2 = Vector3.Transform(ComponentBody.Matrix.Forward, Quaternion.CreateFromAxisAngle(right, ComponentLocomotion.LookAngles.Y));
					Vector3 vector3 = new Vector3(ComponentLocomotion.WalkOrder.Value.X, 0f, ComponentLocomotion.WalkOrder.Value.Y);
					Vector3 v = (ComponentInput.IsControlledByTouch) ? Vector3.Normalize(vector2 + 0.1f * Vector3.UnitY) : Vector3.Normalize(vector2 * new Vector3(1f, 0f, 1f));
					Vector3 v2 = 12f * (right * vector3.X + Vector3.UnitY * vector3.Y + v * vector3.Z);
					float num222 = (vector3 == Vector3.Zero) ? 6f : 3f;
					this.ComponentBody.m_velocity += MathUtils.Saturate(5f * num222 * dt) * (v2 - this.ComponentBody.m_velocity);
				}
				//	this.ComponentBody.m_velocity += 10f * new Vector3(ComponentLocomotion.WalkOrder.Value.X, 0f, ComponentLocomotion.WalkOrder.Value.Y);
				if (this.ComponentBody.m_velocity.Y < -5 && this.ComponentBody.m_position.Y<200)
					this.ComponentBody.m_velocity += new Vector3(0f, 1f, 0f);
				if (m_subsystemTime.PeriodicGameTimeEvent(0.5, 0))
				{
					Utils.SubsystemParticles.AddParticleSystem(new GunSmokeParticleSystem(Utils.SubsystemTerrain, this.ComponentBody.Position + new Vector3(0f, 0.5f, 0f), new Vector3(0f, -1f, 0f)));
					Utils.SubsystemAudio.PlaySound("Audio/Fire", 1f, 0f, this.ComponentBody.Position + new Vector3(0f, 0.5f, 0f), 30f, true);
				}
				//this.ComponentLocomotion.;
			}
			//foreach (Projectile projectile in Utils.SubsystemProjectiles.m_projectiles)
			//{
			//	Vector3 dir = projectile.Position - this.ComponentBody.Position;
			//	if ((dir).Length()<5f)
			//	projectile.Velocity += Vector3.Normalize(dir)*1000f;
			//}
			if (m_subsystemTime.PeriodicGameTimeEvent(0.1, 0))
			if (readOnlyList2.Count > 0 && ClothingBlock.GetClothingData(Terrain.ExtractData(readOnlyList2[readOnlyList2.Count - 1])).DisplayName == Utils.Get("充电背包"))
			{
				int vaaa = ComponentMiner.ActiveBlockValue;
				int vaaa2 = ComponentClothing.m_clothes[ClothingSlot.Torso][0];
				if (Musket5Block.Index== Terrain.ExtractContents(vaaa))
				{
					if (IEBatteryBlock.Index == Terrain.ExtractContents(vaaa) || BlocksManager.DamageItem(vaaa, -3) != 0 && GetDamage(vaaa2) < 15)
					{
						ComponentMiner.DamageActiveTool(-3);
					    if (Utils.Random.Bool(0.01f))
						ComponentClothing.m_clothes[ClothingSlot.Torso][0] = SetDamage(vaaa2, GetDamage(vaaa2)+1);
						//ComponentClothing.RemoveSlotItems(1, 1);// = BlocksManager.DamageItem(vaaa, 1);
						//ComponentClothing.AddSlotItems(1, BlocksManager.DamageItem(vaaa, 1), 1);
					}
				}
				//this.ComponentLocomotion.;
			}
			if (playerInput.Interact.HasValue && !flag && m_subsystemTime.GameTime - m_lastActionTime > 0.33000001311302185)
			{
				Vector3 viewPosition = View.ActiveCamera.ViewPosition;
				var direction = Vector3.Normalize(View.ActiveCamera.ScreenToWorld(new Vector3(playerInput.Interact.Value, 1f), Matrix.Identity) - viewPosition);
				if (!ComponentMiner.Use(viewPosition, direction))
				{
					var body = ComponentMiner.PickBody(viewPosition, direction);
					var result = ComponentMiner.PickTerrainForInteraction(viewPosition, direction);
					if (result.HasValue && (!body.HasValue || result.Value.Distance < body.Value.Distance))
					{
						if (!ComponentMiner.Interact(result.Value))
						{
							if (ComponentMiner.Place(result.Value))
							{
								m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
								flag = true;
								m_isAimBlocked = true;
							}
						}
						else
						{
							m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
							flag = true;
							m_isAimBlocked = true;
						}
					}
				}
				else
				{
					m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
					flag = true;
					m_isAimBlocked = true;
				}
			}
			int num = Terrain.ExtractContents(ComponentMiner.ActiveBlockValue);
			var block = BlocksManager.Blocks[num];
			float num2 = (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || block is Musket2Block) ? 0.1f : 1.4f;
			Vector3 viewPosition2 = View.ActiveCamera.ViewPosition;
			if (mount != null && ComponentMiner.ActiveBlockValue == 0)
			{
				var componentBoat44 = mount.Entity.FindComponent<ComponentEngineT3>();
				if (playerInput.Aim.HasValue && componentBoat44 != null)
				{
					Vector2 value = playerInput.Aim.Value;
					Vector3 v = View.ActiveCamera.ScreenToWorld(new Vector3(value, 1f), Matrix.Identity);
					//Vector3 vv = componentBoat44
					this.ComponentGui.ShowAimingSights(viewPosition2, Vector3.Normalize(v - viewPosition2));
					componentBoat44.vet1 = Vector3.Normalize(v - viewPosition2);
					if (componentBoat44.HeatLevel == 499f)
					{
						componentBoat44.HeatLevel -= 10f;
					}

					//if (componentBoat44.HeatLevel == 500f && m_subsystemTime.GameTime - m_time2>1f)
					//{
					//componentBoat44.HeatLevel -= 10f;
					m_time2 = m_subsystemTime.GameTime;
					//}
					return;
				}

				var componentBoat45 = mount.Entity.FindComponent<ComponentMGun>();
				if (playerInput.Aim.HasValue && componentBoat45 != null)
				{
					Vector2 value = playerInput.Aim.Value;
					Vector3 v = View.ActiveCamera.ScreenToWorld(new Vector3(value, 1f), Matrix.Identity);
					Vector3 vv = mount.ComponentBody.Matrix.Forward;
					Vector3 vv2 = Vector3.Normalize(v - viewPosition2);

					Vector3 vv3 = new Vector3(0f, 0f, 0f);
					if (Vector3.Cross(vv,vv2).Length()>0.5f)
					{
						return;
					}
					this.ComponentGui.ShowAimingSights(viewPosition2, vv2);
					componentBoat45.vect = vv2;
					if (componentBoat45.fire == 0f)
					{
						componentBoat45.fire = 1;
					}

					//if (componentBoat44.HeatLevel == 500f && m_subsystemTime.GameTime - m_time2>1f)
					//{
					//componentBoat44.HeatLevel -= 10f;
					m_time2 = m_subsystemTime.GameTime;
					//}
					return;
				}

				var componentBoat46 = mount.Entity.FindComponent<ComponentCannon>();
				if (playerInput.Aim.HasValue && componentBoat46 != null)
				{
					Vector2 value = playerInput.Aim.Value;
					Vector3 v = View.ActiveCamera.ScreenToWorld(new Vector3(value, 1f), Matrix.Identity);
					Vector3 vv = mount.ComponentBody.Matrix.Forward;
					Vector3 vv2 = Vector3.Normalize(v - viewPosition2);
					
					Vector3 vv3 = new Vector3(0f, 0f, 0f);
					this.ComponentGui.ShowAimingSights(viewPosition2, vv2);
					componentBoat46.vect = vv2;

					//if (componentBoat44.HeatLevel == 500f && m_subsystemTime.GameTime - m_time2>1f)
					//{
					//componentBoat44.HeatLevel -= 10f;
					m_time2 = m_subsystemTime.GameTime;
					//}
					return;
				}
				

				if (m_subsystemTime.GameTime - m_time2 < 2 * dt && m_time2 != 0)
				{
					if (componentBoat46 != null && componentBoat46.fire == 0f)
					{
						m_time2 = 0;
						componentBoat46.fire = 1;
						return;
					}
					if (componentBoat44.HeatLevel == 500f)
						componentBoat44.HeatLevel -= 10f;
					return;

				}
			}
			if (playerInput.Aim.HasValue && block.IsAimable && m_subsystemTime.GameTime - m_lastActionTime > num2)
			{
				if (!m_isAimBlocked)
				{
					Vector2 value = playerInput.Aim.Value;
					Vector3 v = View.ActiveCamera.ScreenToWorld(new Vector3(value, 1f), Matrix.Identity);
					Point2 size = Window.Size;
					if (playerInput.Aim.Value.X >= size.X * 0.1f && playerInput.Aim.Value.X < size.X * 0.9f && playerInput.Aim.Value.Y >= size.Y * 0.1f && playerInput.Aim.Value.Y < size.Y * 0.9f)
					{
						m_aimDirection = Vector3.Normalize(v - viewPosition2);
						if (ComponentMiner.Aim(viewPosition2, m_aimDirection.Value, AimState.InProgress))
						{
							ComponentMiner.Aim(viewPosition2, m_aimDirection.Value, AimState.Cancelled);
							m_aimDirection = null;
							m_isAimBlocked = true;
						}
					}
					else if (m_aimDirection.HasValue)
					{
						ComponentMiner.Aim(viewPosition2, m_aimDirection.Value, AimState.Cancelled);
						m_aimDirection = null;
						m_isAimBlocked = true;
					}
				}
			}
			else
			{
				m_isAimBlocked = false;
				if (m_aimDirection.HasValue)
				{
					ComponentMiner.Aim(viewPosition2, m_aimDirection.Value, AimState.Completed);
					m_aimDirection = null;
					m_lastActionTime = m_subsystemTime.GameTime;
				}
			}
			flag |= m_aimDirection.HasValue;
			if (playerInput.Hit.HasValue && !flag && m_subsystemTime.GameTime - m_lastActionTime > 0.33000001311302185)
			{
				Vector3 viewPosition3 = View.ActiveCamera.ViewPosition;
				var vector = Vector3.Normalize(View.ActiveCamera.ScreenToWorld(new Vector3(playerInput.Hit.Value, 1f), Matrix.Identity) - viewPosition3);
				TerrainRaycastResult? nullable3 = ComponentMiner.PickTerrainForInteraction(viewPosition3, vector);
				BodyRaycastResult? nullable4 = ComponentMiner.PickBody(viewPosition3, vector);
				if (nullable4.HasValue && (!nullable3.HasValue || nullable3.Value.Distance > nullable4.Value.Distance))
				{
					if (ComponentMiner.ActiveBlockValue == 0)
					{
						Widget widget = ComponentNGui.OpenEntity(ComponentMiner.Inventory, nullable4.Value.ComponentBody.Entity);
						if (widget != null)
						{
							ComponentGui.ModalPanelWidget = widget;
							AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
							return;
						}
					}
					flag = true;
					m_isDigBlocked = true;
					if (Vector3.Distance(viewPosition3 + vector * nullable4.Value.Distance, ComponentCreatureModel.EyePosition) <= 2f)
						ComponentMiner.Hit(nullable4.Value.ComponentBody, vector);
				}
			}
			if (playerInput.Dig.HasValue && !flag && !m_isDigBlocked && m_subsystemTime.GameTime - m_lastActionTime > 0.33000001311302185)
			{
				Vector3 viewPosition4 = View.ActiveCamera.ViewPosition;
				Vector3 v2 = View.ActiveCamera.ScreenToWorld(new Vector3(playerInput.Dig.Value, 1f), Matrix.Identity);
				TerrainRaycastResult? nullable5 = ComponentMiner.PickTerrainForDigging(viewPosition4, v2 - viewPosition4);
				if (nullable5.HasValue && Dig(ComponentMiner, nullable5.Value))
				{
					m_lastActionTime = m_subsystemTime.GameTime;
					m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
				}
			}
			if (!playerInput.Dig.HasValue)
				m_isDigBlocked = false;
			if (playerInput.Drop && ComponentMiner.Inventory != null)
			{
				IInventory inventory = ComponentMiner.Inventory;
				int slotValue = inventory.GetSlotValue(inventory.ActiveSlotIndex);
				int num3 = inventory.RemoveSlotItems(count: inventory.GetSlotCount(inventory.ActiveSlotIndex), slotIndex: inventory.ActiveSlotIndex);
				if (slotValue != 0 && num3 != 0)
				{
					Vector3 v3 = ComponentBody.Position + new Vector3(0f, ComponentBody.BoxSize.Y * 0.66f, 0f);
					Matrix matrix = ComponentBody.Matrix;
					Vector3 position = v3 + 0.25f * matrix.Forward;
					matrix = Matrix.CreateFromQuaternion(ComponentCreatureModel.EyeRotation);
					Vector3 value2 = 8f * matrix.Forward;
					m_subsystemPickables.AddPickable(slotValue, num3, position, value2, null);
				}
			}
			if (playerInput.PickBlockType.HasValue && !flag)
			{
				if (ComponentMiner.Inventory is ComponentCreativeInventory componentCreativeInventory)
				{
					Vector3 viewPosition5 = View.ActiveCamera.ViewPosition;
					Vector3 v4 = View.ActiveCamera.ScreenToWorld(new Vector3(playerInput.PickBlockType.Value, 1f), Matrix.Identity);
					TerrainRaycastResult? nullable6 = ComponentMiner.PickTerrainForDigging(viewPosition5, v4 - viewPosition5);
					if (nullable6.HasValue)
					{
						int value3 = nullable6.Value.Value;
						value3 = Terrain.ReplaceLight(value3, 0);
						int num4 = Terrain.ExtractContents(value3);
						var block2 = BlocksManager.Blocks[num4];
						int num5 = 0;
						var creativeValues = block2.GetCreativeValues();
						if (creativeValues.Contains(value3))
							num5 = value3;
						if (num5 == 0 && !block2.IsNonDuplicable)
						{
							var list = new List<BlockDropValue>();
							block2.GetDropValues(m_subsystemTerrain, value3, 0, 2147483647, list, out bool _);
							if (list.Count > 0 && list[0].Count > 0)
								num5 = list[0].Value;
						}
						if (num5 == 0)
							num5 = creativeValues.FirstOrDefault();
						if (num5 != 0)
						{
							int num6 = -1;
							for (int i = 0; i < 6; i++)
							{
								if (componentCreativeInventory.GetSlotCount(i) > 0 && componentCreativeInventory.GetSlotValue(i) == num5)
								{
									num6 = i;
									break;
								}
							}
							if (num6 < 0)
							{
								for (int j = 0; j < 6; j++)
								{
									if (componentCreativeInventory.GetSlotCount(j) == 0 || componentCreativeInventory.GetSlotValue(j) == 0)
									{
										num6 = j;
										break;
									}
								}
							}
							if (num6 < 0)
								num6 = componentCreativeInventory.ActiveSlotIndex;
							componentCreativeInventory.RemoveSlotItems(num6, 2147483647);
							componentCreativeInventory.AddSlotItems(num6, num5, 1);
							componentCreativeInventory.ActiveSlotIndex = num6;
							ComponentGui.DisplaySmallMessage(block2.GetDisplayName(m_subsystemTerrain, value3), false, false);
							m_subsystemAudio.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f, 0f);
						}
					}
				}
			}
			HighlightRaycastResult = ComponentMiner.PickTerrainForDigging(View.ActiveCamera.ViewPosition, View.ActiveCamera.ViewDirection);
		}

		public bool Dig(ComponentMiner m, TerrainRaycastResult raycastResult)
		{
			m.m_lastDigFrameIndex = Time.FrameIndex;
			CellFace cellFace = raycastResult.CellFace;
			int cellValue = m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
			int activeBlockValue = m.ActiveBlockValue;
			int num2 = Terrain.ExtractContents(activeBlockValue);
			int num22 = Terrain.ExtractContents(cellValue);
			Block block2 = BlocksManager.Blocks[num2];
			int x1 = 0;
			if (!m.DigCellFace.HasValue || m.DigCellFace.Value.X != cellFace.X || m.DigCellFace.Value.Y != cellFace.Y || m.DigCellFace.Value.Z != cellFace.Z)
			{
				m.m_digStartTime = m_subsystemTime.GameTime;
				m.DigCellFace = cellFace;
			}
			bool flag2 = Terrain.ExtractContents(activeBlockValue) == IEBatteryBlock.Index,
				 flag3 = flag2 & IEBatteryBlock.GetType(activeBlockValue) == BatteryType.ElectricSaw && 800 != BlocksManager.Blocks[Terrain.ExtractContents(activeBlockValue)].Durability;
			flag2 &= IEBatteryBlock.GetType(activeBlockValue) == BatteryType.ElectricDrill && 800 != BlocksManager.Blocks[Terrain.ExtractContents(activeBlockValue)].Durability;
			float num3 = m.CalculateDigTime(cellValue, num2);
			if (flag2 & IEBatteryBlock.GetType(activeBlockValue) == BatteryType.ElectricDrill && ((Terrain.ExtractData(activeBlockValue) >> 4) & 0xFFF) != BlocksManager.Blocks[Terrain.ExtractContents(activeBlockValue)].Durability)
			{
				num3 = m.CalculateDigTime(cellValue, SteelPickaxeBlock.Index);
			}
			if (flag3)
			{
				num3 = m.CalculateDigTime(cellValue, SteelAxeBlock.Index);
			}
			if (activeBlockValue == ItemBlock.IdTable["Screwdriver"] && (BlocksManager.Blocks[num22].GetCategory(cellValue) == Utils.Get("机器") || BlocksManager.Blocks[num22] is ItemBlock))
			{
				num3 = 0;
			}
			m.m_digProgress = num3 > 0f ? MathUtils.Saturate((float)(m_subsystemTime.GameTime - m.m_digStartTime) / num3) : 1f;
			if (!m.CanUseTool(activeBlockValue))
			{
				m.m_digProgress = 0f;
				if (m_subsystemTime.PeriodicGameTimeEvent(5.0, m.m_digStartTime + 1.0))
				{
					m.ComponentPlayer?.ComponentGui.DisplaySmallMessage(string.Format("Must be level {0} to use {1}", new object[]
					{
						block2.PlayerLevelRequired,
						block2.GetDisplayName(m_subsystemTerrain, activeBlockValue)
					}), true, true);
				}
			}
			bool flag = m.ComponentPlayer != null && !m.ComponentPlayer.ComponentInput.IsControlledByTouch && m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative;
			bool result = false;
			if (flag || (m.m_lastPokingPhase <= 0.5f && m.PokingPhase > 0.5f))
			{
				if (m.m_digProgress >= 1f)
				{
					m.DigCellFace = null;
					if (flag)
					{
						m.Poke(true);
					}
					BlockPlacementData digValue = block.GetDigValue(m_subsystemTerrain, m, cellValue, activeBlockValue, raycastResult);
					if (flag2)
					{
						for (int x2 = cellFace.X - 1; x2 < cellFace.X + 2; x2++)
						{
							for (int y2 = cellFace.Y - 1; y2 < cellFace.Y + 2; y2++)
							{
								for (int z2 = cellFace.Z - 1; z2 < cellFace.Z + 2; z2++)
								{
									int v = m_subsystemTerrain.Terrain.GetCellValue(x2, y2, z2);
									if ((x2 - cellFace.X) * (x2 - cellFace.X) + (y2 - cellFace.Y) * (y2 - cellFace.Y) + (z2 - cellFace.Z) * (z2 - cellFace.Z) <= 1)
									{
										if (BlocksManager.Blocks[Terrain.ExtractContents(v)].IsPlaceable && !BlocksManager.Blocks[Terrain.ExtractContents(v)].IsDiggingTransparent && !BlocksManager.Blocks[Terrain.ExtractContents(v)].DefaultIsInteractive && Terrain.ExtractContents(v) != 31)
										{
											m_subsystemTerrain.ChangeCell(x2, y2, z2, 0);
											if (BlocksManager.Blocks[Terrain.ExtractContents(v)].DefaultCategory != "Plants")
												m_subsystemPickables.AddPickable(v, 1, new Vector3(x2, y2, z2) + new Vector3(0.5f), null, null);
										}
									}
								}
							}
						}
						//	if (m.CalculateDigTime(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X + 1, cellFace.Y, cellFace.Z), SteelPickaxeBlock.Index) <= num3 && m_subsystemTerrain.Terrain.GetCellValue(cellFace.X + 1, cellFace.Y, cellFace.Z)!=0)
						//	{
						//		m_subsystemTerrain.ChangeCell(digValue.CellFace.X + 1, digValue.CellFace.Y, digValue.CellFace.Z, digValue.Value);
						//		m_subsystemPickables.AddPickable(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X + 1, cellFace.Y, cellFace.Z), 1, new Vector3(digValue.CellFace.X+1, digValue.CellFace.Y, digValue.CellFace.Z) + new Vector3(0.5f), null, null);
						//	}
						//	if (m.CalculateDigTime(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X - 1, cellFace.Y, cellFace.Z), SteelPickaxeBlock.Index) <= num3 && m_subsystemTerrain.Terrain.GetCellValue(cellFace.X - 1, cellFace.Y, cellFace.Z)!=0)
						//	{
						//		m_subsystemTerrain.ChangeCell(digValue.CellFace.X - 1, digValue.CellFace.Y, digValue.CellFace.Z, digValue.Value);
						//		m_subsystemPickables.AddPickable(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X - 1, cellFace.Y, cellFace.Z), 1, new Vector3(digValue.CellFace.X-1, digValue.CellFace.Y, digValue.CellFace.Z) + new Vector3(0.5f), null, null);
						//	}
						//	if (m.CalculateDigTime(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y + 1, cellFace.Z), SteelPickaxeBlock.Index) <= num3 && m_subsystemTerrain.Terrain.GetCellValue(cellFace.X , cellFace.Y+1, cellFace.Z) != 0)
						//	{
						//		m_subsystemTerrain.ChangeCell(digValue.CellFace.X, digValue.CellFace.Y + 1, digValue.CellFace.Z, digValue.Value);
						//		m_subsystemPickables.AddPickable(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y + 1, cellFace.Z), 1, new Vector3(digValue.CellFace.X, digValue.CellFace.Y+1, digValue.CellFace.Z) + new Vector3(0.5f), null, null);
						//	}
						//	if (m.CalculateDigTime(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y - 1, cellFace.Z), SteelPickaxeBlock.Index) <= num3 && m_subsystemTerrain.Terrain.GetCellValue(cellFace.X , cellFace.Y-1, cellFace.Z) != 0)
						//	{
						//		m_subsystemTerrain.ChangeCell(digValue.CellFace.X, digValue.CellFace.Y - 1, digValue.CellFace.Z, digValue.Value);
						//		m_subsystemPickables.AddPickable(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y - 1, cellFace.Z), 1, new Vector3(digValue.CellFace.X, digValue.CellFace.Y-1, digValue.CellFace.Z) + new Vector3(0.5f), null, null);
						//	}
						//	if (m.CalculateDigTime(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z + 1), SteelPickaxeBlock.Index) <= num3 && m_subsystemTerrain.Terrain.GetCellValue(cellFace.X , cellFace.Y, cellFace.Z+1) != 0)
						//	{
						//		m_subsystemTerrain.ChangeCell(digValue.CellFace.X, digValue.CellFace.Y, digValue.CellFace.Z + 1, digValue.Value);
						//		m_subsystemPickables.AddPickable(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z + 1), 1, new Vector3(digValue.CellFace.X, digValue.CellFace.Y, digValue.CellFace.Z+1) + new Vector3(0.5f), null, null);
						//	}
						//	if (m.CalculateDigTime(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z - 1), SteelPickaxeBlock.Index) <= num3 && m_subsystemTerrain.Terrain.GetCellValue(cellFace.X , cellFace.Y, cellFace.Z-1) != 0)
						//	{
						//		m_subsystemTerrain.ChangeCell(digValue.CellFace.X, digValue.CellFace.Y, digValue.CellFace.Z - 1, digValue.Value);
						//		m_subsystemPickables.AddPickable(m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z - 1), 1, new Vector3(digValue.CellFace.X, digValue.CellFace.Y, digValue.CellFace.Z-1) + new Vector3(0.5f), null, null);
						//	}
						//m_subsystemTerrain.ChangeCell(digValue.CellFace.X, digValue.CellFace.Y, digValue.CellFace.Z, digValue.Value);
						//m_subsystemPickables.AddPickable(cellValue,1, new Vector3(digValue.CellFace.X, digValue.CellFace.Y, digValue.CellFace.Z) + new Vector3(0.5f),null,null);
					}
					else if (flag3)
					{
						while (true)
						{
							int value2 = m_subsystemTerrain.Terrain.GetCellContentsFast(cellFace.X, cellFace.Y + x1, cellFace.Z);
							if (value2 == OakWoodBlock.Index || value2 == BirchWoodBlock.Index || value2 == SpruceWoodBlock.Index)
							{
								//m_subsystemTerrain.DestroyCell(block2.ToolLevel, digValue.CellFace.X, digValue.CellFace.Y+x1, digValue.CellFace.Z, digValue.Value, false, false);
								x1++;
							}
							else
							{
								break;
							}
						}
						int a = 4;
						m_subsystemTerrain.DestroyCell(block2.ToolLevel, digValue.CellFace.X, digValue.CellFace.Y, digValue.CellFace.Z, digValue.Value, false, false);
						if (x1 > 1)
						{
							for (int x2 = cellFace.X - a; x2 < cellFace.X + a + 1; x2++)
							{
								for (int y2 = cellFace.Y; y2 < cellFace.Y + x1 + 1; y2++)
								{
									for (int z2 = cellFace.Z - a; z2 < cellFace.Z + a + 1; z2++)
									{
										int value2 = m_subsystemTerrain.Terrain.GetCellContentsFast(x2, y2, z2);
										if (value2 == OakWoodBlock.Index || value2 == BirchWoodBlock.Index || value2 == SpruceWoodBlock.Index)
										{
											m_subsystemTerrain.DestroyCell(block2.ToolLevel, x2, y2, z2, digValue.Value, false, false);
										}
										if (value2 == OakLeavesBlock.Index || value2 == BirchLeavesBlock.Index || value2 == SpruceLeavesBlock.Index || value2 == TallSpruceLeavesBlock.Index)
											m_subsystemTerrain.DestroyCell(block2.ToolLevel, x2, y2, z2, digValue.Value, false, false);
									}
								}
							}
						}
					}
					else
						m_subsystemTerrain.DestroyCell(block2.ToolLevel, digValue.CellFace.X, digValue.CellFace.Y, digValue.CellFace.Z, digValue.Value, false, false);
					m.m_subsystemSoundMaterials.PlayImpactSound(cellValue, new Vector3(cellFace.X, cellFace.Y, cellFace.Z), 2f);
					m.DamageActiveTool(1 + x1);
					if (m.ComponentCreature.PlayerStats != null)
					{
						m.ComponentCreature.PlayerStats.BlocksDug++;
					}
					result = true;
				}
				else
				{
					m.m_subsystemSoundMaterials.PlayImpactSound(cellValue, new Vector3(cellFace.X, cellFace.Y, cellFace.Z), 1f);
					Vector3 position = raycastResult.RaycastStart + raycastResult.Distance * Vector3.Normalize(raycastResult.RaycastEnd - raycastResult.RaycastStart) + 0.1f * CellFace.FaceToVector3(cellFace.Face);
					BlockDebrisParticleSystem particleSystem = block.CreateDebrisParticleSystem(m_subsystemTerrain, position, cellValue, 0.35f);
					Project.FindSubsystem<SubsystemParticles>(throwOnError: true).AddParticleSystem(particleSystem);
				}
			}
			return result;
		}
	}


	public class ComponentNHealth : ComponentHealth, IUpdateable
	{
		public new void Update(float dt)
		{
			Vector3 position = m_componentCreature.ComponentBody.Position;
			if (Health > 0f && Health < 1f)
			{
				float num = 0f;
				if (m_componentPlayer != null)
				{
					if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
					{
						num = 0.0166666675f;
					}
					else if (m_componentPlayer.ComponentSleep.SleepFactor == 1f && m_componentPlayer.ComponentVitalStats.Food > 0f)
					{
						num = 0.00166666671f;
					}
					else if (m_componentPlayer.ComponentVitalStats.Food > 0.5f)
					{
						num = 0.00111111114f;
					}
				}
				else
				{
					num = 0.00111111114f;
				}
				Heal(m_subsystemGameInfo.TotalElapsedGameTimeDelta * num);
			}
			if (BreathingMode == BreathingMode.Air)
			{
				int cellContents = m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(position.X), Terrain.ToCell(m_componentCreature.ComponentCreatureModel.EyePosition.Y), Terrain.ToCell(position.Z));
				if (BlocksManager.Blocks[cellContents] is FluidBlock || position.Y > 131f)
				{
					Air = MathUtils.Saturate(Air - dt / AirCapacity);
				}
				else
				{
					Air = 1f;
				}
			}
			else if (BreathingMode == BreathingMode.Water)
			{
				if (m_componentCreature.ComponentBody.ImmersionFactor > 0.25f)
				{
					Air = 1f;
				}
				else
				{
					Air = MathUtils.Saturate(Air - dt / AirCapacity);
				}
			}
			if (m_componentCreature.ComponentBody.ImmersionFactor > 0f && m_componentCreature.ComponentBody.ImmersionFluidBlock is MagmaBlock)
			{
				Injure(2f * m_componentCreature.ComponentBody.ImmersionFactor * dt, null, ignoreInvulnerability: false, "Burned by magma");
				float num2 = 1.1f + 0.1f * (float)MathUtils.Sin(12.0 * m_subsystemTime.GameTime);
				m_redScreenFactor = MathUtils.Max(m_redScreenFactor, num2 * 1.5f * m_componentCreature.ComponentBody.ImmersionFactor);
			}
			float num3 = MathUtils.Abs(m_componentCreature.ComponentBody.CollisionVelocityChange.Y);
			if (!m_wasStanding && num3 > FallResilience)
			{
				float num4 = MathUtils.Sqr(MathUtils.Max(num3 - FallResilience, 0f)) / 15f;
				if (m_componentPlayer != null)
				{
					num4 /= m_componentPlayer.ComponentLevel.ResilienceFactor;
				}
				Injure(num4, null, ignoreInvulnerability: false, "Impact with the ground");
			}
			m_wasStanding = (m_componentCreature.ComponentBody.StandingOnValue.HasValue || m_componentCreature.ComponentBody.StandingOnBody != null);
			//if ((position.Y < 0f || position.Y > 168f) && m_subsystemTime.PeriodicGameTimeEvent(2.0, 0.0))
			//{
			//	Injure(0.1f, null, ignoreInvulnerability: true, "Left the world");
			//	m_componentPlayer?.ComponentGui.DisplaySmallMessage("Come back!", blinking: true, playNotificationSound: false);
			//}
			bool num5 = m_subsystemTime.PeriodicGameTimeEvent(1.0, 0.0);
			if (num5 && Air == 0f)
			{
				float num6 = 0.12f;
				if (m_componentPlayer != null)
				{
					num6 /= m_componentPlayer.ComponentLevel.ResilienceFactor;
				}
				Injure(num6, null, ignoreInvulnerability: false, "Suffocated");
			}
			if (num5 && (m_componentOnFire.IsOnFire || m_componentOnFire.TouchesFire))
			{
				float num7 = 1f / FireResilience;
				if (m_componentPlayer != null)
				{
					num7 /= m_componentPlayer.ComponentLevel.ResilienceFactor;
				}
				Injure(num7, m_componentOnFire.Attacker, ignoreInvulnerability: false, "Burned to death");
			}
			if (num5 && CanStrand && m_componentCreature.ComponentBody.ImmersionFactor < 0.25f && (m_componentCreature.ComponentBody.StandingOnValue != 0 || m_componentCreature.ComponentBody.StandingOnBody != null))
			{
				Injure(0.05f, null, ignoreInvulnerability: false, "Stranded on land");
			}
			HealthChange = Health - m_lastHealth;
			m_lastHealth = Health;
			if (m_redScreenFactor > 0.01f)
			{
				m_redScreenFactor *= MathUtils.Pow(0.2f, dt);
			}
			else
			{
				m_redScreenFactor = 0f;
			}
			if (HealthChange < 0f)
			{
				m_componentCreature.ComponentCreatureSounds.PlayPainSound();
				m_redScreenFactor += -4f * HealthChange;
				m_componentPlayer?.ComponentGui.HealthBarWidget.Flash(MathUtils.Clamp((int)((0f - HealthChange) * 30f), 0, 10));
			}
			if (m_componentPlayer != null)
			{
				m_componentPlayer.ComponentScreenOverlays.RedoutFactor = MathUtils.Max(m_componentPlayer.ComponentScreenOverlays.RedoutFactor, m_redScreenFactor);
			}
			if (m_componentPlayer != null)
			{
				m_componentPlayer.ComponentGui.HealthBarWidget.Value = Health;
			}
			if (Health == 0f && HealthChange < 0f)
			{
				Vector3 position2 = m_componentCreature.ComponentBody.Position + new Vector3(0f, m_componentCreature.ComponentBody.BoxSize.Y / 2f, 0f);
				float x = m_componentCreature.ComponentBody.BoxSize.X;
				m_subsystemParticles.AddParticleSystem(new KillParticleSystem(m_subsystemTerrain, position2, x));
				Vector3 position3 = (m_componentCreature.ComponentBody.BoundingBox.Min + m_componentCreature.ComponentBody.BoundingBox.Max) / 2f;
				foreach (IInventory item in base.Entity.FindComponents<IInventory>())
				{
					item.DropAllItems(position3);
				}
				DeathTime = m_subsystemGameInfo.TotalElapsedGameTime;
			}
			if (Health <= 0f && CorpseDuration > 0f && m_subsystemGameInfo.TotalElapsedGameTime - DeathTime > (double)CorpseDuration)
			{
				m_componentCreature.ComponentSpawn.Despawn();
			}
		}

	}


}