<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganisationTree.ascx.cs" Inherits="NXJC.UI.Web.Common.OrganisationTree" %>

<ul id="organisationTree" class="easyui-tree"></ul>

<script>
    $(document).ready(function () {
        loadOrganisationTree();
    });

    function loadOrganisationTree() {
        var queryUrl = '/Common/OrganisationService.asmx/GetTree';

        $.ajax({
            type: "POST",
            url: queryUrl,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                initializeOrganisationTree(jQuery.parseJSON(msg.d));
            },
            error: function () {
                $.messager.alert('错误', '数据载入失败！');
            }
        });
    }

    // 初始化组织结构树
    function initializeOrganisationTree(jsonData) {
        $('#organisationTree').tree({
            data: jsonData,
            animate: true,
            lines: true,
            onClick: function (node) {
                if (typeof (onOrganisationTreeClick) == "function") {
                    onOrganisationTreeClick(node);
                }
            }
        });
    }
</script>