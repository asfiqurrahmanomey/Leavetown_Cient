# Run Whitelabel site on docker locally

## Prerequiste:
 * Docker app intall on your computer
### optional
* Have the Visual studio Integration 
## Run
###  With command line
 ```
 docker-compose -f .\Leavetown.Client\whitelabel-resources\docker-compose-FiveStarCapeCod.yml up --force-recreate --detach
 ```
 If you need to force the build
 
```
docker-compose -f .\Leavetown.Client\whitelabel-resources\docker-compose-FiveStarCapeCod.yml build
```
