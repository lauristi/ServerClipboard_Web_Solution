pipeline {
  agent any
  stages {
    stage('01- Checkout') {
      steps {
        script {
          withCredentials([string(credentialsId: 'JENKINS_TOKEN', variable: 'GITHUB_TOKEN')]) {
            try {
              sh '''
                 git clone https://${GITHUB_TOKEN}@${GIT_REPO}
                cd ServerClipboard_Web_Solution
                git checkout ${BRANCH}
              ''' 
             } catch (Exception e) {
               TratarErro(e)
            }
          }
        }
      }
    }

    stage('02- Restore Dependencies') {
      steps {
        script {
          try {
            sh "${env.DOTNET_ROOT}/dotnet --version"
            sh "${env.DOTNET_ROOT}/dotnet restore ${env.SOLUTION_PATH}"
          } catch (Exception e) {
            TratarErro(e)
          }
        }
      }
    }

    stage('03- Build') {
      steps {
        script {
          try {
            sh "${env.DOTNET_ROOT}/dotnet build ${env.SOLUTION_PATH} --no-restore --configuration Debug"
          } catch (Exception e) {
            TratarErro(e)
          }
        }
      }
    }

    stage('04- Test') {
      steps {
        script {
          try {
            sh "${env.DOTNET_ROOT}/dotnet test ${env.SOLUTION_PATH} --no-build --verbosity normal"
          } catch (Exception e) {
            TratarErro(e)
          }
        }
      }
    }

    stage('05- Publish') {
      steps {
        script {
          try {
            sh "${env.DOTNET_ROOT}/dotnet publish ${env.PROJECT_PATH} -c Release -o ${env.PUBLISH_PATH}"
          } catch (Exception e) {
            TratarErro(e)
          }
        }
      }
    }

    stage('06- Package Artifacts') {
      steps {
        script {
          try {
            sh """
            mkdir -p ${env.ARTIFACT_PATH}
            cp -r ${env.PUBLISH_PATH}/* ${env.ARTIFACT_PATH}/
            """
            archiveArtifacts artifacts: "${env.ARTIFACT_PATH}/**", allowEmptyArchive: true
          } catch (Exception e) {
            TratarErro(e)
          }
        }
      }
    }

    stage('07- Deploy on server') {
      steps {
        script {
          try {
            sh """
            sudo -S cp -r "${env.ARTIFACT_PATH}"/* "${env.DEPLOY_DIR}/" && echo "Copy succeeded" || echo "Copy failed"
            sudo chown -R www-data:www-data "${env.DEPLOY_DIR}/" && echo "Chown succeeded" || echo "Chown failed"
            """
          } catch (Exception e) {
            TratarErro(e)
          }
        }
      }
    }

    stage('Finish Log') {
      steps {
        cleanWs(cleanWhenAborted: true, cleanWhenFailure: true, cleanWhenNotBuilt: true, cleanWhenSuccess: true, cleanWhenUnstable: true, cleanupMatrixParent: true, deleteDirs: true)
      }
    }
  }
  environment {
    GIT_REPO = 'github.com/lauristi/ServerClipboard_Web_Solution.git'
    BRANCH = 'master'
    SOLUTION_PATH = 'ServerClipboard_Web'
    PROJECT_PATH = 'ServerClipboard_Web/ServerClipboard_Web.csproj'
    PUBLISH_PATH = 'ServerClipboard_Web/bin/Release/net8.0/publish'
    ARTIFACT_PATH = 'ServerClipboard_Web/Artifact'
    DEPLOY_DIR = '/var/www/app/ServerClipboardProjects/ServerClipboard_Web'
    DOTNET_ROOT = '/opt/dotnet'
    PATH = "${env.DOTNET_ROOT}:${env.PATH}"
  }
  post {
    always {
      cleanWs()
    }
  }
}

def TratarErro(Exception e) {
    currentBuild.result = 'FAILURE'
    echo "Deploy failed: ${e.message}"
    error('Deploy failed')
}