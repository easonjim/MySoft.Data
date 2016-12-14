# MySoft.Data
dotnet ORM
##版权
这里版权属于老毛：http://www.cnblogs.com/maoyong
##说明
MySoft体系中的ORM组件，这里的版本为2.7.3，在公司内部企业项目中历练了几年，修复了一些bug，所以直接在这里开源进行维护。  
但是要注意：组件的全部版权属于老毛。  
在这里组件的基础上，又封装了一层单例层，再配合这个单例层的代码生成器，能同时生成实体和针对这个实体的增删改查方法。  
##单组件的使用教程
参考老毛的使用教程：http://www.cnblogs.com/maoyong/archive/2010/04/13/1710879.html
##此版本的使用方法
由于采用了单例进行封装，通过实体代码生成器生成有两个文件：实体和实体对应的单例业务（比如要生成sys_Area这个表的，会生成sys_Area.cs、sys_AreaService）。  
sys_Area.cs是常规的表映射。  
sys_AreaService.cs如下：
