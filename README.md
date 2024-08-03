# WeatherMicroservice

WeatherMicroservice is a .NET 8 microservice for fetching and storing weather data from weather stations.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Setup](#setup)
- [Building the Project](#building-the-project)
- [Running the Application](#running-the-application)
- [Running with Docker](#running-with-docker)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (optional, for running with Docker)

## Setup

1. **Clone the repository:**

    ```sh
    git clone https://github.com/your-username/WeatherMicroservice.git
    cd WeatherMicroservice
    ```

2. **Restore dependencies:**

    ```sh
    dotnet restore
    ```

## Building the Project

1. **Build the project:**

    ```sh
    dotnet build
    ```

## Running the Application

1. **Apply Entity Framework Core migrations:**

    ```sh
    dotnet ef database update -p WeatherMicroservice.Infrastructure -s WeatherMicroservice.Api
    ```

2. **Run the application:**

    ```sh
    dotnet run --project WeatherMicroservice.Api
    ```

3. **Access the Swagger UI:**

    Open your browser and navigate to `http://localhost:5122/swagger` to view and interact with the API documentation.

## Running with Docker

1. **Build the Docker image:**

    ```sh
    docker build -t weather-microservice .
    ```

2. **Run the Docker container:**

    ```sh
    docker run -d -p 5122:80 --name weather-microservice-container weather-microservice
    ```

3. **Access the Swagger UI:**

    Open your browser and navigate to `http://localhost:5122` to view and interact with the API documentation.
