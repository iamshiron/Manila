﻿using Shiron.Manila;
using Shiron.Manila.Attributes;
using Shiron.Manila.Utils;

#if DEBUG
Directory.SetCurrentDirectory("E:/dev/Manila./run");
#endif

Logger.init(true, false);

var engine = ManilaEngine.getInstance();
var extensionManager = ExtensionManager.getInstance();

extensionManager.init("./.manila/plugins");
extensionManager.loadPlugins();
extensionManager.initPlugins();

engine.run();
extensionManager.releasePlugins();
