using NUnit.Framework;
using UnityEngine;
using SentinelForge.Features.Effects.Reactions;

namespace SentinelForge.Tests.EditMode
{
    [TestFixture]
    public class ModifierPipelineTests
    {
        [Test]
        public void ProjectileRuntimeState_Reset_RestoresDefaultValues()
        {
            var state = new ProjectileRuntimeState();
            state.CurrentDamage = 999f;
            state.DamageMultiplier = 2.5f;
            state.SetStat("TestStat", 42);

            state.Reset(50f);

            Assert.AreEqual(50f, state.CurrentDamage);
            Assert.AreEqual(1f, state.DamageMultiplier);
            Assert.AreEqual(0, state.GetStat("TestStat"));
            Assert.IsNull(state.HomingTarget);
        }

        [Test]
        public void ProjectileRuntimeState_Stats_AddAndGetCorrectly()
        {
            var state = new ProjectileRuntimeState();
            state.Reset(10f);

            state.SetStat("Bounces", 3);
            Assert.AreEqual(3, state.GetStat("Bounces"));

            state.AddStat("Bounces", -1);
            Assert.AreEqual(2, state.GetStat("Bounces"));
        }

        [Test]
        public void HitActionContext_RentAndReturnPool_ReusesInstance()
        {
            HitActionContext ctx1 = ProjectileRuntimeState.RentContext();
            ctx1.TerminateProjectile = false;
            ctx1.CancelDamage = true;

            ProjectileRuntimeState.ReturnContext(ctx1);

            HitActionContext ctx2 = ProjectileRuntimeState.RentContext();
            Assert.IsTrue(ctx2.TerminateProjectile, "Context mới thuê từ Pool phải được reset TerminateProjectile = true");
            Assert.IsFalse(ctx2.CancelDamage, "Context mới thuê từ Pool phải được reset CancelDamage = false");

            ProjectileRuntimeState.ReturnContext(ctx2);
        }

        [Test]
        public void PierceModifier_InheritState_CopiesPierceCount()
        {
            var pierceMod = ScriptableObject.CreateInstance<PierceModifier>();
            var sourceState = new ProjectileRuntimeState();
            var destState = new ProjectileRuntimeState();

            sourceState.SetStat(PierceModifier.PIERCE_COUNT, 3);
            pierceMod.InheritState(sourceState, destState);

            Assert.AreEqual(3, destState.GetStat(PierceModifier.PIERCE_COUNT));
        }

        [Test]
        public void BounceModifier_InheritState_CopiesBounceCount()
        {
            var bounceMod = ScriptableObject.CreateInstance<BounceModifier>();
            var sourceState = new ProjectileRuntimeState();
            var destState = new ProjectileRuntimeState();

            sourceState.SetStat(BounceModifier.BOUNCE_COUNT, 5);
            bounceMod.InheritState(sourceState, destState);

            Assert.AreEqual(5, destState.GetStat(BounceModifier.BOUNCE_COUNT));
        }

        [Test]
        public void ElementalReaction_Matches_ReturnsTrueInBothOrders()
        {
            var reaction = ScriptableObject.CreateInstance<ElementalReactionSO>();
            var fireEffect = ScriptableObject.CreateInstance<FireEffectData>();
            var stunEffect = ScriptableObject.CreateInstance<StunEffectData>();

            reaction.primaryEffect = fireEffect;
            reaction.secondaryEffect = stunEffect;

            Assert.IsTrue(reaction.Matches(fireEffect, stunEffect), "Phải khớp theo chiều Thuận (A + B)");
            Assert.IsTrue(reaction.Matches(stunEffect, fireEffect), "Phải khớp theo chiều Nghịch (B + A)");
        }
    }
}
