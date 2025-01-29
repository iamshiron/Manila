const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

Manila.from('manila:console')
Manila.apply('docker:container')
language(Language.cpp)
cppStandard('C++23')
toolchain(ToolChain.clang)

version('1.0.0')
description('Demo Project Server')

binDir(workspace.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))
objDir(workspace.getPath().join('bin-int', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))
runDir(workspace.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))

sourceSets({
	main: Manila.sourceSet(project.getPath().join('src/main')).include('**/*.cpp'),
	test: Manila.sourceSet(project.getPath().join('src/test')).include('**/*.cpp')
})

dependencies([Manila.compile(Manila.getProject(':core'))])

project.docker()

Manila.task('build').execute(() => {
	Manila.build(workspace, project, config)
})

Manila.task('test')
	.after(':build')
	.execute(() => {
		print('Running tests...')
	})

Manila.task('run')
	.after(':build')
	.execute(() => {
		print('Running server...')
		Manila.run(project)
	})
