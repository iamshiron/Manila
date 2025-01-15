const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

Manila.apply('manila.staticlib')
language(Language.cpp)
cppStandard('C++23')
toolchain(ToolChain.clang)

version('1.0.0')
description('Demo Project Core')

binDir(project.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))
objDir(project.getPath().join('bin-int', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))
runDir(project.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))

sourceSets({
	main: Manila.sourceSet(project.getPath().join('src/main/**/*.cpp')),
	test: Manila.sourceSet(project.getPath().join('src/test/**/*.cpp'))
})

dependencies([])

Manila.task('build').execute(() => {
	print('Building core...')
})

Manila.task('test')
	.after('build')
	.execute(() => {
		Manila.test(workspace, project, config)
	})
