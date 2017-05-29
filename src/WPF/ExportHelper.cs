using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NBsoft.Appointment.WPF
{
    static class ExportHelper
    {
        public static void ExportUsingRefection(this DataGrid grid, string destinationFile)
        {

            if (grid.ItemsSource == null || grid.Items.Count.Equals(0))
                throw new InvalidOperationException("You cannot export any data from an empty DataGrid.");

            System.IO.FileInfo fi = new System.IO.FileInfo(destinationFile);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fi.FullName, false, Encoding.UTF8))
            {

                string header = "";
                foreach (var column in grid.Columns)
                {
                    string hdr;
                    if (column.Header is string)
                        hdr = column.Header.ToString();
                    else
                        hdr = "";

                    header += string.Format(";{0}", hdr);
                }
                if (header.Length > 0)
                    header = header.Substring(1);
                sw.WriteLine(header);


                ICollectionView collectionView = null;
                IList<DataGridColumn> columns = null;

                columns = grid.Columns.OrderBy(c => c.DisplayIndex).ToList();
                collectionView = CollectionViewSource.GetDefaultView(grid.ItemsSource);


                foreach (object o in collectionView)
                {
                    if (o.Equals(CollectionView.NewItemPlaceholder))
                        continue;

                    string line = "";

                    foreach (DataGridColumn column in columns)
                    {
                        string exportString = ExportBehaviour.GetExportString(column);
                        string fieldValue = "";

                        if (!string.IsNullOrEmpty(exportString))
                        {
                            fieldValue = exportString;
                        }
                        else if (column is DataGridBoundColumn)
                        {
                            string propertyValue = string.Empty;

                            /* Get the property name from the column's binding */
                            BindingBase bb = (column as DataGridBoundColumn).Binding;
                            if (bb != null)
                            {
                                Binding binding = bb as Binding;
                                if (binding != null)
                                {
                                    string boundProperty = binding.Path.Path;

                                    /* Get the property value using reflection */
                                    PropertyInfo pi = o.GetType().GetProperty(boundProperty);
                                    if (pi != null)
                                    {
                                        object value = pi.GetValue(o);
                                        if (value != null)
                                            propertyValue = value.ToString();
                                        else if (column is DataGridCheckBoxColumn)
                                            propertyValue = "-";
                                    }
                                }
                            }
                            fieldValue = propertyValue;
                        }
                        else if (column is DataGridComboBoxColumn)
                        {
                            DataGridComboBoxColumn cmbColumn = column as DataGridComboBoxColumn;
                            string propertyValue = string.Empty;
                            string displayMemberPath = string.Empty;
                            displayMemberPath = cmbColumn.DisplayMemberPath;


                            /* Get the property name from the column's binding */
                            BindingBase bb = cmbColumn.SelectedValueBinding;
                            if (bb != null)
                            {
                                Binding binding = bb as Binding;
                                if (binding != null)
                                {
                                    string boundProperty = binding.Path.Path; //returns "Category" (or CategoryId) 

                                    /* Get the selected property */
                                    PropertyInfo pi = o.GetType().GetProperty(boundProperty);
                                    if (pi != null)
                                    {
                                        object boundProperyValue = pi.GetValue(o); //returns the selected Category object or CategoryId 
                                        if (boundProperyValue != null)
                                        {
                                            Type propertyType = boundProperyValue.GetType();
                                            if (propertyType.IsPrimitive || propertyType.Equals(typeof(string)))
                                            {
                                                if (cmbColumn.ItemsSource != null)
                                                {
                                                    /* Find the Category object in the ItemsSource of the ComboBox with 
                                                     * an Id (SelectedValuePath) equal to the selected CategoryId */
                                                    IEnumerable<object> comboBoxSource = cmbColumn.ItemsSource.Cast<object>();
                                                    object obj = (from oo in comboBoxSource
                                                                  let prop = oo.GetType().GetProperty(cmbColumn.SelectedValuePath)
                                                                  where prop != null && prop.GetValue(oo).Equals(boundProperyValue)
                                                                  select oo).FirstOrDefault();
                                                    if (obj != null)
                                                    {
                                                        /* Get the Name (DisplayMemberPath) of the Category object */
                                                        if (string.IsNullOrEmpty(displayMemberPath))
                                                        {
                                                            propertyValue = obj.GetType().ToString();
                                                        }
                                                        else
                                                        {
                                                            PropertyInfo displayNameProperty = obj.GetType()
                                                                .GetProperty(displayMemberPath);
                                                            if (displayNameProperty != null)
                                                            {
                                                                object displayName = displayNameProperty.GetValue(obj);
                                                                if (displayName != null)
                                                                    propertyValue = displayName.ToString();
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    /* Export the scalar property value of the selected object 
                                                     * specified by the SelectedValuePath property of the DataGridComboBoxColumn */
                                                    propertyValue = boundProperyValue.ToString();
                                                }
                                            }
                                            else if (!string.IsNullOrEmpty(displayMemberPath))
                                            {
                                                /* Get the Name (DisplayMemberPath) property of the selected Category object */
                                                PropertyInfo pi2 = boundProperyValue.GetType()
                                                    .GetProperty(displayMemberPath);

                                                if (pi2 != null)
                                                {
                                                    object displayName = pi2.GetValue(boundProperyValue);
                                                    if (displayName != null)
                                                        propertyValue = displayName.ToString();
                                                }
                                            }
                                            else
                                            {
                                                propertyValue = o.GetType().ToString();
                                            }
                                        }
                                    }
                                }
                            }
                            fieldValue = propertyValue;
                        }
                        if (fieldValue.Contains("\r\n"))
                            fieldValue = fieldValue.Replace("\r\n", " ");

                        line += string.Format(";{0}", fieldValue);
                    }
                    if (line.Length > 0)
                        line = line.Substring(1);
                    sw.WriteLine(line);
                }



            }
        }

        public class ExportBehaviour
        {
            public static readonly DependencyProperty ExportStringProperty =
                DependencyProperty.RegisterAttached("ExportString", //name of attached property 
                typeof(string), //type of attached property 
                typeof(ExportBehaviour), //type of this owner class 
                new PropertyMetadata(string.Empty)); //the default value of the attached property 

            public static string GetExportString(DataGridColumn column)
            {
                return (string)column.GetValue(ExportStringProperty);
            }

            public static void SetExportString(DataGridColumn column, string value)
            {
                column.SetValue(ExportStringProperty, value);
            }
        }
    }
}
