
# VulnerableApp4APISecurity

This repository was developed using .NET 7.0 API technology based on findings listed in the OWASP 2019 API Security Top 10. This project has been developed to be used as an example project while explaining API Security and to easily generate attack scenarios while companies are trying the products they test to ensure API Security. In addition, Mongo was used as the database in the project, and it was developed considering Clean Architecture and Solid Principles as much as possible.

 **The project will be examined under the following headings.**

- 1 - Vulnerabilities
- 2 - Run
	- 2.1 - Running via IDE
	- 2.2 - Running from Container
	- 2.3 - Running from Docker-compose
	- 2.4 - Running from Kubernetes
- 3 - Dummy Traffics
	- 3.1 - Available Side
	- 3.2 - Updatable Side
- 4 - API Details Documentations 
	- 4.1 - Postman Collection
	- 4.2 - Excel of API Details 
- 5 - API Security Presentation

## 1 - Vulnerabilities
The vulnerabilities on the developed application are as follows. While some of these vulnerabilities can be found directly, some have been developed to find and exploit indirectly (Path/Arguments fuzzing, Brute-force/rate limit) to simulate real-life scenarios.

-  **API 01:2019** — Broken object level authorization
-  **API 02:2019** — Broken authentication
-  **API 03:2019** — Excessive data exposure
-  **API 04:2019** — Lack of resources and rate limiting
-  **API 05:2019** — Broken function level authorization
-  **API 06:2019** — Mass assignment
-  **API 07:2019** — Security misconfiguration
-  **API 08:2019** — Injection
-  **API 09:2019** — Improper Assets Management : *soon*
-  **API 10:2019** — Insufficient logging and monitoring


## 2 - Run
Below is an explanation for running the developed application.

### 2.1 -  Running via IDE
Since the project was developed with .NET 7.0, it can be run platform-independently. For this, if you have .NET 7.0 SDK and Visual Studio in terms of easy use, it will be enough for it to work and analyze. 
```
git clone https://github.com/Erdemstar/VulnerableApp4APISecurity
cd VulnerableApp4APISecurity 
- mouse click on reflected-xss-tag-attribute-src.sln
- devenv reflected-xss-tag-attribute-src.sln (CMD)
```
**NOTE** : When the application is up, you can use/edit the Database settings from the appsettings.json section. Apart from this, once the application is up and running, not only the database is needed, but also elasticsearch and Promethous integration is available if needed. You can review docker-compose for details.

### 2.2 - Running from Container
You can use one of the following images to run through the container.
```
- docker run erdemstar/vulnerableapp4apisecurity:amd64

- docker run erdemstar/vulnerableapp4apisecurity:arm64
```
**NOTE** : When the application stands up, it uses a database settings by default and makes a Mongo connection over it. If you are going to run it in a container environment, I recommend you to check this settings.

### 2.3 - Running from Docker-compose
If you want to run it directly, you can download the previously prepared docker-compose.yml file as follows and skip the installation step quickly.
```
curl https://raw.githubusercontent.com/Erdemstar/VulnerableApp4APISecurity/main/Resource/Dockercompose/docker-compose.yml > docker-compose.yml
docker-compose --compatibility up
```
**NOTE :** Here you need to choose arm64 / amd64 according to the architecture used by our environment, otherwise you may encounter errors.
### 2.4 - Running from Kubernetes
*soon*

## 3 - Dummy Traffics
This part has been added to the developed application to generate dummy traffic. Many API Security products need clean traffic from the application before they receive attack traffic. After making sure that the project is working as expected, you can use the existing container images through the following topics or update the dummy_data.py script to generate new images.

### 3.1 - Available Side
Here, you can use the image to create clean traffic by choosing the one in an architecture compatible with the computer you are using, giving the following parameters.
```
- docker run erdemstar/vulnerableapp4apisecurity:dummy-amd64 <http://app-url:port> <count>

- docker run erdemstar/vulnerableapp4apisecurity:dummy-arm64 <http://app-url:port> <count>  
```

To keep the script simple, the multithreading structure was not used. You can meet this need by containerizing the application and creating more than one container from the same image.  :)

### 3.2 - Updatable Side
If you want to see and edit the existing script, you can see the script and the libraries it uses via the path shared below.
```
cd /VulnerableApp4APISecurity/Resource/Dummy/
```

**NOTE :** When the script structure is examined, it creates random "User" authorized users on the application and sends requests to their endpoints. After every 5 requests, it sends a request using the email : "erdem@star.com" password: "erdem" information with Admin authority. Before running the script, do not forget to create a user with the above email and password information whose Role is "Admin" on the application.

## 4 - API Details Documentations 
This section has been created to explain what kind of attacks can be made using the endpoints of the application after clean traffic is sent on the application.

### 4.1- Postman Collection
Postman collection was prepared to see the endpoints of the application and to trigger the vulnerability corresponding to OWASP API Security Top 10 by using these endpoints. You can follow the path below to reach the created collection.
```
cd /VulnerableApp4APISecurity/Resource/Postman
 ```
### 4.2- Excel of API Details 
This section provides information about the endpoints available on the application and the OWASP API Security Top 10 vulnerabilities category that these endpoints have.
```
cd /VulnerableApp4APISecurity/Resource/Vulnerabilityinfo
 ```
## 5 - API Security Presentation
I created a presentation on OWASP API Security 2019 Top 10 at a Webinar I was invited to last quarter in 2022. If you wish, you can access this presentation via the path below.
```
cd /VulnerableApp4APISecurity/Resource/Presentation
 ```