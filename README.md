# MySoft.Data 2.7.3
dotnet ORM
##版权
这里版权属于老毛：[http://www.cnblogs.com/maoyong](http://www.cnblogs.com/maoyong)
##说明
MySoft体系中的ORM组件，这里的版本为2.7.3，在公司内部企业项目中历练了几年，修复了一些bug，所以直接在这里开源进行维护。  
但是要注意：组件的全部版权属于老毛。  
在这个组件的基础上，又封装了一层单例层，再配合这个单例层的代码生成器，能同时生成实体和针对这个实体的增删改查方法。  
##单组件的使用教程
参考老毛的使用教程：[http://www.cnblogs.com/maoyong/archive/2010/04/13/1710879.html](http://www.cnblogs.com/maoyong/archive/2010/04/13/1710879.html)
##此版本的使用方法
由于采用了单例进行封装，通过实体代码生成器生成有两个文件：实体和实体对应的单例业务（比如要生成sys_Area这个表的，会生成sys_Area.cs、sys_AreaService）。  
sys_Area.cs是常规的表映射。  
sys_AreaService.cs如下：  
```C#  
public class sys_AreaService : BaseDao<sys_Area>
{
    #region "单例"
    private static sys_AreaService service;
    public static sys_AreaService Instance
    {
        get
        {
            if (service == null)
            {
                service = new sys_AreaService();
            }
            return service;
        }
    }
    #endregion
}
```  
BaseDao类为针对单表的增删查改的封装，泛型传入的是针对这张表对应的实体。  
每次进行使用时，直接调用单例即可，这里如果要使用添加的方法如下：  
```C#
//初始化要增加的实体
var area = new sys_Area()
    {
        A_Name = "test"
    };
//调用Add_Entity方法
if (sys_AreaService.Instance.Add_Entity(area))
{
	//成功后会返回主键自增ID
    var areaid = area.Areaid;
}
```  
SQL Server连接字符串例子：  
```SQL
<add name="ConnectionString" connectionString="server=192.168.199.1;database=DataBaseName;uid=sa;pwd=123456;" providerName="MySoft.Data.SqlServer9.SqlServer9Provider" />
```  
说明：是使用了MySoft.Data组件的驱动思想，采用SqlServer9的驱动，最明显区别在于生成的分页将更高效。
##使用技巧  
1、针对中小型业务系统，推荐采用自增列ID的方式，且此版本的组件对非自增列的支持不太强。  
2、使用过程中，数据库主要是SQL Server为主。同时也支持Oracle、MySql、Access这些。