#ifndef ARREGLO_H
#define ARREGLO_H
#include <QString>

class Arreglo
{
public:
    Arreglo();

    Arreglo(int tipo,int tam);
    Arreglo(int tipo, int filas, int columnas);
    Arreglo(int tipo, int filas, int columnas, int profundidad);

    int *int_a;
    int **int_aa;
    int ***int_aaa;
    QString *string_a;
    QString **string_aa;
    QString ***string_aaa;
    bool *bool_a;
    bool **bool_aa;
    bool ***bool_aaa;
    float *float_a;
    float **float_aa;
    float ***float_aaa;
    QChar *char_a;
    QChar **char_aa;
    QChar ***char_aaa;
    int tipo;
    int i,j,k;
};

#endif // ARREGLO_H
