#!/bin/bash
#You should change token (copy file with tocken) and IP for appropriate SWARM manager
sudo docker swarm join --token $(cat ./worker_token) 10.23.21.224:2377