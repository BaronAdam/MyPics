# MyPics
Application for adding original pictures and videos.

## Adding migrations
```bash
cd MyPics.Infrastructure
dotnet ef --startup-project ../MyPics.Api/ migrations add [name]
```
## Docker
```
docker-compose up -d
```
## Flutter: build runner
```bash
flutter packages pub run build_runner build
```
