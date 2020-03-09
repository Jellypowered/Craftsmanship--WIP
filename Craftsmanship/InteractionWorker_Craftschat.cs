using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Craftsmanship
{
	// Token: 0x02000003 RID: 3
	public class InteractionWorker_Craftschat : InteractionWorker
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x0000205C File Offset: 0x0000025C
		public SkillDef discussedSkill
		{
			get
			{	
				return ((CraftingInteractionDef)this.interaction).discussedSkill;
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002080 File Offset: 0x00000280
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			bool flag = initiator.Faction != Faction.OfPlayer || recipient.Faction != Faction.OfPlayer;
			bool flag3 = flag;
			float result;
			if (flag3)
			{
				result = 0f;
			}
			else
			{
				this._sharedPassion = this.GetSharedPassion(initiator, recipient);
				bool flag2 = this._sharedPassion != null;
				bool flag4 = flag2;
				if (flag4)
				{
					result = this._sharedPassion.DiscussionWeight;
				}
				else
				{
					result = 0f;
				}
			}
			return result;
		}


		
		// Token: 0x06000004 RID: 4 RVA: 0x00002100 File Offset: 0x00000300
		public override void Interacted
			(Pawn initiator,
			Pawn recipient,
			List<RulePackDef> extraSentencePacks,
			out string letterText,
			out string letterLabel,
			out LetterDef letterDef,
			out LookTargets lookTargets)
		{
			letterText = null;
			letterDef = null;
			letterLabel = null;
			lookTargets = null;
			SkillRecord skill = this._sharedPassion.Master.skills.GetSkill(this._sharedPassion.Skill);
			SkillRecord skill2 = this._sharedPassion.Student.skills.GetSkill(this._sharedPassion.Skill);
			float xp = 200f;
			bool studentIsScrub = this._sharedPassion.StudentIsScrub;
			bool flag2 = studentIsScrub;
			if (flag2)
			{
				xp = skill2.XpRequiredForLevelUp * 0.33f;
			}
			else
			{
				bool studentIsIntermediate = this._sharedPassion.StudentIsIntermediate;
				bool flag3 = studentIsIntermediate;
				if (flag3)
				{
					xp = skill2.XpRequiredForLevelUp * 0.15f;
				}
			}
			bool flag = skill2.Level >= 9;
			bool flag4 = flag;
			if (flag4)
			{
				xp = 10f;
			}

			skill2.Learn(xp, false);
            extraSentencePacks.Clear();
			}

		// Token: 0x06000005 RID: 5 RVA: 0x000021DC File Offset: 0x000003DC
		private InteractionWorker_Craftschat.SharedPassion GetSharedPassion(Pawn initiator, Pawn target)
		{
			SkillRecord skill = initiator.skills.GetSkill(this.discussedSkill);
			bool flag = initiator.skills.GetSkill(this.discussedSkill) != null && !skill.TotallyDisabled;  
				//skill.passion != null && !skill.TotallyDisabled;
			bool flag3 = flag;
			if (flag3)
			{
				SkillRecord skill2 = target.skills.GetSkill(this.discussedSkill);
				bool flag2 = target.skills.GetSkill(this.discussedSkill) != null && !skill2.TotallyDisabled;
				bool flag4 = flag2;
				if (flag4)
				{
					return new InteractionWorker_Craftschat.SharedPassion(initiator, target, this.discussedSkill);
				}
			}
			return null;
		}

		// Token: 0x04000002 RID: 2
		private InteractionWorker_Craftschat.SharedPassion _sharedPassion;

		// Token: 0x02000004 RID: 4
		private class SharedPassion
		{
			// Token: 0x06000007 RID: 7 RVA: 0x0000226C File Offset: 0x0000046C
			public SharedPassion(Pawn pawnA, Pawn pawnB, SkillDef skill)
			{
				this.Skill = skill;
				SkillRecord skill2 = pawnA.skills.GetSkill(skill);
				SkillRecord skill3 = pawnB.skills.GetSkill(skill);
				bool flag = skill2.Level >= skill3.Level;
				bool flag2 = flag;
				if (flag2)
				{
					this.Master = pawnA;
					this.MasterSkill = skill2;
					this.Student = pawnB;
					this.StudentSkill = skill3;
				}
				else
				{
					this.Master = pawnA;
					this.MasterSkill = skill2;
					this.Student = pawnB;
					this.StudentSkill = skill3;
				}
			}

			// Token: 0x17000002 RID: 2
			// (get) Token: 0x06000008 RID: 8 RVA: 0x000022F8 File Offset: 0x000004F8
			public bool StudentIsScrub
			{
				get
				{
					return (float)this.StudentSkill.Level < (float)this.MasterSkill.Level * 0.33f;
				}
			}

			// Token: 0x17000003 RID: 3
			// (get) Token: 0x06000009 RID: 9 RVA: 0x0000232C File Offset: 0x0000052C
			public bool StudentIsIntermediate
			{
				get
				{
					return (float)this.StudentSkill.Level < (float)this.MasterSkill.Level * 0.66f && (float)this.StudentSkill.Level > (float)this.MasterSkill.Level * 0.33f;
				}
			}

			// Token: 0x17000004 RID: 4
			// (get) Token: 0x0600000A RID: 10 RVA: 0x00002384 File Offset: 0x00000584
			public int LevelDiff
			{
				get
				{
					return this.MasterSkill.Level - this.StudentSkill.Level;
				}
			}

			// Token: 0x17000005 RID: 5
			// (get) Token: 0x0600000B RID: 11 RVA: 0x000023B0 File Offset: 0x000005B0
			public float DiscussionWeight
			{
				get
				{
					float num = 0.12f;
					bool studentIsScrub = this.StudentIsScrub;
					bool flag = studentIsScrub;
					if (flag)
					{
						num += 2f;
					}
					else
					{
						bool studentIsIntermediate = this.StudentIsIntermediate;
						bool flag2 = studentIsIntermediate;
						if (flag2)
						{
							num += 1f;
						}
					}
					return num;
				}
			}

			// Token: 0x04000003 RID: 3
			public SkillDef Skill;

			// Token: 0x04000004 RID: 4
			public Pawn Master;

			// Token: 0x04000005 RID: 5
			public Pawn Student;

			// Token: 0x04000006 RID: 6
			public SkillRecord MasterSkill;

			// Token: 0x04000007 RID: 7
			public SkillRecord StudentSkill;
		}
	}
}
