FROM microsoft/dotnet:2.1-sdk

RUN apt-get update
RUN apt-get install -y unzip libunwind8 gettext
RUN mkdir DeathClock
RUN wget https://github.com/RichTeaMan/DeathClock/archive/netcore.tar.gz -O deathclock.tar.gz
RUN tar -xzf deathclock.tar.gz --strip-components=1 -C DeathClock
WORKDIR /DeathClock
RUN ./cake.sh -target=build
ENTRYPOINT ./cake.sh -target=run

