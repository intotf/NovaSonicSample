using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaSonicApp
{
    /// <summary>
    /// 亚马逊相关配置选项
    /// </summary>
    [Options]
    public class AmazonOptions
    {
        /// <summary>
        /// 秘钥Id
        /// </summary>
        public string AccessKeyId { get; set; } = string.Empty;

        /// <summary>
        /// 秘钥Key
        /// </summary>
        public string SecretAccessKey { get; set; } = string.Empty;

        /// <summary>
        /// 默认区域
        /// </summary>
        public virtual string RegionEndpoint { get; set; } = "us-east-1";

        /// <summary>
        /// 文本大模型Id
        /// </summary>
        public string ModelId { get; set; } = "us.amazon.nova-lite-v1:0";

        /// <summary>
        /// NovaSonic 模型Id
        /// </summary>
        public string NovaSonicModelId { get; set; } = "amazon.nova-sonic-v1:0";
    }
}
