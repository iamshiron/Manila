const workspace = Manila.getWorkspace()
const project = Manila.getProject()
const config = Manila.getBuildConfig()

Manila.log(`Building ${project.name()} -> ${project.getBinDir()}`)

project.configure(c => {
	c.name('Server').version('1.0.0').description('Server for the game')
})
compileHint('static')

dependencies([
	compile(git('github.com/gabime/spdlog', 'v1.x')),
	compile(git('github.com/boostorg/boost', 'master')),
	include(git('github.com/g-truc/glm')).as('glm')
])
