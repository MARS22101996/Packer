#!/bin/bash
git init

git config user.email "maria_suvalova@mail.ru"

git config user.name "Mary Suvalova"

git pull https://user:riama2210@github.com/MARS22101996/Packer.git master

cd ./services

sudo docker-compose build

cd ../
