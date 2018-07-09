# DeathClock

This is a grim project that attempts to predict upcoming deaths of people from their Wikipedia articles.

## Cake Tasks
This project uses [Cake](https://cakebuild.net).
* cake -target=Clean
* cake -target=Build
* cake -target=Test
* cake -target=Run

## Docker

The Deathclock can be summoned from Docker:

```
docker build -t deathclock .

docker volume create deathclock_data
docker run -d -v deathclock_data:/var/deathclock deathclock
```

Insights from the necromicon will be rested in the deathclock_data volume.
