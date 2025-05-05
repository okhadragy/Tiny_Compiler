using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace TINY_Compiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox2.Multiline = true;
            textBox2.ScrollBars = ScrollBars.Vertical;
        }

        private void clear()
        {
            Errors.Error_List.Clear();
            textBox2.Clear();
            TINY_Compiler.Tiny_Scanner.Tokens.Clear();
            dataGridView1.Rows.Clear();
            treeView1.Nodes.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clear();

            string Code = textBox1.Text;
            TINY_Compiler.Start_Compiling(Code);
            treeView1.Nodes.Add(Parser.PrintParseTree(TINY_Compiler.treeroot));
            PrintTokens();
            PrintErrors();
        }
        void PrintTokens()
        {
            for (int i = 0; i < TINY_Compiler.Tiny_Scanner.Tokens.Count; i++)
            {
               dataGridView1.Rows.Add(TINY_Compiler.Tiny_Scanner.Tokens.ElementAt(i).lex, TINY_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type);
            }
        }

        void PrintErrors()
        {
            for(int i=0; i<Errors.Error_List.Count; i++)
            {
                textBox2.Text += Errors.Error_List[i];
                textBox2.Text += "\r\n";
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
