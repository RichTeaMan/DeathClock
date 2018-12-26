FROM microsoft/dotnet:2.1-sdk

ARG branch=master

RUN apt-get update
RUN apt-get install -y unzip libunwind8 gettext
ARG CACHEBUST=1
RUN git clone --single-branch --recurse-submodules -b $branch https://github.com/RichTeaMan/DeathClock.git
WORKDIR /DeathClock
RUN ./cake.sh -target=Test
ENTRYPOINT ./cake.sh -target=Run-All -outputDirectory=/var/deathclock/ -cacheDirectory=/var/deathclockcache/
