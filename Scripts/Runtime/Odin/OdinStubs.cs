#if !ODIN_INSPECTOR
// This is a collection of stubs or quick hacks to match the
// odin apis that we are using.

#if !ODIN_STUBS
#define ODIN_STUBS

using System;
using System.Collections.Generic;

namespace Sirenix.OdinInspector
{
    public class Button : Attribute
    {
        
    }

    public class InlineEditor : Attribute
    {
        
    }

    public class HideIf : Attribute
    {
        public HideIf(string condition) {}
    }
}

namespace Sirenix.Utilities
{
    public static class HashSetUtilities
    {
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                set.Add(value);
            }
        }
    }
}

#endif
#endif