using System;
using System.Linq;
using System.Drawing;
using BarcodeLib;
using BarcodeStandard;

namespace Tools.BarCode
{
    public static class BarCode
    {
        public static string BarCodeGenerator(string barcodenumber)
        {
            try
            {
                Barcode oBarCode = new Barcode();
                oBarCode.IncludeLabel = true;
                oBarCode.LabelPosition = LabelPositions.BOTTOMCENTER;
                oBarCode.LabelFont = new Font("ARIAL", 8);

                Image ImgBarCode = oBarCode.Encode(TYPE.Interleaved2of5, barcodenumber, Color.Black, Color.White, 360, 100);

                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(ImgBarCode, typeof(byte[])); ;
                string ImgToBase64 = Convert.ToBase64String(xByte);

                return ImgToBase64;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}
