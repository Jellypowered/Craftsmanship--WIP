using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Craftsmanship
{
    public class InteractionWorker_Craftschat : InteractionWorker
    {
        // Defines the skill that this interaction is focused on
        public SkillDef DiscussedSkill => ((CraftingInteractionDef)this.interaction).discussedSkill;

        // Checks if a pawn can initiate an interaction by ensuring both can talk and hear each other.
        public class CommunicationChecker
        {
            // Determines if two pawns can communicate.
            public bool CanPawnsCommunicate(Pawn initiator, Pawn recipient)
            {
                return CanPawnTalk(initiator) && CanPawnHear(recipient);
            }

            // Checks if a pawn can speak.
            private bool CanPawnTalk(Pawn pawn)
            {
                return pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking);
            }

            // Checks if a pawn can hear.
            private bool CanPawnHear(Pawn pawn)
            {
                return pawn.health.capacities.CapableOf(PawnCapacityDefOf.Hearing);
            }
        }

        // Internal multiplier to globally reduce interaction frequency (e.g., 0.3 = 30% of original chance).
        private const float BaseInteractionMultiplier = 0.3f;

        // Determines the likelihood of this interaction being selected.
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            // Create an instance of CommunicationChecker to check communication capabilities.
            var comms = new CommunicationChecker();

            // Skip if pawns can't communicate.
            if (!comms.CanPawnsCommunicate(initiator, recipient))
                return 0f;

            // Only allow player-faction colonists to participate
            if (initiator.Faction != Faction.OfPlayer || recipient.Faction != Faction.OfPlayer)
                return 0f;

            // Check for valid shared passion, return its weight if found
            var sharedPassion = GetSharedPassion(initiator, recipient);
            
            // Apply internal multiplier here to reduce overall frequency
            return (sharedPassion?.DiscussionWeight ?? 0f) * BaseInteractionMultiplier;
        }

        // Runs when the interaction actually happens
        public override void Interacted(Pawn initiator, Pawn recipient,
            List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel,
            out LetterDef letterDef, out LookTargets lookTargets)
        {
            letterText = null;
            letterDef = null;
            letterLabel = null;
            lookTargets = null;

            // Get skill relationship between initiator and recipient
            var sharedPassion = new SharedPassion(initiator, recipient, DiscussedSkill);
            var studentSkill = sharedPassion.StudentSkill;

            // Base XP amount
            float xp = 200f;

            // Adjust XP based on relative skill level
            if (sharedPassion.StudentIsScrub)
                xp = studentSkill.XpRequiredForLevelUp * 0.33f;
            else if (sharedPassion.StudentIsIntermediate)
                xp = studentSkill.XpRequiredForLevelUp * 0.15f;

            // Reduce XP if student is already skilled
            if (studentSkill.Level >= 9)
                xp = 10f;

            // 🔧 Apply XP multiplier from mod settings
            xp *= CraftsmanshipMod.Settings.xpMultiplier;

            // Apply the learning
            studentSkill.Learn(xp, false);

            // Apply the CraftChat thought to both pawns
            if (CraftsmanshipMod.Settings.socialBuff)
            {
                ThoughtDef craftChatThought = DefDatabase<ThoughtDef>.GetNamed("CraftChat", false);
                if (craftChatThought != null)
                {
                    initiator.needs?.mood?.thoughts?.memories?.TryGainMemory(craftChatThought, recipient); 
                    recipient.needs?.mood?.thoughts?.memories?.TryGainMemory(craftChatThought, initiator);
                }
            }

            // Skip generic sentence packs
            extraSentencePacks.Clear();
        }

        // Determines if the two pawns can have a meaningful skill discussion
        private SharedPassion GetSharedPassion(Pawn initiator, Pawn target)
        {
            var initiatorSkill = initiator.skills.GetSkill(DiscussedSkill);
            var targetSkill = target.skills.GetSkill(DiscussedSkill);

            // Must both have the skill and it must not be disabled
            if (initiatorSkill == null || initiatorSkill.TotallyDisabled) return null;
            if (targetSkill == null || targetSkill.TotallyDisabled) return null;

            // Both pawns must have at least minor passion for the skill
            if (CraftsmanshipMod.Settings.requirePassion)
            {
                bool initiatorHasPassion = initiatorSkill.passion != Passion.None;
                bool targetHasPassion = targetSkill.passion != Passion.None;
                if (!initiatorHasPassion || !targetHasPassion) return null;
            }

            // Roll chance based on mod setting
            float chance = CraftsmanshipMod.Settings?.chancePercent ?? 100f;
            if (Rand.Value * 100f <= chance)
            {
                return new SharedPassion(initiator, target, DiscussedSkill);
            }

            return null;
        }

        // Represents the relationship between two pawns who share a skill passion
        private class SharedPassion
        {
            public SkillDef Skill { get; }
            public Pawn Master { get; }
            public Pawn Student { get; }
            public SkillRecord MasterSkill { get; }
            public SkillRecord StudentSkill { get; }

            public SharedPassion(Pawn pawnA, Pawn pawnB, SkillDef skill)
            {
                Skill = skill;
                var skillA = pawnA.skills.GetSkill(skill);
                var skillB = pawnB.skills.GetSkill(skill);

                // Higher-skilled pawn becomes the "teacher"
                if (skillA.Level >= skillB.Level)
                {
                    Master = pawnA;
                    MasterSkill = skillA;
                    Student = pawnB;
                    StudentSkill = skillB;
                }
                else
                {
                    Master = pawnB;
                    MasterSkill = skillB;
                    Student = pawnA;
                    StudentSkill = skillA;
                }
            }

            // Student is far behind
            public bool StudentIsScrub => StudentSkill.Level < MasterSkill.Level * 0.33f;

            // Student is moderately behind
            public bool StudentIsIntermediate => StudentSkill.Level < MasterSkill.Level * 0.66f && !StudentIsScrub;

            // Affects how likely this conversation is to occur
            public float DiscussionWeight
            {
                get
                {
                    float weight = 0.12f;
                    if (StudentIsScrub)
                        weight += 2f;
                    else if (StudentIsIntermediate)
                        weight += 1f;
                    return weight;
                }
            }
        }
    }
}

