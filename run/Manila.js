/*
const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

print('Starting build...')
print('Mode:', config.getBuildMode())
print('Platform:', config.getPlatform())
print('Architecture:', config.getArchitecture())
print('Compiler:', config.getCompiler())

repositories([manilaCentral()])

Manila.project('*', () => {
	binDir(workspace.getDir().join('bin', config.getPlatform(), `${config.getBuildMode()}-${config.getArchitecture()}`))
	runDir(workspace.getDir().join('bin', config.getPlatform(), `${config.getBuildMode()}-${config.getArchitecture()}`))
	objDir(workspace.getDir().join('bin-int', config.getPlatform(), `${config.getBuildMode()}-${config.getArchitecture()}`))

	if (config.getBuildMode() === 'Debug') {
		define('MANILA_DEBUG')
		optimize(Manila.OptimizationLevel.None)
		debugSymbols(true)
	}
	if (config.getBuildMode() === 'Release') {
		define('MANILA_RELEASE')
		optimize(Manila.OptimizationLevel.Speed)
	}
	if (config.getBuildMode() === 'Dist') {
		define('MANILA_DIST')
		optimize(Manila.OptimizationLevel.Speed)
	}

	if (config.getPlatform() === 'Windows') {
		define('MANILA_WINDOWS')
	}
	if (config.getPlatform() === 'Linux') {
		define('MANILA_LINUX')
	}
	if (config.getPlatform() === 'Mac') {
		define('MANILA_MAC')
	}
})

*/
// Runs the client by default
Manila.task('run').execute(() => {
	Manila.runProject(Manila.getProject(':Client'))
})
Manila.task('build').execute(() => {
	Manila.buildProject(Manila.getProject(':Client'), config)
})
