# LocalPBackEnd

ver esta clase y levantar dependencia Aspoce.Total.lic de localhost
 public class ExcelService
        {
            public ExcelService()
            {
                Aspose.Cells.License cellsLicense = new Aspose.Cells.License();
                //alex esta lib la levantamos de localhost  cellsLicense.SetLicense("Aspose.Total.lic");
                cellsLicense.SetLicense("C:\\Users\\alex\\Desktop\\git repo 2\\localpayment\\BackgroundSide\\Resources\\Libs\\Aspose.Total.lic");
            }
