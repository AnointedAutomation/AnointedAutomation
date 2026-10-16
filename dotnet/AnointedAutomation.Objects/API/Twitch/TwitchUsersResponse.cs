// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Twitch
{
    /// <summary>
    /// The Helix <c>GET https://api.twitch.tv/helix/users</c> envelope: <c>{ "data": [ user ] }</c>
    /// (<c>https://dev.twitch.tv/docs/api/reference/#get-users</c>). Transport only (no BSON mapping); the user itself
    /// is stored at <c>User.SSO.Twitch</c> through <c>TwitchLogin</c>.
    /// </summary>
    public class TwitchUsersResponse
    {
        /// <summary>The users the request matched (the signed-in user for a token-only call).</summary>
        [JsonProperty("data")]
        public List<TwitchUser>? Data { get; set; }
    }
}
