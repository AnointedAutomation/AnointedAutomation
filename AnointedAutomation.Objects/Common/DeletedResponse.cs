// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

namespace AnointedAutomation.Objects.Common
{
    /// <summary>
    /// Shared delete/mutation acknowledgement. Collapses the many single-field {Deleted} (and
    /// {Deleted, Id}) response DTOs scattered across delete/revoke endpoints into one type.
    /// <see cref="Id"/> is optional and left null when the endpoint does not echo the removed id.
    /// </summary>
    public class DeletedResponse
    {
        /// <summary>Whether the record was deleted.</summary>
        public bool Deleted { get; set; }

        /// <summary>The id of the deleted record, when the endpoint echoes it; otherwise null.</summary>
        public string Id { get; set; }

        /// <summary>A "deleted" acknowledgement, optionally carrying the removed id.</summary>
        public static DeletedResponse Of(bool deleted = true, string id = null) =>
            new DeletedResponse { Deleted = deleted, Id = id };
    }
}
