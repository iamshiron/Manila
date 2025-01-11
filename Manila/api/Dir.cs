
public class Dir {
	public string path { get; private set; }

	public Dir(string path) {
		this.path = path;
	}

	public File[] files() {
		string[] files = Directory.GetFiles(this.path);
		File[] result = new File[files.Length];
		for (int i = 0; i < files.Length; i++) {
			result[i] = new File(files[i]);
		}
		return result;
	}

	public File file(string name) {
		return new File(Path.Combine(this.path, name));
	}

	public Dir join(params string[] path) {
		return new Dir(Path.Combine(this.path, Path.Combine(path)));
	}
	public Dir join(Dir dir) {
		return new Dir(Path.Combine(this.path, dir.path));
	}

	public bool isAbsolute() {
		return Path.IsPathRooted(this.path);
	}
	public bool exists() {
		return Directory.Exists(this.path);
	}
	public void create() {
		Directory.CreateDirectory(this.path);
	}

	public string get() {
		return this.path;
	}
}
