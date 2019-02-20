package practica1_compi1;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileWriter;
import java.io.IOException;
import java.io.StringReader;
import java.util.Iterator;
import java.util.Scanner;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.JFileChooser;
import javax.swing.JOptionPane;
import javax.swing.filechooser.FileNameExtensionFilter;
import recursos.Grafica;
import recursos.GraficaBarras;
import recursos.GraficaLineas;
import recursos.Variable;

public class Principal extends javax.swing.JFrame {

    java.io.File archivo;
    public Principal() {
        initComponents();
    }

    @SuppressWarnings("unchecked")
    // <editor-fold defaultstate="collapsed" desc="Generated Code">//GEN-BEGIN:initComponents
    private void initComponents() {

        jPanel1 = new javax.swing.JPanel();
        jLabel1 = new javax.swing.JLabel();
        btn_Abrir = new javax.swing.JButton();
        btn_Guardar = new javax.swing.JButton();
        btn_GuardarComo = new javax.swing.JButton();
        btn_Reporte = new javax.swing.JButton();
        jPanel2 = new javax.swing.JPanel();
        jScrollPane2 = new javax.swing.JScrollPane();
        txt_Console = new javax.swing.JTextArea();
        btn_Analizar = new javax.swing.JButton();

        setDefaultCloseOperation(javax.swing.WindowConstants.EXIT_ON_CLOSE);
        setTitle("Practica 1");
        setLocation(new java.awt.Point(400, 200));
        setResizable(false);

        jLabel1.setFont(new java.awt.Font("Tahoma", 1, 18)); // NOI18N
        jLabel1.setLabelFor(jLabel1);
        jLabel1.setText("Practica 1 #201701066");
        jLabel1.setToolTipText("");
        jLabel1.setAlignmentX(0.5F);
        jLabel1.setCursor(new java.awt.Cursor(java.awt.Cursor.DEFAULT_CURSOR));

        btn_Abrir.setText("Abrir");
        btn_Abrir.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                btn_AbrirActionPerformed(evt);
            }
        });

        btn_Guardar.setText("Guardar");
        btn_Guardar.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                btn_GuardarActionPerformed(evt);
            }
        });

        btn_GuardarComo.setText("Guardar Como");
        btn_GuardarComo.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                btn_GuardarComoActionPerformed(evt);
            }
        });

        btn_Reporte.setText("Reporte de Errores");
        btn_Reporte.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                btn_ReporteActionPerformed(evt);
            }
        });

        javax.swing.GroupLayout jPanel1Layout = new javax.swing.GroupLayout(jPanel1);
        jPanel1.setLayout(jPanel1Layout);
        jPanel1Layout.setHorizontalGroup(
            jPanel1Layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(jPanel1Layout.createSequentialGroup()
                .addGap(194, 194, 194)
                .addComponent(jLabel1, javax.swing.GroupLayout.PREFERRED_SIZE, 218, javax.swing.GroupLayout.PREFERRED_SIZE)
                .addContainerGap(javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
            .addGroup(javax.swing.GroupLayout.Alignment.TRAILING, jPanel1Layout.createSequentialGroup()
                .addContainerGap(75, Short.MAX_VALUE)
                .addComponent(btn_Abrir)
                .addGap(18, 18, 18)
                .addComponent(btn_Guardar)
                .addGap(18, 18, 18)
                .addComponent(btn_GuardarComo)
                .addGap(18, 18, 18)
                .addComponent(btn_Reporte)
                .addGap(75, 75, 75))
        );
        jPanel1Layout.setVerticalGroup(
            jPanel1Layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(jPanel1Layout.createSequentialGroup()
                .addComponent(jLabel1, javax.swing.GroupLayout.PREFERRED_SIZE, 29, javax.swing.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.RELATED)
                .addGroup(jPanel1Layout.createParallelGroup(javax.swing.GroupLayout.Alignment.BASELINE)
                    .addComponent(btn_Abrir)
                    .addComponent(btn_Guardar)
                    .addComponent(btn_GuardarComo)
                    .addComponent(btn_Reporte))
                .addGap(0, 9, Short.MAX_VALUE))
        );

        txt_Console.setBackground(new java.awt.Color(204, 204, 204));
        txt_Console.setColumns(20);
        txt_Console.setFont(new java.awt.Font("Monospaced", 0, 18)); // NOI18N
        txt_Console.setRows(5);
        jScrollPane2.setViewportView(txt_Console);

        btn_Analizar.setText("Analizar");
        btn_Analizar.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                btn_AnalizarActionPerformed(evt);
            }
        });

        javax.swing.GroupLayout jPanel2Layout = new javax.swing.GroupLayout(jPanel2);
        jPanel2.setLayout(jPanel2Layout);
        jPanel2Layout.setHorizontalGroup(
            jPanel2Layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(jPanel2Layout.createSequentialGroup()
                .addContainerGap()
                .addGroup(jPanel2Layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
                    .addComponent(jScrollPane2)
                    .addGroup(javax.swing.GroupLayout.Alignment.TRAILING, jPanel2Layout.createSequentialGroup()
                        .addGap(0, 0, Short.MAX_VALUE)
                        .addComponent(btn_Analizar)))
                .addContainerGap())
        );
        jPanel2Layout.setVerticalGroup(
            jPanel2Layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(jPanel2Layout.createSequentialGroup()
                .addContainerGap()
                .addComponent(jScrollPane2, javax.swing.GroupLayout.PREFERRED_SIZE, 302, javax.swing.GroupLayout.PREFERRED_SIZE)
                .addGap(18, 18, 18)
                .addComponent(btn_Analizar)
                .addContainerGap(18, Short.MAX_VALUE))
        );

        javax.swing.GroupLayout layout = new javax.swing.GroupLayout(getContentPane());
        getContentPane().setLayout(layout);
        layout.setHorizontalGroup(
            layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(layout.createSequentialGroup()
                .addContainerGap()
                .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
                    .addComponent(jPanel2, javax.swing.GroupLayout.Alignment.TRAILING, javax.swing.GroupLayout.DEFAULT_SIZE, javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                    .addComponent(jPanel1, javax.swing.GroupLayout.DEFAULT_SIZE, javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                .addContainerGap())
        );
        layout.setVerticalGroup(
            layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(layout.createSequentialGroup()
                .addGap(18, 18, 18)
                .addComponent(jPanel1, javax.swing.GroupLayout.PREFERRED_SIZE, javax.swing.GroupLayout.DEFAULT_SIZE, javax.swing.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.RELATED)
                .addComponent(jPanel2, javax.swing.GroupLayout.DEFAULT_SIZE, javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                .addContainerGap())
        );

        pack();
    }// </editor-fold>//GEN-END:initComponents

    /*------------------ BOTON ABRIR ---------------------- */
    private void btn_AbrirActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_btn_AbrirActionPerformed
        JFileChooser nuevo = new JFileChooser();
        FileNameExtensionFilter filtro = new FileNameExtensionFilter("Archivo de Entrada","gu");
        nuevo.setFileFilter(filtro);
        nuevo.setFileSelectionMode(JFileChooser.FILES_ONLY);
        int result = nuevo.showOpenDialog(this);
        if(result == JFileChooser.APPROVE_OPTION) {
            archivo = nuevo.getSelectedFile();
            txt_Console.setText("");
            try {
                Scanner scn = new Scanner(archivo);
                while(scn.hasNext()){
                    txt_Console.insert(scn.nextLine() + "\n", txt_Console.getText().length());
                }
            } catch (FileNotFoundException ex) {
                Logger.getLogger(Principal.class.getName()).log(Level.SEVERE, null, ex);
            }
        }        
    }//GEN-LAST:event_btn_AbrirActionPerformed

    /*------------------ BOTON GUARDAR ---------------------- */
    private void btn_GuardarActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_btn_GuardarActionPerformed
        if(archivo!=null){
            BufferedWriter bw = null;
            try {
                bw = new BufferedWriter(new FileWriter(archivo));
                bw.write(txt_Console.getText());
                JOptionPane.showMessageDialog(null, "Archivo guardado","Congrats!",JOptionPane.INFORMATION_MESSAGE);
                bw.close();
            } catch (IOException ex) {
                Logger.getLogger(Principal.class.getName()).log(Level.SEVERE, null, ex);       
            }
            finally{
                if(bw!=null){
                    try{
                        bw.close();
                    }
                    catch(IOException ex){
                        Logger.getLogger(Principal.class.getName()).log(Level.SEVERE, null, ex);       
                    }
                }
            }
        }
        else{
            try{
                JFileChooser nuevo =new JFileChooser();
                FileNameExtensionFilter filtro = new FileNameExtensionFilter("Archivo de Entrada","gu");
                nuevo.setFileFilter(filtro);
                nuevo.showSaveDialog(this);
                archivo = nuevo.getSelectedFile();
                if(archivo!=null){
                    FileWriter save = new FileWriter(archivo+".gu");
                    save.write(txt_Console.getText());
                    save.close();
                    JOptionPane.showMessageDialog(null,"El archivo se a guardado Exitosamente","Congrats!",JOptionPane.INFORMATION_MESSAGE);
                }
            }
            catch(IOException ex){
                JOptionPane.showMessageDialog(null,"Su archivo no se ha guardado","Advertencia",JOptionPane.ERROR_MESSAGE);
            }
        }
    }//GEN-LAST:event_btn_GuardarActionPerformed
    
    /*------------------ BOTON GUARDAR COMO ---------------------- */
    private void btn_GuardarComoActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_btn_GuardarComoActionPerformed
        try{
                JFileChooser nuevo =new JFileChooser();
                FileNameExtensionFilter filtro = new FileNameExtensionFilter("Archivo de Entrada","gu");
                nuevo.setFileFilter(filtro);
                nuevo.showSaveDialog(this);
                archivo = nuevo.getSelectedFile();
                if(archivo!=null){
                    FileWriter save = new FileWriter(archivo+".gu");
                    save.write(txt_Console.getText());
                    save.close();
                    JOptionPane.showMessageDialog(null,"El archivo se a guardado Exitosamente","Congrats!",JOptionPane.INFORMATION_MESSAGE);
                }
            }
            catch(IOException ex){
                JOptionPane.showMessageDialog(null,"Su archivo no se ha guardado","Advertencia",JOptionPane.ERROR_MESSAGE);
            }
    }//GEN-LAST:event_btn_GuardarComoActionPerformed

    /*------------------ BOTON REPORTE ---------------------- */
    private void btn_ReporteActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_btn_ReporteActionPerformed
        System.out.println("REPORTE");
    }//GEN-LAST:event_btn_ReporteActionPerformed

    /*------------------ BOTON ABRIR ---------------------- */    
    private void btn_AnalizarActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_btn_AnalizarActionPerformed
        try {
            analizar();
        } catch (Exception ex) {
            System.out.println(ex.getMessage());
        }
    }//GEN-LAST:event_btn_AnalizarActionPerformed
    private analizador.Lexer lexico;
    private analizador.parser analizador;
    public  void analizar(){
        /*String input = txt_Console.getText();
        lexico = new analizador.Lexer(new BufferedReader(new StringReader(input)));
        parser = new analizador.parser(lexico);
        try {
            parser.parse();
            imprimirGlobales();
        } catch (Exception ex) {
            System.out.println("Error fatal en la compilación");
        }*/
        interpretar("entrada.gu");
    }
    public void imprimirGlobales(){
        Iterator<Variable> it  = analizador.global.iterator();
        while(it.hasNext()){
            Variable n = it.next();
            System.out.println("Nombre: " + n.getNombreVariable() + " Tipo: " + n.getTipoVariable());
        }
    }
    
    public void imprimirGrafica(){
        Iterator<Grafica> it  = analizador.grafica.iterator();
        while(it.hasNext()){
            Grafica n = it.next();
            if(n instanceof GraficaBarras){
                GraficaBarras gb = (GraficaBarras)n;
                gb.validar_caracteristicas();
                gb.mostrar_caracteristicas();
            }
            else if(n instanceof GraficaLineas){
                GraficaLineas gl = (GraficaLineas)n;
            }
        }
    }
    
    private void interpretar(String path) {
        try {
            analizador = new analizador.parser(new analizador.Lexer(new FileInputStream(path)));
            analizador.parse();
            imprimirGlobales();
            imprimirGrafica();
        } 
        catch (Exception ex) {
            System.out.println("Error fatal en la compilación");
        }
    }
    
    public static void main(String args[]) {
        java.awt.EventQueue.invokeLater(new Runnable() {
            public void run() {
                new Principal().setVisible(true);
            }
        });
    }

    // Variables declaration - do not modify//GEN-BEGIN:variables
    private javax.swing.JButton btn_Abrir;
    private javax.swing.JButton btn_Analizar;
    private javax.swing.JButton btn_Guardar;
    private javax.swing.JButton btn_GuardarComo;
    private javax.swing.JButton btn_Reporte;
    private javax.swing.JLabel jLabel1;
    private javax.swing.JPanel jPanel1;
    private javax.swing.JPanel jPanel2;
    private javax.swing.JScrollPane jScrollPane2;
    private javax.swing.JTextArea txt_Console;
    // End of variables declaration//GEN-END:variables
}
