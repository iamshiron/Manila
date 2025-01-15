
using System.Runtime.InteropServices.Marshalling;

namespace Shiron.Manila.API.Toolchain;

public interface IToolChain {
	abstract void compile(Project project);
}
