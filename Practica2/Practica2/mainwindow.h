#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include "graficador.h"
#include "recorrido.h"
#include <QHash>

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow();

private slots:
    void on_actionCrear_triggered();

    void on_actionAbrir_triggered();

    void on_actionGuardar_triggered();

    void on_actionGuardar_Como_triggered();

    void on_actionCompilar_triggered();

    void on_actionGenerador_AST_triggered();


    void on_actionErrores_triggered();

private:
    Ui::MainWindow *ui;
    QString f;
    void save();
    void addTable(QHash<QString,Variable> tablaSim);
    void generarPagina();
};

#endif // MAINWINDOW_H
