#include "variable.h"
/* int = 1, string =2, boolean = 3, double = 4, char = 5, error = -1
 */

Variable::Variable()
{
    this->columna = -1;
    this->fila = -1;
    this->tipo = -1;
    this->valor = "null";
    def = false;
    inst = false;
}

Variable::Variable(int tipo, int fila, int columna, QString valor,QString id){
    this->tipo = tipo;
    this->fila = fila;
    this->columna = columna;
    this->valor = valor;
    this->id = id;
    def = true;
    inst = true;
}

Variable::Variable(int tipo, int fila, int columna, QString id){
    this->tipo = tipo;
    this->fila = fila;
    this->columna = columna;
    this->valor = "null";
    def = true;
    inst = false;
    this->id = id;
}

Variable::Variable(int tipo, int fila, int columna, QString id, Arreglo arr){
    this->tipo=tipo;
    this->fila =fila;
    this->columna = columna;
    this->id = id;
    this->arr = arr;
}

void Variable::modificar(QString valor){
    this->valor = valor;
}


