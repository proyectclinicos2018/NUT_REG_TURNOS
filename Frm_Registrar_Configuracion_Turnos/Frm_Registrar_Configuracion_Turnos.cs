using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Globalization;

namespace Frm_Registrar_Configuracion_Turnos
{
    public partial class Frm_Registrar_Configuracion_Turnos : Form
    {

        // variables 

        #region Variables

        #region Variables Staticas

        static int cod_turno = 0;
        static int cod_habitacion = 0;
        static int cod_cama = 0;

        #endregion

        #region Datatables

        DataTable dt_turnos = new DataTable();

        #endregion

        #region Datos Conexion

        ConectarFalp CnnFalp;
        Configuration Config;
        string User = string.Empty;
        string[] Conexion = { "", "", "" };
        string PCK = "PCK_NUT001I";
        string PCK1 = "PCK_NUT001M";

        #endregion

        #endregion

        public Frm_Registrar_Configuracion_Turnos()
        {
            InitializeComponent();
        }

        private void Frm_Registrar_Configuracion_Turnos_Load(object sender, EventArgs e)
        {
            conectar();
            Cargar_grilla_turnos();
        }


    
        
        #region Cargar


        #region Cargar Conexion

        void conectar()
        {

            if (!(CnnFalp != null))
            {

                ExeConfigurationFileMap FileMap = new ExeConfigurationFileMap();
                FileMap.ExeConfigFilename = Application.StartupPath + @"\..\WF.config";
                Config = ConfigurationManager.OpenMappedExeConfiguration(FileMap, ConfigurationUserLevel.None);

                CnnFalp = new ConectarFalp(Config.AppSettings.Settings["dbServer"].Value,//ConfigurationManager.AppSettings["dbServer"],
                                           Config.AppSettings.Settings["dbUser"].Value,//ConfigurationManager.AppSettings["dbUser"],
                                           Config.AppSettings.Settings["dbPass"].Value,//ConfigurationManager.AppSettings["dbPass"],
                                           ConectarFalp.TipoBase.Oracle);

                if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir(); // abre la conexion

                Conexion[0] = Config.AppSettings.Settings["dbServer"].Value;
                Conexion[1] = Config.AppSettings.Settings["dbUser"].Value;
                Conexion[2] = Config.AppSettings.Settings["dbPass"].Value;
            }



            // this.Text = this.Text + " [Versión: " + Application.ProductVersion + "] [Conectado: " + Conexion[0] + "]";
            //User = ValidaMenu.LeeUsuarioMenu();
            User = "SICI";
            LblUsuario.Text = "Usuario: " + User;
            //LblUsuario.Text = "Usuario: " + User;
        }
        #endregion

      

        #region Cargar Grilla

        #region Listar Grilla

        protected void Cargar_grilla_turnos()
        {

           if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();
            dt_turnos.Clear();
            CnnFalp.CrearCommand(CommandType.StoredProcedure, PCK + ".P_LISTADOS_TURNOS");

            dt_turnos.Load(CnnFalp.ExecuteReader());

            if (dt_turnos.Rows.Count > 0)
            {
                grilla_turnos.AutoGenerateColumns = false;
                grilla_turnos.DataSource = dt_turnos;
                ocultar_grilla_turnos();
                agrupar_celdas_menu();
                agregarimagen();
            
            }
            else
            {
                //no trajo datos
                // txtmsg.Text = "";
            }

            CnnFalp.Cerrar();
            //ocultar_grilla_menu();
        }

        #endregion

        #region Agrupar

        protected void agrupar_celdas_menu()
        {
            int var_cod = 0;

            foreach (DataGridViewRow miRow in grilla_turnos.Rows)
            {
                if (Convert.ToInt32(miRow.Cells["COD_TURNOS"].Value) != var_cod)
                {

                }
                else
                {

                    miRow.Cells["NOM_TURNOS"].Value = "";

                }
                var_cod = Convert.ToInt32(miRow.Cells["COD_TURNOS"].Value.ToString());
            }

        }


        #endregion

        #region Agregar Imagen

        protected void agregarimagen()
        {
            foreach (DataGridViewRow row in grilla_turnos.Rows)
            {

                string ve = Convert.ToString(row.Cells["VIGENCIA"].Value);
                DataGridViewImageCell Imagen = row.Cells["V"] as DataGridViewImageCell;

                if (ve == "S")
                {
                    Imagen.Value = (System.Drawing.Image)Registrar_Configuracion_Turnos.Properties.Resources.Check;
                }
                else
                {
                    Imagen.Value = (System.Drawing.Image)Registrar_Configuracion_Turnos.Properties.Resources.Delete;

                }

            }



        }

        #endregion

        #region Ocultar Columnas


        void ocultar_grilla_turnos()
        {
            grilla_turnos.AutoResizeColumns();
            grilla_turnos.Columns["COD_HABITACIONS"].Visible = false;
            grilla_turnos.Columns["COD_CAMAS"].Visible = false;
            grilla_turnos.Columns["COD_TURNOS"].Visible = false;
            grilla_turnos.Columns["VIGENCIA"].Visible = false;
            grilla_turnos.Columns["ID_TURNOS"].Visible = false;
 
            //grilla_menu.Columns["ELIMINAR"].Visible = false;
        }

        #endregion

        #region Ordenar Columnas

        #endregion

        #region Pintar Grilla

        private void grilla_turnos_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

            if (e.RowIndex < 0)
            {
                e.PaintBackground(e.ClipBounds, false);
                Font drawFont = new Font("Trebuchet MS", 8, FontStyle.Bold);
                SolidBrush drawBrush = new SolidBrush(Color.White);
                StringFormat StrFormat = new StringFormat();
                StrFormat.Alignment = StringAlignment.Center;
                StrFormat.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawImage(Registrar_Configuracion_Turnos.Properties.Resources.HeaderGV, e.CellBounds);
                e.Graphics.DrawString(grilla_turnos.Columns[e.ColumnIndex].HeaderText, drawFont, drawBrush, e.CellBounds, StrFormat);

                e.Handled = true;
                drawBrush.Dispose();
            }
        }

        #endregion

        #region Pintar Extraer grilla

        private void grilla_turnos_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {


            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 0)
                {
                    DialogResult opc = MessageBox.Show("Estimado usuario, Esta seguro de Eliminar la Cama", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (opc == DialogResult.Yes)
                    {
                        int id_turno = Convert.ToInt32(grilla_turnos.Rows[e.RowIndex].Cells["ID_TURNOS"].Value.ToString());



                        if (id_turno == 0)
                        {

                            dt_turnos.Rows.RemoveAt(e.RowIndex);
                            dt_turnos.AcceptChanges();
                        }
                        else
                        {

                            MessageBox.Show("Estimado Usuario, El N° Cama no se puede Eliminar desde Aqui, ir Al Módulo de Modificar ", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }


                    }
                    else
                    {
                        MessageBox.Show("Estimado usuario, El N° Cama no se puede Eliminar", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }


            }
            ocultar_grilla_turnos();
            agrupar_celdas_menu();
            agregarimagen();

        }

        #endregion

        #endregion

       #region Cargar DataTables

 	   #endregion


        #endregion

        #region Botones

       private void btn_tipo_turno_Click(object sender, EventArgs e)
        {
            txttipo_turno.Text = "";
            Cargar_tipo_turno();

            if (Convert.ToInt32(cod_turno) == 0)
            {
                txttipo_turno.Enabled = false;
                txttipo_turno.Focus();
               

            }
            else
            {
                txtnum_habitacion.Enabled = true;
                btn_num_habitacion.Enabled = true;
                txtnum_habitacion.Focus();
            
            }
        }

        private void btn_num_habitacion_Click(object sender, EventArgs e)
        {
            txtnum_habitacion.Text = "";
            Cargar_tipo_habitacion();

            if (Convert.ToInt32(cod_habitacion) == 0)
            {
                txtnum_habitacion.Enabled = false;
                txtnum_habitacion.Focus();


            }
            else
            {
                txtnum_cama.Enabled = true;
                btn_num_cama.Enabled = true;
                txtnum_cama.Focus();
           
            }
        }

        private void btn_num_cama_Click(object sender, EventArgs e)
        {
            txtnum_cama.Text = "";
            Cargar_tipo_camas();

            if (Convert.ToInt32(cod_cama) == 0)
            {
                txtnum_cama.Enabled = false;
                txtnum_cama.Focus();


            }
            else
            {
                
                btn_agregar.Enabled = true;
                btn_agregar.Focus();
           
            }
        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
           
            if (Validar_Campos_2())
            {
             
                Agregar_turnos();
                btn_guardar.Enabled = true;
                Limpiar();

            }
        }

        private void btn_guardar_Click(object sender, EventArgs e)
        {
            if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();
          
                CnnFalp.IniciarTransaccion();
                Guardar_turnos();
                CnnFalp.ConfirmarTransaccion();
                CnnFalp.Cerrar();


                MessageBox.Show("Estimado Usuario, La Información fue Guardada Correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                


          
        }

        #endregion

        #region Metodos


        protected void Guardar_turnos()
        {
          
                foreach (DataRow fila in dt_turnos.Rows)
                {
                    int c_id = Convert.ToInt32(fila["ID_TURNOS"].ToString());
                    int c_tur = Convert.ToInt32(fila["COD_TURNOS"].ToString());
                    int c_hab = Convert.ToInt32(fila["COD_HABITACIONS"].ToString());
                    int c_cama = Convert.ToInt32(fila["COD_CAMAS"].ToString());
                    string vigencia = fila["VIGENCIA"].ToString();

                    if (c_id == 0)
                    {

                        if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();

                        CnnFalp.CrearCommand(CommandType.StoredProcedure, "PCK_NUT001I.P_REGISTRAR_CONFIG_TURNOS");

                        CnnFalp.ParametroBD("PIN_COD_TURNOS", c_tur, DbType.Int64, ParameterDirection.Input);
                        CnnFalp.ParametroBD("PIN_COD_HABITACION", c_hab, DbType.Int64, ParameterDirection.Input);
                        CnnFalp.ParametroBD("PIN_COD_CAMA", c_cama, DbType.String, ParameterDirection.Input);
                        CnnFalp.ParametroBD("PIN_USUARIO", User.ToUpper().Trim(), DbType.String, ParameterDirection.Input);
                        CnnFalp.ParametroBD("PIN_VIGENCIA", vigencia.ToUpper().Trim(), DbType.String, ParameterDirection.Input);

                        int registro = CnnFalp.ExecuteNonQuery();
                    }




                }
        

        }

        #region Cargar Tipo menu

        protected void Cargar_tipo_turno()
        {
            Cargar_datos_tipo_turno(ref Ayuda);

            if (!Ayuda.EOF())
            {
                cod_turno = Convert.ToInt32(Ayuda.Fields(0));
                txttipo_turno.Text = Ayuda.Fields(1);
            }
            else
            {
                if (Convert.ToInt32(cod_turno) == 0)
                {
                    txttipo_turno.Text = "";
                }
            }


        }

        void Cargar_datos_tipo_turno(ref AyudaSpreadNet.AyudaSprNet Ayuda)
        {
            string[] NomCol = { "Código", "Descripción" };
            int[] AnchoCol = { 80, 350 };
            Ayuda.Nombre_BD_Datos = Conexion[0];
            Ayuda.Pass = Conexion[1];
            Ayuda.User = Conexion[2];
            Ayuda.TipoBase = 1;
            Ayuda.NombreColumnas = NomCol;
            Ayuda.AnchoColumnas = AnchoCol;
            Ayuda.TituloConsulta = "Ingresar Tipo de Turno";
            Ayuda.Package = PCK;
            Ayuda.Procedimiento = "P_CARGAR_TIPO_TURNOS";
            Ayuda.Generar_ParametroBD("PIN_DESCRIPCION", txttipo_turno.Text.ToUpper(), DbType.String, ParameterDirection.Input);
            Ayuda.EjecutarSql();

        }

        #endregion


        #region Cargar Tipo Habitacion

        protected void Cargar_tipo_habitacion()
        {
            Cargar_datos_tipo_habitacion(ref Ayuda);

            if (!Ayuda.EOF())
            {
                cod_habitacion = Convert.ToInt32(Ayuda.Fields(0));
                txtnum_habitacion.Text = Ayuda.Fields(1);
            }
            else
            {
                if (Convert.ToInt32(cod_habitacion) == 0)
                {
                    txtnum_habitacion.Text = "";
                }
            }


        }

        void Cargar_datos_tipo_habitacion(ref AyudaSpreadNet.AyudaSprNet Ayuda)
        {
            string[] NomCol = { "Código", "Descripción" };
            int[] AnchoCol = { 80, 350 };
            Ayuda.Nombre_BD_Datos = Conexion[0];
            Ayuda.Pass = Conexion[1];
            Ayuda.User = Conexion[2];
            Ayuda.TipoBase = 1;
            Ayuda.NombreColumnas = NomCol;
            Ayuda.AnchoColumnas = AnchoCol;
            Ayuda.TituloConsulta = "Ingresar Tipo de Habitacion";
            Ayuda.Package = PCK;
            Ayuda.Procedimiento = "P_CARGAR_TIPO_HABITACION";
            Ayuda.Generar_ParametroBD("PIN_DESCRIPCION", txtnum_habitacion.Text.ToUpper(), DbType.String, ParameterDirection.Input);
            Ayuda.EjecutarSql();

        }

        #endregion


        #region Cargar Tipo Camas

        protected void Cargar_tipo_camas()
        {
            Cargar_datos_tipo_camas(ref Ayuda);

            if (!Ayuda.EOF())
            {
                cod_cama = Convert.ToInt32(Ayuda.Fields(0));
                txtnum_cama.Text = Ayuda.Fields(1);
            }
            else
            {
                if (Convert.ToInt32(cod_habitacion) == 0)
                {
                    txtnum_habitacion.Text = "";
                }
            }


        }

        void Cargar_datos_tipo_camas(ref AyudaSpreadNet.AyudaSprNet Ayuda)
        {
            string[] NomCol = { "Código", "Descripción" };
            int[] AnchoCol = { 80, 350 };
            Ayuda.Nombre_BD_Datos = Conexion[0];
            Ayuda.Pass = Conexion[1];
            Ayuda.User = Conexion[2];
            Ayuda.TipoBase = 1;
            Ayuda.NombreColumnas = NomCol;
            Ayuda.AnchoColumnas = AnchoCol;
            Ayuda.TituloConsulta = "Ingresar Tipo de Camas";
            Ayuda.Package = PCK;
            Ayuda.Procedimiento = "P_CARGAR_TIPO_CAMAS";
            Ayuda.Generar_ParametroBD("PIN_DESCRIPCION", txtnum_cama.Text.ToUpper(), DbType.String, ParameterDirection.Input);
            Ayuda.Generar_ParametroBD("PIN_COD_HAB", cod_habitacion, DbType.Int32, ParameterDirection.Input);
            Ayuda.EjecutarSql();

        }

        #endregion

     

        private void Limpiar()
        {
            cod_turno = 0;
            txttipo_turno.Text = "";
            cod_habitacion = 0;
            txtnum_habitacion.Text = "";
            cod_cama = 0;
            txtnum_cama.Text = "";

        }

        private void CambiarBlanco_TextLeave(object sender, EventArgs e)
        {
            TextBox GB = (TextBox)sender;
            GB.BackColor = Color.White;

        }

        private void CambiarColor_TextEnter(object sender, EventArgs e)
        {
            TextBox GB = (TextBox)sender;
            GB.BackColor = Color.FromArgb(255, 224, 192);
        }

        #endregion


        #region Validaciones

        protected Boolean Validar_Campos_2()
        {
            Boolean var = false;

            if (txttipo_turno.Text == "" && Convert.ToInt32(cod_turno) == 0)
            {
                MessageBox.Show("Estimado Usuario, El Campo Tipo TRurno se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txttipo_turno.Focus();
            }
            else
            {
                if (txtnum_habitacion.Text == "" && Convert.ToInt32(cod_habitacion) == 0)
                {
                    MessageBox.Show("Estimado usuario, El Campo Alimento se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtnum_habitacion.Focus();
                }
                else
                {
                    if (txtnum_cama.Text == "" && Convert.ToInt32(cod_cama) == 0)
                    {
                        MessageBox.Show("Estimado usuario, El Campo N° Cama se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtnum_cama.Focus();
                    }
                    else
                    {

                        int cont = 0;
                        foreach (DataRow miRow in dt_turnos.Select("COD_CAMAS = '" + cod_cama + "'"))
                        {
                            cont++;
                        }

                        if (cont == 0)
                        {

                            var = true;
                        }
                        else
                        {
                            MessageBox.Show("Estimado usuario, El N° Cama ya se encuentra ingresado", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            var = false;
                            txtnum_cama.Text = "";
                            cod_cama = 0;
                            txtnum_cama.Focus();
                        }

                    }
                }
            }

            return var;
        }

        protected void Agregar_turnos()
        {


            int cont = 0;


                foreach (DataRow fila2 in dt_turnos.Select(" COD_CAMAS= '" + cod_cama + "'"))
                {
                    cont++;
                }

                if (cont == 0)
                {
                    DataRow Fila = dt_turnos.NewRow();
                    Fila["COD_TURNOS"] = cod_turno;
                    string t = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txttipo_turno.Text);
                    Fila["NOM_TURNOS"] = t;
                    Fila["COD_HABITACIONS"] = cod_habitacion;
                    string h = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtnum_habitacion.Text);
                    Fila["NOM_HABITACIONS"] =h;
                    Fila["COD_CAMAS"] = cod_cama;
                    Fila["NUM_CAMAS"] = txtnum_cama.Text;
                    Fila["VIGENCIA"] = "S";
                    Fila["ID_TURNOS"] = 0;


                    dt_turnos.Rows.Add(Fila);
      
                    grilla_turnos.AutoGenerateColumns = false;
                    dt_turnos.DefaultView.Sort = "COD_TURNOS";
                    grilla_turnos.DataSource = dt_turnos;
                    agrupar_celdas_menu();
                    agregarimagen();
                    ocultar_grilla_turnos();
                }
            


            //ocultar_grilla_menu();

            MessageBox.Show("Estimado usuario, La Cama  fue Asginada correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }


        #endregion

        private void txttipo_turno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {

                e.Handled = true;

                return;
            }
            else
            {
                Cargar_tipo_turno();
                if (e.KeyChar == (char)13)
                {


                    if (Convert.ToInt32(cod_turno) == 0 || txttipo_turno.Text == "")
                    {
                        btn_tipo_turno.Enabled = false;
                        txttipo_turno.Focus();

                    }
                    else
                    {

                        btn_num_habitacion.Enabled = true;
                        txtnum_habitacion.Enabled = true;
                        txtnum_habitacion.Focus();
                    }

                }
            }
        }

        private void txtnum_habitacion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {

                e.Handled = true;

                return;
            }
            else
            {
                Cargar_tipo_habitacion();
                if (e.KeyChar == (char)13)
                {


                    if (Convert.ToInt32(cod_habitacion) == 0 || txtnum_habitacion.Text == "")
                    {
                        btn_num_habitacion.Enabled = false;
                        txtnum_habitacion.Focus();

                    }
                    else
                    {

                        btn_num_cama.Enabled = true;
                        txtnum_cama.Enabled = true;
                        txtnum_cama.Focus();
                    }

                }
            }
        }

        private void txtnum_cama_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {

                e.Handled = true;

                return;
            }
            else
            {
                Cargar_tipo_camas();
                if (e.KeyChar == (char)13)
                {


                    if (Convert.ToInt32(cod_cama) == 0 || txtnum_cama.Text == "")
                    {
                        btn_num_cama.Enabled = false;
                        txtnum_cama.Focus();

                    }
                    else
                    {

                        btn_agregar.Enabled = true;
                        btn_agregar.Focus();
                    }

                }
            }
        }

        private void btn_limpiar_agregar_Click(object sender, EventArgs e)
        {
            cod_turno=0;
            txttipo_turno.Text = "";
            cod_habitacion = 0;
            txtnum_habitacion.Text = "";
            cod_cama = 0;
            txtnum_cama.Text = "";
        }

       

    
  






    }
}
