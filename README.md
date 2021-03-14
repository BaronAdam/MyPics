# MyPics
![.NET](https://github.com/BaronAdam/MyPics/workflows/.NET/badge.svg)  
Application for adding original pictures and videos.

## Managing migrations
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
## Secrets
```
dotnet user-secrets init
dotnet user-secrets set "AppSettings:EncryptionKey" "TAf30yv4g15177S6EW6idxfE5YxyJiCX8Wf2c4nf9Aw="
dotnet user-secrets set "AppSettings:Token" "TAf30yv4g15177S6EW6idxfE5YxyJiCX8Wf2c4nf9Aw="
dotnet user-secrets set "EmailConfirmation:Sender" ""
dotnet user-secrets set "EmailConfirmation:SenderName" "noreply @ My Pics"
dotnet user-secrets set "EmailConfirmation:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailConfirmation:Port" 465
dotnet user-secrets set "EmailConfirmation:Username" ""
dotnet user-secrets set "EmailConfirmation:Password" ""
dotnet user-secrets set "CloudinarySettings:CloudName" "" 
dotnet user-secrets set "CloudinarySettings:ApiKey" ""
dotnet user-secrets set "CloudinarySettings:ApiSecret" ""
```
