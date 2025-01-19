# Docker-basert Todo API

Prosjektet inneholder en "stacked" Todo API med følgende komponenter:
- Nginx proxy
- .NET 8 REST API
- MySQL database


## Installer og Kjør

1. Last ned docker-compose.yml:
```bash
curl -O https://raw.githubusercontent.com/lollolololololololol/docker-compose.yml
```

2. Start tjenestene:
```bash
docker-compose up -d
```

## Tilgang til API
- API Dokumentasjon (Swagger): http://127.0.0.1/swagger
- API Endepunkt: http://127.0.0.1/todos
- Database er kun tilgjengelig internt i Docker-nettverket

## API Endepunkter
- `GET /todos` - Hent alle todos
- `GET /todos/{id}` - Hent spesifikk todo
- `POST /todos` - Opprett ny todo

### Eksempel POST
```json
{
    "title": "rm -rf /*"
}
```

## Docker Hub
Images er tilgjengelige på Docker Hub:
- nginx: christophermp/arbeidskrav-nginx
- api: christophermp/arbeidskrav-api
- MySQL: mysql:8.0

## Struktur
```
.
├── docker-compose.yml  # Docker Compose konfigurasjon
├── nginx/              # Nginx konfigurasjon
│   ├── Dockerfile
│   └── nginx.conf
├── Program.cs          # API kildekode
└── README.md           # Denne filen
```

## Debug

Got issues??:

1. Sjekk alle containere:
```bash
docker ps
```

2. Se logger:
```bash
docker-compose logs
```

3. Start på nytt tomt volum:
```bash
docker-compose down -v
docker-compose up -d
```