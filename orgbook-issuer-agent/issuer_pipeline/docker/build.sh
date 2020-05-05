
export DOCKERHOST=${APPLICATION_URL-$(docker run  --rm --net=host eclipse/che-ip)}

docker rmi -f maraapp
cd postgres && docker build . -t maradb && cd ..
cd mara-base && docker build . -t marabase && cd ..
cd mara-app && docker build . -t maraapp && cd ..

