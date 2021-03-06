#!/bin/bash

sudo docker swarm init --listen-addr  $(hostname -I | sed 's/ .*//'):2377 --advertise-addr  $(hostname -I | sed 's/ .*//'):2377 > ./worker_token1

sudo docker swarm join-token --quiet worker > ./worker_token

sudo docker run -it -d -p 8080:8080 -v /var/run/docker.sock:/var/run/docker.sock dockersamples/visualizer

sudo docker network create consul-net -d overlay

sudo docker service create --network=consul-net --name=consul \
    -e 'CONSUL_LOCAL_CONFIG={"skip_leave_on_interrupt": true}' \
    -e CONSUL_BIND_INTERFACE='eth0' \
    -e CONSUL=consul \
    -e CONSUL_CHECK_LEADER=true \
    --replicas 3 \
    --update-delay 10s \
    --update-parallelism 1 \
    -p 8500:8500 sdelrio/consul

sudo docker stack deploy --compose-file ./msa-deployment/services/docker-compose.yml poc

sudo docker run -v "/var/run/docker.sock:/var/run/docker.sock" -p 9000:9000 portainer/portainer
