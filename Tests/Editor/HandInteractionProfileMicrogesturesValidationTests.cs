using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace UnityEditor.XR.OpenXR.Tests
{
    [Category("Level.Integration")]
    [NUnit.Framework.Property("Covers", "ProjectValidation")]
    class HandInteractionProfileMicrogesturesValidationTests
    {
        const BuildTargetGroup k_Target = BuildTargetGroup.Standalone;

        static List<OpenXRFeature.ValidationRule> GetFailingRules(HandInteractionProfileMicrogestures microgestures)
        {
            var rules = new List<OpenXRFeature.ValidationRule>();
            microgestures.GetValidationChecks(rules, k_Target);

            var failingRules = new List<OpenXRFeature.ValidationRule>();
            foreach (var rule in rules)
            {
                if (!rule.checkPredicate())
                    failingRules.Add(rule);
            }

            return failingRules;
        }

        [Test]
        [Description("With the Hand Interaction Profile disabled, the microgestures feature reports exactly one failing " +
                     "validation rule: an error that blocks entering play mode and has an automatic fix, which enables " +
                     "the Hand Interaction Profile and clears the failure.")]
        public void HandInteractionProfileRuleFailsUntilFixed()
        {
            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(k_Target);
            var microgestures = settings.GetFeature<HandInteractionProfileMicrogestures>();
            var handInteractionProfile = settings.GetFeature<HandInteractionProfile>();
            Assert.That(microgestures, Is.Not.Null);
            Assert.That(handInteractionProfile, Is.Not.Null);

            // The settings asset is shared project state, so restore it even if an assertion fails.
            var microgesturesWasEnabled = microgestures.enabled;
            var handInteractionProfileWasEnabled = handInteractionProfile.enabled;
            try
            {
                microgestures.enabled = true;
                handInteractionProfile.enabled = false;
                Assert.That(handInteractionProfile.enabled, Is.False);

                var failingRules = GetFailingRules(microgestures);
                Assert.That(failingRules.Count, Is.EqualTo(1));

                var rule = failingRules[0];
                Assert.That(rule.error, Is.True);
                Assert.That(rule.errorEnteringPlaymode, Is.True);
                Assert.That(rule.fixItAutomatic, Is.True);

                rule.fixIt();

                Assert.That(handInteractionProfile.enabled, Is.True);
                Assert.That(rule.checkPredicate(), Is.True);
            }
            finally
            {
                microgestures.enabled = microgesturesWasEnabled;
                handInteractionProfile.enabled = handInteractionProfileWasEnabled;
            }
        }

        [Test]
        [Description("With the Hand Interaction Profile enabled, the microgestures feature reports no failing validation rules.")]
        public void NoRuleFailsWhenHandInteractionProfileEnabled()
        {
            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(k_Target);
            var microgestures = settings.GetFeature<HandInteractionProfileMicrogestures>();
            var handInteractionProfile = settings.GetFeature<HandInteractionProfile>();
            Assert.That(microgestures, Is.Not.Null);
            Assert.That(handInteractionProfile, Is.Not.Null);

            var microgesturesWasEnabled = microgestures.enabled;
            var handInteractionProfileWasEnabled = handInteractionProfile.enabled;
            try
            {
                microgestures.enabled = true;
                handInteractionProfile.enabled = true;

                Assert.That(GetFailingRules(microgestures), Is.Empty);
            }
            finally
            {
                microgestures.enabled = microgesturesWasEnabled;
                handInteractionProfile.enabled = handInteractionProfileWasEnabled;
            }
        }
    }
}
