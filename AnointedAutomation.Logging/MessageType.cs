// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-06-12 11:24:21
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
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
        Celebrate = 6
    }
}
