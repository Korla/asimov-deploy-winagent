﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using AsimovDeploy.WinAgent.Web.Commands;
using NUnit.Framework;
using Shouldly;

namespace AsimovDeploy.WinAgent.IntegrationTests.Scenarios.PasswordScenario
{
    [TestFixture]
    public class PasswordScenario : WinAgentSystemTest
    {
        public override void Given()
        {
            GivenFoldersForScenario();
            GivenRunningAgent();
        }

        [Test]
        public void cannot_deploy_without_correct_password()
        {
            var parameterValues = new Dictionary<string, object>
            {
                ["Password"] = "wrong"
            };

            Agent.Post("/deploy/deploy", NodeFront.ApiKey, new DeployCommand
            {
                unitName = "PasswordTest",
                versionId = "somefile-v12.0.0.0-[bbb]-[123123].zip",
                parameters = parameterValues
            });

            Thread.Sleep(1000);

            File.Exists(Path.Combine(DataDir, "FileCopyTarget\\somefile.txt")).ShouldBe(false);

            parameterValues["Password"] = "the secret";

            Agent.Post("/deploy/deploy", NodeFront.ApiKey, new DeployCommand
            {
                unitName = "PasswordTest",
                versionId = "somefile-v12.0.0.0-[bbb]-[123123].zip",
                parameters = parameterValues
            });

            Thread.Sleep(1000);

            File.Exists(Path.Combine(DataDir, "FileCopyTarget\\somefile.txt")).ShouldBe(true);
        }

        [Test]
        public void can_deploy_with_password_input()
        {
            var parameterValues = new Dictionary<string, object>
            {
                ["Password"] = "Password!"
            };

            Agent.Post("/deploy/deploy", NodeFront.ApiKey, new DeployCommand
            {
                unitName = "PasswordTestWithInput",
                versionId = "somefile2-v12.0.0.0-[bbb]-[123123].zip",
                parameters = parameterValues
            });

            Thread.Sleep(1000);

            File.Exists(Path.Combine(DataDir, "FileCopyTarget\\somefile2.txt")).ShouldBe(true);
        }
    }
}