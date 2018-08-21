using System;
using GameEntitySystem;
using Engine;

namespace Game
{
    public class SubsystemTrainBlockBehavior : SubsystemBlockBehavior
    {
        public override int[] HandledBlocks => new int[]
        {
            TrainBlock.Index
        };

        public override bool OnUse(Vector3 start, Vector3 direction, ComponentMiner componentMiner)
        {
            if (Terrain.ExtractContents(componentMiner.ActiveBlockValue) == TrainBlock.Index)
            {
                var result = componentMiner.PickTerrainForDigging(start, direction);
                if (result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index)
                {
                    var cell = result.Value.CellFace;
                    var position = new Vector3(cell.X + 0.5f, cell.Y + 1f, cell.Z + 0.5f);

                    var entity = DatabaseManager.CreateEntity(Project, "Train", true);
                    entity.FindComponent<ComponentBody>(true).Position = position;
                    entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
                    Project.AddEntity(entity);
                }
                else
                {
                    var body = componentMiner.PickBody(start, direction);
                    if (body.HasValue && body.Value.ComponentBody.Entity.FindComponent<ComponentTrain>() != null)
                        Project.RemoveEntity(body.Value.ComponentBody.Entity, true);
                }
            }
            return base.OnUse(start, direction, componentMiner);
        }
    }
}
