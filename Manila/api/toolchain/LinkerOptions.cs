
namespace Shiron.Manila.API.Toolchain;

public class LinkerOptions {
	public List<string> files = new();
	public string outPath;
	public List<string> libs = new();
	public List<string> libPaths = new();
}
