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
public class St_While implements St{

    private int rep;
    private ArrayList<St> listst;

    public St_While(int rep, ArrayList<St> listst) {
        this.rep = rep;
        this.listst = listst;
    }

    public ArrayList<St> getListst() {
        return listst;
    }

    public void setListst(ArrayList<St> listst) {
        this.listst = listst;
    }

    public int getRep() {
        return rep;
    }

    public void setRep(int rep) {
        this.rep = rep;
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
