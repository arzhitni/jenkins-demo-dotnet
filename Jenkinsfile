pipeline {
  agent any
  options { timestamps() }

  environment {
    DOTNET_CLI_TELEMETRY_OPTOUT = "1"
    DOCKER_HOST = "unix:///var/run/docker.sock"
    IMAGE = "demo/demoapi:${env.BUILD_NUMBER}"
    CONTAINER = "demoapi-${env.BUILD_NUMBER}"
  }

  stages {
    stage('Checkout') {
      steps { checkout scm }
    }
    stage('Build + Test') {
      agent {
        docker {
          image 'mcr.microsoft.com/dotnet/sdk:9.0'
          reuseNode true
        }
      }
      stages {
        stage('Build') { steps { sh 'dotnet restore && dotnet build -c Release --no-restore' } }
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
      }
    }

    stage('Docker build') {
      steps {
        script {
          docker.build(env.IMAGE, ".")
          sh "docker image ls ${env.IMAGE} --digests || true"
        }
      }
    }

    stage('Run container') {
      steps {
        script {
          sh """
            docker rm -f ${env.CONTAINER} >/dev/null 2>&1 || true
            docker run -d --name ${env.CONTAINER} -p 18080:8080 ${env.IMAGE}
          """
        }
      }
    }

    stage('Health check') {
    steps {
      sh '''
        set -e

        for i in $(seq 1 10); do
          state=$(docker inspect -f '{{.State.Status}}' "$CONTAINER" 2>/dev/null || true)
          [ "$state" = "running" ] && break
          echo "Container state: $state (waiting...)"
          sleep 1
        done

        for i in $(seq 1 30); do
          if docker run --rm --network "container:${CONTAINER}" alpine:latest sh -lc \
            "apk add --no-cache wget >/dev/null 2>&1 && wget -qO- http://localhost:8080/health >/dev/null 2>&1"
          then
            echo "Health check OK"
            exit 0
          fi

          echo "Waiting for service... ($i/30)"
          sleep 1
        done

        echo "Service did not become healthy"
        docker logs "$CONTAINER" || true
        exit 1
      '''
    }
}

  }

  post {
    always {
      sh "docker rm -f ${env.CONTAINER} >/dev/null 2>&1 || true"
    }
  }
}