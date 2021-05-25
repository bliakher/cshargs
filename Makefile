
.PHONY: all build test docs clean distclean

all: build docs

build:
	dotnet build

clean:
	dotnet clean

distclean:
	rm -rf obj
	rm -rf bin
	rm -rf html
	rm -rf latex

docs:
	doxygen doxygen.conf

test:
    dotnet test