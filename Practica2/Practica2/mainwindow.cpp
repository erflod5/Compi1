#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QStandardItemModel>
#include <QFileDialog>
#include <QMessageBox>
#include "parser.h"
#include "scanner.h"
#include <iostream>
#include "nodoast.h"
#include "gerror.h"
#include <fstream>

extern int yyparse(); //
extern NodoAst * raiz;
extern int linea; // Linea del token
extern int columna; // Columna de los tokens
extern int yylineno;
extern QList<gError> lerror;

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    QStringList lt;
    lt<<"Linea"<<"Columna"<<"Id"<<"Tipo"<<"Valor";
    ui->tableWidget->setColumnCount(5);
    ui->tableWidget->setHorizontalHeaderLabels(lt);
    //ui->tableWidget->insertRow(ui->tableWidget->rowCount());
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::on_actionCrear_triggered()
{
    f = "";
    ui->txtEditor->clear();
}

void MainWindow::save(){
    QFile file(f);
    if(file.open(QFile::WriteOnly)){
        file.write(ui->txtEditor->toPlainText().toUtf8());
    }
    else{
        QMessageBox::warning(this,"TextEditor",tr("Error").arg(f).arg(file.errorString()));
    }
}

void MainWindow::on_actionAbrir_triggered()
{
    QString fileName = QFileDialog::getOpenFileName(this,
                                                    "TextEditor - Open file",
                                                    "/home/erik/Documentos",
                                                    "Text Files (*.*)");
    if(!fileName.isEmpty()){
        QFile file(fileName);
        if(file.open(QFile::ReadOnly)){
            ui->txtEditor->setPlainText(file.readAll());
        }
        else{
            QMessageBox::warning(this,"Text Editor","Error");
        }
        f = fileName;
    }
}

void MainWindow::on_actionGuardar_triggered()
{
    if(f.isEmpty()){
        on_actionGuardar_Como_triggered();
    }
    else{
        save();
    }
}

void MainWindow::on_actionGuardar_Como_triggered()
{
    QString fileName = QFileDialog::getSaveFileName(this,"TextEditor - Save as","/home/erik/Documentos","Text Files (*.olc)");
    if(!fileName.isEmpty()){
        f = fileName;
        save();
    }
}

void MainWindow::on_actionCompilar_triggered()
{   lerror.clear();
    ui->textBrowser->setText("");
    QString text = ui->txtEditor->toPlainText();
    YY_BUFFER_STATE buffer = yy_scan_string(text.toUtf8().constData());
    Tipo t = DECIMAL;
    std::cout<<t;
    if(yyparse()==0){
       Graficador *g = new Graficador(raiz);
       //g->generarImagen();
        Recorrido *r = new Recorrido();
        Entorno *n = new Entorno();
        r->recorrer(raiz,n);
        ui->textBrowser->append(r->resultado);
        addTable(n->tablaSim); 
    }
    generarPagina();
}

void MainWindow::addTable(QHash<QString,Variable> tablaSim){
    ui->tableWidget->setRowCount(0);
    foreach (Variable b, tablaSim) {
        ui->tableWidget->insertRow(ui->tableWidget->rowCount());

        ui->tableWidget->setItem(ui->tableWidget->rowCount()-1,0,new QTableWidgetItem(QString::number(b.fila)));
        ui->tableWidget->setItem(ui->tableWidget->rowCount()-1,1,new QTableWidgetItem(QString::number(b.columna)));
        ui->tableWidget->setItem(ui->tableWidget->rowCount()-1,2,new QTableWidgetItem(b.id));
        ui->tableWidget->setItem(ui->tableWidget->rowCount()-1,3,new QTableWidgetItem(QString::number(b.tipo)));
        ui->tableWidget->setItem(ui->tableWidget->rowCount()-1,4,new QTableWidgetItem(b.valor));
    }
}

void MainWindow::on_actionGenerador_AST_triggered()
{
    QFileInfo fi("temp");
    QString path =fi.absolutePath() +"/grafo.jpg";
    QString comando = "xdg-open " + path;
    system(comando.toUtf8().constData());
}

void MainWindow::generarPagina(){
    QString fi = "index.html";
    QFile file(fi);
    if(file.open(QFile::WriteOnly)){
        file.write("<html>\n <head>\n <meta charset=\"utf-8\" />\n");
        file.write("<title>Reporte de Tokens</title>\n");
                   file.write("<meta name=\"viewport\" content=\"initial-scale=1.0; maximum-scale=1.0; width=device-width;\">\n");
                   file.write("<link rel=\"stylesheet\" type=\"text/css\" href=\"css/main.css\">\n");
                   file.write("</head>\n");
                   file.write("<body>\n");
                   file.write("<div class=\"table-title\">\n");
                   file.write("<h3>Reporte de Errores</h3>\n");
                   file.write("</div>\n");
                   file.write("\n");
                   file.write("<table class=\"table-fill\">\n");
                   file.write("<thead>\n");
                   file.write("<th class=\"text-left\">Fila</th>\n");
                   file.write("<th class=\"text-left\">Columna</th>\n");
                   file.write("<th class=\"text-left\">Error</th>	\n");
                   file.write("<th class=\"text-left\">Tipo</th>	\n");
                   file.write("</tr>\n");
                   file.write("</thead>\n");
                   file.write("<tbody class=\"table-hover\">");
                   foreach (gError g, lerror) {
                       file.write("<tr>");

                       file.write("<td class=\"text-left\">"); file.write(QString::number(g.fila).toUtf8()); file.write("</td>");
                       file.write("<td class=\"text-left\">"); file.write(QString::number(g.columna).toUtf8()); file.write("</td>");
                       if(g.tipo==1){
                        file.write("<td class=\"text-left\">"); file.write(g.dato.toUtf8()); file.write("</td>");
                        file.write("<td class=\"text-left\"> Lexico </td>");
                       }
                       else if(g.tipo == 2){
                            QString tx = "No se esperaba: " + g.dato;
                            file.write("<td class=\"text-left\">"); file.write(tx.toUtf8()); file.write("</td>");
                            file.write("<td class=\"text-left\"> Sintactico </td>");
                       }
                       else{
                           file.write("<td class=\"text-left\">"); file.write(g.dato.toUtf8()); file.write("</td>");
                           file.write("<td class=\"text-left\"> Semantico </td>");
                       }
                       file.write("</tr>");
                   }
                   file.write("</tbody>\n </table>\n </body>\n </html>");
    }
}

void MainWindow::on_actionErrores_triggered()
{
    QFileInfo fi("temp");
    QString path =fi.absolutePath() +"/index.html";
    QString comando = "xdg-open " + path;
    system(comando.toUtf8().constData());
}
