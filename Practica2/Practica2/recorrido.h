#ifndef RECORRIDO_H
#define RECORRIDO_H
#include "QString"
#include "simbolo.h"
#include "nodoast.h"
#include "entorno.h"
#include "gerror.h"

class Recorrido
{
public:
    Recorrido();
    Simbolo recorrer(NodoAst *raiz, Entorno *h);
    QString resultado;
    QString errores;

};

#endif // RECORRIDO_H
