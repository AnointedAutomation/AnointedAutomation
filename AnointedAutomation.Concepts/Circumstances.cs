// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    // Well-known states of the world that love responds to, each a first-class circumstance rather
    // than a string. Many answer to the works of mercy (Matthew 25:35-36). A circumstance never
    // named here can still be expressed with `new Circumstance("...")`, so any situation is possible.

    /// <summary>Hunger: someone is hungry. "I was hungry and you gave me something to eat." (Matthew 25:35)</summary>
    public sealed class Hunger : Circumstance
    {
        public Hunger() : base("hungry")
        {
        }
    }

    /// <summary>Thirst: someone is thirsty. "I was thirsty and you gave me something to drink." (Matthew 25:35)</summary>
    public sealed class Thirst : Circumstance
    {
        public Thirst() : base("thirsty")
        {
        }
    }

    /// <summary>Estrangement: someone is a stranger or foreigner. "I was a stranger and you invited me in." (Matthew 25:35)</summary>
    public sealed class Estrangement : Circumstance
    {
        public Estrangement() : base("stranger")
        {
        }
    }

    /// <summary>Nakedness: someone needs clothing. "I needed clothes and you clothed me." (Matthew 25:36)</summary>
    public sealed class Nakedness : Circumstance
    {
        public Nakedness() : base("naked")
        {
        }
    }

    /// <summary>Sickness: someone is sick. "I was sick and you looked after me." (Matthew 25:36)</summary>
    public sealed class Sickness : Circumstance
    {
        public Sickness() : base("sick")
        {
        }
    }

    /// <summary>Imprisonment: someone is in prison. "I was in prison and you came to visit me." (Matthew 25:36)</summary>
    public sealed class Imprisonment : Circumstance
    {
        public Imprisonment() : base("imprisoned")
        {
        }
    }

    /// <summary>Need: someone is in need. "Who is my neighbor?" (Luke 10:29-37)</summary>
    public sealed class Need : Circumstance
    {
        public Need() : base("inNeed")
        {
        }
    }

    /// <summary>Means: I have it in my power to help. (Luke 10:33-35; Proverbs 3:27)</summary>
    public sealed class Means : Circumstance
    {
        public Means() : base("iCanHelp")
        {
        }
    }

    /// <summary>Grievance: I have been wronged. (Colossians 3:13)</summary>
    public sealed class Grievance : Circumstance
    {
        public Grievance() : base("wronged")
        {
        }
    }

    /// <summary>Enmity: this person is my enemy. "Love your enemies." (Matthew 5:44)</summary>
    public sealed class Enmity : Circumstance
    {
        public Enmity() : base("enemy")
        {
        }
    }

    /// <summary>Grief: someone is grieving. "Mourn with those who mourn." (Romans 12:15)</summary>
    public sealed class Grief : Circumstance
    {
        public Grief() : base("grieving")
        {
        }
    }

    /// <summary>Gladness: someone is rejoicing. "Rejoice with those who rejoice." (Romans 12:15)</summary>
    public sealed class Gladness : Circumstance
    {
        public Gladness() : base("rejoicing")
        {
        }
    }

    /// <summary>Mortal peril: a friend's life is at stake. "Greater love has no one than this." (John 15:13)</summary>
    public sealed class MortalPeril : Circumstance
    {
        public MortalPeril() : base("friendsLifeAtStake")
        {
        }
    }
}
