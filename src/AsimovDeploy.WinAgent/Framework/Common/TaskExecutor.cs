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
using System.Collections.Concurrent;
using System.Threading.Tasks;
using log4net;

namespace AsimovDeploy.WinAgent.Framework.Common
{
    public class TaskExecutor : ITaskExecutor, IStartable
    {
        private static ILog _log = LogManager.GetLogger(typeof (TaskExecutor));

        private BlockingCollection<AsimovTask> _tasks = new BlockingCollection<AsimovTask>(100);

        private Task _workerTask;

        public Task<T> AddTask<T>(T task) where T : AsimovTask
        {
            var tsc = new TaskCompletionSource<T>();

            task.Completed += (ex) =>
            {
                if (ex != null)
                    tsc.SetException(ex);
                else
                    tsc.SetResult(task);
            };

            _tasks.Add(task);

            return tsc.Task;
        }

        public void Start()
        {
            _workerTask = Task.Factory.StartNew(() =>
            {
                _log.Debug("TaskExecutor started");

                TaskExecutorLoop();

                _log.Debug("TaskExecutor stopped");

            }, TaskCreationOptions.LongRunning);
        }

        private void TaskExecutorLoop()
        {
            while (!_tasks.IsCompleted)
            {
                AsimovTask task = null;

                try
                {
                    task = _tasks.Take();
                }
                catch (InvalidOperationException) { }

                ExecuteTask(task);

                var remainingTasks = _tasks.Count;
                _log.Info(remainingTasks > 0 ? $"Queued tasks remaining: {remainingTasks}" : "All queued tasks completed");
            }
        }

        private static void ExecuteTask(AsimovTask task)
        {
            if (task == null) return;

            try
            {
                task.ExecuteTask();
            }
            catch(Exception e)
            {
                _log.Error("Error executing task.", e);
            }
        }

        public void Stop()
        {
            _tasks.CompleteAdding();
            _workerTask.Wait(TimeSpan.FromMinutes(5));
        }
    }
}