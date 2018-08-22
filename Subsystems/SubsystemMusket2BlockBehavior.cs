using Engine;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemMusket2BlockBehavior : SubsystemBlockBehavior
	{
		private readonly Dictionary<ComponentMiner, double> m_aimStartTimes = new Dictionary<ComponentMiner, double>();

		private readonly Random m_random = new Random();

		private SubsystemAudio m_subsystemAudio;

		private SubsystemNoise m_subsystemNoise;

		private SubsystemParticles m_subsystemParticles;

		private SubsystemProjectiles m_subsystemProjectiles;

		private SubsystemTerrain m_subsystemTerrain;

		private SubsystemTime m_subsystemTime;

		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			componentPlayer.ComponentGui.ModalPanelWidget = (componentPlayer.ComponentGui.ModalPanelWidget == null) ? new Musket2Widget(inventory, slotIndex) : null;
			return true;
		}

		public override bool OnAim(Vector3 start, Vector3 direction, ComponentMiner componentMiner, AimState state)
		{
			IInventory inventory = componentMiner.Inventory;
			if (inventory != null)
			{
				int activeSlotIndex = inventory.ActiveSlotIndex;
				if (activeSlotIndex >= 0)
				{
					int slotValue = inventory.GetSlotValue(activeSlotIndex);
					int slotCount = inventory.GetSlotCount(activeSlotIndex);
					int num = Terrain.ExtractContents(slotValue);
					int data = Terrain.ExtractData(slotValue);
					int num2 = slotValue;
					int num3 = 0;
					if (num == Musket2Block.Index && slotCount > 0)
					{
						double value;
						if (!m_aimStartTimes.TryGetValue(componentMiner, out value))
						{
							value = m_subsystemTime.GameTime;
							m_aimStartTimes[componentMiner] = value;
						}
						float num4 = (float)(m_subsystemTime.GameTime - value);
						float num5 = (float)MathUtils.Remainder(m_subsystemTime.GameTime, 1000.0);
						Vector3 v = (float)((componentMiner.ComponentCreature.ComponentBody.IsSneaking ? 0.00999999977648258 : 0.0299999993294477) + 0.200000002980232 * (double)MathUtils.Saturate((num4 - 6.5f) / 40f)) * new Vector3
						{
							X = SimplexNoise.OctavedNoise(num5, 2f, 3, 2f, 0.5f),
							Y = SimplexNoise.OctavedNoise(num5 + 100f, 2f, 3, 2f, 0.5f),
							Z = SimplexNoise.OctavedNoise(num5 + 200f, 2f, 3, 2f, 0.5f)
						};
						direction = Vector3.Normalize(direction + v);
						switch (state)
						{
						case AimState.InProgress:
						{
							if ((double)num4 >= 30.0)
							{
								componentMiner.ComponentCreature.ComponentCreatureSounds.PlayMoanSound();
								return true;
							}
							if ((double)num4 > 0.2 && !Musket2Block.GetHammerState(Terrain.ExtractData(num2)))
							{
								num2 = Terrain.MakeBlockValue(num, 0, Musket2Block.SetHammerState(Terrain.ExtractData(num2), true));
								m_subsystemAudio.PlaySound("Audio/HammerCock", 1f, m_random.UniformFloat(-0.1f, 0.1f), 0f, 0f);
							}
							ComponentFirstPersonModel componentFirstPersonModel = componentMiner.Entity.FindComponent<ComponentFirstPersonModel>();
							if (componentFirstPersonModel != null)
							{
								ComponentPlayer componentPlayer2 = componentMiner.ComponentPlayer;
								if (componentPlayer2 != null)
								{
									componentPlayer2.ComponentGui.ShowAimingSights(start, direction);
								}
								componentFirstPersonModel.ItemOffsetOrder = new Vector3(-0.21f, 0.15f, 0.08f);
								componentFirstPersonModel.ItemRotationOrder = new Vector3(-0.7f, 0f, 0f);
							}
							componentMiner.ComponentCreature.ComponentCreatureModel.AimHandAngleOrder = 1.4f;
							componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemOffsetOrder = new Vector3(-0.08f, -0.08f, 0.07f);
							componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemRotationOrder = new Vector3(-1.7f, 0f, 0f);
							break;
						}
						case AimState.Cancelled:
							if (Musket2Block.GetHammerState(Terrain.ExtractData(num2)))
							{
								num2 = Terrain.MakeBlockValue(num, 0, Musket2Block.SetHammerState(Terrain.ExtractData(num2), false));
								m_subsystemAudio.PlaySound("Audio/HammerUncock", 1f, m_random.UniformFloat(-0.1f, 0.1f), 0f, 0f);
							}
							m_aimStartTimes.Remove(componentMiner);
							break;
						case AimState.Completed:
						{
							bool flag = false;
							int value2 = 0;
							int num6 = 0;
							float s = 0f;
							Vector3 vector = Vector3.Zero;
							Musket2Block.LoadState loadState = Musket2Block.GetLoadState(data);
							if (Musket2Block.GetHammerState(Terrain.ExtractData(num2)))
							{
								switch (loadState)
								{
								case Musket2Block.LoadState.Gunpowder:
									flag = true;
									value2 = Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, BulletBlock.BulletType.IronBullet));
									s = 320f;
									num6 = 1;
									vector = new Vector3(0.01f, 0.01f, 0.05f);
									goto default;
								default:
									if (loadState == Musket2Block.LoadState.Wad)
									{
										flag = true;
										value2 = Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, BulletBlock.BulletType.IronBullet));
										s = 320f;
										num6 = 1;
										vector = new Vector3(0.01f, 0.01f, 0.05f);
									}
									break;
								case Musket2Block.LoadState.Empty:
								{
									ComponentPlayer componentPlayer = componentMiner.ComponentPlayer;
									if (componentPlayer != null)
									{
										componentPlayer.ComponentGui.DisplaySmallMessage("Load bullet first", true, false);
									}
									break;
								}
								}
							}
							if (flag)
							{
								if ((double)componentMiner.ComponentCreature.ComponentBody.ImmersionFactor > 0.400000005960464)
								{
									m_subsystemAudio.PlaySound("Audio/MusketMisfire", 1f, m_random.UniformFloat(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition, 3f, true);
								}
								else
								{
									Vector3 eyePosition = componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition;
									Matrix matrix = componentMiner.ComponentCreature.ComponentBody.Matrix;
									Vector3 v2 = eyePosition + matrix.Right * 0.3f;
									matrix = componentMiner.ComponentCreature.ComponentBody.Matrix;
									Vector3 vector2 = v2 - matrix.Up * 0.2f;
									Vector3 vector3 = Vector3.Normalize(vector2 + direction * 10f - vector2);
									Vector3 vector4 = Vector3.Normalize(Vector3.Cross(vector3, Vector3.UnitY));
									Vector3 v3 = Vector3.Normalize(Vector3.Cross(vector3, vector4));
									for (int i = 0; i < num6; i++)
									{
										Vector3 v4 = m_random.UniformFloat(0f - vector.X, vector.X) * vector4 + m_random.UniformFloat(0f - vector.Y, vector.Y) * v3 + m_random.UniformFloat(0f - vector.Z, vector.Z) * vector3;
										Projectile projectile = m_subsystemProjectiles.FireProjectile(value2, vector2, s * (vector3 + v4), Vector3.Zero, componentMiner.ComponentCreature);
										if (projectile != null)
										{
											projectile.ProjectileStoppedAction = ProjectileStoppedAction.Disappear;
										}
									}
									m_subsystemAudio.PlaySound("Audio/MusketFire", 1f, m_random.UniformFloat(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition, 10f, true);
									m_subsystemParticles.AddParticleSystem(new GunSmokeParticleSystem(m_subsystemTerrain, vector2 + 0.3f * vector3, vector3));
									m_subsystemNoise.MakeNoise(vector2, 1f, 40f);
									componentMiner.ComponentCreature.ComponentBody.ApplyImpulse(-1f * vector3);
								}
								if (loadState == Musket2Block.LoadState.Gunpowder)
								{
									num2 = Terrain.MakeBlockValue(Terrain.ExtractContents(num2), 0, Musket2Block.SetLoadState(Terrain.ExtractData(num2), Musket2Block.LoadState.Empty));
								}
								if (loadState == Musket2Block.LoadState.Wad)
								{
									num2 = Terrain.MakeBlockValue(Terrain.ExtractContents(num2), 0, Musket2Block.SetLoadState(Terrain.ExtractData(num2), Musket2Block.LoadState.Gunpowder));
								}
								num3 = 1;
							}
							if (Musket2Block.GetHammerState(Terrain.ExtractData(num2)))
							{
								num2 = Terrain.MakeBlockValue(Terrain.ExtractContents(num2), 0, Musket2Block.SetHammerState(Terrain.ExtractData(num2), false));
								m_subsystemAudio.PlaySound("Audio/HammerRelease", 1f, m_random.UniformFloat(-0.1f, 0.1f), 0f, 0f);
							}
							m_aimStartTimes.Remove(componentMiner);
							break;
						}
						}
					}
					if (num2 != slotValue)
					{
						inventory.RemoveSlotItems(activeSlotIndex, 1);
						inventory.AddSlotItems(activeSlotIndex, num2, 1);
					}
					if (num3 > 0)
					{
						componentMiner.DamageActiveTool(num3);
					}
				}
			}
			return false;
		}
		public override int GetProcessInventoryItemCapacity(IInventory inventory, int slotIndex, int value)
		{
			return Terrain.ExtractContents(value) != Bullet2Block.Index || Musket2Block.GetLoadState(Terrain.ExtractData(inventory.GetSlotValue(slotIndex))) == Musket2Block.LoadState.Wad
				? 0
				: 1;
		}

		public override void ProcessInventoryItem(IInventory inventory, int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			processedValue = value;
			processedCount = count;
			if (processCount == 1)
			{
				int data = Terrain.ExtractData(inventory.GetSlotValue(slotIndex));
				Musket2Block.LoadState loadState = Musket2Block.GetLoadState(data);
				switch (loadState)
				{
				case Musket2Block.LoadState.Empty:
					loadState = Musket2Block.LoadState.Gunpowder;
					break;
				case Musket2Block.LoadState.Gunpowder:
					loadState = Musket2Block.LoadState.Wad;
					break;
				}
				processedValue = 0;
				processedCount = 0;
				inventory.RemoveSlotItems(slotIndex, 1);
				inventory.AddSlotItems(slotIndex, Terrain.MakeBlockValue(Musket2Block.Index, 0, Musket2Block.SetBulletType(Musket2Block.SetLoadState(data, loadState), null)), 1);
			}
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
			m_subsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			m_subsystemProjectiles = Project.FindSubsystem<SubsystemProjectiles>(true);
			m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(true);
			m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			m_subsystemNoise = Project.FindSubsystem<SubsystemNoise>(true);
			base.Load(valuesDictionary);
		}
	}
}
