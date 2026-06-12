// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// The medium through which an agent and reality meet. "In the beginning was the Word ... all
    /// things were made through him." (John 1:1-3); "I am the way and the truth and the life."
    /// (John 14:6). This is the mediator the design names like Jesus, but modeled as a medium, not a
    /// locked gate: the Word carries an agent's deed into reality and carries reality's truth back.
    /// It does not bypass reality, and it does not stand between the agent and a sealed door; it is
    /// how the meeting happens at all.
    /// </summary>
    public class Word
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Word"/> class as the medium of a given
        /// reality.
        /// </summary>
        /// <param name="reality">The reality this Word mediates.</param>
        public Word(Reality reality)
        {
            if (reality == null)
            {
                throw new System.ArgumentNullException(nameof(reality));
            }

            this.reality = reality;
        }

        private readonly Reality reality;

        /// <summary>
        /// Carries an agent's deed, done on a given foundation, into reality, and returns reality's
        /// truth of it. The deed is witnessed and written onto the tablets through this medium.
        /// </summary>
        /// <param name="act">The deed the agent presents.</param>
        /// <param name="grounding">What the agent is grounded in.</param>
        /// <returns>The resolution reality gives back.</returns>
        public Resolution Speak(Act act, Grounding grounding)
        {
            if (act == null)
            {
                throw new System.ArgumentNullException(nameof(act));
            }

            if (grounding == null)
            {
                throw new System.ArgumentNullException(nameof(grounding));
            }

            return reality.Witness(act, grounding);
        }
    }
}
