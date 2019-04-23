/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Pojos;

import java.util.ArrayList;

/**
 *
 * @author Erik
 */
public class Tabla {

    private boolean borde;
    private ArrayList<ArrayList<String>> lst;

    public Tabla(){
      this.borde = false;
      this.lst = null;
    }

    public Tabla(boolean borde, ArrayList<ArrayList<String>> lst){
      this.borde = borde;
      this.lst = lst;
    }

    public Tabla(ArrayList<ArrayList<String>> lst){
      this.lst = lst;
      this.borde = false;
    }

    public boolean getBorde(){
        return borde;
    }

    public void setBorde(boolean borde){
        this.borde = borde;
    }

    public String getHtml(){
        StringBuilder bf = new StringBuilder();
        bf.append("\n<table border=").append(this.borde).append(">\n");
        lst.stream().forEach((st)->{
            bf.append("<tr>\n");
            st.stream().forEach((col)->{
                bf.append("<th>");
                bf.append(col);
                bf.append("</th>\n");
            });
            bf.append("</tr>\n");
        });
        bf.append("</table>\n");
        return bf.toString();
    }
}
