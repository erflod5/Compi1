/********************************************************************************
** Form generated from reading UI file 'mainwindow.ui'
**
** Created by: Qt User Interface Compiler version 5.9.5
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_MAINWINDOW_H
#define UI_MAINWINDOW_H

#include <QtCore/QVariant>
#include <QtWidgets/QAction>
#include <QtWidgets/QApplication>
#include <QtWidgets/QButtonGroup>
#include <QtWidgets/QGridLayout>
#include <QtWidgets/QHeaderView>
#include <QtWidgets/QMainWindow>
#include <QtWidgets/QMenu>
#include <QtWidgets/QMenuBar>
#include <QtWidgets/QStatusBar>
#include <QtWidgets/QTabWidget>
#include <QtWidgets/QTableWidget>
#include <QtWidgets/QTextBrowser>
#include <QtWidgets/QTextEdit>
#include <QtWidgets/QWidget>

QT_BEGIN_NAMESPACE

class Ui_MainWindow
{
public:
    QAction *actionCrear;
    QAction *actionAbrir;
    QAction *actionGuardar;
    QAction *actionGuardar_Como;
    QAction *actionErrores;
    QAction *actionGenerador_AST;
    QAction *actionCompilar;
    QWidget *centralWidget;
    QWidget *gridLayoutWidget;
    QGridLayout *gridLayout;
    QTextEdit *txtEditor;
    QWidget *gridLayoutWidget_2;
    QGridLayout *gridLayout_2;
    QTabWidget *tabWidget;
    QWidget *tab;
    QTextBrowser *textBrowser;
    QWidget *tab_2;
    QTableWidget *tableWidget;
    QMenuBar *menuBar;
    QMenu *menuArchivo;
    QMenu *menuCompilar;
    QMenu *menuReportes;
    QStatusBar *statusBar;

    void setupUi(QMainWindow *MainWindow)
    {
        if (MainWindow->objectName().isEmpty())
            MainWindow->setObjectName(QStringLiteral("MainWindow"));
        MainWindow->resize(1383, 825);
        actionCrear = new QAction(MainWindow);
        actionCrear->setObjectName(QStringLiteral("actionCrear"));
        actionAbrir = new QAction(MainWindow);
        actionAbrir->setObjectName(QStringLiteral("actionAbrir"));
        actionGuardar = new QAction(MainWindow);
        actionGuardar->setObjectName(QStringLiteral("actionGuardar"));
        actionGuardar_Como = new QAction(MainWindow);
        actionGuardar_Como->setObjectName(QStringLiteral("actionGuardar_Como"));
        actionErrores = new QAction(MainWindow);
        actionErrores->setObjectName(QStringLiteral("actionErrores"));
        actionGenerador_AST = new QAction(MainWindow);
        actionGenerador_AST->setObjectName(QStringLiteral("actionGenerador_AST"));
        actionCompilar = new QAction(MainWindow);
        actionCompilar->setObjectName(QStringLiteral("actionCompilar"));
        centralWidget = new QWidget(MainWindow);
        centralWidget->setObjectName(QStringLiteral("centralWidget"));
        gridLayoutWidget = new QWidget(centralWidget);
        gridLayoutWidget->setObjectName(QStringLiteral("gridLayoutWidget"));
        gridLayoutWidget->setGeometry(QRect(90, 10, 1061, 501));
        gridLayout = new QGridLayout(gridLayoutWidget);
        gridLayout->setSpacing(6);
        gridLayout->setContentsMargins(11, 11, 11, 11);
        gridLayout->setObjectName(QStringLiteral("gridLayout"));
        gridLayout->setContentsMargins(0, 0, 0, 0);
        txtEditor = new QTextEdit(gridLayoutWidget);
        txtEditor->setObjectName(QStringLiteral("txtEditor"));

        gridLayout->addWidget(txtEditor, 0, 0, 1, 1);

        gridLayoutWidget_2 = new QWidget(centralWidget);
        gridLayoutWidget_2->setObjectName(QStringLiteral("gridLayoutWidget_2"));
        gridLayoutWidget_2->setGeometry(QRect(90, 520, 1061, 261));
        gridLayout_2 = new QGridLayout(gridLayoutWidget_2);
        gridLayout_2->setSpacing(6);
        gridLayout_2->setContentsMargins(11, 11, 11, 11);
        gridLayout_2->setObjectName(QStringLiteral("gridLayout_2"));
        gridLayout_2->setContentsMargins(0, 0, 0, 0);
        tabWidget = new QTabWidget(gridLayoutWidget_2);
        tabWidget->setObjectName(QStringLiteral("tabWidget"));
        tab = new QWidget();
        tab->setObjectName(QStringLiteral("tab"));
        textBrowser = new QTextBrowser(tab);
        textBrowser->setObjectName(QStringLiteral("textBrowser"));
        textBrowser->setGeometry(QRect(10, 10, 1031, 201));
        tabWidget->addTab(tab, QString());
        tab_2 = new QWidget();
        tab_2->setObjectName(QStringLiteral("tab_2"));
        tableWidget = new QTableWidget(tab_2);
        tableWidget->setObjectName(QStringLiteral("tableWidget"));
        tableWidget->setGeometry(QRect(10, 10, 1031, 211));
        tabWidget->addTab(tab_2, QString());

        gridLayout_2->addWidget(tabWidget, 0, 0, 1, 1);

        MainWindow->setCentralWidget(centralWidget);
        menuBar = new QMenuBar(MainWindow);
        menuBar->setObjectName(QStringLiteral("menuBar"));
        menuBar->setGeometry(QRect(0, 0, 1383, 22));
        menuArchivo = new QMenu(menuBar);
        menuArchivo->setObjectName(QStringLiteral("menuArchivo"));
        menuCompilar = new QMenu(menuBar);
        menuCompilar->setObjectName(QStringLiteral("menuCompilar"));
        menuReportes = new QMenu(menuBar);
        menuReportes->setObjectName(QStringLiteral("menuReportes"));
        MainWindow->setMenuBar(menuBar);
        statusBar = new QStatusBar(MainWindow);
        statusBar->setObjectName(QStringLiteral("statusBar"));
        MainWindow->setStatusBar(statusBar);

        menuBar->addAction(menuArchivo->menuAction());
        menuBar->addAction(menuCompilar->menuAction());
        menuBar->addAction(menuReportes->menuAction());
        menuArchivo->addAction(actionCrear);
        menuArchivo->addSeparator();
        menuArchivo->addAction(actionAbrir);
        menuArchivo->addSeparator();
        menuArchivo->addAction(actionGuardar);
        menuArchivo->addSeparator();
        menuArchivo->addAction(actionGuardar_Como);
        menuArchivo->addSeparator();
        menuCompilar->addAction(actionCompilar);
        menuReportes->addAction(actionErrores);
        menuReportes->addSeparator();
        menuReportes->addAction(actionGenerador_AST);
        menuReportes->addSeparator();

        retranslateUi(MainWindow);

        tabWidget->setCurrentIndex(1);


        QMetaObject::connectSlotsByName(MainWindow);
    } // setupUi

    void retranslateUi(QMainWindow *MainWindow)
    {
        MainWindow->setWindowTitle(QApplication::translate("MainWindow", "MainWindow", Q_NULLPTR));
        actionCrear->setText(QApplication::translate("MainWindow", "Crear", Q_NULLPTR));
        actionAbrir->setText(QApplication::translate("MainWindow", "Abrir", Q_NULLPTR));
        actionGuardar->setText(QApplication::translate("MainWindow", "Guardar", Q_NULLPTR));
        actionGuardar_Como->setText(QApplication::translate("MainWindow", "Guardar Como", Q_NULLPTR));
        actionErrores->setText(QApplication::translate("MainWindow", "Errores", Q_NULLPTR));
        actionGenerador_AST->setText(QApplication::translate("MainWindow", "Generador AST", Q_NULLPTR));
        actionCompilar->setText(QApplication::translate("MainWindow", "Compilar", Q_NULLPTR));
        tabWidget->setTabText(tabWidget->indexOf(tab), QApplication::translate("MainWindow", "Consola", Q_NULLPTR));
        tabWidget->setTabText(tabWidget->indexOf(tab_2), QApplication::translate("MainWindow", "Variables", Q_NULLPTR));
        menuArchivo->setTitle(QApplication::translate("MainWindow", "Archivo", Q_NULLPTR));
        menuCompilar->setTitle(QApplication::translate("MainWindow", "Compilar", Q_NULLPTR));
        menuReportes->setTitle(QApplication::translate("MainWindow", "Reportes", Q_NULLPTR));
    } // retranslateUi

};

namespace Ui {
    class MainWindow: public Ui_MainWindow {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_MAINWINDOW_H
