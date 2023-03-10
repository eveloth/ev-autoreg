# AVOID STARTING AN APP USING THIS SCRIPT IN PRODCUTION
# IT WILL SET PASSWORDS AND KEYS TO STATIC DEFAULT VALUES

Copy-Item -Path ".envexample" -Destination ".env"
(Get-Content .env) -replace "(PG_PASS=)'.*'", "`$1'400aaa3a-003a-439c-8d29-f10bc7767bf1'" | Out-File .env
(Get-Content .env) -replace "(REDIS__PASSWORD=)'.*'", "`$1'21e89199-9411-42d2-b61b-552bcd126d4c'" | Out-File .env
(Get-Content .env) -replace "(SYMMETRICSECURITYKEY=)'.*'", "`$1'74bcb7ac-fde0-485d-b47b-ccaf88c8ca6f'" | Out-File .env
(Get-Content .env) -replace "(JWT__KEY=)'.*'", "`$1'2409eb94-fd47-4f84-9c14-2588199c0879'" | Out-File .env

docker compose -f compose-win.yaml up -d --build
