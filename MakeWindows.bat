echo off
echo Trabalho de IA - Agente Coletor de Lixo (A*)
echo Informe os dados solicitados:
echo Tamanho do mapa: 
@set /p n= 
echo Capacidade máxima de lixo coletado:
@set /p t= 
echo Capacidade máxima de recarga:
@set /p c= 
echo Quantidade de lixeiras:
@set /p l= 
echo Quantidade de recargas:
@set /p r=

cls 

cd aplication_csharp_ia\obj\Debug\

echo Processando...

start aplication_csharp_ia.exe n=%n% t=%t% c=%c% l=%l% r=%r%
