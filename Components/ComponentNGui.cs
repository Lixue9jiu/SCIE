using Engine;
using GameEntitySystem;
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
					DisplaySmallMessage("Press H for keyboard controls\n(or see HELP)", true, true);
				}
				else if (!m_gamepadHelpMessageShown && (m_componentPlayer.PlayerData.InputDevice & WidgetInputDevice.Gamepads) != 0 && Time.PeriodicEvent(7.0, 0.0))
				{
					m_gamepadHelpMessageShown = true;
					DisplaySmallMessage("Press START/PAUSE for gamepad controls\n(or see HELP)", true, true);
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
			if (playerInput.ToggleInventory || m_inventoryButtonWidget.IsClicked)
			{
				if (componentRider.Mount != null && ModalPanelWidget == null)
				{
					var componentEngine = m_componentPlayer.ComponentRider.Mount.Entity.FindComponent<ComponentEngine2>();
					if (componentEngine != null)
					{
						ModalPanelWidget = new Engine2Widget(m_componentPlayer.ComponentMiner.Inventory, componentEngine);
						AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
						return;
					}
					var componentEngine3 = m_componentPlayer.ComponentRider.Mount.Entity.FindComponent<ComponentEngineA>();
					if (componentEngine3 != null)
					{
						ModalPanelWidget = new EngineAWidget(m_componentPlayer.ComponentMiner.Inventory, componentEngine3);
						AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
						return;
					}
					var componentEngine2 = m_componentPlayer.ComponentRider.Mount.Entity.FindComponent<ComponentTrain>();
					if (componentEngine2 != null)
					{
						ModalPanelWidget = new TrainWidget(m_componentPlayer.ComponentMiner.Inventory, componentEngine2);
						AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
						return;
					}
				}
				if (IsInventoryVisible())
					ModalPanelWidget = null;
				else if (m_componentPlayer.ComponentMiner.Inventory is ComponentCreativeInventory)
					ModalPanelWidget = new CreativeInventoryWidget(m_componentPlayer.Entity);
				else
					ModalPanelWidget = new FullInventoryWidget(m_componentPlayer.ComponentMiner.Inventory, m_componentPlayer.Entity.FindComponent<ComponentCraftingTable>(true));
			}
			if (playerInput.ToggleClothing || m_clothingButtonWidget.IsClicked)
			{
				if (IsClothingVisible())
					ModalPanelWidget = null;
				else
					ModalPanelWidget = new ClothingWidget(m_componentPlayer);
			}
			if (m_sneakButtonWidget.IsClicked || playerInput.ToggleSneak)
			{
				bool isSneaking = m_componentPlayer.ComponentBody.IsSneaking;
				m_componentPlayer.ComponentBody.IsSneaking = !isSneaking;
				if (m_componentPlayer.ComponentBody.IsSneaking != isSneaking)
				{
					if (m_componentPlayer.ComponentBody.IsSneaking)
						DisplaySmallMessage("Sneak mode on", false, false);
					else
						DisplaySmallMessage("Sneak mode off", false, false);
				}
			}
			if (componentRider != null && (m_mountButtonWidget.IsClicked || playerInput.ToggleMount))
			{
				bool flag = componentRider.Mount != null;
				if (flag)
					componentRider.StartDismounting();
				else
				{
					ComponentMount componentMount = componentRider.FindNearestMount();
					if (componentMount != null)
						componentRider.StartMounting(componentMount);
				}
				if (componentRider.Mount != null != flag)
				{
					if (componentRider.Mount != null)
						DisplaySmallMessage("Mounted", false, false);
					else
						DisplaySmallMessage("Dismounted", false, false);
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
				IInventory inventory = m_componentPlayer.ComponentMiner.Inventory;
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
						DisplaySmallMessage("Fly mode on", false, false);
					}
					else
						DisplaySmallMessage("Fly mode off", false, false);
				}
			}
			if (m_cameraButtonWidget.IsClicked || playerInput.SwitchCameraMode)
			{
				View view = m_componentPlayer.View;
				if (view.ActiveCamera.GetType() == typeof(FppCamera))
				{
					view.ActiveCamera = view.FindCamera<TppCamera>(true);
					DisplaySmallMessage("Third person camera", false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(TppCamera))
				{
					view.ActiveCamera = view.FindCamera<OrbitCamera>(true);
					DisplaySmallMessage("Orbit camera", false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(OrbitCamera))
				{
					view.ActiveCamera = isCreative ? (Camera)new DebugCamera(view) : view.FindCamera<FixedCamera>(true);
					DisplaySmallMessage(isCreative ? "Debug camera" : "Fixed camera", false, false);
				}
				else if (isCreative && view.ActiveCamera.GetType() == typeof(FixedCamera))
				{
					view.ActiveCamera = new FlyCamera(view);
					DisplaySmallMessage("Fly camera", false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(DebugCamera))
				{
					view.ActiveCamera = view.FindCamera<FixedCamera>(true);
					DisplaySmallMessage("Fixed camera", false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(FlyCamera))
				{
					view.ActiveCamera = new RandomJumpCamera(view);
					DisplaySmallMessage("Random jump camera", false, false);
				}
				else if (view.ActiveCamera.GetType() == typeof(RandomJumpCamera))
				{
					view.ActiveCamera = new StraightFlightCamera(view);
					DisplaySmallMessage("Straight flight camera", false, false);
				}
				else
				{
					view.ActiveCamera = view.FindCamera<FppCamera>(true);
					DisplaySmallMessage("First person camera", false, false);
				}
			}
			if (m_photoButtonWidget.IsClicked || playerInput.TakeScreenshot)
			{
				ScreenCaptureManager.CapturePhoto();
				Time.QueueFrameCountDelayedExecution(Time.FrameIndex + 1, delegate
				{
					DisplaySmallMessage("Photo saved in pictures library", false, false);
				});
			}
			if (isCreative && (m_lightningButtonWidget.IsClicked || playerInput.Lighting))
			{
				var matrix = Matrix.CreateFromQuaternion(m_componentPlayer.ComponentCreatureModel.EyeRotation);
				Project.FindSubsystem<SubsystemWeather>(true).ManualLightingStrike(m_componentPlayer.ComponentCreatureModel.EyePosition, matrix.Forward);
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
					DisplaySmallMessage("Dawn", false, false);
				}
				else if (num7 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num7;
					DisplaySmallMessage("Noon", false, false);
				}
				else if (num8 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num8;
					DisplaySmallMessage("Dusk", false, false);
				}
				else if (num9 == num10)
				{
					m_subsystemTimeOfDay.TimeOfDayOffset += num9;
					DisplaySmallMessage("Midnight", false, false);
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
	}
}