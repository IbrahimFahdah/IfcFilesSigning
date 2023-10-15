using IfcFileSigningCommon;
using IfcFileSigningUtility;

namespace IfcFileSigner
{
    public partial class IfcFileSigner : Form
    {
        public IfcFileSigner()
        {
            InitializeComponent();
        }

        private void btnSignIfcFile_Click(object sender, EventArgs e)
        {
            try
            {
                var dlg = openFileDialog1.ShowDialog(this);
                var fileName = openFileDialog1.FileName;
                IfcFileDigitalSignature.SignIfcFile(fileName);
                MessageBox.Show($"File Signed Successfully.", "Signature", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message );
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            try
            {
                var dlg = openFileDialog1.ShowDialog(this);
                var fileName = openFileDialog1.FileName;
                var res = IfcSignatureValidator.ValidateFile(fileName);

                if(res.status==IfcSignatureStatus.Valid)
                    MessageBox.Show($"VALID FILE. File Signed By: {res.singedBy}", "Signature",MessageBoxButtons.OK,MessageBoxIcon.Information);
                if (res.status == IfcSignatureStatus.Invalid)
                    MessageBox.Show($"Invalid file. File data changed since it is last signed","Signature", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (res.status == IfcSignatureStatus.NotFound)
                    MessageBox.Show($"File has not signature", "Signature", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}