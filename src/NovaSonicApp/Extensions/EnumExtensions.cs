using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static class EnumExtensions
    {
        private static readonly ConcurrentDictionary<EnumKey, FieldInfo?> fieldCache = new();

        /// <summary>
        /// 获取枚举字段的DisplayAttribute的Name值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetFieldDisplayName(this Enum value)
        {
            var attribute = value.GetFieldAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        /// <summary>
        /// 获取枚举字段的DisplayAttribute的Description值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetFieldDisplayDescription(this Enum value)
        {
            var attribute = value.GetFieldAttribute<DisplayAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        /// 获取枚举字段的DescriptionAttribute的Description值
        /// </summary>
        /// <param name="value">枚举字段的值</param>
        /// <returns></returns>
        public static string GetFieldDescription(this Enum value)
        {
            var attribute = value.GetFieldAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// 获取枚举字段的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="value">枚举字段的值</param>
        /// <returns></returns>
        public static TAttribute? GetFieldAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var field = value.GetFieldInfo();
            return field?.GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        /// 获取枚举字段的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="value">枚举字段的值</param>
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetFieldAttributes<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var field = value.GetFieldInfo();
            return field == null
                ? Array.Empty<TAttribute>()
                : field.GetCustomAttributes<TAttribute>();
        }

        /// <summary>
        /// 获取枚举的字段信息
        /// </summary>
        /// <param name="value">枚举字段的值</param>
        /// <returns></returns>
        public static FieldInfo? GetFieldInfo(this Enum value)
        {
            return fieldCache.GetOrAdd(new EnumKey(value), key => key.GetFieldInfo());
        }

        private struct EnumKey(Enum value) : IEquatable<EnumKey>
        {
            private int? hashCode;
            private readonly Type type = value.GetType();
            private readonly Enum value = value;

            public readonly FieldInfo? GetFieldInfo()
            {
                var name = Enum.GetName(this.type, this.value);
                return name == null ? null : this.type.GetField(name);
            }

            public readonly bool Equals(EnumKey other)
            {
                return this.type == other.type && this.value.GetHashCode() == other.value.GetHashCode();
            }

            public override readonly bool Equals([NotNullWhen(true)] object? obj)
            {
                return obj is EnumKey other && this.Equals(other);
            }

            public override int GetHashCode()
            {
                this.hashCode ??= HashCode.Combine(this.type, this.value);
                return this.hashCode.Value;
            }
        }
    }
}
