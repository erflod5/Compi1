#ifndef GERROR_H
#define GERROR_H
#include <QString>

class gError
{
public:
    gError();
    gError(int tipo, int columna, int fila, QString dato);
    int tipo;
    int columna;
    int fila;
    QString dato;
};

#endif // GERROR_H
