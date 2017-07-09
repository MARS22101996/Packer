#!/bin/bash
#You should change token (copy file with tocken) and IP for appropriate SWARM manager
docker swarm join --token $(cat ./worker_token) 192.168.56.102:2377