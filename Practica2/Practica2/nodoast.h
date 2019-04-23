#ifndef NODOAST_H
#define NODOAST_H
#include "QString"
#include "qlist.h"

enum Tipo {ERROR = -1, DEFAULT=0, RID=100, RINT=101, RSTRING=102, RDOUBLE=103, RCHAR=104,RBOOL=105, RARRAY=106,
          RIMPRIMIR = 107, RSHOW = 108, RSI = 109, RSINO=110, RPARA=111,RREPETIR=112, RSINOSI = 113,
          NUMERO = 201, DECIMAL = 202, BOOLEANO = 203, CARACTER = 204, CADENA = 205, DATOMATRIZ = 206, POSARR = 207, VID = 208,
          IGUALLOGICO = 1, DESIGUAL = 2, MAYOR = 3, MAYORQUE =4, MENOR = 5, MENORQUE =6, AND = 7, OR = 8, NOT = 9,MAS=10,MENOS=11,POR=12,DIV=13,
          POTENCIA = 14, AUMENTO = 15, DECREMENTO = 16, IGUAL = 17, LISTID = 113, PRINCIPAL = 666,LID = 667,LARRAY = 668,ASID = 669,ASARR=670,INPARA=671,
          DIM_ARRAY=672, LISTCOLUMN=673, LISTROW = 674, LISTIF=675, LISTEIF = 676, INPARA2=677, LISTPROF =678};

class NodoAst
{
public:
    NodoAst();
    NodoAst(int row, int column, Tipo type, QString value, QString tipo);
    void add(NodoAst n);
    QString value;
    QString tipo;
    int row;
    int column;
    int getType();
    QList<NodoAst> l_hijos;
protected:
    Tipo type;
};

#endif // NODOAST_H
