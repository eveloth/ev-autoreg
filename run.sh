#!/bin/bash
#
# AVOID STARTING AN APP USING THIS SCRIPT IN PRODCUTION
# IT WILL SET PASSWORDS AND KEYS TO STATIC DEFAULT VALUES

cp -v .envexample .env
sed -i "/PG_PASS/s|''|'400aaa3a-003a-439c-8d29-f10bc7767bf1'|" .env &&
sed -i "/REDIS__PASSWORD/s|'.*'|'21e89199-9411-42d2-b61b-552bcd126d4c'|" .env &&
sed -i "/SYMMETRICSECURITYKEY/s|'.*'|'74bcb7ac-fde0-485d-b47b-ccaf88c8ca6f'|" .env &&
sed -i "/JWT__KEY/s|'.*'|'2409eb94-fd47-4f84-9c14-2588199c0879'|" .env &&

docker compose up -d --build
