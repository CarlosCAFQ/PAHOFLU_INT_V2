<?xml version='1.0' encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" encoding="utf-8"/>

<xsl:template match="/">
	<xsl:apply-templates/>	
</xsl:template>

<xsl:template match="graph">
	<graph>
		<graphTitle><xsl:value-of select="graphTitle"/></graphTitle>
		<graphHeaders>
			<header><xsl:value-of select="//year[1]/graphHeaders/header[1]/@name"/></header>
			<header><xsl:value-of select="//year[1]/graphHeaders/header[2]"/></header>
			<header><xsl:value-of select="//year[1]/graphHeaders/header[3]"/></header>
			<header><xsl:value-of select="//year[1]/graphHeaders/header[4]"/></header>
		</graphHeaders>
		<subGraphHeaders>
			<subheader><xsl:value-of select="//year[1]/subGraphHeaders/subheader[1]"/></subheader>
			<subheader><xsl:value-of select="concat(//year[1]/subGraphHeaders/subheader[2]/@name,'(',sum(//year/subGraphHeaders/subheader[2]),')')"/></subheader>
			<subheader><xsl:value-of select="//year[1]/subGraphHeaders/subheader[3]/@name"/></subheader>
			<subheader><xsl:value-of select="concat(//year[1]/subGraphHeaders/subheader[4]/@name,'(',sum(//year/subGraphHeaders/subheader[4]),')')"/></subheader>
			<subheader><xsl:value-of select="//year[1]/subGraphHeaders/subheader[5]/@name"/></subheader>
			<subheader><xsl:value-of select="concat(//year[1]/subGraphHeaders/subheader[6]/@name,'(',sum(//year/subGraphHeaders/subheader[6]),')')"/></subheader>
			<subheader><xsl:value-of select="//year[1]/subGraphHeaders/subheader[7]/@name"/></subheader>
		</subGraphHeaders>
		<xsl:for-each select="//year[1]/row">
			<xsl:variable name="numeroRow" select="./@r_no"/>
			<row>
				<col><xsl:value-of select="col[1]"/></col>
				<col><xsl:value-of select="sum(//row[@r_no=$numeroRow]/col[2])"/></col>
				<col><xsl:value-of select="concat(format-number(sum(//row[@r_no=$numeroRow]/col[2]) div sum(//year/subGraphHeaders/subheader[2])*100,'0.##'),'%')"/></col>
				<col><xsl:value-of select="sum(//row[@r_no=$numeroRow]/col[4])"/></col>
				<col><xsl:value-of select="concat(format-number(sum(//row[@r_no=$numeroRow]/col[4]) div sum(//year/subGraphHeaders/subheader[4])*100,'0.##'),'%')"/></col>
				<col><xsl:value-of select="sum(//row[@r_no=$numeroRow]/col[6])"/></col>
				<col><xsl:value-of select="concat(format-number(sum(//row[@r_no=$numeroRow]/col[6]) div sum(//year/subGraphHeaders/subheader[6])*100,'0.##'),'%')"/></col>
			</row>
		</xsl:for-each>
		<!--graphData>
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
		</graphData-->		
	</graph>
</xsl:template>



</xsl:stylesheet> 

