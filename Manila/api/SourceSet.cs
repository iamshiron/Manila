
public class SourceSet {
	public List<Dir> dirs { get; private set; }

	public SourceSet(Dir d) {
		dirs = new List<Dir>();
		dirs.Add(d);
	}

	public void include(Dir d) { }
	public void exclude(Dir d) { }
}
