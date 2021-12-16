YEAR := 
BUILD_CONFIG = -c Release
RUN_CONFIG = $(BUILD_CONFIG) --project .\AdventOfCode --

.DEFAULT_GOAL = run

.PHONY: build
build:
	dotnet build $(BUILD_CONFIG)

.PHONY: run
run:
	dotnet run $(RUN_CONFIG)

.PHONY: all
all:
	dotnet run $(RUN_CONFIG) --all

.PHONY: year
year:
	dotnet run $(RUN_CONFIG) --year $(YEAR)

.PHONY: year-all
year-all:
	dotnet run $(RUN_CONFIG) --year $(YEAR) --all

.PHONY: test
test:
	dotnet test $(BUILD_CONFIG) -l "console;verbosity=detailed"