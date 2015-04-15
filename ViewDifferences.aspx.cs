using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FolderCompare.Helpers;
using FolderDiff;
using Telerik.Web.UI;

namespace FolderCompare
{
    public partial class ViewDifferences : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string fileStaging = Request.QueryString["FileStaging"];
                string fileTest = Request.QueryString["FileTest"];

                if (fileStaging != null)
                {
                    txtStagingFileText.Text = File.ReadAllText(fileStaging);

                }
                if (fileTest != null)
                {
                    txtTestFileText.Text = File.ReadAllText(fileTest);

                }

            }
        }

        protected void btnSaveToStagingMerged_OnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMergedText.Text))
            {
                File.WriteAllText(Request.QueryString["FileStaging"], txtMergedText.Text);
                InfoPanel.Visible = true;
                lblMessage.Text = string.Format("File '{0}' saved/merged successfully", Request.QueryString["FileStaging"]);

            }
        }

        protected void btnSaveToTestMerged_OnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMergedText.Text))
            {
                File.WriteAllText(Request.QueryString["FileTest"], txtMergedText.Text);
                InfoPanel.Visible = true;
                lblMessage.Text = string.Format("File '{0}' saved/merged successfully", Request.QueryString["FileTest"]);

            }
        }



        protected void btnSaveToStaging_OnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtStagingFileText.Text))
            {
                File.WriteAllText(Request.QueryString["FileStaging"], txtStagingFileText.Text);
                InfoPanel.Visible = true;
                lblMessage.Text = string.Format("File '{0}' saved successfully", Request.QueryString["FileStaging"]);
            }
        }

        protected void btnSaveToTest_OnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTestFileText.Text))
            {
                File.WriteAllText(Request.QueryString["FileTest"], txtTestFileText.Text);
                InfoPanel.Visible = true;
                lblMessage.Text = string.Format("File '{0}' saved successfully", Request.QueryString["FileTest"]);
            }
        }
    }
}