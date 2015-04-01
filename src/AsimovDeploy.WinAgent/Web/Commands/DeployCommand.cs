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

using System.Collections.Generic;

namespace AsimovDeploy.WinAgent.Web.Commands
{
    public class DeployCommand : AsimovCommand
    {
        public string correlationId { get; set; }
        public string unitName { get; set; }
        public string versionId { get; set; }
		public string userId { get; set; }
		public string userName { get; set; }

        public Dictionary<string, object> parameters { get; set; }
    }

    public class AsimovCommand
    {

    }
}
