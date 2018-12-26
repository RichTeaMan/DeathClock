FROM microsoft/dotnet:2.1-sdk

ARG branch=master

RUN apt-get update
RUN apt-get install -y unzip libunwind8 gettext
RUN mkdir DeathClock
ADD https://github.com/RichTeaMan/DeathClock/archive/$branch.tar.gz deathclock.tar.gz
RUN tar -xzf deathclock.tar.gz --strip-components=1 -C DeathClock
WORKDIR /DeathClock
ENTRYPOINT ["./cake.sh", "-target=Run-All", "-outputDirectory=/var/deathclock/", "-cacheDirectory=/var/deathclockcache/"]
CMD []
