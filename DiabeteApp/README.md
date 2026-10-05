# -- Application DiabeteApp --

DiabeteApp est une application permettant d'aider les praticiens à détecter le diabète de type 2 chez leurs patients.
Grâce à elle, chaque praticien autorisé peut :
- ajouter un patient à la base de données,
- modifier les informations personnelles d'un patient,
- ajouter une note pour un patient,
- voir la fiche d'un patient regroupant :
	- ses informations personnelles,
	- les notes renseignées par ses praticiens
	- et l'analyse du risque de développer le diabète de type 2.



# Installation et lancement de l'application

## 1. Installer Docker :
Cette application a été dockérisée et nécessite donc l'installation de Docker sur votre machine.
https://docs.docker.com/get-started/get-docker/


## 2. Cloner le dépôt GitHub du projet :
git clone https://github.com/S-Marnat/OC-Projet10

## 3. Personnaliser les informations confidentielles (optionnel) :
Cette application contient un fichier "docker-compose.override.yml" qui contient des informations confidentielles :
	- mot de passe de la base de données SQL Server ("SA_PASSWORD", "ConnectionStrings__DefaultConnection"),
	- mot de passe de la base de données MongoDB ("MONGO_INITDB_ROOT_PASSWORD", "MongoDb__ConnectionString"),
	- clé secrète pour le JWT ("JWT_SECRET_KEY").
Pour plus de sécurité, il est donc nécessaire de personnaliser ce fichier avant de lancer l'application.

Un organisateur par défaut est automatiquement créé à la première exécution de l'application.
Ses identifiants sont :
	- email : organisateur@demo.com
	- mot de passe : Password123!
Ces informations peuvent être modifiées dans le fichier "Program.cs" du projet "Patients", section "Configuration d'un organisateur par défaut".
Seuls l'email, l'userName et le mot de passe peuvent être modifiés.
Le rôle "Organisateur" ne doit pas être changé.


## 4. Lancer l'application :
Ouvrir un terminal à la racine du projet et exécuter la commande suivante :
docker compose up --build

Une fois l'application lancée, vous pouvez y accéder via votre navigateur à l'adresse suivante :
http://localhost:5000

L'application contient déjà des données de test pour les patients et les notes.



# Architecture de l'application
Cette application est composée de 5 projets principaux :
	- un microservice "Patients" qui gère les patients et leurs informations personnelles,
	- un microservice "Notes" qui gère les notes des praticiens pour chaque patient,
	- un microservice "Risques" qui analyse le risque de développer le diabète de type 2 pour chaque patient,
	- un micro-frontend "Front" qui représente l'interface de l'application,
	- un microservice "Gateway" qui sert de passerelle entre les microservices et le micro-frontend.

Elle contient également 3 projets de tests :
	- un projet de tests "Patients.Tests" pour le microservice "Patients",
	- un projet de tests "Notes.Tests" pour le microservice "Notes",
	- un projet de tests "Risques.Tests" pour le microservice "Risques".
Chacun de ces projets contient des tests unitaires pour les services et les contrôleurs de chaque microservice.


## Microservice "Patients"
Le microservice "Patients" est un projet ASP.NET Core Web API 10.0.

Il utilise :
	- une base de données SQL Server pour stocker les informations personnelles des patients,
	- Entity Framework Core pour gérer les opérations CRUD sur la base de données,
	- JWT pour l'authentification et l'autorisation des organisateurs,
	- Identity pour gérer les rôles et les organisateurs.

Il se divise de la manière suivante :
	- Controllers : les requêtes HTTP,
	- Data : le contexte de la base de données,
	- DTOs : les objets de transfert de données,
	- Entities : les entités de la base de données,
	- Migrations : les migrations de la base de données,
	- Repositories : l'accès aux données,
	- Services : la logique métier.


## Microservice "Notes"
Le microservice "Notes" est un projet API Web ASP.NET Core 10.0.

Il utilise :
	- une base de données MongoDB pour stocker les notes des praticiens pour chaque patient,
	- le driver officiel MongoDB pour .NET pour gérer les opérations CRUD sur la base de données,
	- JWT pour l'authentification et l'autorisation des organisateurs.

Il se divise de la manière suivante :
	- Controllers : les requêtes HTTP,
	- Data : les configurations de la base de données,
	- DTOs : les objets de transfert de données,
	- Entities : les entités de la base de données,
	- Repositories : l'accès aux données,
	- Services : la logique métier.


## Microservice "Risques"
Le microservice "Risques" est un projet API Web ASP.NET Core 10.0.

Il utilise :
	- JWT pour l'authentification et l'autorisation des organisateurs.

Il se divise de la manière suivante :
	- Controllers : les requêtes HTTP,
	- DTOs : les objets de transfert de données,
	- Services : la logique métier.


## Micro-frontend "Front"
Le micro-frontend "Front" est un projet Application Web ASP.NET Core MVC 10.0.

Il utilise :
	- Bootstrap pour le design de l'interface utilisateur.

Il se divise de la manière suivante :
	- Controllers : les requêtes HTTP,
	- Services : la logique métier,
	- ViewModels : les objets de transfert de données entre les vues et les contrôleurs,
	- Views : les vues Razor de l'application.


## Microservice "Gateway"
Le microservice "Gateway" est un projet ASP.NET Core 10.0.

Il utilise :
	- Ocelot pour gérer les routes vers les microservices,
	- JWT pour l'authentification et l'autorisation des organisateurs.

Il se divise de la manière suivante :
	- ocelot.json : le fichier de configuration d'Ocelot.



# Dockérisation de l'application
L'application contient un fichier "docker-compose.yml" qui permet de lancer l'ensemble des microservices et du micro-frontend dans des conteneurs Docker.

Le fichier "docker-compose.override.yml" permet de configurer les composants sensibles de l'application, notamment :
	- les mots de passe des bases de données SQL Server et MongoDB,
	- la clé secrète pour le JWT,
	- les chaînes de connexion aux bases de données SQL Server et MongoDB,
	- les ports utilisés par les microservices et le micro-frontend.

Chacun des 5 projets principaux contient un fichier "Dockerfile" qui permet de construire l'image Docker correspondante.



# Green Code
Le Green Code vise à minimiser l'impact environnemental d'une application en optimisant son utilisation des ressources (CPU, mémoire, réseau, stockage)
et en réduisant sa consommation d'énergie.
Il s'inscrit dans une démarche de sobriété numérique, qui contribue à :
	- la réduction de l'empreinte carbone des technologies numériques,
	- la prolongation de la durée de vie des équipements informatiques,
	- et la réduction des coûts liés à l'énergie et à l'infrastructure informatique.

L'application DiabeteApp respecte certains de ces principes, notamment :
	- un code sans éléments inutiles, rendant son exécution plus rapide et donc moins gourmande en ressources,
	- une interface utilisateur sobre et sans éléments superflus, permettant un chargement plus rapide des pages et une consommation d'énergie réduite,
	- une architecture en microservices claire, facilitant la maintenance et donc plus sobre en ressources,
	- une utilisation de Docker permettant un déploiement reproductible et évitant les environnements lourds.

Cependant, il existe encore des améliorations possibles pour rendre l'application plus respectueuse de l'environnement, notamment :
	- limiter les redondances dans le code, pour réduire le temps d'exécution et donc la consommation d'énergie,
	- limiter le nombre de requêtes HTTP envoyées par l'application, en utilisant par exemple la mise en cache,
	- optimiser les requêtes SQL et MongoDB, pour réduire le temps d'exécution et donc la consommation d'énergie,
	- utiliser des images Docker plus légères, pour réduire la taille des conteneurs et la consommation de ressources,
	- analyser la consommation énergétique du code via des outils comme SonarQube ou EcoCode, pour détecter les défauts énergétiques.

Enfin, le choix de l'hébergeur pour l'application peut également avoir un impact sur son empreinte carbone.
Il est donc recommandé de choisir un hébergeur :
	- alimenté en énergie renouvelable,
	- engagé dans une démarche de développement durable,
	- situé à proximité pour réduire les distances de transport de données.