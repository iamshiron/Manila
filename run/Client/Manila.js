const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

Manila.apply('manila.staticlib')
language('C++')
cppStandard('C++23')

version('1.0.0')
description('Demo Project Client')

sourceSets({
	main: Manila.sourceSet(project.getLocation().join('src/main/**/*.cpp')),
	test: Manila.sourceSet(project.getLocation().join('src/test/**/*.cpp'))
})

dependencies([Manila.compile(Manila.getProject(':Core'))], Manila.link('opengl32.lib'))

Manila.task('build').executes(() => {
	Manila.build(workspace, project, config)
})

Manila.task('test')
	.dependsOn('build')
	.executes(() => {
		Manila.test(workspace, project, config)
	})

Manila.task('run')
	.dependsOn('test')
	.executes(() => {
		Manila.run(workspace, project, config)
	})
