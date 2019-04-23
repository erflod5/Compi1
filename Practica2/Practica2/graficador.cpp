#include "graficador.h"
#include <QString>
#include <fstream>
#include <iostream>
#include <QtWidgets>

Graficador::Graficador()
{
    this->count = 0;
}

Graficador::Graficador(NodoAst *raiz){
    this->dot = "";
    this->raiz = raiz;
    this->count = 0;
}

QString Graficador::graficar(){
    dot = "digraph G{";
    dot += "node[shape=\"box\"];";
    dot += "NodoAST0[label=\"" + escapar(raiz->tipo +"\n ("+ QString::number(raiz->row)+"-"+ QString::number(raiz->column)+")"+"\n"+raiz->value)  + "\"];\n";
    this->count = 1;
    recorrer("NodoAST0", raiz);
    dot += "}";
    return dot;
}

void Graficador::recorrer(QString padre, NodoAst *hijo){
     for (int x = 0 ; x < hijo->l_hijos.count() ; x++)
     {
         NodoAst temp = hijo->l_hijos[x];
         QString nombreHijo = "NodoAST" +  QString::number(count);//  this->contador;
         dot += nombreHijo + "[label=\"" + escapar(temp.tipo +"\n("+ QString::number(temp.row)+"-"+ QString::number(temp.column)+")\n"+temp.value)  + "\"];\n";
         dot += padre + "->" + nombreHijo + ";\n";
         count++;
         recorrer(nombreHijo, &temp);
    }
}

QString Graficador::escapar(QString cadena){
   cadena = cadena.replace("\\", "\\\\");
   cadena = cadena.replace("\"", "\\\"");
   return cadena;
}

void Graficador::generarImagen(){
        QFileInfo fi("temp");
        graficar();
        QString grafoDOT = this->dot;
        QString path = fi.absolutePath() +"/";
        QFile qFile(path+"grafo.txt");
        if(qFile.open(QIODevice::WriteOnly))
        {
            QTextStream out(&qFile); out << grafoDOT;
            qFile.close();
        }
        QString cadenaComando = "dot -Tjpg " + path + "grafo.txt -o " + path+"grafo.jpg ";
        std::cout << cadenaComando.toStdString() << "\n" << endl;
        system(cadenaComando.toUtf8().constData());
}
