pipeline {
  agent any
  options { timestamps() }

  environment {
    DOTNET_CLI_TELEMETRY_OPTOUT = "1"
    DOCKER_HOST = "unix:///var/run/docker.sock"
  }

  stages {
    stage('Checkout') { steps { checkout scm } }

    stage('Build + Test + Publish') {
      agent {
        docker {
          image 'jenkins-dotnet-agent:8.0'
          reuseNode true
        }
      }
      stages {
        stage('Restore') { steps { sh 'dotnet restore' } }
        stage('Build')   { steps { sh 'dotnet build -c Release --no-restore' } }
        stage('Test') {
          steps {
            sh '''
              rm -rf TestResults || true
              dotnet test -c Release --no-build \
                --logger "trx;LogFileName=test_results.trx" \
                --results-directory TestResults
            '''
          }
          post {
            always {
              step([$class: 'MSTestPublisher',
                testResultsFile: '**/TestResults/*.trx',
                failOnError: false,
                keepLongStdio: true
              ])
            }
          }
        }
        stage('Publish') {
          steps {
            sh 'rm -rf publish || true'
            sh 'dotnet publish src/DemoApi/DemoApi.csproj -c Release -o publish'
          }
        }
      }
    }

    stage('Docker build') {
      steps {
        script {
          def tag = "demo/demoapi:${env.BUILD_NUMBER}"
          def img = docker.build(tag, ".")
          echo "Built image: ${img.id} (${tag})"
        }
      }
    }
  }
}