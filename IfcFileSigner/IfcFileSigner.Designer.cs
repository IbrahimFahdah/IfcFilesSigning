namespace IfcFileSigner
{
    partial class IfcFileSigner
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSignIfcFile = new Button();
            openFileDialog1 = new OpenFileDialog();
            btnValidate = new Button();
            SuspendLayout();
            // 
            // btnSignIfcFile
            // 
            btnSignIfcFile.Location = new Point(44, 56);
            btnSignIfcFile.Name = "btnSignIfcFile";
            btnSignIfcFile.Size = new Size(210, 62);
            btnSignIfcFile.TabIndex = 0;
            btnSignIfcFile.Text = "Sign IFC File";
            btnSignIfcFile.UseVisualStyleBackColor = true;
            btnSignIfcFile.Click += btnSignIfcFile_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnValidate
            // 
            btnValidate.Location = new Point(292, 56);
            btnValidate.Name = "btnValidate";
            btnValidate.Size = new Size(204, 62);
            btnValidate.TabIndex = 0;
            btnValidate.Text = "Validate Signed IFC File";
            btnValidate.UseVisualStyleBackColor = true;
            btnValidate.Click += btnValidate_Click;
            // 
            // IfcFileSigner
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(563, 203);
            Controls.Add(btnValidate);
            Controls.Add(btnSignIfcFile);
            Name = "IfcFileSigner";
            Text = "Signing IFC Files Demo";
            ResumeLayout(false);
        }

        #endregion

        private Button btnSignIfcFile;
        private OpenFileDialog openFileDialog1;
        private Button btnValidate;
    }
}