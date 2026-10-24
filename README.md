# LocalJobs backend

ASP.NET Core API for the LocalJobs local work and classifieds product.

## Local development configuration

The repository does not contain database credentials or a JWT signing key. Configure them using .NET User Secrets or environment variables before running the API or EF Core commands:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=LocalJobsDb;Username=<user>;Password=<password>" --project LocalJobs.Api
dotnet user-secrets set "Jwt:Key" "<random secret of at least 32 bytes>" --project LocalJobs.Api
```

`Jwt:Issuer` and `Jwt:Audience` are non-secret settings in `appsettings.json`.
