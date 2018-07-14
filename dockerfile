FROM microsoft/dotnet:2.1-sdk

ARG branch=master

RUN apt-get update
RUN apt-get install -y unzip libunwind8 gettext
RUN mkdir DeathClock
ADD https://github.com/RichTeaMan/DeathClock/archive/$branch.tar.gz deathclock.tar.gz
RUN tar -xzf deathclock.tar.gz --strip-components=1 -C DeathClock
WORKDIR /DeathClock
RUN ./cake.sh -target=Test
ENTRYPOINT ./cake.sh -target=Run -outputDirectory=/var/deathclock/
