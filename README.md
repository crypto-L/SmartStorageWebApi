# SmartStorageWebApi

Installation guide.

Necessary pre-installed programs: Docker Desktop, JetBrains Rider


- Make sure you have .net 6 installed.
  Otherwise, download and install it from the official Windows website.
  Downloading link https://dotnet.microsoft.com/en-us/download/dotnet/6.
- Copy the link to the repository from Github
- Open Rider
- In the window that opens, click "Get from VCS" 
and paste the link to the repository to clone the repository to your computer
- Run Docker Desktop on your computer
- While in Rider , find the solution folder named "Other". 
It contains the docker-compose.yml file. Run it up
- In Docker Desktop, make sure containers are running
- In "Run /Debug Configurations" select "SmartStorage" option
- Install EF Core by running the following command in a terminal
~~~
dotnet tool install –global dotnet-ef
~~~
- Create a migration by typing the following in the terminal
~~~
dotnet ef migrations add --project App.DAL --startup-project WebApp --context AppDbContext Initial
~~~
- Apply the generated migration with the following command
~~~
dotnet ef database update --project App.DAL --startup-project WebApp --context AppDbContext
~~~
NB! If during the process of applying the migration an error occurs due to the 
lack of Microsoft.NETCore.App version 2.0.0 (x64), then download and install it from this link

Link: https://dotnet.microsoft.com/en-us/download/dotnet/2.0/runtime?cid=getdotnetcore

- Run the project with the keyboard shortcut Shift + F10
