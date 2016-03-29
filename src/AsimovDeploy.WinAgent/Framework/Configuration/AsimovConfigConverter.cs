/*******************************************************************************
* Copyright (C) 2012 eBay Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
******************************************************************************/

using System;
using System.IO;
using System.Text.RegularExpressions;
using AsimovDeploy.WinAgent.Framework.Models;
using AsimovDeploy.WinAgent.Framework.Models.Units;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;

namespace AsimovDeploy.WinAgent.Framework.Configuration
{
    public class AsimovConfigConverter : JsonConverter
    {
        private static ILog Log = LogManager.GetLogger(typeof(AsimovConfigConverter));

        private readonly string _configDir;
        private readonly string _machineName;

        public AsimovConfigConverter(string machineName, string configDir)
        {
            _configDir = configDir;
            _machineName = machineName.ToLower();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            var config = new AsimovConfig();
            serializer.Populate(json.CreateReader(), config);

            var self = GetSelf(json);
            if (self != null)
                serializer.Populate(self.CreateReader(), config);
            else
                Log.ErrorFormat("Could not find agent specific config / environment for: {0}", _machineName);

            var environments = config.Environment.Split(',');

            foreach (var environment in environments)
            {
                var envConfigFile = Path.Combine(_configDir, $"config.{environment.Trim()}.json");

                if (!File.Exists(envConfigFile))
                    continue;

                Log.DebugFormat("Loading config file {0}", envConfigFile);
				PopulateFromFile(envConfigFile, serializer, config);

				var env = new DeployEnvironment();
				PopulateFromFile(envConfigFile, serializer, env);
				config.Environments.Add(env);
			}

            return config;
        }

	    private void PopulateFromFile(string filename, JsonSerializer serializer, object target)
	    {
			using (var envReader = new StreamReader(filename))
			{
				using (var envJsonReader = new JsonTextReader(envReader))
				{
					serializer.Populate(envJsonReader, target);
				}
			}
		}

		private JToken GetSelf(JObject json)
        {
            var agents = json["Agents"];
            if (agents == null)
                return null;

            if (json["Agents"][_machineName] != null)
            {
                return json["Agents"][_machineName];
            }

            foreach (JProperty agent in json["Agents"].AsJEnumerable())
            {
                if (agent.Name.Contains("*"))
                {
                    var regex = new Regex("^" + agent.Name.Replace("*", ".*"));
                    if (regex.IsMatch(_machineName))
                    {
                        return agent.Value;
                    }
                }

                if (agent.Name.Contains("[") && agent.Name.Contains("]"))
                {
                    var regex = new Regex("^" + agent.Name);
                    if (regex.IsMatch(_machineName))
                    {
                        return agent.Value;
                    }
                }
            }

            return null;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(AsimovConfig);
    }
}