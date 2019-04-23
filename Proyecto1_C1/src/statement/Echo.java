/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package statement;

/**
 *
 * @author Erik
 */
public class Echo implements St{

    private String cadena;

    public Echo() {
    }

    public Echo(String cadena) {
        this.cadena = cadena;
    }

    public String getCadena() {
        return cadena;
    }

    public void setCadena(String cadena) {
        this.cadena = cadena;
    }

    @Override
    public String ejecutar(javax.swing.JTextArea txt_console) {
        txt_console.append("-> "+cadena + "\n");
        return "";
    }

}
