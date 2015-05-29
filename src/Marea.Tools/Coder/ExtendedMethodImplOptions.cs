using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MareaGen
{
    /// <summary>
    /// This enum is used in order to provide compatibility between .NET 4.5 and 4 Frameworks 
    /// in a smarter way instead of using multiple ifdef directives in the code. The option 
    /// AggressiveInlining does not exist in .NET 4.
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [Flags]
    public enum ExtendedMethodImplOptions
    {
        // Summary:
        //     The method is implemented in unmanaged code.
        Unmanaged = 4,
        //
        // Summary:
        //     The method cannot be inlined. Inlining is an optimization by which a method
        //     call is replaced with the method body.
        NoInlining = 8,
        //
        // Summary:
        //     The method is declared, but its implementation is provided elsewhere.
        ForwardRef = 16,
        //
        // Summary:
        //     The method can be executed by only one thread at a time. Static methods lock
        //     on the type, whereas instance methods lock on the instance. Only one thread
        //     can execute in any of the instance functions, and only one thread can execute
        //     in any of a class's static functions.
        Synchronized = 32,
        //
        // Summary:
        //     The method is not optimized by the just-in-time (JIT) compiler or by native
        //     code generation (see Ngen.exe) when debugging possible code generation problems.
        NoOptimization = 64,
        //
        // Summary:
        //     The method signature is exported exactly as declared.
        PreserveSig = 128,
        //
        // Summary:
        //     The method should be inlined if possible.
#if NET45
        [ComVisible(false)]
        AggressiveInlining = 256,
#else
        [ComVisible(true)]
        AggressiveInlining = 8,
#endif
        //
        // Summary:
        //     The call is internal, that is, it calls a method that is implemented within
        //     the common language runtime.
        InternalCall = 4096,

    }
}
