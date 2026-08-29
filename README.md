# Electro API — Cloud-Native DevOps Platform

- <img width="1513" height="972" alt="image" src="https://github.com/user-attachments/assets/70b6fb48-3afe-45e0-8410-e817e4af6049" />


## Overview

**Electro API** is a production-style .NET 8 e-commerce backend that I used as the foundation for building and practicing a complete **Cloud-Native DevOps platform**.

The main goal of this project is not only to develop and run an API, but to build the infrastructure and automation required to **test, containerize, deploy, monitor, and expose the application through Kubernetes**.

The project progressively integrates modern DevOps and cloud-native technologies, creating a complete workflow from source code to a running and observable application.

### Architecture

```text
Developer
   │
   ▼
GitHub Repository
   │
   ├── CI/CD
   │     ├── .NET Restore
   │     ├── Build
   │     ├── Tests
   │     ├── Helm Lint
   │     └── Docker Build & Push
   │
   ▼
Docker Hub
   │
   ▼
Argo CD
   │
   ▼
Kubernetes Cluster
   │
   ├── Electro API
   │     └── .NET 8
   │
   ├── MariaDB
   │
   ├── Helm
   │
   ├── HPA
   │
   └── NetworkPolicies
   │
   ├───────────────┐
   ▼               ▼
Prometheus       Grafana
   │               │
   └───────┬───────┘
           ▼
      Observability
           
Internet
   │
   ▼
Cloudflare Tunnel
   │
   ▼
Traefik LoadBalancer
   │
   ▼
Kubernetes IngressRoute
   │
   ▼
Electro API Service
   │
   ▼
Electro API Pods
```

## Technologies Used

### Application

* .NET 8 / ASP.NET Core
* Entity Framework Core
* MariaDB
* Prometheus metrics

### Containerization

* Docker
* Docker Buildx
* Multi-platform images (`amd64` / `arm64`)
* Docker Hub

### Kubernetes

* Kubernetes
* Kind
* Cloud Provider Kind
* Helm
* Services
* ConfigMaps
* Secrets
* StatefulSets
* Deployments
* HPA
* PodDisruptionBudget
* RBAC
* NetworkPolicies

### CI/CD

* GitHub Actions
* Automated .NET testing
* Helm validation and template rendering
* Docker image building
* Multi-platform Docker image publishing
* Versioned Docker images

### GitOps

* Argo CD
* Git-based Kubernetes deployments
* Helm-based application deployment
* Continuous synchronization between Git and Kubernetes

### Monitoring

* Prometheus
* Grafana
* Prometheus Operator
* ServiceMonitor
* Custom application metrics
* Application performance metrics

The application exposes custom metrics such as:

* Created users
* Created orders
* Processed payments
* Login attempts
* Login failures
* Current users
* Current active orders
* Order creation duration
* Payment processing duration

### Networking & Ingress

* Traefik
* Kubernetes IngressRoute
* LoadBalancer Service
* Cloud Provider Kind
* Cloudflare Tunnel

This allows the locally running Kubernetes application to be exposed externally without requiring a traditional cloud load balancer.

## CI/CD Pipeline

The GitHub Actions pipeline follows this general flow:

```text
Git Push
   │
   ▼
.NET Restore
   │
   ▼
Build
   │
   ▼
Tests
   │
   ▼
Helm Lint
   │
   ▼
Helm Template Validation
   │
   ▼
Docker Buildx
   │
   ▼
Multi-platform Image
   │
   ▼
Docker Hub
   │
   ▼
Argo CD
   │
   ▼
Kubernetes
```

The pipeline also generates versioned Docker image tags. Tagged releases such as:

```text
v1.2.0
```

produce an image such as:

```text
electro-app:1.2.0
```

while normal pushes to `main` can receive automatically generated development versions.

## Kubernetes Deployment

The application is packaged as a Helm chart to avoid maintaining large numbers of independent Kubernetes manifests.

The Helm chart contains resources for:

* Electro API Deployment
* Electro API Service
* MariaDB StatefulSet
* MariaDB Service
* ConfigMap
* Secret
* HPA
* PDB
* RBAC
* ServiceMonitor
* NetworkPolicies

This makes the application reproducible and easier to deploy across different Kubernetes environments.

## Observability

Prometheus collects metrics from the Electro API through a Kubernetes `ServiceMonitor`.

Grafana is connected to Prometheus and provides dashboards for visualizing application and infrastructure metrics.

The project therefore moves beyond simply checking whether the application is running and provides visibility into application behavior and performance.

## GitOps with Argo CD

Argo CD is responsible for continuously deploying the Kubernetes configuration stored in Git.

The Git repository acts as the **source of truth** for the Kubernetes application.

```text
Git Repository
      │
      ▼
   Argo CD
      │
      ▼
Kubernetes Cluster
```

This allows changes to the Helm chart and Kubernetes configuration to be tracked through Git and synchronized automatically with the cluster.

## Local Kubernetes Infrastructure

The project uses **Kind** to create a local Kubernetes cluster for development and experimentation.

Cloud Provider Kind is used to provide `LoadBalancer` functionality inside the local Kind environment.

Traefik then acts as the ingress/load-balancing layer for the application.

For external access during development, Cloudflare Tunnel can forward public HTTPS traffic to the local Traefik LoadBalancer.

## Project Goals

This project was built to gain practical experience with the complete DevOps lifecycle:

1. Develop the application
2. Containerize it with Docker
3. Create a Kubernetes deployment
4. Package Kubernetes resources with Helm
5. Implement Kubernetes networking and security
6. Build automated CI pipelines
7. Publish versioned container images
8. Deploy using GitOps with Argo CD
9. Monitor the application with Prometheus
10. Visualize metrics using Grafana
11. Expose the application through Traefik
12. Experiment with public access using Cloudflare Tunnel

The project is designed as a **hands-on DevOps/Cloud-Native laboratory**, combining application development, Linux, containers, Kubernetes, networking, CI/CD, GitOps, and observability into one system.

## Future Improvements

Planned additions include:

* Loki for centralized log aggregation
* Promtail for log collection
* OpenTelemetry
* Distributed tracing with Tempo
* Advanced Grafana dashboards
* Alertmanager
* More advanced Kubernetes security
* Production cloud deployment
* Automated deployment promotion between environments
* Improved GitOps release strategies
