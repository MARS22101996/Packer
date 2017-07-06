#!/bin/bash

docker service create --network=consul-net --name=consul \
    -e 'CONSUL_LOCAL_CONFIG={"skip_leave_on_interrupt": true}' \
    -e CONSUL_BIND_INTERFACE='eth0' \
    -e CONSUL=consul \
    -e CONSUL_CHECK_LEADER=true \
    --replicas 3 \
    --update-delay 10s \
    --update-parallelism 1 \
    -p 8500:8500 sdelrio/consul

docker stack deploy --compose-file /services/docker-compose.yml poc
