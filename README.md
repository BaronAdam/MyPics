# MyPics
![.NET](https://github.com/BaronAdam/MyPics/workflows/.NET/badge.svg)  
Application for adding original pictures and videos.

## Adding migrations
```bash
cd MyPics.Infrastructure
dotnet ef --startup-project ../MyPics.Api/ migrations add [name]
dotnet ef --startup-project ../MyPics.Api/ migrations list
dotnet ef --startup-project ../MyPics.Api/ database update [name/0]
dotnet ef --startup-project ../MyPics.Api/ migrations remove
```
## Docker
```
docker-compose up -d
```
## Flutter: build runner
```bash
flutter packages pub run build_runner build
```
