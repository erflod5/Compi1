#include "entorno.h"


Entorno::Entorno()
{
    anterior = NULL;
}

Entorno::Entorno(Entorno *anterior){
    this->anterior = anterior;
}

Variable Entorno::getVariable(QString id){
    Entorno *temp = this;
    while(temp!=NULL){
        if(temp->tablaSim.contains(id)){
            return temp->tablaSim[id];
        }
        temp = temp->anterior;
    }
    Variable b = Variable();
    return b;
}

bool Entorno::addVariable(QString id, Variable b){
    if(tablaSim.contains(id)){
        return false;
    }
    else{
        tablaSim.insert(id,b);
        return true;
    }
}

bool Entorno::change(QString id, Variable b){
    Entorno *temp = this;
    while(temp!=NULL){
        if(temp->tablaSim.contains(id)){
            temp->tablaSim.insert(id,b);
            return true;
        }
        temp = temp->anterior;
    }
    return false;
}
