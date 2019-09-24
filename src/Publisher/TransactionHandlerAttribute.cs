using System;

namespace Publisher
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SupressTransactionAttribute : Attribute
    {
    }
}
