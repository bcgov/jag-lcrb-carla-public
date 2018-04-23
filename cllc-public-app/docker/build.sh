export DOCKERHOST=${APPLICATION_URL-$(docker run --rm --net=host codenvy/che-ip)}
sed s/DOCKERHOST/$DOCKERHOST/ < default.conf-template > default.conf
docker build --network="host" -t cllc_proxy .

