# ToDoList

This repository is a little playground where I get to test technologies I find interesting and I don't get to use at work.

So far I've been using
- [x] ASP.NET Core 3.1 GRPC service
- [x] ASP.NET Core 3.1 Web API
- [x] ASP.NET Core 3.1 Web application using Razor pages
- [x] dotnet tye
- [x] AWS X-Ray integration

Future steps:
- [ ] Data storage using DynamoDB
- [ ] MtoM authentication/authorization using IdentityServer
- [ ] Web application using Blazor WebAssembly
- [ ] Web application using Angular
- [ ] CLI using new System.CommandLine libraries

## Architecture

The architecture of this application is willingly overshot.

The application has multiple layers:
- Clients
  - ASP.NET Core 3.1 Web application using Razor pages ✔
  - Web application using Blazor WebAssembly
  - Web application using Angular
  - CLI using new System.CommandLine libraries
- Entrypoint
  - ASP.NET Core 3.1 Web API ✔
- Resource Access
  - ASP.NET Core 3.1 GRPC service ✔
- Storage
  - Data storage using DynamoDB
- Infrastructure
  - IdentityServer
  
## How to run it

This project uses Tye to tie together all the pieces of the system.

You can launch all the applications at one go

```powershell
$ dotnet tool restore
$ dotnet tye run
```

You will see something similar to this on your console:

```text
> dotnet tye run
Loading Application Details...
Launching Tye Host...

[10:52:38 INF] Executing application from C:\Users\Renato\Development\Tests\ToDoList\tye.yaml
[10:52:38 INF] Dashboard running on http://127.0.0.1:8000
[10:52:39 INF] Docker image amazon/aws-xray-daemon already installed
[10:52:39 INF] Creating docker network tye_network_01b01e38-0
[10:52:39 INF] Running docker command network create --driver bridge tye_network_01b01e38-0
[10:52:40 INF] Running image amazon/aws-xray-daemon for xray-daemon_b296bda6-d
[10:52:40 INF] Running image mcr.microsoft.com/dotnet/core/sdk:3.1 for service-proxy_2b8b914c-c
[10:52:40 INF] Running image mcr.microsoft.com/dotnet/core/sdk:3.1 for webapi-proxy_1654fcd3-a
[10:52:40 INF] Running image mcr.microsoft.com/dotnet/core/sdk:3.1 for web-proxy_8a05a32b-9
[10:52:40 INF] Building projects
[10:52:43 INF] Running container webapi-proxy_1654fcd3-a with ID 241c4648c4c9
[10:52:43 INF] Running docker command network connect tye_network_01b01e38-0 webapi-proxy_1654fcd3-a --alias webapi
[10:52:43 INF] Running container service-proxy_2b8b914c-c with ID c3b3a0ae1a89
[10:52:43 INF] Running docker command network connect tye_network_01b01e38-0 service-proxy_2b8b914c-c --alias service
[10:52:44 INF] Running container xray-daemon_b296bda6-d with ID 6964bbeb2772
[10:52:44 INF] Running docker command network connect tye_network_01b01e38-0 xray-daemon_b296bda6-d --alias xray-daemon
[10:52:44 INF] Running container web-proxy_8a05a32b-9 with ID 1dcc649188aa
[10:52:44 INF] Running docker command network connect tye_network_01b01e38-0 web-proxy_8a05a32b-9 --alias web
[10:52:44 INF] Collecting docker logs for webapi-proxy_1654fcd3-a.
[10:52:44 INF] Collecting docker logs for service-proxy_2b8b914c-c.
[10:52:44 INF] Replica xray-daemon_b296bda6-d is moving to a ready state
[10:52:44 INF] Collecting docker logs for xray-daemon_b296bda6-d.
[10:52:44 INF] Collecting docker logs for web-proxy_8a05a32b-9.
[10:52:47 INF] Launching service webapi_2792ce7a-9: C:\Users\Renato\Development\Tests\ToDoList\src\entrypoints\WebAPI\bin\Debug\netcoreapp3.1\WebAPI.exe
[10:52:47 INF] Launching service service_d38802f5-5: C:\Users\Renato\Development\Tests\ToDoList\src\resource-access\Service\bin\Debug\netcoreapp3.1\Service.exe
[10:52:47 INF] Launching service web_eec8efb7-2: C:\Users\Renato\Development\Tests\ToDoList\src\clients\Web\bin\Debug\netcoreapp3.1\Web.exe
[10:52:47 INF] webapi_2792ce7a-9 running on process id 15108 bound to https://localhost:56726
[10:52:47 INF] Replica webapi_2792ce7a-9 is moving to a ready state
[10:52:47 INF] service_d38802f5-5 running on process id 13912 bound to https://localhost:23123
[10:52:47 INF] Replica service_d38802f5-5 is moving to a ready state
[10:52:47 INF] web_eec8efb7-2 running on process id 12216 bound to https://localhost:8080
[10:52:47 INF] Replica web_eec8efb7-2 is moving to a ready state
[10:52:48 INF] Selected process 15108.
[10:52:48 INF] Selected process 13912.
[10:52:48 INF] Listening for event pipe events for webapi_2792ce7a-9 on process id 15108
[10:52:48 INF] Listening for event pipe events for service_d38802f5-5 on process id 13912
[10:52:48 INF] Selected process 12216.
[10:52:48 INF] Listening for event pipe events for web_eec8efb7-2 on process id 12216
```

When everything is ready, just point your browser to http://127.0.0.1:8000 to access Tye's dashboard.

From there you can see all the services been launched, access their logs and operational metrics.

![The dashboard of Tye](/.assets/tye-dashboard.png)

Click any binding to open a tab of your browser to that page.

To exit, simply hit `CTRL+C`.

## AWS X-Ray

All the applications are instrumented to push traces to AWS X-Ray even in development environment.

To do so, you'll need to start a container based on the image `amazon/xray-daemon` and bind it to port 2000.

Currently there is the definition for this container in the `tye.yml` file but this setup is not working at the moment.

To test pushing traces to AWS X-Ray, you'll need to start the container outside of Tye.
