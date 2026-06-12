// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    /// <summary>
    /// Gap 1: behavioral coverage of every concept's effect on God's character, without asserting raw
    /// wiring. Witnessed alone, a virtue or mystery must drive at least one facet fully high (1.0) and
    /// a sin must drive at least one facet fully low (0.0). This catches a concept that bears on
    /// nothing cleanly (e.g. one that both upholds and violates the same facet, cancelling to 0.5),
    /// which the verdict-side reflection test would miss.
    /// </summary>
    public class FacetBehaviorTests
    {
        [Fact]
        public void EveryVirtueOrMystery_DrivesSomeFacetFullyHigh()
        {
            foreach (MoralConcept concept in Concepts().Where(c => c.Gravity == Gravity.None))
            {
                IReadOnlyDictionary<string, double> readings =
                    Reality.Revealed().Witness(new Act(concept.Name, concept)).Readings;

                Assert.True(readings.Values.Any(v => v == 1.0),
                    concept.GetType().Name + " drives no facet fully high; it bears on God's character weakly.");
            }
        }

        [Fact]
        public void EverySin_DrivesSomeFacetFullyLow()
        {
            foreach (MoralConcept concept in Concepts().Where(c => c.Gravity != Gravity.None))
            {
                IReadOnlyDictionary<string, double> readings =
                    Reality.Revealed().Witness(new Act(concept.Name, concept)).Readings;

                Assert.True(readings.Values.Any(v => v == 0.0),
                    concept.GetType().Name + " drives no facet fully low; its offence does not register cleanly.");
            }
        }

        private static IEnumerable<MoralConcept> Concepts()
        {
            Type baseType = typeof(MoralConcept);
            return baseType.Assembly.GetTypes()
                .Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
                .Select(t => (MoralConcept)Activator.CreateInstance(t));
        }
    }
}
