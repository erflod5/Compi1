#ifndef GRAFICADOR_H
#define GRAFICADOR_H
#include "nodoast.h"
#include <fstream>
#include <iostream>

class Graficador
{
public:
    Graficador();
    Graficador(NodoAst *raiz);
    NodoAst *raiz;
    int count;
    QString dot;
    QString graficar();
    void recorrer(QString padre, NodoAst *hijo);
    void generarImagen();
    QString escapar(QString cadena);

};

#endif // GRAFICADOR_H
