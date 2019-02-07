using Engine;
using GameEntitySystem;
using System;
using System.Globalization;
using System.Text;
using TemplatesDatabase;

namespace Game
{
	public enum Trait
	{
		//AutoJump
		JumpStrength,

		//AvoidFire
		AvoidFire_DayRange,
		AvoidFire_NightRange,

		//AvoidPlayer
		AvoidPlayer_DayRange,
		AvoidPlayer_NightRange,

		//Body
		BoxSizeX,
		BoxSizeY,
		BoxSizeZ,
		Mass,
		Density,
		AirDragX,
		AirDragY,
		WaterDragX,
		WaterDragY,
		WaterSwayAngle,
		WaterTurnSpeed,
		MaxSmoothRiseHeight,

		//Chase
		DayChaseRange,
		NightChaseRange,
		DayChaseTime,
		NightChaseTime,
		AutoChaseMask,
		ChaseNonPlayerProbability,
		ChaseWhenAttackedProbability,
		ChaseOnTouchProbability,

		//DigInMud
		MaxDigInDepth,

		//EatPickable
		MeatFoodFactor,
		FishFoodFactor,
		FruitFoodFactor,
		GrassFoodFactor,
		BreadFoodFactor,

		//FindPlayer
		FindPlayer_DayRange,

		FindPlayer_NightRange,

		//GlowingEyes
		GlowingEyesColor,

		//Health
		AttackResilience,
		FallResilience,
		FireResilience,
		CanStrand,
		AirCapacity,

		//Herd
		HerdingRange, //负数表示AutoNearbyCreaturesHelp

		//Locomotion
		AccelerationFactor,
		WalkSpeed,
		LadderSpeed,
		JumpSpeed,
		CreativeFlySpeed,
		FlySpeed,
		SwimSpeed,
		TurnSpeed,
		LookSpeed,
		InAirWalkFactor,
		WalkSpeedWhenTurning,
		LookAutoLevel,

		//Miner
		AttackPower,

		//Model
		DiffuseColor,
		Opacity,

		//LayEgg
		LayFrequency,

		//Loot
		LootMinCount,
		LootMaxCount,
		LootProbability,
		LootOnFireMinCount,
		LootOnFireMaxCount,
		LootOnFireProbability,

		//Shapeshifter
		ShapeshifterProbability,

		//StubbornSteed
		StubbornProbability,

		//Udder
		MilkRegenerationTime,

		//Other
		ClimbSpeed,
		DigSpeed,
		SetFireProbability,
	}

	public struct Genome
	{
		public float[] DominantGenes;
		public float[] RecessiveGenes;

		public float this[Trait index]
		{
			get { return DominantGenes[(int)index]; }
			set { DominantGenes[(int)index] = RecessiveGenes[(int)index] = value; }
		}

		public Genome(float[] dominantGenes, float[] recessiveGenes)
		{
			DominantGenes = dominantGenes;
			RecessiveGenes = recessiveGenes;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			int i;
			for (i = 0; i < DominantGenes.Length; i++)
			{
				sb.Append(((Trait)i).ToString());
				sb.Append(": ");
				sb.Append(DominantGenes[i].ToString());
				if (i < RecessiveGenes.Length)
					sb.Append(RecessiveGenes[i].ToString());
				sb.AppendLine();
			}
			return sb.ToString();
		}
	}

	public class ComponentVariant : Component, IUpdateable
	{
		public Genome Genome;
		public double LastTime;

		public int UpdateOrder => 0;

		public static Genome Hybridize(Genome father, Genome mother)
		{
			var child = new Genome(new float[67], new float[67]);
			int i = 0, len = father.DominantGenes.Length;
			for (; i < len; i++)
				child.DominantGenes[i] = ((Utils.Random.Int() & 1) != 0 ? father.DominantGenes : father.RecessiveGenes)[i];
			for (i = 0; i < len; i++)
			{
				var val = ((Utils.Random.Int() & 1) != 0 ? mother.DominantGenes : mother.RecessiveGenes)[i];
				if (val > child.DominantGenes[i])
				{
					child.RecessiveGenes[i] = child.DominantGenes[i];
					child.DominantGenes[i] = val;
				}
				else
					child.RecessiveGenes[i] = val;
			}
			return child;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			var dGenes = new DynamicArray<float>();
			var rGenes = new DynamicArray<float>();
			var s = valuesDictionary.GetValue("Genome", string.Empty).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (s.Length == 0)
			{
				Genome = new Genome();
				return;
			}
			for (int i = 0; i < s.Length; i++)
			{
				int n = s[i].IndexOf(',');
				dGenes.Add(float.Parse(n > 0 ? s[i].Substring(0, n) : s[i], CultureInfo.InvariantCulture));
				if (n > 0)
					rGenes.Add(float.Parse(s[i].Substring(n + 1), CultureInfo.InvariantCulture));
			}
			var arr = dGenes.Array;
			Array.Resize(ref arr, dGenes.Count);
			arr = rGenes.Array;
			Array.Resize(ref arr, dGenes.Count);
			Genome = new Genome(dGenes.Array, arr);
			LastTime = valuesDictionary.GetValue("Genome", 0.0);
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			int i,s, r = Genome.RecessiveGenes.Length;
			for (i = Genome.DominantGenes.Length; i-- > 0;)
				if (Genome.DominantGenes[i] != 0)
					break;
			s = i;
			for (i = r; i-- > 0;)
				if (Genome.RecessiveGenes[i] != 0)
					break;
			var sb = new StringBuilder();
			for (i = 0; i < s; i++)
			{
				sb.Append(Genome.DominantGenes[i].ToString());
				if (i < r)
				{
					sb.Append(',');
					sb.Append(Genome.RecessiveGenes[i].ToString());
				}
				sb.Append(';');
			}
			valuesDictionary.SetValue("Genome", sb.ToString());
			valuesDictionary.SetValue("LastTime", LastTime);
		}

		public override void OnEntityAdded()
		{
			if (Genome.DominantGenes == null)
			{
				Initialize();
			}
			var caj = Entity.FindComponent<ComponentAutoJump>();
			if (caj != null)
				caj.m_jumpStrength = Genome[Trait.JumpStrength];
			var cafb = Entity.FindComponent<ComponentAvoidFireBehavior>();
			if (cafb != null)
			{
				cafb.m_dayRange = Genome[Trait.AvoidFire_DayRange];
				cafb.m_nightRange = Genome[Trait.AvoidFire_NightRange];
			}
			var capb = Entity.FindComponent<ComponentAvoidPlayerBehavior>();
			if (capb != null)
			{
				capb.m_dayRange = Genome[Trait.AvoidPlayer_DayRange];
				capb.m_nightRange = Genome[Trait.AvoidPlayer_NightRange];
			}
			var cb = Entity.FindComponent<ComponentBody>();
			if (cb != null)
			{
				cb.BoxSize = new Vector3(
					Genome[Trait.BoxSizeX],
					Genome[Trait.BoxSizeY],
					Genome[Trait.BoxSizeZ]);
				cb.Mass = Genome[Trait.Mass];
				cb.Density = Genome[Trait.Density];
				var vec2 = new Vector2(Genome[Trait.AirDragX], Genome[Trait.AirDragY]);
				cb.AirDrag = vec2;
				vec2 = new Vector2(Genome[Trait.WaterDragX], Genome[Trait.WaterDragY]);
				cb.WaterDrag = vec2;
				cb.WaterSwayAngle = Genome[Trait.WaterSwayAngle];
				cb.WaterTurnSpeed = Genome[Trait.WaterTurnSpeed];
				cb.MaxSmoothRiseHeight = Genome[Trait.MaxSmoothRiseHeight];
			}
			var ccb = Entity.FindComponent<ComponentChaseBehavior>();
			if (ccb != null)
			{
				ccb.m_dayChaseRange = Genome[Trait.DayChaseRange];
				ccb.m_nightChaseRange = Genome[Trait.NightChaseRange];
				ccb.m_dayChaseTime = Genome[Trait.DayChaseTime];
				ccb.m_nightChaseTime = Genome[Trait.NightChaseTime];
				ccb.m_autoChaseMask = (CreatureCategory)(int)Genome[Trait.AutoChaseMask];
				ccb.m_chaseNonPlayerProbability = Genome[Trait.ChaseNonPlayerProbability];
				ccb.m_chaseWhenAttackedProbability = Genome[Trait.ChaseWhenAttackedProbability];
				ccb.m_chaseOnTouchProbability = Genome[Trait.ChaseOnTouchProbability];
			}
			var cdb = Entity.FindComponent<ComponentDigInMudBehavior>();
			if (cdb != null)
				cdb.m_maxDigInDepth = Genome[Trait.MaxDigInDepth];
			var ceb = Entity.FindComponent<ComponentEatPickableBehavior>();
			if (ceb != null)
			{
				ceb.m_foodFactors[(int)FoodType.Meat] = Genome[Trait.MeatFoodFactor];
				ceb.m_foodFactors[(int)FoodType.Fish] = Genome[Trait.FishFoodFactor];
				ceb.m_foodFactors[(int)FoodType.Fruit] = Genome[Trait.FruitFoodFactor];
				ceb.m_foodFactors[(int)FoodType.Grass] = Genome[Trait.GrassFoodFactor];
				ceb.m_foodFactors[(int)FoodType.Bread] = Genome[Trait.BreadFoodFactor];
			}
			var cfb = Entity.FindComponent<ComponentFindPlayerBehavior>();
			if (cfb != null)
			{
				cfb.m_dayRange = Genome[Trait.FindPlayer_DayRange];
				cfb.m_nightRange = Genome[Trait.FindPlayer_NightRange];
			}
			var cge = Entity.FindComponent<ComponentGlowingEyes>();
			if (cge != null)
				cge.GlowingEyesColor = new Color((uint)Genome[Trait.GlowingEyesColor] | 4278190080u);
			var ch = Entity.FindComponent<ComponentHealth>();
			if (ch != null)
			{
				ch.AttackResilience = Genome[Trait.AttackResilience];
				ch.FallResilience = Genome[Trait.FallResilience];
				ch.FireResilience = Genome[Trait.FireResilience];
				ch.CanStrand = Genome[Trait.CanStrand] > 0;
				ch.AirCapacity = Genome[Trait.AirCapacity];
			}
			var chb = Entity.FindComponent<ComponentHerdBehavior>();
			if (chb != null)
			{
				float v = Genome[Trait.HerdingRange];
				chb.m_herdingRange = MathUtils.Abs(v);
				chb.m_autoNearbyCreaturesHelp = v < 0;
			}
			var cl = Entity.FindComponent<ComponentLocomotion>();
			if (cl != null)
			{
				cl.AccelerationFactor = Genome[Trait.AccelerationFactor];
				cl.WalkSpeed = Genome[Trait.WalkSpeed];
				cl.m_walkSpeedWhenTurning = Genome[Trait.WalkSpeedWhenTurning];
				cl.LadderSpeed = Genome[Trait.LadderSpeed];
				cl.JumpSpeed = Genome[Trait.JumpSpeed];
				cl.CreativeFlySpeed = Genome[Trait.CreativeFlySpeed];
				cl.FlySpeed = Genome[Trait.FlySpeed];
				cl.SwimSpeed = Genome[Trait.SwimSpeed];
				cl.TurnSpeed = Genome[Trait.TurnSpeed];
				cl.LookSpeed = Genome[Trait.LookSpeed];
				cl.InAirWalkFactor = Genome[Trait.InAirWalkFactor];
				int v = (int)Genome[Trait.LookAutoLevel];
				cl.m_lookAutoLevelX = (v & 1) != 0;
				cl.m_lookAutoLevelY = (v & 2) != 0;
			}
			var cloot = Entity.FindComponent<ComponentLoot>();
			if (cloot != null)
			{
				ComponentLoot.Loot item;
				if (cloot.m_lootList.Count > 0)
				{
					item = cloot.m_lootList[0];
					item.MinCount = (int)Genome[Trait.LootMinCount];
					item.MaxCount = (int)Genome[Trait.LootMaxCount];
					item.Probability = Genome[Trait.LootProbability];
					cloot.m_lootList[0] = item;
				}
				if (cloot.m_lootOnFireList.Count > 0)
				{
					item = cloot.m_lootOnFireList[0];
					item.MinCount = (int)Genome[Trait.LootOnFireMinCount];
					item.MaxCount = (int)Genome[Trait.LootOnFireMaxCount];
					item.Probability = Genome[Trait.LootOnFireProbability];
					cloot.m_lootOnFireList[0] = item;
				}
			}
			var cmr = Entity.FindComponent<ComponentMiner>();
			if (cmr != null)
				cmr.AttackPower = Genome[Trait.AttackPower];
			var cm = Entity.FindComponent<ComponentModel>();
			if (cm != null)
			{
				cm.DiffuseColor = new Vector3(new Color((uint)Genome[Trait.DiffuseColor] | 4278190080u));
				cm.Opacity = Genome[Trait.Opacity];
			}
			var cleb = Entity.FindComponent<ComponentLayEggBehavior>();
			if (cleb != null)
				cleb.m_layFrequency = Genome[Trait.LayFrequency];
			var cs = Entity.FindComponent<ComponentShapeshifter>();
			if (cs != null)
				cs.IsEnabled = Utils.Random.UniformFloat(0f, 1f) < Genome[Trait.ShapeshifterProbability];
			var cssb = Entity.FindComponent<ComponentStubbornSteedBehavior>();
			if (cssb != null)
				cssb.m_stubbornProbability = Genome[Trait.StubbornProbability];
			var cu = Entity.FindComponent<ComponentUdder>();
			if (cu != null)
				cu.m_milkRegenerationTime = Genome[Trait.MilkRegenerationTime];
			Hybridize();
		}

		public void Mutate()
		{
			Trait t;
			for (t = Trait.DayChaseRange; t <= Trait.NightChaseTime; t++)
				if ((Utils.Random.Int() & 1) != 0)
				Genome[t] *= Utils.Random.UniformFloat(1f, 1.1f);
			for (t = Trait.WalkSpeed; t <= Trait.TurnSpeed; t++)
				if ((Utils.Random.Int() & 1) != 0)
					Genome[t] *= Utils.Random.UniformFloat(1f, 1.1f);
			if ((Utils.Random.Int() & 1) != 0)
				Genome[Trait.FindPlayer_DayRange] *= Utils.Random.UniformFloat(1f, 1.1f);
			else
				Genome[Trait.FindPlayer_NightRange] *= Utils.Random.UniformFloat(1f, 1.1f);
			if ((Utils.Random.Int() & 1) != 0)
				Genome[Trait.DigSpeed] *= Utils.Random.UniformFloat(1f, 1.04f);
			if ((Utils.Random.Int() & 1) != 0)
				Genome[Trait.AirCapacity] *= Utils.Random.UniformFloat(1f, 1.04f);
		}

		public void Initialize()
		{
			Genome = new Genome(new float[65], new float[65]);
			var caj = Entity.FindComponent<ComponentAutoJump>();
			if (caj != null)
				Genome[Trait.JumpStrength] = caj.m_jumpStrength;
			var cafb = Entity.FindComponent<ComponentAvoidFireBehavior>();
			if (cafb != null)
			{
				Genome[Trait.AvoidFire_DayRange] = cafb.m_dayRange;
				Genome[Trait.AvoidFire_NightRange] = cafb.m_nightRange;
			}
			var capb = Entity.FindComponent<ComponentAvoidPlayerBehavior>();
			if (capb != null)
			{
				Genome[Trait.AvoidPlayer_DayRange] = capb.m_dayRange;
				Genome[Trait.AvoidPlayer_NightRange] = capb.m_nightRange;
			}
			var cb = Entity.FindComponent<ComponentBody>();
			if (cb != null)
			{
				cb.BoxSize = new Vector3(
					Genome[Trait.BoxSizeX],
					Genome[Trait.BoxSizeY],
					Genome[Trait.BoxSizeZ]);
				Genome[Trait.Mass] = cb.Mass;
				Genome[Trait.Density] = cb.Density;
				var vec2 = new Vector2(Genome[Trait.AirDragX], Genome[Trait.AirDragY]);
				cb.AirDrag = vec2;
				vec2 = new Vector2(Genome[Trait.WaterDragX], Genome[Trait.WaterDragY]);
				cb.WaterDrag = vec2;
				Genome[Trait.WaterSwayAngle] = cb.WaterSwayAngle;
				Genome[Trait.WaterTurnSpeed] = cb.WaterTurnSpeed;
				Genome[Trait.MaxSmoothRiseHeight] = cb.MaxSmoothRiseHeight;
			}
			var ccb = Entity.FindComponent<ComponentChaseBehavior>();
			if (ccb != null)
			{
				Genome[Trait.DayChaseRange] = ccb.m_dayChaseRange;
				Genome[Trait.NightChaseRange] = ccb.m_nightChaseRange;
				Genome[Trait.DayChaseTime] = ccb.m_dayChaseTime;
				Genome[Trait.NightChaseTime] = ccb.m_nightChaseTime;
				Genome[Trait.AutoChaseMask] = (int)ccb.m_autoChaseMask;
				Genome[Trait.ChaseNonPlayerProbability] = ccb.m_chaseNonPlayerProbability;
				Genome[Trait.ChaseWhenAttackedProbability] = ccb.m_chaseWhenAttackedProbability;
				Genome[Trait.ChaseOnTouchProbability] = ccb.m_chaseOnTouchProbability;
			}
			var cdb = Entity.FindComponent<ComponentDigInMudBehavior>();
			if (cdb != null)
				Genome[Trait.MaxDigInDepth] = cdb.m_maxDigInDepth;
			var ceb = Entity.FindComponent<ComponentEatPickableBehavior>();
			if (ceb != null)
			{
				Genome[Trait.MeatFoodFactor] = ceb.m_foodFactors[(int)FoodType.Meat];
				Genome[Trait.FishFoodFactor] = ceb.m_foodFactors[(int)FoodType.Fish];
				Genome[Trait.FruitFoodFactor] = ceb.m_foodFactors[(int)FoodType.Fruit];
				Genome[Trait.GrassFoodFactor] = ceb.m_foodFactors[(int)FoodType.Grass];
				Genome[Trait.BreadFoodFactor] = ceb.m_foodFactors[(int)FoodType.Bread];
			}
			var cfb = Entity.FindComponent<ComponentFindPlayerBehavior>();
			if (cfb != null)
			{
				Genome[Trait.FindPlayer_DayRange] = cfb.m_dayRange;
				Genome[Trait.FindPlayer_NightRange] = cfb.m_nightRange;
			}
			var cge = Entity.FindComponent<ComponentGlowingEyes>();
			if (cge != null)
				Genome[Trait.GlowingEyesColor] = cge.GlowingEyesColor.PackedValue & 16777215u;
			var ch = Entity.FindComponent<ComponentHealth>();
			if (ch != null)
			{
				Genome[Trait.AttackResilience] = ch.AttackResilience;
				Genome[Trait.FallResilience] = ch.FallResilience;
				Genome[Trait.FireResilience] = ch.FireResilience;
				Genome[Trait.CanStrand] = ch.CanStrand ? 1 : -1;
				Genome[Trait.AirCapacity] = ch.AirCapacity;
			}
			var chb = Entity.FindComponent<ComponentHerdBehavior>();
			if (chb != null)
			{
				float v = chb.m_herdingRange;
				if (chb.m_autoNearbyCreaturesHelp) v = -v;
				Genome[Trait.HerdingRange] = v;
			}
			var cl = Entity.FindComponent<ComponentLocomotion>();
			if (cl != null)
			{
				Genome[Trait.AccelerationFactor] = cl.AccelerationFactor;
				Genome[Trait.WalkSpeed] = cl.WalkSpeed;
				Genome[Trait.WalkSpeedWhenTurning] = cl.m_walkSpeedWhenTurning;
				Genome[Trait.LadderSpeed] = cl.LadderSpeed;
				Genome[Trait.JumpSpeed] = cl.JumpSpeed;
				Genome[Trait.CreativeFlySpeed] = cl.CreativeFlySpeed;
				Genome[Trait.FlySpeed] = cl.FlySpeed;
				Genome[Trait.SwimSpeed] = cl.SwimSpeed;
				Genome[Trait.TurnSpeed] = cl.TurnSpeed;
				Genome[Trait.LookSpeed] = cl.LookSpeed;
				Genome[Trait.InAirWalkFactor] = cl.InAirWalkFactor;
				int v = cl.m_lookAutoLevelX ? 1 : 0;
				if(cl.m_lookAutoLevelY) v |=2;
				Genome[Trait.LookAutoLevel] = v;
			}
			var cloot = Entity.FindComponent<ComponentLoot>();
			if (cloot != null)
			{
				ComponentLoot.Loot item;
				if (cloot.m_lootList.Count > 0)
				{
					item = cloot.m_lootList[0];
					Genome[Trait.LootMinCount] = item.MinCount;
					Genome[Trait.LootMaxCount] = item.MaxCount;
					Genome[Trait.LootProbability] = item.Probability;
				}
				if (cloot.m_lootOnFireList.Count > 0)
				{
					item = cloot.m_lootOnFireList[0];
					Genome[Trait.LootOnFireMinCount] = item.MinCount;
					Genome[Trait.LootOnFireMaxCount] = item.MaxCount;
					Genome[Trait.LootOnFireProbability] = item.Probability;
				}
			}
			var cmr = Entity.FindComponent<ComponentMiner>();
			if (cmr != null)
				Genome[Trait.AttackPower] = cmr.AttackPower;
			var cm = Entity.FindComponent<ComponentModel>();
			if (cm != null)
			{
				Genome[Trait.DiffuseColor] = new Color(cm.DiffuseColor ?? Vector3.One).PackedValue & 16777215u;
				Genome[Trait.Opacity] = cm.Opacity ?? 1;
			}
			var cleb = Entity.FindComponent<ComponentLayEggBehavior>();
			if (cleb != null)
				Genome[Trait.LayFrequency] = cleb.m_layFrequency;
			var cs = Entity.FindComponent<ComponentShapeshifter>();
			if (cs != null)
				cs.IsEnabled = Utils.Random.UniformFloat(0f, 1f) < Genome[Trait.ShapeshifterProbability];
			var cssb = Entity.FindComponent<ComponentStubbornSteedBehavior>();
			if (cssb != null)
				Genome[Trait.StubbornProbability] = cssb.m_stubbornProbability;
			var cu = Entity.FindComponent<ComponentUdder>();
			if (cu != null)
				Genome[Trait.MilkRegenerationTime] = cu.m_milkRegenerationTime;
		}
		public override void OnEntityRemoved()
		{
			Hybridize();
		}
		public void Hybridize()
		{
			var name = Entity.ValuesDictionary.DatabaseObject.Name;
			var e = Project.Entities.GetEnumerator();
			while (e.MoveNext())
			{
				var current = e.Current;
				var cv = current.FindComponent<ComponentVariant>();
				if (cv == null) continue;
				if (string.Equals(current.ValuesDictionary.DatabaseObject.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					cv.Genome = Hybridize(Genome, cv.Genome);
					return;
				}
			}
		}

		public void Update(float dt)
		{
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(7, 0.0))
			{
				//for (int i = (int)(Utils.SubsystemGameInfo.TotalElapsedGameTime - LastTime) /600; i-- > 0;)
					Mutate();
				LastTime = Utils.SubsystemGameInfo.TotalElapsedGameTime;
			}
		}
	}
	public class GenomeViewer : FlatItem
	{
		public GenomeViewer()
		{
			DefaultTextureSlot = 122;
			DefaultDisplayName = "Genome Viewer";
		}
	}
}