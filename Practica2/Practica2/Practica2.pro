#-------------------------------------------------
#
# Project created by QtCreator 2019-04-01T21:55:25
#
#-------------------------------------------------

QT       += core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = Practica2
TEMPLATE = app

# The following define makes your compiler emit warnings if you use
# any feature of Qt which has been marked as deprecated (the exact warnings
# depend on your compiler). Please consult the documentation of the
# deprecated API in order to know how to port your code away from it.
DEFINES += QT_DEPRECATED_WARNINGS

# You can also make your code fail to compile if you use deprecated APIs.
# In order to do so, uncomment the following line.
# You can also select to disable deprecated APIs only up to a certain version of Qt.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0


SOURCES += \
        main.cpp \
        mainwindow.cpp \
    scanner.cpp \
    parser.cpp \
    nodoast.cpp \
    recorrido.cpp \
    graficador.cpp \
    variable.cpp \
    entorno.cpp \
    simbolo.cpp \
    arreglo.cpp \
    gerror.cpp

HEADERS += \
        mainwindow.h \
    scanner.h \
    parser.h \
    nodoast.h \
    recorrido.h \
    graficador.h \
    variable.h \
    entorno.h \
    simbolo.h \
    arreglo.h \
    gerror.h

FORMS += \
        mainwindow.ui

DISTFILES +=
