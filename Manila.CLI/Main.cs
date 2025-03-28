﻿using Shiron.Manila;
using Shiron.Manila.Attributes;
using Shiron.Manila.Exceptions;
using Shiron.Manila.Utils;

#if DEBUG
Directory.SetCurrentDirectory("E:/dev/Manila./run");
#endif

Logger.Init(false, false);
ApplicationLogger.Init(false, true);

var engine = ManilaEngine.GetInstance();
var extensionManager = ExtensionManager.GetInstance();

extensionManager.Init("./.manila/plugins");
extensionManager.LoadPlugins();
extensionManager.InitPlugins();

engine.Run();
extensionManager.ReleasePlugins();

if (engine.Workspace == null) throw new Exception("Workspace not found");
foreach (var arg in args) {
    if (arg.StartsWith(":")) {
        try {
            var task = engine.Workspace.GetTask(arg[1..]) ?? throw new Exception("Task not found: " + arg[1..]);
            var order = task.GetExecutionOrder();
            Logger.debug("Execution order: " + string.Join(", ", order));

            ApplicationLogger.BuildStarted();
            foreach (var t in order) {
                var taskToRun = engine.Workspace.GetTask(t) ?? throw new Exception("Task not found: " + t);
                ApplicationLogger.TaskStarted(taskToRun);

                try {
                    if (taskToRun.Action == null) Logger.warn("Task has no action: " + t);
                    else taskToRun.Action.Invoke();
                    ApplicationLogger.TaskFinished();
                } catch (Exception e) {
                    throw new TaskFailedException(taskToRun, e);
                }
            }

            ApplicationLogger.BuildFinished();
        } catch (Exception e) {
            ApplicationLogger.BuildFinished(e.InnerException);
        }
    }
}
