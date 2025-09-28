using Microsoft.OpenApi.Models;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// swagger扩展
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 使用swagger
        /// </summary>
        /// <typeparam name="TGroupEnum">分组枚举</typeparam>
        /// <param name="app"></param>
        /// <param name="appName">应用的名称</param>
        public static void UseSwagger<TGroupEnum>(this IApplicationBuilder app, string appName = "sonic")
            where TGroupEnum : struct, Enum
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = $"/{appName}/swagger/{{documentName}}/swagger.json";
                c.PreSerializeFilters.Add((swagger, request) =>
                {
                    swagger.Servers = Array.Empty<OpenApiServer>();
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = $"{appName}/swagger";
                foreach (var group in Enum.GetValues<TGroupEnum>())
                {
                    var documentName = group.ToString();
                    c.SwaggerEndpoint($"/{appName}/swagger/{documentName}/swagger.json", documentName);
                }
            });
        }


    }
}
