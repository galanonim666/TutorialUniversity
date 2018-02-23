build image (cd to directory with Dockerfile first):

    docker build . -t tutorial_university

create network:

    docker network create tutorial_test

run postgres db

    docker run --name some-postgres -p 5432:5432 -e POSTGRES_PASSWORD=qwe123 -d --network=tutorial_test postgres

run aspnetcore_app (with proper connection string, host is the name of postgres container running)

    docker run -d -p 8080:80 --name aspnetcore_app -e ASPNETCORE_ENVIRONMENT=Development -e ConnectionStrings__DefaultConnection="User ID=postgres;Password=qwe123;Host=some-postgres;Port=5432;Database=TutorialUniversity3;Pooling=true;" --network=tutorial_test tutorial_university


Running migration on database:
1. first cd to project directory

First option:

 docker run -it --rm -v "$(pwd):/app" --workdir /app --network=tutorial_test -e ConnectionStrings__DefaultConnection="User ID=postgres;Password=qwe123;Host=some-postgres;Port=5432;Database=TutorialUniversity3;Pooling=true;" microsoft/aspnetcore-build /bin/bash -c "dotnet restore && dotnet ef database update"

Second option
2. run image with build tools

        docker run -itd -v "$(pwd):/app" --workdir /app --network=tutorial_test --name=aspnetcore_build microsoft/aspnetcore-build /bin/bash
        docker run -itd -v "$(pwd):/app" --workdir /app --network=tutorial_test --name=aspnetcore_build -e ConnectionStrings__DefaultConnection="User ID=postgres;Password=qwe123;Host=some-postgres;Port=5432;Database=TutorialUniversity3;Pooling=true;"  microsoft/aspnetcore-build /bin/bash

3. attach to it

        docker attach aspnetcore_build


    inside execute this commands:

    3.1. restore packages

        dotnet restore

    3.2 change connection string if you have different host in appsettings

        export ConnectionStrings__DefaultConnection="User ID=postgres;Password=qwe123;Host=some-postgres;Port=5432;Database=TutorialUniversity3;Pooling=true;"
    
    3.3 run migrations (create database)

        dotnet ef database update


Optional: How to connect with `psql` command:
Run this command:

    docker run -it --rm --network=tutorial_test postgres psql -h some-postgres -U postgres

enter password and inside you can do 

    \list

to list database

    \connect TutorialUniversity3

to connect to db (if it is created by ef migrations)

    \dt

to list tables in database...



---
How to run pgadmin4 to manage postgres database:

1. run this command:

    docker run -p 8081:80 --name pgadmin4_local --link some-postgres:db -e "PGADMIN_DEFAULT_EMAIL=galanonim.qwe123@gmail.com" -e "PGADMIN_DEFAULT_PASSWORD=qwe123" -d dpage/pgadmin4

or this if you use docker network

	docker run -p 8081:80 --name pgadmin4_local --link some-postgres:db --network=tutorial_test -e "PGADMIN_DEFAULT_EMAIL=galanonim.qwe123@gmail.com" -e "PGADMIN_DEFAULT_PASSWORD=qwe123" -d dpage/pgadmin4

2. open http://localhost:8081 and enter provided email and password

3. Click "Add server", write some name and put the adress of your your database. It's "db" (not "localhost" because this is "localhost" inside container, you need to provide ip of container running database or hostname provided in command 1. where you did container linking (you did "some-postgres:db" so host file inside contaner was modified and "db" name points to ip address of "some-postgress" container, which is you database)). Use username and password provided when you created container that runs database (postgres:qwe123 in this case)