# 简要说明
.net 8 NovaSonic 测试范例

# 下载 .net8 运行环境
https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0

# 运行程序
cmd 进入到 \src\NovaSonicApp,执行 dotnet run

# 访问程序api文档
http://localhost:5666/sonic/swagger/index.html

<img width="1443" height="904" alt="image" src="https://github.com/user-attachments/assets/a4dc2d1f-664a-442d-a164-525d58af0010" />

# 主要程序逻辑
\src\NovaSonicApp\Services\NovaSonicService.cs



# AmazonBedrockRuntimeClient 注册
 /// <summary>
 /// 注册 bedrock 服务
 /// </summary>
 /// <param name="services"></param>
 /// <returns></returns>
 public static void AddBedrockClient(this IServiceCollection services)
 {
     //注册 AmazonBedrockRuntimeClient
     services.AddSingleton(s =>
     {
         var option = s.GetRequiredService<IOptions<AmazonOptions>>().Value;
         return new AmazonBedrockRuntimeClient(option.AccessKeyId, option.SecretAccessKey, RegionEndpoint.GetBySystemName(option.RegionEndpoint));
     });
 }
