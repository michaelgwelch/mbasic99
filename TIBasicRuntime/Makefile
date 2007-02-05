sources := TiBasicRuntime.dll.sources
MCS := gmcs
OUTPUTDIR := ../bin

.PHONY: all
all: $(sources)
	$(MCS) -target:library -debug -out:$(OUTPUTDIR)/TiBasicRuntime.dll @$< 
