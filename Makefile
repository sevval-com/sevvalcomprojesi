ROOT_DIR := $(abspath .)

.PHONY: web backend dev all

#usage frontend: make web
web:
	cd $(ROOT_DIR)/Src/Presentation/Sevval.Web && dotnet watch run
#usage api: make backend
backend:
	cd $(ROOT_DIR)/Src/Presentation/Sevval.Api && dotnet watch run

# Usage: make dev
dev:
	$(MAKE) -j 2 web backend

# Usage: make all
all: dev
