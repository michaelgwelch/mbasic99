MONO := mono
MBASIC := $(MONO) ../bin/mbasic.exe

.PHONY: all
all: printTest readTest dataTest gosubTest arrayTest array2Test

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

.PHONY: gosubTest
gosubTest: gosub.mbas testGosub.py
	$(MBASIC) gosub.mbas
	$(MONO) gosub.exe | ./testGosub.py

.PHONY: arrayTest
arrayTest: array.mbas testArray.py
	$(MBASIC) array.mbas
	$(MONO) array.exe | ./testArray.py

.PHONY: array2Test
array2Test: array2.mbas testArray2.py
	$(MBASIC) array2.mbas
	$(MONO) array2.exe | ./testArray2.py
