<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <GMS:WebPart runat="server" ID="WebPart1" DataSourceType="LocalXml" DataSource="/localData/Person_List.xml" OutputRenderType="Xslt" OutputRenderFile="/webpart/Person_List.xslt"/>     
</body>
</html>
