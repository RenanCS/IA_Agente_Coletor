APPPATH=aplication_csharp_ia

ifdef SYSTEMROOT
	CC-csc.exe
else
	CC=mcs -pkg:dotnet $(APPPATH)/*.cs
endif

OUT=-out:$(APPPATH)/bin/Debug/application_csharp_ia.exe

all:
	$(CC) $(APPPATH)/Program.cs $(OUT)