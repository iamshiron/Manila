using Shiron.Manila.Ext;

namespace Shiron.Manila.API;

[ScriptEnum]
public class EToolChain {
	public string name;

	public EToolChain(string name) {
		this.name = name;
	}

	public static readonly EToolChain gcc = new EToolChain("gcc");
	public static readonly EToolChain clang = new EToolChain("clang");
	public static readonly EToolChain msvc = new EToolChain("msvc");

	public static implicit operator string(EToolChain t) {
		return t.name;
	}

	public override bool Equals(object? obj) {
		return obj is EToolChain toolChain && name == toolChain.name;
	}
	public override string ToString() {
		return name;
	}
}

[ScriptEnum]
public class ELanguage {
	public string name { get; private set; }

	public ELanguage(string name) {
		this.name = name;
	}

	public static readonly ELanguage c = new ELanguage("c");
	public static readonly ELanguage cpp = new ELanguage("cpp");

	public static implicit operator string(ELanguage l) {
		return l.name;
	}

	public override bool Equals(object? obj) {
		return obj is ELanguage language && name == language.name;
	}
	public override string ToString() {
		return name;
	}
}

[ScriptEnum]
public class EPlatform {
	public readonly string name;

	public EPlatform(string name) {
		this.name = name;
	}

	public static readonly EPlatform windows = new EPlatform("windows");
	public static readonly EPlatform linux = new EPlatform("linux");

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

	public static readonly EArchitecture x86 = new EArchitecture("x86");
	public static readonly EArchitecture x64 = new EArchitecture("x64");

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
