using Shiron.Manila.Attributes;

namespace Shiron.Manila.API;

[ScriptEnum]
public class EPlatform {
	public readonly string name;

	public EPlatform(string name) {
		this.name = name;
	}

	public static readonly EPlatform Windows = new EPlatform("windows");
	public static readonly EPlatform Linux = new EPlatform("linux");

	public static implicit operator string(EPlatform p) {
		return p.name;
	}

	public override bool Equals(object? obj) {
		return obj is EPlatform platform && name == platform.name;
	}
	public override string ToString() {
		return name;
	}
}

[ScriptEnum]
public class EArchitecture {
	public readonly string name;

	public EArchitecture(string name) {
		this.name = name;
	}

	public static readonly EArchitecture X86 = new EArchitecture("x86");
	public static readonly EArchitecture X64 = new EArchitecture("x64");

	public static implicit operator string(EArchitecture a) {
		return a.name;
	}

	public override bool Equals(object? obj) {
		return obj is EArchitecture architecture && name == architecture.name;
	}
	public override string ToString() {
		return name;
	}
}
