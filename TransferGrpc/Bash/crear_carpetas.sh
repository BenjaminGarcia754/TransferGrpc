#!/bin/bash

nueva_carpeta="Desarrollo"

if [ -d "$nueva_carpeta" ]; then
	echo "La carpeta '$nueva_carpeta' ya existe. No se puede continuar."
	exit 1
fi

mkdir "$nueva_carpeta"

directorios="lista_de_carpetas.txt"

if [ ! -f "$directorios" ]; then
	echo "El archivo '$directorios' no existe. No se puede continuar."
	exit 1
fi

while IFS= read -r nombre_carpeta
do
	if [ ! -z "$nombre_carpeta" ]; then
		mkdir "$nueva_carpeta/$nombre_carpeta"
		echo "Carpeta creada: $nombre_carpeta"
	fi
done < "$directorios"

echo "Proceso completado. Se han creado las carpetas en '$nueva_carpeta'."
