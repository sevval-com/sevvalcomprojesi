ROOT_DIR := $(abspath .)

.PHONY: web backend

#usage frontend: make web
web:
	cd $(ROOT_DIR)/Src/Presentation/Sevval.Web && dotnet watch run
#usage api: make backend
backend:
	cd $(ROOT_DIR)/Src/Presentation/Sevval.Api && dotnet watch run
