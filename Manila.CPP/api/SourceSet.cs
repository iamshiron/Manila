

using Shiron.Manila.API;

public class SourceSet {
	public List<File> files { get; private set; } = new List<File>();

	public void include(File[] files) {
		this.files.AddRange(files);
	}
}
