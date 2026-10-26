// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Derived from ShopifySharp (https://github.com/nozzlegear/ShopifySharp), Copyright (c) 2015 Joshua Harms, MIT License.
// Adapted by Anointed Automation Corporation. See THIRD-PARTY-NOTICES.md.
#nullable enable
namespace AnointedAutomation.Shopify;

///<summary>
///The billing plan of the shop.
///</summary>
public class ShopPlan
{
    ///<summary>
    ///The name of the shop's billing plan.
    ///</summary>
    public string? DisplayName { get; set; }
    ///<summary>
    ///Whether the shop is a partner development shop for testing purposes.
    ///</summary>
    public bool? PartnerDevelopment { get; set; }
    ///<summary>
    ///Whether the shop has a Shopify Plus subscription.
    ///</summary>
    public bool? ShopifyPlus { get; set; }
}
