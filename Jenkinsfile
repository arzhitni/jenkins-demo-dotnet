@Library('Demo-shared-library') _

pipeline {
  agent any
  options { timestamps() }

  environment {
    DOTNET_CLI_TELEMETRY_OPTOUT = "1"
  }

  stages {
    stage('Checkout') {
      steps { checkout scm }
    }

    stage('Build + Test + Publish') {
      steps {
        dotnetCi(
          image: 'jenkins-dotnet-agent:9.0',
          config: 'Release',
          project: 'src/DemoApi/DemoApi.csproj',
          publishDir: 'publish',
          resultsDir: 'TestResults',
          stashName: 'publish-dir'
        )
      }
    }

    stage('Docker') {
      steps {
        dockerBuildArtifact(
          image: "demo/app:${env.BUILD_NUMBER}",
          stashName: 'publish-dir'
        )
      }
    }
  }
}
