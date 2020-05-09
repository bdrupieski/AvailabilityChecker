using System;
using System.Collections.Generic;

namespace AvailabilityChecker.Extensions
{
    public static class StringExtensions
    {
        public static string StringJoin(this IEnumerable<string> stringEnumerable, string delimiterString)
        {
            return String.Join(delimiterString, stringEnumerable);
        }
    }
}