const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

Manila.apply('manila.staticlib')
language('C++')
cppStandard('C++23')

version('1.0.0')
description('Demo Project Client')

sourceSets({
	main: Manila.sourceSet(project.getPath().join('src/main/**/*.cpp')),
	test: Manila.sourceSet(project.getPath().join('src/test/**/*.cpp'))
})

dependencies([])

Manila.task('build').execute(() => {
	project.build()
	Manila.build(workspace, project, config)
})

Manila.task('test')
	.after('build')
	.execute(() => {
		Manila.test(workspace, project, config)
	})
