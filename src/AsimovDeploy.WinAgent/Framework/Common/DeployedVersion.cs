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
using System.Collections.Generic;
using AsimovDeploy.WinAgent.Framework.Models;

namespace AsimovDeploy.WinAgent.Framework.Common
{
    public class DeployedVersion
    {
        public string VersionNumber;
        public string VersionId;
        public DateTime VersionTimestamp;
        public string VersionBranch;
        public string VersionCommit;
        public DateTime DeployTimestamp;
        public string LogFileName;
        public bool DeployFailed;
		public string UserId { get; set; }
		public string UserName { get; set; }
        public IDictionary<string, dynamic> Parameters { get; set; }
        public string CorrelationId { get; set; }
    }
}