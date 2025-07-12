const module = Manila.getModule()
const workspace = Manila.getWorkspace()
const config = Manila.getConfig()

Manila.apply('shiron.manila:cpp@1.0.0:console')

version('1.0.0')
description('Demo Module Core')

binDir(workspace.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, module.getName()))
objDir(workspace.getPath().join('bin-int', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, module.getName()))
runDir(workspace.getPath().join('bin', config.getPlatform(), `${config.getConfig()}-${config.getArchitecture()}`, module.getName()))

sourceSets({
	main: Manila.sourceSet(module.getPath().join('src/main')).include('**/*.cpp'),
	test: Manila.sourceSet(module.getPath().join('src/test')).include('**/*.cpp')
})

dependencies([Manila.module('core', 'build')])

Manila.task('clean')
	.description('Clean the client')
	.execute(() => {
		print('Cleaning Client...')
	})

Manila.task('build').execute(() => {
	Manila.build(workspace, module, config)
})
Manila.task('build-test').execute(async () => {
	print('Building Test...')
	await Manila.sleep(5000)
	print('Done!')
})

Manila.task('test')
	.description('Run the Client Tests')
	.after('build-test')
	.execute(() => {
		print('Testing Client...')
	})
Manila.task('run')
	.description('Run the Client')
	.after('test')
	.after('build')
	.execute(() => {
		Manila.run(module)
	})
