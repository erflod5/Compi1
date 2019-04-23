/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package statement;

import java.util.ArrayList;

/**
 *
 * @author Erik
 */
public class St_If implements St {

    private boolean condicion;
    private ArrayList<St> listst;

    public St_If(boolean condicion, ArrayList<St> listst){
        this.condicion = condicion;
        this.listst = listst;
    }

    public boolean isCondicion() {
        return condicion;
    }

    public void setCondicion(boolean condicion) {
        this.condicion = condicion;
    }


    public ArrayList<St> getListst() {
        return listst;
    }

    public void setListst(ArrayList<St> listst) {
        this.listst = listst;
    }

    @Override
    public String ejecutar(javax.swing.JTextArea txt_console) {
        StringBuilder cadena = new StringBuilder();
        listst.stream().forEach((st) -> {
            if(st instanceof Var){
                cadena.append(st.ejecutar(txt_console));
            }
            else if(st instanceof Echo){
                cadena.append(st.ejecutar(txt_console));
            }
            else if(st instanceof St_If){
                if(((St_If)st).isCondicion())
                    cadena.append(st.ejecutar(txt_console));
            }
            else if(st instanceof St_IfElse){
                if(((St_If)st).isCondicion())
                    cadena.append(st.ejecutar(txt_console));
            }
            else if(st instanceof St_While){
                for(int i=0; i<((St_While)st).getRep();i++){
                    cadena.append(st.ejecutar(txt_console));
                }
            }
            else if(st instanceof Insert){
                cadena.append(st.ejecutar(txt_console));
            }
        });
        return cadena.toString();
    }

}
