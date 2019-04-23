/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package statement;

import Pojos.Variable;
import static proyecto1_c1.Principal.lista_variable;

/**
 *
 * @author Erik
 */
public class Var implements St {

    private Pojos.Variable vl;

    public Var(Variable vl) {
        this.vl = vl;
    }

    public Var() {
    }

    public Variable getVl() {
        return vl;
    }

    public void setVl(Variable vl) {
        this.vl = vl;
    }

    @Override
    public String ejecutar(javax.swing.JTextArea txt_console) {
        lista_variable.put(vl.getId(),vl);
        return "";
    }


}
