#ifndef VARIABLE_H
#define VARIABLE_H
#include <QString>
#include "arreglo.h"

class Variable
{
public:
    Variable();
    Variable(int tipo, int fila, int columna, QString valor,QString id);
    Variable(int tipo, int fila, int columna, QString id);
    Variable(int tipo,int fila,int columna,QString id,Arreglo arr);

    int tipo;
    int fila;
    int columna;
    QString valor;
    bool def;
    bool inst;
    void modificar(QString valor);
    QString id;
    Arreglo arr;
};

#endif // VARIABLE_H
