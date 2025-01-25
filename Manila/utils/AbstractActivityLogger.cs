
namespace Shiron.Manila.Utils;

public abstract class AbstractActivityLogger {
	public abstract void task(API.Task t);
	public abstract void taskEnd(API.Task t);
	public abstract void compileProject(API.Project p);
	public abstract void compileProjectEnd(API.Project p);
	public abstract void compileFile(string file);
}
