YEAR := 
DAY :=
BUILD_CONFIG = -c Release
RUN_CONFIG = $(BUILD_CONFIG) --project .\AdventOfCode --
RUN = dotnet run $(RUN_CONFIG)

.DEFAULT_GOAL = run

.PHONY: build
build:
	dotnet build $(BUILD_CONFIG)

.PHONY: run
run:
	$(RUN)

.PHONY: day
day:
	$(RUN) $(DAY)

.PHONY: all
all:
	$(RUN) --all

.PHONY: year
year:
	$(RUN) --year $(YEAR)

.PHONY: year-day
year-day:
	$(RUN) --year $(YEAR) $(DAY)

.PHONY: year-all
year-all:
	$(RUN) --year $(YEAR) --all

.PHONY: test
test:
	dotnet test $(BUILD_CONFIG) -l "console;verbosity=detailed"

.PHONY: clean
clean:
	dotnet clean
	dotnet clean -c Release