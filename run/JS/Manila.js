const workspace = Manila.getWorkspace()
const project = Manila.getProject()

Manila.apply('shiron.manila:js@1.0.0/bun')
const config = Manila.getConfig()

project.version('1.0.0')
project.description('A JavaScript project for Manila')

project.sourceSets({
	main: Manila.sourceSet(project.getPath().join('src')).include('**/*')
})

project.artifacts({
	main: Manila.artifact(artifact => {
		Manila.job('build')
			.description('Build the JavaScript Project')
			.execute(async () => {
				await Manila.build(workspace, project, config, artifact)
			})

		Manila.job('dev')
			.description('Run the JavaScript Project')
			.execute(() => {
				Manila.run(project, artifact)
			})
	})
		.from('shiron.manila:js/bun')
		.description('JavaScript Main Artifact')
})
