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
public class Lista_St {

    private ArrayList<St> stlist;

    public Lista_St() {
    }

    public Lista_St(ArrayList<St> stlist) {
        this.stlist = stlist;
    }

    public Lista_St(St st){
        stlist = new ArrayList<>();
        stlist.add(st);
    }

    public void add(St st){
        stlist.add(st);
    }
}
