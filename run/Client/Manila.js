const workspace = Manila.getWorkspace()
const project = Manila.getProject()
const config = Manila.getBuildConfig()

Manila.log(`Building ${project.name()} -> ${project.getBinDir()}`)

project.configure(c => {
	c.name('Server').version('1.0.0').description('Server for the game')
})
compileHint('static')

dependencies([compile(project(':Core')), link('opengl32.lib')])

task('build').execute(() => {
	Manila.build(project, config)
})
task('run')
	.after('build')
	.execute(() => {
		Manila.run(project)
	})
