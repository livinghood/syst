using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.General_Logic
{
    public class PrintLogic
    {
        /// <summary>
        /// Export a list of non-budgeted products to a textfile
        /// </summary>
        /// <param name="path"></param>
        /// <param name="listOfObjects"></param>
        /// <returns></returns>
        public bool ExportNonBudgetedProductsToTextFile(string path, IEnumerable<Product> listOfObjects)
        {
            try
            {
                // StreamWriter is put in 'using' statement for automatic disposal once finished
                using (var writer = new StreamWriter(path))
                {
                    // First line in textfile makes a header
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}\t", "Produkt ID", "Produktbenämning", "Produktgrupp", "Produktkategori");

                    foreach (var row in listOfObjects)
                    {
                        writer.WriteLine("{0}\t{1}\t{2}\t{3}\t", row.ProductID, row.ProductName, row.ProductGroup, row.ProductCategory);
                    }
                }  
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }
    }
}
