using Engine;
using GameEntitySystem;
using System.Collections.Generic;
using System.Linq;
using TemplatesDatabase;

namespace Game
{
	public class ComponentNPlayer : ComponentPlayer, IUpdateable
	{
		public new void Update(float dt)
		{
			PlayerInput playerInput = ComponentInput.PlayerInput;
			if (ComponentInput.IsControlledByTouch && m_aimDirection.HasValue)
			{
				playerInput.Look = Vector2.Zero;
			}
			if (ComponentMiner.Inventory != null)
			{
				ComponentMiner.Inventory.ActiveSlotIndex = MathUtils.Clamp(ComponentMiner.Inventory.ActiveSlotIndex + playerInput.ScrollInventory, 0, 5);
				if (playerInput.SelectInventorySlot.HasValue)
				{
					ComponentMiner.Inventory.ActiveSlotIndex = MathUtils.Clamp(playerInput.SelectInventorySlot.Value, 0, 5);
				}
			}
			ComponentSteedBehavior componentSteedBehavior = null;
			ComponentBoat componentBoat = null;
			ComponentBoatI componentBoatI = null;
			ComponentMount mount = ComponentRider.Mount;
			if (mount != null)
			{
				componentSteedBehavior = mount.Entity.FindComponent<ComponentSteedBehavior>();
				componentBoat = mount.Entity.FindComponent<ComponentBoat>();
				componentBoatI = mount.Entity.FindComponent<ComponentBoatI>();
			}
			if (componentSteedBehavior != null)
			{
				if (playerInput.Move.Z > 0.5f && !m_speedOrderBlocked)
				{
					if (PlayerData.PlayerClass == PlayerClass.Male)
					{
						m_subsystemAudio.PlayRandomSound("Audio/Creatures/MaleYellFast", 0.75f, 0f, ComponentBody.Position, 2f, false);
					}
					else
					{
						m_subsystemAudio.PlayRandomSound("Audio/Creatures/FemaleYellFast", 0.75f, 0f, ComponentBody.Position, 2f, false);
					}
					componentSteedBehavior.SpeedOrder = 1;
					m_speedOrderBlocked = true;
				}
				else if (playerInput.Move.Z < -0.5f && !m_speedOrderBlocked)
				{
					if (PlayerData.PlayerClass == PlayerClass.Male)
					{
						m_subsystemAudio.PlayRandomSound("Audio/Creatures/MaleYellSlow", 0.75f, 0f, ComponentBody.Position, 2f, false);
					}
					else
					{
						m_subsystemAudio.PlayRandomSound("Audio/Creatures/FemaleYellSlow", 0.75f, 0f, ComponentBody.Position, 2f, false);
					}
					componentSteedBehavior.SpeedOrder = -1;
					m_speedOrderBlocked = true;
				}
				else if (MathUtils.Abs(playerInput.Move.Z) <= 0.25f)
				{
					m_speedOrderBlocked = false;
				}
				componentSteedBehavior.TurnOrder = playerInput.Move.X;
				componentSteedBehavior.JumpOrder = (float)(playerInput.Jump ? 1 : 0);
				ComponentLocomotion.LookOrder = new Vector2(playerInput.Look.X, 0f);
			}
			else if (componentBoat != null)
			{
				componentBoat.TurnOrder = playerInput.Move.X;
				componentBoat.MoveOrder = playerInput.Move.Z;
				ComponentLocomotion.LookOrder = new Vector2(playerInput.Look.X, 0f);
				ComponentCreatureModel.RowLeftOrder = (playerInput.Move.X < -0.2f || playerInput.Move.Z > 0.2f);
				ComponentCreatureModel.RowRightOrder = (playerInput.Move.X > 0.2f || playerInput.Move.Z > 0.2f);
			}
			else if (componentBoatI != null)
			{
				componentBoatI.TurnOrder = playerInput.Move.X;
				componentBoatI.MoveOrder = playerInput.Move.Z;
				ComponentLocomotion.LookOrder = new Vector2(playerInput.Look.X, 0f);
				ComponentCreatureModel.RowLeftOrder = (playerInput.Move.X < -0.2f || playerInput.Move.Z > 0.2f);
				ComponentCreatureModel.RowRightOrder = (playerInput.Move.X > 0.2f || playerInput.Move.Z > 0.2f);
			}
			else
			{
				ComponentLocomotion.WalkOrder = (ComponentBody.IsSneaking ? (0.66f * new Vector2(playerInput.SneakMove.X, playerInput.SneakMove.Z)) : new Vector2(playerInput.Move.X, playerInput.Move.Z));
				ComponentLocomotion.FlyOrder = new Vector3(0f, playerInput.Move.Y, 0f);
				ComponentLocomotion.TurnOrder = playerInput.Look * new Vector2(1f, 0f);
				ComponentLocomotion.JumpOrder = MathUtils.Max((float)(playerInput.Jump ? 1 : 0), ComponentLocomotion.JumpOrder);
			}
			ComponentLocomotion.LookOrder += playerInput.Look * (SettingsManager.FlipVerticalAxis ? new Vector2(0f, -1f) : new Vector2(0f, 1f));
			int num = Terrain.ExtractContents(ComponentMiner.ActiveBlockValue);
			Block block = BlocksManager.Blocks[num];
			bool flag = false;
			if (playerInput.Interact.HasValue && !flag && m_subsystemTime.GameTime - m_lastActionTime > 0.33000001311302185)
			{
				Vector3 viewPosition = View.ActiveCamera.ViewPosition;
				Vector3 direction = Vector3.Normalize(View.ActiveCamera.ScreenToWorld(new Vector3(playerInput.Interact.Value, 1f), Matrix.Identity) - viewPosition);
				if (!ComponentMiner.Use(viewPosition, direction))
				{
					BodyRaycastResult? nullable = ComponentMiner.PickBody(viewPosition, direction);
					TerrainRaycastResult? nullable2 = ComponentMiner.PickTerrainForInteraction(viewPosition, direction);
					if (nullable2.HasValue && (!nullable.HasValue || nullable2.Value.Distance < nullable.Value.Distance))
					{
						if (!ComponentMiner.Interact(nullable2.Value))
						{
							if (ComponentMiner.Place(nullable2.Value))
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
			float num2 = (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) ? 0.1f : 1.4f;
			Vector3 viewPosition2 = View.ActiveCamera.ViewPosition;
			if (block.BlockIndex == 524)
			{
				num2 = 0.1f;
			}
			if (playerInput.Aim.HasValue && block.IsAimable && m_subsystemTime.GameTime - m_lastActionTime > (double)num2)
			{
				if (!m_isAimBlocked)
				{
					Vector2 value = playerInput.Aim.Value;
					Vector3 v = View.ActiveCamera.ScreenToWorld(new Vector3(value, 1f), Matrix.Identity);
					Point2 size = Window.Size;
					if (playerInput.Aim.Value.X >= (float)size.X * 0.1f && playerInput.Aim.Value.X < (float)size.X * 0.9f && playerInput.Aim.Value.Y >= (float)size.Y * 0.1f && playerInput.Aim.Value.Y < (float)size.Y * 0.9f)
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
				Vector3 vector = Vector3.Normalize(View.ActiveCamera.ScreenToWorld(new Vector3(playerInput.Hit.Value, 1f), Matrix.Identity) - viewPosition3);
				TerrainRaycastResult? nullable3 = ComponentMiner.PickTerrainForInteraction(viewPosition3, vector);
				BodyRaycastResult? nullable4 = ComponentMiner.PickBody(viewPosition3, vector);
				ComponentEngine2 componentEngine = nullable4.Value.ComponentBody.Entity.FindComponent<ComponentEngine2>();
				if (componentEngine != null)
				{
					ComponentGui.ModalPanelWidget = new Engine2Widget(ComponentMiner.Inventory, componentEngine);
					AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					return;
				}
				if (nullable4.HasValue && (!nullable3.HasValue || nullable3.Value.Distance > nullable4.Value.Distance))
				{
					flag = true;
					m_isDigBlocked = true;
					if (Vector3.Distance(viewPosition3 + vector * nullable4.Value.Distance, ComponentCreatureModel.EyePosition) <= 2f)
					{
						ComponentMiner.Hit(nullable4.Value.ComponentBody, vector);
					}
				}
			}
			if (playerInput.Dig.HasValue && !flag && !m_isDigBlocked && m_subsystemTime.GameTime - m_lastActionTime > 0.33000001311302185)
			{
				Vector3 viewPosition4 = View.ActiveCamera.ViewPosition;
				Vector3 v2 = View.ActiveCamera.ScreenToWorld(new Vector3(playerInput.Dig.Value, 1f), Matrix.Identity);
				TerrainRaycastResult? nullable5 = ComponentMiner.PickTerrainForDigging(viewPosition4, v2 - viewPosition4);
				if (nullable5.HasValue && ComponentMiner.Dig(nullable5.Value))
				{
					m_lastActionTime = m_subsystemTime.GameTime;
					m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
				}
			}
			if (!playerInput.Dig.HasValue)
			{
				m_isDigBlocked = false;
			}
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
				ComponentCreativeInventory componentCreativeInventory = ComponentMiner.Inventory as ComponentCreativeInventory;
				if (componentCreativeInventory != null)
				{
					Vector3 viewPosition5 = View.ActiveCamera.ViewPosition;
					Vector3 v4 = View.ActiveCamera.ScreenToWorld(new Vector3(playerInput.PickBlockType.Value, 1f), Matrix.Identity);
					TerrainRaycastResult? nullable6 = ComponentMiner.PickTerrainForDigging(viewPosition5, v4 - viewPosition5);
					if (nullable6.HasValue)
					{
						int value3 = nullable6.Value.Value;
						value3 = Terrain.ReplaceLight(value3, 0);
						int num4 = Terrain.ExtractContents(value3);
						Block block2 = BlocksManager.Blocks[num4];
						int num5 = 0;
						IEnumerable<int> creativeValues = block2.GetCreativeValues();
						if (block2.GetCreativeValues().Contains(value3))
						{
							num5 = value3;
						}
						if (num5 == 0 && !block2.IsNonDuplicable)
						{
							List<BlockDropValue> list = new List<BlockDropValue>();
							bool _;
							block2.GetDropValues(m_subsystemTerrain, value3, 0, 2147483647, list, out _);
							if (list.Count > 0 && list[0].Count > 0)
							{
								num5 = list[0].Value;
							}
						}
						if (num5 == 0)
						{
							num5 = creativeValues.FirstOrDefault();
						}
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
							{
								num6 = componentCreativeInventory.ActiveSlotIndex;
							}
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

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_subsystemGameInfo = Project.FindSubsystem<SubsystemGameInfo>(true);
			m_subsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			m_subsystemPickables = Project.FindSubsystem<SubsystemPickables>(true);
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
			ComponentGui = Entity.FindComponent<ComponentNGui>(true);
			ComponentInput = Entity.FindComponent<ComponentInput>(true);
			ComponentScreenOverlays = Entity.FindComponent<ComponentScreenOverlays>(true);
			ComponentMiner = Entity.FindComponent<ComponentMiner>(true);
			ComponentRider = Entity.FindComponent<ComponentRider>(true);
			ComponentSleep = Entity.FindComponent<ComponentSleep>(true);
			ComponentVitalStats = Entity.FindComponent<ComponentVitalStats>(true);
			ComponentSickness = Entity.FindComponent<ComponentSickness>(true);
			ComponentFlu = Entity.FindComponent<ComponentFlu>(true);
			ComponentLevel = Entity.FindComponent<ComponentLevel>(true);
			ComponentClothing = Entity.FindComponent<ComponentClothing>(true);
			ComponentOuterClothingModel = Entity.FindComponent<ComponentOuterClothingModel>(true);
			int playerIndex = valuesDictionary.GetValue<int>("PlayerIndex");
			PlayerData = Project.FindSubsystem<SubsystemPlayers>(true).PlayersData.First((PlayerData d) => d.PlayerIndex == playerIndex);
		}
	}
}
