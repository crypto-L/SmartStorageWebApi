# SmartStorageWebApi
RESTful Web API for SmartStorage system

Tools installation
~~~
right click on WebApp project -> Manage NuGet packages -> install 'Microsoft.EntityFrameworkCore' ,
 'Microsoft.EntityFrameworkCore.Tools' and 'Npgsql.EntityFrameworkCore.PostgreSQL'
 
 right click on All.DAL project -> Manage NuGet packages -> install 'Microsoft.EntityFrameworkCore.Migrations' , 'Npgsql.EntityFrameworkCore.PostgreSQL'
 
~~~

Database migration
~~~sh
dotnet ef migrations add --project App.DAL --startup-project WebApp --context AppDbContext Initial
dotnet ef database update --project App.DAL --startup-project WebApp --context AppDbContext
~~~