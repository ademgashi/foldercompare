<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewDifferences.aspx.cs" Inherits="FolderCompare.ViewDifferences" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="CodeMirror/lib/codemirror.css">
    <link rel="stylesheet" href="CodeMirror/addon/merge/merge.css">

    <script src="CodeMirror/lib/codemirror.js"></script>
    <script src="CodeMirror/mode/xml/xml.js"></script>
    <script src="CodeMirror/mode/css/css.js"></script>
    <script src="CodeMirror/mode/clike/clike.js"></script>

    <script src="CodeMirror/mode/javascript/javascript.js"></script>
    <script src="CodeMirror/mode/htmlmixed/htmlmixed.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/diff_match_patch/20121119/diff_match_patch.js"></script>
    <script src="CodeMirror/addon/merge/merge.js"></script>



    <style type="text/css">
        .CodeMirror {
            border: 1px solid #eee;
            height: auto;
        }
    </style>

    <article>
        <h2>Merge view </h2>
        <div class="row">
            <div class="col-md-4">
                <h3>Staging File</h3>
            </div>
            <div class="col-md-4">
                <h3>Merged File</h3>
            </div>
            <div class="col-md-4">
                <h3>Test File</h3>
            </div>
        </div>

        <div id="view" runat="server"></div>




        <script>
            var value, orig1, orig2, dv, panes = 3, highlight = true, connect = null, collapse = true;
            function initUI() {
                if (value == null) return;
                var target = document.getElementById('<%= view.ClientID %>');
                target.innerHTML = "";
                dv = CodeMirror.MergeView(target, {
                    value: value,
                    origLeft: panes == 3 ? orig1 : null,
                    orig: orig2,
                    lineNumbers: true,
                    mode: "text/x-csharp",
                    highlightDifferences: highlight,
                    connect: connect,
                    collapseIdentical: collapse,
                    allowEditingOriginals: true,

                });
            }

            function toggleDifferences() {
                dv.setShowDifferences(highlight = !highlight);
            }

            function CopyTextStaging() {

                var text = dv.leftOriginal().getValue();
                if (confirm('Are you sure you want to save staging file?')) {
                    document.getElementById('<%= txtStagingFileText.ClientID %>').value = text;
                    return true;
                } else {
                    return false;
                }
            }


            function CopyTextTest() {

                var text = dv.rightOriginal().getValue();
                if (confirm('Are you sure you want to save test file?')) {
                    document.getElementById('<%= txtTestFileText.ClientID %>').value = text;
                    return true;
                } else {
                    return false;
                }
            }

            function copyTestTextMerged() {
                //Get the text in the first input
                var text = dv.editor().getValue();
                //Can't use .value here as it returns the starting value of the editor

                //Set the text in the second input to what we copied from the first
                document.getElementById('<%= txtMergedText.ClientID %>').value = text;

                if (confirm('Are you sure you want to save file?')) {
                    document.getElementById('<%= txtTestFileText.ClientID %>').value = text;
                    return true;
                } else {
                    return false;
                }
            }


            function copyStagingTextMerged() {
                //Get the text in the first input
                var text = dv.editor().getValue();
                //Can't use .value here as it returns the starting value of the editor

                //Set the text in the second input to what we copied from the first
                document.getElementById('<%= txtMergedText.ClientID %>').value = text;

                if (confirm('Are you sure you want to save file?')) {
                    document.getElementById('<%= txtStagingFileText.ClientID %>').value = text;
                    return true;
                } else {
                    return false;
                }
            }


            window.onload = function () {
                value = document.getElementById('<%= txtStagingFileText.ClientID %>').value;
                orig1 = document.getElementById('<%= txtTestFileText.ClientID %>').value;
                orig2 = document.getElementById('<%= txtTestFileText.ClientID %>').value;
                initUI();
            };

            function mergeViewHeight(mergeView) {
                function editorHeight(editor) {
                    if (!editor) return 0;
                    return editor.getScrollInfo().height;
                }
                return Math.max(editorHeight(mergeView.leftOriginal()),
                                editorHeight(mergeView.editor()),
                                editorHeight(mergeView.rightOriginal()));
            }

            function resize(mergeView) {
                var height = mergeViewHeight(mergeView);
                for (; ;) {
                    if (mergeView.leftOriginal())
                        mergeView.leftOriginal().setSize(null, height);
                    mergeView.editor().setSize(null, height);
                    if (mergeView.rightOriginal())
                        mergeView.rightOriginal().setSize(null, height);

                    var newHeight = mergeViewHeight(mergeView);
                    if (newHeight >= height) break;
                    else height = newHeight;
                }
                mergeView.wrap.style.height = height + "px";
            }
</script>
    </article>
    <br />
    <br />
    <div class="row">
        <div class="col-md-4">
            <asp:Button ID="btnSaveToStaging"
                OnClick="btnSaveToStaging_OnClick" OnClientClick="return CopyTextStaging();"
                class="btn btn-success" runat="server" Text="Save to Staging" />

        </div>
        <div class="col-md-4 text-center">
            <br />
            <h4>Save merged file</h4>

            <p class="tpbutton btn-toolbar">

                <asp:Button ID="btnSaveToStagingMerged" OnClick="btnSaveToStagingMerged_OnClick"
                    class="btn navbar-btn btn-primary btn-sm" runat="server" OnClientClick="return copyStagingTextMerged();"
                    Text="Staging" />

                <asp:Button ID="btnSaveToTestMerged" OnClick="btnSaveToTestMerged_OnClick"
                    class="btn navbar-btn btn-primary btn-sm" runat="server" OnClientClick="return copyTestTextMerged();"
                    Text="Test" />

                
            </p>

        </div>
        <div class="col-md-4">

            <asp:Button ID="btnSaveToTest" OnClick="btnSaveToTest_OnClick"
                OnClientClick="return CopyTextTest();" class="btn navbar-btn btn-success" runat="server"
                Text="Save to Test" />

        </div>
    </div>

    <div style="display: none;">

        <asp:TextBox ID="txtStagingFileText" TextMode="MultiLine" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtTestFileText" TextMode="MultiLine" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtMergedText" TextMode="MultiLine" runat="server"></asp:TextBox>

    </div>

    <div class="row">
        <asp:Panel ID="InfoPanel" runat="server" class="alert alert-success alert-dismissable" Visible="False">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true"></button>
            <i class="fa-lg fa fa-bullhorn"></i>
            <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
        </asp:Panel>
    </div>


</asp:Content>
