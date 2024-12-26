/*
const workspace = Manila.getWorkspace()
const project = Manila.getProject()
const config = Manila.getBuildConfig()

print(`Building ${project.name()} -> ${project.getBinDir()}`)

version('1.0.0')
description('Server for the game')

compileHint('static')

dependencies([
	compile(git('github.com/gabime/spdlog', 'v1.x')),
	compile(git('github.com/boostorg/boost', 'master')),
	include(git('github.com/g-truc/glm')).as('glm')
])
*/

Manila.apply('shiron.manila:manilacpp:console')
const project = Manila.getProject()

define('MANILA_CORE')
build()

print('Core')
