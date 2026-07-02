// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 10:42:10
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 10:42:10
//Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// The deed a <see cref="Love"/> performs in response to a <see cref="Scenario"/> — what love
    /// does, the virtue that drives it, and the Scripture it answers to.
    /// </summary>
    public class LoveAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoveAction"/> class.
        /// </summary>
        public LoveAction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoveAction"/> class.
        /// </summary>
        /// <param name="acts">Whether love intervenes in the situation.</param>
        /// <param name="deed">What love does.</param>
        /// <param name="virtue">The Biblical virtue (or its absence) that drives the deed.</param>
        /// <param name="reference">The Scripture the deed answers to.</param>
        /// <param name="exhortation">The call the Scripture leaves us with.</param>
        public LoveAction(bool acts, string deed, string virtue, string reference, string exhortation)
        {
            this.acts = acts;
            Deed = deed;
            Virtue = virtue;
            Reference = reference;
            Exhortation = exhortation;
        }

        /// <summary>
        /// Gets or sets whether love intervenes in the situation.
        /// </summary>
        public bool acts
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets what love does.
        /// </summary>
        public string Deed
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the call the Scripture leaves us with (for example, "Go and do likewise.").
        /// </summary>
        public string Exhortation
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Scripture the deed answers to.
        /// </summary>
        public string Reference
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Biblical virtue (or, for loveless responses, the vice) that drives the
        /// deed.
        /// </summary>
        public string Virtue
        {
            get; set;
        }
    }
}
