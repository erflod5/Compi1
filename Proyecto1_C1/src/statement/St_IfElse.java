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
public class St_IfElse implements St {

    private boolean cond;
    private ArrayList<St> listst;
    private ArrayList<St> listtE;

    public St_IfElse(boolean cond, ArrayList<St> listst, ArrayList<St> listE) {
        this.cond = cond;
        this.listst = listst;
        this.listtE = listE;
    }

    public boolean isCond() {
        return cond;
    }

    public void setCond(boolean cond) {
        this.cond = cond;
    }

    public ArrayList<St> getListst() {
        return listst;
    }

    public void setListst(ArrayList<St> listst) {
        this.listst = listst;
    }

    public ArrayList<St> getListtE() {
        return listtE;
    }

    public void setListtE(ArrayList<St> listtE) {
        this.listtE = listtE;
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
            else if(st instanceof St_IfElse){
                if(((St_IfElse)st).isCond())
                    cadena.append(st.ejecutar(txt_console));
                else
                    cadena.append(((St_IfElse)st).ejecutar1(txt_console));
            }
            else if(st instanceof Insert){
                cadena.append(st.ejecutar(txt_console));
            }
        });
        return cadena.toString();
    }
    public String ejecutar1(javax.swing.JTextArea txt_console){
        StringBuilder cadena = new StringBuilder();
        listtE.stream().forEach((st) -> {
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
            else if(st instanceof St_IfElse){
                if(((St_IfElse)st).isCond())
                    cadena.append(st.ejecutar(txt_console));
                else
                    cadena.append(((St_IfElse)st).ejecutar1(txt_console));
            }
            else if(st instanceof Insert){
                cadena.append(st.ejecutar(txt_console));
            }
        });
        return cadena.toString();
    }

}
