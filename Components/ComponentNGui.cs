using Engine;
using GameEntitySystem;
using System.Linq;
using TemplatesDatabase;

namespace Game
{
	public class ComponentNGui : ComponentGui, IUpdateable
	{
		public ComponentGui ComponentGui;
		public new void Update(float dt)
		{
			HandleInput();
			ComponentGui.UpdateWidgets();
		}

		public new void HandleInput()
		{
			WidgetInput input = m_componentPlayer.View.Input;
			PlayerInput playerInput = m_componentPlayer.ComponentInput.PlayerInput;
			ComponentRider componentRider = m_componentPlayer.ComponentRider;
			if (m_componentPlayer.View.ActiveCamera.IsEntityControlEnabled)
			{
				if (!m_keyboardHelpMessageShown && (m_componentPlayer.PlayerData.InputDevice & WidgetInputDevice.Keyboard) != 0 && Time.PeriodicEvent(7.0, 0.0))
				{
					m_keyboardHelpMessageShown = true;
					DisplaySmallMessage(Utils.Get("按 H 键查看键盘控制帮助\n(或看帮助)"), true, true);
				}
				else if (!m_gamepadHelpMessageShown && (m_componentPlayer.PlayerData.InputDevice & WidgetInputDevice.Gamepads) != 0 && Time.PeriodicEvent(7.0, 0.0))
				{
					m_gamepadHelpMessageShown = true;
					DisplaySmallMessage(Utils.Get("按 START/PAUSE 键查看手柄控制帮助\n(或看帮助)"), true, true);
				}
			}
			if (playerInput.KeyboardHelp)
			{
				if (m_keyboardHelpDialog == null)
					m_keyboardHelpDialog = new KeyboardHelpDialog();
				if (m_keyboardHelpDialog.ParentWidget != null)
					DialogsManager.HideDialog(m_keyboardHelpDialog);
				else
					DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, m_keyboardHelpDialog);
			}
			if (playerInput.GamepadHelp)
			{
				if (m_gamepadHelpDialog == null)
					m_gamepadHelpDialog = new GamepadHelpDialog();
				if (m_gamepadHelpDialog.ParentWidget != null)
					DialogsManager.HideDialog(m_gamepadHelpDialog);
				else
					DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, m_gamepadHelpDialog);
			}
			if (m_helpButtonWidget.IsClicked)
				ScreensManager.SwitchScreen("Help");
			IInventory inventory = m_componentPlayer.ComponentMiner.Inventory;
			if (playerInput.ToggleInventory || m_inventoryButtonWidget.IsClicked)
			{
				if (componentRider.Mount != null && ModalPanelWidget == null)
				{
					Widget widget = OpenEntity(inventory, m_componentPlayer.ComponentRider.Mount.Entity);
					if (widget != null)
					{
						ModalPanelWidget = widget;
						AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
						return;
					}
				}
				ModalPanelWidget = IsInventoryVisible()
					? null
					: inventory is ComponentCreativeInventory
					? new CreativeInventoryWidget(m_componentPlayer.Entity)
					: (Widget)new FullInventoryWidget(inventory, m_componentPlayer.Entity.FindComponent<ComponentCraftingTable>(true));
			}
			if (playerInput.ToggleClothing || m_clothingButtonWidget.IsClicked)
			{
				ModalPanelWidget = IsClothingVisible() ? null : new ClothingWidget(m_componentPlayer);
			}
			if (m_sneakButtonWidget.IsClicked || playerInput.ToggleSneak)
			{
				bool isSneaking = m_componentPlayer.ComponentBody.IsSneaking;
				m_componentPlayer.ComponentBody.IsSneaking = !isSneaking;
				if (m_componentPlayer.ComponentBody.IsSneaking != isSneaking)
				{
					DisplaySmallMessage(Utils.Get(m_componentPlayer.ComponentBody.IsSneaking ? "潜行模式：开" : "潜行模式：关"), false, false);
				}
			}
			if (componentRider != null && (m_mountButtonWidget.IsClicked || playerInput.ToggleMount))
			{
				bool flag = componentRider.Mount != null;
				if (flag)
					componentRider.StartDismounting();
				else
				{
					ComponentMount componentMount = FindNearestMount(componentRider);
					if (componentMount != null)
						componentRider.StartMounting(componentMount);
				}
				if (componentRider.Mount != null != flag)
				{
					DisplaySmallMessage(Utils.Get(componentRider.Mount != null ? "上马" : "下马"), false, false);
				}
			}
			if ((m_editItemButton.IsClicked || playerInput.EditItem) && m_nearbyEditableCell.HasValue)
			{
				int cellValue = m_subsystemTerrain.Terrain.GetCellValue(m_nearbyEditableCell.Value.X, m_nearbyEditableCell.Value.Y, m_nearbyEditableCell.Value.Z);
				int contents = Terrain.ExtractContents(cellValue);
				SubsystemBlockBehavior[] blockBehaviors = m_subsystemBlockBehaviors.GetBlockBehaviors(contents);
				for (int i = 0; i < blockBehaviors.Length && !blockBehaviors[i].OnEditBlock(m_nearbyEditableCell.Value.X, m_nearbyEditableCell.Value.Y, m_nearbyEditableCell.Value.Z, cellValue, m_componentPlayer); i++)
				{
				}
			}
			else if ((m_editItemButton.IsClicked || playerInput.EditItem) && IsActiveSlotEditable())
			{
				if (inventory != null)
				{
					int activeSlotIndex = inventory.ActiveSlotIndex;
					int num = Terrain.ExtractContents(inventory.GetSlotValue(activeSlotIndex));
					if (BlocksManager.Blocks[num].IsEditable)
					{
						SubsystemBlockBehavior[] blockBehaviors2 = m_subsystemBlockBehaviors.GetBlockBehaviors(num);
						for (int j = 0; j < blockBehaviors2.Length && !blockBehaviors2[j].OnEditInventoryItem(inventory, activeSlotIndex, m_componentPlayer); j++)
						{
						}
					}
				}
			}
			bool isCreative = m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative;
			if (isCreative && (m_creativeFlyButtonWidget.IsClicked || playerInput.ToggleCreativeFly) && componentRider.Mount == null)
			{
				bool isCreativeFlyEnabled = m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled;
				m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled = !isCreativeFlyEnabled;
				if (m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled != isCreativeFlyEnabled)
				{
					if (m_componentPlayer.ComponentLocomotion.IsCreativeFlyEnabled)
					{
						m_componentPlayer.ComponentLocomotion.JumpOrder = 1f;
						DisplaySmallMessage(Utils.Get("飞行模式：开"), false, false);
					}
					else
						DisplaySmallMessage(Utils.Get("飞行模式：关"), false, false);
				}
			}
			if (m_cameraButtonWidget.IsClicked || playerInput.SwitchCameraMode)
			{
				View view = m_componentPlayer.View;
				if (view.ActiveCamera.GetType() == typeof(FppCamera))
				{
					view.ActiveCamera = view.FindCamera<TppCamera>(true);
					DisplaySmallMessage(Utils.Get("第三人称视角"), false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(TppCamera))
				{
					view.ActiveCamera = view.FindCamera<OrbitCamera>(true);
					DisplaySmallMessage(Utils.Get("滑轨视角"), false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(OrbitCamera))
				{
					view.ActiveCamera = isCreative ? (Camera)new DebugCamera(view) : view.FindCamera<FixedCamera>(true);
					DisplaySmallMessage(Utils.Get(isCreative ? "调试视角" : "固定视角"), false, false);
				}
				else if (isCreative && view.ActiveCamera.GetType() == typeof(FixedCamera))
				{
					view.ActiveCamera = new FlyCamera(view);
					DisplaySmallMessage(Utils.Get("飞行视角"), false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(DebugCamera))
				{
					view.ActiveCamera = view.FindCamera<FixedCamera>(true);
					DisplaySmallMessage(Utils.Get("固定视角"), false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(FlyCamera))
				{
					view.ActiveCamera = new RandomJumpCamera(view);
					DisplaySmallMessage(Utils.Get("随机跳跃视角"), false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(RandomJumpCamera))
				{
					view.ActiveCamera = new StraightFlightCamera(view);
					DisplaySmallMessage(Utils.Get("垂直飞行视角"), false, false);
				}
				else
				{
					view.ActiveCamera = view.FindCamera<FppCamera>(true);
					DisplaySmallMessage(Utils.Get("第一人称视角"), false, false);
				}
			}
			if (m_photoButtonWidget.IsClicked || playerInput.TakeScreenshot)
			{
				ScreenCaptureManager.CapturePhoto();
				Time.QueueFrameCountDelayedExecution(Time.FrameIndex + 1, delegate
				{
					DisplaySmallMessage(Utils.Get("照片已保存到图片相册"), false, false);
				});
			}
			if (isCreative && (m_lightningButtonWidget.IsClicked || playerInput.Lighting))
			{
				Project.FindSubsystem<SubsystemWeather>(true).ManualLightingStrike(m_componentPlayer.ComponentCreatureModel.EyePosition, Matrix.CreateFromQuaternion(m_componentPlayer.ComponentCreatureModel.EyeRotation).Forward);
			}
			if (isCreative && (m_timeOfDayButtonWidget.IsClicked || playerInput.TimeOfDay))
			{
				float num2 = MathUtils.Remainder(0.25f, 1f);
				float num3 = MathUtils.Remainder(0.5f, 1f);
				float num4 = MathUtils.Remainder(0.75f, 1f);
				float num5 = MathUtils.Remainder(1f, 1f);
				float num6 = MathUtils.Remainder(num2 - m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num7 = MathUtils.Remainder(num3 - m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num8 = MathUtils.Remainder(num4 - m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num9 = MathUtils.Remainder(num5 - m_subsystemTimeOfDay.TimeOfDay, 1f);
				float num10 = MathUtils.Min(num6, num7, num8, num9);
				if (num6 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num6;
					DisplaySmallMessage(Utils.Get("黎明"), false, false);
				}
				else if (num7 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num7;
					DisplaySmallMessage(Utils.Get("中午"), false, false);
				}
				else if (num8 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num8;
					DisplaySmallMessage(Utils.Get("黄昏"), false, false);
				}
				else if (num9 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num9;
					DisplaySmallMessage(Utils.Get("午夜"), false, false);
				}
			}
			if (ModalPanelWidget != null)
			{
				if (input.Cancel || input.Back || m_backButtonWidget.IsClicked)
					ModalPanelWidget = null;
			}
			else if (input.Back || m_backButtonWidget.IsClicked)
				DialogsManager.ShowDialog(m_componentPlayer.View.GameWidget, new GameMenuDialog(m_componentPlayer));
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			ComponentGui = Entity.FindComponent<ComponentGui>(true);
			base.Load(valuesDictionary, idToEntityMap);
		}

		public ComponentMount FindNearestMount(ComponentRider rider)
		{
			var bodies = new DynamicArray<ComponentBody>();
			var body = rider.ComponentCreature.ComponentBody;
			Utils.SubsystemBodies.FindBodiesAroundPoint(new Vector2(body.Position.X, body.Position.Z), 2.5f, bodies);
			float num = 0f;
			ComponentMount result = null;
			foreach (ComponentMount m in bodies.Select(GetMount))
			{
				if (m == null || m.Entity == Entity) continue;
				ComponentBody p = m.ComponentBody;
				while (p.ParentBody != null)
				{
					p = p.ParentBody;
					if (p.Entity == Entity) goto b;
				}
				float score = 0f;
				const float maxDistance = 7f;
				if (m.ComponentBody.Velocity.LengthSquared() < 1f)
				{
					var v = m.ComponentBody.Position + Vector3.Transform(m.MountOffset, m.ComponentBody.Rotation) - body.Position;
					if (v.LengthSquared() < maxDistance)
						score = maxDistance - v.Length();
				}
				if (score > num)
				{
					num = score;
					result = m;
				}
			b:;
			}
			return result;
		}

		public static Widget OpenEntity(IInventory inventory, Entity entity)
		{
			var componentTrain = entity.FindComponent<ComponentTrain>();
			var componentChest = entity.FindComponent<ComponentChest>();
			if (componentTrain != null)
				return componentChest != null
					? new NewChestWidget(inventory, componentChest, Utils.Get(componentTrain.ParentBody != null ? "车厢（已连接）" : "车厢"))
					: (Widget)new StoveWidget(inventory, componentTrain.ComponentEngine, "Widgets/EngineWidget", "SteamLocomotive");
			var componentEngine = entity.FindComponent<ComponentEngine>();
			if (componentEngine != null)
				return new Engine2Widget(inventory, componentEngine);
			var componentEngine3 = entity.FindComponent<ComponentEngineA>();
			if (componentEngine3 != null)
				return new EngineAWidget(inventory, componentEngine3);
			return null;
		}

		public static ComponentMount GetMount(Component b)
		{
			return b.Entity.FindComponent<ComponentMount>();
		}
	}
}