#include "arreglo.h"

Arreglo::Arreglo()
{

}

Arreglo::Arreglo(int tipo, int tam){
    this->i = tam; this->j = this->k = 0;
    switch (tipo) {
    case 11:{
        int_a = new int[tam];
        this->tipo = 11;
    }
        break;
    case 12:{
        string_a = new  QString[tam];
        this->tipo = 12;
    }
        break;
    case 13:{
        char_a = new QChar[tam];
        this->tipo = 13;
    }
        break;
    case 14:{
    bool_a = new bool[tam];
    this->tipo = 14;
    }
        break;
    case 15:
    { float_a = new float[tam];
        this->tipo = 15;
    }
        break;
    default:
        break;
    }
}

Arreglo::Arreglo(int tipo, int filas, int columnas){
    this->i = filas; this->j = columnas; this->k = 0;
    switch (tipo) {
    case 21:{
        int_aa = new int*[filas];
        for (int i = 0; i < filas; ++i)
            int_aa[i] = new int[columnas];
        this->tipo = 21;
    }
        break;
    case 22:{
        string_aa = new QString*[filas];
        for (int i = 0; i < filas; ++i)
            string_aa[i] = new QString[columnas];
        this->tipo = 22;
    }
        break;
    case 23:{
        char_aa = new QChar*[filas];
        for (int i = 0; i < filas; ++i)
            char_aa[i] = new QChar[columnas];
        this->tipo = 23;
    }
        break;
    case 24:{
        bool_aa = new bool*[filas];
        for (int i = 0; i < filas; ++i)
            bool_aa[i] = new bool[columnas];
        this->tipo = 24;
    }
        break;
    case 25:
    { float_aa = new float*[filas];
        for (int i = 0; i < filas; ++i)
            float_aa[i] = new float[columnas];
        this->tipo = 25;
    }
        break;
    default:
        break;
    }
}

Arreglo::Arreglo(int tipo, int filas, int columnas, int profundidad){
    this->i = filas; this->j = columnas; this->k = profundidad;
    switch (tipo) {
    case 31:{
        int_aaa = new int**[filas];
        for (int i = 0; i < filas; ++i){
            int_aaa[i] = new int*[columnas];
           for (int j = 0; j < columnas; ++j)
                int_aaa[i][j] = new int[profundidad];
        }
        this->tipo = 31;
    }
        break;
    case 32:{
        string_aaa = new QString**[filas];
        for (int i = 0; i < filas; ++i){
            string_aaa[i] = new QString*[columnas];
           for (int j = 0; j < columnas; ++j)
                string_aaa[i][j] = new QString[profundidad];
        }
        this->tipo = 32;
    }
        break;
    case 33:{
        char_aaa = new QChar**[filas];
        for (int i = 0; i < filas; ++i){
            char_aaa[i] = new QChar*[columnas];
           for (int j = 0; j < columnas; ++j)
                char_aaa[i][j] = new QChar[profundidad];
        }
        this->tipo = 33;
    }
        break;
    case 34:{
        bool_aaa = new bool**[filas];
        for (int i = 0; i < filas; ++i){
            bool_aaa[i] = new bool*[columnas];
           for (int j = 0; j < columnas; ++j)
                bool_aaa[i][j] = new bool[profundidad];
        }
        this->tipo = 34;
    }
        break;
    case 35:
    { float_aaa = new float**[filas];
        for (int i = 0; i < filas; ++i){
            float_aaa[i] = new float*[columnas];
           for (int j = 0; j < columnas; ++j)
                float_aaa[i][j] = new float[profundidad];
        }
        this->tipo = 35;
    }
        break;
    default:
        break;
    }
}
