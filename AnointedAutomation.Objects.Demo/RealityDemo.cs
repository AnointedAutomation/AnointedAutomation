// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using System;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Objects.Demo
{
    /// <summary>
    /// Demonstrates the divine grounding / reality engine: reality is grounded in God's whole
    /// character at once (Love, Justice, Mercy, Order), every deed is composed of first-class moral
    /// concepts (not strings), is witnessed and written onto the heavenly tablets, and the standing
    /// order of the world rises or drifts with what is done in it. Scripture is the oracle: the acts
    /// of God read fully coherent and destabilize nothing, sin reads incoherent and disorders
    /// reality, and deeds never named in Scripture are still judged by principle.
    /// </summary>
    public static class RealityDemo
    {
        public static void Run()
        {
            Console.WriteLine("=== Reality grounded in God: present a deed, reality resolves it ===");
            Console.WriteLine();

            Reality reality = Reality.Revealed();

            // The acts of God: the standard. Never incoherent, never disorder.
            Witness(reality, "The cross (Christ crucified)", new Act("the cross",
                new SelfSacrifice(), new Atonement(), new Pardon(), new CovenantFaithfulness()));

            Witness(reality, "God so loved the world (John 3:16)",
                new Act("the Father gives the Son", new SelfSacrifice(), new Compassion()));

            // Righteous human deeds.
            Witness(reality, "The Good Samaritan (Luke 10:37)",
                new Act("binding a stranger's wounds", new Compassion(), new Protection()));

            // Sin: incoherent, and it disorders the world.
            Witness(reality, "Cain murders Abel (Genesis 4)", new Act("the first murder", new Murder()));

            Witness(reality, "The unforgiving servant (Matthew 18)",
                new Act("mercy refused", new Unforgiveness(), new Condemnation()));

            // A deed never named in Scripture, judged by principle, not by lookup.
            Witness(reality, "Novel: talking a stranger off the ledge",
                new Act("a rescue online", new Compassion(), new Protection()));

            Console.WriteLine();
            Console.WriteLine("--- Grounding: the same act of worship, on God vs on an idol (1 Meqabyan) ---");
            Act worship = new Act("a sacrifice offered", new CovenantFaithfulness());
            Reality onGod = Reality.Revealed();
            Reality onIdol = Reality.Revealed();
            Resolution faithful = onGod.Witness(worship, Grounding.InGod());
            Resolution idolatrous = onIdol.Witness(worship, Grounding.InIdol("the golden calf"));
            Console.WriteLine("  on God        -> coherence " + Format(faithful.Coherence)
                + ", disorder " + Format(faithful.Disorder));
            Console.WriteLine("  on the calf   -> coherence " + Format(idolatrous.Coherence)
                + ", disorder " + Format(idolatrous.Disorder) + "  (drifts toward non-being)");
            Console.WriteLine();
        }

        private static void Witness(Reality reality, string title, Act act)
        {
            Resolution r = reality.Witness(act);
            Console.WriteLine(title);
            Console.WriteLine("  coherence " + Format(r.Coherence) + ", disorder " + Format(r.Disorder)
                + "  | Love " + Format(r.Reading("Love")) + ", Justice " + Format(r.Reading("Justice"))
                + ", Mercy " + Format(r.Reading("Mercy")) + ", Order " + Format(r.Reading("Order")));
            Console.WriteLine("  standing order of reality now: " + Format(reality.Tablets.Coherence()));
            Console.WriteLine();
        }

        private static string Format(double value)
        {
            return value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
