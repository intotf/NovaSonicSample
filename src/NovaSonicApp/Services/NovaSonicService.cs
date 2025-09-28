using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Text;

namespace NovaSonicApp.Services
{
    /// <summary>
    /// 亚马逊 Nova-Sonic 服务
    /// </summary>
    [Service(ServiceLifetime.Scoped)]
    public class NovaSonicService
    {
        private readonly AmazonBedrockRuntimeClient runtimeClient;
        private readonly IOptionsMonitor<AmazonOptions> options;
        private readonly ILogger<NovaSonicService> logger;

        /// <summary>
        /// 亚马逊 Nova-Sonic 服务
        /// </summary>
        public NovaSonicService(AmazonBedrockRuntimeClient runtimeClient, IOptionsMonitor<AmazonOptions> options, ILogger<NovaSonicService> logger)
        {
            this.runtimeClient = runtimeClient;
            this.options = options;
            this.logger = logger;
        }

        /// <summary>
        /// 亚马逊Sdk 自然语言大模型
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ChatResponse> ChatAsync(string message)
        {
            var chatClient = runtimeClient.AsIChatClient(options.CurrentValue.ModelId);
            var f = AIFunctionFactory.Create(GetWeather);

            return await chatClient.GetResponseAsync(message, new ChatOptions
            {
                //设置工具集
                //Tools = [f],
            });
        }

        /// <summary>
        /// 亚马逊Sdk 自然语言大模型
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SonicChatAsync(string message)
        {

            var mp3 = File.ReadAllBytes("whatai.mp3");

            //请求参数
            var request = new InvokeModelWithBidirectionalStreamRequest
            {
                ModelId = options.CurrentValue.NovaSonicModelId,
                BodyPublisher = new Func<Task<IInvokeModelWithBidirectionalStreamInputEvent>>(async () =>
                {
                    await Task.Delay(5000);
                    var model = new BidirectionalInputPayloadPart
                    {
                        Bytes = new MemoryStream(mp3)
                    };
                    logger.LogInformation("发送数据:" + Encoding.UTF8.GetString(model.Bytes.ToArray()));
                    return model;
                })
            };

            //请求NovaSonic 模型
            var result = await runtimeClient.InvokeModelWithBidirectionalStreamAsync(request);


            result.Body.ChunkReceived += (sender, e) =>
            {
                using var reader = new StreamReader(e.EventStreamEvent.Bytes);
                var content = reader.ReadToEnd();
                logger.LogInformation("接收块:" + content);
            };

            result.Body.InitialResponseReceived += (sender, e) =>
            {
                using var reader = new StreamReader(e.EventStreamEvent.Payload);
                var json = reader.ReadToEnd();
                logger.LogInformation("初始响应:" + json);
            };

            result.Body.ExceptionReceived += (sender, e) =>
            {
                logger.LogError("接收数据异常:" + e.EventStreamException.Message);
            };

            result.Body.EventReceived += (sender, e) =>
            {
                if (e.EventStreamEvent is BidirectionalOutputPayloadPart part)
                {
                    using var reader = new StreamReader(part.Bytes);
                    var content = reader.ReadToEnd();
                    logger.LogInformation("接收数据:" + content);
                }
            };
        }


        /// <summary>
        /// 天气查询工具
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        [Description("获取城市天气")]
        public Task<string> GetWeather([Description("城市")] string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                Console.WriteLine("城市为空");
            }
            var temperature = Random.Shared.NextInt64(25, 35);
            var result = Random.Shared.NextDouble() > 0.5 ? "晴天" : "阴天";
            return Task.FromResult($"{city} 当前温度 {temperature},{result}");
        }
    }
}
