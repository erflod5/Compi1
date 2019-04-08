#include "simbolo.h"

/* int = 1, string =2, boolean = 3, double = 4, char = 5, error = -1
*/


Simbolo::Simbolo()
{
    type = -1;
}

Simbolo::Simbolo(int row, int column, int type, QString value){
    this->column = column;
    this->row = row;
    this->type = type;
    this->value = value;
}
