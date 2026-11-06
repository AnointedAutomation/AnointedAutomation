// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Shared [Obsolete] texts so every overload names the same replacements.
    /// </summary>
    public static class MongoObsoleteMessages
    {
        /// <summary>
        /// The message on every whole-collection read (GetAllDocumentsAsync and MongoRepository.GetAllAsync).
        /// </summary>
        public const string WholeCollectionRead = "Loads a whole collection into RAM (no RAM-resident data); use GetPageAsync or GetDocumentIdPageAsync (page + totals), GetDocumentIdsPageAsync, GetPagedAsync, or GetFilteredDocumentsAsync with a filter and a limit instead.";
    }
}
