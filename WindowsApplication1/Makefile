sources := TiCanvas.dll.sources
MCS := gmcs
OUTPUTDIR := ../bin

.PHONY: all
all: $(sources)
	$(MCS) -debug -target:library -resource:Form1.resx -r:System.Drawing -r:System.Windows.Forms -r:System.Data -r:$(OUTPUTDIR)/TiBasicRuntime.dll -out:$(OUTPUTDIR)/TiCanvas.dll @$< 
