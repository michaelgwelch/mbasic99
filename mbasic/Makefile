sources := mbasic.exe.sources
MCS := gmcs
OUTPUTDIR := ../bin

.PHONY: all
all: $(sources)
	$(MCS) -debug -r:$(OUTPUTDIR)/TiBasicRuntime.dll -out:$(OUTPUTDIR)/mbasic.exe @$< 
