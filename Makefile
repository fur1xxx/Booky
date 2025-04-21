COMPOSE_FILE=docker-compose.yml

.PHONY: all
all: build up

.PHONY: build
build:
	@echo "Building Docker images from Dockerfile..."
	docker-compose -f $(COMPOSE_FILE) build

.PHONY: up
up:
	@echo "Starting BookyWeb and SQL Server containers..."
	docker-compose -f $(COMPOSE_FILE) up -d
	@echo "Application will be available at: http://localhost:8080"
	@echo "SQL Server will be accessible at: localhost:1433"
	@echo "To stop containers, run: make down"

.PHONY: down
down:
	@echo "Stopping and removing containers..."
	docker-compose -f $(COMPOSE_FILE) down
	
.PHONY: run
run:
	@echo "Building and starting the application..."
	$(MAKE) build
	$(MAKE) up

.PHONY: restart
restart:
	@echo "Restarting containers..."
	docker-compose -f $(COMPOSE_FILE) down
	docker-compose -f $(COMPOSE_FILE) build
	docker-compose -f $(COMPOSE_FILE) up -d
	@echo "Services restarted."

.PHONY: logs
logs:
	@echo "Showing logs (press Ctrl+C to exit)..."
	docker-compose -f $(COMPOSE_FILE) logs -f

.PHONY: clean
clean:
	@echo "Removing containers, volumes, and networks..."
	docker-compose -f $(COMPOSE_FILE) down -v --remove-orphans
	@echo "Cleanup complete."

.PHONY: bash
bash:
	@echo "Opening interactive shell in 'bookyweb' container..."
	docker exec -it bookyweb /bin/bash
