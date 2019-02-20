package recursos;

/**
 *
 * @author Erik
 */
public class Error {

    private int fila,columna;
    private String tipo,error;

    public Error(int fila, int columna, String tipo, String error) {
        this.fila = fila;
        this.columna = columna;
        this.tipo = tipo;
        this.error = error;
    }

    public Error() {
        this(0,0,"","");
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

    public String getTipo() {
        return tipo;
    }

    public void setTipo(String tipo) {
        this.tipo = tipo;
    }

    public String getError() {
        return error;
    }

    public void setError(String error) {
        this.error = error;
    }
    
    
    
}
