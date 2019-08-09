using Engine;
using Game;
using System;
using TemplatesDatabase;
using System.Xml.Linq;
namespace Game
{
public class HandledTorchBlockBehavior : SubsystemBlockBehavior, IUpdateable
{
	private SubsystemPlayers subsystemPlayers;

	private SubsystemTerrain subsystemTerrain;

	private SubsystemTime subsystemTime;

	private SubsystemFireBlockBehavior subsystemFire;

	private readonly Point3[] lightingPoints = (Point3[])new Point3[4];

	public override int[] HandledBlocks => new int[1]
	{
		31
	};

	public int UpdateOrder => 0;

	public override void Load(ValuesDictionary valuesDictionary)
	{
		base.Load(valuesDictionary);
		subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>();
		subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>();
		subsystemTime = base.Project.FindSubsystem<SubsystemTime>();
		subsystemFire = base.Project.FindSubsystem<SubsystemFireBlockBehavior>();
	}


	public void Update(float dt)
	{
		if (subsystemTime.PeriodicGameTimeEvent(0.1, 0.0))
		{
			int num = 0;
			foreach (ComponentPlayer componentPlayer in subsystemPlayers.ComponentPlayers)
			{
				Point3 point = GetPoint(componentPlayer.ComponentBody.Position) + Point3.UnitY;
				IInventory inventory = componentPlayer.ComponentMiner.Inventory;
				if (Terrain.ExtractContents(inventory.GetSlotValue(inventory.ActiveSlotIndex)) == 31)
				{
					if (point != lightingPoints[num])
					{
						//Log.Information(base.SubsystemTerrain.Terrain.GetCellContents(point.X, point.Y, point.Z));
						if (base.SubsystemTerrain.Terrain.GetCellContents(point.X, point.Y, point.Z) == 0)
						{
							subsystemTerrain.ChangeCell(point.X, point.Y, point.Z, 500);
							if (lightingPoints[num] != Point3.Zero)
							{
								Point3 point2 = lightingPoints[num];
								subsystemTerrain.ChangeCell(point2.X, point2.Y, point2.Z, 0);
							}
							lightingPoints[num] = point;
						}
					}
				}
				else if (lightingPoints[num] != Point3.Zero)
				{
					Point3 point3 = lightingPoints[num];
					subsystemTerrain.ChangeCell(point3.X, point3.Y, point3.Z, 0);
					lightingPoints[num] = Point3.Zero;
				}
				num++;
			}
		}
	}


	public Point3 GetPoint(Vector3 v)
	{
		return new Point3((int)MathUtils.Round(v.X), (int)MathUtils.Round(v.Y), (int)MathUtils.Round(v.Z));
	}

}
}