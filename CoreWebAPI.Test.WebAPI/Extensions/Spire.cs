using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class SpireAttribute : Attribute
{
    private string recordType;      // 记录类型：更新/创建
    private string author;          // 作者
    private DateTime date;          // 更新/创建 日期
    private string memo;         // 备注

    public SpireAttribute()
    {

    }

    // 构造函数，构造函数的参数在特性中也称为“位置参数”。
    public SpireAttribute(string recordType, string author, string date)
    {
        this.recordType = recordType;
        this.author = author;
        this.date = Convert.ToDateTime(date);
    }

    // 对于位置参数，通常只提供get访问器
    public string RecordType
    {
        set { recordType = value; }
        get { return recordType; }
    }
    public string Author { get { return author; } }
    public DateTime Date { get { return date; } }

    // 构建一个属性，在特性中也叫“命名参数”
    public string Memo
    {
        get { return memo; }
        set { memo = value; }
    }
}

