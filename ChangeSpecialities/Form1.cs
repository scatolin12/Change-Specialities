using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAPFEWSELib;
using SapROTWr;
using Excel = Microsoft.Office.Interop.Excel;

namespace ChangeSpecialities
{
    public partial class Specialities : Form
    {
        public Specialities()
        {
            InitializeComponent();
        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            if (tb_id.Text == "" || tb_plant.Text == "")
            {
                return;
            }
            if (tb_id.TextLength > 4)
            {
                MessageBox.Show("ID item not valid", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (tb_plant.TextLength != 4)
            {
                MessageBox.Show("Centro inválido", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            //INICIAR ALTERAÇÃO

            #region
            if (dataGridView1.RowCount == 1)
            {
                return;
            }

            try
            {


                //Get the Windows Running Object Table
                SapROTWr.CSapROTWrapper sapROTWrapper = new SapROTWr.CSapROTWrapper();
                //Get the ROT Entry for the SAP Gui to connect to the COM
                object SapGuilRot = sapROTWrapper.GetROTEntry("SAPGUI");
                //Get the reference to the Scripting Engine
                object engine = SapGuilRot.GetType().InvokeMember("GetScriptingEngine", System.Reflection.BindingFlags.InvokeMethod, null, SapGuilRot, null);
                //Get the reference to the running SAP Application Window
                GuiApplication GuiApp = (GuiApplication)engine;
                //Get the reference to the first open connection
                GuiConnection connection = (GuiConnection)GuiApp.Connections.ElementAt(0);
                //get the first available session
                GuiSession session = (GuiSession)connection.Children.ElementAt(0);
                //Get the reference to the main "Frame" in which to send virtual key commandss
                //GuiFrameWindow frame = session.ActiveWindow;
                GuiFrameWindow frame = (GuiFrameWindow)session.FindById("wnd[0]");
                //((GuiFrameWindow)frame.FindById("wnd[0]"));
                ((GuiOkCodeField)frame.FindById("wnd[0]/tbar[0]/okcd")).Text = "/nztvc025";
                frame.SendVKey(0);


                int linhas1 = dataGridView1.RowCount;
                int i;


                ((GuiTextField)frame.FindById("wnd[0]/usr/ctxtP_PRFID")).Text = "0001";
                ((GuiTextField)frame.FindById("wnd[0]/usr/ctxtP_WERKS")).Text = tb_plant.Text;


                for (i = 0; i < linhas1 - 1; i++)
                {
                    try
                    {
                        ((GuiButton)frame.FindByName("btn[8]", "GuiButton")).Press();


                        string material = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        int tam = material.Length;
                        string component1 = dataGridView1.Rows[i].Cells[1].Value.ToString();

                        if (tam == 11)
                        {
                            ((GuiTextField)session.ActiveWindow.FindById("wnd[1]/usr/sub:SAPLSPO4:0300/ctxtSVALD-VALUE[1,21]")).Text = material;

                        }

                        else ((GuiTextField)session.ActiveWindow.FindById("wnd[1]/usr/sub:SAPLSPO4:0300/ctxtSVALD-VALUE[0,21]")).Text = material;

                        ((GuiButton)frame.FindByName("btn[0]", "GuiButton")).Press();

                        ((GuiToolbarControl)frame.FindById("wnd[0]/usr/subSUBSCR_TIPOS:SAPLZRVC_ZBOM_POS_ESP:0200/cntlCONT_TIPOS/shellcont/shell/shellcont[1]/shell[0]")).PressButton("&FIND");
                        ((GuiTextField)session.ActiveWindow.FindById("wnd[1]/usr/txtLVC_S_SEA-STRING")).Text = tb_id.Text.ToString();
                        ((GuiButton)frame.FindByName("btn[0]", "GuiButton")).Press();
                        ((GuiButton)frame.FindByName("btn[0]", "GuiButton")).Press();

                        if (session.Children.Count != 2)
                        {
                            dataGridView1.Rows[i].Cells[2].Value = "N/A";
                            goto acabou;
                        }

                        ((GuiButton)frame.FindByName("btn[0]", "GuiButton")).Press();

                        for (int j = 1; j < 100; j++)
                        {

                            string esp;
                            if (j < 10) { esp = "          "; }
                            else { esp = "         "; }

                            ((GuiTree)frame.FindById("wnd[0]/usr/subSUBSCR_TIPOS:SAPLZRVC_ZBOM_POS_ESP:0200/cntlCONT_TIPOS/shellcont/shell/shellcont[1]/shell[1]")).SelectItem(esp + j, "&Hierarchy");
                            ((GuiTree)frame.FindById("wnd[0]/usr/subSUBSCR_TIPOS:SAPLZRVC_ZBOM_POS_ESP:0200/cntlCONT_TIPOS/shellcont/shell/shellcont[1]/shell[1]")).EnsureVisibleHorizontalItem(esp + j, "&Hierarchy");
                            ((GuiTree)frame.FindById("wnd[0]/usr/subSUBSCR_TIPOS:SAPLZRVC_ZBOM_POS_ESP:0200/cntlCONT_TIPOS/shellcont/shell/shellcont[1]/shell[1]")).DoubleClickItem(esp + j, "&Hierarchy");


                            if (((GuiCTextField)session.ActiveWindow.FindByName("ZTBVC_657-ID_ITEM", "GuiCTextField")).Text == tb_id.Text.ToString())
                            {
                                ((GuiToolbarControl)frame.FindById("wnd[0]/usr/subSUBSCR_TIPOS:SAPLZRVC_ZBOM_POS_ESP:0200/cntlCONT_TIPOS/shellcont/shell/shellcont[1]/shell[0]")).PressButton("EDIT_ESP");
                                ((GuiTextField)frame.FindById("wnd[0]/usr/subSUBSCR_ESPEC:SAPLZRVC_ZBOM_POS_ESP:0300/subSUBSCR_ESP:SAPLZRVC_ZBOM_POS_ESP:0340/ctxtZTBVC_658-IDNRK")).Text = component1;
                                ((GuiButton)frame.FindByName("btn[8]", "GuiButton")).Press();
                                dataGridView1.Rows[i].Cells[2].Value = "OK";
                                break;
                            }

                        }



                    acabou: ((GuiButton)frame.FindByName("btn[2]", "GuiButton")).Press();
                    }
                    catch
                    {
                        ((GuiButton)frame.FindByName("btn[2]", "GuiButton")).Press();
                    }

                    #endregion
                }

                MessageBox.Show("Alteração Concluída", "Alteração Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            catch
            {
                MessageBox.Show("Erro ao conectar com SAP", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;

            }
        }



        private void dataGridView1_KeyDown_1(object sender, KeyEventArgs e)
        {

            if ((e.Shift && e.KeyCode == Keys.Insert) || (e.Control && e.KeyCode == Keys.V))

            {

                char[] rowSplitter = { '\r', '\n' };

                char[] columnSplitter = { '\t' };

                //get the text from clipboard

                IDataObject dataInClipboard = Clipboard.GetDataObject();

                string stringInClipboard = (string)dataInClipboard.GetData(DataFormats.Text);

                //split it into lines

                string[] rowsInClipboard = stringInClipboard.Split(rowSplitter, StringSplitOptions.RemoveEmptyEntries);

                //get the row and column of selected cell in grid

                int r = dataGridView1.SelectedCells[0].RowIndex;

                int c = dataGridView1.SelectedCells[0].ColumnIndex;

                //add rows into grid to fit clipboard lines

                if (dataGridView1.Rows.Count < (r + rowsInClipboard.Length))

                {

                    dataGridView1.Rows.Add(r + rowsInClipboard.Length - dataGridView1.Rows.Count + 1);

                }

                //loop through the lines, split them into cells and place the values in the corresponding cell.

                for (int iRow = 0; iRow < rowsInClipboard.Length; iRow++)

                {

                    //split row into cell values

                    string[] valuesInRow = rowsInClipboard[iRow].Split(columnSplitter);

                    //cycle through cell values

                    for (int iCol = 0; iCol < valuesInRow.Length; iCol++)

                    {

                        //assign cell value, only if it within columns of the grid

                        if (dataGridView1.ColumnCount - 1 >= c + iCol)

                        {

                            dataGridView1.Rows[r + iRow].Cells[c + iCol].Value = valuesInRow[iCol];

                        }

                    }

                }
                label5.Text = (dataGridView1.Rows.Count - 1).ToString() + " " + "linhas";

                if (dataGridView1.Rows.Count < 60)
                {
                    label6.Text = "Tempo Aprox. = " + ((dataGridView1.Rows.Count - 1) * 1).ToString() + " " + "segundos";
                }
                else
                {
                    label6.Text = "Tempo Aprox. = " + ((dataGridView1.Rows.Count - 1) * 0.016).ToString() + " " + "minutos";
                }

            }


        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (dataGridView1.RowCount == 1)
            {
                return;
            }

            SaveFileDialog salvar = new SaveFileDialog();
            // novo SaveFileDialog


            Excel.Application App; // Aplicação Excel
            Excel.Workbook WorkBook; // Pasta
            Excel.Worksheet WorkSheet; // Planilha
            object misValue = System.Reflection.Missing.Value;

            App = new Excel.Application();
            WorkBook = App.Workbooks.Add(misValue);
            WorkSheet = (Excel.Worksheet)WorkBook.Worksheets.get_Item(1);
            int i = 0;
            int j = 0;

            // passa as celulas do DataGridView para a Pasta do Excel
            for (i = 1; i < dataGridView1.ColumnCount + 1; i++)
            {
                WorkSheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
            }

            for (i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                for (j = 0; j <= dataGridView1.ColumnCount - 1; j++)
                {
                    DataGridViewCell cell = dataGridView1[j, i];
                    WorkSheet.Cells[i + 2, j + 1] = cell.Value;
                }
            }

            App.Visible = true;

        }

        private void btn_new_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            tb_id.Text = null;
            tb_plant.Text = null;

        }
    }

}
