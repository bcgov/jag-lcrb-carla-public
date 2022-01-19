
# getDockerHost; for details refer to https://github.com/bcgov/DITP-DevOps/tree/main/code/snippets#getdockerhost
. /dev/stdin <<<"$(cat <(curl -s --raw https://raw.githubusercontent.com/bcgov/DITP-DevOps/main/code/snippets/getDockerHost))" 
export DOCKERHOST=$(getDockerHost)

docker rmi -f maraapp
cd postgres && docker build . -t maradb && cd ..
cd mara-base && docker build . -t marabase && cd ..
cd mara-app && docker build . -t maraapp && cd ..

