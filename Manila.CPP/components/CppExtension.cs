using Shiron.Manila.Ext;

namespace Shiron.Manila.CPP.Components;

public abstract class CppExtension : ProjectExtension {
	[ScriptAttribute]
	public string cppStandard { get; set; } = "c++17";
}
