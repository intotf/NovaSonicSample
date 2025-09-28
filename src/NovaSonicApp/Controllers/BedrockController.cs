using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using NovaSonicApp.Services;

namespace NovaSonicApp.Controllers
{
    /// <summary>
    /// 亚马逊 Bedrock 控制器
    /// </summary>
    public class BedrockController : ApiController
    {
        private readonly NovaSonicService service;
        private readonly ILogger<BedrockController> logger;

        /// <summary>
        /// 亚马逊 Bedrock 控制器
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public BedrockController(NovaSonicService service, ILogger<BedrockController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        /// <summary>
        /// 亚马逊Sdk 自然语言大模型-在线天聊
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost("chat")]
        public async Task<ChatResponse> ChatAsync(string message)
        {
            return await service.ChatAsync(message);
        }


        /// <summary>
        /// 亚马逊Sdk InvokeModelWithBidirectionalStreamResponse请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost("sonic")]
        public async Task SonicChatAsync(string message)
        {
            await service.SonicChatAsync(message);
        }
    }
}
