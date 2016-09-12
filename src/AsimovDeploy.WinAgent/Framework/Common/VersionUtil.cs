﻿/*******************************************************************************
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AsimovDeploy.WinAgent.Framework.Models;
using Newtonsoft.Json;

namespace AsimovDeploy.WinAgent.Framework.Common
{
    public static class VersionUtil
    {
        public const int MAX_LOG_ENTRIES = 250;
        public static string VERSION_LOG_FILENAME = "version-log.json";

        public static string GetAgentVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var infoAttribute = (AssemblyFileVersionAttribute)assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0];
            return infoAttribute.Version;
        }

        public static string ExtractVersionFromString(string str)
        {
            var match = Regex.Match(str, @"(v\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})");
            return match.Success ? match.Groups[0].Value : null;
        }

        public static void UpdateVersionLog(string inDirectory, DeployedVersion version)
        {
            var log = ReadVersionLog(inDirectory);
            log.Insert(0, version);

            UpdateLog(inDirectory, log);
        }

        private static void UpdateLog(string inDirectory, List<DeployedVersion> log)
        {
            //Node frontend chokes when the log gets to long so we only save the last 250 entries
            var deployedVersionsToSave = log.Take(MAX_LOG_ENTRIES).ToList();

            using (var writer = new StreamWriter(Path.Combine(inDirectory, VERSION_LOG_FILENAME), false, Encoding.UTF8))
            {
                var serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, deployedVersionsToSave);
            }
        }

        public static DeployedVersion GetCurrentVersion(string directory)
        {
            var log = ReadVersionLog(directory);
            return log.Count > 0 ? log[0] : new DeployedVersion() {VersionNumber = "0.0.0.0"};
        }

        public static List<DeployedVersion> ReadVersionLog(string inDirectory)
        {
            var versionFile = Path.Combine(inDirectory, VERSION_LOG_FILENAME);
            if (File.Exists(versionFile))
            {
                using (var reader = new StreamReader(versionFile, Encoding.UTF8))
                {
                    var serializer = new JsonSerializer();
                    return (List<DeployedVersion>)serializer.Deserialize(reader, typeof(List<DeployedVersion>));
                }
            }

            versionFile = Path.Combine(inDirectory, "version.txt");
            if (!File.Exists(versionFile))
            {
                return new List<DeployedVersion>();
            }

            using (var reader = new StreamReader(Path.Combine(inDirectory, "version.txt"), Encoding.UTF8))
            {
                return new List<DeployedVersion>() { new DeployedVersion() { VersionNumber = reader.ReadLine() } };
            }
        }
    }
}