#include "recorrido.h"
#include <cmath>
#include <iostream>
#include <QMessageBox>
extern QList<gError> lerror;

Recorrido::Recorrido()
{
    resultado = "";
}

Simbolo Recorrido::recorrer(NodoAst *raiz, Entorno *h){
    Simbolo s = Simbolo();
    s.row = raiz->row;
    s.column = raiz->column;
    switch (raiz->getType()) {
        case PRINCIPAL:{
            if(raiz != NULL){
                foreach (NodoAst hijo, raiz->l_hijos) {
                    recorrer(&hijo,h);
                }
            }
            break;
        }

        case NUMERO:{
          if(raiz->l_hijos.count()==1){
              NodoAst left = raiz->l_hijos.at(0);
              if(left.getType()==AUMENTO){
                s.type = NUMERO;
                s.value = QString::number(raiz->value.toInt() + 1);
              }
              else if(left.getType() == DECREMENTO){
                s.type = NUMERO;
                s.value = QString::number(raiz->value.toInt() - 1);
              }
          }

          else{
            s.type = NUMERO;
            s.value = raiz->value;
          }
          break;}
        case DECIMAL:{
          if(raiz->l_hijos.count()==1){
              NodoAst left = raiz->l_hijos.at(0);
              if(left.getType()==AUMENTO){
                s.type = DECIMAL;
                s.value = QString::number(raiz->value.toFloat() + 1);
              }
              else if(left.getType() == DECREMENTO){
                s.type = DECIMAL;
                s.value = QString::number(raiz->value.toFloat() - 1);
              }
          }
          else{
            s.type = DECIMAL;
            s.value = raiz->value;
          }
          break;}
        case BOOLEANO:{
            s.type = BOOLEANO;
            s.value = raiz->value.replace("\"","");
            break;}
        case CARACTER:{
          if(raiz->l_hijos.count()==1){
              NodoAst left = raiz->l_hijos.at(0);
              if(left.getType()==AUMENTO){
                s.type = NUMERO;
                QChar c = raiz->value.at(0);
                int x = c.toLatin1() + 1;
                s.value = QString::number(x);
              }
              else if(left.getType() == DECREMENTO){
                s.type = NUMERO;
                QChar c = raiz->value.at(0);
                int x = c.toLatin1() - 1;
                s.value = QString::number(x);
              }
          }
          else{
            s.type = CARACTER;
            s.value = raiz->value.replace("'","");
          }
          break;}
        case CADENA:{
            s.type = CADENA;
            s.value = raiz->value.replace("\"","");
            break;}
        case VID:{
            Variable b = h->getVariable(raiz->value);
            if(b.tipo != -1){
                if(raiz->l_hijos.count()==1){
                    NodoAst left = raiz->l_hijos.at(0);
                    if(left.getType()==AUMENTO){
                        switch (b.tipo) {
                        case 1:{s.type = NUMERO; int x = b.valor.toInt()+1; s.value = QString::number(x);
                            b.valor = QString::number(x); h->change(raiz->value,b);}
                            break;
                        case 4:{s.type = DECIMAL; float x = b.valor.toFloat()+1; s.value = QString::number(x);
                        b.valor = QString::number(x); h->change(raiz->value,b);}
                            break;
                        case 5:{s.type = CARACTER; QChar c = b.valor.at(0); int x = c.toLatin1()+1;
                             s.value = QString::number(x); b.valor = QString::number(x); h->change(raiz->value,b);}
                            break;
                        default:{s.type = ERROR; s.value = "ERROR";
                            lerror.append(*new gError(3,s.row,s.column,"Aumento no definidio para este tipo"));
                        }
                            break;
                        }
                    }
                    else if(left.getType() == DECREMENTO){
                        switch (b.tipo) {
                        case 1:{s.type = NUMERO; int x = b.valor.toInt()-1; s.value = QString::number(x);
                            b.valor = QString::number(x); h->change(raiz->value,b);}
                            break;
                        case 4:{s.type = DECIMAL; float x = b.valor.toFloat()-1; s.value = QString::number(x);
                            b.valor = QString::number(x); h->change(raiz->value,b);}
                            break;
                        case 5:{s.type = CARACTER; QChar c = b.valor.at(0); int x = c.toLatin1()-1;
                            s.value = QString::number(x); b.valor = QString::number(x); h->change(raiz->value,b);}
                            break;
                        default:{s.type = ERROR; s.value = "ERROR";
                        lerror.append(*new gError(3,s.row,s.column,"Decremento no definidio para este tipo"));}
                            break;
                        }
                    }
                }
                else{
                    switch (b.tipo) {
                    case 1:
                        s.type = NUMERO;
                        break;
                    case 2:
                        s.type = CADENA;
                        break;
                    case 3:
                        s.type = BOOLEANO;
                        break;
                    case 4:
                        s.type = DECIMAL;
                        break;
                    case 5:
                        s.type = CARACTER;
                        break;
                    default:{
                        s.type = ERROR;
                    lerror.append(*new gError(3,s.row,s.column,"Dato erroneo"));}
                        break;
                    }
                    s.value = b.valor;
                }
            }
            else{
                s.type = ERROR;
                s.value = "-1";
                lerror.append(*new gError(3,s.row,s.column,"Variable no existe"));
            }
            break;
        }

        case DATOMATRIZ:{
        int hijos = raiz->l_hijos.count();
        if(hijos==2){
            NodoAst id = raiz->l_hijos.at(0);
            Variable b = h->getVariable(id.value);
            NodoAst pos = raiz->l_hijos.at(1); NodoAst pos1= pos.l_hijos.at(0); Simbolo spos = recorrer(&pos1,h);
            if(spos.type == NUMERO){
                Arreglo *ar = &b.arr;
                if(spos.value.toInt()<ar->i){
                    switch (ar->tipo) {
                    case 11:
                        s.value =QString::number(ar->int_a[spos.value.toInt()]);
                        s.type = NUMERO;
                        break;
                    case 12:
                        s.value = ar->string_a[spos.value.toInt()];
                        s.type = CADENA;
                        break;
                    case 13:
                        s.value = ar->char_a[spos.value.toInt()];
                        s.type = CARACTER;
                        break;
                    case 14:
                        s.value = ar->bool_a[spos.value.toInt()] == true ? "true" : "false";
                        s.type = BOOLEANO;
                        break;
                    case 15:
                        s.value = QString::number(ar->float_a[spos.value.toInt()]);
                        s.type = DECIMAL;
                        break;
                    default:
                        break;
                    }
                }
                else{
                    /*fuera del intervalos*/
                }
            }
            else{
                lerror.append(*new gError(3,s.row,s.column,"La posicion debe ser entero"));
            }
        }
        else if(hijos==3){
            NodoAst id = raiz->l_hijos.at(0);
            Variable b = h->getVariable(id.value);
            NodoAst posx = raiz->l_hijos.at(1); NodoAst pos1x = posx.l_hijos.at(0); Simbolo sposx = recorrer(&pos1x,h);
            NodoAst posy = raiz->l_hijos.at(2); NodoAst pos1y = posy.l_hijos.at(0); Simbolo sposy = recorrer(&pos1y,h);

            if(sposx.type == NUMERO && sposy.type==NUMERO){
                Arreglo *ar = &b.arr;
                if(sposx.value < ar->i && sposy.value<ar->j){
                    switch (ar->tipo) {
                    case 21:
                        s.value =QString::number(ar->int_aa[sposx.value.toInt()][sposy.value.toInt()]);
                        s.type = NUMERO;
                        break;
                    case 22:
                        s.value = ar->string_aa[sposx.value.toInt()][sposy.value.toInt()];
                        s.type = CADENA;
                        break;
                    case 23:
                        s.value = ar->char_aa[sposx.value.toInt()][sposy.value.toInt()];
                        s.type = CARACTER;
                        break;
                    case 24:
                        s.value = ar->bool_aa[sposx.value.toInt()][sposy.value.toInt()] == true ? "true" : "false";
                        s.type = BOOLEANO;
                        break;
                    case 25:
                        s.value = QString::number(ar->float_aa[sposx.value.toInt()][sposy.value.toInt()]);
                        s.type = DECIMAL;
                        break;
                    default:
                        break;
                    }
                }
                else{
                    /*fuera de intervalo*/
                }
            }
            else{
                lerror.append(*new gError(3,s.row,s.column,"La posicion debe ser entero"));
            }
        }
        else{ //int arreglo x[2][2][2] = {{{1,2},{3,4}},{{5,6},{7,8}}}; int xx = x[0][0][0];
            NodoAst id = raiz->l_hijos.at(0);
            Variable b = h->getVariable(id.value);
            NodoAst posx = raiz->l_hijos.at(1); NodoAst pos1x= posx.l_hijos.at(0); Simbolo sposx = recorrer(&pos1x,h);
            NodoAst posy = raiz->l_hijos.at(2); NodoAst pos1y= posy.l_hijos.at(0); Simbolo sposy = recorrer(&pos1y,h);
            NodoAst posz = raiz->l_hijos.at(3);
            NodoAst pos1z= posz.l_hijos.at(0);
            Simbolo sposz = recorrer(&pos1z,h);
            if(sposx.type == NUMERO && sposy.type==NUMERO && sposz.type ==NUMERO){
                Arreglo *ar = &b.arr;
                if(sposx.value.toInt()<ar->i && sposy.value.toInt()<ar->j && sposz.value.toInt()<ar->k){
                    switch (ar->tipo) {
                    case 31:
                        s.value =QString::number(ar->int_aaa[sposx.value.toInt()][sposy.value.toInt()][sposz.value.toInt()]);
                        s.type = NUMERO;
                        break;
                    case 32:
                        s.value = ar->string_aaa[sposx.value.toInt()][sposy.value.toInt()][sposz.value.toInt()];
                        s.type = CADENA;
                        break;
                    case 33:
                        s.value = ar->string_aaa[sposx.value.toInt()][sposy.value.toInt()][sposz.value.toInt()];
                        break;
                    case 34:
                        s.value = ar->bool_aaa[sposx.value.toInt()][sposy.value.toInt()][sposz.value.toInt()] == true ? "true" : "false";
                        s.type = BOOLEANO;
                        break;
                    case 35:
                        s.value = QString::number(ar->float_aaa[sposx.value.toInt()][sposy.value.toInt()][sposz.value.toInt()]);
                        s.type = DECIMAL;
                        break;
                    default:
                        break;
                    }
                }
                else{
                    /*fuera de intervalo*/
                }
            }
            else{
                lerror.append(*new gError(3,s.row,s.column,"La posicion debe ser entero"));
            }

        }
        break;
        }

        case MAS:{
            NodoAst left = raiz->l_hijos.at(0);
            Simbolo sl = recorrer(&left,h);
            NodoAst right = raiz->l_hijos.at(1);
            Simbolo sr = recorrer(&right,h);
            switch (sl.type) {
                case NUMERO:
                    switch (sr.type) {
                        case NUMERO:
                            {s.type = NUMERO;
                            int r = sl.value.toInt() + sr.value.toInt();
                            s.value = QString::number(r);}
                            break;
                        case DECIMAL:
                            {s.type = DECIMAL;
                             float r = sl.value.toFloat() + sr.value.toFloat();
                             s.value = QString::number(r);}
                            break;
                        case BOOLEANO:
                            {s.type = NUMERO;
                             int b = 0;
                             if(sr.value == "true") b = 1;
                             int r = sl.value.toInt() + b;
                             s.value = QString::number(r);}
                            break;
                        case CARACTER:
                            {s.type = NUMERO;
                            QChar c = sr.value.at(0);
                            int x = c.toLatin1() + sl.value.toInt();
                             s.value = QString::number(x);}
                            break;
                        case CADENA:
                            {s.type = CADENA;
                             s.value = sl.value + sr.value;}
                            break;
                        case ERROR:
                            {s.type = ERROR;
                            s.value = "ERROR";}
                            break;
                        default:
                            {s.type = ERROR; s.value = "ERROR";
                    lerror.append(*new gError(3,s.row,s.column,"No se puede sumar Error mas Entero"));}
                            break;
                    }
                    break;
                case DECIMAL:
                    switch (sr.type) {
                        case NUMERO:
                    {s.type = DECIMAL;
                     float r = sl.value.toFloat() + sr.value.toFloat();
                     s.value = QString::number(r);}
                            break;
                        case DECIMAL:
                    {s.type = DECIMAL;
                     float r = sl.value.toFloat() + sr.value.toFloat();
                     s.value = QString::number(r);}
                            break;
                        case BOOLEANO:
                    {s.type = DECIMAL;
                     float b = 0;
                     if(sr.value == "true") b = 1;
                     float r = sl.value.toFloat() + b;
                     s.value = QString::number(r);}
                            break;
                        case CARACTER:
                    {s.type = DECIMAL;
                    QChar c = sr.value.at(0);
                    float x = c.toLatin1() + sl.value.toFloat();
                     s.value = QString::number(x);}
                            break;
                        case CADENA:
                    {s.type = CADENA;
                     s.value = sl.value + sr.value;}
                            break;
                        case ERROR:
                    {s.type = ERROR;
                    s.value = "ERROR";}
                            break;
                        default:
                    {s.type = ERROR;
                    s.value = "ERROR"; lerror.append(*new gError(3,s.row,s.column,"No se puede sumar Double mas Error"));}
                            break;
                        }
                    break;
                case BOOLEANO:
                    switch (sr.type) {
                        case NUMERO:
                    {s.type = NUMERO;
                     int b = 0;
                     if(sl.value == "true") b = 1;
                     int r = sr.value.toInt() + b;
                     s.value = QString::number(r);}
                            break;
                        case DECIMAL:
                    {s.type = DECIMAL;
                     float b = 0;
                     if(sl.value == "true") b = 1;
                     float r = sr.value.toFloat() + b;
                     s.value = QString::number(r);}
                            break;
                        case BOOLEANO:
                            {s.type = BOOLEANO;
                            if(sl.value == "true" || sr.value == "true")
                                s.value = "true";
                            s.value = "false";}
                            break;
                        case CARACTER:
                    {s.type = NUMERO;
                        int b = 0;
                        if(sl.value == "true") b = 1;
                    QChar c = sr.value.at(0);
                    int x = c.toLatin1() + b;
                     s.value = QString::number(x);}
                            break;
                    case CADENA:
                        {/*ERROR DE CADENA*/}
                        break;
                        case ERROR:
                    {s.type = ERROR;
                    s.value = "ERROR"; lerror.append(*new gError(3,s.row,s.column,"Error mas Booleano no se puede"));}
                            break;
                        default:
                    {s.type = ERROR;
                    s.value = "ERROR"; lerror.append(*new gError(3,s.row,s.column,"Error mas bool no se puede"));}
                            break;
                        }
                    break;
                case CADENA:
                    switch (sr.type) {
                        case NUMERO:
                            {s.type = CADENA;
                             s.value = sl.value + sr.value;}
                            break;
                        case DECIMAL:
                            {s.type = CADENA;
                             s.value = sl.value + sr.value;}
                            break;
                        case CARACTER:
                            {s.type = CADENA;
                             s.value = sl.value + sr.value;}
                                break;
                        case CADENA:
                            {s.type = CADENA;
                             s.value = sl.value + sr.value;}
                    case BOOLEANO:{
                            s.type = CADENA; s.value = sl.value + sr.value;
                        }
                            break;
                        case ERROR:
                    {s.type = ERROR;
                    s.value = "ERROR"; lerror.append(*new gError(3,s.row,s.column,"Error mas Cadena no se puede"));}
                            break;
                        default:
                    {s.type = ERROR;
                    s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"Error mas cadena"));}
                            break;
                        }
                        break;
                case CARACTER:
                    switch (sr.type) {
                            case NUMERO:
                    {s.type = NUMERO;
                    QChar c = sl.value.at(0);
                    int x = c.toLatin1() + sr.value.toInt();
                     s.value = QString::number(x);}
                                break;
                            case DECIMAL:
                    {s.type = DECIMAL;
                    QChar c = sl.value.at(0);
                    float x = c.toLatin1() + sr.value.toFloat();
                     s.value = QString::number(x);}
                                break;
                            case BOOLEANO:
                    {s.type = NUMERO;
                        int b = 0;
                        if(sl.value == "true") b = 1;
                    QChar c = sl.value.at(0);
                    int x = c.toLatin1() + b;
                     s.value = QString::number(x);}
                                break;
                            case CARACTER:
                    {s.type = NUMERO; QChar c = sl.value.at(0);
                     QChar c1 = sr.value.at(0);
                     int x = c.toLatin1() + c1.toLatin1();
                     s.value = QString::number(x);}
                                break;
                            case CADENA:
                    {s.type = CADENA;
                     s.value = sl.value + sr.value;}
                                break;
                            case ERROR:
                    {s.type = ERROR;
                    s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"Error mas char no se puede"));}
                                break;
                            default:
                    {s.type = ERROR;
                    s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"Error mas char no se puede"));}
                                break;
                            }
                        break;
                case ERROR:
                    {s.type = ERROR;
                    s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"Error no puede sumar"));}
                        break;
                default:
                    {s.type = ERROR;
                    s.value = "ERROR"; lerror.append(*new gError(3,s.row,s.column,"Error no puede sumar"));}
                    break;
            }
            break;
            }
        case MENOS:{
            if(raiz->l_hijos.count()==2){
                NodoAst left = raiz->l_hijos.at(0);
                Simbolo sl = recorrer(&left,h);
                NodoAst right = raiz->l_hijos.at(1);
                Simbolo sr = recorrer(&right,h);
                switch (sl.type) {
                    case NUMERO:
                        switch (sr.type) {
                            case NUMERO:
                                {s.type = NUMERO;
                                int r = sl.value.toInt() - sr.value.toInt();
                                s.value = QString::number(r);}
                                break;
                            case DECIMAL:
                                {s.type = DECIMAL;
                                 float r = sl.value.toFloat() - sr.value.toFloat();
                                 s.value = QString::number(r);}
                                break;
                            case BOOLEANO:
                                {s.type = NUMERO;
                                 int b = 0;
                                 if(sr.value == "true") b = 1;
                                 int r = sl.value.toInt() - b;
                                 s.value = QString::number(r);}
                                break;
                            case CARACTER:
                                {s.type = NUMERO;
                                QChar c = sr.value.at(0);
                                int x = sl.value.toInt() - c.toLatin1();
                                 s.value = QString::number(x);}
                                break;
                            case CADENA:{
                                /*error numero - cadena*/
                                }
                                break;
                            case ERROR:
                                {s.type = ERROR;
                                s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"Error menos numero"));}
                                break;
                            default:
                                break;
                        }
                        break;
                    case DECIMAL:
                        switch (sr.type) {
                            case NUMERO:
                                {s.type = DECIMAL;
                                 float r = sl.value.toFloat() - sr.value.toFloat();
                                 s.value = QString::number(r);}
                                break;
                            case DECIMAL:
                                {s.type = DECIMAL;
                                 float r = sl.value.toFloat() - sr.value.toFloat();
                                 s.value = QString::number(r);}
                                break;
                            case BOOLEANO:
                                {s.type = DECIMAL;
                                 float b = 0;
                                 if(sr.value == "true") b = 1;
                                 float r = sl.value.toFloat() - b;
                                 s.value = QString::number(r);}
                                break;
                            case CARACTER:
                                {s.type = DECIMAL;
                                QChar c = sr.value.at(0);
                                float x = sl.value.toFloat() - c.toLatin1();
                                 s.value = QString::number(x);}
                                break;
                            case CADENA:{
                                /*ERROR DECIMAL - CARACTER*/
                            }
                                break;
                            case ERROR:
                                {s.type = ERROR;
                                s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"Error menos decimal"));}
                                break;
                            default:
                                {s.type = ERROR;
                                s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"error menos decimal"));}
                                break;
                            }
                        break;
                    case BOOLEANO:
                        switch (sr.type) {
                            case NUMERO:
                                {s.type = NUMERO;
                                 int b = 0;
                                 if(sl.value == "true") b = 1;
                                 int r = b - sr.value.toInt();
                                 s.value = QString::number(r);}
                                break;
                            case DECIMAL:
                                {s.type = DECIMAL;
                                 float b = 0;
                                 if(sl.value == "true") b = 1;
                                 float r = b-sr.value.toFloat();
                                 s.value = QString::number(r);}
                                break;
                            case CADENA:{

                                }
                                break;
                            case CARACTER:{

                                }
                                break;
                            case BOOLEANO:{

                                }
                                break;
                            case ERROR:
                                {s.type = ERROR;
                                s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"Error menos booleano"));}
                                break;
                            default:
                                {s.type = ERROR;
                                s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"Error menos booleano"));}
                                break;
                            }
                        break;
                    case CARACTER:
                        switch (sr.type) {
                                case NUMERO:
                                    {s.type = NUMERO;
                                    QChar c = sl.value.at(0);
                                    int x = c.toLatin1() - sr.value.toInt() ;
                                     s.value = QString::number(x);}
                                    break;
                                case DECIMAL:
                                    {s.type = DECIMAL;
                                    QChar c = sl.value.at(0);
                                    float x = c.toLatin1() - sr.value.toFloat() ;
                                     s.value = QString::number(x);}
                                    break;
                                case CARACTER:
                                    {s.type = NUMERO;
                                      QChar c = sl.value.at(0);
                                     QChar c1 = sr.value.at(0);
                                     int x = c.toLatin1() - c1.toLatin1();
                                     s.value = QString::number(x);}
                                    break;
                                case CADENA:{

                                    }
                                    break;
                                case BOOLEANO:{

                                    }
                                break;
                                case ERROR:
                                    {s.type = ERROR;
                                    s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"Error menos caracter"));}
                                    break;
                                default:
                                    {s.type = ERROR;
                                    s.value = "ERROR"; lerror.append(*new gError(3,s.row,s.column,"Error menos caracter"));}
                                    break;
                                }
                        break;
                    case CADENA:
                        switch (sr.type) {
                            case NUMERO:
                                {

                                }
                                break;
                            case DECIMAL:
                                {}
                                break;
                            case BOOLEANO:
                                {}
                                break;
                            case CARACTER:
                                {}
                                break;
                            case CADENA:{
                                /*error numero - cadena*/
                                }
                                break;
                            case ERROR:
                                {}
                                break;
                            default:
                                break;
                        }
                        break;
                    case ERROR:
                        {s.type = ERROR;
                        s.value = "ERROR"; lerror.append(*new gError(3,s.row,s.column,"Error no puede restar"));}
                        break;
                default:
                    break;
                }
                    break;
            }
            else{
              NodoAst hijo = raiz->l_hijos.at(0);
              Simbolo sl = recorrer(&hijo,h);
              switch (sl.type) {
                case NUMERO:{
                    s.type = NUMERO;
                    s.value = QString::number(sl.value.toInt()*-1);
                }
                  break;
                case DECIMAL:{
                    s.type =DECIMAL;
                    s.value = QString::number(sl.value.toFloat()*-1);
                }
                  break;
              default:{lerror.append(*new gError(3,s.row,s.column,"Tipo de dato no puede ser negativo"));}
                  break;
              }
            }
            break;
            }
        case POR:{
        NodoAst left = raiz->l_hijos.at(0);
        Simbolo sl = recorrer(&left,h);
        NodoAst right = raiz->l_hijos.at(1);
        Simbolo sr = recorrer(&right,h);
            switch (sl.type) {
                case NUMERO:
                    switch (sr.type) {
                        case NUMERO:
                            {s.type = NUMERO;
                            int r = sl.value.toInt() * sr.value.toInt();
                            s.value = QString::number(r);}
                            break;
                        case DECIMAL:
                            {s.type = DECIMAL;
                             float r = sl.value.toFloat() * sr.value.toFloat();
                             s.value = QString::number(r);}
                            break;
                        case BOOLEANO:
                            {s.type = NUMERO;
                             int b = 0;
                             if(sr.value == "true") b = 1;
                             int r = sl.value.toInt() * b;
                             s.value = QString::number(r);}
                            break;
                        case CARACTER:
                            {s.type = NUMERO;
                            QChar c = sr.value.at(0);
                            int x = c.toLatin1() * sl.value.toInt();
                             s.value = QString::number(x);}
                            break;
                        case CADENA:{}
                            break;
                        case ERROR:
                              {s.type = ERROR;
                              s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"entero por error"));}
                                break;
                        default:
                              {s.type = ERROR;
                              s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"entero por error"));}
                            break;
                    }
                    break;
                case DECIMAL:
                    switch (sr.type) {
                        case NUMERO:
                            {s.type = DECIMAL;
                             float r = sl.value.toFloat() * sr.value.toFloat();
                             s.value = QString::number(r);}
                            break;
                        case DECIMAL:
                            {s.type = DECIMAL;
                             float r = sl.value.toFloat() * sr.value.toFloat();
                             s.value = QString::number(r);}
                            break;
                        case BOOLEANO:
                            {s.type = DECIMAL;
                             float b = 0;
                             if(sr.value == "true") b = 1;
                             float r = sl.value.toFloat() * b;
                             s.value = QString::number(r);}
                            break;
                        case CARACTER:
                            {s.type = DECIMAL;
                            QChar c = sr.value.at(0);
                            float x = c.toLatin1() * sl.value.toFloat();
                             s.value = QString::number(x);}
                            break;
                        case CADENA:{}
                            break;
                        case ERROR:
                            {s.type = ERROR;
                            s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"decimal por error"));}
                            break;
                        default:
                            {s.type = ERROR;
                            s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"decimal por error"));}
                            break;
                        }
                    break;
                case BOOLEANO:
                    switch (sr.type) {
                        case NUMERO:
                            {s.type = NUMERO;
                             int b = 0;
                             if(sl.value == "true") b = 1;
                             int r = b * sr.value.toInt();
                             s.value = QString::number(r);}
                            break;
                        case DECIMAL:
                            {s.type = DECIMAL;
                             float b = 0;
                             if(sl.value == "true") b = 1;
                             float r = b * sr.value.toFloat();
                             s.value = QString::number(r);}
                            break;
                        case BOOLEANO:
                            {s.type = BOOLEANO;
                            if(sl.value == "true" && sr.value == "true")
                                s.value = "true";
                            s.value = "false";}
                            break;
                        case CARACTER:
                            {s.type = NUMERO;
                            QChar c = sr.value.at(0);
                            int x = sl.value.toInt() * c.toLatin1();
                             s.value = QString::number(x);}
                            break;
                        case CADENA:{}
                            break;
                        case ERROR:
                            {s.type = ERROR;
                            s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"booleano por error"));}
                            break;
                        default:
                            {s.type = ERROR;
                            s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"bool por error"));}
                            break;
                        }
                    break;
                case CARACTER:
                    switch (sr.type) {
                            case NUMERO:
                                {s.type = NUMERO;
                                QChar c = sl.value.at(0);
                                int x = c.toLatin1() * sr.value.toInt();
                                 s.value = QString::number(x);}
                                break;
                            case DECIMAL:
                                {s.type = DECIMAL;
                                QChar c = sl.value.at(0);
                                float x = c.toLatin1() * sr.value.toFloat();
                                 s.value = QString::number(x);}
                                break;
                            case BOOLEANO:
                                {s.type = NUMERO;
                                    int b = 0;
                                    if(sr.value == "true") b = 1;
                                QChar c = sl.value.at(0);
                                int x = c.toLatin1() * b;
                                 s.value = QString::number(x);}
                                break;
                            case CARACTER:
                                {s.type = NUMERO; QChar c = sl.value.at(0);
                                 QChar c1 = sr.value.at(0);
                                 int x = c.toLatin1() * c1.toLatin1();
                                 s.value = QString::number(x);}
                                break;
                            case CADENA:{}
                                break;
                            case ERROR:
                                lerror.append(*new gError(3,s.row,s.column,"caracter por error"));
                                break;
                            default:
                                {s.type = ERROR;
                                s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"decimal por error")); }
                                break;
                            }
                        break;
                case ERROR:
                    {s.type = ERROR;
                    s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"error por error")); }
                        break;
                case CADENA:
                    switch (sr.type) {
                        case NUMERO:
                            {

                            }
                            break;
                        case DECIMAL:
                            {}
                            break;
                        case BOOLEANO:
                            {}
                            break;
                        case CARACTER:
                            {}
                            break;
                        case CADENA:{
                            /*error numero - cadena*/
                            }
                            break;
                        case ERROR:
                            {}
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    {s.type = ERROR;
                    s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"error por error")); }
                    break;
        }
        break;}
        case DIV:{
        NodoAst left = raiz->l_hijos.at(0);
        Simbolo sl = recorrer(&left,h);
        NodoAst right = raiz->l_hijos.at(1);
        Simbolo sr = recorrer(&right,h);
            switch (sl.type) {
                case NUMERO:
                    switch (sr.type) {
                        case NUMERO:
                        {s.type = NUMERO;
                          if(sr.value.toInt()!=0){
                            int r = sl.value.toInt() / sr.value.toInt();
                            s.value = QString::number(r);}
                          else{
                            s.type = ERROR;
                            s.value = "ERROR";
                            lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                          }
                        }
                            break;
                        case DECIMAL:
                        {if(sr.value.toFloat()!=0){ s.type = DECIMAL;
                            float r = sl.value.toFloat() / sr.value.toFloat();
                            s.value = QString::number(r);
                          }
                          else{
                            s.type = ERROR;
                            s.value = "ERROR";
                            lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                          }
                        }
                            break;
                        case BOOLEANO:
                        { int b = 0; if(sr.value == "true") b = 1;
                          if(b!=0){
                            s.type = NUMERO;
                            int r = sl.value.toInt() / b;
                            s.value = QString::number(r);
                          }
                          else{
                            s.type = ERROR;
                            s.value = "ERROR";
                            lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                          }
                        }
                            break;
                        case CARACTER:
                        { s.type = NUMERO;
                          QChar c = sr.value.at(0);
                          if(c.toLatin1()!=0){
                            int x =  sl.value.toInt() / c.toLatin1();
                            s.value = QString::number(x);
                          }
                          else{
                            s.type = ERROR;
                            s.value = "ERROR";
                            lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                          }
                        }
                            break;
                        case CADENA:{}
                            break;
                        case ERROR:
                            {s.type = ERROR;
                            s.value = "ERROR";
                    lerror.append(*new gError(3,s.row,s.column,"numero dividido error"));}
                            break;
                        default:
                            {s.type = ERROR;
                            s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"numero entre error"));}
                            break;
                    }
                    break;
                case DECIMAL:
                    switch (sr.type) {
                        case NUMERO:{
                            if (sr.value.toFloat()!=0) {
                              s.type = DECIMAL;
                              float r = sl.value.toFloat() / sr.value.toFloat();
                              s.value = QString::number(r);
                            } else {
                              s.type = ERROR;
                              s.value = "ERROR";
                              lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                            }
                          }
                            break;
                        case DECIMAL:{
                            if (sr.value.toFloat()!=0) {
                              s.type = DECIMAL;
                               float r = sl.value.toFloat() / sr.value.toFloat();
                               s.value = QString::number(r);
                            } else {
                              s.type = ERROR;
                              s.value = "ERROR";
                              lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                            }
                          }
                            break;
                        case BOOLEANO:{
                            float b = 0;
                            if(sr.value == "true") b = 1;
                            if (b!=0) {
                              s.type = DECIMAL;
                               float r = sl.value.toFloat() / b;
                               s.value = QString::number(r);
                            } else {
                              s.type = ERROR;
                              s.value = "ERROR";
                              lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                            }
                          }
                            break;
                        case CARACTER:{
                            QChar c = sr.value.at(0);
                            if (c.toLatin1()!=0) {
                              s.type = DECIMAL;
                              float x = sl.value.toFloat() / c.toLatin1() ;
                               s.value = QString::number(x);
                            } else {
                              s.type = ERROR;
                              s.value = "ERROR";
                              lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                            }
                          }
                            break;
                        case CADENA:{}
                            break;
                        case ERROR:{
                            s.type = ERROR;
                            s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"decimal entre error"));}
                            break;
                        default:{
                            s.type = ERROR;
                            s.value = "ERROR";lerror.append(*new gError(3,s.row,s.column,"decimal entre error"));}
                            break;
                        }
                    break;
                case BOOLEANO:
                    switch (sr.type) {
                        case NUMERO:{
                            if (sr.value.toInt()!=0) {
                              s.type = NUMERO;
                               int b = 0;
                               if(sl.value == "true") b = 1;
                               int r = b / sr.value.toInt();
                               s.value = QString::number(r);
                            } else {
                              s.type = ERROR;
                              s.value = "ERROR";
                            }
                          }

                            break;
                        case DECIMAL:{
                            if (sr.value.toFloat()!=0) {
                              s.type = DECIMAL;
                               float b = 0;
                               if(sl.value == "true") b = 1;
                               float r = b / sr.value.toFloat();
                               s.value = QString::number(r);
                            } else {
                              s.type = ERROR;
                              s.value = "ERROR";
                            }
                          }
                            break;
                        case CARACTER:{
                            QChar c = sr.value.at(0);
                            if (c.toLatin1()!=0) {
                              s.type = NUMERO;
                              int x = sl.value.toInt() / c.toLatin1();
                               s.value = QString::number(x);
                            } else {
                              s.type = ERROR;
                              s.value = "ERROR";
                              lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                            }
                          }
                            break;
                        case BOOLEANO:{}
                            break;
                        case CADENA:{}
                            break;
                        case ERROR:{
                          s.type = ERROR;
                          s.value = "ERROR";
                        }
                            break;
                        default:{
                          s.type = ERROR;
                          s.value = "ERROR";
                        }
                            break;
                        }
                    break;
                case CARACTER:
                    switch (sr.type) {
                            case NUMERO:{
                                if (sr.value.toInt()!=0) {
                                  s.type = NUMERO;
                                  QChar c = sl.value.at(0);
                                  int x = c.toLatin1() / sr.value.toInt();
                                   s.value = QString::number(x);
                                } else {
                                  s.type = ERROR;
                                  s.value = "ERROR";
                                  lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                                }
                              }
                                break;
                            case DECIMAL:{
                                if (sr.value.toFloat()!=0) {
                                  s.type = DECIMAL;
                                  QChar c = sl.value.at(0);
                                  float x = c.toLatin1() / sr.value.toFloat();
                                   s.value = QString::number(x);
                                } else {
                                  s.type = ERROR;
                                  s.value = "ERROR";
                                  lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                                }
                              }
                                break;
                            case BOOLEANO:{
                                  int b = 0;
                                  if(sr.value == "true") b = 1;
                                  if (b!=0) {
                                    s.type = NUMERO;
                                    QChar c = sl.value.at(0);
                                    int x = c.toLatin1() / b;
                                     s.value = QString::number(x);
                                  } else {
                                    s.type = ERROR;
                                    s.value = "ERROR";
                                    lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                                  }
                                }
                                break;
                            case CARACTER:{
                                s.type = NUMERO; QChar c = sl.value.at(0);
                                 QChar c1 = sr.value.at(0);
                                  if (c1.toLatin1()) {
                                     int x = c.toLatin1() / c1.toLatin1();
                                     s.value = QString::number(x);
                                  } else {
                                    s.type = ERROR;
                                    s.value = "ERROR";
                                    lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                                  }
                                 }
                                break;
                            case CADENA:{}
                                break;
                            case ERROR:{
                                  s.type = ERROR;
                                  s.value = "ERROR";
                                  lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                                }
                                break;
                            default:{
                                  s.type = ERROR;
                                  s.value = "ERROR";
                                  lerror.append(*new gError(3,s.row,s.column,"division no puede ser entre 0"));
                                }
                                break;
                            }
                        break;
                case CADENA:
                    switch (sr.type) {
                        case NUMERO:
                            {

                            }
                            break;
                        case DECIMAL:
                            {}
                            break;
                        case BOOLEANO:
                            {}
                            break;
                        case CARACTER:
                            {}
                            break;
                        case CADENA:{
                            /*error numero - cadena*/
                            }
                            break;
                        case ERROR:
                            {}
                            break;
                        default:
                            break;
                    }
                    break;
                case ERROR:{
                  s.type = ERROR;
                  s.value = "ERROR";
                }
                    break;
              default:{
                s.type = ERROR;
                s.value = "ERROR";}
                  break;
              }

        break;}
        case POTENCIA:{
            NodoAst left = raiz->l_hijos.at(0);
            Simbolo sl = recorrer(&left,h);
            NodoAst right = raiz->l_hijos.at(1);
            Simbolo sr = recorrer(&right,h);
            switch (sl.type) {
              case NUMERO:
                  switch (sr.type) {
                      case NUMERO:
                          {s.type = NUMERO;
                           int r = pow(sl.value.toInt(),sr.value.toInt());
                          s.value = QString::number(r);}
                          break;
                      case DECIMAL:
                          {s.type = DECIMAL;
                           float r = pow(sl.value.toFloat(),sr.value.toFloat());
                           s.value = QString::number(r);}
                          break;
                      case BOOLEANO:
                          {s.type = NUMERO;
                           int b = 0;
                           if(sr.value == "true") b = 1;
                           int r = pow(sl.value.toInt(), b);
                           s.value = QString::number(r);}
                          break;
                      case CARACTER:
                          {s.type = NUMERO;
                          QChar c = sr.value.at(0);
                          int x = pow(c.toLatin1() , sl.value.toInt());
                           s.value = QString::number(x);}
                          break;
                      case CADENA:{}
                          break;
                      case ERROR:
                            {s.type = ERROR;
                            s.value = "ERROR";}
                              break;
                      default:
                            {s.type = ERROR;
                            s.value = "ERROR";}
                          break;
                  }
                  break;
                case DECIMAL:
                    switch (sr.type) {
                      case NUMERO:
                          {s.type = DECIMAL;
                           double r = pow(sl.value.toFloat() , sr.value.toFloat());
                           s.value = QString::number(r);}
                          break;
                      case DECIMAL:
                          {s.type = DECIMAL;
                           double r = pow(sl.value.toFloat() , sr.value.toFloat());
                           s.value = QString::number(r);}
                          break;
                      case BOOLEANO:
                          {s.type = DECIMAL;
                           float b = 0;
                           if(sr.value == "true") b = 1;
                           float r = pow(sl.value.toFloat() , b);
                           s.value = QString::number(r);}
                          break;
                      case CARACTER:
                          {s.type = DECIMAL;
                          QChar c = sr.value.at(0);
                          float x = pow(c.toLatin1() , sl.value.toFloat());
                           s.value = QString::number(x);}
                          break;
                        case CADENA:{}
                            break;
                      case ERROR:
                          {s.type = ERROR;
                          s.value = "ERROR";}
                          break;
                      default:
                          {s.type = ERROR;
                          s.value = "ERROR";}
                          break;
                        }
                    break;
                case BOOLEANO:
                    switch (sr.type) {
                      case NUMERO:
                          {s.type = NUMERO;
                           int b = 0;
                           if(sl.value == "true") b = 1;
                           int r = pow(b , sr.value.toInt());
                           s.value = QString::number(r);}
                          break;
                      case DECIMAL:
                          {s.type = DECIMAL;
                           float b = 0;
                           if(sl.value == "true") b = 1;
                           float r = pow( b , sr.value.toFloat());
                           s.value = QString::number(r);}
                          break;
                      case CARACTER:
                          {s.type = NUMERO;
                          QChar c = sr.value.at(0);
                          int x = pow(sl.value.toInt() , c.toLatin1());
                           s.value = QString::number(x);}
                          break;
                        case BOOLEANO:{}
                            break;
                        case CADENA:{}
                            break;
                      case ERROR:
                          {s.type = ERROR;
                          s.value = "ERROR";}
                          break;
                      default:
                          {s.type = ERROR;
                          s.value = "ERROR";}
                          break;
                        }
                    break;
                case CARACTER:
                  switch (sr.type) {
                        case NUMERO:
                            {s.type = NUMERO;
                            QChar c = sl.value.at(0);
                            int x = pow(c.toLatin1() , sr.value.toInt());
                             s.value = QString::number(x);}
                            break;
                        case DECIMAL:
                            {s.type = DECIMAL;
                            QChar c = sl.value.at(0);
                            float x = pow(c.toLatin1() , sr.value.toFloat());
                             s.value = QString::number(x);}
                            break;
                        case BOOLEANO:
                            {s.type = NUMERO;
                                int b = 0;
                                if(sr.value == "true") b = 1;
                            QChar c = sl.value.at(0);
                            int x = pow(c.toLatin1() , b);
                             s.value = QString::number(x);}
                            break;
                        case CARACTER:
                            {s.type = NUMERO; QChar c = sl.value.at(0);
                             QChar c1 = sr.value.at(0);
                             int x = pow(c.toLatin1() , c1.toLatin1());
                             s.value = QString::number(x);}
                            break;
                      case CADENA:{}
                          break;
                        case ERROR:
                            {s.type = ERROR;
                            s.value = "ERROR";}
                            break;
                        default:
                            {s.type = ERROR;
                            s.value = "ERROR";}
                            break;
                        }
                    break;
                case ERROR:
                    {s.type = ERROR;
                    s.value = "ERROR";}
                    break;
                case CADENA:
                    switch (sr.type) {
                        case NUMERO:
                            {

                            }
                            break;
                        case DECIMAL:
                            {}
                            break;
                        case BOOLEANO:
                            {}
                            break;
                        case CARACTER:
                            {}
                            break;
                        case CADENA:{
                            /*error numero - cadena*/
                            }
                            break;
                        case ERROR:
                            {}
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    {s.type = ERROR;
                    s.value = "ERROR";}
                    break;
                }
            break;
            }

        case AND:{
          NodoAst left = raiz->l_hijos.at(0);
          Simbolo sl = recorrer(&left,h);
          NodoAst right = raiz->l_hijos.at(1);
          Simbolo sr = recorrer(&right,h);
          if(sl.type == BOOLEANO && sr.type == BOOLEANO){
             s.value = sl.value == "true" && sr.value =="true" ? "true" :"false";
             s.type = BOOLEANO;
          }
          else {
            s.type = ERROR;
            s.value = "ERROR";
            lerror.append(*new gError(3,s.row,s.column,"Condicion no es tipo booleano"));
          }
          break;
        }
        case OR:{
          NodoAst left = raiz->l_hijos.at(0);
          Simbolo sl = recorrer(&left,h);
          NodoAst right = raiz->l_hijos.at(1);
          Simbolo sr = recorrer(&right,h);
          if(sl.type == BOOLEANO && sr.type == BOOLEANO){
            s.value = sl.value == "true" || sr.value == "true" ? "true" : "false";
            s.type = BOOLEANO;
          }
          else {
              s.type = ERROR;
              s.value = "ERROR";
                          lerror.append(*new gError(3,s.row,s.column,"Condicion no es tipo booleano"));
          }
          break;
        }
        case NOT:{
            NodoAst nodo = raiz->l_hijos.at(0);
            Simbolo sl = recorrer(&nodo,h);
            if(sl.type==BOOLEANO){
                s.value = sl.value == "true" ? "false" : "true";
                s.type = BOOLEANO;
            }
            else{
                            lerror.append(*new gError(3,s.row,s.column,"Condicion no es tipo booleano"));
            }
            break;
        }

        case IGUALLOGICO:{
          NodoAst left = raiz->l_hijos.at(0);
          Simbolo sl = recorrer(&left,h);
          NodoAst right = raiz->l_hijos.at(1);
          Simbolo sr = recorrer(&right,h);
            if(sl.type == NUMERO && sr.type == NUMERO){
                s.type = BOOLEANO;
                s.value = sl.value.toInt() == sr.value.toInt() ? "true" : "false";
            }
            else if(sl.type== DECIMAL && sr.type ==DECIMAL){
                s.type = BOOLEANO;
                s.value = sl.value.toFloat() == sr.value.toFloat() ? "true" : "false";
            }
            else if(sl.type == BOOLEANO && sr.type == BOOLEANO){
                s.type = BOOLEANO;
                s.value = sl.value == sr.value ? "true" : "false";
            }
            else if(sl.type == CADENA && sr.type == CADENA){
                s.type = BOOLEANO;
                s.value = sl.value == sr.value ? "true" : "false";
            }
            else if(sl.type == CARACTER && sr.type ==CARACTER){
                s.type = BOOLEANO;
                s.value = sl.value == sr.value ? "true" : "false";
            }
            else{
                s.type = ERROR;
                s.value = "ERROR";
                            lerror.append(*new gError(3,s.row,s.column,"tipos de datos diferentes"));
            }
          break;
        }
        case DESIGUAL:{
            NodoAst left = raiz->l_hijos.at(0);
            Simbolo sl = recorrer(&left,h);
            NodoAst right = raiz->l_hijos.at(1);
            Simbolo sr = recorrer(&right,h);
              if(sl.type == NUMERO && sr.type == NUMERO){
                  s.type = BOOLEANO;
                  s.value = sl.value.toInt() != sr.value.toInt() ? "true" : "false";
              }
              else if(sl.type== DECIMAL && sr.type ==DECIMAL){
                  s.type = BOOLEANO;
                  s.value = sl.value.toFloat() != sr.value.toFloat() ? "true" : "false";
              }
              else if(sl.type == BOOLEANO && sr.type == BOOLEANO){
                  s.type = BOOLEANO;
                  s.value = sl.value != sr.value ? "true" : "false";
              }
              else if(sl.type == CADENA && sr.type == CADENA){
                  s.type = BOOLEANO;
                  s.value = sl.value != sr.value ? "true" : "false";
              }
              else if(sl.type == CARACTER && sr.type ==CARACTER){
                  s.type = BOOLEANO;
                  s.value = sl.value != sr.value ? "true" : "false";
              }
              else{
                  s.type = ERROR;
                  s.value = "ERROR";
                  lerror.append(*new gError(3,s.row,s.column,"tipos de datos diferentes"));
              }
          break;
        }
        case MAYOR:{
            NodoAst left = raiz->l_hijos.at(0);
            Simbolo sl = recorrer(&left,h);
            NodoAst right = raiz->l_hijos.at(1);
            Simbolo sr = recorrer(&right,h);
              if(sl.type == NUMERO && sr.type == NUMERO){
                  s.type = BOOLEANO;
                  s.value = sl.value.toInt() > sr.value.toInt() ? "true" : "false";
              }
              else if(sl.type== DECIMAL && sr.type ==DECIMAL){
                  s.type = BOOLEANO;
                  s.value = sl.value.toFloat() > sr.value.toFloat() ? "true" : "false";
              }
              else if(sl.type == BOOLEANO && sr.type == BOOLEANO){
                  s.type = BOOLEANO;
                  s.value = sl.value > sr.value ? "true" : "false";
              }
              else if(sl.type == CADENA && sr.type == CADENA){
                  s.type = BOOLEANO;
                  s.value = sl.value > sr.value ? "true" : "false";
              }
              else if(sl.type == CARACTER && sr.type ==CARACTER){
                  s.type = BOOLEANO;
                  QChar c = sl.value.at(0); QChar c1 = sr.value.at(0);
                  int x1 = c.toLatin1(); int x2 = c1.toLatin1();
                  s.value = x1 > x2 ? "true" : "false";
              }
              else{
                  s.type = ERROR;
                  s.value = "ERROR";
                  lerror.append(*new gError(3,s.row,s.column,"tipos de datos diferentes"));
              }
          break;
        }
        case MAYORQUE:{
            NodoAst left = raiz->l_hijos.at(0);
            Simbolo sl = recorrer(&left,h);
            NodoAst right = raiz->l_hijos.at(1);
            Simbolo sr = recorrer(&right,h);
              if(sl.type == NUMERO && sr.type == NUMERO){
                  s.type = BOOLEANO;
                  s.value = sl.value.toInt() >= sr.value.toInt() ? "true" : "false";
              }
              else if(sl.type== DECIMAL && sr.type ==DECIMAL){
                  s.type = BOOLEANO;
                  s.value = sl.value.toFloat() >= sr.value.toFloat() ? "true" : "false";
              }
              else if(sl.type == BOOLEANO && sr.type == BOOLEANO){
                  s.type = BOOLEANO;
                  s.value = sl.value >= sr.value ? "true" : "false";
              }
              else if(sl.type == CADENA && sr.type == CADENA){
                  s.type = BOOLEANO;
                  s.value = sl.value >= sr.value ? "true" : "false";
              }
              else if(sl.type == CARACTER && sr.type ==CARACTER){
                  s.type = BOOLEANO;
                  QChar c = sl.value.at(0); QChar c1 = sr.value.at(0);
                  int x1 = c.toLatin1(); int x2 = c1.toLatin1();
                  s.value = x1 >= x2 ? "true" : "false";
              }
              else{
                  s.type = ERROR;
                  s.value = "ERROR";
                  lerror.append(*new gError(3,s.row,s.column,"tipos de datos diferentes"));
              }
          break;
        }
        case MENOR:{
            NodoAst left = raiz->l_hijos.at(0);
            Simbolo sl = recorrer(&left,h);
            NodoAst right = raiz->l_hijos.at(1);
            Simbolo sr = recorrer(&right,h);
              if(sl.type == NUMERO && sr.type == NUMERO){
                  s.type = BOOLEANO;
                  s.value = sl.value.toInt() < sr.value.toInt() ? "true" : "false";
              }
              else if(sl.type== DECIMAL && sr.type ==DECIMAL){
                  s.type = BOOLEANO;
                  s.value = sl.value.toFloat() < sr.value.toFloat() ? "true" : "false";
              }
              else if(sl.type == BOOLEANO && sr.type == BOOLEANO){
                  s.type = BOOLEANO;
                  s.value = sl.value < sr.value ? "true" : "false";
              }
              else if(sl.type == CADENA && sr.type == CADENA){
                  s.type = BOOLEANO;
                  s.value = sl.value < sr.value ? "true" : "false";
              }
              else if(sl.type == CARACTER && sr.type ==CARACTER){
                  s.type = BOOLEANO;
                  QChar c = sl.value.at(0); QChar c1 = sr.value.at(0);
                  int x1 = c.toLatin1(); int x2 = c1.toLatin1();
                  s.value = x1 < x2 ? "true" : "false";
              }
              else{
                  s.type = ERROR;
                  s.value = "ERROR";
                  lerror.append(*new gError(3,s.row,s.column,"tipos de datos diferentes"));
              }
              break;
        }
        case MENORQUE:{
            NodoAst left = raiz->l_hijos.at(0);
            Simbolo sl = recorrer(&left,h);
            NodoAst right = raiz->l_hijos.at(1);
            Simbolo sr = recorrer(&right,h);
              if(sl.type == NUMERO && sr.type == NUMERO){
                  s.type = BOOLEANO;
                  s.value = sl.value.toInt() <= sr.value.toInt() ? "true" : "false";
              }
              else if(sl.type== DECIMAL && sr.type ==DECIMAL){
                  s.type = BOOLEANO;
                  s.value = sl.value.toFloat() <= sr.value.toFloat() ? "true" : "false";
              }
              else if(sl.type == BOOLEANO && sr.type == BOOLEANO){
                  s.type = BOOLEANO;
                  s.value = sl.value <= sr.value ? "true" : "false";
              }
              else if(sl.type == CADENA && sr.type == CADENA){
                  s.type = BOOLEANO;
                  s.value = sl.value <= sr.value ? "true" : "false";
              }
              else if(sl.type == CARACTER && sr.type ==CARACTER){
                  s.type = BOOLEANO;
                  QChar c = sl.value.at(0); QChar c1 = sr.value.at(0);
                  int x1 = c.toLatin1(); int x2 = c1.toLatin1();
                  s.value = x1 <= x2 ? "true" : "false";
              }
              else{
                  s.type = ERROR;
                  s.value = "ERROR";
                  lerror.append(*new gError(3,s.row,s.column,"tipos de datos diferentes"));
              }
              break;
        }

        case RSHOW:{
          if(raiz->l_hijos.count()==1){
            NodoAst left = raiz->l_hijos.at(0);
            Simbolo sl = recorrer(&left,h);
            if(sl.type != ERROR){
              QMessageBox mj;
              mj.setText(sl.value);
              mj.exec();
            }
            else{
              s.type = ERROR;
              s.value = "ERROR";
              lerror.append(*new gError(3,s.row,s.column,"no se puede imprimir un error"));
            }
          }
          else{
            NodoAst left = raiz->l_hijos.at(0);
            Simbolo sl = recorrer(&left,h);
            NodoAst right = raiz->l_hijos.at(1);
            Simbolo sr = recorrer(&right,h);
            if(sl.type != ERROR && sr.type != ERROR){
              QMessageBox mj; mj.setWindowTitle(sl.value);
              mj.setText(sr.value);
              mj.exec();
            }
            else{
              s.type = ERROR;
              s.value = "ERROR";
            }
          }
          break;
        }
        case RIMPRIMIR:{
          NodoAst left = raiz->l_hijos.at(0);
          Simbolo sl = recorrer(&left,h);
          if(sl.type != ERROR){
            resultado.append(sl.value + "\n");
          }
          else{
            s.type = ERROR;
            s.value = "ERROR";
            lerror.append(*new gError(3,s.row,s.column,"tipos de datos diferentes"));
          }
          break;
        }

        case LID:{
            if(raiz->l_hijos.count()==2){
                NodoAst left = raiz->l_hijos.at(0);
                int tipo = 0;
                switch (left.getType()) {
                case RINT:
                    tipo = 1;
                    break;
                case RSTRING:
                    tipo = 2;
                    break;
                case RDOUBLE:
                    tipo = 4;
                    break;
                case RCHAR:
                    tipo = 5;
                    break;
                case RBOOL:
                    tipo = 3;
                    break;
                default:
                    tipo = -1;
                    break;
                }
                NodoAst right = raiz->l_hijos.at(1);
                foreach (NodoAst hijo, right.l_hijos) {
                    Variable *b = new Variable(tipo,hijo.row,hijo.column,hijo.value);
                    if(h->addVariable(hijo.value,*b)){

                    }
                    else{

                    }
                }
            }
            else if(raiz->l_hijos.count()==3){
                NodoAst left = raiz->l_hijos.at(0);
                NodoAst center = raiz->l_hijos.at(1);
                NodoAst right = raiz->l_hijos.at(2);
                Simbolo s = recorrer(&right,h);
                int tipo = 0;
                switch (left.getType()) {
                    case RINT:
                        tipo = 1;
                        break;
                    case RSTRING:
                        tipo = 2;
                        break;
                    case RDOUBLE:
                        tipo = 4;
                        break;
                    case RCHAR:
                        tipo = 5;
                        break;
                    case RBOOL:
                        tipo = 3;
                        break;
                    default:
                        tipo = -1;
                        break;
                }
                    foreach (NodoAst hijo, center.l_hijos) {
                        Variable *b = new Variable(tipo,hijo.row,hijo.column,s.value,hijo.value);
                        if(h->addVariable(hijo.value,*b)){

                        }
                        else{

                        }
                    }

            }
            break;
        }

        case LARRAY:{
            if(raiz->l_hijos.count()==3){
                NodoAst left = raiz->l_hijos.at(0);
                NodoAst center = raiz->l_hijos.at(1);
                NodoAst right = raiz->l_hijos.at(2);

                int dim = right.l_hijos.count();

                if(dim==1){
                    NodoAst n = right.l_hijos.at(0);
                    Simbolo s1 = recorrer(&n,h);
                    if(s1.type==NUMERO){
                        switch (left.getType()) {
                        case RINT:
                            {Arreglo *a = new Arreglo(11,s1.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(11,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RSTRING:
                            {Arreglo *a = new Arreglo(12,s1.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(12,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RDOUBLE:
                            {Arreglo *a = new Arreglo(15,s1.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(15,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RCHAR:
                            {Arreglo *a = new Arreglo(13,s1.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(13,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RBOOL:
                            {Arreglo *a = new Arreglo(14,s1.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(14,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        default:
                            /*ERROR*/
                            break;
                        }
                    }
                }
                else if(dim==2){
                    NodoAst n = right.l_hijos.at(0); NodoAst n1 = right.l_hijos.at(1);
                    Simbolo s1 = recorrer(&n,h);
                    Simbolo s2 = recorrer(&n1,h);
                    if(s1.type==NUMERO && s2.type == NUMERO){

                        switch (left.getType()) {
                        case RINT:
                            {Arreglo *a = new Arreglo(21,s1.value.toInt(),s2.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(21,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RSTRING:
                            {Arreglo *a = new Arreglo(22,s1.value.toInt(),s2.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(22,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RDOUBLE:
                            {Arreglo *a = new Arreglo(25,s1.value.toInt(),s2.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(25,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RCHAR:
                            {Arreglo *a = new Arreglo(23,s1.value.toInt(),s2.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(23,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RBOOL:
                            {Arreglo *a = new Arreglo(24,s1.value.toInt(),s2.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(24,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        default:

                            break;
                        }
                    }
                }
                else if(dim==3){
                    NodoAst n = right.l_hijos.at(0); NodoAst n1 = right.l_hijos.at(1); NodoAst n2 = right.l_hijos.at(2);
                    Simbolo s1 = recorrer(&n,h);
                    Simbolo s2 = recorrer(&n1,h);
                    Simbolo s3 = recorrer(&n2,h);
                    if(s1.type==NUMERO && s2.type==NUMERO && s3.type == NUMERO){
                        switch (left.getType()) {
                        case RINT:
                            {Arreglo *a = new Arreglo(31,s1.value.toInt(),s2.value.toInt(),s3.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(31,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RSTRING:
                            {Arreglo *a = new Arreglo(32,s1.value.toInt(),s2.value.toInt(),s3.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(32,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RDOUBLE:
                            {Arreglo *a = new Arreglo(35,s1.value.toInt(),s2.value.toInt(),s3.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(35,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RCHAR:
                            {Arreglo *a = new Arreglo(33,s1.value.toInt(),s2.value.toInt(),s3.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(33,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RBOOL:
                            {Arreglo *a = new Arreglo(34,s1.value.toInt(),s2.value.toInt(),s3.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(34,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){

                                }
                                else{

                                }
                            }
                        }
                            break;
                        default:

                            break;
                        }
                    }
                }
            }
            else if(raiz->l_hijos.count()==4){
                NodoAst left = raiz->l_hijos.at(0);
                NodoAst center = raiz->l_hijos.at(1);
                NodoAst right = raiz->l_hijos.at(2);
                NodoAst up = raiz->l_hijos.at(3);

                int dim = right.l_hijos.count();

                if(dim==1){
                    NodoAst n = right.l_hijos.at(0);
                    Simbolo s1 = recorrer(&n,h);
                    if(s1.type==NUMERO){
                        switch (left.getType()) {
                        case RINT:
                            {Arreglo *a = new Arreglo(11,s1.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(11,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    Arreglo *temp = &b->arr;
                                    int pos = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        Simbolo sim = recorrer(&n,h);
                                        if(sim.type==NUMERO && pos < temp->i){
                                            temp->int_a[pos]= sim.value.toInt();
                                            pos++;
                                        }
                                        else{
                                            break;
                                        }
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RSTRING:
                            {Arreglo *a = new Arreglo(12,s1.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(12,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    Arreglo *temp = &b->arr;
                                    int pos = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        Simbolo sim = recorrer(&n,h);
                                        if(sim.type==CADENA && pos < temp->i){
                                            temp->string_a[pos]= sim.value;
                                            pos++;
                                        }
                                        else{
                                            break;
                                        }
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RDOUBLE:
                            {Arreglo *a = new Arreglo(15,s1.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(15,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    Arreglo *temp = &b->arr;
                                    int pos = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        Simbolo sim = recorrer(&n,h);
                                        if(sim.type==DECIMAL&& pos < temp->i){
                                            temp->float_a[pos]= sim.value.toDouble();
                                            pos++;
                                        }
                                        else{
                                            break;
                                        }
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RCHAR:
                            {Arreglo *a = new Arreglo(13,s1.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(13,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    Arreglo *temp = &b->arr;
                                    int pos = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        Simbolo sim = recorrer(&n,h);
                                        if(sim.type==CARACTER && pos < temp->i){
                                            QChar c = sim.value.at(0);
                                            temp->char_a[pos]= c;
                                            pos++;
                                        }
                                        else{
                                            break;
                                        }
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RBOOL:
                            {Arreglo *a = new Arreglo(14,s1.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(14,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    Arreglo *temp = &b->arr;
                                    int pos = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        Simbolo sim = recorrer(&n,h);
                                        if(sim.type==BOOLEANO && pos < temp->i){
                                            temp->bool_a[pos]= sim.value == "true" ? true : false;
                                            pos++;
                                        }
                                        else{
                                            break;
                                        }                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        default:
                            /*ERROR*/
                            break;
                        }
                    }
                }
                else if(dim==2){
                    NodoAst n = right.l_hijos.at(0); NodoAst n1 = right.l_hijos.at(1);
                    Simbolo s1 = recorrer(&n,h);
                    Simbolo s2 = recorrer(&n1,h);
                    if(s1.type==NUMERO && s2.type == NUMERO){
                        switch (left.getType()) {
                        case RINT:
                            {Arreglo *a = new Arreglo(21,s1.value.toInt(),s2.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(21,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    Arreglo *temp = &b->arr;
                                    int posx = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        int posy = 0;
                                        foreach (NodoAst n1, n.l_hijos) {
                                            Simbolo sim = recorrer(&n1,h);
                                            if(sim.type==NUMERO && posx < temp->i && posy <temp->j){
                                                temp->int_aa[posx][posy] = sim.value.toInt();
                                            }
                                            else{
                                                break;
                                            }
                                            posy++;
                                        }
                                        posx++;
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RSTRING:
                            {Arreglo *a = new Arreglo(22,s1.value.toInt(),s2.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(22,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    Arreglo *temp = &b->arr;
                                    int posx = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        int posy = 0;
                                        foreach (NodoAst n1, n.l_hijos) {
                                            Simbolo sim = recorrer(&n1,h);
                                            if(sim.type==CADENA && posx < temp->i && posy <temp->j){
                                                temp->string_aa[posx][posy] = sim.value;
                                            }
                                            else{
                                                break;
                                            }
                                            posy++;
                                        }
                                        posx++;
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RDOUBLE:
                            {Arreglo *a = new Arreglo(25,s1.value.toInt(),s2.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(25,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    Arreglo *temp = &b->arr;
                                    int posx = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        int posy = 0;
                                        foreach (NodoAst n1, n.l_hijos) {
                                            Simbolo sim = recorrer(&n1,h);
                                            if(sim.type==DECIMAL && posx < temp->i && posy <temp->j){
                                                temp->float_aa[posx][posy] = sim.value.toFloat();
                                            }
                                            else{
                                                break;
                                            }
                                            posy++;
                                        }
                                        posx++;
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RCHAR:
                            {Arreglo *a = new Arreglo(23,s1.value.toInt(),s2.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(23,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    Arreglo *temp = &b->arr;
                                    int posx = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        int posy = 0;
                                        foreach (NodoAst n1, n.l_hijos) {
                                            Simbolo sim = recorrer(&n1,h);
                                            if(sim.type==CARACTER && posx < temp->i && posy <temp->j){
                                                QChar c = sim.value.at(0);
                                                temp->char_aa[posx][posy] = c;
                                            }
                                            else{
                                                break;
                                            }
                                            posy++;
                                        }
                                        posx++;
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RBOOL:
                            {Arreglo *a = new Arreglo(24,s1.value.toInt(),s2.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(24,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    Arreglo *temp = &b->arr;
                                    int posx = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        int posy = 0;
                                        foreach (NodoAst n1, n.l_hijos) {
                                            Simbolo sim = recorrer(&n1,h);
                                            if(sim.type==BOOLEANO && posx < temp->i && posy <temp->j){
                                                temp->bool_aa[posx][posy] = sim.value == "true" ? true : false;
                                            }
                                            else{
                                                break;
                                            }
                                            posy++;
                                        }
                                        posx++;
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        default:

                            break;
                        }
                    }
                }
                else if(dim==3){
                    NodoAst n = right.l_hijos.at(0); NodoAst n1 = right.l_hijos.at(1); NodoAst n2 = right.l_hijos.at(2);
                    Simbolo s1 = recorrer(&n,h);
                    Simbolo s2 = recorrer(&n1,h);
                    Simbolo s3 = recorrer(&n2,h);
                    if(s1.type==NUMERO && s2.type==NUMERO && s3.type == NUMERO){
                        switch (left.getType()) {
                        case RINT:
                            {Arreglo *a = new Arreglo(31,s1.value.toInt(),s2.value.toInt(),s3.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(31,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    int posx = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        int posy = 0;
                                        foreach (NodoAst n1, n.l_hijos) {
                                            int posz = 0;
                                            foreach (NodoAst n2, n1.l_hijos) {
                                                Simbolo sim = recorrer(&n2,h);
                                                if(sim.type == NUMERO && posx < a->i && posy <a->j && posz <a->k ){
                                                    a->int_aaa[posx][posy][posz] = sim.value.toInt();
                                                }
                                                else {
                                                    break;
                                                }
                                                posz++;
                                            }
                                            posy++;
                                        }
                                        posx++;
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RSTRING:
                            {Arreglo *a = new Arreglo(32,s1.value.toInt(),s2.value.toInt(),s3.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(32,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    int posx = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        int posy = 0;
                                        foreach (NodoAst n1, n.l_hijos) {
                                            int posz = 0;
                                            foreach (NodoAst n2, n1.l_hijos) {
                                                Simbolo sim = recorrer(&n2,h);
                                                if(sim.type == CADENA && posx < a->i && posy <a->j && posz <a->k){
                                                    a->string_aaa[posx][posy][posz] = sim.value;
                                                }
                                                else {
                                                    break;
                                                }
                                                posz++;
                                            }
                                            posy++;
                                        }
                                        posx++;
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RDOUBLE:
                            {Arreglo *a = new Arreglo(35,s1.value.toInt(),s2.value.toInt(),s3.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(35,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    int posx = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        int posy = 0;
                                        foreach (NodoAst n1, n.l_hijos) {
                                            int posz = 0;
                                            foreach (NodoAst n2, n1.l_hijos) {
                                                Simbolo sim = recorrer(&n2,h);
                                                if(sim.type == DECIMAL && posx < a->i && posy <a->j && posz <a->k){
                                                    a->float_aaa[posx][posy][posz] = sim.value.toFloat();
                                                }
                                                else {
                                                    break;
                                                }
                                                posz++;
                                            }
                                            posy++;
                                        }
                                        posx++;
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RCHAR:
                            {Arreglo *a = new Arreglo(33,s1.value.toInt(),s2.value.toInt(),s3.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(33,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    int posx = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        int posy = 0;
                                        foreach (NodoAst n1, n.l_hijos) {
                                            int posz = 0;
                                            foreach (NodoAst n2, n1.l_hijos) {
                                                Simbolo sim = recorrer(&n2,h);
                                                if(sim.type == CARACTER && posx < a->i && posy <a->j && posz <a->k){
                                                    QChar c = sim.value.at(0);
                                                    a->char_aaa[posx][posy][posz] = c;
                                                }
                                                else {
                                                    break;
                                                }
                                                posz++;
                                            }
                                            posy++;
                                        }
                                        posx++;
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        case RBOOL:
                        {Arreglo *a = new Arreglo(34,s1.value.toInt(),s2.value.toInt(),s3.value.toInt());
                            foreach (NodoAst hijo, center.l_hijos) {
                                Variable *b = new Variable(34,hijo.row,hijo.column,hijo.value,*a);
                                if(h->addVariable(hijo.value,*b)){
                                    int posx = 0;
                                    foreach (NodoAst n, up.l_hijos) {
                                        int posy = 0;
                                        foreach (NodoAst n1, n.l_hijos) {
                                            int posz = 0;
                                            foreach (NodoAst n2, n1.l_hijos) {
                                                Simbolo sim = recorrer(&n2,h);
                                                if(sim.type == BOOLEANO && posx < a->i && posy <a->j && posz <a->k){
                                                    a->bool_aaa[posx][posy][posz] = sim.value == "true" ? true : false;
                                                }
                                                else {
                                                    break;
                                                }
                                                posz++;
                                            }
                                            posy++;
                                        }
                                        posx++;
                                    }
                                }
                                else{

                                }
                            }
                        }
                            break;
                        default:

                            break;
                        }
                    }
                }

            }
            break;
        }

        case RREPETIR:{
            NodoAst left = raiz->l_hijos.at(0);
            NodoAst right = raiz->l_hijos.at(1);
            Simbolo sl = recorrer(&left,h);
            if(sl.type==NUMERO){
                int rep = sl.value.toInt();
                Entorno *h1 = new Entorno(h);
                while(rep>0){
                    foreach (NodoAst hijo, right.l_hijos) {
                        recorrer(&hijo,h1);
                    }
                    rep--;
                }
            }
            else{
                /*ERROR*/
                lerror.append(*new gError(3,s.row,s.column,"condicion repetir no es un numero"));
            }
            break;
        }

        case LISTIF:{
            if(raiz->l_hijos.count()==1){
                NodoAst hijo = raiz->l_hijos.at(0);
                NodoAst left = hijo.l_hijos.at(0);
                Simbolo s1 = recorrer(&left,h);
                if(s1.type== BOOLEANO){
                    if(s1.value == "true"){
                        Entorno *h1 = new Entorno(h);
                        NodoAst right = hijo.l_hijos.at(1);
                        recorrer(&right,h1);
                    }
                }
                else{
                    lerror.append(*new gError(3,s.row,s.column,"condicion if no es booleano"));
                }
            }
            else if(raiz->l_hijos.count()==2){
                NodoAst hijoi = raiz->l_hijos.at(0);
                NodoAst hijod = raiz->l_hijos.at(1);

                NodoAst left = hijoi.l_hijos.at(0);
                Simbolo s1 = recorrer(&left,h);
                if(s1.type==BOOLEANO){
                    if(s1.value == "true"){
                        Entorno *h1 = new Entorno(h);
                        NodoAst right = hijoi.l_hijos.at(1);
                        recorrer(&right,h1);
                    }
                    else{
                        if(hijod.getType()==RSINO){
                            Entorno *h1 = new Entorno(h);
                            NodoAst right = hijod.l_hijos.at(0);
                            recorrer(&right,h1);
                        }
                        else{
                            recorrer(&hijod,h);
                        }
                    }
                }
                else{
                    /*ERROR*/
                    lerror.append(*new gError(3,s.row,s.column,"condicion if no es booleano"));
                }
            }
            else if(raiz->l_hijos.count()==3){
                NodoAst hijo1 = raiz->l_hijos.at(0);
                NodoAst left = hijo1.l_hijos.at(0);
                Simbolo s1 = recorrer(&left,h);
                if(s1.type ==BOOLEANO){
                    if(s1.value == "true"){
                        Entorno *h1 = new Entorno(h);
                        NodoAst right = hijo1.l_hijos.at(1);
                        recorrer(&right,h1);
                    }
                    else{
                        NodoAst hijo2 = raiz->l_hijos.at(1);
                        Simbolo s1 = recorrer(&hijo2,h);
                        if(s1.value=="no"){
                            NodoAst hijo3 = raiz->l_hijos.at(2);
                            NodoAst in = hijo3.l_hijos.at(0);
                            Entorno *h1 = new Entorno(h);
                            recorrer(&in,h1);
                        }
                    }
                }
                else{
                    lerror.append(*new gError(3,s.row,s.column,"condicion if no es booleano"));
                }
            }
            break;
        }
        case LISTEIF:{
            s.type = BOOLEANO; s.value = "no";
            foreach (NodoAst hijo, raiz->l_hijos) {
                NodoAst left = hijo.l_hijos.at(0);
                Simbolo s1 = recorrer(&left,h);
                if(s1.type == BOOLEANO){
                    if(s1.value=="true"){
                        Entorno *h1 = new Entorno(h);
                        NodoAst right = hijo.l_hijos.at(1);
                        recorrer(&right,h1);
                        s.value ="si";
                        break;
                    }
                }
                else{
                    lerror.append(*new gError(3,s.row,s.column,"condicion if no es booleano"));
                    break;
                }
            }
            break;
        }

        case ASID:{
            NodoAst left = raiz->l_hijos.at(0);
            NodoAst right = raiz->l_hijos.at(1);
            Simbolo s1 = recorrer(&right,h);
            Variable b = h->getVariable(left.value);
            if(b.def){
                if(b.tipo == 1 && s1.type==NUMERO){
                    b.valor = s1.value; h->change(left.value,b);
                }
                else if(b.tipo == 2 && s1.type==CADENA){
                    b.valor = s1.value; h->change(left.value,b);
                }
                else if(b.tipo == 3 && s1.type==BOOLEANO){
                    b.valor = s1.value; h->change(left.value,b);
                }
                else if(b.tipo == 4 && s1.type==DECIMAL){
                    b.valor = s1.value; h->change(left.value,b);
                }
                else if(b.tipo == 5 && s1.type==CARACTER){
                    b.valor = s1.value; h->change(left.value,b);
                }
                else if(right.getType() == AUMENTO){
                    switch (b.tipo) {
                    case 1:
                        b.valor = QString::number(b.valor.toInt() + 1);
                        h->change(left.value,b);
                        break;
                    case 2:
                        /*error*/
                        break;
                    case 3:
                        /*error*/
                        break;
                    case 4:
                        b.valor = QString::number(b.valor.toFloat()+1);
                        h->change(left.value,b);
                        break;
                    case 5:{
                        QChar c = b.valor.at(0);
                        b.valor = QString::number(c.toLatin1() + 1);
                        h->change(left.value,b);}
                        break;
                    default:
                        break;
                    }
                }
                else if(right.getType() == DECREMENTO){
                    switch (b.tipo) {
                    case 1:
                        b.valor = QString::number(b.valor.toInt() - 1);
                        h->change(left.value,b);
                        break;
                    case 2:
                        /*error*/
                        break;
                    case 3:
                        /*error*/
                        break;
                    case 4:
                        b.valor = QString::number(b.valor.toFloat() - 1);
                        h->change(left.value,b);
                        break;
                    case 5:{
                        QChar c = b.valor.at(0);
                        b.valor = QString::number(c.toLatin1() - 1);
                        h->change(left.value,b);}
                        break;
                    default:
                        break;
                    }
                }
                else{
                    lerror.append(*new gError(3,s.row,s.column,"tipos de datos diferentes"));
                }
            }
            break;
        }

        case RPARA:{
            NodoAst left = raiz->l_hijos.at(0);
            NodoAst right = raiz->l_hijos.at(1);

            NodoAst hijo1 = left.l_hijos.at(0);//variable
            NodoAst hijo2 = left.l_hijos.at(1);//num
            NodoAst hijo3 = left.l_hijos.at(2);//condicion
            NodoAst hijo5 = left.l_hijos.at(4); //aumento - decremento

            Simbolo s1 = recorrer(&hijo2,h);
            if(s1.type == NUMERO){
                if(left.getType()==INPARA){
                    Variable *b = new Variable(1,hijo1.row,hijo1.column,s1.value,hijo1.value);
                    Entorno *h1 = new Entorno(h);
                    h1->addVariable(hijo1.value,*b);
                    int n1 = s1.value.toInt();
                    if(hijo5.getType()==AUMENTO){
                        Simbolo condicion = recorrer(&hijo3,h1);
                        b->valor = QString::number(n1); h1->change(hijo1.value,*b);
                        while(condicion.value=="true"){
                            Entorno *h2 = new Entorno(h1);
                            recorrer(&right,h2);
                            Variable temp = h1->getVariable(hijo1.value); temp.valor = QString::number(temp.valor.toInt()+1);
                            h1->change(hijo1.value,temp);
                            condicion = recorrer(&hijo3,h2);
                        }
                    }
                    else if(hijo5.getType()==DECREMENTO){
                        Simbolo condicion = recorrer(&hijo3,h1);
                        b->valor = QString::number(n1); h1->change(hijo1.value,*b);
                        while(condicion.value=="true"){
                            Entorno *h2 = new Entorno(h1);
                            recorrer(&right,h2);
                            Variable temp = h1->getVariable(hijo1.value); temp.valor = QString::number(temp.valor.toInt()-1);
                            h1->change(hijo1.value,temp);
                            condicion = recorrer(&hijo3,h2);
                        }
                    }
                }
                else if(left.getType()==INPARA2){
                    Variable b = h->getVariable(hijo1.value);
                    b.valor = s1.value;
                    bool s = h->change(hijo1.value,b);
                    if(s==true){
                        if(hijo5.getType()==AUMENTO){
                            Simbolo condicion = recorrer(&hijo3,h);
                            Variable *b1 = new Variable(1,hijo1.row,hijo1.column,s1.value,hijo1.value);
                            h->change(hijo1.value,*b1);
                            while(condicion.value=="true"){
                                Entorno *h1 = new Entorno(h);
                                recorrer(&right,h1);
                                Variable temp = h->getVariable(hijo1.value); temp.valor = QString::number(temp.valor.toInt()+1);
                                h->change(hijo1.value,temp);
                                condicion = recorrer(&hijo3,h1);
                            }
                        }
                        else if(hijo5.getType()==DECREMENTO){
                            Simbolo condicion = recorrer(&hijo3,h);
                            while(condicion.value=="true"){
                                Entorno *h1 = new Entorno(h);
                                recorrer(&right,h1);
                                Variable temp = h->getVariable(hijo1.value); temp.valor = QString::number(temp.valor.toInt()-1);
                                h->change(hijo1.value,temp);
                                condicion = recorrer(&hijo3,h1);
                            }
                        }
                    }
                    else{

                    }
                }
            }
            else{
                lerror.append(*new gError(3,s.row,s.column,"Condicion para no valida"));
            }
            break;
        }


        case ASARR:{
        int hijos = raiz->l_hijos.count();
        if(hijos==3){
            NodoAst id = raiz->l_hijos.at(0);
            Variable b = h->getVariable(id.value);
            NodoAst pos = raiz->l_hijos.at(1); Simbolo spos = recorrer(&pos,h);
            NodoAst val = raiz->l_hijos.at(2); Simbolo sval = recorrer(&val,h);
            if(spos.type == NUMERO){
                Arreglo *ar = &b.arr;
                if(spos.value.toInt()<ar->i){
                    switch (ar->tipo) {
                    case 11:
                        ar->int_a[spos.value.toInt()] = sval.value.toInt();
                        break;
                    case 12:
                        ar->string_a[spos.value.toInt()] = sval.value;
                        break;
                    case 13:
                        {QChar c = sval.value.at(0);
                        ar->char_a[spos.value.toInt()] = c;}
                        break;
                    case 14:
                        ar->bool_a[spos.value.toInt()] = sval.value == "true" ? true : false;
                        break;
                    case 15:
                        ar->float_a[spos.value.toInt()] = sval.value.toFloat();
                        break;
                    default:
                        break;
                    }
                }
                else{
                    /*fuera de intervalo*/
                }
            }
            else{
                /*ERROR*/
            }
        }
        else if(hijos==4){
            NodoAst id = raiz->l_hijos.at(0);
            Variable b = h->getVariable(id.value);
            NodoAst posx = raiz->l_hijos.at(1); Simbolo sposx = recorrer(&posx,h);
            NodoAst posy = raiz->l_hijos.at(2); Simbolo sposy = recorrer(&posy,h);
            NodoAst val = raiz->l_hijos.at(3); Simbolo sval = recorrer(&val,h);
            if(sposx.type == NUMERO && sposy.type==NUMERO){
                Arreglo *ar = &b.arr;
                if(sposx.value.toInt()<ar->i && sposy.value.toInt()<ar->j){
                    switch (ar->tipo) {
                    case 21:
                        ar->int_aa[sposx.value.toInt()][sposy.value.toInt()] = sval.value.toInt();
                        break;
                    case 22:
                        ar->string_aa[sposx.value.toInt()][sposy.value.toInt()] = sval.value;
                        break;
                    case 23:
                        {QChar c = sval.value.at(0);
                        ar->char_aa[sposx.value.toInt()][sposy.value.toInt()] = c;}
                        break;
                    case 24:
                        ar->bool_aa[sposx.value.toInt()][sposy.value.toInt()] = sval.value == "true" ? true : false;
                        break;
                    case 25:
                        ar->float_aa[sposx.value.toInt()][sposy.value.toInt()] = sval.value.toFloat();
                        break;
                    default:
                        break;
                    }
                }
                else{
                    /*fuera de intervalo*/
                }
            }
            else{
                /*ERROR*/
            }
        }
        else{
            NodoAst id = raiz->l_hijos.at(0);
            Variable b = h->getVariable(id.value);
            NodoAst posx = raiz->l_hijos.at(1); Simbolo sposx = recorrer(&posx,h);
            NodoAst posy = raiz->l_hijos.at(2); Simbolo sposy = recorrer(&posy,h);
            NodoAst posz = raiz->l_hijos.at(3); Simbolo sposz = recorrer(&posz,h);
            NodoAst val = raiz->l_hijos.at(4); Simbolo sval = recorrer(&val,h);
            if(sposx.type == NUMERO && sposy.type==NUMERO && sposz.type ==NUMERO){
                Arreglo *ar = &b.arr;
                if(sposx.value.toInt()<ar->i && sposy.value.toInt()<ar->j && sposz.value.toInt()<ar->k){
                    switch (ar->tipo) {
                    case 31:
                        ar->int_aaa[sposx.value.toInt()][sposy.value.toInt()][sposz.value.toInt()] = sval.value.toInt();
                        break;
                    case 32:
                        ar->string_aaa[sposx.value.toInt()][sposy.value.toInt()][sposz.value.toInt()] = sval.value;
                        break;
                    case 33:
                        {QChar c = sval.value.at(0);
                        ar->char_aaa[sposx.value.toInt()][sposy.value.toInt()][sposz.value.toInt()] = c;}
                        break;
                    case 34:
                        ar->bool_aaa[sposx.value.toInt()][sposy.value.toInt()][sposz.value.toInt()] = sval.value == "true" ? true : false;
                        break;
                    case 35:
                        ar->float_aaa[sposx.value.toInt()][sposy.value.toInt()][sposz.value.toInt()] = sval.value.toFloat();
                        break;
                    default:
                        break;
                    }
                }
                else {
                    /*fuera de intervalo*/
                }
            }
            else{
                /*ERROR*/
            }

        }
        break;
        }
    }
    return s;
}


/* int = 1, string =2, boolean = 3, double = 4, char = 5, error = -1
*/


/*
    ERROR = -1, DEFAULT=0, RID=100, RINT=101, RSTRING=102, RDOUBLE=103, RCHAR=104,RBOOL=105, RARRAY=106,
          RIMPRIMIR = 107, RSHOW = 108, RSI = 109, RSINO=110, RPARA=111,RREPETIR=112, RSINOSI = 113,
          NUMERO = 201, DECIMAL = 202, BOOLEANO = 203, CARACTER = 204, CADENA = 205, DATOMATRIZ = 206, POSARR = 207,
          IGUALLOGICO = 1, DESIGUAL = 2, MAYOR = 3, MAYORQUE =4, MENOR = 5, MENORQUE =6, AND = 7, OR = 8, NOT = 9,MAS=10,MENOS=11,POR=12,DIV=13,
          POTENCIA = 14, AUMENTO = 15, DECREMENTO = 16, IGUAL = 17, LISTID = 113, PRINCIPAL = 666,LID = 667,LARRAY = 668,ASID = 669,ASARR=670,INPARA=671,
          DIM_ARRAY=672, LISTCOLUMN=673, LISTROW = 674, LISTIF=675, LISTEIF = 676
*/
