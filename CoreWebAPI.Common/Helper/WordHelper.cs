using System.IO;
using System.Data;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace CoreWebAPI.Common.Helper
{
    public class WordHelper
    {
        public XWPFDocument _document;
        public int _pagesize = 0;
        public int _pos = 0;
        private static WordHelper _wordHelper;

        public WordHelper()
        {
            _document = new XWPFDocument();
        }

        public static WordHelper _
        {
            get => _wordHelper ??= new WordHelper();
            set => _wordHelper = value;
        }


        /// <summary>
        /// 创建word文档中的段落对象和设置段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary>
        /// <param name="document">文档</param>
        /// <param name="content">段落第一个文本对象填充的内容</param>
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="firstLineIndent">首行缩进</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontColor">字体颜色--十六进制</param>
        /// <param name="isBold">是否加粗</param>
        /// <param name="isItalic">是否设置斜体</param>
        /// <returns></returns>
        public static XWPFParagraph ParagraphSetting(XWPFDocument document, string content, ParagraphAlignment paragraphAlign = ParagraphAlignment.LEFT,int firstLineIndent = 0, string fontFamily = "宋体", int fontSize = 12, string fontColor = "000000", bool isBold = false, bool isItalic = false)
        {

            XWPFParagraph paragraph = document.CreateParagraph();//创建段落对象
            paragraph.Alignment = paragraphAlign;//文字显示位置,段落排列（左对齐，居中，右对齐）
            paragraph.FirstLineIndent = firstLineIndent;

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.SetText(content);//填充内容
            xwpfRun.SetFontFamily(fontFamily, FontCharRange.None); //设置标题样式如：（宋体，微软雅黑，隶书，楷体）根据自己的需求而定
            xwpfRun.FontSize = fontSize;//设置文字大小
            xwpfRun.SetColor(fontColor);//设置字体颜色--十六进制
            xwpfRun.IsBold = isBold;//文字加粗
            xwpfRun.IsItalic = isItalic;//是否设置斜体（字体倾斜）

            return paragraph;
        }

        /// <summary>
        /// 创建word文档中的段落对象和设置段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary>
        /// <param name="content">段落第一个文本对象填充的内容</param>
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="firstLineIndent">首行缩进</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontColor">字体颜色--十六进制</param>
        /// <param name="isBold">是否加粗</param>
        /// <param name="isItalic">是否设置斜体</param>
        /// <returns></returns>
        public void WriteText(string content, ParagraphAlignment paragraphAlign = ParagraphAlignment.LEFT, int firstLineIndent = 0, string fontFamily = "宋体", int fontSize = 12, string fontColor = "000000", bool isBold = false, bool isItalic = false)
        {
            _document.SetParagraph(ParagraphSetting(_document, content, paragraphAlign, firstLineIndent, fontFamily, fontSize, fontColor, isBold, isItalic), _pos);
            _pos++;
        }

        /// <summary>
        /// 创建word文档中的段落对象和设置段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary>
        /// <param name="dt">段落第一个文本对象填充的内容</param>
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="firstLineIndent">首行缩进</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontColor">字体颜色--十六进制</param>
        /// <param name="isBold">是否加粗</param>
        /// <param name="isItalic">是否设置斜体</param>
        /// <returns></returns>
        public void WriteTable(DataTable dt, ParagraphAlignment paragraphAlign = ParagraphAlignment.LEFT, int firstLineIndent = 0, string fontFamily = "宋体", int fontSize = 12, string fontColor = "000000", bool isBold = false, bool isItalic = false)
        {
            if (dt == null)
                return;
            
            //创建文档中的表格对象实例
            XWPFTable table = _document.CreateTable(dt.Rows.Count + 1, dt.Columns.Count);//显示的行列数
            table.SetCellMargins(0,2,0,0);
            table.Width = 2200;//总宽度
            for(int i = 0; i < dt.Columns.Count; i++)
            {
                table.SetColumnWidth(i, (ulong)(2200 / dt.Columns.Count));
                table.GetRow(0).GetCell(i).SetParagraph(TableParagraphSetting(table, dt.Columns[i].ColumnName.GetCString(), paragraphAlign, firstLineIndent, fontFamily, fontSize, "FFFFFF", isBold:true, isItalic));
                table.GetRow(0).GetCell(i).SetColor("#0099cc");

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    table.GetRow(j + 1).GetCell(i).SetParagraph(TableParagraphSetting(table, dt.Rows[j][i].GetCString(), paragraphAlign, firstLineIndent, fontFamily, fontSize, fontColor, isBold, isItalic));
                }
            }
        }

        /// <summary> 
        /// 创建Word文档中表格段落实例和设置表格段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary> 
        /// <param name="table">表格对象</param> 
        /// <param name="content">要填充的文字</param> 
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="firstLineIndent">首行缩进</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="isBold">是否加粗（true加粗，false不加粗）</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontColor">字体颜色--十六进制</param>
        /// <param name="isItalic">是否设置斜体（字体倾斜）</param>
        /// <returns></returns> 
        public static XWPFParagraph TableParagraphSetting(XWPFTable table, string content, ParagraphAlignment paragraphAlign = ParagraphAlignment.LEFT, int firstLineIndent = 0, string fontFamily = "宋体", int fontSize = 12, string fontColor = "000000", bool isBold = false, bool isItalic = false)
        {
            var para = new CT_P();
            //设置单元格文本对齐
            para.AddNewPPr().AddNewTextAlignment();

            //创建表格中的段落对象
            XWPFParagraph paragraph = new XWPFParagraph(para, table.Body)
            {
                //文字显示位置,段落排列（左对齐，居中，右对齐）
                //paragraph.FontAlignment =Convert.ToInt32(ParagraphAlignment.CENTER);//字体在单元格内显示位置与 paragraph.Alignment效果相似
                Alignment = paragraphAlign,
                FirstLineIndent = firstLineIndent
            };

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.SetText(content);
            xwpfRun.SetFontFamily(fontFamily, FontCharRange.None);//设置字体（如：微软雅黑,华文楷体,宋体）
            xwpfRun.FontSize = fontSize;//字体大小
            xwpfRun.SetColor(fontColor);//设置字体颜色--十六进制
            xwpfRun.IsItalic = isItalic;//是否设置斜体（字体倾斜）
            xwpfRun.IsBold = isBold;//是否加粗
            //xwpfRun.SetTextPosition(textPosition);//设置文本位置（设置两行之间的行间），从而实现table的高度设置效果 

            return paragraph;
        }
    
        public void Write(FileStream stream)
        {
            _document.Write(stream);
        }
    }

}
