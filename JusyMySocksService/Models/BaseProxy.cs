﻿using YamlDotNet.Serialization;

namespace JustMySocksService.Models
{
    public class BaseProxy
    {
        [YamlMember(Order = 1, ScalarStyle = YamlDotNet.Core.ScalarStyle.DoubleQuoted)]
        public string name;
        [YamlMember(Order = 2)]
        public readonly string type;
        [YamlMember(Order = 3)]
        public string server;
        [YamlMember(Order = 4, ScalarStyle = YamlDotNet.Core.ScalarStyle.Plain)]
        public string port;

        public BaseProxy(string type)
        {
            this.type = type;
        }

        public BaseProxy(string type, string name)
        {
            this.type = type;
            this.name = name;
        }
    }
}
