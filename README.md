# Weather Microservice

A .NET 8 microservice for fetching, storing, and querying weather measurements from specified weather stations.

## Table of Contents

- [Introduction](#introduction)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running with Docker](#running-with-docker)
  - [Running the Application](#running-the-application)
- [Testing](#testing)

## Introduction

This microservice retrieves weather data from specific stations and stores it in a database. You can query the data to get various measurements.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (optional)

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/weather-microservice.git
    cd weather-microservice
    ```

2. Restore dependencies:
    ```sh
    dotnet restore
    ```

### Running with Docker

1. Build the Docker image:
    ```sh
    docker build -t weather-microservice .
    ```

2. Run the Docker container:
    ```sh
    docker run -d -p 8080:80 --name weather-microservice-container weather-microservice
    ```

3. Access the Swagger UI:
    `http://localhost:8080`

### Running the Application

#### Using .NET CLI

1. Build the application:
    ```sh
    dotnet build
    ```

2. Run the application:
    ```sh
    dotnet run --project WeatherMicroservice.Api
    ```

3. Access the Swagger UI:
    `http://localhost:5122/swagger`


## Testing

Run unit tests:
```sh
dotnet test
