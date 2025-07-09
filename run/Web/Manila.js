const project = Manila.getProject()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

Manila.apply('shiron.manila:javascript@1.0.0:ts')

version('1.0.0')
description('Demo Project JavaScript')

sourceSets({
	main: Manila.sourceSet(project.getPath().join('src/main')).include('**/*.js'),
	test: Manila.sourceSet(project.getPath().join('src/test')).include('**/*.js'),
	alias: Manila.sourceSet(project.getPath().join('src/test')).include('**/alias/*.js')
})

dependencies([Manila.npm('axios')])

Manila.task('build')
	.description('Builds the project')
	.execute(() => {
		Manila.build(project)
	})

Manila.task('run')
	.description('Runs the project')
	.execute(() => {
		Manila.run(project)
	})
