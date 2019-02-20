package recursos;

/**
 *
 * @author Erik
 */
public class Caracteristica {

    private String nombre, valor;
    private int global,fila,columna;

    public Caracteristica(String nombre, String valor,int global) {
        this.nombre = nombre;
        this.valor = valor;
        this.global = global;
    }
    
    public Caracteristica(String nombre, String valor, int global,int fila,int columna){
        this.nombre = nombre;
        this.valor = valor;
        this.global = global;
        this.fila = fila;
        this.columna = columna;
    }
    
    public Caracteristica(){
        this("","",-1);
    }

    public String getNombre() {
        return nombre;
    }

    public void setNombre(String nombre) {
        this.nombre = nombre;
    }

    public String getValor() {
        return valor;
    }

    public void setValor(String valor) {
        this.valor = valor;
    }

    public int getGlobal() {
        return global;
    }

    public void setGlobal(int global) {
        this.global = global;
    }

    public int getFila() {
        return fila;
    }

    public void setFila(int fila) {
        this.fila = fila;
    }

    public int getColumna() {
        return columna;
    }

    public void setColumna(int columna) {
        this.columna = columna;
    }
    
}
