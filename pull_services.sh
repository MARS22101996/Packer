#!/bin/bash
sudo git init

sudo git config user.email "maria_suvalova@mail.ru"

sudo git config user.name "Mary Suvalova"

sudo git pull https://user:riama2210@github.com/MARS22101996/Packer.git master

cd ./services

sudo docker-compose build

cd ../
