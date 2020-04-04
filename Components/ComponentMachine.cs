using Chemistry;
using Engine;
using GameEntitySystem;
using System;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public abstract class ComponentMachine : ComponentInventoryBase, ICraftingMachine
	{
		public float m_fireTimeRemaining;
		protected int m_furnaceSize;
		protected ComponentBlockEntity m_componentBlockEntity;
		protected bool m_updateSmeltingRecipe;

		public virtual int FuelSlotIndex => SlotsCount - 2;

		public virtual int ResultSlotIndex => 0;
		public virtual int RemainsSlotIndex => SlotsCount - 1;

		public int SlotIndex { get => -1; set => throw new NotImplementedException(); }

		public int UpdateOrder => 0;

		public float HeatLevel;

		public float SmeltingProgress;

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex == FuelSlotIndex)
			{
				var block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
				if ((block is IFuel fuel ? fuel.GetHeatLevel(value) : block.FuelHeatLevel) < 1f)
					return 0;
			}
			int v = GetSlotValue(slotIndex);
			if ((v == 262384 || v == 786672 || v == 1048816 || v == 1310960 || v == WaterBlock.Index || v == MagmaBlock.Index) && value != v && v != 0)
			{
				return 0;
			}
			return base.GetSlotCapacity(slotIndex, value);
		}

		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			if (GetSlotCount(slotIndex) != 0 && (GetSlotValue(slotIndex) != value || GetSlotCount(slotIndex) + count > GetSlotCapacity(slotIndex, value)))
				return;
			base.AddSlotItems(slotIndex, value, count);
			m_updateSmeltingRecipe = true;
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			int v = GetSlotValue(slotIndex);
			if (v == 262384 || v == 786672 || v == 1048816 || v == 1310960 || v == WaterBlock.Index || v == MagmaBlock.Index)
			{
				return 0;
			}
			return base.RemoveSlotItems(slotIndex, count);
		}

		public int RemoveSlotItems2(int slotIndex, int count)
		{
			return base.RemoveSlotItems(slotIndex, count);
		}

		public override void ProcessSlotItems(int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			IItem item = Item.Block.GetItem(ref value);
			if (slotIndex == FuelSlotIndex && item is FuelCylinder fuel)
			{
				HeatLevel = fuel.GetHeatLevel(value);
				m_fireTimeRemaining = fuel.GetFuelFireDuration(value);
				processedValue = fuel.GetDamageDestructionValue();
				processedCount = 1;
				return;
			}
			base.ProcessSlotItems(slotIndex, value, count, processCount, out processedValue, out processedCount);
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>();
			m_updateSmeltingRecipe = true;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		public CraftingRecipe GetRecipe() => null;

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			if (m_fireTimeRemaining > 0f)
				valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			if (HeatLevel > 0f)
				valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}

		public int FindSmeltingRecipe(Dictionary<int, int> result, int value)
		{
			if (value == 0)
				return 0;
			var e = result.GetEnumerator();
			while (e.MoveNext())
			{
				int index = FindAcquireSlotForItem(this, e.Current.Key);
				if (index < 0)
					return 0;
			}
			return value;
		}

		public new void DropAllItems(Vector3 position)
		{
			SubsystemPickables subsystemPickables = Project.FindSubsystem<SubsystemPickables>(throwOnError: true);
			for (int i = 0; i < SlotsCount; i++)
			{
				int slotCount = GetSlotCount(i);
				int v = GetSlotValue(i);
				if (slotCount > 0 && !(v == 262384 || v == 786672 || v == 1048816 || v == 1310960 || v == WaterBlock.Index || v == MagmaBlock.Index))
				{
					int slotValue = GetSlotValue(i);
					int count = RemoveSlotItems(i, slotCount);
					Vector3 value = m_random.UniformFloat(5f, 10f) * Vector3.Normalize(new Vector3(m_random.UniformFloat(-1f, 1f), m_random.UniformFloat(1f, 2f), m_random.UniformFloat(-1f, 1f)));
					subsystemPickables.AddPickable(slotValue, count, position, value, null);
				}
			}
		}

		/*public static float GetFuelHeatLevel(int value)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			return block is IFuel fuel ? fuel.GetHeatLevel(value) : block.FuelHeatLevel;
		}*/

		public int FindSmeltingRecipe(Dictionary<int, int> result, ReactionSystem system, Condition condition = Condition.H | Condition.l, ushort t = 2200)
		{
			result.Clear();
			int value, count;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				var item = ChemicalBlock.Get(GetSlotValue(i));
				if (item != null)
				{
					system.Add(item.GetDispersionSystem(), GetSlotCount(i));
					value = item.GetDamageDestructionValue();
					if (value != 0)
					{
						result.TryGetValue(value, out count);
						result[value] = count + GetSlotCount(i);
					}
				}
			}
			Equation equation = system.React(condition, t);
			if (equation == null)
				return 0;
			system.Normalize();
			Dictionary<Compound, int>.Enumerator e;
			Dictionary<Compound, int>.Enumerator e2;
			//result.Clear();
			for (e = equation.Reactants.GetEnumerator(); e.MoveNext();)
			{
				int num = 1;
				for (int i = 0; i < m_furnaceSize; i++)
				{
					//var item = ChemicalBlock.Get(GetSlotValue(i));
					if (ChemicalBlock.Get(e.Current.Key.ToString()) == GetSlotValue(i) && GetSlotCount(i) > e.Current.Value)
					{
						num = GetSlotCount(i);
						break;
					}
				}
				num = MathUtils.Max(e.Current.Value, num);
				result[ChemicalBlock.Get(e.Current.Key.ToString())] = -num;
			}
			bool flag = false;
			for (e = system.GetEnumerator(); e.MoveNext();)
			{
				flag = false;
				int val = ChemicalBlock.Get(e.Current.Key.ToString());
				for (e2 = equation.Reactants.GetEnumerator(); e2.MoveNext();)
				{
					if (val == ChemicalBlock.Get(e2.Current.Key.ToString()))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (ChemicalBlock.Get(val) == null)
						return 0;
					value = ChemicalBlock.Get(val).GetDamageDestructionValue();

					if (value != 0)
					{
						result.TryGetValue(value, out count);
						count = e.Current.Value;
						if (count < 0)
							continue;
						if (count > 0)
							result[value] = count;
					}
					value = e.Current.Value;
					if (value > 999)
						value /= 1000;
					if (value > 200)
						value = 1;
					result[val] = value;
				}
			}

			return equation.GetHashCode();
		}
	}

	public abstract class ComponentPMach : ComponentSMachine, IUpdateable
	{
		protected int m_count;
		protected float m_speed;
		protected string m_smeltingRecipe;

		//protected int m_music;

		protected string m_smeltingRecipe2;

		public override int ResultSlotIndex => SlotsCount - 1;

		public void Update(float dt)
		{
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				m_smeltingRecipe2 = FindSmeltingRecipe();
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != null)
			{
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				m_smeltingRecipe = ComponentEngine.IsPowered(Utils.Terrain, coordinates.X, coordinates.Y, coordinates.Z) ? m_smeltingRecipe2 : null;
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else
				m_fireTimeRemaining = 100f;
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + m_speed * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					for (int l = 0; l < m_furnaceSize; l++)
						if (m_slots[l].Count > 0)
							m_slots[l].Count--;
					m_slots[ResultSlotIndex].Value = ItemBlock.IdTable[m_smeltingRecipe];
					m_slots[ResultSlotIndex].Count += m_count;
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			//m_speed = valuesDictionary.GetValue("Speed", 0.1f);
			//m_count = valuesDictionary.GetValue("Count", 1);
			m_speed = 0.1f;
			m_count = 1;
		}

		protected virtual string FindSmeltingRecipe()
		{
			return null;
		}
	}

	public class ComponentSMachine : ComponentMachine
	{
		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => -1;

		public override int FuelSlotIndex => -1;
	}

	public class ComponentSorter : ComponentSMachine
	{
	}

	public class ComponentFRCore : ComponentSMachine, IUpdateable
	{
		//public override float m_fireTimeRemaining;
		//public float HeatLevel;
		public float veloc;

		public void Update(float dt)
		{
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(10.0, 0.0))
			{
				Point3 coordinates = m_componentBlockEntity.Coordinates;

				int num = 1, num2 = 0;
				int num3 = coordinates.X;
				int num4 = coordinates.Y - 1;
				int num5 = coordinates.Z, v;
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						for (int k = -1; k < 2; k++)
						{
							int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3 + i, num4 + j, num5 + k), 0);
							int cellContents = Terrain.ExtractContents(cellValue);
							if ((j==0 && i==0 && k==0) || (j == 1 && i == 0 && k == 0))
							{

							}else
							{
								if (Terrain.ExtractData(cellValue) >> 10 != 6 || cellContents!= 507)
								{
									num = 0;
									return;
								}
							}

						}
					}
				}


				int i235 = 0;
				int i239 = 0;
				for (int i = 0; i < SlotsCount; i++)
				{
					int va1 = GetSlotValue(i);
					if (va1 == ItemBlock.IdTable["U235C"])
						i235+=GetSlotCount(i);
					if (va1 == ItemBlock.IdTable["PU239C"])
						i239+= GetSlotCount(i);
				}
				veloc = i235 * 0.000001f + i239 * 0.002f;
				for (int i = 0; i < SlotsCount; i++)
				{
					int va1 = GetSlotValue(i);
					if (va1 == ItemBlock.IdTable["U238C"] && Utils.Random.Bool(veloc/ GetSlotCount(i)))
					{
						int aaa = GetSlotCount(i);
						RemoveSlotItems(i, aaa);
						AddSlotItems(i, ItemBlock.IdTable["U235C"], aaa);
					}
				}
			}
		}
	}


	public class ComponentRCore : ComponentSMachine, IUpdateable
	{
		//public override float m_fireTimeRemaining;
		//public float HeatLevel;
		public float veloc;

		public void Update(float dt)
		{
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(0.2, 0.0))
			{
				int veo = 0;
				int sat = 0;
				if (m_updateSmeltingRecipe)
				{
					m_fireTimeRemaining = 0f;
					veloc = 1f;
					for (int i = 0; i < SlotsCount; i++)
					{
						//if ()
						int va1 = GetSlotValue(i);

						if (Terrain.ExtractContents(va1) == FuelRodBlock.Index)
						{
							if (FuelRodBlock.GetType(va1) == RodType.UFuelRod)
							{
								m_fireTimeRemaining += 5;
								veo += 0;
								sat++;
							}
							if (FuelRodBlock.GetType(va1) == RodType.ControlRod)
							{
								m_fireTimeRemaining -= 4;
								veloc -= 0.2f;
							}
							if (FuelRodBlock.GetType(va1) == RodType.CarbonRod)
							{
								m_fireTimeRemaining += 3;
								veloc += 0.2f;
							}
							if (FuelRodBlock.GetType(va1) == RodType.PFuelRod)
							{
								m_fireTimeRemaining += 25;
								veloc += 0f;
								sat++;
							}
						}
					}
				}
				veloc = MathUtils.Max(veloc, 0);
				if (sat > 0)
				{
					Point3 coor = m_componentBlockEntity.Coordinates;
					bool flag = false;
					if (Utils.SubsystemTime.PeriodicGameTimeEvent(1.2, 0.0))
						for (int iii = 0; iii < Utils.SubsystemSour.m_radations.Count; iii++)
						{
							if (Utils.SubsystemSour.m_radations.Array[iii] == new Vector4(coor.X, coor.Y, coor.Z, 10f))
							{
								flag = true; break;
							}
						}
					if (Utils.SubsystemSour.m_radations.Count == 0 || !flag)
						Utils.SubsystemSour.m_radations.Add(new Vector4(coor.X, coor.Y, coor.Z, 10f));
					m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining);
					//bool vap = false;
					Point3 coordinates = m_componentBlockEntity.Coordinates;
					HeatLevel = MathUtils.Max(0f, HeatLevel + m_fireTimeRemaining * 0.5f - 0.1f);
					if (ElementBlock.Block.GetDevice(coordinates.X, coordinates.Y - 1, coordinates.Z, Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y - 1, coordinates.Z)) is TControl)
					{
						var point3 = new Point3(coordinates.X, coordinates.Y - 1, coordinates.Z);
						var entity = Utils.GetBlockEntity(point3);
						ComponentTControl component3 = entity.Entity.FindComponent<ComponentTControl>();
						if (component3 != null)
						{
							HeatLevel = MathUtils.Min(component3.m_fireTimeRemaining,HeatLevel);
						}

						//IInventory inventory = entity.Entity.FindComponent<ComponentSMachine>(true);
						//vap = true;
					}


					
					//HeatLevel += m_fireTimeRemaining*5;
					//Point3 coordinates = m_componentBlockEntity.Coordinates;
					if (HeatLevel > 3000)
					{
						for (int iii = 0; iii < Utils.SubsystemSour.m_radations.Count; iii++)
						{
							//Point3 coor = m_componentBlockEntity.Coordinates;
							if (Utils.SubsystemSour.m_radations.Array[iii] == new Vector4(coor.X, coor.Y, coor.Z, 1f))
							{
								Utils.SubsystemSour.m_radations.Remove(new Vector4(coor.X, coor.Y, coor.Z, 1f));
							}
						}
						Utils.SubsystemSour.m_radations.Add(new Vector4(coor.X, coor.Y, coor.Z, 15000f));
						Utils.SubsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, LargeGunpowderKegBlock.Index);
						Utils.SubsystemParticles.AddParticleSystem(new RParticleSystem(new Vector3(coordinates) + new Vector3(.5f), 2f, 50f));
					}

					if (m_fireTimeRemaining > 0)
						for (int i = 0; i < SlotsCount; i++)
						{
							//if ()
							int va1 = GetSlotValue(i);
							if (Terrain.ExtractContents(va1) == FuelRodBlock.Index)
							{
								if (Utils.Random.Bool(0.006f * veloc))
								{
									RemoveSlotItems(i, 1);
									if (BlocksManager.DamageItem(va1, 1) != va1)
									{
										AddSlotItems(i, BlocksManager.DamageItem(va1, 1), 1);
									}else
									{
										if (Terrain.ExtractContents(va1) == FuelRodBlock.Index)
										{
											if (FuelRodBlock.GetType(va1) == RodType.UFuelRod)
											{
												//inventory.AddSlotItems(i, va1, -1);
												AddSlotItems(i, ItemBlock.IdTable["UsedUpFuel"], 1);
											}
											if (FuelRodBlock.GetType(va1) == RodType.ControlRod)
											{
												//inventory.AddSlotItems(i, va1, -1);
												AddSlotItems(i, ItemBlock.IdTable["B"], 1);
											}
											if (FuelRodBlock.GetType(va1) == RodType.CarbonRod)
											{
												//inventory.AddSlotItems(i, va1, -1);
												AddSlotItems(i, ItemBlock.IdTable["C"], 1);
											}
										}
									}
								}
							}
							else
							{
								//RemoveSlotItems(i, 1);
							}
						}
				}
				else
				{
					if (Utils.SubsystemTime.PeriodicGameTimeEvent(1.2, 0.0))
						for (int iii = 0; iii < Utils.SubsystemSour.m_radations.Count; iii++)
						{
							Point3 coor = m_componentBlockEntity.Coordinates;
							if (Utils.SubsystemSour.m_radations.Array[iii] == new Vector4(coor.X, coor.Y, coor.Z, 10f))
								Utils.SubsystemSour.m_radations.Remove(new Vector4(coor.X, coor.Y, coor.Z, 10f));
						}
					HeatLevel = MathUtils.Max(0f, HeatLevel - 0.1f);
				}
			}
		}
	}

	public class ComponentHChanger : ComponentMachine, IUpdateable
	{
		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => -1;

		public override int FuelSlotIndex => -1;

		public int Pressure;

		public int Output;

		public bool vap;

		public void Update(float dt)
		{
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(0.5, 0.0))
			{

				Pressure = 0;
				Output = 0;
				vap = false;
				Point3 coordinates = m_componentBlockEntity.Coordinates;

				if (ElementBlock.Block.GetDevice(coordinates.X, coordinates.Y + 1, coordinates.Z, Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y + 1, coordinates.Z)) is ColdBlock em && em.Powered)
				{
					vap = true;
				}

				
				var point = CellFace.FaceToPoint3(Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)) >> 15);
				int num3 = coordinates.X - point.X;
				int num4 = coordinates.Y - point.Y;
				int num5 = coordinates.Z - point.Z;
				int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3, num4, num5), 0);
				//int cellContents = Terrain.ExtractContents(cellValue);Terrain.ExtractData(value) >> 15
				if (4 == Terrain.ExtractData(cellValue) >> 10)
				{
					var component3 = Utils.GetBlockEntity(new Point3(num3, num4, num5))?.Entity.FindComponent<ComponentRCore>();
					if (component3 != null)
					{
						Pressure = (int)(component3.HeatLevel / 100);
						if (Pressure > 0)
						{
							for (int i = 0; i < 8; i++)
							{
								if (base.GetSlotValue(i) == WaterBlock.Index)
								{
									if (Utils.SubsystemTime.PeriodicGameTimeEvent(( 30f / Pressure ) / 10f, 0.0) && !vap)
										m_slots[i].Count--;
									Output += Pressure;
									component3.HeatLevel -= 2 * Output;
									break;
								}
							}
						}
					}
				}

				if (ElementBlock.Block.GetDevice(coordinates.X, coordinates.Y+1, coordinates.Z, Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y+1, coordinates.Z)) is WaterExtractor em2 && em2.Powered)
				{
					var point3 = new Point3(coordinates.X, coordinates.Y + 1, coordinates.Z);
					var entity = Utils.GetBlockEntity(point3);
					Component component3 = entity.Entity.FindComponent<ComponentSMachine>();
					if (entity != null && component3 != null)
					{
						IInventory inventory = entity.Entity.FindComponent<ComponentSMachine>(true);
						for (int i = 0; i < 6; i++)
						{
							int va1 = inventory.GetSlotValue(i);
							if (Utils.Random.Bool(0.0002f * Pressure))
							{
								if (va1 == EmptyBucketBlock.Index && AcquireItems(inventory, RottenMeatBlock.Index | 15 << 18, 1) == 0)
								{
									inventory.RemoveSlotItems(i, 1);
								}
							}
						}
					}
				}


			}
		}
	}

	public class ComponentTurbine : ComponentMachine, IUpdateable
	{
		public int Pressure;

		public int Output;
		public bool Powered;

		public void Update(float dt)
		{
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(0.5, 0.0))
			{
				Pressure = 0;
				Output = 0;
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				var point = CellFace.FaceToPoint3(Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)) >> 15);
				int num3 = coordinates.X - point.X;
				int num4 = coordinates.Y - point.Y;
				int num5 = coordinates.Z - point.Z;
				int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3, num4, num5), 0);
				//int cellContents = Terrain.ExtractContents(cellValue);Terrain.ExtractData(value) >> 15
				if (ElementBlock.Block.GetDevice(num3, num4, num5, cellValue) is Hchanger)
				{
					var point3 = new Point3(num3, num4, num5);
					var entity = Utils.GetBlockEntity(point3);
					ComponentHChanger component3 = entity.Entity.FindComponent<ComponentHChanger>();
					Output = component3.Output;
				}
				Powered = Output > 0;
			}
		}
	}


	public class ComponentRadioC : ComponentMachine, IUpdateable
	{
		public int Frequency;
		public double time;
		public int Output;
		public bool Powered;
		public bool flag;
		public void Update(float dt)
		{
			if (Utils.SubsystemTime.GameTime - time > 0.7f)
			{
				flag = false;
				InActive();
			}
		}

		public virtual void Delete()
		{
			InActive();
		}

		public virtual void Active()
		{
			Point3 coor = m_componentBlockEntity.Coordinates;
			for (int iii = 0; iii < Utils.SubsystemSour.m_radio.Count; iii++)
			{
				if (Utils.SubsystemSour.m_radio.Array[iii] == new Vector4(coor.X, coor.Y, coor.Z, Utils.SubsystemSour.m_radio.Array[iii].W))
				{
					Utils.SubsystemSour.m_radio.Remove(Utils.SubsystemSour.m_radio.Array[iii]);
				}
			}
			if (m_fireTimeRemaining >= 1000f && Powered)
			{
				flag = true;
				time = Utils.SubsystemTime.GameTime;
				Utils.SubsystemSour.m_radio.Add(new Vector4(coor.X, coor.Y, coor.Z, m_fireTimeRemaining));
			}
		}

		public virtual void InActive()
		{
			Point3 coor = m_componentBlockEntity.Coordinates;
			for (int iii = 0; iii < Utils.SubsystemSour.m_radio.Count; iii++)
			{
				if (Utils.SubsystemSour.m_radio.Array[iii] == new Vector4(coor.X, coor.Y, coor.Z, Utils.SubsystemSour.m_radio.Array[iii].W))
				{
					Utils.SubsystemSour.m_radio.Remove(Utils.SubsystemSour.m_radio.Array[iii]);
				}
			}

		}
	}

	public class ComponentRadioR : ComponentMachine
	{
		public int Frequency;

		public int Output;
		public bool Powered;

		public virtual bool Active()
		{
			Point3 coor = m_componentBlockEntity.Coordinates;
			for (int iii = 0; iii < Utils.SubsystemSour.m_radio.Count; iii++)
			{
				if (Utils.SubsystemSour.m_radio.Array[iii].W == m_fireTimeRemaining)
				{
					return true;
				}
			}
			return false;
		}
	}

	public class ComponentTControl : ComponentMachine
	{
		public int Frequency;

		public int Output;
		public bool Powered;
	}

	public class ComponentLaserG : ComponentMachine
	{
		public int Pressure;

		public int Output;
		public bool Powered;

		public virtual void Beam()
		{
			if (m_fireTimeRemaining >= 5000f || Utils.SubsystemGameInfo.WorldSettings.GameMode == 0)
			{
				m_fireTimeRemaining -= 5000f;
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				//int data = Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
				//int direction = FourDirectionalBlock.GetDirection(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
				int face = Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)) >> 15;
				Laser(coordinates, face);
			}
		}

		protected void Laser(Point3 point, int face)
		{
			Vector3 vector = CellFace.FaceToVector3(face);
			int x = point.X;
			int y = point.Y;
			int z = point.Z;
			var dpoint = Terrain.ToCell(vector);
			var end = new Vector3(point.X, point.Y, point.Z);

			int dmax = 50;
			ComponentCreature cre1 = null;
			var e1 = Utils.SubsystemBodies.Bodies.GetEnumerator();
			while (e1.MoveNext())
			{
				var entity = e1.Current.Entity;
				//var entity = 0;
				ComponentCreature cre = entity.FindComponent<ComponentCreature>();

				if (cre != null && ((vector.X > 0 && (cre.ComponentBody.Position.X - point.X - 1.0f) > 0f) || (vector.X < 0 && (cre.ComponentBody.Position.X - point.X) < 0f) || vector.X == 0) &&
					 ((vector.Z > 0 && (cre.ComponentBody.Position.Z - point.Z - 1.0f) > 0f) || (vector.Z < 0 && (cre.ComponentBody.Position.Z - point.Z) < 0f) || vector.Z == 0) &&
					((vector.X == 0 && Math.Abs(cre.ComponentBody.Position.X - point.X - 0.5f) <= 0.5f) || (vector.Z == 0 && Math.Abs(cre.ComponentBody.Position.Z - point.Z - 0.5f) <= 0.5f))
					&& (Math.Abs(cre.ComponentBody.Position.Y - point.Y - 0.5f) <= 0.5f) && cre.ComponentBody.Position.Y - point.Y < 1f)
				{
					if (dmax > (int)Math.Abs(Vector3.Distance(new Vector3(point) + new Vector3(.5f), cre.ComponentBody.Position)))
					{
						dmax = (int)Math.Abs(Vector3.Distance(new Vector3(point) + new Vector3(.5f), cre.ComponentBody.Position));
						cre1 = cre;
					}
				}
			}

			for (int d = 1; d < 50; d++)
			{
				Point3 npoint = point + d * dpoint;
				int value = Utils.Terrain.GetCellValueFast(npoint.X, npoint.Y, npoint.Z);
				int value2 = Utils.Terrain.GetCellContentsFast(npoint.X, npoint.Y, npoint.Z);
				end = new Vector3(npoint.X, npoint.Y, npoint.Z);
				//var bodies = new DynamicArray<ComponentBody>();
				if (d >= dmax)
				{
					int nnn = (int)cre1.Entity.FindComponent<ComponentHealth>()?.AttackResilience / 8;
					for (int zz = 0; zz <= nnn; zz++)
						Utils.SubsystemPickables.AddPickable(ItemBlock.IdTable["CoalPowder"], 1, cre1.ComponentBody.Position, null, null);
					Utils.SubsystemParticles.AddParticleSystem(new KillParticleSystem(Utils.SubsystemTerrain, cre1.ComponentBody.Position, 1f));
					//end = cre1.ComponentBody.Position;
					if (cre1.Entity.FindComponent<ComponentNPlayer>() != null)
					{
						cre1.Entity.FindComponent<ComponentHealth>()?.Injure(1f, null, false, "Killed by Laser");
					}
					else
						Project.RemoveEntity(cre1.Entity, true);
					break;
				}

				if (value != 0)
				{
					var block = BlocksManager.Blocks[value2];
					if (value2 != GlassBlock.Index)
						if (block.IsPlaceable)
						{
							if (block.ExplosionResilience >= 1000f)
							{
								break;
							}
							else if (block.DefaultExplosionPressure > 0)
							{
								Utils.SubsystemExplosions.TryExplodeBlock(npoint.X, npoint.Y, npoint.Z, value);
							}
							else if (block.FuelFireDuration > 0)
							{
								int v1 = (int)block.FuelFireDuration / 10;
								for (int zz = 0; zz <= v1; zz++)
									Utils.SubsystemPickables.AddPickable(ItemBlock.IdTable["CoalPowder"], 1, new Vector3(npoint.X, npoint.Y, npoint.Z), null, null);
								Utils.SubsystemTerrain.DestroyCell(5, npoint.X, npoint.Y, npoint.Z, 0, true, false);
							}
							else
								Utils.SubsystemTerrain.DestroyCell(5, npoint.X, npoint.Y, npoint.Z, 0, false, false);
							break;
						}
				}
			}
			Utils.SubsystemLaser.MakeLightningStrike(new Vector3(point.X, point.Y, point.Z) + new Vector3(0.5f), end + new Vector3(0.5f));
			//var e = Utils.SubsystemBodies.Bodies.GetEnumerator();
		}
	}



	public class ComponentAGun : ComponentMachine, IUpdateable
	{
		public int Pressure;
		public SubsystemPlayers subsystemPlayers;
		public int Output;
		public bool Powered;

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			subsystemPlayers = Project.FindSubsystem<SubsystemPlayers>(true);
		}

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			return (Terrain.ExtractContents(value) == Bullet2Block.Index && value == Terrain.MakeBlockValue(521, 0, Bullet2Block.SetBulletType(0, Bullet2Block.BulletType.HandBullet))) ? base.GetSlotCapacity(slotIndex, value)
			: 0;
		}

		public void Update(float dt)
		{

			if (Utils.SubsystemTime.PeriodicGameTimeEvent(0.34, 0.0) && Powered && HeatLevel==1f)
			{


				int abb = -1;
				int flag2 = 0;
				for (int i = 0; i < 12; i++)
				{
					int va1 = GetSlotValue(i);
					if (va1 == Terrain.MakeBlockValue(521, 0, Bullet2Block.SetBulletType(0, Bullet2Block.BulletType.HandBullet)))
					{
						abb = i;
						flag2 = -1;
						break;
					}
				}
				if (flag2 == 0)
					return;


				Point3 c = m_componentBlockEntity.Coordinates;
				var componentMiner = subsystemPlayers.FindNearestPlayer(new Vector3(c.X, c.Y, c.Z) + new Vector3(0.5f)).ComponentMiner.Entity.FindComponent<ComponentNPlayer>();
				//var componentMiner = componentMiner1.Entity.FindComponent<ComponentNPlayer>();
				//var player = componentMiner.Entity.FindComponent<ComponentMiner>();
				Vector3 po = new Vector3(c.X, c.Y, c.Z) + new Vector3(0.5f);
				var e1 = Utils.SubsystemBodies.Bodies.GetEnumerator();
				while (e1.MoveNext())
				{
					var entity = e1.Current.Entity;
					//var entity = 0;
					ComponentCreature cre = entity.FindComponent<ComponentCreature>();
					if (cre != null && entity.FindComponent<ComponentNPlayer>() == null && entity.FindComponent<ComponentHealth>().Health>0)
					{
						ComponentBody cra = entity.FindComponent<ComponentBody>();
						Vector3 vec = new Vector3(cra.BoundingBox.Center().X - po.X, cra.BoundingBox.Center().Y - po.Y, cra.BoundingBox.Center().Z - po.Z);
						var body = componentMiner.PickBody2(po + 1f * Vector3.Normalize(vec), Vector3.Normalize(vec),50f);
						var result = componentMiner.PickTerrainForDigging2(po+ 1f* Vector3.Normalize(vec), Vector3.Normalize(vec), 50f);
						if (body.HasValue && (!result.HasValue || body.Value.Distance < result.Value.Distance))
						{

							if (vec.Length() < 20f)
							{
								Utils.SubsystemProjectiles.FireProjectile(Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, BulletBlock.BulletType.IronBullet)), po + Vector3.Normalize(vec) * 1f, Vector3.Normalize(vec) * 260f, Vector3.Zero, null);
								Utils.SubsystemAudio.PlaySound("Audio/MusketFire", 1f, 0f, new Vector3(c.X + 0.5f, c.Y + 0.5f, c.Z + 0.5f), 10f, true);
								m_slots[abb].Count -= 1;
								break;
							}
						}
					}
				}

			}

		}
	}
}