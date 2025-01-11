/*
const workspace = Manila.getWorkspace()
const project = Manila.getProject()
const config = Manila.getBuildConfig()

print(`Building ${project.name()} -> ${project.getBinDir()}`)

version('1.0.0')
description('Server for the game')

dependencies([compile(project(':Core')), link('opengl32.lib')])

Manila.task('build').execute(() => {
	Manila.build(project, config)
})
Manila.task('run')
	.after('build')
	.execute(() => {
		Manila.run(project)
	})
*/

Manila.apply('shiron.manila:manilacpp:console')
const project = Manila.getProject()

const sourceSet = ManilaCPP.sourceSet()
sourceSet.include(project.getPath().join('src/main').files())
project.sourceSet('main', sourceSet)

project.define('DEBUG')
project.build()
project.toolChain('GCC')
