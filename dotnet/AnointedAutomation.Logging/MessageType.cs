// Copyright © Anointed Automation Corporation., 2024. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-06-12 11:24:21 Jesus is King ✝️
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
// Edited 2026-11-12: appended Debug = 7 and Trace = 8.
//Stewarded by Alexander Fields

namespace AnointedAutomation.Optimization.Logging
{
    /// <summary>
    /// Defines the severity levels for log messages.
    /// </summary>
    public enum MessageType : int
    {
        /// <summary>
        /// Indicates an error condition that should be investigated.
        /// </summary>
        Error = 0,
        /// <summary>
        /// Indicates a potential issue that may require attention.
        /// </summary>
        Warning = 1,
        /// <summary>
        /// Indicates a successful operation or event.
        /// </summary>
        Success = 2,
        /// <summary>
        /// Provides general information about system operation.
        /// </summary>
        Informational = 3,
        /// <summary>
        /// A general message without specific severity level.
        /// </summary>
        Message = 4,
        /// <summary>
        /// Indicates a severe error that requires immediate attention.
        /// </summary>
        Critical = 5,
        /// <summary>
        /// Indicates a significant achievement or milestone.
        /// </summary>
        Celebrate = 6,
        /// <summary>
        /// Diagnostic detail useful while developing or investigating; hosts usually do not persist it in production.
        /// Appended in 2026-11: values are a wire format, never renumber.
        /// </summary>
        Debug = 7,
        /// <summary>
        /// The most verbose tracing detail; hosts usually drop it. Appended in 2026-11: never renumber.
        /// </summary>
        Trace = 8
    }
}
