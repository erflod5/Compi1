/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package tarea4;

import java.io.FileReader;
/**
 *
 * @author Erik
 */
public class Tarea4 {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        interpretar("entrada.txt");
        
    }

    private static void interpretar(String path){
        analizador.parser parse;
        try{
          parse = new analizador.parser(new analizador.Lexer(new FileReader(path)));
          parse.parse();
        }
        catch(Exception ex){
          System.err.println("Error fatal en la compilacion");
        }
    }
}
