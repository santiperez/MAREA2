using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Marea
{
    /// <summary>
    /// This classs includes Compare-And-Swap (CAS) function used to provide  thread-safe operations. CAS only allows us to change one item at a time.
    /// </summary>
    public static class SyncMethods
    {
        /// <summary>
        /// Allows any thread enqueuing a node to advance the tail reference.
        /// </summary>
        public static bool CAS<T>(ref T location, T comparand, T newValue) where T : class
        {
            return (object)comparand == (object)Interlocked.CompareExchange<T>(ref location, newValue, comparand);
        }
    }
}
