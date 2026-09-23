using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace hwp2pdf
{
    public partial class FormConfig : Form
    {
        int m_option_overwite = 0;
        string m_option_source_ext_csv = ""; // CSV로 확장자를 저장
        HashSet<string> m_option_source_ext_set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        bool m_option_PDF_print = false;
        string m_strPrinter = "";
        int m_nPrintMethod = 1; 

        public FormConfig()
        {
            InitializeComponent();
        }
        public void setOption(int option_overwrite, string option_ext_csv, bool option_PDF_print, string strPrinter, int nPrintMethod)
        {
            m_option_overwite = option_overwrite;
            m_option_source_ext_csv = option_ext_csv ?? "";
            m_option_source_ext_set.Clear();
            foreach (var ex in m_option_source_ext_csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                m_option_source_ext_set.Add(ex.Trim().ToLower());
            m_option_PDF_print = option_PDF_print;
            m_strPrinter = strPrinter;
            m_nPrintMethod = nPrintMethod;
        }
        public int get_option_overwrite()
        {
            return m_option_overwite;
        }
        public string get_option_extcsv()
        {
            return m_option_source_ext_csv;
        }
        public bool get_option_PDF_print()
        {
            return m_option_PDF_print;
        }
        public string get_option_printer()
        {
            return m_strPrinter;
        }
        public int get_option_printmethod()
        {
            return m_nPrintMethod;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (radioButton_newname.Checked == true) m_option_overwite = 0;
            else if (radioButton_skip.Checked == true) m_option_overwite = 1;
            else if (radioButton_overwrite.Checked == true) m_option_overwite = 2;

            // build CSV from checked items
            List<string> sel = new List<string>();
            foreach (ListViewItem ext_item in list_ext_option.Items)
            {
                if (ext_item.Checked == true) sel.Add(ext_item.Text.ToLower());
            }
            m_option_source_ext_csv = string.Join(",", sel);
            m_option_source_ext_set.Clear();
            foreach (var ex in sel) m_option_source_ext_set.Add(ex);
           
            m_option_PDF_print = radio_PDF_Print.Checked;
            m_strPrinter = comboBox_PDF_Printer.SelectedItem == null
                ? ""
                : comboBox_PDF_Printer.SelectedItem.ToString();
            int nTemp = comboBox_PDF_PrintMethod.SelectedIndex;
            if (nTemp == 0)
            {
                m_nPrintMethod = 1;
            }
            else if (nTemp > 0)
            {   // 1 = 용지맞춤, 2 = 나눠찍기, 3 = 자동 모아찍기 설정은 무시
                m_nPrintMethod = nTemp + 3;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FormConfig_Load(object sender, EventArgs e)
        {
            if (m_option_overwite == 0) radioButton_newname.Checked = true;
            else if (m_option_overwite == 1) radioButton_skip.Checked = true;
            else if (m_option_overwite == 2) radioButton_overwrite.Checked = true;
            foreach (string temp_ext in FormMain.source_ext_array)
            {
                ListViewItem new_item = list_ext_option.Items.Add(temp_ext);
                // if no selection saved, default to checked
                if (m_option_source_ext_set.Count == 0)
                    new_item.Checked = true;
                else
                    new_item.Checked = m_option_source_ext_set.Contains(temp_ext.ToLower());
            }

            if (m_option_PDF_print == true) radio_PDF_Print.Checked = true;
            else radio_PDF_SaveAs.Checked = true;
            UpdatePDFOption(m_option_PDF_print);
            // PDF 변환용 프린터를 콤보박스에 추가
            System.Collections.ArrayList printer_names
                = new System.Collections.ArrayList(System.Drawing.Printing.PrinterSettings.InstalledPrinters);
            for (int i = 0; i < printer_names.Count; i++)
            {
                string name = printer_names[i].ToString();
                if (name.Contains("PDF"))
                {
                    int nIndex = comboBox_PDF_Printer.Items.Add(name);
                    if (name == m_strPrinter) comboBox_PDF_Printer.SelectedIndex = nIndex;
                }
            }
            if (comboBox_PDF_Printer.Items.Count > 0 && comboBox_PDF_Printer.SelectedIndex <= 0) 
                comboBox_PDF_Printer.SelectedIndex = 0;

            //모아찍기 설정값 선택
            if (m_nPrintMethod == 1)
            {
                comboBox_PDF_PrintMethod.SelectedIndex = 0;
            }
            else if (m_nPrintMethod > 3)
            {   // 1 = 용지맞춤, 2 = 나눠찍기, 3 = 자동 모아찍기 설정은 무시
                comboBox_PDF_PrintMethod.SelectedIndex = m_nPrintMethod - 3;
            }
        }
        private void UpdatePDFOption(bool bPDF)
        {
            label_PDF_Printer.Enabled = bPDF;
            label_PDF_PrintMethod.Enabled = bPDF;
            comboBox_PDF_Printer.Enabled = bPDF;
            comboBox_PDF_PrintMethod.Enabled = bPDF;
        }
        private void radio_PDF_SaveAs_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePDFOption(false);
        }

        private void radio_PDF_Print_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePDFOption(true);
        }
    }
}
