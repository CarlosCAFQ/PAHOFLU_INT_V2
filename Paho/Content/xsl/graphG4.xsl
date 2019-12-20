<?xml version='1.0' encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" encoding="utf-8"/>

<xsl:template match="/">
	<xsl:apply-templates/>	
</xsl:template>

<xsl:template match="graph">
	<graph>
		<graphTitle><xsl:value-of select="graphTitle"/></graphTitle>
		<graphXAxisTitle><xsl:value-of select="graphXAxisTitle"/></graphXAxisTitle>
		<graphYAxisTitle><xsl:value-of select="graphYAxisTitle"/></graphYAxisTitle>		
		<graphData>
			<xsl:for-each select="//year[1]/graphDataItem/edad">
				<xsl:variable name="etario" select="."/>
				<graphDataItem>
					<edad><xsl:value-of select="."/></edad>
					<serie1><xsl:value-of select="sum(//graph/graphData/year/graphDataItem[edad=$etario]/serie1)"/></serie1>
					<serie2><xsl:value-of select="sum(//graph/graphData/year/graphDataItem[edad=$etario]/serie2)"/></serie2>
					<serie3><xsl:value-of select="sum(//graph/graphData/year/graphDataItem[edad=$etario]/serie3)"/></serie3>
					<serie4><xsl:value-of select="sum(//graph/graphData/year/graphDataItem[edad=$etario]/serie4)"/></serie4>
					<serie5><xsl:value-of select="sum(//graph/graphData/year/graphDataItem[edad=$etario]/serie5)"/></serie5>
					<serie6><xsl:value-of select="sum(//graph/graphData/year/graphDataItem[edad=$etario]/serie6)"/></serie6>					
					<serie7><xsl:value-of select="sum(//graph/graphData/year/graphDataItem[edad=$etario]/serie7)"/></serie7>
					<serie8><xsl:value-of select="sum(//graph/graphData/year/graphDataItem[edad=$etario]/serie8)"/></serie8>
					<serie9><xsl:value-of select="sum(//graph/graphData/year/graphDataItem[edad=$etario]/serie9)"/></serie9>
					<serie10><xsl:value-of select="sum(//graph/graphData/year/graphDataItem[edad=$etario]/serie10)"/></serie10>
				</graphDataItem>
			</xsl:for-each>				
		</graphData>
		<graphSeries1Label><xsl:value-of select="graphSeries1Label"/></graphSeries1Label>
		<graphSeries2Label><xsl:value-of select="graphSeries2Label"/></graphSeries2Label>
		<graphSeries3Label><xsl:value-of select="graphSeries3Label"/></graphSeries3Label>
		<graphSeries4Label><xsl:value-of select="graphSeries4Label"/></graphSeries4Label>
		<graphSeries5Label><xsl:value-of select="graphSeries5Label"/></graphSeries5Label>
		<graphSeries6Label><xsl:value-of select="graphSeries6Label"/></graphSeries6Label>
		<graphSeries7Label><xsl:value-of select="graphSeries7Label"/></graphSeries7Label>
		<graphSeries8Label><xsl:value-of select="graphSeries8Label"/></graphSeries8Label>
		<graphSeries9Label><xsl:value-of select="graphSeries9Label"/></graphSeries9Label>
		<graphSeries10Label><xsl:value-of select="graphSeries10Label"/></graphSeries10Label>
	</graph>
</xsl:template>



</xsl:stylesheet> 

