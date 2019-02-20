package recursos;

public class Variable {
    private String tipoVariable, nombreVariable, valorString;
    private int valorEntero;

    public Variable() {
        this("","","",0);
    }

    public Variable(String tipoVariable, String nombreVariable, String valorString, int valorEntero) {
        this.tipoVariable = tipoVariable;
        this.nombreVariable = nombreVariable;
        this.valorString = valorString;
        this.valorEntero = valorEntero;
    }

    public String getTipoVariable() {
        return tipoVariable;
    }

    public void setTipoVariable(String tipoVariable) {
        this.tipoVariable = tipoVariable;
    }

    public String getNombreVariable() {
        return nombreVariable;
    }

    public void setNombreVariable(String nombreVariable) {
        this.nombreVariable = nombreVariable;
    }

    public String getValorString() {
        return valorString;
    }

    public void setValorString(String valorString) {
        this.valorString = valorString;
    }

    public int getValorEntero() {
        return valorEntero;
    }

    public void setValorEntero(int valorEntero) {
        this.valorEntero = valorEntero;
    }
    
    
}
