
using Shiron.Manila.API;
using Shiron.Manila.Artifacts;
using Shiron.Manila.Ext;

namespace Shiron.Manila.JS;

public abstract class JavaScriptComponent(string name, Type buildConfigType) : LanguageComponent(name, buildConfigType) { }
