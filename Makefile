APPPATH=aplication_csharp_ia
CC=mcs -pkg:dotnet $(APPPATH)/*.cs
OUT=$(APPPATH)/bin/Debug/application_csharp_ia.exe

.DEFAULT_GOAL := info
.PHONY: info
default: info;

info:
	@echo "Trabalho de IA - Agente Coletor de Lixo"
	@echo "Execute 'make compile' para compilar"
	@echo "Para executar o agente, use:"
	@echo "make run n=10 t=20 c=10 l=3 r=3"
	@echo "Onde:"
	@echo "n : tamanho do mapa"
	@echo "t : capacidade máxima de lixo coletado"
	@echo "c : capacidade máxima de recarga"
	@echo "l : quantidade de lixeiras"
	@echo "r : quantidade de recargas"
	@echo ""
	@echo "Caso queira os valores padrões, execute make run_def"
	@echo ""

run_def:
	./$(OUT)

run:
	./$(OUT) n=$(n) t=$(t) c=$(c) l=$(l) r=$(r)

compile:
	$(CC) $(APPPATH)/Program.cs -out:$(OUT)
