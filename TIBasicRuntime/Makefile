sources := TIBasicRuntime.dll.sources
MCS := gmcs
OUTPUTDIR := ../bin

.PHONY: all
all: $(sources)
	$(MCS) -target:library -debug -out:$(OUTPUTDIR)/TIBasicRuntime.dll @$< 
