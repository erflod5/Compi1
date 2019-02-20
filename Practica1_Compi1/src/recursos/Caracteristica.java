package recursos;

/**
 *
 * @author Erik
 */
public class Caracteristica {

    private String nombre, valor;
    private int global;

    public Caracteristica(String nombre, String valor,int global) {
        this.nombre = nombre;
        this.valor = valor;
        this.global = global;
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
    
}
