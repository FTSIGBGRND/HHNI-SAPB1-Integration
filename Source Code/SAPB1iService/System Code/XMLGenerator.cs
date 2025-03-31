using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Xml;

namespace FTSISAPB1iService
{
    public class XMLGenerator
    {
        public static void GenerateXMLFile(SAPbobsCOM.BoObjectTypes boObjectTypes, DataSet dataSet, string filePath)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";
                settings.Encoding = Encoding.Unicode;

                using (XmlWriter xmlWriter = XmlWriter.Create(filePath, settings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("BOM");
                    xmlWriter.WriteStartElement("BO");

                    xmlWriter.WriteStartElement("AdmInfo");
                    xmlWriter.WriteStartElement("Object");
                    xmlWriter.WriteString(((int)boObjectTypes).ToString());
                    xmlWriter.WriteEndElement(); //End AdminInfo
                    xmlWriter.WriteEndElement(); //End Object

                    // Add Table Here
                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        xmlWriter.WriteStartElement(dataTable.TableName.ToString());

                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            xmlWriter.WriteStartElement("row");

                            foreach (DataColumn dataColumn in dataTable.Columns)
                            {
                                xmlWriter.WriteStartElement(dataColumn.ColumnName);
                                xmlWriter.WriteString(dataRow[dataColumn.ColumnName].ToString());
                                xmlWriter.WriteEndElement();
                            }

                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteFullEndElement();//End BO
                    xmlWriter.WriteFullEndElement();//End BOM
                    xmlWriter.WriteEndDocument();

                    xmlWriter.Close();
                }
            }
            catch (Exception ex)
            {
                SystemFunction.errorAppend( string.Format("Failed to Generate XML File. {0}. {1}", filePath, ex.Message));
                throw ex;
            }
        }
    }
}
