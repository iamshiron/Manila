const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

Manila.apply('manila.console')
language(Language.cpp)
cppStandard('C++23')
toolchain(ToolChain.clang)

version('1.0.0')
description('Demo Project Server')

binDir(project.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))
objDir(project.getPath().join('bin-int', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))
runDir(project.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))

sourceSets({
	main: Manila.sourceSet(project.getPath().join('src/main')).include('**/*.cpp'),
	test: Manila.sourceSet(project.getPath().join('src/test')).include('**/*.cpp')
})

dependencies([Manila.compile(Manila.getProject(':core'))])

Manila.task('build').execute(() => {
	Manila.build(workspace, project, config)
})

Manila.task('test')
	.after(':build')
	.execute(() => {
		print('Runnin tests...')
	})

Manila.task('run')
	.after(':build')
	.execute(() => {
		print('Running server...')
		Manila.run(project)
	})
