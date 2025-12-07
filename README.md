# Nombre del Proyecto
**Versión:** 1.0.0 (Inicial)

## Descripción
Breve descripción del sistema que explique su propósito y alcance. (Ej: Sistema de gestión de inventarios para X...).

## Objetivo General
Describir el objetivo general del sistema...

## Equipo de Trabajo
* **Scrum Master:** Dylan Cuaces
* **Backend Developer:** Riofrio Patrick
* **Frontend Developer:** Ponce Melisa

CI/CD Pipeline
Estado del Pipeline
Pendiente de configuración de runners
Este proyecto incluye un pipeline de CI/CD configurado en .gitlab-ci.yml que automatiza:

Validación de estructura del proyecto
Build - Compilación del proyecto ASP.NET
Test - Ejecución de pruebas unitarias y de integración
Deploy - Preparación para despliegue a staging/producción

Etapas del Pipeline
yamlstages:
  - validate    # Validación de estructura
  - build       # Compilación (dotnet build)
  - test        # Pruebas (dotnet test)
  - deploy      # Despliegue (manual)
Requisitos para ejecución

GitLab Runner configurado con .NET SDK 8.0
Permisos de ejecución en el proyecto

Nota
El pipeline está configurado pero requiere que se habiliten runners en la instancia de GitLab UPEC para su ejecución.
Tecnologías

ASP.NET Core


**Cultura DevOps**
Este proyecto adopta prácticas DevOps mediante:

Control de versiones con Git/GitLab
Ramas de trabajo por rol (backend/frontend)
Pipeline CI/CD automatizado
Integración y entrega continua
Trabajo colaborativo y ágil con Scrum
