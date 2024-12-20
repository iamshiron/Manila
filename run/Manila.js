const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

Manila.log('Starting build...')
Manila.log('Mode:', config.getBuildMode())
Manila.log('Platform:', config.getPlatform())
Manila.log('Architecture:', config.getArchitecture())
Manila.log('Compiler:', config.getCompiler())

// Runs the client by default
task('run').execute(() => {
	Manila.getProject(':Client').run('run')
})
