#!/bin/bash
sudo docker run -it -d -p 8080:8080 -v /var/run/docker.sock:/var/run/docker.sock dockersamples/visualizer

sudo docker stack deploy --compose-file docker-compose.yml poc