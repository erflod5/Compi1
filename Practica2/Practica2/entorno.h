#ifndef ENTORNO_H
#define ENTORNO_H
#include <QString>
#include "qlist.h"
#include <QHash>
#include "variable.h"

class Entorno
{
public:
    Entorno();
    Entorno(Entorno *anterior);
    Entorno *anterior;
    QHash<QString,Variable> tablaSim;
    Variable getVariable(QString id);
    bool addVariable(QString id, Variable b);
    bool change(QString id,Variable b);
};

#endif // ENTORNO_H
