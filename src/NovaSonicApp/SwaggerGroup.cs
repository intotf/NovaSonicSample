using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace NovaSonicApp
{
    /// <summary>
    /// swagger分组
    /// </summary>
    enum SwaggerGroup
    {
        [Description("其它接口")]
        OtherApi,
    }


    /// <summary>
    /// Api分组特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SwaggerGroupAttribute<TGroupEnum> : ApiExplorerSettingsAttribute
        where TGroupEnum : struct, Enum
    {
        /// <summary>
        /// Api分组特性
        /// </summary>
        /// <param name="group"></param>
        public SwaggerGroupAttribute(TGroupEnum group)
        {
            this.GroupName = group.ToString();
        }
    }
}
