// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// The converter moved to AnointedAutomation.Serialization.Newtonsoft.StringOrBoolConverter. This subclass keeps the old
// AnointedAutomation.Objects.Apple name (and its static Parse) resolving for code written against it; it adds nothing.

namespace AnointedAutomation.Objects.Apple
{
    /// <summary>
    /// Same as <see cref="AnointedAutomation.Serialization.Newtonsoft.StringOrBoolConverter"/>; prefer that type in new code.
    /// </summary>
    public class StringOrBoolConverter : AnointedAutomation.Serialization.Newtonsoft.StringOrBoolConverter
    {
    }
}
