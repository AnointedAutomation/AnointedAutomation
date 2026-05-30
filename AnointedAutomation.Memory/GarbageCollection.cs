// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-15 18:02:13
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
﻿using System;

// Stewarded by Alexander Fields

namespace AnointedAutomation.Optimization.Memory
{
    /// <summary>
    /// Provides functionality for managing garbage collection and memory optimization.
    /// </summary>
    public class GarbageCollection
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GarbageCollection"/> class.
        /// </summary>
        public GarbageCollection()
        {
        }

        /// <summary>
        /// Finalizer for the GarbageCollection class.
        /// Suppresses finalization to prevent redundant cleanup.
        /// </summary>
        ~GarbageCollection()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs garbage collection and attempts to optimize memory usage.
        /// </summary>
        /// <param name="state">An optional state object (unused).</param>
        public void PerformGarbageCollection(object state)
        {
            // Perform garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Attempt to return memory to the OS
            ReturnUnusedMemoryToOS();
            //Log the finish?
        }

        /// <summary>
        /// Attempts to return unused memory to the operating system by utilizing no GC regions.
        /// </summary>
        private void ReturnUnusedMemoryToOS()
        {
            // Attempt to return unused memory to the OS
            // This can fail if memory is still in use
            long memorySize = GC.GetTotalMemory(false);

            // Check if it's feasible to enter a no GC region
            if (memorySize > 0)
            {
                try
                {
                    // Attempt to allocate a no GC region, if successful return memory
                    if (GC.TryStartNoGCRegion(memorySize))
                    {
                        // End the no GC region and force the GC to return memory
                        GC.EndNoGCRegion();
                    }
                }
                catch (InvalidOperationException)
                {
                    //Log the failure
                }
            }
        }
    }
}
