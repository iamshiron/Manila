const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

Manila.apply('shiron.manila:cpp@1.0.0:console')
language(Language.cpp)
cppStandard('C++23')
toolchain(ToolChain.clang)

version('1.0.0')
description('Demo Project Client')

binDir(workspace.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))
objDir(workspace.getPath().join('bin-int', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))
runDir(workspace.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, project.getName()))

sourceSets({
	main: Manila.sourceSet(project.getPath().join('src/main/**/*.cpp')),
	test: Manila.sourceSet(project.getPath().join('src/test/**/*.cpp'))
})

dependencies([Manila.compile(Manila.getProject(':core')), Manila.link('opengl32.lib')])

Manila.task('build').execute(() => {
	print('Building client...')
})

Manila.task('test')
	.after('build')
	.execute(() => {
		Manila.test(workspace, project, config)
	})

Manila.task('run')
	.after('test')
	.execute(() => {
		Manila.run(workspace, project, config)
	})
