#include "gerror.h"

gError::gError()
{

}

gError::gError(int tipo, int columna, int fila, QString dato){
    this->tipo = tipo;
    this->columna = columna;
    this->fila = fila;
    this->dato = dato;
}
