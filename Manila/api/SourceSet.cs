
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Shiron.Manila;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public class SourceSet {
	public List<string> fileGlobs { get; private set; } = new List<string>();
	public List<string> excludeGlobs { get; private set; } = new List<string>();

	private readonly string root;

	public SourceSet() : this(Shiron.Manila.Manila.getInstance().root) { }
	public SourceSet(string root) {
		this.root = root;
	}

	public SourceSet include(string d) {
		Logger.debug("Including " + d);
		fileGlobs.Add(toRelativePath(d));
		return this;
	}
	public SourceSet exclude(string d) {
		Logger.debug("Excluding " + d);
		excludeGlobs.Add(toRelativePath(d));
		return this;
	}

	public File[] files() {
		Logger.debug("Discovering files...");

		Matcher matcher = new();
		matcher.AddIncludePatterns(fileGlobs);
		matcher.AddExcludePatterns(excludeGlobs);

		DirectoryInfoWrapper root = new DirectoryInfoWrapper(new DirectoryInfo(this.root));
		PatternMatchingResult result = matcher.Execute(root);


		return result.Files.Select(f => new File(f.Path)).ToArray();
	}

	private string toRelativePath(string path) {
		return Path.GetRelativePath(root, path);
	}
}
