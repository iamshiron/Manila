const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

Manila.apply('manila.staticlib')
language('C++')
cppStandard('C++23')

version('1.0.0')
description('Demo Project Core')

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
