MONO := mono
MBASIC := $(MONO) ../bin/mbasic.exe

.PHONY: all
all: printTest readTest dataTest

.PHONY: printTest
printTest: print.mbas testPrint.py
	$(MBASIC) print.mbas
	$(MONO) print.exe | ./testPrint.py

.PHONY: readTest
readTest: read.mbas testRead.py
	$(MBASIC) read.mbas
	$(MONO) read.exe | ./testRead.py

.PHONY: dataTest
dataTest: data.mbas testData.py
	$(MBASIC) data.mbas
	$(MONO) data.exe | ./testData.py
