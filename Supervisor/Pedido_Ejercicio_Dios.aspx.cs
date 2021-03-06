﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logica;
using Datos;
using System.Web.UI.WebControls.WebParts;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;

namespace Supervisor
{
    public partial class Pedido_Ejercicio_Dios : System.Web.UI.Page
    {
        Logica_Bloque_Pedido_Ejercicio_Dios LBPED = new Logica_Bloque_Pedido_Ejercicio_Dios();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Administrador"] == null || Session["www"] == null)
            {
                Response.Redirect("sefue.aspx");
            }
            Etiqueta_Administrador_Chico.Text = ((string)Session["Administrador"]).ToUpper();
            Etiqueta_Administrador_Grande.Text = ((string)Session["Administrador"]).ToUpper();
            Etiqueta_Hora_Grande.Text = DateTime.Now.ToString();
            Etiqueta_Hora_Chica.Text = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();
            Etiqueta_Localizador_Grande.Text = Request.UserHostAddress.ToString();
            Etiqueta_Localizador_Chico.Text = Request.UserHostAddress.ToString();

            if (!Page.IsPostBack) // se carga la primera vez al abrir la pagina
            {
                Condiciones_Paginacion_Dios();
                Mostrar_Datos_Dios(0);
            }
        }

        protected void Volver_A_Consola_Click(object sender, EventArgs e)
        {
            Response.Redirect((string)Session["URL"]);
        }

        #region Paginacion_Dios
        public void Condiciones_Paginacion_Dios() // pregunta en que momento tomo las condiciones del paginado si cuando arranca la pagina o cuando presiono el boton de buscar
        {

            ViewState["Pagina"] = 0; // posiciono por las dudas la pagina en cero siempre
            Interno_Dios.Visible = false; // contiene siguiente y anterior de las paginaciones centrales
            Siguiente_Dios_Primero.Visible = true; // siguiente de la primera pagina
            Anterior_Dios_Ultimo.Visible = false; // anterior de la ultima pagina
            ViewState["Cantidad_De_Datos"] = LBPED.Logica_Contar_Cantidad_Ejercicios(); // cuenta la cantidad de datos
            ViewState["Cantidad_De_Paginas"] = (int)ViewState["Cantidad_De_Datos"] / 20; // cantidad de paginas segun la cantidad de datos            
            ViewState["Resto"] = (int)ViewState["Cantidad_De_Datos"] % 20; // me dice cuantos datos faltan para completar una pagina completa
            if ((int)ViewState["Resto"] == 0) // sin resto hay una cantidad de paginas completas y le debo restar uno para asegurarme que como inicio de cero la ultima pagina no se encuentre vacia
            {
                ViewState["Cantidad_De_Paginas"] = (int)ViewState["Cantidad_De_Paginas"] - 1;
            }
            if ((int)ViewState["Cantidad_De_Datos"] <= 20) // para saber si hay menos de 20 datos no aparezca el boton de siguiente
            {
                Siguiente_Dios_Primero.Visible = false;
            }
        }

        protected void Siguiente_Dios_Click(object sender, EventArgs e)
        {
            Formulario_Dios.Visible = false;
            ViewState["Pagina"] = (int)ViewState["Pagina"] + 1;// sumo una pagina
            if ((int)ViewState["Pagina"] == (int)ViewState["Cantidad_De_Paginas"]) // mira si la pagina en la que estoy es igual a la pagina final (estoy en la ultima pagina)
            {
                Mostrar_Datos_Dios((int)ViewState["Pagina"]); // carga cada presion el gridview
                Interno_Dios.Visible = false; // como estoy en la ultima pagina solo debe mostrar el anterior ultimo 
                Extremo_Dios.Visible = true; // como estoy en la ultima pagina solo debe mostrar el anterior ultimo (contiene anterior ultimo y siguiente primero)
                Siguiente_Dios_Primero.Visible = false; // como estoy en la ultima pagina solo debe mostrar el anterior ultimo 
                Anterior_Dios_Ultimo.Visible = true; // como estoy en la ultima pagina solo debe mostrar el anterior ultimo 
            }
            else // sin no estoy en la ultima pagina
            {
                Mostrar_Datos_Dios((int)ViewState["Pagina"]); // carga cada presion el gridview
                Interno_Dios.Visible = true; // estoy en las paginas del centro
                Extremo_Dios.Visible = false; // no muestra ni siguiente ni anterior de las paginas ultima e inicial
            }
        }

        protected void Anterior_Dios_Click(object sender, EventArgs e)
        {
            Formulario_Dios.Visible = false;
            ViewState["Pagina"] = (int)ViewState["Pagina"] - 1; // resto una pagina
            if ((int)ViewState["Pagina"] == 0) // mira si esta en la primera pagina
            {
                Mostrar_Datos_Dios((int)ViewState["Pagina"]); // carga cada presion el gridview
                Interno_Dios.Visible = false;// como estoy en la primera pagina solo debe mostrar el siguiente primero 
                Extremo_Dios.Visible = true;// como estoy en la primera pagina solo debe mostrar el siguiente primero (contiene anterior ultimo y siguiente primero)
                Siguiente_Dios_Primero.Visible = true;// como estoy en la primera pagina solo debe mostrar el siguiente primero
                Anterior_Dios_Ultimo.Visible = false;// como estoy en la primera pagina solo debe mostrar el siguiente primero
            }
            else// sin no estoy en la primera pagina
            {
                Mostrar_Datos_Dios((int)ViewState["Pagina"]); // carga cada presion el gridview
                Interno_Dios.Visible = true; // estoy en las paginas del centro
                Extremo_Dios.Visible = false;// no muestra ni siguiente ni anterior de las paginas ultima e inicial
            }
        }

        #endregion

        #region GridView_Dios
        public void Mostrar_Datos_Dios(int Pagina)
        {
            GridView_Dios.DataSource = LBPED.Logica_Mostrar_Ejercicio().Skip(Pagina * 20).Take(20);
            GridView_Dios.DataBind();
        }
        #endregion


        #region Seleccionar_El_Dios_Dios

        protected void Identificador_Dios(object sender, EventArgs e) // convierto al datalist en editable 
        {
            Formulario_Dios.Visible = true;
            GridViewRow row = GridView_Dios.SelectedRow; // defiene la variable row del tipo gridview
            Session["ID_Pedido"] = GridView_Dios.DataKeys[row.RowIndex].Value; // captura el identificador del Dios seleccionado
            List<Seleccionar_Ejercicio_Pedido_DiosResult> Datos = LBPED.Logica_Seleccionar_Ejercicicio((int)Session["ID_Pedido"]); // carga los datos del Dios elegido por el Dios
            CheckBox_Ejercicio_Dios.Checked = (bool)Datos[0].Ejercicio_Pedido;
            CheckBox_Explicacion_Dios.Checked = (bool)Datos[0].Explicacion_Pedida;
            Session["Adjunto"] = Datos[0].Archivo_Adjunto;
            Session["Ficha"] = Datos[0].Archivo_Ficha;
            Session["Enunciado"] = Datos[0].Archivo_Enunciado;

            Etiqueta_Usuario_Dios.Text = Datos[0].Usuario;
            Etiqueta_Fecha_Dios.Text = Datos[0].Fecha_De_Pedido.ToString();
            if (Datos[0].Administrador == null)
            {
                Etiqueta_Administrador_Dios.Text = string.Empty;
            }
            else
            { 
                Etiqueta_Administrador_Dios.Text = Datos[0].Administrador; // carga el Dios de la base          
            }
           
            Etiqueta_Empresa_Dios.Text = Datos[0].Empresa;


            CheckBox_Realizado_Dios.Enabled = false;
            CheckBox_Realizado_Dios.Checked = (bool)Datos[0].Realizado; // carga de la base si el Dios esta bloqueado

           

        }

        #endregion
        #region Botonera_De_Dios
       

        protected void Boton_Borrar_Dios_Click(object sender, EventArgs e)
        {
            int Valor = LBPED.Logica_Borrar_Ejercicico((int)Session["ID_Pedido"]); // Borrar el administrador
            if (Valor == 1) // todo ok
            {
                string alerta = @"alert('Usted borro corectamente la consulta'); 

                window.location.reload();";

                ScriptManager.RegisterStartupScript(this, typeof(Page), "", alerta, true);
                return;
            }
        }


       
        protected void Boton_Archivo_Adjunto_Dios_Click(object sender, EventArgs e)
        {
            if (Session["Adjunto"] == null)
            {
                ScriptManager.RegisterClientScriptBlock(Page, typeof(string), "", "alert('No hay archivo adjunto');", true);
                return;

            }
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + (string)Session["Adjunto"]);
            Response.TransmitFile("C:\\archivo/" + (string)Session["Adjunto"]);
            Response.End();
        }

        protected void Boton_Archivo_Ficha_Dios_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + (string)Session["Ficha"] + "_ficha.txt");
            Response.TransmitFile("C:\\archivo/" + (string)Session["Ficha"] + "_ficha.txt");          
            Response.End();
        }

        protected void Boton_Archivo_Enunciado_Dios_Click(object sender, EventArgs e)
        {
            if (Session["Enunciado"] == null)
            {
                ScriptManager.RegisterClientScriptBlock(Page, typeof(string), "", "alert('No hay enunciado');", true);
                return;

            }
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + (string)Session["Enunciado"] + "_math.txt");    
            Response.TransmitFile("C:\\archivo/" + (string)Session["Enunciado"] + "_math.txt");
            Response.End();
            
        }

        protected void Boton_Archivo_Enunciado_Dios_CLEAN_Click(object sender, EventArgs e)
        {
            if (Session["Enunciado"] == null)
            {
                ScriptManager.RegisterClientScriptBlock(Page, typeof(string), "", "alert('No hay enunciado');", true);
                return;

            }
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + (string)Session["Enunciado"] + "_clean.txt");
            Response.TransmitFile("C:\\archivo/" + (string)Session["Enunciado"] + "_clean.txt");
            Response.End();

        }
        #endregion
    }
}