<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
      exclude-result-prefixes="msxsl">
  
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/">
      <table>
        <tr>
          <td>Name</td>
          <td>Age</td>
        </tr>
        <xsl:for-each select="/Root/List/item">
          <tr>
            <td>
              <xsl:value-of select="PersonName"/>
            </td>
            <td>
              <xsl:value-of select="Age"/>
            </td>
          </tr>
        </xsl:for-each>

      </table>
    </xsl:template>
</xsl:stylesheet>
