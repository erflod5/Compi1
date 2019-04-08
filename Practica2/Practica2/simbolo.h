#ifndef SIMBOLO_H
#define SIMBOLO_H
#include <QString>

class Simbolo
{
public:
    Simbolo();
    Simbolo(int row, int column, int type,QString value);
    int column;
    int row;
    int type;
    QString value;
};

#endif // SIMBOLO_H
