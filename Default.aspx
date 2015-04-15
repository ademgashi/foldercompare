<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FolderCompare._Default" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .exists_on_staging {
            background-color: #93b45c !important;
            font-size: 12px;
        }

        .exists_on_test {
            background-color: #c25a51 !important;
            font-size: 12px;
        }

        .itisdifferent {
            background-color: #b5d7fd !important;
            font-size: 12px;
        }
    </style>
    <script type="text/javascript" src="Scripts/radtreescript.js"></script>
   <div class="row">
        <p></p>
    </div>

    <div class="row">
        <asp:Panel ID="InfoPanel" runat="server" class="alert alert-success alert-dismissable" Visible="False">
            <button type="button" class="close" data-dismiss="alert" aria-hidden="true"></button>
            <i class="fa-lg fa fa-bullhorn"></i>
            <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
        </asp:Panel>
    </div>

    <div class="row">
        <ul style="margin-top: 0; list-style: none">
            <li style="float: left; margin-right: 30px;"><span style="width: 20px; margin-right: 5px; background: #93b45c; display: block; border: solid 1px #5c5c5c; float: left">&nbsp;</span>
                Exist on the staging folder
            </li>
            <li style="float: left; margin-right: 30px;">
                <span style="width: 20px; margin-right: 5px; background: #c25a51; display: block; border: solid 1px #5c5c5c; float: left">&nbsp;</span>
                Exists on the testing folder
            </li>
            <li>
                <span style="width: 20px; background: #b5d7fd; display: block; margin-right: 5px; border: solid 1px #5c5c5c; float: left">&nbsp;</span>
                Exists on both folders but is different
            </li>
        </ul>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Staging - Folder Path</h2>
            <p>
                <asp:TextBox ID="txtStagingPath" Width="100%" Text="C:\code\mana-api-library" placeholder="Write staging folder path" runat="server"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                    runat="server" ControlToValidate="txtStagingPath" ErrorMessage="Please enter a path"></asp:RequiredFieldValidator>
            </p>
            <p>
                <telerik:RadTreeView ID="treeStagingPath" runat="server" OnContextMenuItemClick="StagingTreeView_ContextMenuItemClick"
                    OnClientContextMenuItemClicking="onClientContextMenuItemClicking" OnClientContextMenuShowing="onClientContextMenuShowingStaging">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="RadTreeViewContextMenu1" runat="server">
                            <Items>
                                <telerik:RadMenuItem Value="Copy" Text="Copy ..." ImageUrl="~/images/copy.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Delete" Text="Delete" ImageUrl="~/images/delete.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem IsSeparator="true">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="ViewDiff" ImageUrl="~/images/diff.png" Text="View differences">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>

                    </ContextMenus>

                </telerik:RadTreeView>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Test - Folder Path</h2>
            <p>
                <asp:TextBox ID="txtTestPath" Width="100%" Text="C:\code\mana-api-library - Copy" placeholder="Write test folder path" runat="server"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                    runat="server" ControlToValidate="txtTestPath" ErrorMessage="Please enter a path"></asp:RequiredFieldValidator>

            </p>
            <p>
                <telerik:RadTreeView ID="treeTestPath" runat="server" OnContextMenuItemClick="TestTreeView_ContextMenuItemClick"
                    OnClientContextMenuItemClicking="onClientContextMenuItemClicking" OnClientContextMenuShowing="onClientContextMenuShowing">
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu ID="MainContextMenu" runat="server">
                            <Items>
                                <telerik:RadMenuItem Value="Copy" Text="Copy ..." ImageUrl="~/images/copy.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Delete" Text="Delete" ImageUrl="~/images/delete.gif">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem IsSeparator="true">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="ViewDiff" ImageUrl="~/images/diff.png" Text="View differences">
                                </telerik:RadMenuItem>
                            </Items>
                            <CollapseAnimation Type="none"></CollapseAnimation>
                        </telerik:RadTreeViewContextMenu>

                    </ContextMenus>

                </telerik:RadTreeView>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Production - Folder Path</h2>
            <p>
                <asp:TextBox ID="txtProductionPath" Width="100%" placeholder="Write production folder path" runat="server"></asp:TextBox>
                <%--                                                <br/><asp:RequiredFieldValidator ID="RequiredFieldValidator3" 
                    runat="server" ControlToValidate="txtProductionPath" ErrorMessage="Please enter a path"></asp:RequiredFieldValidator>--%>
            </p>

        </div>

    </div>
    <div class="row">
        <div class="col-md-4">
            <asp:Button ID="btnStartCompare" OnClick="btnStartCompare_OnClick" class="btn btn-success" runat="server" Text="Start Comparing" />
        </div>
    </div>
    <br />
    <div class="row">
        <div class="col-md-12">
            <asp:GridView ID="grdChanges" AutoGenerateColumns="True" runat="server"></asp:GridView>
        </div>

    </div>
    <div class="row">
        <div class="col-md-12">
            <asp:TextBox ID="txtOutPut" runat="server" Height="200px" Width="100%" TextMode="MultiLine"></asp:TextBox>
        </div>

    </div>
    <script type="text/javascript">
        $(document).ready(function ($) {

            $(document).find('.exists_on_staging').closest('.rtLI').addClass('exists_on_staging');
            $(document).find('.exists_on_test').closest('.rtLI').addClass('exists_on_test');
            $(document).find('.itisdifferent').closest('.rtLI').addClass('itisdifferent');


        });
    </script>

</asp:Content>
