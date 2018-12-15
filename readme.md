# DeathClock

This is a grim project that attempts to predict upcoming deaths of people from their Wikipedia articles.

## Cake Tasks
This project uses [Cake](https://cakebuild.net).
* cake -target=Clean
* cake -target=Build
* cake -target=Test
* cake -target=Run

## Api Keys

This project requires an api key from [The Movie DB](https://www.themoviedb.org/?language=en-US).

Then add it to the secrets of the project with the dotnet CLI from the DeathClock project directory:

```
dotnet user-secrets set TmdbApiKey <api key>
```

## CI

|        | Windows | Linux |
| ------ | --------|-------|
| Master | [![Build status](https://ci.appveyor.com/api/projects/status/upa1q3k9khvvq8jp/branch/master?svg=true)](https://ci.appveyor.com/project/RichTeaMan/deathclock/branch/master) | [![Build Status](https://travis-ci.org/RichTeaMan/DeathClock.svg?branch=master)](https://travis-ci.org/RichTeaMan/DeathClock) |


## Docker

The Deathclock can be summoned from Docker:

```
docker build -t deathclock .

docker volume create deathclock_data
docker run -d -v deathclock_data:/var/deathclock deathclock
```

Insights from the necromicon will be rested in the deathclock_data volume.
