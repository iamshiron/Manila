
using System.Text.RegularExpressions;

namespace Shiron.Manila.Ext.Utils;

public static class PluginIdentifierParser {
	private static readonly Regex PATTERN = new(@"^(?:(?<group>[^:\n]+):)?(?<plugin>[^:@\n]+)(?:@(?<version>[^:\n]+))?:(?<component>[^:\n]+)$", RegexOptions.Compiled);

	public static bool isValid(string identifier) {
		return PATTERN.IsMatch(identifier);
	}

	public static (string group, string plugin, string version, string component) parse(string s) {
		var match = PATTERN.Match(s);
		if (!match.Success) throw new Exception($"Invalid plugin identifier: {s}");
		return (
			match.Groups["group"].Value == "" ? "shiron.manila" : match.Groups["group"].Value,
			match.Groups["plugin"].Value,
			match.Groups["version"].Value == "" ? "latest" : match.Groups["version"].Value,
			match.Groups["component"].Value
		);
	}
}
