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
    public partial class _Default : Page
    {

        private readonly string[] knownExtensions = new string[]
        {
            "aspx", "asmx", "cs", "rar","txt","config",
            "vb", "xml", "ascx", "jpg", "png", "gif", "html"
        };

        private void BindTreeToDirectory(string sourcePath, RadTreeNode sourceTreeNode, bool replace, string css, bool enableMenu)
        {
            var directoriesStaging = new string[] { };

            if (Directory.Exists(sourcePath))
            {
                directoriesStaging = Directory.GetDirectories(sourcePath);
            }


            foreach (string directory in directoriesStaging)
            {
                RadTreeNode node = new RadTreeNode(Path.GetFileName(directory));
                node.Value = replace ? directory.Replace(txtStagingPath.Text, txtTestPath.Text) : directory;
                node.ImageUrl = "~/images/folder.png";
                node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                node.EnableContextMenu = enableMenu;

                node.CssClass = css;


                sourceTreeNode.Nodes.Add(node);


                BindTreeToDirectory(node.Value = replace ? directory.Replace(txtTestPath.Text, txtStagingPath.Text) : directory, node, replace, css, enableMenu);

                //parentNode.Expanded = true;
                //node.Expanded = true;
            }


            string[] files = Directory.GetFiles(sourcePath);

            foreach (string file in files)
            {
                RadTreeNode node = new RadTreeNode(Path.GetFileName(file));
                string extension = Path.GetExtension(file).ToLower().TrimStart('.');

                node.Value = replace ? file.Replace(txtStagingPath.Text, txtTestPath.Text) : file;

                // node.CssClass = File.Exists(node.Value) ? "exists_on_test" : "exists_on_staging";

                node.NavigateUrl = file;
                node.EnableContextMenu = enableMenu;

                if (Array.IndexOf(knownExtensions, extension) > -1)
                {
                    node.ImageUrl = "~/images/" + extension + ".png";
                }
                else
                {
                    node.ImageUrl = "~/images/unknown.png";
                }

                sourceTreeNode.Nodes.Add(node);

            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                RadTreeNode rootNode = new RadTreeNode("Staging");
                rootNode.Value = "~/";
                rootNode.ImageUrl = "~/images/folder.png";
                rootNode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                rootNode.Expanded = true;
                rootNode.EnableContextMenu = false;

                treeStagingPath.Nodes.Add(rootNode);

                RadTreeNode rootNodeTest = new RadTreeNode("Test");
                rootNodeTest.Value = "~/";
                rootNodeTest.ImageUrl = "~/images/folder.png";
                rootNodeTest.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                rootNodeTest.Expanded = true;
                rootNodeTest.EnableContextMenu = false;

                treeTestPath.Nodes.Add(rootNodeTest);


            }
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        protected void StagingTreeView_ContextMenuItemClick(object sender, RadTreeViewContextMenuEventArgs e)
        {
            RadTreeNode clickedNode = e.Node;
            var isFile = !clickedNode.ImageUrl.Contains("folder.png");
            var existsOnLeft = clickedNode.CssClass == "exists_on_staging";

            switch (e.MenuItem.Value)
            {

                case "Copy":


                    if (isFile)
                    {

                        var dest = string.IsNullOrEmpty(clickedNode.Text)
                            ? Path.GetDirectoryName(clickedNode.Value)
                                .Replace(txtTestPath.Text, txtStagingPath.Text)
                            : Path.GetDirectoryName(clickedNode.Value)
                                .Replace(txtStagingPath.Text, txtTestPath.Text);

                        if (!Directory.Exists(dest))
                            Directory.CreateDirectory(dest);

                        if (existsOnLeft)
                        {
                            File.Copy(clickedNode.Value, Path.Combine(dest, Path.GetFileName(clickedNode.Value)));
                            lblMessage.Text = string.Format("File '{0}' was copied successfully from {1} to {2}", clickedNode.Value, "Test", "Staging");

                        }
                        else
                        {
                            File.Copy(clickedNode.Value, Path.Combine(dest, clickedNode.Text));
                            lblMessage.Text = string.Format("File '{0}' was copied successfully from {1} to {2}", clickedNode.Value, "Staging", "Test");

                        }
                    }
                    else
                    {
                        var dest = !existsOnLeft
                            ? clickedNode.Value.Replace(txtTestPath.Text, txtStagingPath.Text)
                            : clickedNode.Value.Replace(txtStagingPath.Text, txtTestPath.Text);


                        // var dest = clickedNode.Value.Replace(txtStagingPath.Text, txtTestPath.Text);

                        if (!Directory.Exists(dest))
                            Directory.CreateDirectory(dest);

                        if (existsOnLeft)
                        {
                            CopyAll(new DirectoryInfo(clickedNode.Value), new DirectoryInfo(dest));
                            lblMessage.Text = string.Format("Folder '{0}' was copied successfully from {1} to {2}", clickedNode.Value, "Staging", "Staging");

                        }
                        else
                        {
                            CopyAll(new DirectoryInfo(dest), new DirectoryInfo(clickedNode.Value));
                            lblMessage.Text = string.Format("Folder '{0}' was copied successfully from {1} to {2}", clickedNode.Value, "Test", "Staging");

                        }

                    }

                    txtOutPut.Text = "";

                    CompareThem();

                    InfoPanel.Visible = true;

                    //RadTreeNode clonedNode = clickedNode.Clone();
                    //clonedNode.Text = string.Format("Copy of {0}", clickedNode.Text);
                    //clickedNode.InsertAfter(clonedNode);
                    ////set node's value so we can find it in startNodeInEditMode
                    //clonedNode.Value = clonedNode.GetFullPath("/");
                    //clonedNode.Selected = true;
                    // StartNodeInEditMode(clonedNode.Value);
                    break;

                case "ViewDiff":
                    //EmptyFolder(clickedNode, false);

                    Response.Redirect("ViewDifferences.aspx?FileStaging=" + clickedNode.Value + "&FileTest=" +
                                      clickedNode.Value.Replace(txtStagingPath.Text, txtTestPath.Text));





                    break;
                case "Delete":
                    InfoPanel.Visible = true;

                    clickedNode.Remove();

                    if (isFile)
                    {
                        File.Delete(clickedNode.Value);
                        lblMessage.Text = string.Format("File '{0}' was deleted successfully", clickedNode.Value);

                    }
                    else
                    {
                        Directory.Delete(clickedNode.Value, true);
                        lblMessage.Text = string.Format("Folder '{0}' was deleted successfully", clickedNode.Value);

                    }

                    break;
            }
        }
        protected void TestTreeView_ContextMenuItemClick(object sender, RadTreeViewContextMenuEventArgs e)
        {
            RadTreeNode clickedNode = e.Node;
            var isFile = !clickedNode.ImageUrl.Contains("folder.png");
            var existsOnTest = clickedNode.CssClass == "exists_on_test";


            switch (e.MenuItem.Value)
            {
                case "Copy":

                    if (isFile)
                    {

                        string dest;
                        if (existsOnTest)
                            dest = Path.GetDirectoryName(clickedNode.Value)
                                .Replace(txtTestPath.Text, txtStagingPath.Text);

                        else
                            dest = Path.GetDirectoryName(clickedNode.Value);
                        //.Replace(txtTestPath.Text, txtStagingPath.Text);


                        if (!Directory.Exists(dest))
                            Directory.CreateDirectory(dest);

                        if (!existsOnTest)
                        {
                            File.Copy(clickedNode.Value.Replace(txtTestPath.Text, txtStagingPath.Text),
                               clickedNode.Value);
                            //copy from staging
                            lblMessage.Text = string.Format("File '{0}' was copied successfully from {1} to {2}", clickedNode.Value, "Staging", "Test");


                        }
                        else
                        {
                            File.Copy(clickedNode.Value,
                                clickedNode.Value.Replace(txtTestPath.Text, txtStagingPath.Text));
                            lblMessage.Text = string.Format("File '{0}' was copied successfully from {1} to {2}", clickedNode.Value, "Test", "Staging");

                        }

                    }
                    else
                    {
                        var dest = existsOnTest ?
                            clickedNode.Value.Replace(txtTestPath.Text, txtStagingPath.Text) :
                            clickedNode.Value.Replace(txtStagingPath.Text, txtTestPath.Text);

                        // var dest = clickedNode.Value.Replace(txtStagingPath.Text, txtTestPath.Text);

                        if (!Directory.Exists(dest))
                            Directory.CreateDirectory(dest);

                        if (!existsOnTest)
                        {
                            CopyAll(new DirectoryInfo(dest), new DirectoryInfo(clickedNode.Value));
                            lblMessage.Text = string.Format("Folder '{0}' was copied successfully from {1} to {2}", clickedNode.Value, "Staging", "Test");

                        }
                        else
                        {
                            CopyAll(new DirectoryInfo(clickedNode.Value), new DirectoryInfo(dest));

                            lblMessage.Text = string.Format("Folder '{0}' was copied successfully from {1} to {2}", clickedNode.Value, "Test", "Staging");

                        }

                    }

                    txtOutPut.Text = "";

                    CompareThem();

                    InfoPanel.Visible = true;

                    break;

                case "ViewDiff":

                    Response.Redirect("ViewDifferences.aspx?FileTest=" + clickedNode.Value + "&FileStaging=" +
                                  clickedNode.Value.Replace(txtTestPath.Text, txtStagingPath.Text));
                    
                    break;
                case "Delete":
                    //clickedNode.Remove();

                    InfoPanel.Visible = true;

                    clickedNode.Remove();

                    if (isFile)
                    {
                        File.Delete(clickedNode.Value);
                        lblMessage.Text = string.Format("File '{0}' was deleted successfully", clickedNode.Value);

                    }
                    else
                    {
                        Directory.Delete(clickedNode.Value, true);
                        lblMessage.Text = string.Format("Folder '{0}' was deleted successfully", clickedNode.Value);

                    }

                    break;
            }
        }

        protected void btnStartCompare_OnClick(object sender, EventArgs e)
        {
            txtOutPut.Text = "";
            CompareThem();


            // treeStagingPath.ExpandAllNodes();


            //BindTreeToDirectory(txtTestPath.Text, treeTestPath.Nodes[0]);

            //treeTestPath.ExpandAllNodes();

            //var dirStaging = new System.IO.DirectoryInfo(stagingPath);
            //var dirTesting = new System.IO.DirectoryInfo(testingPath);

            //// Take a snapshot of the files.
            //IEnumerable<System.IO.FileInfo> filesStagingList = dirStaging.GetFiles("*.*",
            //    System.IO.SearchOption.AllDirectories);
            //IEnumerable<System.IO.FileInfo> filesTestList = dirTesting.GetFiles("*.*",
            //    System.IO.SearchOption.AllDirectories);

            //// Take a snapshot of the folders
            //IEnumerable<System.IO.DirectoryInfo> foldersStagingList = dirStaging.GetDirectories("*.*",
            //    System.IO.SearchOption.AllDirectories);
            //IEnumerable<System.IO.DirectoryInfo> foldersTestList = dirTesting.GetDirectories("*.*",
            //    System.IO.SearchOption.AllDirectories);


            ////A custom file comparer defined below
            //var myFileCompare = new FileCompare();

            //// This query determines whether the two folders contain 
            //// identical file lists, based on the custom file comparer 
            //// that is defined in the FileCompare class. 
            //// The query executes immediately because it returns a bool. 
            //bool areIdentical = filesStagingList.SequenceEqual(filesTestList, myFileCompare);

            //if (areIdentical == true)
            //{
            //    txtOutPut.Text += "The two folders are the same\n";
            //}
            //else
            //{
            //    txtOutPut.Text += "The two folders are not the same\n";
            //}

            //// Find the common files. It produces a sequence and doesn't  
            //// execute until the foreach statement. 
            //var queryCommonFiles = filesStagingList.Intersect(filesTestList, myFileCompare);

            //if (queryCommonFiles.Count() > 0)
            //{
            //    //txtOutPut.Text += "The following files are in both folders:";

            //    //foreach (var v in queryCommonFiles)
            //    //{
            //    //    txtOutPut.Text += v.FullName + Environment.NewLine; //shows which items end up in result list
            //    //}
            //}
            //else
            //{

            //    txtOutPut.Text += "There are no common files in the two folders.\n";
            //}

            //// Find the set difference between the two folders. 
            //// For this example we only check one way. 
            //var queryList1Only = (from file in filesStagingList
            //                      select file).Except(filesTestList, myFileCompare);

            //txtOutPut.Text += "These are the diffrences:\n";

            //foreach (var v in queryList1Only)
            //{
            //    txtOutPut.Text += v.FullName + Environment.NewLine;
            //}



        }

        private void CompareThem()
        {
            string stagingPath = txtStagingPath.Text;
            string testingPath = txtTestPath.Text;

            treeStagingPath.Nodes[0].Nodes.Clear();
            treeTestPath.Nodes[0].Nodes.Clear();

            //BindTreeToDirectory(stagingPath, treeStagingPath.Nodes[0], testingPath, treeTestPath.Nodes[0]);
            //BindTreeToDirectory(testingPath, treeTestPath.Nodes[0], stagingPath, treeStagingPath.Nodes[0]);

            BindTreeToDirectory(stagingPath, treeStagingPath.Nodes[0], false, "", true);

            BindTreeToDirectory(testingPath, treeTestPath.Nodes[0], false, "", true);

            //treeTestPath.Nodes[0].ExpandChildNodes();
            //treeStagingPath.Nodes[0].ExpandChildNodes();

            FindDiffsOnTrees(stagingPath, testingPath);
        }

        private void FindDiffsOnTrees(string stagingPath, string testingPath)
        {
            var folderdiff = new FolderDiff.FolderDiff(stagingPath, testingPath);
            var diffs = folderdiff.GetDiffs().OrderBy(o => o.DiffType).Reverse().ToList();

            //grdChanges.DataSource = diffs;
            //grdChanges.DataBind();


            if (diffs.Count > 0)
            {
                var fsItems = new List<FileSystemItem>();


                txtOutPut.Text += "The two folders are not the same\n";
                foreach (var diff in diffs)
                {
                    var css = "";


                    switch (diff.DiffType)
                    {
                        case DiffType.FolderDelete:
                            {
                                fsItems.Add(new FileSystemItem(new DirectoryInfo(diff.SourcePath)));

                                RadTreeNode node = new RadTreeNode(Path.GetFileName(diff.SourcePath));
                                //RadTreeNode node = new RadTreeNode(Path.GetFileName(diff.DestinationPath));

                                css = Directory.Exists(diff.DestinationPath) ? "exists_on_test" : "exists_on_staging";

                                node.Value = diff.DestinationPath;
                                node.ImageUrl = "~/images/folder.png";
                                node.CssClass = css;
                                node.EnableContextMenu = true;
                                node.Expanded = true;

                                var found = treeStagingPath.Nodes[0].Nodes.FindNodeByValue(diff.SourcePath) ??
                                            treeStagingPath.GetAllNodes().FirstOrDefault(n => n.Value == diff.SourcePath);


                                found.CssClass = css;
                                found.EnableContextMenu = true;
                                found.Expanded = true;
                                found.ExpandChildNodes();

                                foreach (var fn in found.GetAllNodes())
                                {
                                    fn.CssClass = css;
                                    fn.EnableContextMenu = true;

                                }


                                var firstFolder = diff.SourcePath.Between(stagingPath, "\\").Trim(Convert.ToChar("\\"));

                                var foundNode = treeTestPath.GetAllNodes().FirstOrDefault(n =>
                                          n.Value == Path.Combine(testingPath, firstFolder));

                                //works

                                if (foundNode != null)
                                {
                                    //node.Value = diff.DestinationPath;
                                    node.ImageUrl = "~/images/folder.png";
                                    node.CssClass = foundNode.CssClass;
                                    node.Text = firstFolder;
                                    node.Value = Path.Combine(stagingPath, firstFolder);
                                    BindTreeToDirectory(node.Value, node, true, "exists_on_staging", true);
                                    foundNode.ExpandChildNodes();
                                    foundNode.ParentNode.Nodes.Insert(foundNode.Index, node);

                                    foundNode.EnableContextMenu = true;
                                    foundNode.Remove();

                                }
                                //top node
                                else
                                {
                                    var cnt = treeTestPath.Nodes[0].Nodes.Count;
                                    BindTreeToDirectory(found.Value, node, true, "exists_on_staging", true);
                                    node.ExpandChildNodes();
                                    treeTestPath.Nodes[0].Nodes.Insert(cnt < found.Index ? cnt : found.Index, node);
                                }

                                //treeTestPath.Nodes[0].Nodes.Insert(cnt < found.Index ? cnt : found.Index, node);


                                txtOutPut.Text += string.Format("Folder exists only on staging --- {0}\n", diff.SourcePath);

                                break;
                            }

                        case DiffType.FolderAdd:
                            {
                                fsItems.Add(new FileSystemItem(new DirectoryInfo(diff.SourcePath)));
                                //exists_on_test

                                css = Directory.Exists(diff.DestinationPath) ? "exists_on_test" : "exists_on_staging";

                                RadTreeNode node = new RadTreeNode(Path.GetFileName(diff.SourcePath));
                                node.Value = diff.DestinationPath;
                                node.ImageUrl = "~/images/folder.png";
                                node.CssClass = css;
                                node.EnableContextMenu = true;
                                node.Expanded = true;

                                var found = treeTestPath.Nodes[0].Nodes.FindNodeByValue(diff.DestinationPath) ??
                                            treeTestPath.GetAllNodes().FirstOrDefault(n => n.Value == diff.DestinationPath);

                                found.CssClass = css;
                                found.EnableContextMenu = true;
                                found.Expanded = true;
                                found.ExpandChildNodes();



                                foreach (var fn in found.GetAllNodes())
                                {
                                    fn.CssClass = css;
                                    fn.EnableContextMenu = true;
                                }



                                var firstFolder = diff.SourcePath.Between(stagingPath, "\\").Trim(Convert.ToChar("\\"));

                                var foundNode = treeStagingPath.GetAllNodes().FirstOrDefault(n =>
                                          n.Value == Path.Combine(stagingPath, firstFolder));

                                if (foundNode != null)
                                {
                                    //node.Value = diff.DestinationPath;
                                    node.ImageUrl = "~/images/folder.png";
                                    node.CssClass = foundNode.CssClass;
                                    node.EnableContextMenu = true;
                                    node.Text = firstFolder;
                                    node.Value = Path.Combine(testingPath, firstFolder);
                                    BindTreeToDirectory(node.Value, node, false, "exists_on_test", true);
                                    foundNode.ParentNode.Nodes.Insert(foundNode.Index, node);
                                    foundNode.EnableContextMenu = true;
                                    foundNode.Remove();
                                }
                                //top node
                                else
                                {
                                    var cnt = treeStagingPath.Nodes[0].Nodes.Count;
                                    BindTreeToDirectory(node.Value, node, false, "exists_on_test", true);
                                    node.ExpandChildNodes();
                                    treeStagingPath.Nodes[0].Nodes.Insert(cnt < found.Index ? cnt : found.Index, node);

                                }


                                txtOutPut.Text += string.Format("Folder exists only on test --- {0}\n", diff.SourcePath);


                                break;
                            }

                        case DiffType.FileAdd:
                            {
                                fsItems.Add(new FileSystemItem(new FileInfo(diff.DestinationPath)));
                                css = File.Exists(diff.DestinationPath) ? "exists_on_test" : "exists_on_staging";

                                // exists_on_test
                                RadTreeNode node = new RadTreeNode(Path.GetFileName(diff.SourcePath));
                                node.Value = diff.DestinationPath;
                                node.CssClass = css;
                                node.EnableContextMenu = true;
                                node.Expanded = true;

                                var extension = Path.GetExtension(diff.DestinationPath).ToLower().TrimStart('.');

                                if (Array.IndexOf(knownExtensions, extension) > -1)
                                {
                                    node.ImageUrl = "~/images/" + extension + ".png";
                                }
                                else
                                {
                                    node.ImageUrl = "~/images/unknown.png";
                                }

                                var found = treeTestPath.Nodes[0].Nodes.FindNodeByValue(diff.DestinationPath) ??
                                             treeTestPath.GetAllNodes().FirstOrDefault(n => n.Value == diff.DestinationPath);

                                found.CssClass = css;
                                found.EnableContextMenu = true;
                                found.Expanded = true;
                                found.ExpandChildNodes();
                                found.ExpandParentNodes();

                                foreach (var fn in found.GetAllNodes())
                                {
                                    fn.CssClass = css;
                                    fn.EnableContextMenu = true;
                                    fn.ExpandChildNodes();
                                }

                                var foundParentNode = treeStagingPath.GetAllNodes().FirstOrDefault(n =>
                                         n.Value == Path.GetDirectoryName(diff.SourcePath));



                                if (foundParentNode != null)
                                {
                                    //node.Value = diff.DestinationPath;

                                    //node.Text = firstFolder;
                                    //node.Value = Path.Combine(stagingPath, firstFolder);
                                    //BindTreeToDirectory(node.Value, node, true, "exists_on_test");
                                    foundParentNode.ExpandChildNodes();
                                    foundParentNode.ExpandParentNodes();
                                    foundParentNode.Nodes.Add(node);
                                    node.ExpandChildNodes();
                                    node.ExpandParentNodes();

                                }
                                else
                                {
                                    var cnt = treeTestPath.Nodes[0].Nodes.Count;
                                    treeStagingPath.Nodes[0].Nodes.Insert(cnt < found.Index ? cnt : found.Index, node);

                                }

                                txtOutPut.Text += string.Format("File exists only on test --- {0}\n", diff.SourcePath);

                                break;
                            }
                        case DiffType.FileDelete:
                            {
                                // exists_on_test

                                fsItems.Add(
                                    new FileSystemItem(
                                        new FileInfo(File.Exists(diff.DestinationPath) ? diff.DestinationPath : diff.SourcePath)));

                                css = File.Exists(diff.DestinationPath) ? "exists_on_test" : "exists_on_staging";


                                RadTreeNode node =
                                    new RadTreeNode(File.Exists(diff.DestinationPath)
                                        ? Path.GetFileName(diff.DestinationPath)
                                        : Path.GetFileName(diff.SourcePath));
                                node.Value = diff.DestinationPath;
                                node.CssClass = css;
                                node.EnableContextMenu = true;

                                string extension = Path.GetExtension(diff.DestinationPath).ToLower().TrimStart('.');

                                if (Array.IndexOf(knownExtensions, extension) > -1)
                                {
                                    node.ImageUrl = "~/images/" + extension + ".png";
                                }
                                else
                                {
                                    node.ImageUrl = "~/images/unknown.png";
                                }


                                var found = treeStagingPath.Nodes[0].Nodes.FindNodeByValue(diff.SourcePath) ??
                                            treeStagingPath.GetAllNodes().FirstOrDefault(n => n.Value == diff.SourcePath);

                                found.CssClass = css;
                                found.EnableContextMenu = true;
                                found.Expanded = true;
                                found.ExpandChildNodes();
                                found.ExpandParentNodes();

                                foreach (var fn in found.GetAllNodes())
                                {
                                    fn.CssClass = css;
                                    fn.EnableContextMenu = true;
                                    fn.ExpandChildNodes();

                                }

                                var foundParentNode = treeStagingPath.GetAllNodes().FirstOrDefault(n =>
                                         n.Value == Path.GetFileName(diff.SourcePath));


                                if (foundParentNode != null)
                                {
                                    foundParentNode.ExpandParentNodes();
                                    node.ExpandParentNodes();
                                    node.ExpandChildNodes();
                                    foundParentNode.Nodes.Add(node);

                                }
                                else
                                {
                                    var cnt = treeTestPath.Nodes[0].Nodes.Count;
                                    treeTestPath.Nodes[0].Nodes.Insert(cnt < found.Index ? cnt : found.Index, node);
                                }

                                txtOutPut.Text += string.Format("File exists only on staging --- {0}\n", diff.SourcePath);

                                break;
                            }

                        case DiffType.FileUpdate:
                            {
                                fsItems.Add(new FileSystemItem(new FileInfo(diff.SourcePath)));

                                fsItems.Add(new FileSystemItem(new FileInfo(diff.DestinationPath)));

                                var found = treeStagingPath.Nodes[0].Nodes.FindNodeByValue(diff.SourcePath);
                                found.CssClass = "itisdifferent";

                                found.EnableContextMenu = true;

                                var found1 = treeTestPath.Nodes[0].Nodes.FindNodeByValue(diff.DestinationPath);
                                found1.CssClass = "itisdifferent";
                                found1.EnableContextMenu = true;
                                treeTestPath.ContextMenus[0].Items[2].Enabled = true;

                                //treeStagingPath.Nodes[0].Nodes.Insert(found.Index, node);

                                txtOutPut.Text += string.Format("File exists on both folder but it is different --- {0}\n",
                                    diff.SourcePath);

                                break;
                            }
                    }
                }

                grdChanges.DataSource = fsItems;
                grdChanges.DataBind();
            }

            else
            {
                txtOutPut.Text += "The two folders are the same\n";
            }
        }

        internal static List<DirectoryInfo> Split(DirectoryInfo path)
        {
            if (path == null) throw new ArgumentNullException("path");
            var ret = new List<DirectoryInfo>();
            if (path.Parent != null) ret.AddRange(Split(path.Parent));
            ret.Add(path);
            return ret;
        }

    }
}