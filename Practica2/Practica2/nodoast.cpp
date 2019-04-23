#include "nodoast.h"

NodoAst::NodoAst()
{
    type = DEFAULT;
    value = "";
    row = 0;
    column = 0;
    l_hijos = QList<NodoAst>();
}

NodoAst::NodoAst(int row,int column, Tipo type, QString value,QString tipo){
    this->row = row;
    this->column = column;
    this->type = type;
    this->value = value;
    this->tipo = tipo;
    l_hijos = QList<NodoAst>();
    this->value.replace("\"","");
}

void NodoAst::add(NodoAst n){
    this->l_hijos.append(n);
}

int NodoAst::getType(){
    return type;
}
