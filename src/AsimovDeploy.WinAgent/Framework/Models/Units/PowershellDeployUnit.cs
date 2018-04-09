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

using AsimovDeploy.WinAgent.Framework.Common;
using AsimovDeploy.WinAgent.Framework.Configuration;
using AsimovDeploy.WinAgent.Framework.Deployment.Steps;
using AsimovDeploy.WinAgent.Framework.Tasks;

namespace AsimovDeploy.WinAgent.Framework.Models.Units
{
    public class PowerShellDeployUnit : DeployUnit
    {
        public string Script { get; set; }
        public string Url { get; set; }

        public override string UnitType => DeployUnitTypes.PowerShell;

        public override AsimovTask GetDeployTask(AsimovVersion version, ParameterValues parameterValues, AsimovUser user, string correlationId)
        {
            var task = new DeployTask(this, version, parameterValues, user, correlationId);
            task.AddDeployStep<PowerShellDeployStep>();
            return task;
        }

        public override void SetupDeployActions() { }

        public override DeployUnitInfo GetUnitInfo(bool refreshUnitStatus)
        {
            var deployUnitInfo = base.GetUnitInfo(refreshUnitStatus);

            deployUnitInfo.Status = UnitStatus.NA;
            deployUnitInfo.Url = Url?.Replace("localhost", HostNameUtil.GetFullHostName());

            return deployUnitInfo;
        }
    }
}