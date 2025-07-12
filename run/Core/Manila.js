const module = Manila.getModule()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

Manila.apply('shiron.manila:cpp@1.0.0:staticlib')
version('1.0.0')
description('Demo Module Core')

binDir(workspace.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, module.getName()))
objDir(workspace.getPath().join('bin-int', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, module.getName()))

sourceSets({
	main: Manila.sourceSet(module.getPath().join('src/main')).include('**/*.cpp'),
	test: Manila.sourceSet(module.getPath().join('src/test')).include('**/*.cpp')
})

Manila.task('build')
	.description('Build the Core')
	.execute(() => {
		Manila.build(workspace, module, config)
	})
