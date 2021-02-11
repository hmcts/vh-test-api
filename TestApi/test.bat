cd .\TestApi
dotnet restore
cd..

$env:configuration="debug"
dotnet publish .\TestApi\ --configuration debug
docker-compose -f docker-compose.yml -f docker-compose.test.yml -p TestApi_AC up -d --build