using System;
using System.Linq;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200027C RID: 636
	public class ComponentTwoBodyModel : ComponentModel
	{
		// Token: 0x060013CD RID: 5069 RVA: 0x000859C0 File Offset: 0x00083BC0
		public float m_headAngleY =0f;
		public override void Animate()
		{
			if (this.m_componentSpawn != null)
			{
				base.Opacity = new float?((this.m_componentSpawn.SpawnDuration > 0f) ? ((float)MathUtils.Saturate((this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_componentSpawn.SpawnTime) / (double)this.m_componentSpawn.SpawnDuration)) : 1f);
				if (this.m_componentSpawn.DespawnTime != null)
				{
					base.Opacity = new float?(MathUtils.Min(base.Opacity.Value, (float)MathUtils.Saturate(1.0 - (this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_componentSpawn.DespawnTime.Value) / (double)this.m_componentSpawn.DespawnDuration)));
				}
			}
			//base.Opacity = 1f;
			//SetBoneTransform(base.Model.RootBone.Index, m_componentFrame.Matrix);
			SetBoneTransform(this.m_bodyBone.Model.RootBone.Index, m_componentFrame.Matrix);
			//SetBoneTransform(this.m_leg1Bone.Index, this.m_componentFrame.Matrix);
			//SetBoneTransform(this.m_leg2Bone.Index, this.m_componentFrame.Matrix);
			//	SetBoneTransform(this.m_leg3Bone.Index, this.m_componentFrame.Matrix);
			//	SetBoneTransform(this.m_leg4Bone.Index, this.m_componentFrame.Matrix);
			//m_headBone.Transform = m_headBone.Transform * Matrix.CreateRotationZ(m_headAngleY);
			SetBoneTransform(this.m_headBone.Model.RootBone.Index, m_componentFrame.Matrix );
			SetBoneTransform(this.m_headBone.Index, Matrix.CreateRotationZ(m_headAngleY));
			//RenderingMode = ModelRenderingMode.Solid;
			//base.Animate();
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x00085AC1 File Offset: 0x00083CC1
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentSpawn = base.Entity.FindComponent<ComponentSpawn>();
			base.Load(valuesDictionary, idToEntityMap);
		}

		public override void SetModel(Model model)
		{
			base.SetModel(model);
			if (base.Model != null)
			{
				//m_boneTransforms = new Matrix?[m_model.Bones.Count];
				//AbsoluteBoneTransformsForCamera = new Matrix[m_model.Bones.Count];
				//MeshDrawOrders = Enumerable.Range(0, m_model.Meshes.Count).ToArray();
				this.m_bodyBone = base.Model.FindBone("Body", true);
				this.m_headBone = base.Model.FindBone("Head", true);
			//	this.m_leg1Bone = base.Model.FindBone("Leg1", true);
			//	this.m_leg2Bone = base.Model.FindBone("Leg2", true);
			//	this.m_leg3Bone = base.Model.FindBone("Leg3", true);
			//	this.m_leg4Bone = base.Model.FindBone("Leg4", true);
				return;
			}
			this.m_bodyBone = null;
			this.m_headBone = null;
		}
		public ModelBone m_bodyBone;

		// Token: 0x04000D5C RID: 3420
		public ModelBone m_headBone;
		// Token: 0x04001005 RID: 4101
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04001006 RID: 4102
		public ComponentSpawn m_componentSpawn;

		public ModelBone m_leg1Bone;

		// Token: 0x04000D5E RID: 3422
		public ModelBone m_leg2Bone;

		// Token: 0x04000D5F RID: 3423
		public ModelBone m_leg3Bone;

		// Token: 0x04000D60 RID: 3424
		public ModelBone m_leg4Bone;
	}
}