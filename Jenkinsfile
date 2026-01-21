@Library('Demo-shared-library') _

pipeline {
  agent any
  options { timestamps() }

  environment {
    DOTNET_CLI_TELEMETRY_OPTOUT = "1"
    IMAGE = "demo/demoapi:${env.BUILD_NUMBER}"
    CONTAINER = "demoapi-${env.BUILD_NUMBER}"
  }

  stages {
    stage('Checkout') {
      steps { checkout scm }
    }

    stage('CI (Build + Test)') {
      steps {
        dotnetCi(image: 'jenkins-dotnet-agent:8.0', config: 'Release')
      }
    }

    stage('Docker build') {
      steps {
        dockerBuildImage(image: env.IMAGE, dockerfile: 'Dockerfile', contextDir: '.')
      }
    }

    stage('Run') {
      steps {
        script {
          dockerRunContainer(image: env.IMAGE, name: env.CONTAINER, hostPort: 18080, containerPort: 8080)
        }
      }
    }

    stage('Health') {
      steps {
        dockerHealthCheck(name: env.CONTAINER, path: '/health', port: 8080)
      }
    }
  }

  post {
    always {
      dockerRemoveContainer(name: env.CONTAINER)
    }
  }
}