using Amazon;
using Amazon.BedrockRuntime;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NovaSonicApp;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ServiceCollection扩展
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册 bedrock 服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddBedrockClient(this IServiceCollection services)
        {
            //注册OpenAi 抽象的 AmazonBedrockRuntimeClient
            services.AddSingleton(s =>
            {
                var option = s.GetRequiredService<IOptions<AmazonOptions>>().Value;
                return new AmazonBedrockRuntimeClient(option.AccessKeyId, option.SecretAccessKey, RegionEndpoint.GetBySystemName(option.RegionEndpoint));
            });
        }


        /// <summary>
        /// 注册swagger服务
        /// </summary>
        /// <typeparam name="TGroupEnum">分组枚举</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger<TGroupEnum>(this IServiceCollection services)
             where TGroupEnum : struct, Enum
        {
            return services.AddSwaggerGen(c =>
            {
                c.AddSwaggerBearerAuth();
                c.UseNonNullableRequired();

                c.SchemaGeneratorOptions.UseAllOfForInheritance = true;
                c.SchemaGeneratorOptions.UseAllOfToExtendReferenceSchemas = false;

                c.MapType<JsonNode>(() => new OpenApiSchema
                {
                    Type = "object",
                    Required = new HashSet<string>(),
                });

                c.MapType<TimeSpan>(() => new OpenApiSchema
                {
                    Type = "object",
                    Required = new HashSet<string>(),
                });

                foreach (var xml in Directory.GetFiles(AppContext.BaseDirectory, $"{nameof(NovaSonicApp)}*.xml"))
                {
                    c.IncludeXmlComments(xml);
                }

                foreach (var group in Enum.GetValues<TGroupEnum>())
                {
                    var documentName = group.ToString();
                    var title = group.GetFieldDescription();
                    c.SwaggerDoc(documentName, new OpenApiInfo { Title = title, Version = "v1" });
                }
            });
        }


        /// <summary>
        /// 添加swagger的Bearer token
        /// </summary>
        /// <param name="swaggerGenOptions"></param>  
        /// <param name="description">说明</param>
        /// <returns></returns>
        private static void AddSwaggerBearerAuth(this SwaggerGenOptions swaggerGenOptions, string? description = null)
        {
            swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = description ?? "请在下面输入token值",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "bearer",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT"
            });
            swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new List<string>()
                }
            });
        }

        /// <summary>
        /// 添加swagger的Required标记
        /// </summary>
        /// <param name="swaggerGenOptions"></param>   
        /// <returns></returns>
        private static void UseNonNullableRequired(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.SupportNonNullableReferenceTypes();
            swaggerGenOptions.UseAllOfToExtendReferenceSchemas();
            swaggerGenOptions.UseAllOfForInheritance();
            swaggerGenOptions.SchemaFilter<SwaggerRequiredFilter>();
            swaggerGenOptions.ParameterFilter<SwaggerRequiredFilter>();
            swaggerGenOptions.RequestBodyFilter<SwaggerRequiredFilter>();
        }

        /// <summary>
        /// 表示swagger的自动Required标记过滤器
        /// </summary>
        private class SwaggerRequiredFilter : ISchemaFilter, IParameterFilter, IRequestBodyFilter
        {
            /// <summary>
            /// 应用模型过滤器
            /// </summary>
            /// <param name="schema"></param>
            /// <param name="context"></param>
            public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {
                if (schema.Properties != null)
                {
                    foreach (var kv in schema.Properties)
                    {
                        if (kv.Value.Nullable == false)
                        {
                            schema.Required.Add(kv.Key);
                        }
                    }
                }
            }

            /// <summary>
            /// 应用参数过滤器
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="context"></param>
            public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
            {
                if (context.ApiParameterDescription.DefaultValue == null)
                {
                    parameter.Required = context.ApiParameterDescription.ModelMetadata.IsRequired;
                }
            }

            /// <summary>
            /// 应用Form表单过滤器
            /// </summary>
            /// <param name="requestBody"></param>
            /// <param name="context"></param> 
            public void Apply(OpenApiRequestBody requestBody, RequestBodyFilterContext context)
            {
                if (context.FormParameterDescriptions != null)
                {
                    foreach (var kv in requestBody.Content)
                    {
                        foreach (var item in context.FormParameterDescriptions)
                        {
                            if (item.ModelMetadata.IsRequired)
                            {
                                kv.Value.Schema.Required.Add(item.Name);
                            }
                        }
                    }
                }
            }
        }



    }
}
