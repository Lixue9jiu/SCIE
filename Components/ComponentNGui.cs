using Engine;
using GameEntitySystem;
using System.Linq;
using TemplatesDatabase;
using Engine.Graphics;
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
					DisplaySmallMessage(Utils.Get("�� H ���鿴���̿��ư���\n(�򿴰���)"), true, true);
				}
				else if (!m_gamepadHelpMessageShown && (m_componentPlayer.PlayerData.InputDevice & WidgetInputDevice.Gamepads) != 0 && Time.PeriodicEvent(7.0, 0.0))
				{
					m_gamepadHelpMessageShown = true;
					DisplaySmallMessage(Utils.Get("�� START/PAUSE ���鿴�ֱ����ư���\n(�򿴰���)"), true, true);
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
					DisplaySmallMessage(Utils.Get(m_componentPlayer.ComponentBody.IsSneaking ? "Ǳ��ģʽ����" : "Ǳ��ģʽ����"), false, false);
				}
			}
			if (componentRider != null && (m_mountButtonWidget.IsClicked || playerInput.ToggleMount))
			{
				bool flag = componentRider.Mount != null;
				if (flag)
					componentRider.StartDismounting();
				else
				{
					ComponentMount componentMount = FindNearestMount(Entity, componentRider.ComponentCreature.ComponentBody);
					if (componentMount != null)
						componentRider.StartMounting(componentMount);
				}
				if (componentRider.Mount != null != flag)
				{
					DisplaySmallMessage(Utils.Get(componentRider.Mount != null ? "����" : "����"), false, false);
					//componentRider.Mount.ComponentBody.Entity.FindComponent<ComponentCar>(true).SetModel(ContentManager.Get<Model>("Models/Tank"));
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
						DisplaySmallMessage(Utils.Get("����ģʽ����"), false, false);
					}
					else
						DisplaySmallMessage(Utils.Get("����ģʽ����"), false, false);
				}
			}
			if (m_cameraButtonWidget.IsClicked || playerInput.SwitchCameraMode)
			{
				View view = m_componentPlayer.View;
				if (view.ActiveCamera.GetType() == typeof(FppCamera))
				{
					view.ActiveCamera = view.FindCamera<TppCamera>(true);
					DisplaySmallMessage(Utils.Get("�����˳��ӽ�"), false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(TppCamera))
				{
					view.ActiveCamera = view.FindCamera<OrbitCamera>(true);
					DisplaySmallMessage(Utils.Get("�����ӽ�"), false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(OrbitCamera))
				{
					view.ActiveCamera = isCreative ? (Camera)new DebugCamera(view) : view.FindCamera<FixedCamera>(true);
					DisplaySmallMessage(Utils.Get(isCreative ? "�����ӽ�" : "�̶��ӽ�"), false, false);
				}
				else if (isCreative && view.ActiveCamera.GetType() == typeof(FixedCamera))
				{
					view.ActiveCamera = new FlyCamera(view);
					DisplaySmallMessage(Utils.Get("�����ӽ�"), false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(DebugCamera))
				{
					view.ActiveCamera = view.FindCamera<FixedCamera>(true);
					DisplaySmallMessage(Utils.Get("�̶��ӽ�"), false, false);
				}
#if !DEBUG
				else if (view.ActiveCamera.GetType() == typeof(FlyCamera))
				{
					view.ActiveCamera = new RandomJumpCamera(view);
					DisplaySmallMessage(Utils.Get("�����Ծ�ӽ�"), false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(RandomJumpCamera))
				{
					view.ActiveCamera = new StraightFlightCamera(view);
					DisplaySmallMessage(Utils.Get("��ֱ�����ӽ�"), false, false);
				}
#endif
				else
				{
					view.ActiveCamera = view.FindCamera<FppCamera>(true);
					DisplaySmallMessage(Utils.Get("��һ�˳��ӽ�"), false, false);
				}
			}
			if (m_photoButtonWidget.IsClicked || playerInput.TakeScreenshot)
			{
				ScreenCaptureManager.CapturePhoto();
				Time.QueueFrameCountDelayedExecution(Time.FrameIndex + 1, delegate
				{
					DisplaySmallMessage(Utils.Get("��Ƭ�ѱ��浽ͼƬ���"), false, false);
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
					DisplaySmallMessage(Utils.Get("����"), false, false);
				}
				else if (num7 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num7;
					DisplaySmallMessage(Utils.Get("����"), false, false);
				}
				else if (num8 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num8;
					DisplaySmallMessage(Utils.Get("�ƻ�"), false, false);
				}
				else if (num9 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num9;
					DisplaySmallMessage(Utils.Get("��ҹ"), false, false);
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

		public static ComponentMount FindNearestMount(Entity entity, ComponentBody body)
		{
			var bodies = new DynamicArray<ComponentBody>();
			Utils.SubsystemBodies.FindBodiesAroundPoint(new Vector2(body.Position.X, body.Position.Z), 2.5f, bodies);
			float num = 0f;
			ComponentMount result = null;
			foreach (ComponentMount m in bodies.Select(GetMount))
			{
				if (m == null || m.Entity == entity) continue;
				ComponentBody p = m.ComponentBody;
				while (p.ParentBody != null)
				{
					p = p.ParentBody;
					if (p.Entity == entity) goto b;
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
					? new NewChestWidget(inventory, componentChest, Utils.Get(componentTrain.ParentBody != null ? "���ᣨ�����ӣ�" : "����"))
					: (Widget)new StoveWidget(inventory, componentTrain.ComponentEngine, "Widgets/EngineWidget", "SteamLocomotive");
			var componentEngine5 = entity.FindComponent<ComponentEngineT2>();
			if (componentEngine5 != null)
				return new EngineTWidget(inventory, componentEngine5, "Widgets/DiggerWidget");
			var componentEngine6 = entity.FindComponent<ComponentEngineT3>();
			if (componentEngine6 != null)
				return new EngineTWidget(inventory, componentEngine6, "Widgets/TankWidget");
			var componentEngine4 = entity.FindComponent<ComponentEngineT>();
			if (componentEngine4 != null)
				return new EngineTWidget(inventory, componentEngine4);
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