// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me on 2026-05-28 11:31:07
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Created by Alexander Fields

using System;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Demo
{
    /// <summary>
    /// Demonstrates the abstract Love concept as a decision engine: feed a situation in (a set of
    /// facts), and each love walks its behavior tree to produce the fitting deed — including for
    /// situations the Bible never named.
    /// </summary>
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== Love as a decision engine: feed a situation in, get a deed out ===");
            Console.WriteLine();

            // A neighbor in need whom you have the means to help (the Good Samaritan principle).
            RunAllLoves(new Situation("A stranger lies beaten by the road; I have the means to help.")
                .Set("inNeed")
                .Set("iCanHelp"));

            // A situation never named in Scripture: an online teammate betrays the group.
            RunAllLoves(new Situation("A teammate betrayed the group and ninja-looted the boss drop.")
                .Set("wronged"));

            // A work of mercy (Matthew 25:35): someone hungry before you.
            RunAllLoves(new Situation("A hungry person is at your door.")
                .Set("hungry"));

            // A composed condition (Romans 12:20): enemy AND (hungry OR thirsty).
            RunAllLoves(new Situation("An enemy who hates you is now hungry at your door.")
                .Set("enemy")
                .Set("hungry"));

            // A friend's life on the line — where sacrificial love goes further than the rest.
            RunAllLoves(new Situation("A friend's life is at stake; saving them would cost my own.")
                .Set("friendsLifeAtStake"));

            // A situation the engine has no specific branch for at all.
            RunAllLoves(new Situation("Something the Bible never described.")
                .Set("quantumComputerMalfunctioned"));
        }

        private static void RunAllLoves(Situation situation)
        {
            Console.WriteLine("SITUATION: " + situation.Description);
            Decide("Agape          ", new Agape(), situation);
            Decide("SelfSeekingLove", new SelfSeekingLove(), situation);
            Decide("SacrificialLove", new SacrificialLove(), situation);
            Console.WriteLine();
        }

        private static void Decide(string loveName, Love love, Situation situation)
        {
            LoveAction action = love.Decide(situation);
            Console.WriteLine("  " + loveName + " -> " + action.Deed + "  [" + action.Reference + "]");
        }
    }
}
