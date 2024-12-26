/*
const workspace = Manila.getWorkspace()
const config = Manila.getBuildConfig()

print(`Building ${project.name()} -> ${project.getBinDir()}`)

version('1.0.0')
description('Server project')

const mainSrcSet = Manila.sourceSet('main')
mainSrcSet.addFiles(project.getDir().join('src/main'))

const mainIncludeSet = Manila.includeSet('main')
mainIncludeSet.addFiles(project.getDir().join('src/main'))

dependencies([
	compile(project('Core')),
	compile(git('github.com/gabime/spdlog', 'v1.x')),
	include(git('github.com/g-truc/glm', 'master')).as('GLM')
])

addSourceSet(mainSrcSet)
addIncludeSet(Manila.Public, mainIncludeSet)

addIncludeDir(Manila.Private, workspace.getDir().join('include'))
addIncludeDir(Manila.Public, mainSrcSet.getDir())

Manila.task('build').execute(() => {
	Manila.build(project, config)
})
Manila.task('run')
	.after('build')
	.execute(() => {
		Manila.run(project)
	})
*/

print('Server')

Manila.task('build').execute(() => {
	print('Building Server...')
})

Manila.task('run')
	.after(':build')
	.execute(() => {
		print('Running Server...')
	})
