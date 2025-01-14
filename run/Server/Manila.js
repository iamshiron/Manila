const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

Manila.apply('manila.console')
language(Language.Cpp)
cppStandard('C++23')

version('1.0.0')
description('Demo Project Server')
toolchain(ToolChain.Clang)

sourceSets({
	main: Manila.sourceSet(project.getPath().join('src/main/**/*.cpp')),
	test: Manila.sourceSet(project.getPath().join('src/test/**/*.cpp'))
})

dependencies([Manila.compile(Manila.getProject(':core'))])

Manila.task('build').execute(() => {
	project.build()
	print('Building server...')
})

Manila.task('test')
	.after(':build')
	.execute(() => {
		print('Runnin tests...')
	})

Manila.task('run')
	.after(':test')
	.execute(() => {
		print('Running server...')
	})
