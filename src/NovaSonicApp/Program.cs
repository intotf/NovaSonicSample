namespace NovaSonicApp
{
    internal class Program
    {
        /// <summary>
        /// 入口方法，应用程序的主入口点
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //注册控制器
            builder.Services.AddControllers();

            //注册内存缓存
            builder.Services.AddMemoryCache();

            //注册 Swagger 支持
            builder.Services.AddSwagger<SwaggerGroup>();

            //路由忽略大小写
            builder.Services.AddRouting(x => x.LowercaseUrls = true);

            //注册Serilog日志服务
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog(writeToProviders: true);

            //注册 AmazonBedrockRuntimeClient
            builder.Services.AddBedrockClient();
            builder.Services.AddNovaSonicApp();
            builder.Services.AddNovaSonicAppOptions(builder.Configuration);

            var app = builder.Build();

            //配置中间件 Swagger
            app.UseSwagger<SwaggerGroup>("sonic");

            //配置静成文件中间件
            app.UseDefaultFiles();
            app.UseStaticFiles();

            //配置路由中间件
            app.UseRouting();

            //配置控制器中间件
            app.MapControllers();
            app.Run();
        }
    }
}
