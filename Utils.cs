
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CShargs
{
    internal static class ThrowIf
    {
        public static void ArgumentNull(string paramName, object param, string message = null)
        {
            if (param == null) {
                if (message != null) {
                    throw new ArgumentNullException(paramName, message);
                } else {
                    throw new ArgumentNullException(paramName);
                }
            }
        }

        internal static void InState(bool condition, string message = null)
        {
            if (condition) {
                throw new InvalidOperationException(message);
            }
        }

        internal static void ArgumentValue(bool condition, string message = null)
        {
            if (condition) {
                throw new ArgumentException(message);
            }
        }
    }

    internal static class OperatorsEx
    {
        public static bool In<T, TElem>(this T obj, ICollection<TElem> collection) where T : TElem
        {
            return collection.Contains(obj);
        }
    }

    internal static class ReflectionEx
    {
        public static Type GetOwnType(this MemberInfo member)
        {
            switch (member.MemberType) {
                case MemberTypes.Constructor: return member.DeclaringType;
                case MemberTypes.Field: return ((FieldInfo)member).FieldType;
                case MemberTypes.Method: return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property: return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event: return ((EventInfo)member).EventHandlerType;
                case MemberTypes.TypeInfo: return ((TypeInfo)member).AsType();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}