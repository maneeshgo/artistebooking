<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
      exclude-result-prefixes="msxsl">
  
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/">
      <div class="navbar navbar-default navbar-static-top navbar-inverse">
        <div class="container">
          <ul class="nav navbar-nav">
            <li>
              <a href="#">Home</a>
            </li>
            <li>
              <a href="#">About</a>
            </li>
            <li>
              <a href="#">Contact</a>
            </li>
          </ul>
        </div>
      </div>

      <div class="jumbotron">
        <div class="container">
          <div class="row">
            <div class="col-xs-6 col-md-3">
              <img class="img-circle" src="http://images.mid-day.com/images/2014/jan/20-Emraan-Hashmi.jpg" width="200" height="200" alt="Manish Gupta"/>
            </div>
            <div class="col-xs-6 col-md-9">
              <h1>
                <xsl:value-of select="/Root/Name"/>
              </h1>
              
              <h2>
                <xsl:value-of select="/Root/Role"/>
              </h2>
              <p>
                <xsl:value-of select="/Root/Technology"/>
              </p>
            </div>
          </div>
        </div>
      </div>

      <div class="container">
        <ul class="nav nav-tabs">
          <li role="presentation" class="active">
            <a href="#Team"  aria-controls="Team" role="tab" data-toggle="tab">Team</a>
          </li>
          <li role="presentation">
            <a a="" href="#Message"  aria-controls="Message" role="tab" data-toggle="tab">Message</a>
          </li>
          <li role="presentation">
            <a href="#Project"  aria-controls="Project" role="tab" data-toggle="tab">Project</a>
          </li>
        </ul>
      </div>

      <div class="container">
        <div class="tab-content">
          <div role="tabpanel" class="tab-pane fade in active" id="Team">
            <div class="panel panel-default">
              <div class="panel-body">
                <table>
                  <tr>
                    <td>Name</td>
                    <td>Age</td>
                  </tr>
                  <xsl:for-each select="/Root/Person/item">
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
              </div>
            </div>
          </div>
          <div role="tabpanel" class="tab-pane fade" id="Message">
            <div class="panel panel-default">
              <div class="panel-body">
                <xsl:value-of select="/Root/Message"/>
              </div>
            </div>
          </div>
          <div role="tabpanel" class="tab-pane fade" id="Project">
            <div class="panel panel-default">
              <div class="panel-body">
                <ul class="list-group">
                  <xsl:for-each select="/Root/Project/item">
                    <li class="list-group-item">
                      <xsl:value-of select="Name"/>
                    </li>
                  </xsl:for-each>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </div>
      
    </xsl:template>
</xsl:stylesheet>
