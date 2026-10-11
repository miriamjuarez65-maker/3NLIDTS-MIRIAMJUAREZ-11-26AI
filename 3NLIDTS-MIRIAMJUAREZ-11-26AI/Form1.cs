using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3NLIDTS_MIRIAMJUAREZ_11_26AI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AsignarManejadoresEventos();
        }
        private void AsignarManejadoresEventos()
        {
            // Validamos al perder el foco (Leave) para no interrumpir mientras el usuario escribe
            tbNombre.Leave += ValidarCampoFormato;
            tbApellidos.Leave += ValidarCampoFormato;
            tbEdad.Leave += ValidarCampoFormato;
            tbEstatura.Leave += ValidarCampoFormato;
            tbTelefono.Leave += ValidarCampoFormato;
        }

        #region Métodos de Validación General

        private void ValidarCampoFormato(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Si está vacío, se deja en color neutro
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.BackColor = Color.White;
                    return;
                }

                bool esValido = false;

                if (textBox == tbNombre || textBox == tbApellidos)
                    esValido = EsTextoValido(textBox.Text);
                else if (textBox == tbEdad)
                    esValido = EsEnteroValido(textBox.Text);
                else if (textBox == tbEstatura)
                    esValido = EsDecimalValido(textBox.Text);
                else if (textBox == tbTelefono)
                    esValido = TelefonoValido(textBox.Text);

                // Feedback visual claro sin interrumpir con MessageBox
                textBox.BackColor = esValido ? Color.LightGreen : Color.LightPink;
            }
        }

        private bool EsTextoValido(string valor)
        {
            // Incluye letras con acentos, espacios y la Ñ/ñ
            return Regex.IsMatch(valor.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$");
        }

        private bool EsEnteroValido(string valor)
        {
            return int.TryParse(valor, out int edad) && edad > 0 && edad < 120;
        }

        private bool EsDecimalValido(string valor)
        {
            return decimal.TryParse(valor, out decimal estatura) && estatura > 0.3m && estatura < 2.8m;
        }

        private bool TelefonoValido(string valor)
        {
            return long.TryParse(valor, out _) && valor.Trim().Length == 10;
        }

        #endregion

        #region Eventos de Botones

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // 1. Validar que no haya campos vacíos o con formato erróneo antes de guardar
            if (!ValidarFormularioCompleto())
            {
                MessageBox.Show("Por favor, corrija los campos marcados en rojo antes de guardar.",
                    "Datos Incompletos o Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Obtener género seleccionado
            string genero = ObtenerGeneroSeleccionado();
            if (string.IsNullOrEmpty(genero))
            {
                MessageBox.Show("Seleccione un género para continuar.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Preparar mensaje
            string registro = $"Nombre: {tbNombre.Text.Trim()}\n" +
                             $"Apellidos: {tbApellidos.Text.Trim()}\n" +
                             $"Teléfono: {tbTelefono.Text.Trim()}\n" +
                             $"Edad: {tbEdad.Text.Trim()}\n" +
                             $"Estatura: {tbEstatura.Text.Trim()} m\n" +
                             $"Género: {genero}";

            // 4. Guardar en el archivo de texto
            try
            {
                string rutaArchivo = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "3NAgoDic26.txt");

                using (StreamWriter escritor = new StreamWriter(rutaArchivo, append: true))
                {
                    escritor.WriteLine("----------------------------------------");
                    escritor.WriteLine(registro);
                }

                MessageBox.Show("Registro guardado con éxito:\n\n" + registro,
                    "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnLimpiar_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al guardar el archivo: {ex.Message}",
                    "Error de Archivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarFormularioCompleto()
        {
            bool nombreOk = EsTextoValido(tbNombre.Text);
            bool apellidoOk = EsTextoValido(tbApellidos.Text);
            bool edadOk = EsEnteroValido(tbEdad.Text);
            bool estaturaOk = EsDecimalValido(tbEstatura.Text);
            bool telefonoOk = TelefonoValido(tbTelefono.Text);

            tbNombre.BackColor = nombreOk ? Color.LightGreen : Color.LightPink;
            tbApellidos.BackColor = apellidoOk ? Color.LightGreen : Color.LightPink;
            tbEdad.BackColor = edadOk ? Color.LightGreen : Color.LightPink;
            tbEstatura.BackColor = estaturaOk ? Color.LightGreen : Color.LightPink;
            tbTelefono.BackColor = telefonoOk ? Color.LightGreen : Color.LightPink;

            return nombreOk && apellidoOk && edadOk && estaturaOk && telefonoOk;
        }

        private string ObtenerGeneroSeleccionado()
        {
            if (rbFemenino.Checked) return "Femenino";
            if (rbMasculino.Checked) return "Masculino";
            if (rbOtro.Checked) return "Otro";
            return string.Empty;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            tbNombre.Clear();
            tbApellidos.Clear();
            tbEdad.Clear();
            tbEstatura.Clear();
            tbTelefono.Clear();

            // Restablecer colores de fondo
            tbNombre.BackColor = Color.White;
            tbApellidos.BackColor = Color.White;
            tbEdad.BackColor = Color.White;
            tbEstatura.BackColor = Color.White;
            tbTelefono.BackColor = Color.White;

            rbFemenino.Checked = false;
            rbMasculino.Checked = false;
            rbOtro.Checked = false;

            tbNombre.Focus();
        }
        #endregion
    }
}